using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Salary_Cal
{
    public class EmployeeItem
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
    }

    public partial class FormAdvance : Form
    {
        private ComboBox cmbEmployees;
        private TextBox txtAmount;
        private DateTimePicker dtDate;
        private Button btnSave;
        private Label lblTitle;
        private GroupBox groupBox;
        private ComboBox cmbBranches;

        public FormAdvance()
        {
            InitializeComponent();
            this.Load += FormAdvance_Load;
        }

        private void InitializeComponent()
        {
            this.cmbEmployees = new ComboBox();
            this.txtAmount = new TextBox();
            this.dtDate = new DateTimePicker();
            this.btnSave = new Button();
            this.lblTitle = new Label();
            this.groupBox = new GroupBox();
            this.cmbBranches = new ComboBox();

            this.Text = "إدخال سلفة";
            this.ClientSize = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);


            this.lblTitle.Text = "إدخال بيانات السلفة";
            this.lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Height = 40;


            this.groupBox.Text = "";
            this.groupBox.Size = new Size(340, 320);
            this.groupBox.Location = new Point(30, 50);


            this.cmbBranches.Location = new Point(40, 30);
            this.cmbBranches.Width = 250;
            this.cmbBranches.SelectedIndexChanged += CmbBranches_SelectedIndexChanged;


            this.cmbEmployees.Location = new Point(40, 70);
            this.cmbEmployees.Width = 250;

            this.txtAmount.Location = new Point(40, 110);
            this.txtAmount.Width = 250;
            SetPlaceholder(txtAmount, "قيمة السلفة");


            this.dtDate.Location = new Point(40, 150);
            this.dtDate.Width = 250;
            this.dtDate.Format = DateTimePickerFormat.Short;


            this.btnSave.Text = "حفظ";
            this.btnSave.Size = new Size(250, 35);
            this.btnSave.Location = new Point(40, 190);

            this.btnSave.BackColor = Color.SteelBlue;
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += btnSave_Click;

            this.groupBox.Controls.Add(this.cmbBranches);
            this.groupBox.Controls.Add(this.cmbEmployees);
            this.groupBox.Controls.Add(this.txtAmount);
            this.groupBox.Controls.Add(this.dtDate);
            this.groupBox.Controls.Add(this.btnSave);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.groupBox);

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

        private void FormAdvance_Load(object sender, EventArgs e)
        {
            LoadBranches();
        }

        private void LoadBranches()
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT Id, Name FROM Branches", conn);
                var reader = cmd.ExecuteReader();
                var list = new List<EmployeeItem>();

                while (reader.Read())
                {
                    list.Add(new EmployeeItem
                    {
                        EmployeeID = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }

                cmbBranches.DataSource = list;
                cmbBranches.DisplayMember = "Name";
                cmbBranches.ValueMember = "EmployeeID";
            }
        }

        private void CmbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = (EmployeeItem)cmbBranches.SelectedItem;
            if (selected != null)
            {
                LoadEmployeesByBranch(selected.Name);
            }
        }

        private void LoadEmployeesByBranch(string branchName)
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT EmployeeID, Name FROM Employees WHERE Branch = @branchName", conn);
                cmd.Parameters.AddWithValue("@branchName", branchName);

                var reader = cmd.ExecuteReader();
                var list = new List<EmployeeItem>();

                while (reader.Read())
                {
                    list.Add(new EmployeeItem
                    {
                        EmployeeID = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }

                cmbEmployees.DataSource = list;
                cmbEmployees.DisplayMember = "Name";
                cmbEmployees.ValueMember = "EmployeeID";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null || string.IsNullOrWhiteSpace(txtAmount.Text)) return;

            var selected = (EmployeeItem)cmbEmployees.SelectedItem;
            int empId = selected.EmployeeID;
            decimal amount = decimal.Parse(txtAmount.Text);
            DateTime date = dtDate.Value;

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Advances (EmployeeID, Amount, Date) VALUES (@id, @amount, @date)", conn);
                cmd.Parameters.AddWithValue("@id", empId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("تم حفظ السلفة بنجاح.");
        }
    }
}
