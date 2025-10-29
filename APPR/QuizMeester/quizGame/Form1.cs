using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Media;               // 用于播放特殊题提示音
using System.Windows.Forms;

namespace quizGame
{
    public partial class Form1 : Form
    {
        // === 游戏状态 ===
        int correctAnswer = 0;
        int correctAnswers = 0;
        int wrongAnswers = 0;
        int questionNumber = 1;
        int score;
        int percentage;
        int totalQuestions;
        int currentQuestionNumber = 1;

        // === 题目顺序 ===
        readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;
        readonly List<int> questionOrder = new List<int>();

        // === 计时系统 ===
        private Timer _questionTimer;
        private Timer _quizTimer;

        // 每题剩余时间（秒）
        private int _qTimeLeft = 0;

        // 整场剩余时间（秒）
        private int _quizTimeLeft = 180;

        // 整场计时是否已启动（只在第一题启动一次）
        private bool _quizStarted = false;

        // 时间提醒配色
        private readonly Color _timeNormal = Color.White;
        private readonly Color _timeWarn = Color.OrangeRed;

        // === 跳过功能（只允许一次） ===
        private bool _hasSkipped = false;

        // === 50/50 功能 ===
        private bool _used5050 = false;  // 是否已经使用过

        // === 玩家名（用于保存到 scores.username_snapshot）===
        private string _playerName = "Gast"; // 默认访客名（你是强制登录，这里只是兜底）

        // === Special Question（特殊题） ===
        private readonly Random _rand = new Random();
        private int _specialQuestionIndex = -1;   // 本次游戏的特殊题是第几题（1-based），位于前20题内
        private const int _specialBonus = 2;      // 答对特殊题的额外加分（在原本+1基础上再+2）
        private bool _isSpecialNow = false;       // 当前是否处于特殊题

        // 记录原始配色，便于恢复
        private Color _origFormBackColor;
        private Color _origQuestionForeColor;
        private Color _origQuestionBackColor;

        public Form1()
        {
            InitializeComponent();

            // 初始化计时器
            _questionTimer = new Timer { Interval = 1000 };
            _questionTimer.Tick += QuestionTimer_Tick;

            _quizTimer = new Timer { Interval = 1000 };
            _quizTimer.Tick += QuizTimer_Tick;

            if (lblQuestionTime != null)
            {
                lblQuestionTime.Text = "Vraag tijd: --s";
                lblQuestionTime.ForeColor = _timeNormal;
            }
            if (lblQuizTime != null)
            {
                lblQuizTime.Text = "Quiz tijd: --:--";
                lblQuizTime.ForeColor = _timeNormal;
            }

            // 开局跳过按钮可用
            if (btnSkip != null) btnSkip.Enabled = true;
            if (btn5050 != null) btn5050.Enabled = true;
            // 构建题序并开始
            BuildQuestionOrder();

            totalQuestions = questionOrder.Count;
            if (totalQuestions == 0)
            {
                MessageBox.Show("Er staan nog geen vragen in de database.", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            // 记录原始配色
            _origFormBackColor = this.BackColor;
            _origQuestionForeColor = lblQuestion.ForeColor;
            _origQuestionBackColor = lblQuestion.BackColor;

            // 生成“特殊题”位置：随机 1..min(20, totalQuestions)
            int specialMax = Math.Min(20, totalQuestions);
            if (specialMax >= 1)
            {
                _specialQuestionIndex = _rand.Next(1, specialMax + 1);
            }

            // 从登录态设置玩家名（你项目是强制登录，这里会拿到用户名；兜底保留“Gast”）
            if (Auth.CurrentUser != null && !string.IsNullOrWhiteSpace(Auth.CurrentUser.Username))
                _playerName = Auth.CurrentUser.Username.Trim();

            askQuestion(questionNumber);
        }

        /// <summary>生成本次测验题序（最多 20 题）</summary>
        private void BuildQuestionOrder()
        {
            int limit = 20;
            using (var con = new SqlConnection(_cs))
            {
                con.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT TOP (@n) id
                    FROM questions
                    ORDER BY NEWID();", con))
                {
                    cmd.Parameters.AddWithValue("@n", limit);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            questionOrder.Add(r.GetInt32(0));
                        }
                    }
                }
            }
        }

        /// <summary>点击答案（所有答案按钮共用）</summary>
        private void ClickAnswerEvent(object sender, EventArgs e)
        {
            // 停止本题计时，避免多减
            _questionTimer.Stop();

            var senderObject = (Button)sender;
            int buttonTag = Convert.ToInt32(senderObject.Tag);

            if (buttonTag == correctAnswer)
            {
                score++;                         // 普通题 +1
                if (_isSpecialNow) score += _specialBonus;  // 特殊题额外 +2
                correctAnswers++;
                currentQuestionNumber++;
                senderObject.BackColor = Color.DarkGreen;
                correctAnswersLabel.Text = "Goede antwoorden: " + correctAnswers;
            }
            else
            {
                wrongAnswers++;
                currentQuestionNumber++;
                senderObject.BackColor = Color.DarkRed;
                WrongAnswersLabel.Text = "Foute antwoorden: " + wrongAnswers;
            }

            // 禁用四个按钮
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            // 最后一题？→ 统一收尾
            if (questionNumber == totalQuestions)
            {
                FinishQuiz();
            }
            else
            {
                questionNumber++;
                askQuestion(questionNumber);
            }
        }

        /// <summary>出题 + 启动计时</summary>
        private void askQuestion(int qnum)
        {
            if (qnum < 1 || qnum > questionOrder.Count) return;

            currentQuestion.Text = "Vraag nummer: " + qnum;

            int qid = questionOrder[qnum - 1];

            string questionText = "";
            int perQuestionSeconds = 15; // 默认单题 15 秒
            var answers = new List<Tuple<string, bool>>();

            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                // 题干 + 单题时长
                using (var q = new SqlCommand(
                    @"SELECT question_text, per_question_seconds
                      FROM questions WHERE id=@id;", con))
                {
                    q.Parameters.AddWithValue("@id", qid);
                    using (var r = q.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            questionText = r.GetString(0);
                            perQuestionSeconds = !r.IsDBNull(1) ? r.GetInt32(1) : 15;
                        }
                        else
                        {
                            MessageBox.Show("Vraag niet gevonden (id=" + qid + ").", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                // 四个选项（随机）
                using (var a = new SqlCommand(
                    @"SELECT answer_text, is_correct
                      FROM answers
                      WHERE question_id=@id
                      ORDER BY NEWID();", con))
                {
                    a.Parameters.AddWithValue("@id", qid);
                    using (var ra = a.ExecuteReader())
                    {
                        while (ra.Read())
                        {
                            string text = ra.GetString(0);
                            bool isCorrect = ra.GetBoolean(1);
                            answers.Add(Tuple.Create(text, isCorrect));
                        }
                    }
                }
            }

            if (answers.Count != 4)
            {
                MessageBox.Show("Deze vraag heeft niet precies 4 opties. (id=" + qid + ")", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            pictureBox1.Image = null;
            lblQuestion.Text = questionText;

            var btns = new[] { button1, button2, button3, button4 };
            correctAnswer = 0;

            for (int i = 0; i < 4; i++)
            {
                btns[i].Text = answers[i].Item1;
                btns[i].BackColor = Color.DarkCyan;
                btns[i].Enabled = true;

                int choiceIndex = i + 1; // 1..4
                if (answers[i].Item2)
                {
                    correctAnswer = choiceIndex;
                }
            }

            if (correctAnswer == 0)
            {
                MessageBox.Show("Antwoord sleutel ontbreekt voor vraag (id=" + qid + ").", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // —— Special Question 标记/样式/音效 —— 
            _isSpecialNow = (qnum == _specialQuestionIndex);
            if (_isSpecialNow)
            {
                // 显眼的样式（按需自定义）
                this.BackColor = Color.MediumPurple;
                lblQuestion.ForeColor = Color.Yellow;
                lblQuestion.BackColor = Color.FromArgb(32, 0, 0, 0);

                try
                {
                    // 若有自带 wav，可改为 new SoundPlayer("special.wav").Play();
                    SystemSounds.Asterisk.Play();
                }
                catch { /* 忽略声音异常 */ }
            }
            else
            {
                RestoreNormalTheme();
            }

            // === 启动“每题倒计时” ===
            _qTimeLeft = perQuestionSeconds > 0 ? perQuestionSeconds : 15;
            if (lblQuestionTime != null)
            {
                lblQuestionTime.ForeColor = _timeNormal;
                lblQuestionTime.Text = $"Vraag tijd: {_qTimeLeft}s";
            }
            _questionTimer.Stop();
            _questionTimer.Start();

            // === 启动“整场倒计时”（仅第一次） ===
            if (!_quizStarted)
            {
                _quizStarted = true;
                if (lblQuizTime != null)
                {
                    lblQuizTime.Text = FormatQuizTime(_quizTimeLeft);
                    lblQuizTime.ForeColor = _timeNormal;
                }
                _quizTimer.Start();
            }

            // === 跳过按钮：没用过就启用，用过就禁用 ===
            if (btnSkip != null) btnSkip.Enabled = !_hasSkipped;

            // 50/50 按钮每题都可使用一次（也可以改成整场一次）
            if (btn5050 != null) btn5050.Enabled = !_used5050;

        }

        private void QuestionTimer_Tick(object sender, EventArgs e)
        {
            if (_qTimeLeft > 0)
            {
                _qTimeLeft--;

                if (lblQuestionTime != null)
                {
                    lblQuestionTime.Text = $"Vraag tijd: {_qTimeLeft}s";
                    if (_qTimeLeft <= 5) lblQuestionTime.ForeColor = _timeWarn;
                }

                if (_qTimeLeft == 0)
                {
                    _questionTimer.Stop();

                    // 超时按错误处理
                    wrongAnswers++;
                    WrongAnswersLabel.Text = "Foute antwoorden: " + wrongAnswers;

                    button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = false;

                    if (questionNumber == totalQuestions)
                    {
                        FinishQuiz();
                    }
                    else
                    {
                        questionNumber++;
                        askQuestion(questionNumber);
                    }
                }
            }
        }

        private void QuizTimer_Tick(object sender, EventArgs e)
        {
            if (_quizTimeLeft > 0)
            {
                _quizTimeLeft--;

                if (lblQuizTime != null)
                {
                    lblQuizTime.Text = FormatQuizTime(_quizTimeLeft);
                    if (_quizTimeLeft <= 10) lblQuizTime.ForeColor = _timeWarn;
                }

                if (_quizTimeLeft == 0)
                {
                    // 整场时间结束
                    _questionTimer.Stop();
                    _quizTimer.Stop();
                    FinishQuiz();
                }
            }
        }

        private string FormatQuizTime(int totalSeconds)
        {
            int m = totalSeconds / 60;
            int s = totalSeconds % 60;
            return $"Quiz tijd: {m:00}:{s:00}";
        }

        /// <summary>统一收尾（最后一题 / 单题超时 / 总时到 都调这里）</summary>
        private void FinishQuiz()
        {
            _questionTimer.Stop();
            _quizTimer.Stop();

            // 恢复普通主题
            RestoreNormalTheme();

            percentage = (int)Math.Round((double)(100 * score) / Math.Max(1, totalQuestions));

            // 先保存分数到数据库
            SaveScore(_playerName, score);

            var dialogResult = MessageBox.Show(
                "De quiz is afgelopen" + Environment.NewLine + Environment.NewLine +
                "Je hebt " + score + " vragen goed beantwoord" + Environment.NewLine +
                "Je totale percentage is " + percentage + "%" + Environment.NewLine +
                "Wil je opnieuw spelen?",
                "Resultaten",
                MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                // 重置所有状态
                score = 0;
                correctAnswer = 0;
                correctAnswers = 0;
                wrongAnswers = 0;
                questionNumber = 1;

                questionOrder.Clear();
                BuildQuestionOrder();
                totalQuestions = questionOrder.Count;

                correctAnswersLabel.Text = "Goede antwoorden: 0";
                WrongAnswersLabel.Text = "Foute antwoorden: 0";

                // 重置计时
                _quizStarted = false;
                _quizTimeLeft = 180;
                if (lblQuizTime != null)
                {
                    lblQuizTime.Text = FormatQuizTime(_quizTimeLeft);
                    lblQuizTime.ForeColor = _timeNormal;
                }
                if (lblQuestionTime != null)
                {
                    lblQuestionTime.Text = "Vraag tijd: --s";
                    lblQuestionTime.ForeColor = _timeNormal;
                }

                // 重置“跳过一次”
                _hasSkipped = false;
                if (btnSkip != null) btnSkip.Enabled = true;

                // 重新生成“特殊题”位置（下一局重新随机）
                int specialMax = Math.Min(20, totalQuestions);
                _specialQuestionIndex = specialMax >= 1 ? _rand.Next(1, specialMax + 1) : -1;

                askQuestion(questionNumber);
            }
            else
            {
                this.Close();
            }
        }

        private void RestoreNormalTheme()
        {
            this.BackColor = _origFormBackColor;
            lblQuestion.ForeColor = _origQuestionForeColor;
            lblQuestion.BackColor = _origQuestionBackColor;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TotalQuestionsLabel.Text = "Aantal vragen: " + totalQuestions;
            correctAnswersLabel.Text = "Goede antwoorden: 0";
            WrongAnswersLabel.Text = "Foute antwoorden: 0";
        }

        private void FullScreenClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void ExitFullScreenClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void finishClick(object sender, EventArgs e)
        {
            _questionTimer?.Stop();
            _quizTimer?.Stop();
            this.Close();
        }

        private void playAgainClick(object sender, EventArgs e)
        {
            if (questionNumber > 1)
            {
                _questionTimer.Stop();
                _quizTimer.Stop();

                score = 0;
                correctAnswer = 0;
                correctAnswers = 0;
                wrongAnswers = 0;
                questionNumber = 1;

                questionOrder.Clear();
                BuildQuestionOrder();
                totalQuestions = questionOrder.Count;

                correctAnswersLabel.Text = "Goede antwoorden: 0";
                WrongAnswersLabel.Text = "Foute antwoorden: 0";

                _quizStarted = false;
                _quizTimeLeft = 180;
                if (lblQuizTime != null)
                {
                    lblQuizTime.Text = FormatQuizTime(_quizTimeLeft);
                    lblQuizTime.ForeColor = _timeNormal;
                }
                if (lblQuestionTime != null)
                {
                    lblQuestionTime.Text = "Vraag tijd: --s";
                    lblQuestionTime.ForeColor = _timeNormal;
                }

                _hasSkipped = false;
                if (btnSkip != null) btnSkip.Enabled = true;

                // 重新生成“特殊题”位置
                int specialMax = Math.Min(20, totalQuestions);
                _specialQuestionIndex = specialMax >= 1 ? _rand.Next(1, specialMax + 1) : -1;

                askQuestion(questionNumber);
            }
        }

        // 跳过一次按钮
        private void btnSkip_Click(object sender, EventArgs e)
        {
            if (_hasSkipped)
            {
                btnSkip.Enabled = false;
                MessageBox.Show("Je hebt al een vraag overgeslagen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _hasSkipped = true;
            btnSkip.Enabled = false;
            _questionTimer.Stop(); // 停止当前题倒计时

            // 不计对错，直接下一题或结束
            if (questionNumber == totalQuestions)
            {
                FinishQuiz();
            }
            else
            {
                questionNumber++;
                askQuestion(questionNumber);
            }
        }

        private void btnScoreboard_Click(object sender, EventArgs e)
        {
            using (var sb = new ScoreboardForm())
            {
                sb.ShowDialog(this);
            }
        }

        // ===== 保存分数到 scores（username_snapshot / score / created_at / user_id）=====
        private void SaveScore(string username, int scoreValue)
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                {
                    con.Open();

                    int? userId = null;
                    string snapshot = string.IsNullOrWhiteSpace(username) ? "Onbekend" : username.Trim();

                    // 你是强制登录：优先取登录的用户ID
                    if (Auth.CurrentUser != null)
                        userId = Auth.CurrentUser.Id;

                    // 兜底：如果 CurrentUser 为 null，尝试按用户名查询
                    if (!userId.HasValue)
                    {
                        using (var find = new SqlCommand("SELECT id FROM dbo.users WHERE username=@u;", con))
                        {
                            find.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = snapshot;
                            var obj = find.ExecuteScalar();
                            if (obj != null && obj != DBNull.Value)
                                userId = Convert.ToInt32(obj);
                        }
                    }

                    // 最后再兜底一次：如果依旧没有 userId，就创建一个“Guest”用户（可选）
                    if (!userId.HasValue)
                    {
                        using (var insUser = new SqlCommand(
                            "INSERT INTO dbo.users(username,password_hash,role_id,created_at) OUTPUT INSERTED.id VALUES(@u,'',1,GETDATE());",
                            con))
                        {
                            insUser.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = snapshot;
                            userId = Convert.ToInt32(insUser.ExecuteScalar());
                        }
                    }

                    // 此处 user_id 一定有值，避免 NULL 违反约束
                    using (var cmd = new SqlCommand(
                        @"INSERT INTO dbo.scores(user_id, username_snapshot, score, created_at)
                          VALUES(@uid, @u, @s, GETDATE());", con))
                    {
                        cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId.Value;
                        cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = snapshot;
                        cmd.Parameters.Add("@s", SqlDbType.Int).Value = scoreValue;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het opslaan van de score: " + ex.Message, "Fout",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn5050_Click(object sender, EventArgs e)
        {
            // 已用过则禁用
            if (_used5050)
            {
                btn5050.Enabled = false;
                MessageBox.Show("Je hebt al de 50/50 gebruikt.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _used5050 = true;
            btn5050.Enabled = false;

            // 找出错误的三个答案
            var btns = new[] { button1, button2, button3, button4 };
            var wrongIndices = new List<int>();

            for (int i = 0; i < btns.Length; i++)
            {
                int tag = Convert.ToInt32(btns[i].Tag);
                if (tag != correctAnswer) wrongIndices.Add(i);
            }

            // 随机禁用其中两个错误选项
            if (wrongIndices.Count >= 2)
            {
                var rand = new Random();
                var toDisable = wrongIndices.OrderBy(x => rand.Next()).Take(2).ToList();
                foreach (var i in toDisable)
                {
                    btns[i].Enabled = false;
                    btns[i].BackColor = Color.Gray;
                }
            }

            // 留下正确答案和一个错误答案，提升正确率到 50%
        }

        private void btnSpecialQuiz_Click(object sender, EventArgs e)
        {
            using (var sq = new SpecialQuizForm())
            {
                sq.ShowDialog(this);
            }
        }
    }
}
