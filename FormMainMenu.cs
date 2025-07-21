using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Salary_Cal.gridAttendance;

namespace Salary_Cal
{
    public partial class FormMainMenu : Form
    {
        string currentUser;
        string currentRole;

        public FormMainMenu(string username, string role)
        {
            InitializeComponent();
            currentUser = username;
            currentRole = role;


            if (currentRole != "Admin")
            {
                btnEmployeeInfo.Enabled = false;
                btnSalaryCalc.Enabled = false;
            }
        }


        private void FormMainMenu_Load(object sender, EventArgs e)
        {

        }

        private void btnEmployeeInfo_Click(object sender, EventArgs e)
        {
            new FormEmployeeInfo().ShowDialog();
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            new gridAttendance().ShowDialog();
           
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            new FormSettings().ShowDialog();

        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            new FormAdvance().ShowDialog();
        }
        private void btnSalaryCalc_Click(object sender, EventArgs e)
        {
            var summary = new EmployeeWorkSummary(); 
            var salaryForm = new FormSalaryCalculation(summary);
            salaryForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormUsers().ShowDialog();
        }
    }
}
