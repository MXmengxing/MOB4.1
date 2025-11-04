using System;
using System.Windows.Forms;

namespace quizGame
{
    public partial class LoginForm : Form
    {
        // Nadat inloggen is gelukt, kan het gebruikersobject teruggegeven worden aan Program.cs (optioneel)
        public LoggedInUser CurrentUser;

        public LoginForm()
        {
            InitializeComponent();

            // Kleine UI-instellingen (kan ook via de Designer gedaan worden)
            this.Text = "QuizMeester - Inloggen";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AcceptButton = btnLogin;               // Enter-toets activeert de inlogknop
            txtPass.UseSystemPasswordChar = true;       // Verberg wachtwoordtekens
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Controleer gebruikersnaam en wachtwoord via Auth.Login
            var user = Auth.Login(txtUser.Text, txtPass.Text);

            if (user == null)
            {
                // Foutmelding als inloggegevens onjuist zijn
                MessageBox.Show("Onjuiste gebruikersnaam of wachtwoord.");
                return;
            }

            // Sla ingelogde gebruiker globaal op
            Auth.CurrentUser = user;

            // Meld aanroeper (bijv. Program.cs) dat inloggen is gelukt
            this.DialogResult = DialogResult.OK;
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open het registratievenster (zorg dat RegisterForm bestaat en correct is ingesteld)
            using (var rf = new RegisterForm())
            {
                rf.ShowDialog();
            }
        }
    }
}
