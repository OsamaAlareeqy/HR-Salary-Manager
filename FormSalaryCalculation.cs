using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using static Salary_Cal.gridAttendance;

namespace Salary_Cal
{
    public partial class FormSalaryCalculation : Form
    {
        private List<AttendanceRecord> allRecords = new List<AttendanceRecord>();
        private double totalSalary = 0;
        private List<gridAttendance.AttendanceRecord> allAttendanceRecords;



        public void LoadAttendanceRecords(List<gridAttendance.AttendanceRecord> records)
        {
            allAttendanceRecords = records;
        }

        private EmployeeWorkSummary summary;
        public FormSalaryCalculation(EmployeeWorkSummary passedSummary)
        {
            InitializeComponent();
            summary = passedSummary;
            LoadEmployees();
            LoadAttendanceRecords();
            LoadSummaryToForm();
        }

        private void FormSalaryCalculation_Load(object sender, EventArgs e)
        {
            LoadEmployees();
            LoadAttendanceRecords();
        }

        private void LoadEmployees()
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT EmployeeID, Name FROM Employees", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbEmployees.Items.Add(new
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            cmbEmployees.DisplayMember = "Name";
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null || allAttendanceRecords == null)
            {
                MessageBox.Show("يرجى اختيار موظف وتحميل سجلات الحضور أولاً.");
                return;
            }

            dynamic selectedEmp = cmbEmployees.SelectedItem;
            int empId = selectedEmp.ID;
            DateTime selectedMonth = dtMonth.Value;

            var summary = gridAttendance.GenerateSummaryForEmployee(empId, selectedMonth, allAttendanceRecords);

            txtRegularHours.Text = summary.TotalRegularHours.ToString("0.##");
            txtOvertimeHours.Text = summary.TotalOvertimeHours.ToString("0.##");
            txtHolidayHours.Text = summary.TotalHolidayOvertimeHours.ToString("0.##");

            if (double.TryParse(txtMonthlySalary.Text, out double monthlySalary))
            {
                double hourlyRate = monthlySalary / 30 / 9;

                double totalPay =
                    summary.TotalRegularHours * hourlyRate +
                    summary.TotalOvertimeHours * hourlyRate * 1.25 +
                    summary.TotalHolidayOvertimeHours * hourlyRate * 1.5;

                txtCalculatedSalary.Text = totalPay.ToString("0.##");
                txtResult.Text = totalPay.ToString("0.##");
            }
            else
            {
                MessageBox.Show("الرجاء إدخال راتب شهري صحيح.");
            }
        }


        private void LoadAttendanceRecords()
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT EmployeeID, Timestamp, IsIn FROM Attendance", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    allRecords.Add(new AttendanceRecord
                    {
                        EmployeeId = reader.GetInt32(0),
                        Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                        IsIn = Convert.ToBoolean(reader["IsIn"])
                    });
                }
            }
        }

        private void LoadSummaryToForm()
        {
            txtRegularHours.Text = summary.TotalRegularHours.ToString("0.##");
            txtOvertimeHours.Text = summary.TotalOvertimeHours.ToString("0.##");
            txtHolidayHours.Text = summary.TotalHolidayOvertimeHours.ToString("0.##");

            if (double.TryParse(txtMonthlySalary.Text, out double salary))
            {
                double hourlyRate = salary / 30 / 9;

                double totalPay =
                    hourlyRate * summary.TotalRegularHours +
                    hourlyRate * 1.25 * summary.TotalOvertimeHours +
                    hourlyRate * 1.5 * summary.TotalHolidayOvertimeHours;

                txtCalculatedSalary.Text = totalPay.ToString("0.##");
            }
        }

        private double GetSalary(int empId)
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Salary FROM Employees WHERE EmployeeID = @id", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                return Convert.ToDouble(cmd.ExecuteScalar());
            }
        }

        private (string Title, string NationalID, string HireDate) GetEmployeeDetails(int empId)
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Title, NationalID, HireDate FROM Employees WHERE EmployeeID = @id", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        reader["Title"].ToString(),
                        reader["NationalID"].ToString(),
                        reader["HireDate"].ToString()
                    );
                }
            }
            return ("", "", "");
        }

        private List<(DateTime Date, DateTime? In, DateTime? Out, double Hours)> GetDailyAttendance(List<AttendanceRecord> records)
        {
            var grouped = new List<(DateTime, DateTime?, DateTime?, double)>();
            var days = records
                .GroupBy(r => r.Timestamp.Date)
                .OrderBy(g => g.Key);

            foreach (var day in days)
            {
                var ins = day.Where(r => r.IsIn).OrderBy(r => r.Timestamp).FirstOrDefault();
                var outs = day.Where(r => !r.IsIn).OrderByDescending(r => r.Timestamp).FirstOrDefault();

                if (ins != null && outs != null && outs.Timestamp > ins.Timestamp)
                {
                    double hours = (outs.Timestamp - ins.Timestamp).TotalHours;
                    grouped.Add((day.Key, ins.Timestamp, outs.Timestamp, Math.Round(hours, 2)));
                }
                else
                {
                    grouped.Add((day.Key, ins?.Timestamp, outs?.Timestamp, 0));
                }
            }
            return grouped;
        }

        private HashSet<string> LoadOfficialHolidays()
        {
            var holidays = new HashSet<string>();
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Date FROM Holidays", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    holidays.Add(Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd"));
                }
            }
            return holidays;
        }

        public class AttendanceRecord
        {
            public int EmployeeId { get; set; }
            public DateTime Timestamp { get; set; }
            public bool IsIn { get; set; }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {

        }
        public List<AttendanceRecord> GetAttendanceRecords()
        {
            return allRecords;
        }

        private List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();

        public void LoadAttendanceRecords(List<AttendanceRecord> records)
        {
            attendanceRecords = records;
            MessageBox.Show("Loaded: " + records.Count + " records");

        }
    }
}

