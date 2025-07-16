using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Salary_Cal
{
    public static class DatabaseHelper
    {
        private static string dbPath = "employees.db";

        public static void InitializeDatabase()
        {
            

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                string createEmployeeTable = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    EmployeeID INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Branch TEXT NOT NULL,
                    Salary REAL NOT NULL
                );";

                using (var command = new SQLiteCommand(createEmployeeTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string createBranchTable = @"
                CREATE TABLE IF NOT EXISTS Branches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );";

                using (var command = new SQLiteCommand(createBranchTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                string createHolidayTable = @"
                CREATE TABLE IF NOT EXISTS Holidays (
                    Date TEXT PRIMARY KEY,
                    Description TEXT
                );";

                using (var command = new SQLiteCommand(createHolidayTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                string createAdvancesTable = @"
                CREATE TABLE IF NOT EXISTS Advances (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeID INTEGER NOT NULL,
                    Amount REAL NOT NULL,
                    Date TEXT NOT NULL
                );";

                using (var command = new SQLiteCommand(createAdvancesTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string createAttendanceTable = @"
                CREATE TABLE IF NOT EXISTS Attendance (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeID INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    Regular REAL DEFAULT 0,
                    Overtime REAL DEFAULT 0,
                    HolidayOver REAL DEFAULT 0,
                    FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
                );";

                using (var command = new SQLiteCommand(createAttendanceTable, connection))
                {
                    command.ExecuteNonQuery();

                }

            }
        }

        public static void AddEmployee(int id, string name, string title, string branch, double salary)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO Employees (EmployeeID, Name, Title, Branch, Salary) VALUES (@id, @name, @title, @branch, @salary)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@branch", branch);
                    cmd.Parameters.AddWithValue("@salary", salary);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("تمت إضافة الموظف بنجاح ✅");
        }

        public static void AddBranch(string branchName)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT OR IGNORE INTO Branches (Name) VALUES (@name)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", branchName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<string> GetAllBranches()
        {
            List<string> branches = new List<string>();
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Name FROM Branches";
                using (var cmd = new SQLiteCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        branches.Add(reader["Name"].ToString());
                    }
                }
            }
            return branches;
        }
        public static void AddHoliday(string date, string description)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT OR IGNORE INTO Holidays (Date, Description) VALUES (@date, @desc)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static bool IsHoliday(DateTime date)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT 1 FROM Holidays WHERE Date = @date";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

    }
}
