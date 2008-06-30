/*
    Autowikibrowser
    Copyright (C) 2007 Martin Richards

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

namespace AutoWikiBrowser
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.txtWarning = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.linkBluemoose = new System.Windows.Forms.LinkLabel();
            this.linkAWBPage = new System.Windows.Forms.LinkLabel();
            this.linkLigulem = new System.Windows.Forms.LinkLabel();
            this.linkMets501 = new System.Windows.Forms.LinkLabel();
            this.linkMaxSem = new System.Windows.Forms.LinkLabel();
            this.linkBugs = new System.Windows.Forms.LinkLabel();
            this.linkFeatureRequests = new System.Windows.Forms.LinkLabel();
            this.linkReedy = new System.Windows.Forms.LinkLabel();
            this.linkKingboy = new System.Windows.Forms.LinkLabel();
            this.linkMartinp23 = new System.Windows.Forms.LinkLabel();
            this.lblDevs = new System.Windows.Forms.Label();
            this.lblTimeAndEdits = new System.Windows.Forms.Label();
            this.lblNETVersion = new System.Windows.Forms.Label();
            this.lblIEVersion = new System.Windows.Forms.Label();
            this.lblOSVersion = new System.Windows.Forms.Label();
            this.lblAWBVersion = new System.Windows.Forms.Label();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblOriginalDevs = new System.Windows.Forms.Label();
            this.linkJogers = new System.Windows.Forms.LinkLabel();
            this.UsageStatsLabel = new System.Windows.Forms.LinkLabel();
            this.lblRevision = new System.Windows.Forms.Label();
            this.flwDevs = new System.Windows.Forms.FlowLayoutPanel();
            this.flwOriginalDevs = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.flwOSVersion = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flwDevs.SuspendLayout();
            this.flwOriginalDevs.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flwOSVersion.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtWarning
            // 
            this.txtWarning.Location = new System.Drawing.Point(234, 97);
            this.txtWarning.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.txtWarning.Multiline = true;
            this.txtWarning.Name = "txtWarning";
            this.txtWarning.ReadOnly = true;
            this.txtWarning.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtWarning.Size = new System.Drawing.Size(254, 127);
            this.txtWarning.TabIndex = 9;
            this.txtWarning.TabStop = false;
            this.txtWarning.Text = "WarningMessage";
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.SystemColors.Control;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(413, 230);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkBluemoose
            // 
            this.linkBluemoose.AutoSize = true;
            this.linkBluemoose.Location = new System.Drawing.Point(3, 0);
            this.linkBluemoose.Name = "linkBluemoose";
            this.linkBluemoose.Size = new System.Drawing.Size(84, 13);
            this.linkBluemoose.TabIndex = 0;
            this.linkBluemoose.TabStop = true;
            this.linkBluemoose.Text = "User:Bluemoose";
            this.linkBluemoose.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBluemoose_LinkClicked);
            // 
            // linkAWBPage
            // 
            this.linkAWBPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkAWBPage.Location = new System.Drawing.Point(12, 9);
            this.linkAWBPage.Name = "linkAWBPage";
            this.linkAWBPage.Size = new System.Drawing.Size(197, 29);
            this.linkAWBPage.TabIndex = 0;
            this.linkAWBPage.TabStop = true;
            this.linkAWBPage.Text = "AutoWikiBrowser";
            this.linkAWBPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAWBPage_LinkClicked);
            // 
            // linkLigulem
            // 
            this.linkLigulem.AutoSize = true;
            this.linkLigulem.Location = new System.Drawing.Point(93, 0);
            this.linkLigulem.Name = "linkLigulem";
            this.linkLigulem.Size = new System.Drawing.Size(68, 13);
            this.linkLigulem.TabIndex = 1;
            this.linkLigulem.TabStop = true;
            this.linkLigulem.Text = "User:Ligulem";
            this.linkLigulem.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLigulem_LinkClicked);
            // 
            // linkMets501
            // 
            this.linkMets501.AutoSize = true;
            this.linkMets501.Location = new System.Drawing.Point(88, 13);
            this.linkMets501.Name = "linkMets501";
            this.linkMets501.Size = new System.Drawing.Size(73, 13);
            this.linkMets501.TabIndex = 4;
            this.linkMets501.TabStop = true;
            this.linkMets501.Text = "User:Mets501";
            this.linkMets501.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMets501_LinkClicked);
            // 
            // linkMaxSem
            // 
            this.linkMaxSem.AutoSize = true;
            this.linkMaxSem.Location = new System.Drawing.Point(88, 0);
            this.linkMaxSem.Name = "linkMaxSem";
            this.linkMaxSem.Size = new System.Drawing.Size(73, 13);
            this.linkMaxSem.TabIndex = 3;
            this.linkMaxSem.TabStop = true;
            this.linkMaxSem.Text = "User:MaxSem";
            this.linkMaxSem.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMaxSem_LinkClicked);
            // 
            // linkBugs
            // 
            this.linkBugs.AutoSize = true;
            this.linkBugs.Location = new System.Drawing.Point(3, 0);
            this.linkBugs.Name = "linkBugs";
            this.linkBugs.Size = new System.Drawing.Size(66, 13);
            this.linkBugs.TabIndex = 0;
            this.linkBugs.TabStop = true;
            this.linkBugs.Text = "Bugs reports";
            this.linkBugs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBugs_LinkClicked);
            // 
            // linkFeatureRequests
            // 
            this.linkFeatureRequests.AutoSize = true;
            this.linkFeatureRequests.Location = new System.Drawing.Point(3, 13);
            this.linkFeatureRequests.Name = "linkFeatureRequests";
            this.linkFeatureRequests.Size = new System.Drawing.Size(86, 13);
            this.linkFeatureRequests.TabIndex = 1;
            this.linkFeatureRequests.TabStop = true;
            this.linkFeatureRequests.Text = "Feature requests";
            this.linkFeatureRequests.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkFeatureRequests_LinkClicked);
            // 
            // linkReedy
            // 
            this.linkReedy.AutoSize = true;
            this.linkReedy.Location = new System.Drawing.Point(88, 26);
            this.linkReedy.Name = "linkReedy";
            this.linkReedy.Size = new System.Drawing.Size(63, 13);
            this.linkReedy.TabIndex = 5;
            this.linkReedy.TabStop = true;
            this.linkReedy.Text = "User:Reedy";
            this.linkReedy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReedy_LinkClicked);
            // 
            // linkKingboy
            // 
            this.linkKingboy.AutoSize = true;
            this.linkKingboy.Location = new System.Drawing.Point(3, 13);
            this.linkKingboy.Name = "linkKingboy";
            this.linkKingboy.Size = new System.Drawing.Size(76, 13);
            this.linkKingboy.TabIndex = 1;
            this.linkKingboy.TabStop = true;
            this.linkKingboy.Text = "User:Kingboyk";
            this.linkKingboy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkKingboy_LinkClicked);
            // 
            // linkMartinp23
            // 
            this.linkMartinp23.AutoSize = true;
            this.linkMartinp23.Location = new System.Drawing.Point(3, 26);
            this.linkMartinp23.Name = "linkMartinp23";
            this.linkMartinp23.Size = new System.Drawing.Size(79, 13);
            this.linkMartinp23.TabIndex = 2;
            this.linkMartinp23.TabStop = true;
            this.linkMartinp23.Text = "User:Martinp23";
            this.linkMartinp23.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMartinp23_LinkClicked);
            // 
            // lblDevs
            // 
            this.lblDevs.BackColor = System.Drawing.Color.Transparent;
            this.lblDevs.Location = new System.Drawing.Point(9, 126);
            this.lblDevs.Name = "lblDevs";
            this.lblDevs.Size = new System.Drawing.Size(174, 13);
            this.lblDevs.TabIndex = 4;
            this.lblDevs.Text = "Now developed and maintained by:";
            // 
            // lblTimeAndEdits
            // 
            this.lblTimeAndEdits.Location = new System.Drawing.Point(234, 235);
            this.lblTimeAndEdits.Name = "lblTimeAndEdits";
            this.lblTimeAndEdits.Size = new System.Drawing.Size(161, 13);
            this.lblTimeAndEdits.TabIndex = 10;
            this.lblTimeAndEdits.Visible = false;
            // 
            // lblNETVersion
            // 
            this.lblNETVersion.AutoSize = true;
            this.lblNETVersion.Location = new System.Drawing.Point(3, 13);
            this.lblNETVersion.Name = "lblNETVersion";
            this.lblNETVersion.Size = new System.Drawing.Size(72, 13);
            this.lblNETVersion.TabIndex = 1;
            this.lblNETVersion.Text = ".NET version:";
            // 
            // lblIEVersion
            // 
            this.lblIEVersion.AutoSize = true;
            this.lblIEVersion.Location = new System.Drawing.Point(3, 0);
            this.lblIEVersion.Name = "lblIEVersion";
            this.lblIEVersion.Size = new System.Drawing.Size(124, 13);
            this.lblIEVersion.TabIndex = 0;
            this.lblIEVersion.Text = "Internet Explorer version:";
            // 
            // lblOSVersion
            // 
            this.lblOSVersion.AutoSize = true;
            this.lblOSVersion.Location = new System.Drawing.Point(3, 26);
            this.lblOSVersion.Name = "lblOSVersion";
            this.lblOSVersion.Size = new System.Drawing.Size(91, 13);
            this.lblOSVersion.TabIndex = 2;
            this.lblOSVersion.Text = "Windows version:";
            // 
            // lblAWBVersion
            // 
            this.lblAWBVersion.AutoSize = true;
            this.lblAWBVersion.Location = new System.Drawing.Point(3, 0);
            this.lblAWBVersion.Name = "lblAWBVersion";
            this.lblAWBVersion.Size = new System.Drawing.Size(48, 13);
            this.lblAWBVersion.TabIndex = 0;
            this.lblAWBVersion.Text = "Version: ";
            // 
            // lblDetails
            // 
            this.lblDetails.BackColor = System.Drawing.Color.Transparent;
            this.lblDetails.Location = new System.Drawing.Point(12, 193);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(42, 13);
            this.lblDetails.TabIndex = 6;
            this.lblDetails.Text = "Details:";
            // 
            // lblOriginalDevs
            // 
            this.lblOriginalDevs.BackColor = System.Drawing.Color.Transparent;
            this.lblOriginalDevs.Location = new System.Drawing.Point(7, 81);
            this.lblOriginalDevs.Name = "lblOriginalDevs";
            this.lblOriginalDevs.Size = new System.Drawing.Size(100, 13);
            this.lblOriginalDevs.TabIndex = 2;
            this.lblOriginalDevs.Text = "Original developers:";
            // 
            // linkJogers
            // 
            this.linkJogers.AutoSize = true;
            this.linkJogers.Location = new System.Drawing.Point(3, 0);
            this.linkJogers.Name = "linkJogers";
            this.linkJogers.Size = new System.Drawing.Size(63, 13);
            this.linkJogers.TabIndex = 0;
            this.linkJogers.TabStop = true;
            this.linkJogers.Text = "User:Jogers";
            this.linkJogers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkJogers_LinkClicked);
            // 
            // UsageStatsLabel
            // 
            this.UsageStatsLabel.AutoSize = true;
            this.UsageStatsLabel.Location = new System.Drawing.Point(3, 26);
            this.UsageStatsLabel.Name = "UsageStatsLabel";
            this.UsageStatsLabel.Size = new System.Drawing.Size(81, 13);
            this.UsageStatsLabel.TabIndex = 2;
            this.UsageStatsLabel.TabStop = true;
            this.UsageStatsLabel.Text = "Usage statistics";
            this.UsageStatsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UsageStatsLabel_LinkClicked);
            // 
            // lblRevision
            // 
            this.lblRevision.AutoSize = true;
            this.lblRevision.Location = new System.Drawing.Point(3, 13);
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Size = new System.Drawing.Size(35, 13);
            this.lblRevision.TabIndex = 1;
            this.lblRevision.Text = "SVN: ";
            // 
            // flwDevs
            // 
            this.flwDevs.Controls.Add(this.linkJogers);
            this.flwDevs.Controls.Add(this.linkKingboy);
            this.flwDevs.Controls.Add(this.linkMartinp23);
            this.flwDevs.Controls.Add(this.linkMaxSem);
            this.flwDevs.Controls.Add(this.linkMets501);
            this.flwDevs.Controls.Add(this.linkReedy);
            this.flwDevs.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwDevs.Location = new System.Drawing.Point(28, 142);
            this.flwDevs.Name = "flwDevs";
            this.flwDevs.Size = new System.Drawing.Size(200, 48);
            this.flwDevs.TabIndex = 5;
            // 
            // flwOriginalDevs
            // 
            this.flwOriginalDevs.Controls.Add(this.linkBluemoose);
            this.flwOriginalDevs.Controls.Add(this.linkLigulem);
            this.flwOriginalDevs.Location = new System.Drawing.Point(28, 97);
            this.flwOriginalDevs.Name = "flwOriginalDevs";
            this.flwOriginalDevs.Size = new System.Drawing.Size(200, 22);
            this.flwOriginalDevs.TabIndex = 3;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.linkBugs);
            this.flowLayoutPanel3.Controls.Add(this.linkFeatureRequests);
            this.flowLayoutPanel3.Controls.Add(this.UsageStatsLabel);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(28, 209);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(200, 44);
            this.flowLayoutPanel3.TabIndex = 7;
            // 
            // flwOSVersion
            // 
            this.flwOSVersion.Controls.Add(this.lblIEVersion);
            this.flwOSVersion.Controls.Add(this.lblNETVersion);
            this.flwOSVersion.Controls.Add(this.lblOSVersion);
            this.flwOSVersion.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwOSVersion.Location = new System.Drawing.Point(234, 41);
            this.flwOSVersion.Name = "flwOSVersion";
            this.flwOSVersion.Size = new System.Drawing.Size(254, 43);
            this.flwOSVersion.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblAWBVersion);
            this.flowLayoutPanel1.Controls.Add(this.lblRevision);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 41);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(216, 37);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(500, 265);
            this.Controls.Add(this.lblTimeAndEdits);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.txtWarning);
            this.Controls.Add(this.flwOSVersion);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.flwDevs);
            this.Controls.Add(this.lblDevs);
            this.Controls.Add(this.flwOriginalDevs);
            this.Controls.Add(this.lblOriginalDevs);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.linkAWBPage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.TopMost = true;
            this.flwDevs.ResumeLayout(false);
            this.flwDevs.PerformLayout();
            this.flwOriginalDevs.ResumeLayout(false);
            this.flwOriginalDevs.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flwOSVersion.ResumeLayout(false);
            this.flwOSVersion.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtWarning;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel linkBluemoose;
        private System.Windows.Forms.LinkLabel linkAWBPage;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblAWBVersion;
        private System.Windows.Forms.Label lblOSVersion;
        private System.Windows.Forms.Label lblIEVersion;
        private System.Windows.Forms.Label lblNETVersion;
        private System.Windows.Forms.LinkLabel linkLigulem;
        private System.Windows.Forms.Label lblTimeAndEdits;
        private System.Windows.Forms.Label lblDevs;
        private System.Windows.Forms.LinkLabel linkMets501;
        private System.Windows.Forms.LinkLabel linkMaxSem;
        private System.Windows.Forms.LinkLabel linkBugs;
        private System.Windows.Forms.LinkLabel linkFeatureRequests;
        private System.Windows.Forms.LinkLabel linkReedy;
        private System.Windows.Forms.LinkLabel linkKingboy;
        private System.Windows.Forms.LinkLabel linkMartinp23;
        private System.Windows.Forms.Label lblOriginalDevs;
        private System.Windows.Forms.LinkLabel linkJogers;
        private System.Windows.Forms.LinkLabel UsageStatsLabel;
        private System.Windows.Forms.Label lblRevision;
        private System.Windows.Forms.FlowLayoutPanel flwDevs;
        private System.Windows.Forms.FlowLayoutPanel flwOriginalDevs;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flwOSVersion;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
