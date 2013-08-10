﻿using System.ComponentModel;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    partial class TypoStatsControl
    {
        private ContextMenuStrip contextMenu;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ToolStripMenuItem miCopyFind;
        private ToolStripMenuItem miCopyReplace;
        private ToolStripMenuItem miClear;
        private ToolStripMenuItem miTestRegex;
        private ToolStripMenuItem miSaveLog;
        private SaveFileDialog saveListDialog;
        private IContainer components;

        public TypoStatsControl()
        {
            InitializeComponent();
        }

        #region Autogenerated stuff
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyFind = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.miClear = new System.Windows.Forms.ToolStripMenuItem();
            this.miTestRegex = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveLog = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyFind,
            this.miCopyReplace,
            this.miClear,
            this.miTestRegex,
            this.miSaveLog});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(171, 114);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // miCopyFind
            // 
            this.miCopyFind.Name = "miCopyFind";
            this.miCopyFind.Size = new System.Drawing.Size(170, 22);
            this.miCopyFind.Text = "Copy &Find part";
            this.miCopyFind.Click += new System.EventHandler(this.miCopyFind_Click);
            // 
            // miCopyReplace
            // 
            this.miCopyReplace.Name = "miCopyReplace";
            this.miCopyReplace.Size = new System.Drawing.Size(170, 22);
            this.miCopyReplace.Text = "Copy &Replace part";
            this.miCopyReplace.Click += new System.EventHandler(this.miCopyReplace_Click);
            // 
            // miClear
            // 
            this.miClear.Name = "miClear";
            this.miClear.Size = new System.Drawing.Size(170, 22);
            this.miClear.Text = "&Clear statistics";
            this.miClear.Click += new System.EventHandler(this.miClear_Click);
            // 
            // miTestRegex
            // 
            this.miTestRegex.Name = "miTestRegex";
            this.miTestRegex.Size = new System.Drawing.Size(170, 22);
            this.miTestRegex.Text = "&Test regex...";
            this.miTestRegex.Click += new System.EventHandler(this.TestRegex);
            // 
            // miSaveLog
            // 
            this.miSaveLog.Name = "miSaveLog";
            this.miSaveLog.Size = new System.Drawing.Size(170, 22);
            this.miSaveLog.Text = "Save log";
            this.miSaveLog.Click += new System.EventHandler(this.miSaveLog_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Find";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Replace";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Total";
            this.columnHeader3.Width = 50;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "No changes";
            this.columnHeader4.Width = 75;
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file|*.txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // TypoStatsControl
            // 
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.ContextMenuStrip = this.contextMenu;
            this.MultiSelect = false;
            this.View = System.Windows.Forms.View.Details;
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
