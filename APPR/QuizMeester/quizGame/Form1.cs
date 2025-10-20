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
        int correctAnswer = 0;
        int correctAnswers = 0;
        int wrongAnswers = 0;
        int questionNumber = 1;
        int score;
        int percentage;
        int totalQuestions;
        int currentQuestionNumber = 1;

        readonly string _cs = ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;
        readonly List<int> questionOrder = new List<int>();

        public Form1()
        {
            InitializeComponent();

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

        private void ClickAnswerEvent(object sender, EventArgs e)
        {
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

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            if (questionNumber == totalQuestions)
            {
                percentage = (int)Math.Round((double)(100 * score) / totalQuestions);

                var dialogResult = MessageBox.Show(
                    "De quiz is afgelopen" + Environment.NewLine + Environment.NewLine +
                    "Je hebt " + score + " vragen goed beantwoord" + Environment.NewLine +
                    "Je totale percentage is " + percentage + "%" + Environment.NewLine +
                    "Wil je opnieuw spelen?",
                    "Resultaten",
                    MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
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

                    askQuestion(questionNumber);
                }
                else
                {
                    Close();
                }
            }
            else
            {
                questionNumber++;
                askQuestion(questionNumber);
            }
        }

        private void askQuestion(int qnum)
        {
            if (qnum < 1 || qnum > questionOrder.Count) return;

            currentQuestion.Text = "Vraag nummer: " + qnum;

            int qid = questionOrder[qnum - 1];

            string questionText = "";
            int perQuestionSeconds = 15;
            var answers = new List<Tuple<string, bool>>();

            using (var con = new SqlConnection(_cs))
            {
                con.Open();

                // vraag
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
                            perQuestionSeconds = r.GetInt32(1);
                        }
                        else
                        {
                            MessageBox.Show("Vraag niet gevonden (id=" + qid + ").", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                // antwoorden (random)
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
            this.Close();
        }

        private void playAgainClick(object sender, EventArgs e)
        {
            if (questionNumber > 1)
            {
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

                askQuestion(questionNumber);
            }
        }
    }
}
