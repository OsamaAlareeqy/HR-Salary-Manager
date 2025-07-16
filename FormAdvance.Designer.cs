using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Salary_Cal
{
    public partial class FormAdvance : Form
    {
        private TextBox txtEmployeeID;
        private TextBox txtAmount;
        private DateTimePicker dtDate;
        private Button btnSave;

        public FormAdvance()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtEmployeeID = new System.Windows.Forms.TextBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // txtEmployeeID
            this.txtEmployeeID.Location = new System.Drawing.Point(50, 30);
            SetPlaceholder(txtEmployeeID, "رقم الموظف");

            // txtAmount
            this.txtAmount.Location = new System.Drawing.Point(50, 70);
            SetPlaceholder(txtAmount, "قيمة السلفة");

            // dtDate
            this.dtDate.Location = new System.Drawing.Point(50, 110);

            // btnSave
            this.btnSave.Text = "حفظ";
            this.btnSave.Location = new System.Drawing.Point(50, 150);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // FormAdvance
            this.Controls.Add(this.txtEmployeeID);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.dtDate);
            this.Controls.Add(this.btnSave);
            this.Text = "إدخال سلفة";
            this.ResumeLayout(false);
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtEmployeeID.Text, out int empId) || !double.TryParse(txtAmount.Text, out double amount))
            {
                MessageBox.Show("يرجى إدخال بيانات صحيحة.");
                return;
            }

            string empName = GetEmployeeName(empId);

            using (var conn = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Advances (EmployeeID, Amount, Date) VALUES (@id, @amount, @date)", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@date", dtDate.Value);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"تم حفظ السلفة بنجاح للموظف: {empName}");
        }

        private string GetEmployeeName(int empId)
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Name FROM Employees WHERE EmployeeID = @id", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                var result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "اسم غير معروف";
            }
        }
    }
}
