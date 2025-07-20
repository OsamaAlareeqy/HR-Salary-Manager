using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Salary_Cal
{
    public partial class FormEmployeeInfo : Form
    {
        public FormEmployeeInfo()
        {
            InitializeComponent();
            ApplyCustomStyles();
        }

        private void ApplyCustomStyles()
        {
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            lblTitle.Text = "قائمة الموظفين";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

            btnAddEmployee.BackColor = System.Drawing.Color.SteelBlue;
            btnAddEmployee.ForeColor = System.Drawing.Color.White;

            btnUpdateEmployee.BackColor = System.Drawing.Color.SteelBlue;
            btnUpdateEmployee.ForeColor = System.Drawing.Color.White;

            btnExportExcel.BackColor = System.Drawing.Color.ForestGreen;
            btnExportExcel.ForeColor = System.Drawing.Color.White;

            btnExportPdf.BackColor = System.Drawing.Color.Firebrick;
            btnExportPdf.ForeColor = System.Drawing.Color.White;

            btnSearch.BackColor = System.Drawing.Color.DimGray;
            btnSearch.ForeColor = System.Drawing.Color.White;

            dataGridEmployees.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridEmployees.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dataGridEmployees.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
        }

        private void FormEmployeeInfo_Load(object sender, EventArgs e)
        {
            DatabaseHelper.InitializeDatabase();

            var branches = DatabaseHelper.GetAllBranches();

            if (branches.Count == 0)
            {
                branches = new List<string> {
                   "الياسمين 3",
                   "الياسمين 1",
                   "الاذاعة"

                };

                foreach (var branch in branches)
                {
                    DatabaseHelper.AddBranch(branch);
                }
            }

            cmbBranch.Items.Clear();
            cmbBranch.Items.AddRange(branches.ToArray());

            if (cmbBranch.Items.Count > 0)
                cmbBranch.SelectedIndex = 0;

            cmbSearchBy.Items.AddRange(new string[] { "ID", "الاسم", "الفرع", "المسمى الوظيفي", "الراتب" });
            cmbSearchBy.SelectedIndex = 0;

            LoadEmployees();
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtID.Text.Trim());
                string name = txtName.Text.Trim();
                string title = txtTitle.Text.Trim();
                string branch = cmbBranch.SelectedItem?.ToString();
                double salary = double.Parse(txtSalary.Text.Trim());
                string nationalId = txtNationalID.Text.Trim();
                string hireDate = dtpHireDate.Value.ToString("yyyy-MM-dd");


                DatabaseHelper.AddEmployee( id,  name,  title,  branch,  salary,  nationalId,  hireDate);

                txtID.Clear();
                txtName.Clear();
                txtTitle.Clear();
                txtSalary.Clear();
                txtNationalID.Clear();
                
                cmbBranch.SelectedIndex = 0;

                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء الإضافة: " + ex.Message);
            }
        }

        private void btnAddBranch_Click(object sender, EventArgs e)
        {
            string newBranch = Interaction.InputBox("ادخل اسم الفرع الجديد:", "إضافة فرع");

            if (!string.IsNullOrWhiteSpace(newBranch) && !cmbBranch.Items.Contains(newBranch))
            {
                cmbBranch.Items.Add(newBranch);
                cmbBranch.SelectedItem = newBranch;
            }
            DatabaseHelper.AddBranch(newBranch);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchBy = cmbSearchBy.SelectedItem.ToString();
            string value = txtSearch.Text.Trim();

            string column = null;
            if (searchBy == "ID")
                column = "EmployeeID";
            else if (searchBy == "الاسم")
                column = "Name";
            else if (searchBy == "الفرع")
                column = "Branch";
            else if (searchBy == "المسمى الوظيفي")
                column = "Title";
            else if (searchBy == "الراتب")
                column = "Salary";

            LoadEmployees(column, value);
        }

        private void LoadEmployees(string filterColumn = null, string filterValue = null)
        {
            using (var connection = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                connection.Open();
                string query = "SELECT * FROM Employees";

                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                {
                    query += $" WHERE {filterColumn} LIKE @value";
                }

                using (var command = new SQLiteCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(filterColumn))
                    {
                        command.Parameters.AddWithValue("@value", $"%{filterValue}%");
                    }

                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        dataGridEmployees.DataSource = table;
                    }
                }
            }
        }

        private void dataGridEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridEmployees.Rows[e.RowIndex];

                txtID.Text = row.Cells["EmployeeID"].Value.ToString();
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtTitle.Text = row.Cells["Title"].Value.ToString();
                cmbBranch.SelectedItem = row.Cells["Branch"].Value.ToString();
                txtSalary.Text = row.Cells["Salary"].Value.ToString();
                
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dataGridEmployees.Rows.Count == 0)
            {
                MessageBox.Show("لا يوجد بيانات للتصدير.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.FileName = "الموظفين.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("الموظفين");

                    for (int i = 0; i < dataGridEmployees.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dataGridEmployees.Columns[i].HeaderText;
                    }

                    for (int i = 0; i < dataGridEmployees.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridEmployees.Columns.Count; j++)
                        {
                            worksheet.Cell(i + 2, j + 1).Value = dataGridEmployees.Rows[i].Cells[j].Value?.ToString();
                        }
                    }

                    workbook.SaveAs(sfd.FileName);
                }

                MessageBox.Show("تم التصدير إلى Excel بنجاح.");
            }
        }
        private void btnUpdateEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtID.Text.Trim());
                string name = txtName.Text.Trim();
                string title = txtTitle.Text.Trim();
                string branch = cmbBranch.SelectedItem?.ToString();
                double salary = double.Parse(txtSalary.Text.Trim());

                using (var connection = new SQLiteConnection("Data Source=employees.db;Version=3;"))
                {
                    connection.Open();
                    string query = "UPDATE Employees SET Name = @name, Title = @title, Branch = @branch, Salary = @salary WHERE EmployeeID = @id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@branch", branch);
                        command.Parameters.AddWithValue("@salary", salary);
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("تم تعديل بيانات الموظف بنجاح.");
                            LoadEmployees();
                        }
                        else
                        {
                            MessageBox.Show("لم يتم العثور على موظف بهذا الرقم.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التعديل: " + ex.Message);
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (dataGridEmployees.Rows.Count == 0)
            {
                MessageBox.Show("لا يوجد بيانات للتصدير.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files|*.pdf";
            sfd.FileName = "الموظفين.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var doc = new Document(PageSize.A4.Rotate()))
                {
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    PdfPTable table = new PdfPTable(dataGridEmployees.Columns.Count);

                    foreach (DataGridViewColumn column in dataGridEmployees.Columns)
                    {
                        table.AddCell(new Phrase(column.HeaderText));
                    }

                    foreach (DataGridViewRow row in dataGridEmployees.Rows)
                    {
                        if (row.IsNewRow) continue;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            table.AddCell(new Phrase(cell.Value?.ToString()));
                        }
                    }

                    doc.Add(table);
                    doc.Close();
                }

                MessageBox.Show("تم التصدير إلى PDF بنجاح.");
            }
        }
    }
}
