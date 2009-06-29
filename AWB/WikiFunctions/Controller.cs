﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.Background;
using WikiFunctions.API;

namespace WikiFunctions
{
    /// <summary>
    /// This class controls editing process in one wiki
    /// </summary>
    public class Controller
    {
        private AsyncApiEdit Editor;

        public SiteInfo Site
        { get; private set; }

        public bool IsBot
        { get; private set; }

        public bool IsSysop
        {
            get { return Editor.User.IsSysop; }
        }

        private readonly static Regex Message = new Regex("<!--[Mm]essage:(.*?)-->", RegexOptions.Compiled);
        private readonly static Regex VersionMessage = new Regex("<!--VersionMessage:(.*?)\\|\\|\\|\\|(.*?)-->", RegexOptions.Compiled);
        private readonly static Regex Underscores = new Regex("<!--[Uu]nderscores:(.*?)-->", RegexOptions.Compiled);

        /// <summary>
        /// Matches <head> on right-to-left wikis
        /// </summary>
        private static readonly Regex HeadRTL = new Regex("<html [^>]*? dir=\"rtl\">", RegexOptions.Compiled);

        /// <summary>
        /// Checks log in status, registered and version.
        /// </summary>
        public WikiStatusResult UpdateWikiStatus()
        {
            try
            {
                string typoPostfix = "";

                //TODO: login?
                Site = new SiteInfo(Editor);

                //load version check page
                BackgroundRequest br = new BackgroundRequest();
                br.GetHTML(
                    "http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/Version&action=raw");

                //load check page
                string url;
                if (Variables.IsWikia)
                    url = "http://www.wikia.com/wiki/index.php?title=Wikia:AutoWikiBrowser/CheckPage&action=edit";
                else if ((Variables.Project == ProjectEnum.wikipedia) && (Variables.LangCode == LangCodeEnum.ar))
                    url = "http://ar.wikipedia.org/w/index.php?title=%D9%88%D9%8A%D9%83%D9%8A%D8%A8%D9%8A%D8%AF%D9%8A%D8%A7:%D8%A7%D9%84%D8%A3%D9%88%D8%AA%D9%88%D9%88%D9%8A%D9%83%D9%8A_%D8%A8%D8%B1%D8%A7%D9%88%D8%B2%D8%B1/%D9%85%D8%B3%D9%85%D9%88%D8%AD&action=edit";
                else
                    url = Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPage&action=edit";

                string strText = Editor.Editor.HttpGet(url);

                Variables.RTL = HeadRTL.IsMatch(Editor.S.ToString());

                if (Variables.IsWikia)
                {
                    //this object loads a local checkpage on Wikia
                    //it cannot be used to approve users, but it could be used to set some settings
                    //such as underscores and pages to ignore
                    AsyncApiEdit webBrowserWikia = (AsyncApiEdit)Editor.Clone();
                    webBrowserWikia.Editor.Open(Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPage&action=edit");
                    //webBrowserWikia.Wait();
                    try
                    {
                        Variables.LangCode = Variables.ParseLanguage(webBrowserWikia.GetScriptingVar("wgContentLanguage"));
                    }
                    catch
                    {
                        // use English if language not recognized
                        Variables.LangCode = LangCodeEnum.en;
                    }
                    typoPostfix = "-" + Variables.ParseLanguage(webBrowserWikia.GetScriptingVar("wgContentLanguage"));
                    string s = webBrowserWikia.Page.Text;

                    // selectively add content of the local checkpage to the global one
                    strText += Message.Match(s).Value
                        /*+ Underscores.Match(s).Value*/
                               + WikiRegexes.NoGeneralFixes.Match(s);

                }

                if (Variables.IsCustomProject)
                {
                    try
                    {
                        Variables.LangCode = 
                            Variables.ParseLanguage(Site.ContentLanguage);
                    }
                    catch
                    {
                        // use English if language not recognized
                        Variables.LangCode = LangCodeEnum.en;
                    }
                }

                br.Wait();
                string strVersionPage = (string)br.Result;

                //see if this version is enabled
                if (!strVersionPage.Contains(AWBVersion + " enabled"))
                {
                    IsBot = WikiStatus = false;
                    return WikiStatusResult.OldVersion;
                }

                // else
                if (!WeAskedAboutUpdate && strVersionPage.Contains(AWBVersion + " enabled (old)"))
                {
                    WeAskedAboutUpdate = true;
                    if (
                        MessageBox.Show(
                            "This version has been superceeded by a new version.  You may continue to use this version or update to the newest version.\r\n\r\nWould you like to automatically upgrade to the newest version?",
                            "Upgrade?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Match version = Regex.Match(strVersionPage, @"<!-- Current version: (.*?) -->");
                        if (version.Success && version.Groups[1].Value.Length == 4)
                        {
                            System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) +
                                                             "\\AWBUpdater.exe");
                        }
                        else if (
                            MessageBox.Show("Error automatically updating AWB.  Load the download page instead?",
                                            "Load download page?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Tools.OpenURLInBrowser("http://sourceforge.net/project/showfiles.php?group_id=158332");
                        }
                    }
                }

                CheckPageText = strText;

                //AWB does not support any skin other than Monobook
                if (Editor.GetScriptingVar("skin") == "cologneblue")
                {
                    MessageBox.Show("This software does not support the Cologne Blue skin." +
                                    "\r\nPlease choose another skin in your preferences and relogin.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return WikiStatusResult.Null;
                }

                // don't run GetInLogInStatus if we don't have the username, we sometimes get 2 error message boxes otherwise
                bool loggedIn = Editor.User.IsRegistered;

                if (!loggedIn)
                {
                    IsBot = WikiStatus = false;
                    return WikiStatusResult.NotLoggedIn;
                }

                // check if username is globally blacklisted
                foreach (
                    Match m3 in Regex.Matches(strVersionPage, @"badname:\s*(.*)\s*(:?|#.*)$", RegexOptions.IgnoreCase))
                {
                    if (!string.IsNullOrEmpty(m3.Groups[1].Value.Trim()) &&
                        !string.IsNullOrEmpty(Editor.User.Name) &&
                        Regex.IsMatch(Editor.User.Name, m3.Groups[1].Value.Trim(),
                                      RegexOptions.IgnoreCase | RegexOptions.Multiline))
                        return WikiStatusResult.NotRegistered;
                }

                //see if there is a message
                Match m = Message.Match(strText);
                if (m.Success && m.Groups[1].Value.Trim().Length > 0)
                    MessageBox.Show(m.Groups[1].Value, "Automated message", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                //see if there is a version-specific message
                m = VersionMessage.Match(strText);
                if (m.Success && m.Groups[1].Value.Trim().Length > 0 && m.Groups[1].Value == AWBVersion)
                    MessageBox.Show(m.Groups[2].Value, "Automated message", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                m = Regex.Match(strText, "<!--[Tt]ypos" + typoPostfix + ":(.*?)-->");
                if (m.Success && m.Groups[1].Value.Trim().Length > 0)
                    Variables.RetfPath = m.Groups[1].Value.Trim();

                List<string> us = new List<string>();
                foreach (Match m1 in Underscores.Matches(strText))
                {
                    if (m1.Success && m1.Groups[1].Value.Trim().Length > 0)
                        us.Add(m1.Groups[1].Value.Trim());
                }
                if (us.Count > 0) Variables.LoadUnderscores(us.ToArray());

                //don't require approval if checkpage does not exist.
                if (strText.Length < 1)
                {
                    WikiStatus = true;
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }

                if (strText.Contains("<!--All users enabled-->"))
                {
                    //see if all users enabled
                    WikiStatus = true;
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }

                //see if we are allowed to use this softare
                strText = Tools.StringBetween(strText, "<!--enabledusersbegins-->", "<!--enabledusersends-->");

                string strBotUsers = Tools.StringBetween(strText, "<!--enabledbots-->", "<!--enabledbotsends-->");
                string strAdmins = Tools.StringBetween(strText, "<!--adminsbegins-->", "<!--adminsends-->");
                Regex username = new Regex(@"^\*\s*" + Tools.CaseInsensitive(Variables.User.Name)
                                           + @"\s*$", RegexOptions.Multiline);

                if (IsSysop)
                {
                    WikiStatus = true;
                    IsBot = username.IsMatch(strBotUsers);
                    return WikiStatusResult.Registered;
                }

                if (!string.IsNullOrEmpty(Editor.User.Name) && username.IsMatch(strText))
                {
                    //enable botmode
                    IsBot = username.IsMatch(strBotUsers);

                    WikiStatus = true;

                    return WikiStatusResult.Registered;
                }

                IsBot = WikiStatus = false;
                return WikiStatusResult.NotRegistered;
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(ToString(), ex.Message);
                Tools.WriteDebug(ToString(), ex.StackTrace);
                IsBot = WikiStatus = false;
                return WikiStatusResult.Error;
            }
        }

        public void EnsureLoaded()
        {
            if (!bLoaded) UpdateWikiStatus();
        }
    }
}
