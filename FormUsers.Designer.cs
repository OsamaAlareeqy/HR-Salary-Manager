namespace Salary_Cal
{
    partial class FormUsers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.gridUsers = new System.Windows.Forms.DataGridView();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.groupBoxUserInfo = new System.Windows.Forms.GroupBox();

            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).BeginInit();
            this.groupBoxUserInfo.SuspendLayout();
            this.SuspendLayout();

            // gridUsers
            this.gridUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUsers.Location = new System.Drawing.Point(30, 250);
            this.gridUsers.Name = "gridUsers";
            this.gridUsers.Size = new System.Drawing.Size(700, 300);
            this.gridUsers.TabIndex = 0;

            // groupBoxUserInfo
            this.groupBoxUserInfo.Controls.Add(this.lblUsername);
            this.groupBoxUserInfo.Controls.Add(this.txtUsername);
            this.groupBoxUserInfo.Controls.Add(this.lblPassword);
            this.groupBoxUserInfo.Controls.Add(this.txtPassword);
            this.groupBoxUserInfo.Controls.Add(this.lblRole);
            this.groupBoxUserInfo.Controls.Add(this.cmbRole);
            this.groupBoxUserInfo.Location = new System.Drawing.Point(30, 30);
            this.groupBoxUserInfo.Name = "groupBoxUserInfo";
            this.groupBoxUserInfo.Size = new System.Drawing.Size(400, 180);
            this.groupBoxUserInfo.Text = "User Information";

            // lblUsername
            this.lblUsername.Text = "Username:";
            this.lblUsername.Location = new System.Drawing.Point(20, 30);
            this.lblUsername.Size = new System.Drawing.Size(80, 24);

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(120, 30);
            this.txtUsername.Size = new System.Drawing.Size(250, 24);

            // lblPassword
            this.lblPassword.Text = "Password:";
            this.lblPassword.Location = new System.Drawing.Point(20, 70);
            this.lblPassword.Size = new System.Drawing.Size(80, 24);

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(120, 70);
            this.txtPassword.Size = new System.Drawing.Size(250, 24);

            // lblRole
            this.lblRole.Text = "Role:";
            this.lblRole.Location = new System.Drawing.Point(20, 110);
            this.lblRole.Size = new System.Drawing.Size(80, 24);

            // cmbRole
            this.cmbRole.Location = new System.Drawing.Point(120, 110);
            this.cmbRole.Size = new System.Drawing.Size(250, 24);

            // btnAddUser
            this.btnAddUser.Text = "Add User";
            this.btnAddUser.Location = new System.Drawing.Point(450, 60);
            this.btnAddUser.Size = new System.Drawing.Size(120, 35);
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);

            // btnDeleteUser
            this.btnDeleteUser.Text = "Delete Selected";
            this.btnDeleteUser.Location = new System.Drawing.Point(450, 110);
            this.btnDeleteUser.Size = new System.Drawing.Size(120, 35);
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);

            // FormUsers
            this.ClientSize = new System.Drawing.Size(780, 600);
            this.Controls.Add(this.gridUsers);
            this.Controls.Add(this.groupBoxUserInfo);
            this.Controls.Add(this.btnAddUser);
            this.Controls.Add(this.btnDeleteUser);
            this.Name = "FormUsers";
            this.Text = "User Management";

            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).EndInit();
            this.groupBoxUserInfo.ResumeLayout(false);
            this.groupBoxUserInfo.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion


        private System.Windows.Forms.DataGridView gridUsers;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.GroupBox groupBoxUserInfo;

    }
}