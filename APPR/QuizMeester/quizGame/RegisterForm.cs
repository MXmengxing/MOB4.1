using System;
using System.Windows.Forms;

namespace quizGame
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            this.Text = "QuizMeester - Registreren";
            this.StartPosition = FormStartPosition.CenterScreen;
            txtPass.UseSystemPasswordChar = true;
            txtPass2.UseSystemPasswordChar = true;
            this.AcceptButton = btnRegister;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // 基本验证
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

            // 调用 Auth 注册方法
            string error;
            if (Auth.Register(txtUser.Text, txtPass.Text, out error))
            {
                MessageBox.Show("Registratie geslaagd! Je kunt nu inloggen.");
                this.Close(); // 注册成功后关闭窗口
            }
            else
            {
                MessageBox.Show(error ?? "Registratie mislukt.");
            }
        }
    }
}
