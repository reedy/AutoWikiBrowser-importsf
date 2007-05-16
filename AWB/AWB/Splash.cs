/*
Splash Screen

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WikiFunctions.Controls;

namespace AutoWikiBrowser
{
    internal sealed partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            lblVersion.Text = "Version " + Program.VersionString;
        }

        private void ClickHandler(object sender, EventArgs e)
        {
            this.Close();
        }

        public void setProgress(int percent)
        {
            progressBar.Value = percent;
        }
    }
}