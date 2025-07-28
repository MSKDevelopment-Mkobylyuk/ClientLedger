// File: Helpers/LedgerDatabaseHelper.cs
using System.Data.SQLite;
using System.IO;

namespace ClientLedger.Helpers
{
    public static class MSKCustHelper
    {
        public static string DbPath = "";

        public static void InitializeDatabase(string dbPath)
        {
            DbPath = dbPath;

            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            // No default tables needed here (CustomerRepository.EnsureTable will handle it)
        }
    }
}
