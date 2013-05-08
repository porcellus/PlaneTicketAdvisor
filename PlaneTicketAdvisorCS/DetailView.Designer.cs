namespace PlaneTicketAdvisorCS
{
    partial class DetailView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lEngineName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lTicketCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lPrice = new System.Windows.Forms.Label();
            this.btnPin = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kereső neve:";
            // 
            // lEngineName
            // 
            this.lEngineName.AutoSize = true;
            this.lEngineName.Location = new System.Drawing.Point(81, 4);
            this.lEngineName.Name = "lEngineName";
            this.lEngineName.Size = new System.Drawing.Size(10, 13);
            this.lEngineName.TabIndex = 1;
            this.lEngineName.Text = " ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Jegyek száma:";
            // 
            // lTicketCount
            // 
            this.lTicketCount.AutoSize = true;
            this.lTicketCount.Location = new System.Drawing.Point(84, 21);
            this.lTicketCount.Name = "lTicketCount";
            this.lTicketCount.Size = new System.Drawing.Size(10, 13);
            this.lTicketCount.TabIndex = 3;
            this.lTicketCount.Text = " ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Összár:";
            // 
            // lPrice
            // 
            this.lPrice.AutoSize = true;
            this.lPrice.Location = new System.Drawing.Point(171, 21);
            this.lPrice.Name = "lPrice";
            this.lPrice.Size = new System.Drawing.Size(0, 13);
            this.lPrice.TabIndex = 5;
            // 
            // btnPin
            // 
            this.btnPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPin.Location = new System.Drawing.Point(413, 0);
            this.btnPin.Name = "btnPin";
            this.btnPin.Size = new System.Drawing.Size(20, 23);
            this.btnPin.TabIndex = 7;
            this.btnPin.Text = "X";
            this.btnPin.UseVisualStyleBackColor = true;
            this.btnPin.Click += new System.EventHandler(this.btnPin_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(434, 102);
            this.dataGridView1.TabIndex = 8;
            // 
            // DetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnPin);
            this.Controls.Add(this.lPrice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lTicketCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lEngineName);
            this.Controls.Add(this.label1);
            this.Name = "DetailView";
            this.Size = new System.Drawing.Size(434, 151);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lEngineName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lTicketCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lPrice;
        private System.Windows.Forms.Button btnPin;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
