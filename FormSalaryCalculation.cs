using PdfSharp.Drawing;
using System;
using System.Data.SQLite;
using System.Windows.Forms;
using PdfSharp.Pdf;
namespace Salary_Cal
{
    public partial class FormSalaryCalculation : Form
    {
        public FormSalaryCalculation()
        {
            InitializeComponent();
            LoadEmployees();
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
            if (cmbEmployees.SelectedItem == null) return;

            dynamic selectedEmp = cmbEmployees.SelectedItem;
            int empId = selectedEmp.ID;

            DateTime start = new DateTime(dtMonth.Value.Year, dtMonth.Value.Month, 1);
            DateTime end = start.AddMonths(1);

            double hoursWorked = 0;
            double overtime = 0;

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT TimeIn, TimeOut FROM Attendance WHERE EmployeeID = @id AND TimeIn >= @start AND TimeIn < @end", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var timeIn = DateTime.Parse(reader["TimeIn"].ToString());
                    var timeOut = DateTime.Parse(reader["TimeOut"].ToString());
                    var duration = (timeOut - timeIn).TotalHours;

                    if (duration <= 9)
                        hoursWorked += duration;
                    else
                    {
                        hoursWorked += 9;
                        overtime += (duration - 9);
                    }
                }
            }

            // حساب الراتب
            double monthlySalary = GetEmployeeSalary(empId);
            double hourlyRate = monthlySalary / (30 * 9);
            double total = (hoursWorked * hourlyRate) + (overtime * hourlyRate * 1.25);

            txtResult.Text = $"الموظف: {selectedEmp.Name}\r\nالساعات العادية: {hoursWorked:0.##}\r\nالساعات الإضافية: {overtime:0.##}\r\nالراتب: {total:0.##} دينار";
        }

        private double GetEmployeeSalary(int empId)
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Salary FROM Employees WHERE EmployeeID = @id", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                return Convert.ToDouble(cmd.ExecuteScalar());
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null || string.IsNullOrWhiteSpace(txtResult.Text))
            {
                MessageBox.Show("يرجى حساب الراتب أولاً.");
                return;
            }

            dynamic emp = cmbEmployees.SelectedItem;
            string fileName = $"كشف_راتب_{emp.Name}_{dtMonth.Value:yyyy_MM}.pdf";

            using (PdfDocument doc = new PdfDocument())
            {
                doc.Info.Title = "كشف الراتب";
                PdfPage page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                var fontOptions = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
                XFont titleFont = new XFont("Tahoma", 18, XFontStyleEx.Bold, fontOptions);
                XFont regularFont = new XFont("Tahoma", 14, XFontStyleEx.Regular, fontOptions);



                int y = 50;

                gfx.DrawString("كشف الراتب الشهري", titleFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.Center);
                y += 50;

                gfx.DrawString($"اسم الموظف: {emp.Name}", regularFont, XBrushes.Black, new XPoint(40, y));
                y += 30;

                gfx.DrawString($"الشهر: {dtMonth.Value.ToString("MMMM yyyy")}", regularFont, XBrushes.Black, new XPoint(40, y));
                y += 30;

                string[] lines = txtResult.Text.Split('\n');
                foreach (string line in lines)
                {
                    gfx.DrawString(line.Trim(), regularFont, XBrushes.Black, new XPoint(40, y));
                    y += 25;
                }

                // حفظ الملف
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.FileName = fileName;
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    doc.Save(saveDialog.FileName);
                    MessageBox.Show("تم تصدير الكشف بنجاح!");
                }
            }
        }
    }
}
