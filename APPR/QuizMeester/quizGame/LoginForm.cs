using System;
using System.Windows.Forms;

namespace quizGame
{
    public partial class LoginForm : Form
    {
        // 登陆成功后把用户对象带回 Program.cs（可选）
        public LoggedInUser CurrentUser;

        public LoginForm()
        {
            InitializeComponent();

            // 小设置（也可在设计器里设）
            this.Text = "QuizMeester - Inloggen";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AcceptButton = btnLogin;      // 回车键 = 登录
            txtPass.UseSystemPasswordChar = true; // 密码隐藏
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var user = Auth.Login(txtUser.Text, txtPass.Text);
            if (user == null) { MessageBox.Show("Onjuiste gebruikersnaam of wachtwoord."); return; }

            Auth.CurrentUser = user;         // 直接写到全局
            this.DialogResult = DialogResult.OK;
        }


        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 打开注册窗口（确保已创建 RegisterForm，并把按钮事件实现好）
            using (var rf = new RegisterForm())
            {
                rf.ShowDialog();
            }
        }
    }
}
