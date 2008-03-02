/*

Copyright (C) 2007 Martin Richards
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
using System.Text;
using WikiFunctions.Plugin;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions;
using WikiFunctions.Logging;
using WikiFunctions.Parse;
using WikiFunctions.Lists;
using System.Xml.Serialization;

[assembly: CLSCompliant(true)]
namespace AutoWikiBrowser.Plugins.CFD
{
    public sealed class CfdCore : IAWBPlugin
    {
        private ToolStripMenuItem pluginenabledMenuItem = new ToolStripMenuItem("Categories For Deletion plugin");
        private ToolStripMenuItem pluginconfigMenuItem = new ToolStripMenuItem("Configuration");
        private ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About the CFD plugin");
        internal static IAutoWikiBrowser AWB;
        internal static CfdSettings Settings = new CfdSettings();

        public void Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            AWB = sender;

            // Menuitem should be checked when CFD plugin is active and unchecked when not, and default to not!
            pluginenabledMenuItem.CheckOnClick = true;
            PluginEnabled = Settings.Enabled;

            pluginconfigMenuItem.Click += ShowSettings;
            pluginenabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            aboutMenuItem.Click += AboutMenuItemClicked;
            pluginenabledMenuItem.DropDownItems.Add(pluginconfigMenuItem);

            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginenabledMenuItem);
            sender.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);
        }

        public string Name
        { get { return "CFD-Plugin"; } }

        public string WikiName
        { get { return "[[WP:CFD|CFD]] Plugin version " + 
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs)
        {
            //If menu item is not checked, then return
            if (!PluginEnabled || Settings.Categories.Count == 0)
            {
                eventargs.Skip = false;
                return eventargs.ArticleText;
            }

            eventargs.EditSummary = "";
            string text = eventargs.ArticleText;

            Parsers parse = new Parsers();

            
            foreach (KeyValuePair<string, string> p in Settings.Categories)
            {
                bool noChange = true;

                if (p.Value.Length == 0)
                {
                    text = parse.RemoveCategory(p.Key, text, out noChange);
                    if(!noChange) eventargs.EditSummary += ", removed " + Variables.Namespaces[14] + p.Key;
                }
                else
                {
                    text = parse.ReCategoriser(p.Key, p.Value, text, out noChange);
                    if (!noChange) eventargs.EditSummary += ", replaced: " + Variables.Namespaces[14]
                         + p.Key + FindandReplace.Arrow + Variables.Namespaces[14] + p.Value;
                }
                if (!noChange) text = Regex.Replace(text, "<includeonly>[\\s\\r\\n]*\\</includeonly>", "");
            }

            eventargs.Skip = (text == eventargs.ArticleText) && Settings.Skip;

            return text;
        }

        // TODO: Why doesn't plugin load and save settings? Perhaps it couldn't be made to work at the time? (should be possible now, as I fixed the AWB plugin settings code to get kingbotk to work)
        public void LoadSettings(object[] prefs)
        {
            //Settings = (CfdSettings)Prefs[0];
            //PluginEnabled = Settings.Enabled;
        }

        public object[] SaveSettings()
        {
            //Settings.Enabled = PluginEnabled;

            //object[] Prefs = new object[1];
            //Prefs[0] = Settings;

            return null;//Prefs;
        }

        public void Reset()
        {
            //set default settings
            Settings = new CfdSettings();
            PluginEnabled = false;
        }

        public void Nudge(out bool Cancel) { Cancel = false; }
        public void Nudged(int Nudges) { }

        private void ShowSettings(Object sender, EventArgs e)
        {
            CfdOptions o = new CfdOptions();

            o.Show();
        }

        private bool PluginEnabled { get { return pluginenabledMenuItem.Checked; } 
            set { pluginenabledMenuItem.Checked = value; }  }

        private void PluginEnabledCheckedChange(Object sender, EventArgs e)
        {
            Settings.Enabled = PluginEnabled;
            if (PluginEnabled)
                AWB.NotifyBalloon("CFD plugin enabled", ToolTipIcon.Info);
            else
                AWB.NotifyBalloon("CFD plugin disabled", ToolTipIcon.Info);
        }

        private void AboutMenuItemClicked(Object sender, EventArgs e)
        {
            new AboutBox().Show();
        }
    }

    [Serializable]
    internal sealed class CfdSettings
    { // TODO: This is crap!
        public bool Enabled;
        public Dictionary<string, string> Categories = new Dictionary<string, string>();
        public bool Skip = true;
    }

}

