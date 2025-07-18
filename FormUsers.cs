using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Salary_Cal
{
    public partial class FormUsers : Form
    {
        public FormUsers()
        {
            InitializeComponent();
            LoadUsers();
            cmbRole.Items.AddRange(new string[] { "Admin", "User" });
            cmbRole.SelectedIndex = 0;
        }

        private void LoadUsers()
        {
            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT ID, Username, Role FROM Users", conn);
                var adapter = new SQLiteDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                gridUsers.DataSource = dt;
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();
            string role = cmbRole.SelectedItem.ToString();

            if (user == "" || pass == "")
            {
                MessageBox.Show("يرجى تعبئة جميع الحقول ❗");
                return;
            }

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("INSERT INTO Users (Username, Password, Role) VALUES (@user, @pass, @role)", conn);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@pass", pass);
                cmd.Parameters.AddWithValue("@role", role);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تمت الإضافة ✅");
                    LoadUsers();
                }
                catch
                {
                    MessageBox.Show("اسم المستخدم موجود مسبقًا ❌");
                }
            }
        }
        private void FormUsers_Load(object sender, EventArgs e)
        {

        }
        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count == 0) return;

            string username = gridUsers.SelectedRows[0].Cells["Username"].Value.ToString();

            if (username == "admin")
            {
                MessageBox.Show("لا يمكن حذف المستخدم admin ❌");
                return;
            }

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("DELETE FROM Users WHERE Username = @user", conn);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.ExecuteNonQuery();
                MessageBox.Show("تم الحذف ✅");
                LoadUsers();
            }
        }

        private void gridUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
