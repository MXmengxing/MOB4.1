using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace quizGame
{
    public partial class SpecialQuizForm : Form
    {
        // --- Config/State ----------------------------------------------------
        private readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;
        private readonly Random _rand = new Random();
        private readonly HashSet<int> _usedQuestionIds = new HashSet<int>(); // al gestelde vragen

        private int _correctCount;    // aantal goede antwoorden (doel: 10)
        private int _timeElapsed;     // totale tijd in seconden
        private Timer _timer;         // 1s-timer voor de klok

        public SpecialQuizForm()
        {
            InitializeComponent();

            // Basisteksten
            lblTitle.Text = "Special Quiz – 10 vragen, zo snel mogelijk!";
            lblTimer.Text = "Tijd: 00:00";
            lblQuestion.Text = "Vraag wordt geladen...";

            // Start klok
            _timer = new Timer { Interval = 1000 };
            _timer.Tick += (s, e) =>
            {
                _timeElapsed++;
                lblTimer.Text = $"Tijd: {_timeElapsed / 60:00}:{_timeElapsed % 60:00}";
            };
            _timer.Start();

            // Laad eerste vraag
            LoadQuestion();
        }

        // --------------------------------------------------------------------
        // Laadt één vraag (met exact 4 opties). Als er geen geldige vraag meer
        // is, rondt de quiz af.
        // --------------------------------------------------------------------
        private void LoadQuestion()
        {
            int qid;

            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                qid = GetRandomEligibleQuestionId(con);
                if (qid == -1)
                {
                    Finish();
                    return;
                }

                // Vraagtekst
                string questionText = "";
                using (var cmd = new SqlCommand(
                    "SELECT question_text FROM dbo.questions WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", qid);
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        questionText = Convert.ToString(obj);
                }
                lblQuestion.Text = questionText;

                // Antwoorden in willekeurige volgorde
                var answers = new List<(string Text, bool IsCorrect)>();
                using (var cmd = new SqlCommand(@"
                    SELECT answer_text, is_correct
                    FROM dbo.answers
                    WHERE question_id = @id
                    ORDER BY NEWID();", con))
                {
                    cmd.Parameters.AddWithValue("@id", qid);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            answers.Add((r.GetString(0), r.GetBoolean(1)));
                    }
                }

                // Veilige guard: we eisen 4 opties. Zo niet, markeer deze vraag als gebruikt
                // en probeer gewoon de volgende (zonder crash).
                if (answers.Count != 4)
                {
                    _usedQuestionIds.Add(qid);
                    LoadQuestion();
                    return;
                }

                // Tonen op knoppen
                var btns = new[] { btnAnswer1, btnAnswer2, btnAnswer3, btnAnswer4 };
                for (int i = 0; i < 4; i++)
                {
                    btns[i].Enabled = true;
                    btns[i].BackColor = Color.WhiteSmoke;

                    btns[i].Text = answers[i].Text;   // tekst
                    btns[i].Tag = answers[i].IsCorrect; // true/false in Tag
                }
            }
        }

        // --------------------------------------------------------------------
        // Haalt een willekeurige vraag-ID op die voldoet aan:
        //  - exact 4 antwoorden in dbo.answers
        //  - nog niet eerder gesteld (not in _usedQuestionIds)
        // Geen geldige vragen meer -> return -1
        // --------------------------------------------------------------------
        private int GetRandomEligibleQuestionId(SqlConnection con)
        {
            var allEligible = new List<int>();

            using (var cmd = new SqlCommand(@"
                SELECT q.id
                FROM dbo.questions q
                WHERE (SELECT COUNT(*) FROM dbo.answers a WHERE a.question_id = q.id) = 4;", con))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                    allEligible.Add(r.GetInt32(0));
            }

            var unused = allEligible.Where(id => !_usedQuestionIds.Contains(id)).ToList();
            if (unused.Count == 0) return -1;

            int pick = unused[_rand.Next(unused.Count)];
            _usedQuestionIds.Add(pick);
            return pick;
        }

        // --------------------------------------------------------------------
        // Klik op een antwoord: goed -> groen en +1; fout -> rood en +5 sec
        // Daarna korte pauze en door naar de volgende vraag.
        // --------------------------------------------------------------------
        private void AnswerClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            bool isCorrect = btn.Tag is bool b && b;

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
                _timeElapsed += 5; // straf +5s
                lblTimer.Text = $"Tijd: {_timeElapsed / 60:00}:{_timeElapsed % 60:00}";
            }

            // alles tijdelijk uitzetten
            btnAnswer1.Enabled = btnAnswer2.Enabled = btnAnswer3.Enabled = btnAnswer4.Enabled = false;

            // halve seconde pauze, dan volgende vraag
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

        // --------------------------------------------------------------------
        // Afronden en resultaat tonen
        // --------------------------------------------------------------------
        private void Finish()
        {
            _timer?.Stop();
            MessageBox.Show(
                $"Klaar!\nJe hebt {_correctCount} goed.\nTotale tijd: {_timeElapsed / 60:00}:{_timeElapsed % 60:00}",
                "Resultaat",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Close();
        }

        // --------------------------------------------------------------------
        // Event-wiring (Designer moet deze koppelingen hebben)
        // --------------------------------------------------------------------
        private void btnAnswer1_Click(object sender, EventArgs e) => AnswerClick(sender, e);
        private void btnAnswer2_Click(object sender, EventArgs e) => AnswerClick(sender, e);
        private void btnAnswer3_Click(object sender, EventArgs e) => AnswerClick(sender, e);
        private void btnAnswer4_Click(object sender, EventArgs e) => AnswerClick(sender, e);
        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
