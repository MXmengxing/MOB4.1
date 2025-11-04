using System;
using System.Windows.Forms;

namespace quizGame
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();

            // Basisvensterinstellingen
            this.Text = "QuizMeester - Registreren";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Verberg wachtwoordtekens
            txtPass.UseSystemPasswordChar = true;
            txtPass2.UseSystemPasswordChar = true;

            // Enter-toets activeert registreren
            this.AcceptButton = btnRegister;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // === Basisvalidatie ===
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Gebruikersnaam en wachtwoord zijn vereist.");
                return;
            }

            if (txtPass.Text != txtPass2.Text)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen.");
                return;
            }

            // === Roep de Auth.Register-methode aan om de gebruiker toe te voegen ===
            string error;
            if (Auth.Register(txtUser.Text, txtPass.Text, out error))
            {
                MessageBox.Show("Registratie geslaagd! Je kunt nu inloggen.");
                this.Close(); // Venster sluiten na succesvolle registratie
            }
            else
            {
                MessageBox.Show(error ?? "Registratie mislukt.");
            }
        }
    }
}
