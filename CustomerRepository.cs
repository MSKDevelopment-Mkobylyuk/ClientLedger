using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ClientLedger.Helpers;
using Newtonsoft.Json;

namespace ClientLedger.Data
{
    public static class CustomerRepository
    {
        public static void EnsureTable()
        {
            Console.WriteLine("CustomerRepository.EnsureTable called.");
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    @"
                    CREATE TABLE IF NOT EXISTS Customers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        ContactName TEXT,
                        AgreementType INTEGER NOT NULL DEFAULT 0,
                        BaseRate REAL DEFAULT 0,
                        MonthlyCost REAL DEFAULT 0
                    );",
                    conn
                );
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("CustomerRepository.EnsureTable finished.");
        }

        public static List<Customer> GetAll()
        {
            Console.WriteLine("CustomerRepository.GetAll called.");
            var list = new List<Customer>();
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Id, Name, ContactName, AgreementType, BaseRate, MonthlyCost FROM Customers",
                    conn
                );
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customer = new Customer
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ContactName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            AgreementType = (AgreementType)reader.GetInt32(3),
                            BaseRate = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                            MonthlyCost = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                        };

                        Console.WriteLine(
                            $"  Retrieved Customer Id={customer.Id}, Name={customer.Name}, AgreementType={customer.AgreementType}"
                        );
                        list.Add(customer);
                    }
                }
            }
            Console.WriteLine($"CustomerRepository.GetAll returning {list.Count} customers.");
            return list;
        }

        public static int Add(Customer customer)
        {
            Console.WriteLine(
                $"CustomerRepository.Add called with Name={customer.Name}, AgreementType={customer.AgreementType}, BaseRate={customer.BaseRate}, MonthlyCost={customer.MonthlyCost}"
            );

            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();

                using (
                    var cmd = new SQLiteCommand(
                        @"
            INSERT INTO Customers (Name, ContactName, AgreementType, BaseRate, MonthlyCost)
            VALUES (@name, @contact, @agreement, @baseRate, @monthlycost);
            SELECT last_insert_rowid();
            ",
                        conn
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@name", customer.Name);
                    cmd.Parameters.AddWithValue("@contact", customer.ContactName);
                    cmd.Parameters.AddWithValue("@agreement", (int)customer.AgreementType);
                    cmd.Parameters.AddWithValue("@baseRate", customer.BaseRate);
                    cmd.Parameters.AddWithValue("monthlycost", customer.MonthlyCost);

                    long newId = (long)cmd.ExecuteScalar();
                    customer.Id = (int)newId;

                    Console.WriteLine($"  Insert succeeded, new Customer Id={customer.Id}");

                    return customer.Id;
                }
            }
        }

        public static void Update(Customer customer)
        {
            Console.WriteLine($"CustomerRepository.Update called for Customer Id={customer.Id}");
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    @"UPDATE Customers SET 
                Name = @n, 
                ContactName = @c, 
                AgreementType = @a,
                BaseRate = @b
              WHERE Id = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@n", customer.Name);
                cmd.Parameters.AddWithValue("@c", customer.ContactName ?? "");
                cmd.Parameters.AddWithValue("@a", (int)customer.AgreementType);
                cmd.Parameters.AddWithValue("@b", customer.BaseRate);
                cmd.Parameters.AddWithValue("@id", customer.Id);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"  Update executed, rows affected: {rows}");
            }
        }

        public static void Delete(int customerId)
        {
            Console.WriteLine($"CustomerRepository.Delete called for Customer Id={customerId}");
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand("DELETE FROM Customers WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", customerId);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"  Delete executed, rows affected: {rows}");
            }
        }

        public static Customer GetById(int id)
        {
            using (
                var conn = new SQLiteConnection($"Data Source={MSKCustHelper.DbPath};Version=3;")
            )
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Id, Name, AgreementType FROM Customers WHERE Id = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            AgreementType = reader.IsDBNull(2)
                                ? AgreementType.ServiceAgreement // or default fallback
                                : (AgreementType)reader.GetInt32(2),
                        };
                    }
                }
            }
            return null;
        }
    }
}
