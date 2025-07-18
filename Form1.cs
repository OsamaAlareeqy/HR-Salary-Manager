using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
            openFileDialog.Filter = "DAT files (*.dat)|*.dat|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(filePath);
                allRecords.Clear();

                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 6) continue;

                    int employeeId = int.Parse(parts[0]);
                    string datetimeStr = parts[1] + " " + parts[2];
                    DateTime timestamp = DateTime.ParseExact(datetimeStr, "yyyy-MM-dd HH:mm:ss", null);
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

                MessageBox.Show($"تم تحميل {allRecords.Count} سجل.");
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
            var grouped = recordsToShow
                .OrderBy(r => r.Timestamp)
                .GroupBy(r => r.EmployeeId);

            var holidays = LoadOfficialHolidays();

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
                            // إهمال الدخول السابق وإضافة سطر فارغ
                            rows.Add(new
                            {
                                الرقم_الوظيفي = lastIn.EmployeeId,
                                التاريخ = lastIn.Timestamp.ToString("yyyy-MM-dd"),
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
                    الرقم_الوظيفي = group.Key,
                    التاريخ = "مجموع",
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
