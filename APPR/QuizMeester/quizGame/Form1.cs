using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
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

        // 整场剩余时间（秒）——可自行调整总时长（例如 180 = 3 分钟）
        private int _quizTimeLeft = 180;

        // 整场计时是否已启动（只在第一题启动一次）
        private bool _quizStarted = false;

        // 时间提醒配色
        private readonly Color _timeNormal = Color.White;
        private readonly Color _timeWarn = Color.OrangeRed;

        // === 跳过功能（只允许一次） ===
        private bool _hasSkipped = false;

        // === 玩家名（用于保存到 scores.username_snapshot）===
        private string _playerName = "Gast"; // 没有登录时就用“Gast”

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

            // 构建题序并开始
            BuildQuestionOrder();

            totalQuestions = questionOrder.Count;
            if (totalQuestions == 0)
            {
                MessageBox.Show("Er staan nog geen vragen in de database.", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            askQuestion(questionNumber);
        }

        /// <summary>
        /// 从登录窗体设置玩家名（可选）
        /// </summary>
        public void SetPlayer(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
                _playerName = username.Trim();
        }

        // 生成本次测验题序（最多 20 题）
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

        // 点击答案（所有答案按钮共用）
        private void ClickAnswerEvent(object sender, EventArgs e)
        {
            // 停止本题计时，避免多减
            _questionTimer.Stop();

            var senderObject = (Button)sender;
            int buttonTag = Convert.ToInt32(senderObject.Tag);

            if (buttonTag == correctAnswer)
            {
                score++;
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

        // 出题 + 启动计时
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

        // 统一收尾（最后一题 / 单题超时 / 总时到 都调这里）
        private void FinishQuiz()
        {
            _questionTimer.Stop();
            _quizTimer.Stop();

            percentage = (int)Math.Round((double)(100 * score) / Math.Max(1, totalQuestions));

            // —— 先保存分数到数据库（username_snapshot / score / created_at）——
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
                _quizTimeLeft = 180; // 重新给总时长
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

                askQuestion(questionNumber);
            }
            else
            {
                this.Close();
            }
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
            // 结束按钮也要停表
            _questionTimer?.Stop();
            _quizTimer?.Stop();
            this.Close();
        }

        private void playAgainClick(object sender, EventArgs e)
        {
            if (questionNumber > 1)
            {
                // 停表 + 重置状态
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

                // 重置跳过
                _hasSkipped = false;
                if (btnSkip != null) btnSkip.Enabled = true;

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
                sb.ShowDialog(this); // 模态显示
            }
        }

        // ===== 保存分数到 scores（username_snapshot / score / created_at）=====
        // 可选 userId：以后接入登录时可以加上（目前没用就不传）
        private void SaveScore(string username, int scoreValue)
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                {
                    con.Open();

                    // 尝试查找用户ID
                    int? userId = null;
                    using (var find = new SqlCommand(
                        "SELECT id FROM dbo.users WHERE username = @u", con))
                    {
                        find.Parameters.AddWithValue("@u", username ?? "Gast");
                        var obj = find.ExecuteScalar();
                        if (obj != null && obj != DBNull.Value)
                            userId = Convert.ToInt32(obj);
                    }

                    string sql;
                    if (userId.HasValue)
                    {
                        sql = "INSERT INTO dbo.scores(user_id, username_snapshot, score, created_at) " +
                              "VALUES(@uid, @u, @s, GETDATE());";
                    }
                    else
                    {
                        // 这里会向 user_id 写 NULL —— 需要配合方案 A，使 user_id 允许为 NULL
                        sql = "INSERT INTO dbo.scores(user_id, username_snapshot, score, created_at) " +
                              "VALUES(NULL, @u, @s, GETDATE());";
                    }

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        if (userId.HasValue) cmd.Parameters.AddWithValue("@uid", userId.Value);
                        cmd.Parameters.AddWithValue("@u", string.IsNullOrWhiteSpace(username) ? "Onbekend" : username);
                        cmd.Parameters.AddWithValue("@s", scoreValue);
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

    }
}
