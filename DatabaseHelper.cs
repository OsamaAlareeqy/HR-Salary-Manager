
using DocumentFormat.OpenXml.Drawing.Diagrams;
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

                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL
                );";
                new SQLiteCommand(createUsersTable, connection).ExecuteNonQuery();

                string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
                if ((long)new SQLiteCommand(checkAdmin, connection).ExecuteScalar() == 0)
                {
                    string insertAdmin = "INSERT INTO Users (Username, Password, Role) VALUES ('admin', 'admin123', 'Admin')";
                    new SQLiteCommand(insertAdmin, connection).ExecuteNonQuery();
                }

                string createEmployeeTable = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    EmployeeID INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Branch TEXT NOT NULL,
                    NationalID TEXT,
                    HireDate TEXT,
                    Salary REAL NOT NULL
                );";
                new SQLiteCommand(createEmployeeTable, connection).ExecuteNonQuery();

                string createBranchTable = @"
                CREATE TABLE IF NOT EXISTS Branches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );";
                new SQLiteCommand(createBranchTable, connection).ExecuteNonQuery();

                string createHolidayTable = @"
                CREATE TABLE IF NOT EXISTS Holidays (
                    Date TEXT PRIMARY KEY,
                    Description TEXT
                );";
                new SQLiteCommand(createHolidayTable, connection).ExecuteNonQuery();

                string createAdvancesTable = @"
                CREATE TABLE IF NOT EXISTS Advances (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeID INTEGER NOT NULL,
                    Amount REAL NOT NULL,
                    Date TEXT NOT NULL
                );";
                new SQLiteCommand(createAdvancesTable, connection).ExecuteNonQuery();

                string createAttendanceTable = @"
                CREATE TABLE IF NOT EXISTS Attendance (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeID INTEGER,
                    Name TEXT,
                    Date TEXT,
                    InTime INTEGER,
                    OutTime INTEGER,
                    RegularTime REAL,
                    OverTime REAL
                );
            ";

                new SQLiteCommand(createAttendanceTable, connection).ExecuteNonQuery();
            }
        }

        public static void AddEmployee(int id, string name, string title, string branch, double salary, string nationalId, string hireDate)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO Employees (EmployeeID, Name, Title, Branch, Salary, NationalID, HireDate) VALUES (@id, @name, @title, @branch, @salary, @nid, @hire)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@branch", branch);
                    cmd.Parameters.AddWithValue("@salary", salary);
                    cmd.Parameters.AddWithValue("@nid", nationalId);
                    cmd.Parameters.AddWithValue("@hire", hireDate);
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("تمت إضافة الموظف بنجاح ✅");
        }

        public static double GetEmployeeAdvances(int empId, int month, int year)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string q = "SELECT SUM(Amount) FROM Advances WHERE EmployeeID = @id AND strftime('%m', Date) = @month AND strftime('%Y', Date) = @year";
                using (var cmd = new SQLiteCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@id", empId);
                    cmd.Parameters.AddWithValue("@month", month.ToString("D2"));
                    cmd.Parameters.AddWithValue("@year", year.ToString());
                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }
            }
        }
        public static double GetEmployeeTotalAdvances(int employeeId)
        {
            double total = 0;

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT IFNULL(SUM(Amount), 0) FROM Advances WHERE EmployeeID = @empId", conn);
                cmd.Parameters.AddWithValue("@empId", employeeId);

                object result = cmd.ExecuteScalar();
                total = Convert.ToDouble(result);
            }

            return total;
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

        public static void AddBranch(string branchName)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string query = "INSERT OR IGNORE INTO Branches (Name) VALUES (@name)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", branchName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<string> GetAllBranches()
        {
            List<string> branches = new List<string>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string query = "SELECT Name FROM Branches";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        branches.Add(reader.GetString(0));
                    }
                }
            }
            return branches;
        }

        public static void AddHoliday(DateTime date, string description)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string query = "INSERT OR IGNORE INTO Holidays (Date, Description) VALUES (@date, @desc)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void SaveAttendanceRecord(AttendanceRecord record)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string query = @"INSERT INTO Attendance 
                        (EmployeeID, Name, Date, InTime, OutTime, RegularTime, OverTime) 
                        VALUES (@EmployeeID, @Name, @Date, @InTime, @OutTime, @RegularTime, @OverTime)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", record.EmployeeID);
                    cmd.Parameters.AddWithValue("@Name", record.Name);
                    cmd.Parameters.AddWithValue("@Date", record.Date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@InTime", record.InTime);
                    cmd.Parameters.AddWithValue("@OutTime", record.OutTime);
                    cmd.Parameters.AddWithValue("@RegularTime", record.RegularTime);
                    cmd.Parameters.AddWithValue("@OverTime", record.OverTime);

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}