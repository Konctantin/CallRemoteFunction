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
            this.tbFuncAddress = new System.Windows.Forms.TextBox();
            this.cbProcess = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.bRefresh = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbParam1 = new System.Windows.Forms.TextBox();
            this.bFlexInject = new System.Windows.Forms.Button();
            this.tbParam2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            //
            // tbFuncAddress
            //
            this.tbFuncAddress.Location = new System.Drawing.Point(89, 39);
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
            this.cbProcess.Location = new System.Drawing.Point(89, 12);
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
            this.tbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbConsole.Location = new System.Drawing.Point(2, 208);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConsole.Size = new System.Drawing.Size(354, 233);
            this.tbConsole.TabIndex = 9;
            //
            // bRefresh
            //
            this.bRefresh.Location = new System.Drawing.Point(316, 10);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(28, 23);
            this.bRefresh.TabIndex = 10;
            this.bRefresh.Text = "R";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.Form1_Load);
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbParam1);
            this.groupBox1.Controls.Add(this.bFlexInject);
            this.groupBox1.Controls.Add(this.tbParam2);
            this.groupBox1.Location = new System.Drawing.Point(15, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 137);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Param2:";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Param1:";
            //
            // tbParam1
            //
            this.tbParam1.Location = new System.Drawing.Point(58, 19);
            this.tbParam1.Name = "tbParam1";
            this.tbParam1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbParam1.Size = new System.Drawing.Size(263, 20);
            this.tbParam1.TabIndex = 5;
            this.tbParam1.Text = "print(\"Hello wow! #{0}\");";
            //
            // bFlexInject
            //
            this.bFlexInject.Location = new System.Drawing.Point(9, 71);
            this.bFlexInject.Name = "bFlexInject";
            this.bFlexInject.Size = new System.Drawing.Size(312, 60);
            this.bFlexInject.TabIndex = 4;
            this.bFlexInject.Text = "Flex Inject";
            this.bFlexInject.UseVisualStyleBackColor = true;
            this.bFlexInject.Click += new System.EventHandler(this.bInject_Click);
            //
            // tbParam2
            //
            this.tbParam2.Location = new System.Drawing.Point(58, 45);
            this.tbParam2.Name = "tbParam2";
            this.tbParam2.Size = new System.Drawing.Size(263, 20);
            this.tbParam2.TabIndex = 3;
            this.tbParam2.Text = "profile.lua";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Func address:";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 441);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bRefresh);
            this.Controls.Add(this.tbConsole);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbProcess);
            this.Controls.Add(this.tbFuncAddress);
            this.Name = "Form1";
            this.Text = "Test wow function caller";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFuncAddress;
        private System.Windows.Forms.ComboBox cbProcess;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.Button bRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbParam1;
        private System.Windows.Forms.Button bFlexInject;
        private System.Windows.Forms.TextBox tbParam2;
        private System.Windows.Forms.Label label1;
    }
}

