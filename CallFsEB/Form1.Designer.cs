namespace CallFsEB
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.bInject = new System.Windows.Forms.Button();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbInjectionAddress = new System.Windows.Forms.TextBox();
            this.tbFuncAddress = new System.Windows.Forms.TextBox();
            this.cbProcess = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.bRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bInject
            // 
            this.bInject.Location = new System.Drawing.Point(12, 243);
            this.bInject.Name = "bInject";
            this.bInject.Size = new System.Drawing.Size(367, 23);
            this.bInject.TabIndex = 0;
            this.bInject.Text = "Inject";
            this.bInject.UseVisualStyleBackColor = true;
            this.bInject.Click += new System.EventHandler(this.bInject_Click);
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(12, 134);
            this.tbCode.Multiline = true;
            this.tbCode.Name = "tbCode";
            this.tbCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbCode.Size = new System.Drawing.Size(367, 103);
            this.tbCode.TabIndex = 1;
            this.tbCode.Text = "print(\"Hello wow!\");";
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(12, 108);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(367, 20);
            this.tbPath.TabIndex = 2;
            this.tbPath.Text = "profile.lua";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Injection address: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Func address:";
            // 
            // tbInjectionAddress
            // 
            this.tbInjectionAddress.Location = new System.Drawing.Point(126, 46);
            this.tbInjectionAddress.Name = "tbInjectionAddress";
            this.tbInjectionAddress.Size = new System.Drawing.Size(253, 20);
            this.tbInjectionAddress.TabIndex = 5;
            // 
            // tbFuncAddress
            // 
            this.tbFuncAddress.Location = new System.Drawing.Point(126, 71);
            this.tbFuncAddress.Name = "tbFuncAddress";
            this.tbFuncAddress.Size = new System.Drawing.Size(253, 20);
            this.tbFuncAddress.TabIndex = 6;
            this.tbFuncAddress.Text = "0x1000";
            // 
            // cbProcess
            // 
            this.cbProcess.DisplayMember = "Name";
            this.cbProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProcess.FormattingEnabled = true;
            this.cbProcess.Location = new System.Drawing.Point(126, 12);
            this.cbProcess.Name = "cbProcess";
            this.cbProcess.Size = new System.Drawing.Size(221, 21);
            this.cbProcess.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Process:";
            // 
            // tbConsole
            // 
            this.tbConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbConsole.Location = new System.Drawing.Point(15, 272);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConsole.Size = new System.Drawing.Size(367, 170);
            this.tbConsole.TabIndex = 9;
            // 
            // bRefresh
            // 
            this.bRefresh.Location = new System.Drawing.Point(353, 10);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(28, 23);
            this.bRefresh.TabIndex = 10;
            this.bRefresh.Text = "R";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.Form1_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 454);
            this.Controls.Add(this.bRefresh);
            this.Controls.Add(this.tbConsole);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbProcess);
            this.Controls.Add(this.tbFuncAddress);
            this.Controls.Add(this.tbInjectionAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.bInject);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bInject;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbInjectionAddress;
        private System.Windows.Forms.TextBox tbFuncAddress;
        private System.Windows.Forms.ComboBox cbProcess;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.Button bRefresh;
    }
}

