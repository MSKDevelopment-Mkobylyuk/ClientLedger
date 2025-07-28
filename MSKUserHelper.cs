using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ClientLedger.Helpers
{
    public static class MSKUserHelper
    {
        public static string DbPath = ""; // Set from Settings later

        public static void InitializeDatabase(string dbPath)
        {
            DbPath = dbPath;

            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var conn = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                conn.Open();

                string createUsersTable =
                    @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL
                );";

                var cmd = new SQLiteCommand(createUsersTable, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool AddUser(string username, string plainPassword)
        {
            string passwordHash = HashPassword(plainPassword);

            using (var conn = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)",
                    conn
                );
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", passwordHash);

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SQLiteException)
                {
                    return false; // username exists or other error
                }
            }
        }

        public static bool ValidateUser(string username, string plainPassword)
        {
            string passwordHash = HashPassword(plainPassword);

            using (var conn = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username = @u AND PasswordHash = @p",
                    conn
                );
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", passwordHash);

                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static string HashPassword(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        public static bool UserExists(string username)
        {
            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
