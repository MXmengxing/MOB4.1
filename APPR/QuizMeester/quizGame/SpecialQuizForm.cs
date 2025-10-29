using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace quizGame
{
    public partial class SpecialQuizForm : Form
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;
        private readonly Random _rand = new Random();
        private readonly HashSet<int> _usedQuestionIds = new HashSet<int>();

        private int _correctCount;     // 已答对数量（目标 10）
        private int _timeElapsed;      // 总用时（秒）
        private Timer _timer;          // 计时器（1s 累加）

        public SpecialQuizForm()
        {
            InitializeComponent();

            // UI 初始文案
            lblTitle.Text = "Special Quiz – 10 vragen, zo snel mogelijk!";
            lblTimer.Text = "Tijd: 00:00";
            lblQuestion.Text = "Vraag wordt geladen...";

            // 启动计时
            _timer = new Timer { Interval = 1000 };
            _timer.Tick += (s, e) =>
            {
                _timeElapsed++;
                lblTimer.Text = string.Format("Tijd: {0:00}:{1:00}", _timeElapsed / 60, _timeElapsed % 60);
            };
            _timer.Start();

            // 加载第一题
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            int qid = -1;

            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                qid = GetRandomQuestionId(con);
                if (qid == -1)
                {
                    Finish();
                    return;
                }

                // 题干
                string questionText;
                using (var cmd = new SqlCommand("SELECT question_text FROM dbo.questions WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", qid);
                    object obj = cmd.ExecuteScalar();
                    questionText = obj == null ? "" : Convert.ToString(obj);
                }
                lblQuestion.Text = questionText;

                // 答案（随机顺序）
                var answers = new List<Tuple<string, bool>>();
                using (var cmd = new SqlCommand(
                    "SELECT answer_text, is_correct FROM dbo.answers WHERE question_id=@id ORDER BY NEWID();", con))
                {
                    cmd.Parameters.AddWithValue("@id", qid);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            answers.Add(Tuple.Create(r.GetString(0), r.GetBoolean(1)));
                        }
                    }
                }

                // 显示到按钮
                var btns = new[] { btnAnswer1, btnAnswer2, btnAnswer3, btnAnswer4 };
                for (int i = 0; i < 4; i++)
                {
                    btns[i].Enabled = true;
                    btns[i].BackColor = Color.WhiteSmoke;
                    btns[i].Text = answers[i].Item1;
                    btns[i].Tag = answers[i].Item2; // true/false 放在 Tag
                }
            }
        }

        private int GetRandomQuestionId(SqlConnection con)
        {
            var ids = new List<int>();
            using (var cmd = new SqlCommand("SELECT id FROM dbo.questions;", con))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                    ids.Add(r.GetInt32(0));
            }

            // 选择还没出过的题
            var unused = ids.Except(_usedQuestionIds).ToList();
            if (unused.Count == 0) return -1;

            int qid = unused[_rand.Next(unused.Count)];
            _usedQuestionIds.Add(qid);
            return qid;
        }

        private void AnswerClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            bool isCorrect = btn.Tag is bool && (bool)btn.Tag;

            if (isCorrect)
            {
                btn.BackColor = Color.PaleGreen;
                _correctCount++;
                if (_correctCount >= 10)
                {
                    Finish();
                    return;
                }
            }
            else
            {
                btn.BackColor = Color.LightCoral;
                _timeElapsed += 5; // 答错 +5 秒
                lblTimer.Text = string.Format("Tijd: {0:00}:{1:00}", _timeElapsed / 60, _timeElapsed % 60);
            }

            // 暂时禁用按钮，短暂停顿后进入下一题
            btnAnswer1.Enabled = btnAnswer2.Enabled = btnAnswer3.Enabled = btnAnswer4.Enabled = false;

            var delay = new Timer { Interval = 500 };
            EventHandler goNext = null;
            goNext = (s, ev) =>
            {
                delay.Tick -= goNext;
                delay.Stop();
                delay.Dispose();
                LoadQuestion();
            };
            delay.Tick += goNext;
            delay.Start();
        }

        private void Finish()
        {
            _timer?.Stop();
            MessageBox.Show(
                string.Format("Klaar!\nJe hebt {0} goed.\nTotale tijd: {1:00}:{2:00}",
                    _correctCount, _timeElapsed / 60, _timeElapsed % 60),
                "Resultaat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Close();
        }

        // 事件绑定（Designer 已连接到这几个方法即可）
        private void btnAnswer1_Click(object sender, EventArgs e) { AnswerClick(sender, e); }
        private void btnAnswer2_Click(object sender, EventArgs e) { AnswerClick(sender, e); }
        private void btnAnswer3_Click(object sender, EventArgs e) { AnswerClick(sender, e); }
        private void btnAnswer4_Click(object sender, EventArgs e) { AnswerClick(sender, e); }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }
    }
}
