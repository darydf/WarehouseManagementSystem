namespace WarehouseManagementSystem
{
    partial class FormSettings
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
            this.numDiscountPercent = new System.Windows.Forms.NumericUpDown();
            this.numDiscountDays = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSettings = new System.Windows.Forms.Label();
            this.lblDaysTitle = new System.Windows.Forms.Label();
            this.lblDays = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.labelCurrency = new System.Windows.Forms.Label();
            this.comboBoxCurrency = new System.Windows.Forms.ComboBox();
            this.labelHint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numDiscountPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiscountDays)).BeginInit();
            this.SuspendLayout();
            // 
            // numDiscountPercent
            // 
            this.numDiscountPercent.Location = new System.Drawing.Point(29, 128);
            this.numDiscountPercent.Name = "numDiscountPercent";
            this.numDiscountPercent.Size = new System.Drawing.Size(120, 22);
            this.numDiscountPercent.TabIndex = 0;
            // 
            // numDiscountDays
            // 
            this.numDiscountDays.Location = new System.Drawing.Point(209, 127);
            this.numDiscountDays.Name = "numDiscountDays";
            this.numDiscountDays.Size = new System.Drawing.Size(120, 22);
            this.numDiscountDays.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Location = new System.Drawing.Point(42, 431);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(121, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(225, 432);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 34);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblStatus.Location = new System.Drawing.Point(38, 315);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 20);
            this.lblStatus.TabIndex = 4;
            // 
            // lblSettings
            // 
            this.lblSettings.AutoSize = true;
            this.lblSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSettings.Location = new System.Drawing.Point(29, 26);
            this.lblSettings.Name = "lblSettings";
            this.lblSettings.Size = new System.Drawing.Size(109, 20);
            this.lblSettings.TabIndex = 5;
            this.lblSettings.Text = "Настройки";
            // 
            // lblDaysTitle
            // 
            this.lblDaysTitle.AutoSize = true;
            this.lblDaysTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDaysTitle.Location = new System.Drawing.Point(30, 62);
            this.lblDaysTitle.Name = "lblDaysTitle";
            this.lblDaysTitle.Size = new System.Drawing.Size(193, 18);
            this.lblDaysTitle.TabIndex = 6;
            this.lblDaysTitle.Text = "Скидка по сроку годности";
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(209, 105);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(114, 16);
            this.lblDays.TabIndex = 7;
            this.lblDays.Text = "За сколько дней";
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(29, 104);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(77, 16);
            this.lblPercent.TabIndex = 8;
            this.lblPercent.Text = "Скидка (%)";
            // 
            // labelCurrency
            // 
            this.labelCurrency.AutoSize = true;
            this.labelCurrency.Location = new System.Drawing.Point(30, 190);
            this.labelCurrency.Name = "labelCurrency";
            this.labelCurrency.Size = new System.Drawing.Size(150, 16);
            this.labelCurrency.TabIndex = 9;
            this.labelCurrency.Text = "Валюта отображения:";
            // 
            // comboBoxCurrency
            // 
            this.comboBoxCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrency.FormattingEnabled = true;
            this.comboBoxCurrency.Items.AddRange(new object[] {
            "RUB",
            "USD",
            "EUR"});
            this.comboBoxCurrency.Location = new System.Drawing.Point(33, 229);
            this.comboBoxCurrency.Name = "comboBoxCurrency";
            this.comboBoxCurrency.Size = new System.Drawing.Size(121, 24);
            this.comboBoxCurrency.TabIndex = 10;
            // 
            // labelHint
            // 
            this.labelHint.AutoSize = true;
            this.labelHint.Location = new System.Drawing.Point(26, 278);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(503, 16);
            this.labelHint.TabIndex = 11;
            this.labelHint.Text = "Выберите валюту для отображения цен.Курсы обновляются автоматически";
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 510);
            this.Controls.Add(this.labelHint);
            this.Controls.Add(this.comboBoxCurrency);
            this.Controls.Add(this.labelCurrency);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.lblDays);
            this.Controls.Add(this.lblDaysTitle);
            this.Controls.Add(this.lblSettings);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numDiscountDays);
            this.Controls.Add(this.numDiscountPercent);
            this.Name = "FormSettings";
            this.Text = "FormSettings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numDiscountPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiscountDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numDiscountPercent;
        private System.Windows.Forms.NumericUpDown numDiscountDays;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSettings;
        private System.Windows.Forms.Label lblDaysTitle;
        private System.Windows.Forms.Label lblDays;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label labelCurrency;
        private System.Windows.Forms.ComboBox comboBoxCurrency;
        private System.Windows.Forms.Label labelHint;
    }
}