﻿/*

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

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace WikiFunctions.Parse
{
    public class RegExTypoFix
    {
        public RegExTypoFix()
        {
            MakeRegexes();
        }

        Regex IgnoreRegex = new Regex("133t|-ology|\\(sic\\)|\\[sic\\]|\\[''sic''\\]|\\{\\{sic\\}\\}|spellfixno", RegexOptions.Compiled);
        Dictionary<Regex, string> TypoRegexes = new Dictionary<Regex, string>();
        HideText RemoveText = new HideText(true, false, true);

        private void MakeRegexes()
        {
            try
            {
                TypoRegexes.Clear();
                Dictionary<string, string> TypoStrings = LoadTypos();

                Regex r;
                RegexOptions roptions = RegexOptions.Compiled;
                foreach (KeyValuePair<string, string> k in TypoStrings)
                {
                    try
                    {
                        r = new Regex(k.Key, roptions);
                        TypoRegexes.Add(r, k.Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        MatchCollection Matches;

        public string PerformTypoFixes(string ArticleText, out bool NoChange, out string Summary)
        {
            Summary = "";
            if (IgnoreRegex.IsMatch(ArticleText))
            {
                NoChange = true;
                return ArticleText;
            }

            ArticleText = RemoveText.HideMore(ArticleText);
            string OriginalText = ArticleText;
            string Replace = "";
            string strSummary = "";
            string tempSummary = "";

            foreach (KeyValuePair<Regex, string> k in TypoRegexes)
            {
                Matches = k.Key.Matches(ArticleText);

                if (Matches.Count > 0)
                {
                    Replace = k.Value;
                    ArticleText = k.Key.Replace(ArticleText, Replace);

                    if (Matches[0].Value != Matches[0].Result(Replace))
                    {
                        tempSummary = Matches[0].Value + " → " + Matches[0].Result(Replace);

                        if (Matches.Count > 1)
                            tempSummary += " (" + Matches.Count.ToString() + ")";

                        strSummary += tempSummary + ", ";
                    }
                }
            }

            if (OriginalText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            ArticleText = RemoveText.AddBackMore(ArticleText);

            if (strSummary != "")
            {
                strSummary = ", Typos fixed: " + strSummary.Trim();
                Summary = strSummary;
            }

            return ArticleText;
        }

        public bool DetectTypo(string ArticleText)
        {
            if (IgnoreRegex.IsMatch(ArticleText))
                return false;

            ArticleText = RemoveText.Hide(ArticleText);

            foreach (KeyValuePair<Regex, string> k in TypoRegexes)
            {
                if (k.Key.IsMatch(ArticleText))
                    return true;
            }

            return false;
        }

        private Dictionary<string, string> LoadTypos()
        {
            Dictionary<string, string> TypoStrings = new Dictionary<string, string>();

            Regex TypoRegex = new Regex("<(?:Typo )?word=\"(.*?)\"[ \\t]find=\"(.*?)\"[ \\t]replace=\"(.*?)\" ?/?>");
            try
            {
                string text = "";
                try
                {
                    string s = Variables.RETFPath;

                    if (!s.StartsWith("http:")) s = Variables.URLLong + "index.php?title=" + Tools.WikiEncode(s) + "&action=raw&ctype=text/plain&dontcountme=s";

                    text = Tools.GetHTML(s, Encoding.UTF8);
                }
                catch
                {
                    if (text == "")
                    {
                        if (MessageBox.Show("No list of typos was found.  Would you like to use the list of typos from the English Wikipedia?\r\nOnly choose OK if this is an English wiki.", "Load from English Wikipedia?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                text = Tools.GetHTML("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Typos&action=raw&ctype=text/plain&dontcountme=s", Encoding.UTF8);
                            }
                            catch
                            {
                                MessageBox.Show("There was a problem loading the list of typos.");
                            }
                        }
                    }
                }
                foreach (Match m in TypoRegex.Matches(text))
                {
                    try
                    {
                        TypoStrings.Add(m.Groups[2].Value, m.Groups[3].Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was a problem with the word: " + m.Groups[1].Value + "\r\n\r\n" + ex.Message, "Problem");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return TypoStrings;
        }

        public Dictionary<Regex, string> Typos
        {
            get { return TypoRegexes; }
        }
    }
}
