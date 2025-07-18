using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Salary_Cal
{
    public partial class FormAdvance : Form
    {



        public FormAdvance()
        {
            InitializeComponent();
        }
        #region
        private void InitializeComponent()
        {
            this.txtEmployeeID = new System.Windows.Forms.TextBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();

            // FormAdvance
            this.Text = "إدخال سلفة";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            // lblTitle
            this.lblTitle.Text = "إدخال بيانات السلفة";
            this.lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Height = 40;

            // groupBox
            this.groupBox.Text = "";
            this.groupBox.Size = new Size(340, 200);
            this.groupBox.Location = new Point(30, 50);

            // txtEmployeeID
            this.txtEmployeeID.Size = new Size(250, 30);
            this.txtEmployeeID.Location = new Point(40, 30);
            SetPlaceholder(txtEmployeeID, "رقم الموظف");

            // txtAmount
            this.txtAmount.Size = new Size(250, 30);
            this.txtAmount.Location = new Point(40, 70);
            SetPlaceholder(txtAmount, "قيمة السلفة");

            // dtDate
            this.dtDate.Size = new Size(250, 30);
            this.dtDate.Location = new Point(40, 110);
            this.dtDate.Format = DateTimePickerFormat.Short;

            // btnSave
            this.btnSave.Text = "حفظ";
            this.btnSave.Size = new Size(250, 35);
            this.btnSave.Location = new Point(40, 150);
            this.btnSave.BackColor = Color.SteelBlue;
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // إضافة إلى النموذج
            this.groupBox.Controls.Add(this.txtEmployeeID);
            this.groupBox.Controls.Add(this.txtAmount);
            this.groupBox.Controls.Add(this.dtDate);
            this.groupBox.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.groupBox);

            this.ResumeLayout(false);
        }

        #endregion
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

        private TextBox txtEmployeeID;
        private TextBox txtAmount;
        private DateTimePicker dtDate;
        private Button btnSave;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBox;
    }
}
