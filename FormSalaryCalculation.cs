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

namespace Salary_Cal
{
    public partial class FormSalaryCalculation : Form
    {
        private List<AttendanceRecord> allRecords = new List<AttendanceRecord>();
        private double totalSalary = 0;

        public FormSalaryCalculation()
        {
            InitializeComponent();
            LoadEmployees();
            LoadAttendanceRecords();
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
            if (cmbEmployees.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار موظف");
                return;
            }

            dynamic selected = cmbEmployees.SelectedItem;
            int empId = selected.ID;
            double salary = GetSalary(empId);
            var empDetails = GetEmployeeDetails(empId);
            var empRecords = allRecords.Where(r => r.EmployeeId == empId).ToList();
            var dailyAttendance = GetDailyAttendance(empRecords);

            double totalHours = dailyAttendance.Sum(d => d.Hours);
            double regularHours = dailyAttendance.Sum(d => d.Hours > 9 ? 9 : d.Hours);
            double overtimeHours = dailyAttendance.Sum(d => d.Hours > 9 ? d.Hours - 9 : 0);
            double hourlyRate = salary / (22 * 9);  // 22 يوم دوام، 9 ساعات باليوم
            double overtimeAmount = overtimeHours * hourlyRate * 1.25;

            double holidayBonus = 0;
            var holidays = LoadOfficialHolidays();
            holidayBonus = dailyAttendance
                .Where(d => holidays.Contains(d.Date.ToString("yyyy-MM-dd")))
                .Sum(d => d.Hours) * hourlyRate;

            double totalEarnings = salary + overtimeAmount + holidayBonus;
            double totalDeductions = DatabaseHelper.GetEmployeeTotalAdvances(empId);
            double net = totalEarnings - totalDeductions;

            totalSalary = net;

            MessageBox.Show($"الراتب الصافي: {net:0.##} دينار");
        }
        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار موظف");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.FileName = "كشف_الراتب.pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(1200, 1800);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    var emp = (dynamic)cmbEmployees.SelectedItem;
                    int empId = emp.ID;
                    double salary = GetSalary(empId);
                    var empDetails = GetEmployeeDetails(empId);
                    var empRecords = allRecords.Where(r => r.EmployeeId == empId).ToList();
                    var dailyAttendance = GetDailyAttendance(empRecords);

                    var fontTitle = new Font("Segoe UI", 18, FontStyle.Bold);
                    var font14 = new Font("Segoe UI", 14);
                    var font16b = new Font("Segoe UI", 16, FontStyle.Bold);

                    float y = 40;

                    // عنوان الشركة
                    g.DrawString("اسم الشركة", fontTitle, Brushes.Black, 450, y);
                    g.DrawString($"قسائم رواتب الموظفين عن شهر {DateTime.Now:MM} لعام {DateTime.Now:yyyy}", font14, Brushes.Black, 350, y += 50);

                    // بيانات الموظف
                    g.DrawString($"الاسم: {emp.Name}    الرقم الوظيفي: {empId}", font14, Brushes.Black, 50, y += 60);
                    g.DrawString($"المسمى الوظيفي: {empDetails.Title}", font14, Brushes.Black, 50, y += 40);
                    g.DrawString($"الراتب الأساسي: {salary:0.##}    الرقم الوطني: {empDetails.NationalID}    تاريخ التعيين: {empDetails.HireDate}", font14, Brushes.Black, 50, y += 40);

                    // جداول العلاوات والاقتطاعات
                    g.DrawString("علاوات الموظف", font16b, Brushes.Black, 100, y += 60);
                    g.DrawString("النوع", font14, Brushes.Black, 100, y += 30);
                    g.DrawString("القيمة", font14, Brushes.Black, 300, y);
                    g.DrawString("عمل إضافي", font14, Brushes.Black, 100, y += 30);
                    g.DrawString((dailyAttendance.Sum(d => d.Hours > 9 ? d.Hours - 9 : 0) * (salary / (22 * 9)) * 1.25).ToString("0.##"), font14, Brushes.Black, 300, y);
                    g.DrawString("بدل عطل رسمية", font14, Brushes.Black, 100, y += 30);
                    g.DrawString("...", font14, Brushes.Black, 300, y); // قيمة البدل

                    y += 60;
                    g.DrawString("اقتطاعات الموظف", font16b, Brushes.Black, 700, y - 90);
                    g.DrawString("النوع", font14, Brushes.Black, 700, y - 60);
                    g.DrawString("القيمة", font14, Brushes.Black, 900, y - 60);
                    g.DrawString("الخصومات", font14, Brushes.Black, 700, y - 30);
                    g.DrawString("0", font14, Brushes.Black, 900, y - 30);  // خصومات إضافية
                    g.DrawString("مجموع السلف", font14, Brushes.Black, 700, y);
                    g.DrawString(DatabaseHelper.GetEmployeeTotalAdvances(empId).ToString("0.##"), font14, Brushes.Black, 900, y);

                    // الراتب الصافي
                    y += 60;
                    g.DrawString($"صافي الراتب: {totalSalary:0.##} دينار", font16b, Brushes.Black, 50, y);

                    // جدول الحضور
                    y += 80;
                    DrawAttendanceTable(g, dailyAttendance, font14, font16b, ref y);

                    // التوقيع
                    g.DrawString($"أنا الموقع أدناه {emp.Name} أقر بأن التفاصيل صحيحة...", font14, Brushes.Black, 50, y += 40);
                    g.DrawString($"التاريخ: {DateTime.Now:yyyy-MM-dd}                التوقيع: ___________", font14, Brushes.Black, 50, y += 40);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    PdfDocument pdf = new PdfDocument();
                    PdfPage page = pdf.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XImage img = XImage.FromStream(new MemoryStream(ms.ToArray()));
                    gfx.DrawImage(img, 0, 0, page.Width, page.Height);
                    pdf.Save(saveFileDialog.FileName);
                }

                MessageBox.Show("تم تصدير الكشف بنجاح!");
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

        private void DrawAttendanceTable(Graphics g, List<(DateTime Date, DateTime? In, DateTime? Out, double Hours)> dailyAttendance, Font f14, Font f16b, ref float y)
        {
            g.DrawString("سجل حضور الموظف", f16b, Brushes.Black, 50, y);
            y += 40;

            float colW = 130, rowH = 30;
            float x1 = 50, x2 = 650;

            void DrawHeader(float startX, float startY)
            {
                g.DrawRectangle(Pens.Black, startX, startY, colW, rowH);
                g.DrawRectangle(Pens.Black, startX + colW, startY, colW, rowH);
                g.DrawRectangle(Pens.Black, startX + 2 * colW, startY, colW, rowH);
                g.DrawRectangle(Pens.Black, startX + 3 * colW, startY, colW, rowH);
                g.DrawString("التاريخ", f14, Brushes.Black, startX + 10, startY + 5);
                g.DrawString("دخول", f14, Brushes.Black, startX + colW + 10, startY + 5);
                g.DrawString("خروج", f14, Brushes.Black, startX + 2 * colW + 10, startY + 5);
                g.DrawString("عدد الساعات", f14, Brushes.Black, startX + 3 * colW + 10, startY + 5);
            }

            int half = (int)Math.Ceiling(dailyAttendance.Count / 2.0);
            DrawHeader(x1, y);
            DrawHeader(x2, y);
            y += rowH;

            for (int i = 0; i < dailyAttendance.Count; i++)
            {
                var d = dailyAttendance[i];
                float colX = (i < half) ? x1 : x2;
                float rowY = (i < half) ? y + i * rowH : y + (i - half) * rowH;

                g.DrawRectangle(Pens.Black, colX, rowY, colW, rowH);
                g.DrawRectangle(Pens.Black, colX + colW, rowY, colW, rowH);
                g.DrawRectangle(Pens.Black, colX + 2 * colW, rowY, colW, rowH);
                g.DrawRectangle(Pens.Black, colX + 3 * colW, rowY, colW, rowH);

                g.DrawString(d.Date.ToString("yyyy-MM-dd"), f14, Brushes.Black, colX + 5, rowY + 5);
                g.DrawString(d.In?.ToString("HH:mm") ?? "-", f14, Brushes.Black, colX + colW + 5, rowY + 5);
                g.DrawString(d.Out?.ToString("HH:mm") ?? "-", f14, Brushes.Black, colX + 2 * colW + 5, rowY + 5);
                g.DrawString(d.Hours.ToString("0.##"), f14, Brushes.Black, colX + 3 * colW + 5, rowY + 5);
            }

            y += (half * rowH) + 20;
        }
    }

    public class AttendanceRecord
    {
        public int EmployeeId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsIn { get; set; }
    }
}