using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace AwbUpdater
{
    /// <summary>
    /// This class provides helper functions for handling errors and displaying them to users
    /// </summary>
    public partial class ErrorHandler : Form
    {
        public ErrorHandler()
        {
            InitializeComponent();
        }

        private void ErrorHandler_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName;
        }

        /// <summary>
        /// Displays exception information. Should be called from try...catch handlers
        /// </summary>
        /// <param name="ex">Exception object to handle</param>
        new public static void Handle(Exception ex)
        {
            ErrorHandler Handler = new ErrorHandler();

            Handler.txtError.Text = ex.Message;

            Handler.txtDetails.Text = "{{AWB bug\r\n | status      = new <!-- when fixed replace with \"fixed\" -->\r\n | description = Exception: " + ex.GetType().Name + "\r\nMessage: " +
                ex.Message + "\r\nCall stack:" + ex.StackTrace + "\r\n~~~~\r\n | OS          = " + Environment.OSVersion.ToString() + "\r\n | version     = " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Handler.txtDetails.Text += "\r\n}}";

            Handler.ShowDialog();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            System.Threading.Thread.Sleep(1000);
            Clipboard.SetText(txtDetails.Text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start("http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&action=edit&section=new");
            }
            catch { }
        }
    }
}