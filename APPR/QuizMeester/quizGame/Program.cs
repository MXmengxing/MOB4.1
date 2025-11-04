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

            // --- Toon het inlogvenster ---
            using (var lf = new LoginForm())
            {
                // Alleen doorgaan als de gebruiker correct heeft ingelogd
                if (lf.ShowDialog() != DialogResult.OK || Auth.CurrentUser == null)
                    return; // Stop het programma als inloggen is geannuleerd of mislukt
            }

            // --- Navigeer op basis van de gebruikersrol ---
            if (Auth.CurrentUser.RoleId == 2)
            {
                // Beheerder → open het beheervenster
                Application.Run(new AdminForm());
            }
            else
            {
                // Gewone gebruiker → start de quiz
                Application.Run(new Form1());
            }
        }
    }
}
