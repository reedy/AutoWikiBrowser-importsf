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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;

namespace WikiFunctions.Parse
{
    internal sealed class InterWikiComparer : IComparer<string>
    {
        Dictionary<string, int> Order = new Dictionary<string, int>();
        public InterWikiComparer(string[] order)
        {
            int n = 0;
            foreach (string s in order)
            {
                Order.Add("[[" + s, n);
                n++;
            }
        }

        string RawCode(string iw)
        {
            return iw.Substring(0, iw.IndexOf(':'));
        }

        public int Compare(string x, string y)
        {
            //should NOT be enclosed into try ... catch - I'd like to see exceptions if something goes wrong,
            //not quiet missorting --MaxSem
            int ix = Order[RawCode(x)], iy = Order[RawCode(y)];

            if (ix < iy) return -1;
            else if (ix == iy) return 0;
            else return 1;
        }
    }

    public enum InterWikiOrderEnum : byte { LocalLanguageAlpha, LocalLanguageFirstWord, Alphabetical, AlphabeticalEnFirst }
    class MetaDataSorter
    {
        Parsers parser;
        public MetaDataSorter(Parsers p)
        {
            parser = p;

            LoadInterWiki();

            //InterWikisList.Clear();
            //foreach (string s in InterwikiLocalAlpha)
            //    //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled));
            //    InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));

            string s = string.Join("|", InterwikiLocalAlpha);
            s = @"\[\[\s*(" + s + @")\s*:\s*([^\]]*)\s*\]\]";
            FastIW = new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            //create a comparer
            InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
        }

        // now will be generated dynamically using Variables.Stub
        //Regex StubsRegex = new Regex("<!-- ?\\{\\{.*?stub\\}\\}.*?-->|:?\\{\\{.*?stub\\}\\}");
        Regex InterLangRegex = new Regex("<!-- ?(other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex CatCommentRegex = new Regex("<!-- ?cat(egories)? ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string[] InterwikiLocalAlpha;
        private string[] InterwikiLocalFirst;
        private string[] InterwikiAlpha;
        private string[] InterwikiAlphaEnFirst; 
        //List<Regex> InterWikisList = new List<Regex>();
        Regex IWSplit = new Regex(",", RegexOptions.Compiled);

        Regex FastIW;


        private InterWikiComparer Comparer;
        private InterWikiOrderEnum order = InterWikiOrderEnum.LocalLanguageAlpha;
        public InterWikiOrderEnum InterWikiOrder
        {//orders from http://meta.wikimedia.org/wiki/Interwiki_sorting_order
            set
            {
                order = value;

                string[] seq;
                switch (order)
                {
                    case InterWikiOrderEnum.Alphabetical:
                        seq = InterwikiAlpha;
                        break;
                    case InterWikiOrderEnum.AlphabeticalEnFirst:
                        seq = InterwikiAlphaEnFirst;
                        break;
                    case InterWikiOrderEnum.LocalLanguageAlpha:
                        seq = InterwikiLocalAlpha;
                        break;
                    case InterWikiOrderEnum.LocalLanguageFirstWord:
                        seq = InterwikiLocalFirst;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("MetaDataSorter.InterWikiOrder",
                            (System.Exception)null);
                }
                Comparer = new InterWikiComparer(seq);
            }
            get
            {
                return order;
            }
        }

        private void LoadInterWiki()
        {
            try
            {
                WikiFunctions.Browser.WebControl webBrowser = new WikiFunctions.Browser.WebControl();
                webBrowser.Navigate("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/IW&action=edit");
                webBrowser.Wait();
                string text = webBrowser.GetArticleText();

                string interwikiLocalAlphaRaw = remExtra(Tools.StringBetween(text, "<!--InterwikiLocalAlphaBegins-->", "<!--InterwikiLocalAlphaEnds-->").Replace("<!--InterwikiLocalAlphaBegins-->", ""));
                string interwikiLocalFirstRaw = remExtra(Tools.StringBetween(text, "<!--InterwikiLocalFirstBegins-->", "<!--InterwikiLocalFirstEnds-->").Replace("<!--InterwikiLocalFirstBegins--", ""));

                int no = 0;
                int size = IWSplit.Matches(interwikiLocalFirstRaw).Count + 1;
                
                InterwikiLocalAlpha = new string[IWSplit.Matches(interwikiLocalAlphaRaw).Count + 1];

                foreach (string s in interwikiLocalAlphaRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiLocalAlpha[no] = s.Trim().ToLower();
                    no++;
                }

                InterwikiLocalFirst = new string[size];
                no = 0;

                foreach (string s in interwikiLocalFirstRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiLocalFirst[no] = s.Trim().ToLower();
                    no++;
                }

                InterwikiAlpha = (string[])InterwikiLocalFirst.Clone();
                Array.Sort(InterwikiAlpha);

                string[] temp = (string[])InterwikiAlpha.Clone();
                temp[Array.IndexOf(temp, "en")] = "";

                InterwikiAlphaEnFirst = new string[size + 1];
                InterwikiAlphaEnFirst[0] = "en";
                no = 1;

                foreach (string s in temp)
                {
                    if (s.Trim().Length > 0)
                        InterwikiAlphaEnFirst[no] = s;
                    no++;
                }
            }
            catch { }
        }

        private string remExtra(string Input)
        {
            return Input.Replace("\r\n", "").Replace(">", "");
        }

        private string Newline(string s)
        {
            return s.Length == 0 ? s : "\r\n" + s;
        }
                               
        internal string Sort(string ArticleText, string ArticleTitle)
        {
            string strSave = ArticleText;
            try
            {
                ArticleText = Regex.Replace(ArticleText, "<!-- ?\\[\\[en:.*?\\]\\] ?-->", "");

                string strPersonData = Newline(removePersonData(ref ArticleText));
                string strDisambig = Newline(removeDisambig(ref ArticleText));
                string strCategories = Newline(removeCats(ref ArticleText, ArticleTitle));
                string strInterwikis = Newline(interwikis(ref ArticleText));
                string strStub = Newline(removeStubs(ref ArticleText));

                //filter out excess white space and remove "----" from end of article
                ArticleText = Parsers.RemoveWhiteSpace(ArticleText) + "\r\n";
                ArticleText += strDisambig;

                switch (Variables.LangCode)
                {
                    case LangCodeEnum.de:
                        ArticleText += strStub + strCategories + strPersonData;
                        break;
                    case LangCodeEnum.pl:
                        ArticleText += strPersonData + strStub + strCategories;
                        break;
                    case LangCodeEnum.ru:
                        ArticleText += strPersonData + strStub + strCategories;
                        break;
                    case LangCodeEnum.simple:
                        ArticleText += strPersonData + strStub + strCategories;
                        break;
                    default:
                        ArticleText += strPersonData + strCategories + strStub;
                        break;
                }
                return ArticleText + strInterwikis;
            }
            catch(Exception ex)
            {
                if (!ex.Message.Contains("DEFAULTSORT")) ErrorHandler.Handle(ex);
                return strSave;
            }
        }

        private string removeCats(ref string ArticleText, string ArticleTitle)
        {
            List<string> categoryList = new List<string>();
            string x = "";

            Regex r = new Regex("<!-- ? ?\\[\\[" + Variables.NamespacesCaseInsensitive[14] + ".*?(\\]\\]|\\|.*?\\]\\]).*?-->|\\[\\[" + Variables.NamespacesCaseInsensitive[14] + ".*?(\\]\\]|\\|.*?\\]\\])( {0,4}⌊⌊⌊⌊[0-9]{1,4}⌋⌋⌋⌋)?");

            foreach (Match m in r.Matches(ArticleText))
            {
                x = m.Value;
                //add to array, replace underscores with spaces, ignore=
                if (!Regex.IsMatch(x, "\\[\\[Category:(Pages|Categories|Articles) for deletion\\]\\]"))
                {
                    categoryList.Add(x.Replace("_", " "));
                }
            }

            ArticleText = r.Replace(ArticleText, "");

            if (parser.addCatKey)
                categoryList = catKeyer(categoryList, ArticleTitle);

            if (CatCommentRegex.IsMatch(ArticleText))
            {
                string catComment = CatCommentRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(catComment, "");
                categoryList.Insert(0, catComment);
            }

            MatchCollection mc = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (mc.Count > 1) throw new ArgumentException("Page contains multiple {{DEFAULTSORTS}} tags. Metadata sorting cancelled");

            string defaultSort = "";
            try { defaultSort = mc[0].Value; }
            catch { }
            if (defaultSort != "")
                ArticleText = ArticleText.Replace(defaultSort, "");
            defaultSort = WikiRegexes.Defaultsort.Replace(defaultSort, "{{DEFAULTSORT:${key}}}");
            if (defaultSort != "") defaultSort += "\r\n";

            return defaultSort + ListToString(categoryList);
        }

        private string removePersonData(ref string ArticleText)
        {
            string strPersonData = Parsers.GetTemplate(ArticleText, "[Pp]ersondata");

            if (strPersonData != "")
                ArticleText = ArticleText.Replace(strPersonData, "");

            return strPersonData;
        }

        private string removeStubs(ref string ArticleText)
        {
            Regex stubsRegex = new Regex("<!-- ?\\{\\{.*?" + Variables.Stub + "b\\}\\}.*?-->|:?\\{\\{.*?" + Variables.Stub + "\\}\\}");

            List<string> stubList = new List<string>();
            MatchCollection n = stubsRegex.Matches(ArticleText);
            string x = "";

            foreach (Match m in n)
            {
                x = m.Value;
                if (!((Regex.IsMatch(x, Variables.SectStub) || (Regex.IsMatch(x, "tl\\|")))))
                {
                    stubList.Add(x);
                    //remove old stub
                    ArticleText = ArticleText.Replace(x, "");
                }
            }

            if (stubList.Count != 0)
                return ListToString(stubList);
            else
                return "";
        }

        private string removeDisambig(ref string ArticleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return "";

            string strDisambig = "";
            if (WikiRegexes.Disambigs.IsMatch(ArticleText))
            {
                strDisambig = WikiRegexes.Disambigs.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(strDisambig, "");
            }

            return strDisambig;
        }

        private List<string> removeLinkFAs(ref string ArticleText)
        {
            List<string> linkFAList = new List<string>();
            string x = "";
            foreach (Match m in WikiRegexes.LinkFAs.Matches(ArticleText))
            {
                x = m.Value;
                linkFAList.Add(x);
                //remove old LinkFA
                ArticleText = ArticleText.Replace(x, "");
            }

            return linkFAList;
        }

        private string interwikis(ref string ArticleText)
        {
            string interwikis = ListToString(removeLinkFAs(ref ArticleText)) + ListToString(removeInterWikis(ref ArticleText));
            return interwikis;
        }

        private List<string> removeInterWikis(ref string ArticleText)
        {
            List<string> interWikiList = new List<string>();
            //Regex interwikiregex = new Regex(@"\[\[(?<site>.*?):(?<text>.*?)\]\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string site;
            foreach(Match m in FastIW.Matches(ArticleText))
            {
                site = m.Groups[1].Value.ToLower();
                if (site != "ru-sib") // remove links to Zolotaryovpedia now that it has been closed
                    interWikiList.Add("[[" + site + ":" + m.Groups[2].Value + "]]");
            }

            string interWikiComment = "";
            if (InterLangRegex.IsMatch(ArticleText))
            {

                interWikiComment = InterLangRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(interWikiComment, "");
            }

            ArticleText = FastIW.Replace(ArticleText, "");

            if (parser.sortInterwikiOrder)
            {
                interWikiList.Sort(Comparer);
            }
            else
            {
                //keeps existing order
            }

            if (interWikiComment != "") interWikiList.Insert(0, interWikiComment);

            return interWikiList;
        }

        public static string IWMatchEval(Match match)
        {
            string[] textArray = new string[] { "[[", match.Groups["site"].ToString().ToLower(), ":", match.Groups["text"].ToString(), "]]" };
            return string.Concat(textArray);
        }

        private string ListToString(List<string> items)
        {//remove duplicates, and return List as string.

            if (items.Count == 0)
                return "";

            string list = "";
            List<string> uniqueItems = new List<string>();

            //remove duplicates
            foreach (string s in items)
            {
                if (!uniqueItems.Contains(s))
                    uniqueItems.Add(s);
            }

            //add to string
            foreach (string s in uniqueItems)
            {
                list += s + "\r\n";
            }

            return list;
        }

        private List<string> catKeyer(List<string> List, string strName)
        {
            // make key
            strName = Tools.MakeHumanCatKey(strName);

            //add key to cats that need it
            List<string> newCats = new List<string>();
            foreach (string s in List)
            {
                string z = s;
                if (!z.Contains("|"))
                    z = z.Replace("]]", "|" + strName + "]]");

                newCats.Add(z);
            }
            return newCats;
        }
    }
}
