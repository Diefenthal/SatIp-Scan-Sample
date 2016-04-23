namespace SatIp
{
    partial class Form2
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
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxSourceD = new System.Windows.Forms.ComboBox();
            this.cbxSourceC = new System.Windows.Forms.ComboBox();
            this.cbxSourceB = new System.Windows.Forms.ComboBox();
            this.cbxSourceA = new System.Windows.Forms.ComboBox();
            this.cbxDiseqC = new System.Windows.Forms.ComboBox();
            this.lblSourceD = new System.Windows.Forms.Label();
            this.lblSourceC = new System.Windows.Forms.Label();
            this.lblSourceB = new System.Windows.Forms.Label();
            this.lblSourceA = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.pgbSignalLevel = new System.Windows.Forms.ProgressBar();
            this.pgbSignalQuality = new System.Windows.Forms.ProgressBar();
            this.pgbSearchResult = new System.Windows.Forms.ProgressBar();
            this.lwResults = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbxDVBC = new System.Windows.Forms.PictureBox();
            this.pbxDVBS = new System.Windows.Forms.PictureBox();
            this.pbxDVBT = new System.Windows.Forms.PictureBox();
            this.lblManufacture = new System.Windows.Forms.Label();
            this.lblModelDescription = new System.Windows.Forms.Label();
            this.tbxManufacture = new System.Windows.Forms.TextBox();
            this.tbxModelDescription = new System.Windows.Forms.TextBox();
            this.pbxManufactureBrand = new System.Windows.Forms.PictureBox();
            this.tbxDeviceType = new System.Windows.Forms.TextBox();
            this.tbxUniqueDeviceName = new System.Windows.Forms.TextBox();
            this.tbxFriendlyName = new System.Windows.Forms.TextBox();
            this.lblDeviceType = new System.Windows.Forms.Label();
            this.blUniqueDeviceName = new System.Windows.Forms.Label();
            this.lblFriendlyName = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxDVBC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxDVBS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxDVBT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxManufactureBrand)).BeginInit();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Status";
            this.columnHeader1.Width = 470;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.cbxSourceD);
            this.groupBox2.Controls.Add(this.cbxSourceC);
            this.groupBox2.Controls.Add(this.cbxSourceB);
            this.groupBox2.Controls.Add(this.cbxSourceA);
            this.groupBox2.Controls.Add(this.cbxDiseqC);
            this.groupBox2.Controls.Add(this.lblSourceD);
            this.groupBox2.Controls.Add(this.lblSourceC);
            this.groupBox2.Controls.Add(this.lblSourceB);
            this.groupBox2.Controls.Add(this.lblSourceA);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(13, 186);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(487, 166);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sources";
            // 
            // cbxSourceD
            // 
            this.cbxSourceD.FormattingEnabled = true;
            this.cbxSourceD.Location = new System.Drawing.Point(117, 132);
            this.cbxSourceD.Name = "cbxSourceD";
            this.cbxSourceD.Size = new System.Drawing.Size(364, 21);
            this.cbxSourceD.TabIndex = 9;
            this.cbxSourceD.Visible = false;
            // 
            // cbxSourceC
            // 
            this.cbxSourceC.FormattingEnabled = true;
            this.cbxSourceC.Location = new System.Drawing.Point(117, 104);
            this.cbxSourceC.Name = "cbxSourceC";
            this.cbxSourceC.Size = new System.Drawing.Size(364, 21);
            this.cbxSourceC.TabIndex = 8;
            this.cbxSourceC.Visible = false;
            // 
            // cbxSourceB
            // 
            this.cbxSourceB.FormattingEnabled = true;
            this.cbxSourceB.Location = new System.Drawing.Point(117, 76);
            this.cbxSourceB.Name = "cbxSourceB";
            this.cbxSourceB.Size = new System.Drawing.Size(364, 21);
            this.cbxSourceB.TabIndex = 7;
            this.cbxSourceB.Visible = false;
            // 
            // cbxSourceA
            // 
            this.cbxSourceA.FormattingEnabled = true;
            this.cbxSourceA.Location = new System.Drawing.Point(117, 48);
            this.cbxSourceA.Name = "cbxSourceA";
            this.cbxSourceA.Size = new System.Drawing.Size(364, 21);
            this.cbxSourceA.TabIndex = 6;
            // 
            // cbxDiseqC
            // 
            this.cbxDiseqC.FormattingEnabled = true;
            this.cbxDiseqC.Location = new System.Drawing.Point(117, 20);
            this.cbxDiseqC.Name = "cbxDiseqC";
            this.cbxDiseqC.Size = new System.Drawing.Size(132, 21);
            this.cbxDiseqC.TabIndex = 5;
            this.cbxDiseqC.SelectedIndexChanged += new System.EventHandler(this.cbxDiseqC_SelectedIndexChanged);
            // 
            // lblSourceD
            // 
            this.lblSourceD.AutoSize = true;
            this.lblSourceD.Location = new System.Drawing.Point(7, 135);
            this.lblSourceD.Name = "lblSourceD";
            this.lblSourceD.Size = new System.Drawing.Size(52, 13);
            this.lblSourceD.TabIndex = 4;
            this.lblSourceD.Text = "Source D";
            this.lblSourceD.Visible = false;
            // 
            // lblSourceC
            // 
            this.lblSourceC.AutoSize = true;
            this.lblSourceC.Location = new System.Drawing.Point(7, 107);
            this.lblSourceC.Name = "lblSourceC";
            this.lblSourceC.Size = new System.Drawing.Size(51, 13);
            this.lblSourceC.TabIndex = 3;
            this.lblSourceC.Text = "Source C";
            this.lblSourceC.Visible = false;
            // 
            // lblSourceB
            // 
            this.lblSourceB.AutoSize = true;
            this.lblSourceB.Location = new System.Drawing.Point(7, 79);
            this.lblSourceB.Name = "lblSourceB";
            this.lblSourceB.Size = new System.Drawing.Size(51, 13);
            this.lblSourceB.TabIndex = 2;
            this.lblSourceB.Text = "Source B";
            this.lblSourceB.Visible = false;
            // 
            // lblSourceA
            // 
            this.lblSourceA.AutoSize = true;
            this.lblSourceA.Location = new System.Drawing.Point(7, 51);
            this.lblSourceA.Name = "lblSourceA";
            this.lblSourceA.Size = new System.Drawing.Size(51, 13);
            this.lblSourceA.TabIndex = 1;
            this.lblSourceA.Text = "Source A";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Diseq C";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.btnScan);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.pgbSignalLevel);
            this.groupBox3.Controls.Add(this.pgbSignalQuality);
            this.groupBox3.Controls.Add(this.pgbSearchResult);
            this.groupBox3.Controls.Add(this.lwResults);
            this.groupBox3.Location = new System.Drawing.Point(12, 358);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(487, 292);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(379, 13);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(102, 20);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Start Search";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 58);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Signal Quality";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 39);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Signal Level";
            // 
            // pgbSignalLevel
            // 
            this.pgbSignalLevel.Location = new System.Drawing.Point(117, 39);
            this.pgbSignalLevel.Name = "pgbSignalLevel";
            this.pgbSignalLevel.Size = new System.Drawing.Size(364, 13);
            this.pgbSignalLevel.TabIndex = 3;
            // 
            // pgbSignalQuality
            // 
            this.pgbSignalQuality.Location = new System.Drawing.Point(117, 58);
            this.pgbSignalQuality.Name = "pgbSignalQuality";
            this.pgbSignalQuality.Size = new System.Drawing.Size(364, 13);
            this.pgbSignalQuality.TabIndex = 2;
            // 
            // pgbSearchResult
            // 
            this.pgbSearchResult.Location = new System.Drawing.Point(6, 77);
            this.pgbSearchResult.Name = "pgbSearchResult";
            this.pgbSearchResult.Size = new System.Drawing.Size(475, 10);
            this.pgbSearchResult.TabIndex = 1;
            // 
            // lwResults
            // 
            this.lwResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lwResults.Location = new System.Drawing.Point(6, 93);
            this.lwResults.Name = "lwResults";
            this.lwResults.Size = new System.Drawing.Size(475, 193);
            this.lwResults.TabIndex = 0;
            this.lwResults.UseCompatibleStateImageBehavior = false;
            this.lwResults.View = System.Windows.Forms.View.Details;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbxDVBC);
            this.groupBox1.Controls.Add(this.pbxDVBS);
            this.groupBox1.Controls.Add(this.pbxDVBT);
            this.groupBox1.Controls.Add(this.lblManufacture);
            this.groupBox1.Controls.Add(this.lblModelDescription);
            this.groupBox1.Controls.Add(this.tbxManufacture);
            this.groupBox1.Controls.Add(this.tbxModelDescription);
            this.groupBox1.Controls.Add(this.pbxManufactureBrand);
            this.groupBox1.Controls.Add(this.tbxDeviceType);
            this.groupBox1.Controls.Add(this.tbxUniqueDeviceName);
            this.groupBox1.Controls.Add(this.tbxFriendlyName);
            this.groupBox1.Controls.Add(this.lblDeviceType);
            this.groupBox1.Controls.Add(this.blUniqueDeviceName);
            this.groupBox1.Controls.Add(this.lblFriendlyName);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(486, 174);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // pbxDVBC
            // 
            this.pbxDVBC.Image = global::SatIp.Scan.Properties.Resources.dvb_c;
            this.pbxDVBC.InitialImage = null;
            this.pbxDVBC.Location = new System.Drawing.Point(117, 145);
            this.pbxDVBC.Name = "pbxDVBC";
            this.pbxDVBC.Size = new System.Drawing.Size(71, 20);
            this.pbxDVBC.TabIndex = 29;
            this.pbxDVBC.TabStop = false;
            this.pbxDVBC.Visible = false;
            // 
            // pbxDVBS
            // 
            this.pbxDVBS.Image = global::SatIp.Scan.Properties.Resources.dvb_s;
            this.pbxDVBS.InitialImage = null;
            this.pbxDVBS.Location = new System.Drawing.Point(194, 145);
            this.pbxDVBS.Name = "pbxDVBS";
            this.pbxDVBS.Size = new System.Drawing.Size(71, 20);
            this.pbxDVBS.TabIndex = 30;
            this.pbxDVBS.TabStop = false;
            this.pbxDVBS.Visible = false;
            // 
            // pbxDVBT
            // 
            this.pbxDVBT.Image = global::SatIp.Scan.Properties.Resources.dvb_t;
            this.pbxDVBT.InitialImage = null;
            this.pbxDVBT.Location = new System.Drawing.Point(271, 145);
            this.pbxDVBT.Name = "pbxDVBT";
            this.pbxDVBT.Size = new System.Drawing.Size(71, 20);
            this.pbxDVBT.TabIndex = 31;
            this.pbxDVBT.TabStop = false;
            this.pbxDVBT.Visible = false;
            // 
            // lblManufacture
            // 
            this.lblManufacture.AutoSize = true;
            this.lblManufacture.Location = new System.Drawing.Point(5, 123);
            this.lblManufacture.Name = "lblManufacture";
            this.lblManufacture.Size = new System.Drawing.Size(67, 13);
            this.lblManufacture.TabIndex = 28;
            this.lblManufacture.Text = "Manufacture";
            // 
            // lblModelDescription
            // 
            this.lblModelDescription.AutoSize = true;
            this.lblModelDescription.Location = new System.Drawing.Point(5, 98);
            this.lblModelDescription.Name = "lblModelDescription";
            this.lblModelDescription.Size = new System.Drawing.Size(92, 13);
            this.lblModelDescription.TabIndex = 27;
            this.lblModelDescription.Text = "Model Description";
            // 
            // tbxManufacture
            // 
            this.tbxManufacture.Location = new System.Drawing.Point(116, 120);
            this.tbxManufacture.Name = "tbxManufacture";
            this.tbxManufacture.Size = new System.Drawing.Size(238, 20);
            this.tbxManufacture.TabIndex = 26;
            // 
            // tbxModelDescription
            // 
            this.tbxModelDescription.Location = new System.Drawing.Point(116, 95);
            this.tbxModelDescription.Name = "tbxModelDescription";
            this.tbxModelDescription.Size = new System.Drawing.Size(238, 20);
            this.tbxModelDescription.TabIndex = 25;
            // 
            // pbxManufactureBrand
            // 
            this.pbxManufactureBrand.Location = new System.Drawing.Point(360, 19);
            this.pbxManufactureBrand.Name = "pbxManufactureBrand";
            this.pbxManufactureBrand.Size = new System.Drawing.Size(120, 120);
            this.pbxManufactureBrand.TabIndex = 24;
            this.pbxManufactureBrand.TabStop = false;
            // 
            // tbxDeviceType
            // 
            this.tbxDeviceType.Location = new System.Drawing.Point(116, 69);
            this.tbxDeviceType.Name = "tbxDeviceType";
            this.tbxDeviceType.Size = new System.Drawing.Size(238, 20);
            this.tbxDeviceType.TabIndex = 23;
            // 
            // tbxUniqueDeviceName
            // 
            this.tbxUniqueDeviceName.Location = new System.Drawing.Point(116, 44);
            this.tbxUniqueDeviceName.Name = "tbxUniqueDeviceName";
            this.tbxUniqueDeviceName.Size = new System.Drawing.Size(238, 20);
            this.tbxUniqueDeviceName.TabIndex = 22;
            // 
            // tbxFriendlyName
            // 
            this.tbxFriendlyName.Location = new System.Drawing.Point(116, 19);
            this.tbxFriendlyName.Name = "tbxFriendlyName";
            this.tbxFriendlyName.Size = new System.Drawing.Size(238, 20);
            this.tbxFriendlyName.TabIndex = 21;
            // 
            // lblDeviceType
            // 
            this.lblDeviceType.AutoSize = true;
            this.lblDeviceType.Location = new System.Drawing.Point(5, 72);
            this.lblDeviceType.Name = "lblDeviceType";
            this.lblDeviceType.Size = new System.Drawing.Size(68, 13);
            this.lblDeviceType.TabIndex = 20;
            this.lblDeviceType.Text = "Device Type";
            // 
            // blUniqueDeviceName
            // 
            this.blUniqueDeviceName.AutoSize = true;
            this.blUniqueDeviceName.Location = new System.Drawing.Point(5, 47);
            this.blUniqueDeviceName.Name = "blUniqueDeviceName";
            this.blUniqueDeviceName.Size = new System.Drawing.Size(109, 13);
            this.blUniqueDeviceName.TabIndex = 19;
            this.blUniqueDeviceName.Text = "Unique Device Name";
            // 
            // lblFriendlyName
            // 
            this.lblFriendlyName.AutoSize = true;
            this.lblFriendlyName.Location = new System.Drawing.Point(6, 22);
            this.lblFriendlyName.Name = "lblFriendlyName";
            this.lblFriendlyName.Size = new System.Drawing.Size(74, 13);
            this.lblFriendlyName.TabIndex = 18;
            this.lblFriendlyName.Text = "Friendly Name";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(511, 662);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxDVBC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxDVBS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxDVBT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxManufactureBrand)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbxSourceD;
        private System.Windows.Forms.ComboBox cbxSourceC;
        private System.Windows.Forms.ComboBox cbxSourceB;
        private System.Windows.Forms.ComboBox cbxSourceA;
        private System.Windows.Forms.ComboBox cbxDiseqC;
        private System.Windows.Forms.Label lblSourceD;
        private System.Windows.Forms.Label lblSourceC;
        private System.Windows.Forms.Label lblSourceB;
        private System.Windows.Forms.Label lblSourceA;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ProgressBar pgbSignalLevel;
        private System.Windows.Forms.ProgressBar pgbSignalQuality;
        private System.Windows.Forms.ProgressBar pgbSearchResult;
        private System.Windows.Forms.ListView lwResults;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pbxDVBC;
        private System.Windows.Forms.PictureBox pbxDVBS;
        private System.Windows.Forms.PictureBox pbxDVBT;
        private System.Windows.Forms.Label lblManufacture;
        private System.Windows.Forms.Label lblModelDescription;
        private System.Windows.Forms.TextBox tbxManufacture;
        private System.Windows.Forms.TextBox tbxModelDescription;
        private System.Windows.Forms.PictureBox pbxManufactureBrand;
        private System.Windows.Forms.TextBox tbxDeviceType;
        private System.Windows.Forms.TextBox tbxUniqueDeviceName;
        private System.Windows.Forms.TextBox tbxFriendlyName;
        private System.Windows.Forms.Label lblDeviceType;
        private System.Windows.Forms.Label blUniqueDeviceName;
        private System.Windows.Forms.Label lblFriendlyName;
    }
}