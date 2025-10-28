using System;
using System.Windows.Forms;

namespace quizGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 打开登录窗口
            using (var login = new LoginForm())
            {
                // 如果登录成功（DialogResult = OK）
                if (login.ShowDialog() == DialogResult.OK && login.CurrentUser != null)
                {
                    // 进入主界面（可把登录用户传给 Form1）
                    Application.Run(new Form1());
                }
            }
        }
    }
}
