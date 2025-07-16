using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Salary_Cal
{
    public partial class FormEmployees : Form
    {
        public FormEmployees()
        {
            InitializeComponent();
        }

        private void FormEmployees_Load(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (var connection = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                connection.Open();
                var query = "SELECT * FROM Employees";
                using (var adapter = new SQLiteDataAdapter(query, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dataGridView1.DataSource = table;

                    // إضافة زر التعديل
                    if (!dataGridView1.Columns.Contains("Edit"))
                    {
                        DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
                        editButton.Name = "Edit";
                        editButton.HeaderText = "تعديل";
                        editButton.Text = "تعديل";
                        editButton.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(editButton);
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Edit"].Index && e.RowIndex >= 0)
            {
                // استخراج بيانات الموظف
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["EmployeeID"].Value);
                string name = dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                string title = dataGridView1.Rows[e.RowIndex].Cells["Title"].Value.ToString();
                string branch = dataGridView1.Rows[e.RowIndex].Cells["Branch"].Value.ToString();
                double salary = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells["Salary"].Value);

                // فتح نموذج التعديل وتمرير البيانات
                EditEmployeeForm editForm = new EditEmployeeForm(id, name, title, branch, salary);
                editForm.FormClosed += (s, args) => LoadEmployees(); // إعادة تحميل بعد التعديل
                editForm.ShowDialog();
            }
        }
    }
}
