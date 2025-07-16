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
    public partial class FormSettings : Form
    {
        private DataGridView dataGridViewHolidays;
        private Button btnAddHoliday;
        private Button btnDeleteSelected;
        private Button btnLoadDefault;
        private DateTimePicker dateTimePicker;
        private TextBox txtDescription;
        private Label lblDate;
        private Label lblDescription;

        public FormSettings()
        {
            InitializeComponent();
            LoadHolidays();
        }

        private void InitializeComponent()
        {
            this.dataGridViewHolidays = new DataGridView();
            this.btnAddHoliday = new Button();
            this.btnDeleteSelected = new Button();
            this.btnLoadDefault = new Button();
            this.dateTimePicker = new DateTimePicker();
            this.txtDescription = new TextBox();
            this.lblDate = new Label();
            this.lblDescription = new Label();
            this.SuspendLayout();

            // FormSettings Styling
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // dataGridViewHolidays
            this.dataGridViewHolidays.Location = new Point(20, 150);
            this.dataGridViewHolidays.Size = new Size(510, 230);
            this.dataGridViewHolidays.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewHolidays.BackgroundColor = Color.WhiteSmoke;

            // dateTimePicker
            this.dateTimePicker.Location = new Point(130, 20);
            this.dateTimePicker.Format = DateTimePickerFormat.Short;
            this.dateTimePicker.Width = 150;

            // txtDescription
            this.txtDescription.Location = new Point(130, 60);
            this.txtDescription.Size = new Size(150, 22);

            // lblDate
            this.lblDate.Text = "تاريخ العطلة:";
            this.lblDate.Location = new Point(30, 20);
            this.lblDate.AutoSize = true;

            // lblDescription
            this.lblDescription.Text = "الوصف:";
            this.lblDescription.Location = new Point(30, 60);
            this.lblDescription.AutoSize = true;

            // btnAddHoliday
            this.btnAddHoliday.Text = "إضافة عطلة";
            this.btnAddHoliday.Location = new Point(300, 20);
            this.btnAddHoliday.Size = new Size(150, 30);
            this.btnAddHoliday.BackColor = Color.SteelBlue;
            this.btnAddHoliday.ForeColor = Color.White;
            this.btnAddHoliday.FlatStyle = FlatStyle.Flat;
            this.btnAddHoliday.Click += new EventHandler(this.btnAddHoliday_Click);

            // btnDeleteSelected
            this.btnDeleteSelected.Text = "حذف المحددة";
            this.btnDeleteSelected.Location = new Point(300, 60);
            this.btnDeleteSelected.Size = new Size(150, 30);
            this.btnDeleteSelected.BackColor = Color.Firebrick;
            this.btnDeleteSelected.ForeColor = Color.White;
            this.btnDeleteSelected.FlatStyle = FlatStyle.Flat;
            this.btnDeleteSelected.Click += new EventHandler(this.btnDeleteSelected_Click);

            // btnLoadDefault
            this.btnLoadDefault.Text = "تحميل عطل السنة";
            this.btnLoadDefault.Location = new Point(160, 100);
            this.btnLoadDefault.Size = new Size(180, 30);
            this.btnLoadDefault.BackColor = Color.ForestGreen;
            this.btnLoadDefault.ForeColor = Color.White;
            this.btnLoadDefault.FlatStyle = FlatStyle.Flat;
            this.btnLoadDefault.Click += new EventHandler(this.btnLoadDefault_Click);

            // FormSettings
            this.ClientSize = new Size(560, 410);
            this.Controls.Add(this.dataGridViewHolidays);
            this.Controls.Add(this.btnAddHoliday);
            this.Controls.Add(this.btnDeleteSelected);
            this.Controls.Add(this.btnLoadDefault);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblDescription);
            this.Text = "إعدادات العطل الرسمية";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadHolidays()
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db;Version=3;"))
            {
                conn.Open();
                string query = "SELECT * FROM Holidays ORDER BY Date";
                using (var adapter = new SQLiteDataAdapter(query, conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewHolidays.DataSource = table;
                }
            }
        }

        private void btnAddHoliday_Click(object sender, EventArgs e)
        {
            string date = dateTimePicker.Value.ToString("yyyy-MM-dd");
            string desc = txtDescription.Text.Trim();
            if (string.IsNullOrWhiteSpace(desc))
            {
                MessageBox.Show("أدخل وصف العطلة");
                return;
            }
            DatabaseHelper.AddHoliday(date, desc);
            LoadHolidays();
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (dataGridViewHolidays.CurrentRow != null)
            {
                string date = dataGridViewHolidays.CurrentRow.Cells["Date"].Value.ToString();
                using (var conn = new SQLiteConnection("Data Source=employees.db;Version=3;"))
                {
                    conn.Open();
                    string query = "DELETE FROM Holidays WHERE Date = @date";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadHolidays();
            }
        }

        private void btnLoadDefault_Click(object sender, EventArgs e)
        {
            int year = dateTimePicker.Value.Year;
            DatabaseHelper.AddHoliday($"{year}-01-01", "رأس السنة الميلادية");
            DatabaseHelper.AddHoliday($"{year}-05-01", "عيد العمال");
            DatabaseHelper.AddHoliday($"{year}-05-25", "عيد الاستقلال");
            DatabaseHelper.AddHoliday($"{year}-12-25", "عيد الميلاد المجيد");
            LoadHolidays();
        }
    }
}
