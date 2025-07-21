using System.Drawing;
using System.Windows.Forms;

namespace Salary_Cal
{
    partial class FormSalaryCalculation
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
            this.cmbEmployees = new System.Windows.Forms.ComboBox();
            this.dtMonth = new System.Windows.Forms.DateTimePicker();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnExportPDF = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.lblMonth = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtRegularHours = new System.Windows.Forms.TextBox();
            this.txtOvertimeHours = new System.Windows.Forms.TextBox();
            this.txtHolidayHours = new System.Windows.Forms.TextBox();
            this.txtMonthlySalary = new System.Windows.Forms.TextBox();
            this.txtCalculatedSalary = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbEmployees
            // 
            this.cmbEmployees.Location = new System.Drawing.Point(140, 36);
            this.cmbEmployees.Name = "cmbEmployees";
            this.cmbEmployees.Size = new System.Drawing.Size(200, 31);
            this.cmbEmployees.TabIndex = 1;
            // 
            // dtMonth
            // 
            this.dtMonth.CustomFormat = "MMMM yyyy";
            this.dtMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMonth.Location = new System.Drawing.Point(140, 76);
            this.dtMonth.Name = "dtMonth";
            this.dtMonth.ShowUpDown = true;
            this.dtMonth.Size = new System.Drawing.Size(200, 30);
            this.dtMonth.TabIndex = 3;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(30, 130);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(120, 32);
            this.btnCalculate.TabIndex = 4;
            this.btnCalculate.Text = "احسب الراتب";
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(261, 132);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(120, 30);
            this.txtResult.TabIndex = 6;
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnExportPDF.Location = new System.Drawing.Point(12, 406);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(140, 32);
            this.btnExportPDF.TabIndex = 1;
            this.btnExportPDF.Text = " PDF  تصدير إلى ";
            this.btnExportPDF.UseVisualStyleBackColor = false;
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.lblEmployee);
            this.groupBox.Controls.Add(this.cmbEmployees);
            this.groupBox.Controls.Add(this.lblMonth);
            this.groupBox.Controls.Add(this.dtMonth);
            this.groupBox.Controls.Add(this.btnCalculate);
            this.groupBox.Controls.Add(this.lblResult);
            this.groupBox.Controls.Add(this.txtResult);
            this.groupBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox.Location = new System.Drawing.Point(30, 20);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(540, 200);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "بيانات الحساب";
            // 
            // lblEmployee
            // 
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(30, 40);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(108, 23);
            this.lblEmployee.TabIndex = 0;
            this.lblEmployee.Text = "اسم الموظف:";
            // 
            // lblMonth
            // 
            this.lblMonth.AutoSize = true;
            this.lblMonth.Location = new System.Drawing.Point(30, 80);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(55, 23);
            this.lblMonth.TabIndex = 2;
            this.lblMonth.Text = "الشهر:";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(402, 139);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(121, 23);
            this.lblResult.TabIndex = 5;
            this.lblResult.Text = " : الراتب النهائي";
            // 
            // txtRegularHours
            // 
            this.txtRegularHours.Location = new System.Drawing.Point(239, 226);
            this.txtRegularHours.Name = "txtRegularHours";
            this.txtRegularHours.ReadOnly = true;
            this.txtRegularHours.Size = new System.Drawing.Size(120, 30);
            this.txtRegularHours.TabIndex = 7;
            // 
            // txtOvertimeHours
            // 
            this.txtOvertimeHours.Location = new System.Drawing.Point(239, 262);
            this.txtOvertimeHours.Name = "txtOvertimeHours";
            this.txtOvertimeHours.ReadOnly = true;
            this.txtOvertimeHours.Size = new System.Drawing.Size(120, 30);
            this.txtOvertimeHours.TabIndex = 8;
            // 
            // txtHolidayHours
            // 
            this.txtHolidayHours.Location = new System.Drawing.Point(239, 302);
            this.txtHolidayHours.Name = "txtHolidayHours";
            this.txtHolidayHours.ReadOnly = true;
            this.txtHolidayHours.Size = new System.Drawing.Size(120, 30);
            this.txtHolidayHours.TabIndex = 9;
            // 
            // txtMonthlySalary
            // 
            this.txtMonthlySalary.Location = new System.Drawing.Point(239, 342);
            this.txtMonthlySalary.Name = "txtMonthlySalary";
            this.txtMonthlySalary.Size = new System.Drawing.Size(120, 30);
            this.txtMonthlySalary.TabIndex = 10;
            // 
            // txtCalculatedSalary
            // 
            this.txtCalculatedSalary.Location = new System.Drawing.Point(239, 382);
            this.txtCalculatedSalary.Name = "txtCalculatedSalary";
            this.txtCalculatedSalary.ReadOnly = true;
            this.txtCalculatedSalary.Size = new System.Drawing.Size(120, 30);
            this.txtCalculatedSalary.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(418, 229);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 23);
            this.label1.TabIndex = 12;
            this.label1.Text = "ساعات العمل الأساسية";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(418, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 23);
            this.label2.TabIndex = 13;
            this.label2.Text = "ساعات العمل الإضافي";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(398, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 23);
            this.label3.TabIndex = 14;
            this.label3.Text = "العمل ايام العطل الرسمية";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(472, 342);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 23);
            this.label4.TabIndex = 15;
            this.label4.Text = "الراتب الاساسي";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(484, 385);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 23);
            this.label5.TabIndex = 16;
            this.label5.Text = "حساب الراتب";
            // 
            // FormSalaryCalculation
            // 
            this.ClientSize = new System.Drawing.Size(600, 450);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.btnExportPDF);
            this.Controls.Add(this.txtRegularHours);
            this.Controls.Add(this.txtOvertimeHours);
            this.Controls.Add(this.txtHolidayHours);
            this.Controls.Add(this.txtMonthlySalary);
            this.Controls.Add(this.txtCalculatedSalary);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormSalaryCalculation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "حساب الراتب";
            this.Load += new System.EventHandler(this.FormSalaryCalculation_Load);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbEmployees;
        private System.Windows.Forms.DateTimePicker dtMonth;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnExportPDF;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.TextBox txtRegularHours;
        private System.Windows.Forms.TextBox txtOvertimeHours;
        private System.Windows.Forms.TextBox txtHolidayHours;
        private System.Windows.Forms.TextBox txtMonthlySalary;
        private System.Windows.Forms.TextBox txtCalculatedSalary;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}
