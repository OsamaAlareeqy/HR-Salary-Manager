// FormSalaryCalculation.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Salary_Cal
{
    public partial class FormSalaryCalculation : Form
    {
        private ComboBox cmbEmployees;
        private DateTimePicker dtMonth;
        private Button btnCalculate;
        private DataGridView gridSalary;
        private Label lblTotal;
        private Button btnExport;

        public FormSalaryCalculation()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void InitializeComponent()
        {
            this.cmbEmployees = new System.Windows.Forms.ComboBox();
            this.dtMonth = new System.Windows.Forms.DateTimePicker();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.gridSalary = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridSalary)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbEmployees
            // 
            this.cmbEmployees.Location = new System.Drawing.Point(30, 30);
            this.cmbEmployees.Name = "cmbEmployees";
            this.cmbEmployees.Size = new System.Drawing.Size(200, 24);
            this.cmbEmployees.TabIndex = 0;
            // 
            // dtMonth
            // 
            this.dtMonth.CustomFormat = "yyyy-MM";
            this.dtMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMonth.Location = new System.Drawing.Point(250, 30);
            this.dtMonth.Name = "dtMonth";
            this.dtMonth.ShowUpDown = true;
            this.dtMonth.Size = new System.Drawing.Size(200, 22);
            this.dtMonth.TabIndex = 1;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(474, 29);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(75, 23);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "حساب الراتب";
            this.btnCalculate.Click += new System.EventHandler(this.BtnCalculate_Click);
            // 
            // gridSalary
            // 
            this.gridSalary.ColumnHeadersHeight = 29;
            this.gridSalary.Location = new System.Drawing.Point(30, 80);
            this.gridSalary.Name = "gridSalary";
            this.gridSalary.RowHeadersWidth = 51;
            this.gridSalary.Size = new System.Drawing.Size(740, 300);
            this.gridSalary.TabIndex = 3;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(30, 400);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(0, 28);
            this.lblTotal.TabIndex = 4;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(650, 400);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "تصدير PDF";
            // 
            // FormSalaryCalculation
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmbEmployees);
            this.Controls.Add(this.dtMonth);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.gridSalary);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnExport);
            this.Name = "FormSalaryCalculation";
            this.Text = "حساب الرواتب";
            this.Load += new System.EventHandler(this.FormSalaryCalculation_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.gridSalary)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void LoadEmployees()
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                string query = "SELECT EmployeeID, Name FROM Employees";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
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
                cmbEmployees.ValueMember = "ID";
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null) return;
            dynamic selected = cmbEmployees.SelectedItem;
            int empId = selected.ID;
            string month = dtMonth.Value.ToString("yyyy-MM");

            double baseSalary = 0;
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();

                var cmd = new SQLiteCommand("SELECT Salary FROM Employees WHERE EmployeeID = @id", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                baseSalary = Convert.ToDouble(cmd.ExecuteScalar());
            }

            var records = LoadAttendance(empId, month);
            double regularHours = records.Sum(r => r.Regular);
            double overtime = records.Sum(r => r.Overtime);
            double holidayOver = records.Sum(r => r.HolidayOver);

            double hourlyRate = baseSalary / (30 * 9); // assuming 9 hrs per 30 days
            double salary = (regularHours * hourlyRate) + (overtime * hourlyRate * 1.25) + (holidayOver * hourlyRate * 1.5);

            double advances = LoadAdvances(empId, month);
            double net = salary - advances;

            gridSalary.DataSource = new[]
            {
                new {
                    الموظف = selected.Name,
                    الشهر = month,
                    ساعات_عادية = regularHours,
                    إضافي = overtime,
                    رسمي = holidayOver,
                    الراتب_الأساسي = baseSalary,
                    مجموع_السلف = advances,
                    الصافي = net
                }
            };

            lblTotal.Text = $"صافي الراتب: {net:0.00} دينار";

        }

        private class Record
        {
            public double Regular { get; set; }
            public double Overtime { get; set; }
            public double HolidayOver { get; set; }
        }

        private List<Record> LoadAttendance(int empId, string month)
        {
            var list = new List<Record>();
            string monthStart = month + "-01";
            DateTime start = DateTime.Parse(monthStart);
            DateTime end = start.AddMonths(1);

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Timestamp, IsIn, IsOut FROM Attendance WHERE EmployeeID = @id AND Timestamp >= @start AND Timestamp < @end", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                using (var reader = cmd.ExecuteReader())
                {
                    DateTime? lastIn = null;
                    while (reader.Read())
                    {
                        DateTime ts = DateTime.Parse(reader["Timestamp"].ToString());
                        bool isIn = reader.GetBoolean(1);
                        bool isOut = reader.GetBoolean(2);

                        if (isIn)
                        {
                            lastIn = ts;
                        }
                        else if (lastIn.HasValue)
                        {
                            var dur = ts - lastIn.Value;
                            var h = dur.TotalHours;

                            bool isHoliday = DatabaseHelper.IsHoliday(lastIn.Value);

                            list.Add(new Record
                            {
                                Regular = isHoliday ? 0 : Math.Min(h, 9),
                                Overtime = isHoliday ? 0 : Math.Max(0, h - 9),
                                HolidayOver = isHoliday ? h : 0
                            });
                            lastIn = null;
                        }
                    }
                }
            }

            return list;
        }

        private double LoadAdvances(int empId, string month)
        {
            double sum = 0;
            string start = month + "-01";
            DateTime from = DateTime.Parse(start);
            DateTime to = from.AddMonths(1);

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Amount FROM Advances WHERE EmployeeID = @id AND Date >= @from AND Date < @to", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sum += Convert.ToDouble(reader["Amount"]);
                    }
                }
            }

            return sum;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (gridSalary.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files|*.pdf";
            sfd.FileName = "تقرير الراتب.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var doc = new Document())
                {
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    PdfPTable table = new PdfPTable(gridSalary.Columns.Count);

                    foreach (DataGridViewColumn col in gridSalary.Columns)
                        table.AddCell(new Phrase(col.HeaderText));

                    foreach (DataGridViewRow row in gridSalary.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                            table.AddCell(cell.Value?.ToString());
                    }

                    doc.Add(table);
                    doc.Close();
                }

                MessageBox.Show("تم التصدير بنجاح ✅");
            }
        }
    }
}
