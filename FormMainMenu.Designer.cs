namespace Salary_Cal
{
    partial class FormMainMenu
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEmployeeInfo = new System.Windows.Forms.Button();
            this.btnAttendance = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnAdvance = new System.Windows.Forms.Button();
            this.btnSalaryCalc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEmployeeInfo
            // 
            this.btnEmployeeInfo.Location = new System.Drawing.Point(363, 85);
            this.btnEmployeeInfo.Name = "btnEmployeeInfo";
            this.btnEmployeeInfo.Size = new System.Drawing.Size(165, 23);
            this.btnEmployeeInfo.TabIndex = 0;
            this.btnEmployeeInfo.Text = "إدارة الموظفين ";
            this.btnEmployeeInfo.UseVisualStyleBackColor = true;
            this.btnEmployeeInfo.Click += new System.EventHandler(this.btnEmployeeInfo_Click);
            // 
            // btnAttendance
            // 
            this.btnAttendance.Location = new System.Drawing.Point(299, 214);
            this.btnAttendance.Name = "btnAttendance";
            this.btnAttendance.Size = new System.Drawing.Size(139, 23);
            this.btnAttendance.TabIndex = 1;
            this.btnAttendance.Text = "سجل الحضور";
            this.btnAttendance.UseVisualStyleBackColor = true;
            this.btnAttendance.Click += new System.EventHandler(this.btnAttendance_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(420, 326);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(163, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "إنهاء البرنامج";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(691, 156);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.Text = "الإعدادات";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnAdvance
            // 
            this.btnAdvance.Location = new System.Drawing.Point(565, 257);
            this.btnAdvance.Name = "btnAdvance";
            this.btnAdvance.Size = new System.Drawing.Size(170, 23);
            this.btnAdvance.TabIndex = 4;
            this.btnAdvance.Text = "إدخال السلف";
            this.btnAdvance.UseVisualStyleBackColor = true;
            this.btnAdvance.Click += new System.EventHandler(this.btnAdvance_Click);
            // 
            // btnSalaryCalc
            // 
            this.btnSalaryCalc.Location = new System.Drawing.Point(130, 118);
            this.btnSalaryCalc.Name = "btnSalaryCalc";
            this.btnSalaryCalc.Size = new System.Drawing.Size(163, 23);
            this.btnSalaryCalc.TabIndex = 5;
            this.btnSalaryCalc.Text = "حساب ";
            this.btnSalaryCalc.UseVisualStyleBackColor = true;
            this.btnSalaryCalc.Click += new System.EventHandler(this.btnSalaryCalc_Click);
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSalaryCalc);
            this.Controls.Add(this.btnAdvance);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnAttendance);
            this.Controls.Add(this.btnEmployeeInfo);
            this.Name = "FormMainMenu";
            this.Text = "FormMainMenu";
            this.Load += new System.EventHandler(this.FormMainMenu_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEmployeeInfo;
        private System.Windows.Forms.Button btnAttendance;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnAdvance;
        private System.Windows.Forms.Button btnSalaryCalc;

    }
}