namespace DigipoolServer
{
    partial class ReaderSettings
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
            this.Submit = new System.Windows.Forms.Button();
            this.reader_Address = new System.Windows.Forms.TextBox();
            this.tx_Power_In_Dbm = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Submit
            // 
            this.Submit.Location = new System.Drawing.Point(389, 152);
            this.Submit.Name = "Submit";
            this.Submit.Size = new System.Drawing.Size(96, 34);
            this.Submit.TabIndex = 0;
            this.Submit.Text = "Submit";
            this.Submit.UseVisualStyleBackColor = true;
            this.Submit.Click += new System.EventHandler(this.Submit_Click);
            // 
            // reader_Address
            // 
            this.reader_Address.Location = new System.Drawing.Point(129, 46);
            this.reader_Address.Name = "reader_Address";
            this.reader_Address.Size = new System.Drawing.Size(188, 22);
            this.reader_Address.TabIndex = 1;
            this.reader_Address.TextChanged += new System.EventHandler(this.readerAddress_TextChanged);
            // 
            // tx_Power_In_Dbm
            // 
            this.tx_Power_In_Dbm.Location = new System.Drawing.Point(129, 90);
            this.tx_Power_In_Dbm.Name = "tx_Power_In_Dbm";
            this.tx_Power_In_Dbm.Size = new System.Drawing.Size(188, 22);
            this.tx_Power_In_Dbm.TabIndex = 2;
            this.tx_Power_In_Dbm.TextChanged += new System.EventHandler(this.txPowerInDbm_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Reader Address";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tx Power in Dbm";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(327, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Min: 10.00 Max: 27.00";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(362, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Tx Power goes up in increments of .25 e.g 10.5 or 23.75";
            // 
            // ReaderSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 198);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tx_Power_In_Dbm);
            this.Controls.Add(this.reader_Address);
            this.Controls.Add(this.Submit);
            this.Name = "ReaderSettings";
            this.Text = "ReaderSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Submit;
        private System.Windows.Forms.TextBox reader_Address;
        private System.Windows.Forms.TextBox tx_Power_In_Dbm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}