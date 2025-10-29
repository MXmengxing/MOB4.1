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

            using (var lf = new LoginForm())
            {
                // 登录窗口：OK 且 Auth.CurrentUser 不为空才继续
                if (lf.ShowDialog() != DialogResult.OK || Auth.CurrentUser == null)
                    return;
            }

            // 根据角色跳转
            if (Auth.CurrentUser.RoleId == 2)
            {
                // 管理员 → 直接进入管理员界面
                Application.Run(new AdminForm());
            }
            else
            {
                // 普通用户 → 进入游戏
                Application.Run(new Form1());
            }
        }
    }
}
