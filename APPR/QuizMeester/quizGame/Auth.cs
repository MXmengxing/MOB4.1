using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace quizGame
{
    /// <summary>
    /// 已登录用户信息（运行时存储）
    /// </summary>
    public class LoggedInUser
    {
        public int Id;
        public string Username;
        public int RoleId;
    }

    /// <summary>
    /// 登录与注册逻辑
    /// </summary>
    public static class Auth
    {
        // 从 App.config 读取连接字符串
        private static readonly string cs =
            ConfigurationManager.ConnectionStrings["QuizDb"].ConnectionString;

        // 当前登录的用户（全局保存）
        public static LoggedInUser CurrentUser;

        /// <summary>
        /// 注册用户（明文密码写入 password_hash 列——作业允许）
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
                    // 唯一键冲突（用户名已存在）
                    if (ex.Number == 2627 || ex.Number == 2601)
                        error = "Gebruikersnaam bestaat al.";
                    else
                        error = "Databasefout: " + ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// 登录验证
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

                    if (stored.Trim() == password.Trim())
                    {
                        var user = new LoggedInUser
                        {
                            Id = Convert.ToInt32(r["id"]),
                            Username = Convert.ToString(r["username"]),
                            RoleId = Convert.ToInt32(r["role_id"])
                        };

                        // 登录成功 → 全局保存
                        CurrentUser = user;
                        return user;
                    }

                    return null;
                }
            }
        }
    }
}
