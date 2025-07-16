using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_Cal
{
    public partial class EditEmployeeForm : Form
    {
        private int employeeId;

        public EditEmployeeForm(int id, string name, string title, string branch, double salary)
        {
            InitializeComponent();

            employeeId = id;
            txtName.Text = name;
            txtTitle.Text = title;
            txtBranch.Text = branch;
            txtSalary.Text = salary.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string title = txtTitle.Text.Trim();
            string branch = txtBranch.Text.Trim();
            double salary;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(branch) || !double.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("يرجى إدخال بيانات صحيحة", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var connection = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                connection.Open();
                string query = @"UPDATE Employees 
                         SET Name = @Name, Title = @Title, Branch = @Branch, Salary = @Salary 
                         WHERE EmployeeID = @ID";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Branch", branch);
                    command.Parameters.AddWithValue("@Salary", salary);
                    command.Parameters.AddWithValue("@ID", employeeId);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("تم تحديث البيانات بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("حدث خطأ أثناء التحديث", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void EditEmployeeForm_Load(object sender, EventArgs e)
        {

        }
    }


    
}
