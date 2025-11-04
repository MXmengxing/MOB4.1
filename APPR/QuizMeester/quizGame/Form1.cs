using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Media;               // Voor het afspelen van het geluid bij de speciale vraag
using System.Windows.Forms;

namespace quizGame
{
    public partial class Form1 : Form
    {
        // === Spelstatus ===
        int correctAnswer = 0;
        int correctAnswers = 0;
        int wrongAnswers = 0;
        int questionNumber = 1;
        int score;
        int percentage;
        int totalQuestions;
        int currentQuestionNumber = 1;

        // === Vraagvolgorde ===
        readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;
        readonly List<int> questionOrder = new List<int>();

        // === Timersysteem ===
        private Timer _questionTimer;
        private Timer _quizTimer;

        // Resttijd per vraag (seconden)
        private int _qTimeLeft = 0;

        // Resttijd voor de hele quiz (seconden)
        private int _quizTimeLeft = 180;

        // Is de totaaltimer al gestart (slechts bij de eerste vraag)?
        private bool _quizStarted = false;

        // Kleuren voor tijdswaarschuwingen
        private readonly Color _timeNormal = Color.White;
        private readonly Color _timeWarn = Color.OrangeRed;

        // === Overslaan (1 keer toegestaan) ===
        private bool _hasSkipped = false;

        // === 50/50 joker ===
        private bool _used5050 = false;  // Is 50/50 al gebruikt?

        // === Spelersnaam (snapshot voor scores.username_snapshot) ===
        private string _playerName = "Gast"; // Fallback; normaal komt dit uit de login

        // === Speciale vraag ===
        private readonly Random _rand = new Random();
        private int _specialQuestionIndex = -1;   // Welke vraag (1-based) is speciaal, binnen de eerste 20
        private const int _specialBonus = 2;      // Extra punten bij goede beantwoording van de speciale vraag
        private bool _isSpecialNow = false;       // Is de huidige vraag speciaal?

        // Originele kleuren om na de speciale vraag te herstellen
        private Color _origFormBackColor;
        private Color _origQuestionForeColor;
        private Color _origQuestionBackColor;

        public Form1()
        {
            InitializeComponent();

            // Timers initialiseren
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

            // Start: knoppen toestaan
            if (btnSkip != null) btnSkip.Enabled = true;
            if (btn5050 != null) btn5050.Enabled = true;

            // Vraagvolgorde opbouwen en starten
            BuildQuestionOrder();

            totalQuestions = questionOrder.Count;
            if (totalQuestions == 0)
            {
                MessageBox.Show("Er staan nog geen vragen in de database.", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            // Originele thema-kleuren onthouden
            _origFormBackColor = this.BackColor;
            _origQuestionForeColor = lblQuestion.ForeColor;
            _origQuestionBackColor = lblQuestion.BackColor;

            // Speciale vraag bepalen: random tussen 1..min(20, aantal vragen)
            int specialMax = Math.Min(20, totalQuestions);
            if (specialMax >= 1)
            {
                _specialQuestionIndex = _rand.Next(1, specialMax + 1);
            }

            // Spelersnaam uit login (fallback blijft “Gast”)
            if (Auth.CurrentUser != null && !string.IsNullOrWhiteSpace(Auth.CurrentUser.Username))
                _playerName = Auth.CurrentUser.Username.Trim();

            askQuestion(questionNumber);
        }

        /// <summary>Genereer de vraagvolgorde van deze ronde (max. 20 vragen) — alleen vragen met precies 4 antwoorden.</summary>
        private void BuildQuestionOrder()
        {
            int limit = 20;
            using (var con = new SqlConnection(_cs))
            {
                con.Open();
                using (var cmd = new SqlCommand(@"
            SELECT TOP (@n) q.id
            FROM dbo.questions q
            WHERE (SELECT COUNT(*) FROM dbo.answers a WHERE a.question_id = q.id) = 4
            ORDER BY NEWID();
        ", con))
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

        /// <summary>Klik op een antwoord (gemeenschappelijke handler voor alle antwoordknoppen).</summary>
        private void ClickAnswerEvent(object sender, EventArgs e)
        {
            // Stop de vraag-timer om dubbele decrement te voorkomen
            _questionTimer.Stop();

            var senderObject = (Button)sender;
            int buttonTag = Convert.ToInt32(senderObject.Tag);

            if (buttonTag == correctAnswer)
            {
                score++;                         // Normaal +1
                if (_isSpecialNow) score += _specialBonus;  // Extra bonus bij speciale vraag
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

            // Alle vier knoppen tijdelijk uit
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            // Laatste vraag? -> afronden
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

        /// <summary>Toon vraag + start timers.</summary>
        private void askQuestion(int qnum)
        {
            if (qnum < 1 || qnum > questionOrder.Count) return;

            currentQuestion.Text = "Vraag nummer: " + qnum;

            int qid = questionOrder[qnum - 1];

            string questionText = "";
            int perQuestionSeconds = 15; // Standaard 15s per vraag
            var answers = new List<Tuple<string, bool>>();

            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                // Vraagtekst + individuele tijd per vraag
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

                // Vier willekeurige opties
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
                MessageBox.Show("Antwoord-sleutel ontbreekt voor vraag (id=" + qid + ").", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // —— Speciale vraag: markering / thema / geluid ——
            _isSpecialNow = (qnum == _specialQuestionIndex);
            if (_isSpecialNow)
            {
                // Duidelijke stijlwijziging
                this.BackColor = Color.MediumPurple;
                lblQuestion.ForeColor = Color.Yellow;
                lblQuestion.BackColor = Color.FromArgb(32, 0, 0, 0);

                try
                {
                    // Eventueel vervangen door eigen WAV-bestand
                    SystemSounds.Asterisk.Play();
                }
                catch { /* Geluidsfout stil negeren */ }
            }
            else
            {
                RestoreNormalTheme();
            }

            // === Per-vraag aftellen ===
            _qTimeLeft = perQuestionSeconds > 0 ? perQuestionSeconds : 15;
            if (lblQuestionTime != null)
            {
                lblQuestionTime.ForeColor = _timeNormal;
                lblQuestionTime.Text = $"Vraag tijd: {_qTimeLeft}s";
            }
            _questionTimer.Stop();
            _questionTimer.Start();

            // === Start totaaltimer (alleen één keer) ===
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

            // === Overslaan: alleen beschikbaar als nog niet gebruikt ===
            if (btnSkip != null) btnSkip.Enabled = !_hasSkipped;

            // === 50/50: per ronde één keer (of pas aan naar per vraag) ===
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

                    // Tijd op -> telt als fout
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
                    // Totale tijd op -> afronden
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

        /// <summary>Centraal afronden (laatste vraag / tijd voorbij / handmatig stoppen).</summary>
        private void FinishQuiz()
        {
            _questionTimer.Stop();
            _quizTimer.Stop();

            // Normaal thema terugzetten
            RestoreNormalTheme();

            percentage = (int)Math.Round((double)(100 * score) / Math.Max(1, totalQuestions));

            // Score opslaan
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
                // Complete reset
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

                // Timers resetten
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

                // Overslaan resetten
                _hasSkipped = false;
                if (btnSkip != null) btnSkip.Enabled = true;

                // Nieuwe speciale vraag voor de volgende ronde
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

                // Nieuwe speciale vraagpositie
                int specialMax = Math.Min(20, totalQuestions);
                _specialQuestionIndex = specialMax >= 1 ? _rand.Next(1, specialMax + 1) : -1;

                askQuestion(questionNumber);
            }
        }

        // Eén keer overslaan
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
            _questionTimer.Stop(); // Huidige vraag-timer stoppen

            // Geen goed/fout, direct volgende of afronden
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

        // ===== Score opslaan naar dbo.scores (username_snapshot / score / created_at / user_id) =====
        private void SaveScore(string username, int scoreValue)
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                {
                    con.Open();

                    int? userId = null;
                    string snapshot = string.IsNullOrWhiteSpace(username) ? "Onbekend" : username.Trim();

                    // In jouw app is login verplicht: neem eerst CurrentUser.Id
                    if (Auth.CurrentUser != null)
                        userId = Auth.CurrentUser.Id;

                    // Fallback: lookup op gebruikersnaam
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

                    // Nog steeds geen id? Maak een “gast”-record aan (optioneel)
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

                    // Nu is user_id gegarandeerd gevuld
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
            // Reeds gebruikt? Dan uitzetten
            if (_used5050)
            {
                btn5050.Enabled = false;
                MessageBox.Show("Je hebt al de 50/50 gebruikt.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _used5050 = true;
            btn5050.Enabled = false;

            // Zoek de drie foute opties
            var btns = new[] { button1, button2, button3, button4 };
            var wrongIndices = new List<int>();

            for (int i = 0; i < btns.Length; i++)
            {
                int tag = Convert.ToInt32(btns[i].Tag);
                if (tag != correctAnswer) wrongIndices.Add(i);
            }

            // Schakel willekeurig twee van de foute opties uit
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

            // Overblijven: juiste antwoord + één fout antwoord => 50% kans
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
