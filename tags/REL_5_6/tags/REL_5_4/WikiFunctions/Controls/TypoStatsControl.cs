﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using WikiFunctions.Parse;

namespace WikiFunctions.Controls
{
    public class TypoStatsControl : NoFlickerExtendedListView
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

        private Dictionary<string, TypoStat> Data;
        /// <summary>
        /// Whether this controll accumulates statistics for the whole run
        /// </summary>
        [DefaultValue(false)]
        public bool IsOverallStats
        {
            get { return Data != null; }
            set 
            {
                if (value)
                {
                    if (Data == null) Data = new Dictionary<string, TypoStat>();
                }
                else
                    Data = null;
            }
        }

        public TypoStatsListViewItem SelectedItem
        {
            get
            {
                if (SelectedItems.Count == 0) return null;

                return (TypoStatsListViewItem)SelectedItems[0];
            }
        }

        public int TotalTypos, SelfMatches, FalsePositives, Saves, Pages;

        public string TyposPerSave
        {
            get
            {
                double fixes = TotalTypos - SelfMatches - FalsePositives;
                // fixed 2 decimal places http://www.csharp-examples.net/string-format-double/
                return string.Format("{0:0.00}", fixes/Saves);
            }   
        }

        public void ClearStats()
        {
            if (Data != null) Data.Clear();
            Items.Clear();
            TotalTypos = SelfMatches = FalsePositives = Saves = Pages = 0;
        }

        private void CountStats()
        {
            TotalTypos = SelfMatches = FalsePositives = 0;
            foreach (TypoStat st in Data.Values)
            {
                TotalTypos += st.Total;
                SelfMatches += st.SelfMatches;
                FalsePositives += st.FalsePositives;
            }
        }

        /// <summary>
        /// Updates statistics
        /// </summary>
        /// <param name="stats">Results of typo processing on one page</param>
        /// <param name="skipped">If true, the page was skipped, otherwise skipped</param>
        public void UpdateStats(IEnumerable<TypoStat> stats, bool skipped)
        {
            if (stats == null) return;
            BeginUpdate();
            if (IsOverallStats)
            {
                foreach (TypoStat typo in stats)
                {
                    TypoStat old;
                    if (Data.TryGetValue(typo.Find, out old))
                    {
                        old.Total += typo.Total;
                        old.SelfMatches += typo.SelfMatches;
                        old.FalsePositives += typo.FalsePositives;

                        // if skipped, all changes considered false positives
                        if (skipped) old.FalsePositives += typo.Total - typo.SelfMatches; 

                        old.ListViewItem.Refresh();
                    }
                    else
                    {
                        Data.Add(typo.Find, typo);
                        Items.Add(new TypoStatsListViewItem(typo));
                    }
                }
                Pages++;
                if (!skipped) Saves++;
                CountStats();
            }
            else
            {
                Items.Clear();
                foreach (TypoStat typo in stats)
                {
                    Items.Add(new TypoStatsListViewItem(typo));
                }
            }
            EndUpdate();
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            miCopyFind.Enabled = miCopyReplace.Enabled = miTestRegex.Enabled = SelectedItems.Count > 0;
            miClear.Visible = miSaveLog.Visible = IsOverallStats;
        }

        private void miClear_Click(object sender, EventArgs e)
        {
            ClearStats();
        }

        private void miCopyReplace_Click(object sender, EventArgs e)
        {
            TypoStatsListViewItem typo = SelectedItem;
            if (typo == null) return;

            Tools.CopyToClipboard(typo.Typo.Replace);
        }

        private void miCopyFind_Click(object sender, EventArgs e)
        {
            TypoStatsListViewItem typo = SelectedItem;
            if (typo == null) return;

            Tools.CopyToClipboard(typo.Typo.Find);
        }

        private void TestRegex(object sender, EventArgs e)
        {
            TypoStatsListViewItem typo = SelectedItem;
            if (typo == null) return;

            using (RegexTester t = new RegexTester())
            {
                t.Find = typo.Typo.Find;
                t.Replace = typo.Typo.Replace;
                if (Variables.MainForm != null && Variables.MainForm.EditBox.Enabled)
                    t.ArticleText = Variables.MainForm.EditBox.Text;

                t.ShowDialog(FindForm());
            }
        }

        private void miSaveLog_Click(object sender, EventArgs e)
        {
            if ((Saves > 0) && (saveListDialog.ShowDialog() == DialogResult.OK))
            {
                System.Text.StringBuilder strList = new System.Text.StringBuilder();

                strList.AppendLine("Total: " + TotalTypos);
                strList.AppendLine("No change: " + SelfMatches);
                strList.AppendLine("Typo/save: " + TyposPerSave);

                foreach (TypoStatsListViewItem item in Items)
                {
                    strList.AppendLine(item.SubItems[0].Text + ", " + item.SubItems[1].Text + ", " + item.SubItems[2].Text + ", " + item.SubItems[3].Text);
                }

                Tools.WriteTextFileAbsolutePath(strList.ToString(), saveListDialog.FileName, false);
            }
        }
    }

    public class TypoStatsListViewItem : ListViewItem
    {
        public TypoStat Typo;
        private bool IsYellow;

        public TypoStatsListViewItem(TypoStat stat)
            : base(new [] { stat.Find, stat.Replace, "", "" })
        {
            Typo = stat;
            Typo.ListViewItem = this;
            Refresh();
        }

        public void Refresh()
        {
            SubItems[2].Text = Typo.Total.ToString();
            SubItems[3].Text = Typo.SelfMatches.ToString();

            if ((Typo.Total == Typo.SelfMatches) && !IsYellow)
            {
                BackColor = System.Drawing.Color.Yellow;
                IsYellow = true;
            }
            else if (IsYellow)
            {
                BackColor = System.Drawing.Color.White;
                IsYellow = false;
            }
        }
    }
}
