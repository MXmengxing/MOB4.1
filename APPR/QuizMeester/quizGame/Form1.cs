using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace quizGame
{
    public partial class Form1 : Form
    {
        // variables list for this quiz game
        int correctAnswer = 0;
        int correctAnswers = 0;
        int wrongAnswers = 0;
        int questionNumber = 1;
        int score;
        int percentage;
        int totalQuestions;
        int currentQuestionNumber = 1;

        public Form1()
        {
            InitializeComponent();

            askQuestion(questionNumber);

            totalQuestions = 20;
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

                DialogResult dialogResult = MessageBox.Show("De quiz is afgelopen" + Environment.NewLine + Environment.NewLine +
                                "Je hebt " + score + " vragen goed beantwoord" + Environment.NewLine +
                                "Je totale percentage is " + percentage + "%" + Environment.NewLine +
                                "Wil je opnieuw spelen?", "Resultaten", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    score = 0;
                    correctAnswer = 0;
                    correctAnswers = 0;
                    wrongAnswers = 0;
                    questionNumber = 1;
                    correctAnswersLabel.Text = "Goede antwoorden: " + 0;
                    WrongAnswersLabel.Text = "Foute antwoorden: " + 0;
                    askQuestion(questionNumber);
                }
                else
                {
                    this.Close();
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
            if (questionNumber != 21)
            {
                currentQuestion.Text = "Vraag nummer: " + questionNumber;
            }

            switch (qnum)
            {
                case 1:
                    pictureBox1.Image = Properties.Resources.sky;
                    lblQuestion.Text = "Welke kleur heeft de lucht?";

                    button1.Text = "Blauw";
                    button2.Text = "Geel";
                    button3.Text = "Paars";
                    button4.Text = "Rood";

                    correctAnswer = 1;
                    break;

                case 2:
                    pictureBox1.Image = Properties.Resources.ironMan;
                    lblQuestion.Text = "Wat is de naam van het hoofdpersonage uit de film Iron Man?";

                    button1.Text = "Tony Stank";
                    button2.Text = "Tony Stark";
                    button3.Text = "Rhodey";
                    button4.Text = "Peter Quill";

                    correctAnswer = 2;
                    break;

                case 3:
                    pictureBox1.Image = Properties.Resources.fortnite;
                    lblQuestion.Text = "Welke uitgever maakte de game hierboven?";

                    button1.Text = "EA";
                    button2.Text = "Activision";
                    button3.Text = "Square Enix";
                    button4.Text = "Epic Games";

                    correctAnswer = 4;
                    break;

                case 4:
                    pictureBox1.Image = Properties.Resources.london;
                    lblQuestion.Text = "Wat is de hoofdstad van Engeland?";

                    button1.Text = "Birmingham";
                    button2.Text = "Londen";
                    button3.Text = "Brighton";
                    button4.Text = "Liverpool";

                    correctAnswer = 2;
                    break;

                case 5:
                    pictureBox1.Image = Properties.Resources.gears_of_war;
                    lblQuestion.Text = "Wat is de naam van deze game?";

                    button1.Text = "Gears of War";
                    button2.Text = "Call of Duty";
                    button3.Text = "Battlefield";
                    button4.Text = "Bionic Commando";

                    correctAnswer = 1;
                    break;

                case 6:
                    pictureBox1.Image = Properties.Resources.halo;
                    lblQuestion.Text = "Wat is de naam van het hoofdpersonage in deze game?";

                    button1.Text = "Altair";
                    button2.Text = "Lara Croft";
                    button3.Text = "Master Chief";
                    button4.Text = "Drake";

                    correctAnswer = 3;
                    break;

                case 7:
                    pictureBox1.Image = Properties.Resources.csgo;
                    lblQuestion.Text = "Wat is de naam van deze game?";

                    button1.Text = "Counter Strike: GO";
                    button2.Text = "Call of Duty";
                    button3.Text = "Battlefield";
                    button4.Text = "Half Life 3";

                    correctAnswer = 1;
                    break;

                case 8:
                    pictureBox1.Image = Properties.Resources.chrinaGreatWall;
                    lblQuestion.Text = "Welk land heeft de meeste inwoners ter wereld?";

                    button1.Text = "China";
                    button2.Text = "India";
                    button3.Text = "Verenigde Staten";
                    button4.Text = "Rusland";

                    correctAnswer = 1;
                    break;

                case 9:
                    pictureBox1.Image = Properties.Resources.mars;
                    lblQuestion.Text = "Welke planeet staat bekend als de rode planeet?";

                    button1.Text = "Mars";
                    button2.Text = "Venus";
                    button3.Text = "Jupiter";
                    button4.Text = "Saturnus";

                    correctAnswer = 1;
                    break;

                case 10:
                    pictureBox1.Image = Properties.Resources.brazil;
                    lblQuestion.Text = "Wat is de officiële taal in Brazilië?";

                    button1.Text = "Spaans";
                    button2.Text = "Frans";
                    button3.Text = "Engels";
                    button4.Text = "Portugees";

                    correctAnswer = 4;
                    break;

                case 11:
                    pictureBox1.Image = Properties.Resources.oscar;
                    lblQuestion.Text = "Welke film won de Oscar voor Beste Film in 2021?";

                    button1.Text = "Nomadland";
                    button2.Text = "The Trial of the Chicago 7";
                    button3.Text = "Minari";
                    button4.Text = "Sound of Metal";

                    correctAnswer = 1;
                    break;

                case 12:
                    pictureBox1.Image = Properties.Resources.japan;
                    lblQuestion.Text = "Wat is de hoofdstad van Japan?";

                    button1.Text = "Kyoto";
                    button2.Text = "Tokio";
                    button3.Text = "Osaka";
                    button4.Text = "Hiroshima";

                    correctAnswer = 2;
                    break;

                case 13:
                    pictureBox1.Image = Properties.Resources.everest;
                    lblQuestion.Text = "Wat is de hoogste berg ter wereld?";

                    button1.Text = "Mount Everest";
                    button2.Text = "Kilimanjaro";
                    button3.Text = "Mount McKinley";
                    button4.Text = "Mont Blanc";

                    correctAnswer = 1;
                    break;

                case 14:
                    pictureBox1.Image = Properties.Resources.earth;
                    lblQuestion.Text = "Wat is het meest voorkomende element in de atmosfeer?";

                    button1.Text = "Zuurstof";
                    button2.Text = "Stikstof";
                    button3.Text = "Waterstof";
                    button4.Text = "Koolstof";

                    correctAnswer = 2;
                    break;

                case 15:
                    pictureBox1.Image = Properties.Resources.Aristotle;
                    lblQuestion.Text = "Hoe heet de Griekse filosoof uit de 4e eeuw v.Chr., een van de beroemdste filosofen in de geschiedenis?";

                    button1.Text = "Socrates";
                    button2.Text = "Aristoteles";
                    button3.Text = "Plato";
                    button4.Text = "Democritus";

                    correctAnswer = 2;
                    break;

                case 16:
                    pictureBox1.Image = Properties.Resources.coding;
                    lblQuestion.Text = "Welke programmeertaal wordt gebruikt voor Android-appontwikkeling?";

                    button1.Text = "Java";
                    button2.Text = "C#";
                    button3.Text = "Python";
                    button4.Text = "JavaScript";

                    correctAnswer = 1;
                    break;

                case 17:
                    pictureBox1.Image = Properties.Resources.leonardoDaVinci;
                    lblQuestion.Text = "Welke Italiaanse schilder maakte het beroemde werk Het Laatste Avondmaal?";

                    button1.Text = "Leonardo da Vinci";
                    button2.Text = "Rafaël";
                    button3.Text = "Michelangelo";
                    button4.Text = "Picasso";

                    correctAnswer = 1;
                    break;

                case 18:
                    pictureBox1.Image = Properties.Resources.lang;
                    lblQuestion.Text = "Wat is de meest gesproken taal ter wereld?";

                    button1.Text = "Engels";
                    button2.Text = "Chinees";
                    button3.Text = "Hindi";
                    button4.Text = "Spaans";

                    correctAnswer = 2;
                    break;

                case 19:
                    pictureBox1.Image = Properties.Resources.countires;
                    lblQuestion.Text = "Hoeveel landen zijn er in de wereld?";

                    button1.Text = "177";
                    button2.Text = "195";
                    button3.Text = "212";
                    button4.Text = "243";

                    correctAnswer = 2;
                    break;

                case 20:
                    pictureBox1.Image = Properties.Resources.moonWalker;
                    lblQuestion.Text = "Wat is de naam van de eerste astronaut die op de maan liep?";

                    button1.Text = "Buzz Aldrin";
                    button2.Text = "Neil Armstrong";
                    button3.Text = "Michael Collins";
                    button4.Text = "Juri Gagarin";

                    correctAnswer = 2;
                    break;
            }

            System.Threading.Thread.Sleep(500);

            button1.BackColor = Color.DarkCyan;
            button2.BackColor = Color.DarkCyan;
            button3.BackColor = Color.DarkCyan;
            button4.BackColor = Color.DarkCyan;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TotalQuestionsLabel.Text = "Aantal vragen: " + totalQuestions;
            correctAnswersLabel.Text = "Goede antwoorden: " + 0;
            WrongAnswersLabel.Text = "Foute antwoorden: " + 0;
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
                correctAnswersLabel.Text = "Goede antwoorden: " + 0;
                WrongAnswersLabel.Text = "Foute antwoorden: " + 0;
                askQuestion(questionNumber);
            }
        }
    }
}
