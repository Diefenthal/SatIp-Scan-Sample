﻿/*  
    Copyright (C) <2007-2019>  <Kay Diefenthal>

    SatIp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SatIp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SatIp.  If not, see <http://www.gnu.org/licenses/>.
*/ 
namespace SatIp.Usercontrols
{
    partial class TerrestrialInformation
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblNIT = new System.Windows.Forms.Label();
            this.lblSDT = new System.Windows.Forms.Label();
            this.lblPMT = new System.Windows.Forms.Label();
            this.lblPAT = new System.Windows.Forms.Label();
            this.btnScan = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.pgbSignalLevel = new System.Windows.Forms.ProgressBar();
            this.pgbSignalQuality = new System.Windows.Forms.ProgressBar();
            this.pgbSearchResult = new System.Windows.Forms.ProgressBar();
            this.lwResults = new System.Windows.Forms.ListView();
            this.colFrequency = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServiceType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServiceName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServiceProvider = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServiceId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxSourceA = new System.Windows.Forms.ComboBox();
            this.lblSourceA = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.lblNIT);
            this.groupBox3.Controls.Add(this.lblSDT);
            this.groupBox3.Controls.Add(this.lblPMT);
            this.groupBox3.Controls.Add(this.lblPAT);
            this.groupBox3.Controls.Add(this.btnScan);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.pgbSignalLevel);
            this.groupBox3.Controls.Add(this.pgbSignalQuality);
            this.groupBox3.Controls.Add(this.pgbSearchResult);
            this.groupBox3.Controls.Add(this.lwResults);
            this.groupBox3.Location = new System.Drawing.Point(2, 56);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(447, 355);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search";
            // 
            // lblNIT
            // 
            this.lblNIT.AutoSize = true;
            this.lblNIT.BackColor = System.Drawing.Color.LimeGreen;
            this.lblNIT.Location = new System.Drawing.Point(111, 80);
            this.lblNIT.Name = "lblNIT";
            this.lblNIT.Size = new System.Drawing.Size(25, 13);
            this.lblNIT.TabIndex = 10;
            this.lblNIT.Text = "NIT";
            // 
            // lblSDT
            // 
            this.lblSDT.AutoSize = true;
            this.lblSDT.BackColor = System.Drawing.Color.LimeGreen;
            this.lblSDT.Location = new System.Drawing.Point(76, 80);
            this.lblSDT.Name = "lblSDT";
            this.lblSDT.Size = new System.Drawing.Size(29, 13);
            this.lblSDT.TabIndex = 9;
            this.lblSDT.Text = "SDT";
            // 
            // lblPMT
            // 
            this.lblPMT.AutoSize = true;
            this.lblPMT.BackColor = System.Drawing.Color.LimeGreen;
            this.lblPMT.Location = new System.Drawing.Point(40, 80);
            this.lblPMT.Name = "lblPMT";
            this.lblPMT.Size = new System.Drawing.Size(30, 13);
            this.lblPMT.TabIndex = 8;
            this.lblPMT.Text = "PMT";
            // 
            // lblPAT
            // 
            this.lblPAT.AutoSize = true;
            this.lblPAT.BackColor = System.Drawing.Color.LimeGreen;
            this.lblPAT.Location = new System.Drawing.Point(6, 80);
            this.lblPAT.Name = "lblPAT";
            this.lblPAT.Size = new System.Drawing.Size(28, 13);
            this.lblPAT.TabIndex = 7;
            this.lblPAT.Text = "PAT";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(339, 73);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(102, 20);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Start Search";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.BtnScan_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 38);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Signal Quality";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Signal Level";
            // 
            // pgbSignalLevel
            // 
            this.pgbSignalLevel.Location = new System.Drawing.Point(79, 19);
            this.pgbSignalLevel.Name = "pgbSignalLevel";
            this.pgbSignalLevel.Size = new System.Drawing.Size(362, 13);
            this.pgbSignalLevel.TabIndex = 3;
            // 
            // pgbSignalQuality
            // 
            this.pgbSignalQuality.Location = new System.Drawing.Point(79, 38);
            this.pgbSignalQuality.Name = "pgbSignalQuality";
            this.pgbSignalQuality.Size = new System.Drawing.Size(362, 13);
            this.pgbSignalQuality.TabIndex = 2;
            // 
            // pgbSearchResult
            // 
            this.pgbSearchResult.Location = new System.Drawing.Point(9, 57);
            this.pgbSearchResult.Name = "pgbSearchResult";
            this.pgbSearchResult.Size = new System.Drawing.Size(432, 10);
            this.pgbSearchResult.TabIndex = 1;
            // 
            // lwResults
            // 
            this.lwResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFrequency,
            this.colServiceType,
            this.colServiceName,
            this.colServiceProvider,
            this.colServiceId});
            this.lwResults.Location = new System.Drawing.Point(6, 99);
            this.lwResults.Name = "lwResults";
            this.lwResults.Size = new System.Drawing.Size(435, 250);
            this.lwResults.TabIndex = 0;
            this.lwResults.UseCompatibleStateImageBehavior = false;
            this.lwResults.View = System.Windows.Forms.View.Details;
            // 
            // colFrequency
            // 
            this.colFrequency.Text = "Frequency";
            this.colFrequency.Width = 100;
            // 
            // colServiceType
            // 
            this.colServiceType.Text = "ServiceType";
            this.colServiceType.Width = 100;
            // 
            // colServiceName
            // 
            this.colServiceName.Text = "ServiceName";
            this.colServiceName.Width = 100;
            // 
            // colServiceProvider
            // 
            this.colServiceProvider.Text = "ServiceProvider";
            this.colServiceProvider.Width = 100;
            // 
            // colServiceId
            // 
            this.colServiceId.Text = "ServiceId";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.cbxSourceA);
            this.groupBox2.Controls.Add(this.lblSourceA);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(446, 47);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sources";
            // 
            // cbxSourceA
            // 
            this.cbxSourceA.FormattingEnabled = true;
            this.cbxSourceA.Location = new System.Drawing.Point(78, 19);
            this.cbxSourceA.Name = "cbxSourceA";
            this.cbxSourceA.Size = new System.Drawing.Size(362, 21);
            this.cbxSourceA.TabIndex = 6;
            // 
            // lblSourceA
            // 
            this.lblSourceA.AutoSize = true;
            this.lblSourceA.Location = new System.Drawing.Point(7, 22);
            this.lblSourceA.Name = "lblSourceA";
            this.lblSourceA.Size = new System.Drawing.Size(51, 13);
            this.lblSourceA.TabIndex = 1;
            this.lblSourceA.Text = "Source A";
            // 
            // TerrestrialInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "TerrestrialInformation";
            this.Size = new System.Drawing.Size(452, 411);
            this.Load += new System.EventHandler(this.TerrestrialInformation_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblNIT;
        private System.Windows.Forms.Label lblSDT;
        private System.Windows.Forms.Label lblPMT;
        private System.Windows.Forms.Label lblPAT;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ProgressBar pgbSignalLevel;
        private System.Windows.Forms.ProgressBar pgbSignalQuality;
        private System.Windows.Forms.ProgressBar pgbSearchResult;
        private System.Windows.Forms.ListView lwResults;
        private System.Windows.Forms.ColumnHeader colFrequency;
        private System.Windows.Forms.ColumnHeader colServiceType;
        private System.Windows.Forms.ColumnHeader colServiceName;
        private System.Windows.Forms.ColumnHeader colServiceProvider;
        private System.Windows.Forms.ColumnHeader colServiceId;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbxSourceA;
        private System.Windows.Forms.Label lblSourceA;
    }
}
