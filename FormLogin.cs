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
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (var conn = new SQLiteConnection("Data Source=employees.db"))
            {
                conn.Open();
                string query = "SELECT Role FROM Users WHERE Username = @user AND Password = @pass";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string role = result.ToString();
                        FormMainMenu main = new FormMainMenu(username, role); // تمرير المستخدم والصلاحية
                        main.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("بيانات الدخول غير صحيحة ❌");
                    }
                }
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
