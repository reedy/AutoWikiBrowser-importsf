/*
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    enum Colour { Red, Green, Blue };

    /// <summary>
    /// A simple "LED" user control
    /// </summary>
    public partial class LED : UserControl
    {
        private Brush br = Brushes.Red;
        private Colour col = Colour.Red;

        public LED()
        {
            InitializeComponent();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillEllipse(br, 2, 2, this.Size.Width - 2, this.Size.Height - 2);
        }

        private Colour Colour
        {
            get { return col; }
            set
            {
                switch (col)
                {
                    case Colour.Blue:
                        br = Brushes.Blue;
                        col = Colour.Blue;
                        break;
                    case Colour.Green:
                        br = Brushes.Green;
                        col = Colour.Green;
                        break;
                    case Colour.Red:
                        br = Brushes.Red;
                        col = Colour.Red;
                        break;
                }

                base.Refresh();
            }
        }
    }
}
