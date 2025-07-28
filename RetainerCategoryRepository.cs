using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ClientLedger.Helpers;

namespace ClientLedger.Data
{
    public static class RetainerCategoryRepository
    {
        public static void EnsureTable()
        {
            Console.WriteLine("RetainerCategoryRepository.EnsureTable called.");
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                using (
                    var cmd = new SQLiteCommand(
                        @"
                    CREATE TABLE IF NOT EXISTS RetainerCategories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerId INTEGER NOT NULL,
                        CategoryName TEXT NOT NULL,
                        MaxHours REAL NOT NULL DEFAULT 0,
                        BaseRate REAL NOT NULL DEFAULT 0,
                        UsedHours REAL NOT NULL DEFAULT 0,
                        FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE
                    );",
                        conn
                    )
                )
                {
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("RetainerCategoryRepository.EnsureTable finished.");
        }

        public static List<RetainerCategory> GetByCustomerId(int customerId)
        {
            Console.WriteLine(
                $"RetainerCategoryRepository.GetByCustomerId called with customerId={customerId}"
            );
            var list = new List<RetainerCategory>();

            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                using (
                    var cmd = new SQLiteCommand(
                        "SELECT Id, CategoryName, MaxHours, BaseRate, UsedHours FROM RetainerCategories WHERE CustomerId = @custId",
                        conn
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@custId", customerId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var category = new RetainerCategory
                            {
                                Id = reader.GetInt32(0),
                                CategoryName = reader.GetString(1),
                                MaxHours = (decimal)reader.GetDouble(2),
                                BaseRate = (decimal)reader.GetDouble(3),
                                UsedHours = (decimal)reader.GetDouble(4),
                            };
                            Console.WriteLine(
                                $"  Retrieved Category Id={category.Id}, Name={category.CategoryName}"
                            );
                            list.Add(category);
                        }
                    }
                }
            }

            Console.WriteLine(
                $"RetainerCategoryRepository.GetByCustomerId returning {list.Count} categories."
            );
            return list;
        }

        public static void Add(int customerId, RetainerCategory category)
        {
            Console.WriteLine(
                $"RetainerCategoryRepository.Add called with customerId={customerId}, CategoryName={category.CategoryName}, MaxHours={category.MaxHours}, BaseRate={category.BaseRate}, UsedHours={category.UsedHours}"
            );
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                using (
                    var cmd = new SQLiteCommand(
                        @"
                    INSERT INTO RetainerCategories (CustomerId, CategoryName, MaxHours, BaseRate, UsedHours)
                    VALUES (@custId, @name, @max, @rate, @used)",
                        conn
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@custId", customerId);
                    cmd.Parameters.AddWithValue("@name", category.CategoryName);
                    cmd.Parameters.AddWithValue("@max", category.MaxHours);
                    cmd.Parameters.AddWithValue("@rate", category.BaseRate);
                    cmd.Parameters.AddWithValue("@used", category.UsedHours);
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"  Insert executed, rows affected: {rows}");
                }
            }
        }

        public static void DeleteByCustomerId(int customerId)
        {
            Console.WriteLine(
                $"RetainerCategoryRepository.DeleteByCustomerId called with customerId={customerId}"
            );
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                using (
                    var cmd = new SQLiteCommand(
                        "DELETE FROM RetainerCategories WHERE CustomerId = @custId",
                        conn
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@custId", customerId);
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"  Deleted {rows} rows for customerId={customerId}");
                }
            }
        }
    }
}
