using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace quizGame
{
    /// <summary>
    /// Ingelogde gebruikersinformatie (tijdelijke opslag tijdens runtime)
    /// </summary>
    public class LoggedInUser
    {
        public int Id;
        public string Username;
        public int RoleId;
    }

    /// <summary>
    /// Inlog- en registratiefunctionaliteit
    /// </summary>
    public static class Auth
    {
        // Lees de verbindingsstring uit App.config
        private static readonly string cs =
            ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;

        // Globale opslag van de huidige ingelogde gebruiker
        public static LoggedInUser CurrentUser;

        /// <summary>
        /// Registreer een nieuwe gebruiker.
        /// (Het wachtwoord wordt in platte tekst opgeslagen in de kolom 'password_hash' — toegestaan voor deze opdracht)
        /// </summary>
        public static bool Register(string username, string password, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                error = "Gebruikersnaam en wachtwoord zijn vereist.";
                return false;
            }

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(
                "INSERT INTO users(username, password_hash, role_id) VALUES(@u, @p, 1);", con))
            {
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = username.Trim();
                cmd.Parameters.Add("@p", SqlDbType.NVarChar, 100).Value = password.Trim();

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    // Unieke sleutelconflict (gebruikersnaam bestaat al)
                    if (ex.Number == 2627 || ex.Number == 2601)
                        error = "Gebruikersnaam bestaat al.";
                    else
                        error = "Databasefout: " + ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Controleer de inloggegevens en geef een LoggedInUser terug als ze geldig zijn.
        /// </summary>
        public static LoggedInUser Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(
                @"SELECT TOP 1 id, username, password_hash, role_id
                  FROM users
                  WHERE username = @u;", con))
            {
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = username.Trim();
                con.Open();

                using (var r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!r.Read()) return null;

                    string stored = Convert.ToString(r["password_hash"]) ?? string.Empty;

                    // Vergelijk ingevoerd wachtwoord met opgeslagen wachtwoord (platte tekst)
                    if (stored.Trim() == password.Trim())
                    {
                        var user = new LoggedInUser
                        {
                            Id = Convert.ToInt32(r["id"]),
                            Username = Convert.ToString(r["username"]),
                            RoleId = Convert.ToInt32(r["role_id"])
                        };

                        // Inloggen geslaagd → opslaan als globale sessiegebruiker
                        CurrentUser = user;
                        return user;
                    }

                    // Onjuist wachtwoord
                    return null;
                }
            }
        }
    }
}
