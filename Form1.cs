using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

                        if (!int.TryParse(parts[0], out int employeeId))
                            continue;

                        string datetimeStr = parts[1] + " " + parts[2];
                        if (!DateTime.TryParseExact(datetimeStr, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out DateTime timestamp))
                            continue;

                        bool isIn = parts[4] == "0";
                        bool isOut = parts[5] == "1";

                        allRecords.Add(new AttendanceRecord
                        {
                            EmployeeId = employeeId,
                            Timestamp = timestamp,
                            IsIn = isIn,
                            IsOut = isOut
                        });
                    }
                }

                allRecords = allRecords
                    .GroupBy(r => new { r.EmployeeId, r.Timestamp, r.IsIn, r.IsOut })
                    .Select(g => g.First()).ToList();

                MessageBox.Show($"تم تحميل {allRecords.Count} سجل من {openFileDialog.FileNames.Length} ملف.");
                DisplayRecords(allRecords);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date.AddDays(1).AddTicks(-1);

            var filtered = allRecords
                .Where(r => r.Timestamp >= fromDate && r.Timestamp <= toDate)
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
                .OrderBy(r => r.Timestamp)
                .GroupBy(r => new { r.EmployeeId, Date = r.Timestamp.Date });

            foreach (var group in grouped)
            {
                var records = group.OrderBy(r => r.Timestamp).ToList();
                AttendanceRecord lastIn = null;

                double totalRegular = 0;
                double totalOvertime = 0;
                double totalHolidayOvertime = 0;

                foreach (var rec in records)
                {
                    if (rec.IsIn)
                    {
                        if (lastIn == null)
                        {
                            lastIn = rec;
                        }
                        else
                        {
                            rows.Add(new
                            {
                                الرقم_الوظيفي = lastIn.EmployeeId,
                                التاريخ = lastIn.Timestamp.ToString("yyyy-MM-dd"),
                                اليوم = lastIn.Timestamp.ToString("dddd", new CultureInfo("ar-JO")),
                                وقت_الدخول = lastIn.Timestamp.ToString("HH:mm:ss"),
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
                        TimeSpan duration = rec.Timestamp - lastIn.Timestamp;
                        double totalHours = duration.TotalHours;
                        string dayKey = lastIn.Timestamp.ToString("yyyy-MM-dd");
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
                            الرقم_الوظيفي = lastIn.EmployeeId,
                            التاريخ = lastIn.Timestamp.ToString("yyyy-MM-dd"),
                            اليوم = lastIn.Timestamp.ToString("dddd", new CultureInfo("ar-JO")),
                            وقت_الدخول = lastIn.Timestamp.ToString("HH:mm:ss"),
                            وقت_الخروج = rec.Timestamp.ToString("HH:mm:ss"),
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
                            الرقم_الوظيفي = rec.EmployeeId,
                            التاريخ = rec.Timestamp.ToString("yyyy-MM-dd"),
                            اليوم = rec.Timestamp.ToString("dddd", new CultureInfo("ar-JO")),
                            وقت_الدخول = "",
                            وقت_الخروج = rec.Timestamp.ToString("HH:mm:ss"),
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
                        الرقم_الوظيفي = lastIn.EmployeeId,
                        التاريخ = lastIn.Timestamp.ToString("yyyy-MM-dd"),
                        اليوم = lastIn.Timestamp.ToString("dddd", new CultureInfo("ar-JO")),
                        وقت_الدخول = lastIn.Timestamp.ToString("HH:mm:ss"),
                        وقت_الخروج = "",
                        مدة_العمل_بالساعة = "",
                        ساعات_اساسية = "",
                        ساعات_اضافية = "",
                        ساعات_اضافية_رسمية = ""
                    });
                }

                rows.Add(new
                {
                    الرقم_الوظيفي = group.Key.EmployeeId,
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

        public class AttendanceRecord
        {
            public int EmployeeId { get; set; }
            public DateTime Timestamp { get; set; }
            public bool IsIn { get; set; }
            public bool IsOut { get; set; }

            
            public bool IsHoliday { get; set; } 
            public DateTime Date => Timestamp.Date; 
            public double RegularTime { get; set; } 
            public double OverTime { get; set; } 
        }


        public class EmployeeWorkSummary
        {
            public int EmployeeId { get; set; }
            public double TotalRegularHours { get; set; }
            public double TotalOvertimeHours { get; set; }
            public double TotalHolidayOvertimeHours { get; set; }
        }
        public static EmployeeWorkSummary GenerateSummaryForEmployee(int employeeId, DateTime selectedMonth, List<AttendanceRecord> attendanceRecords)
        {
            var summary = new EmployeeWorkSummary
            {
                EmployeeId = employeeId,
                TotalRegularHours = 0,
                TotalOvertimeHours = 0,
                TotalHolidayOvertimeHours = 0
            };

            foreach (var record in attendanceRecords)
            {
                if (record.EmployeeId != employeeId)
                    continue;

                if (record.Date.Month != selectedMonth.Month || record.Date.Year != selectedMonth.Year)
                    continue;

                if (record.IsHoliday)
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
                .GroupBy(r => new { r.EmployeeId, Date = r.Timestamp.Date });

            var summaryMap = new Dictionary<int, EmployeeWorkSummary>();

            foreach (var group in grouped)
            {
                var records = group.OrderBy(r => r.Timestamp).ToList();
                AttendanceRecord lastIn = null;

                foreach (var rec in records)
                {
                    if (rec.IsIn)
                    {
                        lastIn = rec;
                    }
                    else if (lastIn != null)
                    {
                        TimeSpan duration = rec.Timestamp - lastIn.Timestamp;
                        double hours = duration.TotalHours;
                        bool isHoliday = holidays.Contains(lastIn.Timestamp.ToString("yyyy-MM-dd"));

                        if (!summaryMap.ContainsKey(group.Key.EmployeeId))
                        {
                            summaryMap[group.Key.EmployeeId] = new EmployeeWorkSummary
                            {
                                EmployeeId = group.Key.EmployeeId
                            };
                        }

                        if (isHoliday)
                        {
                            summaryMap[group.Key.EmployeeId].TotalHolidayOvertimeHours += hours;
                        }
                        else
                        {
                            summaryMap[group.Key.EmployeeId].TotalRegularHours += Math.Min(hours, 9);
                            summaryMap[group.Key.EmployeeId].TotalOvertimeHours += Math.Max(0, hours - 9);
                        }

                        lastIn = null;
                    }
                }
            }

            summaries = summaryMap.Values.ToList();
            return summaries;
        }

        private HashSet<string> LoadOfficialHolidays()
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
