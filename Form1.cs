using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Salary_Cal;


namespace Salary_Cal
{
    public partial class gridAttendance : Form
    {
        List<AttendanceRecord> allRecords = new List<AttendanceRecord>();

        public gridAttendance()
        {
            InitializeComponent();
        }

        private void gridAttendance_Load(object sender, EventArgs e)
        {
            DatabaseHelper.InitializeDatabase();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Supported files (*.dat;*.txt;*.log;*.csv)|*.dat;*.txt;*.log;*.csv|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                allRecords.Clear();

                foreach (string filePath in openFileDialog.FileNames)
                {
                    string[] lines = File.ReadAllLines(filePath);


                    foreach (var line in lines)
                    {
                        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 6) continue;

                        AttendanceRecord record = new AttendanceRecord
                        {
                            EmployeeID = int.Parse(parts[0]),
                            Name = parts[1],
                            Date = DateTime.Parse(parts[2]),
                            InTime = parts[3] == "True",
                            OutTime = parts[4] == "True",
                            RegularTime = double.Parse(parts[5]),
                            OverTime = double.Parse(parts[6])
                        };

                        allRecords.Add(record);

                        DatabaseHelper.SaveAttendanceRecord(record);
                    }

                }

                allRecords = allRecords
                    .GroupBy(r => new { r.EmployeeID, r.Date, r.InTime, r.OutTime })
                    .Select(g => g.First()).ToList();

                MessageBox.Show($"تم تحميل {allRecords.Count} سجل من {openFileDialog.FileNames.Length} ملف.");
                DisplayRecords(allRecords);
                int selectedEmployeeID = 21003; 
                DateTime selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                EmployeeWorkSummary summary = GenerateSummaryForEmployee(selectedEmployeeID, selectedMonth, allRecords);
                FormSalaryCalculation formSalary = new FormSalaryCalculation(summary);
                formSalary.LoadAttendanceRecords(allRecords);
                formSalary.Show();

            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date.AddDays(1).AddTicks(-1);

            var filtered = allRecords
                .Where(r => r.Date >= fromDate && r.Date <= toDate)
                .ToList();

            if (filtered.Count == 0)
            {
                MessageBox.Show("لا يوجد سجلات ضمن النطاق الزمني المحدد.");
                return;
            }

            DisplayRecords(filtered);
        }

        private void DisplayRecords(List<AttendanceRecord> recordsToShow)
        {
            var rows = new List<dynamic>();
            var holidays = LoadOfficialHolidays();

            var grouped = recordsToShow
                .OrderBy(r => r.Date)
                .GroupBy(r => new { r.EmployeeID, Date = r.Date.Date });

            foreach (var group in grouped)
            {
                var records = group.OrderBy(r => r.Date).ToList();
                AttendanceRecord lastIn = null;

                double totalRegular = 0;
                double totalOvertime = 0;
                double totalHolidayOvertime = 0;

                foreach (var rec in records)
                {
                    if (rec.InTime)
                    {
                        if (lastIn == null)
                        {
                            lastIn = rec;
                        }
                        else
                        {
                            rows.Add(new
                            {
                                الرقم_الوظيفي = lastIn.EmployeeID,
                                التاريخ = lastIn.Date.ToString("yyyy-MM-dd"),
                                اليوم = lastIn.Date.ToString("dddd", new CultureInfo("ar-JO")),
                                وقت_الدخول = lastIn.Date.ToString("HH:mm:ss"),
                                وقت_الخروج = "",
                                مدة_العمل_بالساعة = "",
                                ساعات_اساسية = "",
                                ساعات_اضافية = "",
                                ساعات_اضافية_رسمية = ""
                            });
                            lastIn = rec;
                        }
                    }
                    else if (lastIn != null)
                    {
                        TimeSpan duration = rec.Date - lastIn.Date;
                        double totalHours = duration.TotalHours;
                        string dayKey = lastIn.Date.ToString("yyyy-MM-dd");
                        bool isHoliday = holidays.Contains(dayKey);

                        double regularHours = 0;
                        double overtimeHours = 0;
                        double holidayOvertimeHours = 0;

                        if (isHoliday)
                        {
                            holidayOvertimeHours = totalHours;
                        }
                        else
                        {
                            regularHours = Math.Min(totalHours, 9);
                            overtimeHours = totalHours > 9 ? totalHours - 9 : 0;
                        }

                        totalRegular += regularHours;
                        totalOvertime += overtimeHours;
                        totalHolidayOvertime += holidayOvertimeHours;

                        rows.Add(new
                        {
                            الرقم_الوظيفي = lastIn.EmployeeID,
                            التاريخ = lastIn.Date.ToString("yyyy-MM-dd"),
                            اليوم = lastIn.Date.ToString("dddd", new CultureInfo("ar-JO")),
                            وقت_الدخول = lastIn.Date.ToString("HH:mm:ss"),
                            وقت_الخروج = rec.Date.ToString("HH:mm:ss"),
                            مدة_العمل_بالساعة = duration.ToString(@"hh\:mm\:ss"),
                            ساعات_اساسية = regularHours.ToString("0.##"),
                            ساعات_اضافية = overtimeHours.ToString("0.##"),
                            ساعات_اضافية_رسمية = holidayOvertimeHours.ToString("0.##")
                        });

                        lastIn = null;
                    }
                    else
                    {
                        rows.Add(new
                        {
                            الرقم_الوظيفي = rec.EmployeeID,
                            التاريخ = rec.Date.ToString("yyyy-MM-dd"),
                            اليوم = rec.Date.ToString("dddd", new CultureInfo("ar-JO")),
                            وقت_الدخول = "",
                            وقت_الخروج = rec.Date.ToString("HH:mm:ss"),
                            مدة_العمل_بالساعة = "",
                            ساعات_اساسية = "",
                            ساعات_اضافية = "",
                            ساعات_اضافية_رسمية = ""
                        });
                    }
                }

                if (lastIn != null)
                {
                    rows.Add(new
                    {
                        الرقم_الوظيفي = lastIn.EmployeeID,
                        التاريخ = lastIn.Date   .ToString("yyyy-MM-dd"),
                        اليوم = lastIn.Date.ToString("dddd", new CultureInfo("ar-JO")),
                        وقت_الدخول = lastIn.Date.ToString("HH:mm:ss"),
                        وقت_الخروج = "",
                        مدة_العمل_بالساعة = "",
                        ساعات_اساسية = "",
                        ساعات_اضافية = "",
                        ساعات_اضافية_رسمية = ""
                    });
                }

                rows.Add(new
                {
                    الرقم_الوظيفي = group.Key.EmployeeID,
                    التاريخ = "مجموع",
                    اليوم = "",
                    وقت_الدخول = "",
                    وقت_الخروج = "",
                    مدة_العمل_بالساعة = "",
                    ساعات_اساسية = totalRegular.ToString("0.##"),
                    ساعات_اضافية = totalOvertime.ToString("0.##"),
                    ساعات_اضافية_رسمية = totalHolidayOvertime.ToString("0.##")
                });
            }

            gridView.DataSource = rows;

            foreach (DataGridViewRow row in gridView.Rows)
            {
                var دخولCell = row.Cells["وقت_الدخول"];
                var خروجCell = row.Cells["وقت_الخروج"];
                var مدةCell = row.Cells["مدة_العمل_بالساعة"];

                if (دخولCell.Value == null || string.IsNullOrWhiteSpace(دخولCell.Value.ToString()))
                    دخولCell.Style.BackColor = System.Drawing.Color.Red;

                if (خروجCell.Value == null || string.IsNullOrWhiteSpace(خروجCell.Value.ToString()))
                    خروجCell.Style.BackColor = System.Drawing.Color.Red;

                if (مدةCell.Value == null || string.IsNullOrWhiteSpace(مدةCell.Value.ToString()))
                    مدةCell.Style.BackColor = System.Drawing.Color.Red;
            }
        }




        public class EmployeeWorkSummary
        {
            public int EmployeeID { get; set; }
            public double TotalRegularHours { get; set; }
            public double TotalOvertimeHours { get; set; }
            public double TotalHolidayOvertimeHours { get; set; }
        }
        public static EmployeeWorkSummary GenerateSummaryForEmployee(int EmployeeID, DateTime selectedMonth, List<AttendanceRecord> attendanceRecords)
        {
            var summary = new EmployeeWorkSummary
            {
                EmployeeID = EmployeeID,
                TotalRegularHours = 0,
                TotalOvertimeHours = 0,
                TotalHolidayOvertimeHours = 0
            };

            foreach (var record in attendanceRecords)
            {

                if (record.EmployeeID != EmployeeID)
                    continue;

                if (record.Date.Month != selectedMonth.Month || record.Date.Year != selectedMonth.Year)
                    continue;

                var holidays = LoadOfficialHolidays();

                if (holidays.Contains(record.Date.ToString("yyyy-MM-dd")))
                {
                    summary.TotalHolidayOvertimeHours += record.OverTime;
                }
                else
                {
                    summary.TotalRegularHours += record.RegularTime;
                    summary.TotalOvertimeHours += record.OverTime;
                }

            }

            return summary;
        }


        public List<EmployeeWorkSummary> GetWorkSummaries()
        {
            var summaries = new List<EmployeeWorkSummary>();
            var holidays = LoadOfficialHolidays();

            var grouped = allRecords
                .GroupBy(r => new { r.EmployeeID, Date = r.Date.Date });

            var summaryMap = new Dictionary<int, EmployeeWorkSummary>();

            foreach (var group in grouped)
            {
                var records = group.OrderBy(r => r.Date).ToList();
                AttendanceRecord lastIn = null;

                foreach (var rec in records)
                {
                    if (rec.InTime)
                    {
                        lastIn = rec;
                    }
                    else if (lastIn != null)
                    {
                        TimeSpan duration = rec.Date - lastIn.Date;
                        double hours = duration.TotalHours;
                        bool isHoliday = holidays.Contains(lastIn.Date.ToString("yyyy-MM-dd"));

                        if (!summaryMap.ContainsKey(group.Key.EmployeeID))
                        {
                            summaryMap[group.Key.EmployeeID] = new EmployeeWorkSummary
                            {
                                EmployeeID = group.Key.EmployeeID
                            };
                        }

                        if (isHoliday)
                        {
                            summaryMap[group.Key.EmployeeID].TotalHolidayOvertimeHours += hours;
                        }
                        else
                        {
                            summaryMap[group.Key.EmployeeID].TotalRegularHours += Math.Min(hours, 9);
                            summaryMap[group.Key.EmployeeID].TotalOvertimeHours += Math.Max(0, hours - 9);
                        }

                        lastIn = null;
                    }
                }
            }

            summaries = summaryMap.Values.ToList();
            return summaries;
        }

        private static HashSet<string> LoadOfficialHolidays()
        {
            var holidays = new HashSet<string>();
            using (var conn = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                conn.Open();
                string query = "SELECT Date FROM Holidays";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        holidays.Add(Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd"));
                    }
                }
            }
            return holidays;
        }
    }
}
