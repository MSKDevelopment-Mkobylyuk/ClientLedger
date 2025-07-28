using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ClientLedger.Helpers;

namespace ClientLedger.Data
{
    public static class WorkEntryRepository
    {
        public static void EnsureTable()
        {
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    @"
                    CREATE TABLE IF NOT EXISTS WorkEntries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerId INTEGER NOT NULL,
                        WorkDate TEXT NOT NULL,
                        Hours REAL NOT NULL,
                        Description TEXT,
                        CategoryName TEXT,
                        FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE
                    );",
                    conn
                );
                cmd.ExecuteNonQuery();
            }
        }

        public static List<WorkEntry> GetAll()
        {
            var list = new List<WorkEntry>();
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Id, CustomerId, WorkDate, Hours, Description, CategoryName FROM WorkEntries",
                    conn
                );
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new WorkEntry
                            {
                                Id = reader.GetInt32(0),
                                CustomerId = reader.GetInt32(1),
                                WorkDate = DateTime.Parse(reader.GetString(2)),
                                Hours = reader.GetDouble(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                CategoryName = reader.GetString(5),
                            }
                        );
                    }
                }
            }
            return list;
        }

        public static void Add(WorkEntry entry)
        {
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "INSERT INTO WorkEntries (CustomerId, WorkDate, Hours, Description, CategoryName) VALUES (@custId, @date, @hours, @desc, @catName)",
                    conn
                );
                cmd.Parameters.AddWithValue("@custId", entry.CustomerId);
                cmd.Parameters.AddWithValue("@date", entry.WorkDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@hours", entry.Hours);
                cmd.Parameters.AddWithValue("@desc", entry.Description ?? "");
                cmd.Parameters.AddWithValue("@catName", entry.CategoryName ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        public static void Update(WorkEntry entry)
        {
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE WorkEntries SET CustomerId = @custId, WorkDate = @date, Hours = @hours, Description = @desc WHERE Id = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@custId", entry.CustomerId);
                cmd.Parameters.AddWithValue("@date", entry.WorkDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@hours", entry.Hours);
                cmd.Parameters.AddWithValue("@desc", entry.Description ?? "");
                cmd.Parameters.AddWithValue("@id", entry.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int workEntryId)
        {
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand("DELETE FROM WorkEntries WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", workEntryId);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<WorkEntry> GetByCustomerId(int customerId)
        {
            var list = new List<WorkEntry>();
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Id, CustomerId, WorkDate, Hours, Description FROM WorkEntries WHERE CustomerId = @custId",
                    conn
                );
                cmd.Parameters.AddWithValue("@custId", customerId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new WorkEntry
                            {
                                Id = reader.GetInt32(0),
                                CustomerId = reader.GetInt32(1),
                                WorkDate = DateTime.Parse(reader.GetString(2)),
                                Hours = reader.GetDouble(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            }
                        );
                    }
                }
            }
            return list;
        }
    }

    public class WorkEntry
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime WorkDate { get; set; }
        public double Hours { get; set; }
        public string Description { get; set; }

        public string CategoryName { get; set; }

        public int? CategoryId { get; set; }
    }
}
