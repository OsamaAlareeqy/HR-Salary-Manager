namespace Salary_Cal
{
    partial class FormMainMenu
    {
        private System.ComponentModel.IContainer components = null;

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
            this.btnEmployeeInfo = new System.Windows.Forms.Button();
            this.btnAttendance = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnAdvance = new System.Windows.Forms.Button();
            this.btnSalaryCalc = new System.Windows.Forms.Button();
            this.btnUserManagement = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnEmployeeInfo
            // 
            this.btnEmployeeInfo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnEmployeeInfo.Location = new System.Drawing.Point(300, 80);
            this.btnEmployeeInfo.Name = "btnEmployeeInfo";
            this.btnEmployeeInfo.Size = new System.Drawing.Size(200, 40);
            this.btnEmployeeInfo.TabIndex = 1;
            this.btnEmployeeInfo.Text = "إدارة الموظفين";
            this.btnEmployeeInfo.UseVisualStyleBackColor = true;
            this.btnEmployeeInfo.Click += new System.EventHandler(this.btnEmployeeInfo_Click);
            // 
            // btnAttendance
            // 
            this.btnAttendance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnAttendance.Location = new System.Drawing.Point(300, 180);
            this.btnAttendance.Name = "btnAttendance";
            this.btnAttendance.Size = new System.Drawing.Size(200, 40);
            this.btnAttendance.TabIndex = 3;
            this.btnAttendance.Text = "سجل الحضور";
            this.btnAttendance.UseVisualStyleBackColor = true;
            this.btnAttendance.Click += new System.EventHandler(this.btnAttendance_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.IndianRed;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(300, 390);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(200, 45);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "إنهاء البرنامج";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSettings.Location = new System.Drawing.Point(300, 330);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(200, 40);
            this.btnSettings.TabIndex = 6;
            this.btnSettings.Text = "الإعدادات";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnAdvance
            // 
            this.btnAdvance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnAdvance.Location = new System.Drawing.Point(300, 230);
            this.btnAdvance.Name = "btnAdvance";
            this.btnAdvance.Size = new System.Drawing.Size(200, 40);
            this.btnAdvance.TabIndex = 4;
            this.btnAdvance.Text = "إدخال السلف";
            this.btnAdvance.UseVisualStyleBackColor = true;
            this.btnAdvance.Click += new System.EventHandler(this.btnAdvance_Click);
            // 
            // btnSalaryCalc
            // 
            this.btnSalaryCalc.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSalaryCalc.Location = new System.Drawing.Point(300, 130);
            this.btnSalaryCalc.Name = "btnSalaryCalc";
            this.btnSalaryCalc.Size = new System.Drawing.Size(200, 40);
            this.btnSalaryCalc.TabIndex = 2;
            this.btnSalaryCalc.Text = "حساب الرواتب";
            this.btnSalaryCalc.UseVisualStyleBackColor = true;
            this.btnSalaryCalc.Click += new System.EventHandler(this.btnSalaryCalc_Click);
            // 
            // btnUserManagement
            // 
            this.btnUserManagement.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnUserManagement.Location = new System.Drawing.Point(300, 280);
            this.btnUserManagement.Name = "btnUserManagement";
            this.btnUserManagement.Size = new System.Drawing.Size(200, 40);
            this.btnUserManagement.TabIndex = 5;
            this.btnUserManagement.Text = "إدارة المستخدمين";
            this.btnUserManagement.UseVisualStyleBackColor = true;
            this.btnUserManagement.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(260, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(297, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "القائمة الرئيسية للنظام";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnEmployeeInfo);
            this.Controls.Add(this.btnSalaryCalc);
            this.Controls.Add(this.btnAttendance);
            this.Controls.Add(this.btnAdvance);
            this.Controls.Add(this.btnUserManagement);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "نظام إدارة الرواتب - القائمة الرئيسية";
            this.Load += new System.EventHandler(this.FormMainMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEmployeeInfo;
        private System.Windows.Forms.Button btnAttendance;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnAdvance;
        private System.Windows.Forms.Button btnSalaryCalc;
        private System.Windows.Forms.Button btnUserManagement;
        private System.Windows.Forms.Label lblTitle;
    }
}
