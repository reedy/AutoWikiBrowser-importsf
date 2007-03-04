﻿/*
WikiFunctions
Copyright (C) 2006 Martin Richards

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
using System.Configuration;
using System.Collections;
using System.Web;

[assembly: CLSCompliant(true)]
namespace WikiFunctions.Parse
{
    /// <summary>
    /// Provides functions for editting wiki text, such as formatting and re-categorisation.
    /// </summary>
    public class Parsers
    {
        #region constructor etc.
        public Parsers()
        {//default constructor
            metaDataSorter = new MetaDataSorter(this);
            MakeRegexes();
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="StubWordCount">The number of maximum number of words for a stub.</param>
        public Parsers(int StubWordCount, bool AddHumanKey)
        {
            metaDataSorter = new MetaDataSorter(this);
            StubMaxWordCount = StubWordCount;
            addCatKey = AddHumanKey;
            MakeRegexes();
        }

        private void MakeRegexes()
        {
            //look bad if changed
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|nbsp|thinsp|shy|lrm|rlm|[Pp]rime);", RegexOptions.Compiled), "&amp;$1;");
            //IE6 does like these
            RegexUnicode.Add(new Regex("&#(705|803|596|620|699|700|8652|9408|9848|12288|160|61|x27|39);", RegexOptions.Compiled), "&amp;#$1;");
            
            //Decoder doesn't like these
            RegexUnicode.Add(new Regex("&#(x109[0-9A-Z]{2});", RegexOptions.Compiled), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#((?:277|119|84|x1D|x100)[A-Z0-9a-z]{2,3});", RegexOptions.Compiled), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#(x12[A-Za-z0-9]{3});", RegexOptions.Compiled), "&amp;#$1;");
            
            //interfere with wiki syntax
            RegexUnicode.Add(new Regex("&#(126|x5D|x5B|x7b|x7c|x7d|0?9[13]|0?12[345]|0?0?3[92]);", RegexOptions.Compiled | RegexOptions.IgnoreCase), "&amp;#$1;");
            //not entity, but still wrong
            RegexUnicode.Add(new Regex("(cm| m|mm|km|mi)<sup>2</sup>", RegexOptions.Compiled), "$1²");
            RegexUnicode.Add(new Regex("(cm| m|mm|km|mi)<sup>3</sup>", RegexOptions.Compiled), "$1³");

            RegexTagger.Add(new Regex("\\{\\{(template:)?(wikify|wikify-date|wfy|wiki)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Wikify|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Clean ?up|CU|Clean|Tidy)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Linkless|Orphan)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Linkless|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorized|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Unreferenced|add references|cite[ -]sources?|cleanup-sources?|needs? references|no sources|no references?|not referenced|references|sources|unref|Unreferencedsect|unsourced)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Unreferenced|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(Dab|Disamb|Disambiguation)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(2cc|2LAdisambig|2LCdisambig|2LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{2CC}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(3cc|3LW|Tla|Tla-dab|TLA-disambig|TLAdisambig|3LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{3CC}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(4cc|4LW|4LA|4LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{4CC}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(Bio-dab|Hndisambig)", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Hndis");

            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(Prettytable|Prettytable100|Pt)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex("\\{\\{(?:[Tt]emplate:)?(PAGENAMEE?\\}\\}|[Ll]ived\\||[Bb]io-cats\\|)", RegexOptions.Compiled), "{{subst:$1");

            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|([0-9]{4})\|([0-9]{4})\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:$1 births|$3]]\r\n[[Category:$2 deaths|$3]]");
            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|\|([0-9]{4})\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:Year of birth unknown|$2]]\r\n[[Category:$1 deaths|$2]]");
            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|([0-9]{4})\|\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:$1 births|$2]]\r\n[[Category:Year of death unknown|$2]]");
        }

        Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();
        Dictionary<Regex, string> RegexTagger = new Dictionary<Regex, string>();

        HideText hider = new HideText();
        MetaDataSorter metaDataSorter;
        string testText = "";
        int StubMaxWordCount = 500;
        
        /// <summary>
        /// Sort interwiki link order
        /// </summary>
        public bool sortInterwikiOrder
        {
            get { return boolInterwikiOrder; }
            set { boolInterwikiOrder = value; }
        }
        private bool boolInterwikiOrder = true;

        /// <summary>
        /// The interwiki link order to use
        /// </summary>
        public InterWikiOrderEnum InterWikiOrder
        {
            set { metaDataSorter.InterWikiOrder = value; }
            get { return metaDataSorter.InterWikiOrder; }
        }

        /// <summary>
        /// When set to true, adds key to categories (for people only) when parsed
        /// </summary>
        public bool addCatKey
        {
            get { return boolAddCatKey; }
            set { boolAddCatKey = value; }
        }
        private bool boolAddCatKey = false;

        #endregion

        #region General Parse

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <param name="sortWikis">True, sort interwiki order per pywiki bots, false keep current order.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string ArticleText, string ArticleTitle)
        {
            return metaDataSorter.Sort(ArticleText, ArticleTitle);
        }

        readonly Regex regexFixDates0 = new Regex("([Tt]he |later? |early |mid-)(\\[\\[)?([12][0-9][0-9]0)(\\]\\])?'s(\\]\\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexFixDates1 = new Regex("(January|February|March|April|May|June|July|August|September|October|November|December) ([1-9][0-9]?)(?:st|nd|rd|th)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexFixDates2 = new Regex("([1-9][0-9]?)(?:st|nd|rd|th) (January|February|March|April|May|June|July|August|September|October|November|December)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings1 = new Regex("(== ?)(external links:?|external sites:?|outside links|web ?links:?|exterior links:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings3 = new Regex("(== ?)(reference:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings6 = new Regex("(== ?)(Early|Personal|Adult|Later) Life( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings7 = new Regex("(== ?)(Current|Past|Prior) Members( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadingsCareer = new Regex("(== ?)([a-zA-Z]+) Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        readonly Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|profile|(?:general )?information|background|intro(?:duction)?|summary|bio(?:graphy)?) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixHeadings(ArticleText, ArticleTitle);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText.Trim();
        }

        /// <summary>
        /// Fix ==See also== and similar section common errors. Removes unecessary introductary headings.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string ArticleText, string ArticleTitle)
        {
            ArticleText = Regex.Replace(ArticleText, "^={1,4} ?" + Regex.Escape(ArticleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            ArticleText = RegexBadHeader.Replace(ArticleText, "");

            if (!Regex.IsMatch(ArticleText, "= ?See also ?="))
                ArticleText = regexHeadings0.Replace(ArticleText, "$1See also$3");

            ArticleText = regexHeadings1.Replace(ArticleText, "$1External links$3");
            ArticleText = regexHeadings2.Replace(ArticleText, "$1External link$3");
            ArticleText = regexHeadings3.Replace(ArticleText, "$1Reference$3");
            ArticleText = regexHeadings4.Replace(ArticleText, "$1Source$3");
            ArticleText = regexHeadings5.Replace(ArticleText, "$1Further reading$3");
            ArticleText = regexHeadings6.Replace(ArticleText, "$1$2 life$3");
            ArticleText = regexHeadings7.Replace(ArticleText, "$1$2 members$3");
            ArticleText = regexHeadingsCareer.Replace(ArticleText, "$1$2 career$3");

            return ArticleText;
        }

        /// <summary>
        /// Fix date and decade formatting errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDates(string ArticleText)
        {
            ArticleText = regexFixDates0.Replace(ArticleText, "$1$2$3$4s$5");
            /*
            ArticleText = regexFixDates1.Replace(ArticleText, "$1 $2");
            ArticleText = regexFixDates2.Replace(ArticleText, "$1 $2");
            */
            return ArticleText;
        }

        /// <summary>
        /// Footnote formatting errors per [[WP:FN]].
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixFootnotes(string ArticleText)
        {
            string factTag = "({{[ ]*fact[ ]*}}|{{[ ]*fact[ ]*[\\|][^}]*}}|{{[ ]*facts[ ]*}}|{{[ ]*citequote[ ]*}}|{{[ ]*citation needed[ ]*}}|{{[ ]*cn[ ]*}}|{{[ ]*verification needed[ ]*}}|{{[ ]*verify source[ ]*}}|{{[ ]*verify credibility[ ]*}}|{{[ ]*who[ ]*}}|{{[ ]*failed verification[ ]*}}|{{[ ]*nonspecific[ ]*}}|{{[ ]*dubious[ ]*}}|{{[ ]*or[ ]*}}|{{[ ]*lopsided[ ]*}}|{{[ ]*GR[ ]*[\\|][ ]*[^ ]+[ ]*}}|{{[ ]*[c]?r[e]?f[ ]*[\\|][^}]*}}|{{[ ]*ref[ _]label[ ]*[\\|][^}]*}}|{{[ ]*ref[ _]num[ ]*[\\|][^}]*}})";
            ArticleText = Regex.Replace(ArticleText, "[\\n\\r\\f\\t ]+?" + factTag, "$1");

            // One space/linefeed
            ArticleText = Regex.Replace(ArticleText, "[\\n\\r\\f\\t ]+?<ref([ >])", "<ref$1");
            // remove trailing spaces from named refs
            ArticleText = Regex.Replace(ArticleText, "<ref ([^>]*[^>])[ ]*>", "<ref $1>");
            // removed superscripted punctuation between refs
            ArticleText = Regex.Replace(ArticleText, "(</ref>|<ref[^>]*?/>)<sup>[ ]*[,;-]?[ ]*</sup><ref", "$1<ref");
            ArticleText = Regex.Replace(ArticleText, "(</ref>|<ref[^>]*?/>)[ ]*[,;-]?[ ]*<ref", "$1<ref");

            string LacksPunctuation = "([^\\.,;:!\\?\"'’])";
            string QuestionOrExclam = "([!\\?])";
            string MinorPunctuation = "([\\.,;:])";
            string AnyPunctuation = "([\\.,;:!\\?])";
            string MajorPunctuation = "([,;:!\\?])";
            string Period = "([\\.])";
            string Quote = "([\"'’]*)";
            string Space = "[ ]*";

            string RefTag = "(<ref>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]|</ref[^>])*?</ref>" + "|<ref[^>]*?[^/]>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]" + "|</ref[^>])*?</ref>|<ref[^>]*?/>)";

            string match0a = LacksPunctuation + Quote + factTag + Space + AnyPunctuation;
            string match0b = QuestionOrExclam + Quote + factTag + Space + MajorPunctuation;
            string match0c = MinorPunctuation + Quote + factTag + Space + AnyPunctuation;
            string match0d = QuestionOrExclam + Quote + factTag + Space + Period;

            string match1a = LacksPunctuation + Quote + RefTag + Space + AnyPunctuation;
            string match1b = QuestionOrExclam + Quote + RefTag + Space + MajorPunctuation;
            string match1c = MinorPunctuation + Quote + RefTag + Space + AnyPunctuation;
            string match1d = QuestionOrExclam + Quote + RefTag + Space + Period;

            string oldArticleText = "";

            while (oldArticleText != ArticleText)
            { // repeat for multiple refs together
                oldArticleText = ArticleText;
                ArticleText = Regex.Replace(ArticleText, match0a, "$1$2$4$3");
                ArticleText = Regex.Replace(ArticleText, match0b, "$1$2$4$3");
                ArticleText = Regex.Replace(ArticleText, match0c, "$2$4$3");
                ArticleText = Regex.Replace(ArticleText, match0d, "$1$2$3");

                ArticleText = Regex.Replace(ArticleText, match1a, "$1$2$6$3");
                ArticleText = Regex.Replace(ArticleText, match1b, "$1$2$6$3");
                ArticleText = Regex.Replace(ArticleText, match1c, "$2$6$3");
                ArticleText = Regex.Replace(ArticleText, match1d, "$1$2$3");
            }
            return ArticleText;
        }

        /// <summary>
        /// Applies/removes some excess whitespace from the article
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string ArticleText)
        {
            ArticleText = Regex.Replace(ArticleText, "\r\n(\r\n)+", "\r\n\r\n");

            ArticleText = Regex.Replace(ArticleText, "== ? ?\r\n\r\n==", "==\r\n==");
            ArticleText = ArticleText.Replace("\r\n\r\n(* ?\\[?http)", "\r\n$1");

            ArticleText = Regex.Replace(ArticleText.Trim(), "----+$", "");
            ArticleText = Regex.Replace(ArticleText.Trim(), "<br ?/?>$", "", RegexOptions.IgnoreCase);

            return ArticleText.Trim();
        }

        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string RemoveAllWhiteSpace(string ArticleText)
        {//removes all whitespace
            ArticleText = ArticleText.Replace("\t", " ");
            ArticleText = RemoveWhiteSpace(ArticleText);

            ArticleText = ArticleText.Replace("\r\n\r\n*", "\r\n*");

            ArticleText = Regex.Replace(ArticleText, "  +", " ");
            ArticleText = Regex.Replace(ArticleText, " \r\n", "\r\n");

            ArticleText = Regex.Replace(ArticleText, "==\r\n\r\n", "==\r\n");

            //fix bullet points
            ArticleText = Regex.Replace(ArticleText, "^([\\*#]+) ", "$1", RegexOptions.Multiline);
            ArticleText = Regex.Replace(ArticleText, "^([\\*#]+)", "$1 ", RegexOptions.Multiline);

            //fix heading space
            ArticleText = Regex.Replace(ArticleText, "^(={1,4}) ?(.*?) ?(={1,4})$", "$1$2$3", RegexOptions.Multiline);

            //fix dash spacing
            ArticleText = Regex.Replace(ArticleText, " ?(–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ?", "$1");
            ArticleText = Regex.Replace(ArticleText, "(—|&#151;|&mdash;|&#8212;|&#x2014;|–|&#150;|&ndash;|&#8211;|&#x2013;)", " $1 ");

            return ArticleText.Trim();
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixSyntax(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        readonly Regex SyntaxRegex1 = new Regex("\\[\\[http:\\/\\/([^][]*?)\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex2 = new Regex("\\[http:\\/\\/([^][]*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex3 = new Regex("\\[\\[http:\\/\\/(.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex4 = new Regex("\\[\\[([^][]*?)\\]([^][][^\\]])", RegexOptions.Compiled);
        readonly Regex SyntaxRegex5 = new Regex("([^][])\\[([^][]*?)\\]\\]([^\\]])", RegexOptions.Compiled);

        readonly Regex SyntaxRegex6 = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex7 = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        readonly Regex SyntaxRegex8 = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        readonly Regex SyntaxRegex9 = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        readonly Regex SyntaxRegexTemplate = new Regex("(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", RegexOptions.Singleline | RegexOptions.Compiled);
        readonly Regex SyntaxRegex11 = new Regex("^((#|\\*).*?)<br ?/?>\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        readonly Regex SyntaxRegexItalic = new Regex("<i>(.*?)</i>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegexBold = new Regex("<b>(.*?)</b>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string ArticleText)
        {
            //replace html with wiki syntax
            if (!Regex.IsMatch(ArticleText, "'</?[ib]>|</?[ib]>'", RegexOptions.IgnoreCase))
            {
                ArticleText = SyntaxRegexItalic.Replace(ArticleText, "''$1''");
                ArticleText = SyntaxRegexBold.Replace(ArticleText, "'''$1'''");
            }
            ArticleText = Regex.Replace(ArticleText, "^<hr>|^----+", "----", RegexOptions.Multiline);
                      
            //remove appearance of double line break
            ArticleText = Regex.Replace(ArticleText, "(^==?[^=]*==?)\r\n(\r\n)?----+", "$1", RegexOptions.Multiline);

            //remove unnecessary namespace
            ArticleText = SyntaxRegexTemplate.Replace(ArticleText, "$1$2");

            //remove <br> from lists
            ArticleText = SyntaxRegex11.Replace(ArticleText, "$1\r\n");

            //can cause problems
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]2>(.*?)</[Hh]2>", "==$1==", RegexOptions.Multiline);
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]3>(.*?)</[Hh]3>", "===$1===", RegexOptions.Multiline);
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]4>(.*?)</[Hh]4>", "====$1====", RegexOptions.Multiline);

            //fix uneven bracketing on links
            if (!Regex.IsMatch(ArticleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                ArticleText = SyntaxRegex1.Replace(ArticleText, "[http://$1]");
                ArticleText = SyntaxRegex2.Replace(ArticleText, "[http://$1]");
                ArticleText = SyntaxRegex3.Replace(ArticleText, "[http://$1]");
                ArticleText = SyntaxRegex4.Replace(ArticleText, "[[$1]]$2");
                ArticleText = SyntaxRegex5.Replace(ArticleText, "$1[[$2]]$3");
            }

            //repair bad external links
            ArticleText = SyntaxRegex6.Replace(ArticleText, "[$1]");

            //repair bad internal links
            ArticleText = SyntaxRegex7.Replace(ArticleText, "[[$1]]");
            ArticleText = SyntaxRegex8.Replace(ArticleText, "[[$1]]");
            ArticleText = SyntaxRegex9.Replace(ArticleText, "[[$1#$2]]");

            ArticleText = Regex.Replace(ArticleText, "ISBN: ?([0-9])", "ISBN $1");

            return ArticleText.Trim();
        }        

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixLinks(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;

            string y = "";

            string cat = "[[" + Variables.Namespaces[14];

            foreach (Match m in WikiRegexes.SimpleWikiLink.Matches(ArticleText))
            {
                if (!m.Value.StartsWith(cat) && !m.Value.StartsWith("[[Image:") && !m.Value.StartsWith("[[image:") && !m.Value.StartsWith("[[_") && !m.Value.Contains("|_"))
                {
                    y = m.Value.Replace("_", " ");
                    y = Regex.Replace(y, " ?\\| ?", "|");
                }
                else
                    y = m.Value;

                y = y.Replace("+", "%2B");
                y = HttpUtility.UrlDecode(y);

                ArticleText = ArticleText.Replace(m.Value, y);
            }

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = LinkSimplifier(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string ArticleText)
        {
            string n = "";
            string a = "";
            string b = "";
            string k = "";

            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(ArticleText))
            {
                n = m.Value;
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || Tools.TurnFirstToLower(a) == b)
                {
                    k = WikiRegexes.PipedWikiLink.Replace(n, "[[$2]]");
                    ArticleText = ArticleText.Replace(n, k);
                }
                else if (a + "s" == b || Tools.TurnFirstToLower(a) + "s" == b)
                {
                    k = WikiRegexes.PipedWikiLink.Replace(n, "$2");
                    k = "[[" + k.Substring(0, k.Length - 1) + "]]s";
                    ArticleText = ArticleText.Replace(n, k);
                }
            }

            return ArticleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = BulletExternalLinks(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string ArticleText)
        {
            int intStart = 0;
            string ArticleTextSubstring = "";

            Match m = Regex.Match(ArticleText, "= ? ?external links? ? ?=", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            if (!m.Success)
                return ArticleText;

            intStart = m.Index;

            ArticleTextSubstring = ArticleText.Substring(intStart);
            ArticleText = ArticleText.Substring(0, intStart);
            ArticleTextSubstring = Regex.Replace(ArticleTextSubstring, "(\r\n)?(\r\n)(\\[?http)", "$2* $3");
            ArticleText += ArticleTextSubstring;

            return ArticleText;
        }

        public string FixCategories(string ArticleText)
        {//Fix common spacing/capitalisation errors in categories

            Regex catregex = new Regex("\\[\\[ ?" + Variables.NamespacesCaseInsensitive[14] + " ?(.*?)\\]\\]");
            string cat = "[[" + Variables.Namespaces[14];
            string x = "";

            foreach (Match m in catregex.Matches(ArticleText))
            {
                x = cat + m.Groups[1].Value.Replace("_", " ") + "]]";
                ArticleText = ArticleText.Replace(m.Value, x);
            }

            return ArticleText;
        }

        #endregion

        #region other functions

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = Unicodify(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string ArticleText)
        {
            if (Regex.IsMatch(ArticleText, "<[Mm]ath>"))
                return ArticleText;
            
            ArticleText = Regex.Replace(ArticleText, "&#150;|&#8211;|&#x2013;", "&ndash;");
            ArticleText = Regex.Replace(ArticleText, "&#151;|&#8212;|&#x2014;", "&mdash;");
            ArticleText = ArticleText.Replace(" &amp; ", " & ");
            ArticleText = ArticleText.Replace("&amp;", "&amp;amp;");

            foreach (KeyValuePair<Regex, string> k in RegexUnicode)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }
            try
            {
                ArticleText = HttpUtility.HtmlDecode(ArticleText);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return ArticleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            //ignore date articles
            if (WikiRegexes.Dates2.IsMatch(ArticleTitle))
            {
                NoChange = true;
                return ArticleText;
            }            

            string escTitle = Regex.Escape(ArticleTitle);

            //remove self links first
            Regex tregex = new Regex("\\[\\[(" + Tools.CaseInsensitive(escTitle) + ")\\]\\]");
            if (!ArticleText.Contains("'''"))
            {
                ArticleText = tregex.Replace(ArticleText, "'''$1'''", 1);
            }
            else
            {
                ArticleText = ArticleText.Replace("[[" + ArticleTitle + "]]", ArticleTitle);
                ArticleText = ArticleText.Replace("[[" + Tools.TurnFirstToLower(ArticleTitle) + "]]", Tools.TurnFirstToLower(ArticleTitle));
            }

            if (Regex.IsMatch(ArticleText, "^(\\[\\[|\\*|:)") || Regex.IsMatch(ArticleText, "''' ?" + escTitle + " ?'''", RegexOptions.IgnoreCase))
            {
                NoChange = true;
                return ArticleText;
            }

            ArticleText = hider.HideMore(ArticleText);

            escTitle = Regex.Replace(ArticleTitle, " \\(.*?\\)$", "");
            escTitle = Regex.Escape(escTitle);

            Regex regexBold = new Regex("([^\\[]|^)(" + escTitle + ")([ ,.:;])", RegexOptions.IgnoreCase);

            string strSecondHalf = "";
            if (ArticleText.Length > 80)
            {
                strSecondHalf = ArticleText.Substring(80);
                ArticleText = ArticleText.Substring(0, 80);
            }

            if (ArticleText.Contains("'''"))
            {
                ArticleText = ArticleText + strSecondHalf;
                ArticleText = hider.AddBackMore(ArticleText);
                NoChange = true;
                return ArticleText;
            }

            if (regexBold.IsMatch(ArticleText))
            {
                NoChange = false;
                if (!(ArticleText.IndexOf("Image") != 0))
                {
                    ArticleText = regexBold.Replace(ArticleText, "$1'''$2'''$3", 1);
                }
            }
            else
                NoChange = true;

            ArticleText = ArticleText + strSecondHalf;
            ArticleText = hider.AddBackMore(ArticleText);
            
            return ArticleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string ReplaceImage(string OldImage, string NewImage, string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = ReplaceImage(OldImage, NewImage, ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public string ReplaceImage(string OldImage, string NewImage, string ArticleText)
        {
            //remove image prefix
            OldImage = Regex.Replace(OldImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            NewImage = Regex.Replace(NewImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = Variables.NamespacesCaseInsensitive[6] + Tools.CaseInsensitive(OldImage);
            NewImage = Variables.Namespaces[6] + NewImage;

            ArticleText = Regex.Replace(ArticleText, OldImage, NewImage);

            return ArticleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="Image">The image to remove.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment)
        {
            //remove image prefix
            Image = Regex.Replace(Image, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            Image = Regex.Escape(Image).Replace("\\ ", "[ _]");
            Image = Tools.CaseInsensitive(Image);

            Regex r = new Regex("\\[\\[" + Variables.NamespacesCaseInsensitive[6] + Image + ".*\\]\\]");
            MatchCollection n = r.Matches(ArticleText);

            if (n.Count > 0)
            {
                foreach (Match m in n)
                {
                    string match = m.Value;

                    int i = 0;
                    int j = 0;

                    foreach (char c in match)
                    {
                        if (c == '[')
                            j++;
                        else if (c == ']')
                            j--;

                        i++;

                        if (j == 0)
                        {
                            if (match.Length > i)
                                match = match.Remove(i);

                            Regex t = new Regex(Regex.Escape(match));

                            if (CommentOut)
                                ArticleText = t.Replace(ArticleText, "<!-- " + Comment + " " + match + " -->", 1, m.Index);
                            else
                                ArticleText = t.Replace(ArticleText, "", 1);

                            break;
                        }

                    }
                }
            }
            else
            {
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[6] + ")?" + Image);
                n = r.Matches(ArticleText);

                foreach (Match m in n)
                {
                    Regex t = new Regex(Regex.Escape(m.Value));

                    if (CommentOut)
                        ArticleText = t.Replace(ArticleText, "<!-- " + Comment + " $0 -->", 1, m.Index);
                    else
                        ArticleText = t.Replace(ArticleText, "", 1, m.Index);
                }
            }

            return ArticleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The image to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = RemoveImage(Image, ArticleText, CommentOut, Comment);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = AddCategory(NewCategory, ArticleText, ArticleTitle);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle)
        {
            if (Regex.IsMatch(ArticleText, "\\[\\[ ?[Cc]ategory ?: ?" + Regex.Escape(NewCategory)))
                return ArticleText;

            string cat = "\r\n[[" + Variables.Namespaces[14] + NewCategory + "]]";
            cat = Tools.ApplyKeyWords(ArticleTitle, cat);

            if (ArticleTitle.StartsWith(Variables.Namespaces[10]))
                ArticleText += "<noinclude>" + cat + "\r\n</noinclude>";
            else
                ArticleText += cat;

            return ArticleText;
        }

        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldCategory">The old category to replace.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public string ReCategoriser(string OldCategory, string NewCategory, string ArticleText, out bool NoChange)
        {
            //remove category prefix
            OldCategory = Regex.Replace(OldCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            NewCategory = Regex.Replace(NewCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);

            //format categories properly
            ArticleText = FixCategories(ArticleText);

            testText = ArticleText;

            if (Regex.IsMatch(ArticleText, "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + Tools.CaseInsensitive(Regex.Escape(NewCategory)) + "( ?\\|| ?\\]\\])"))
            {
                ArticleText = RemoveCategory(OldCategory, ArticleText);
            }
            else
            {
                OldCategory = Regex.Escape(OldCategory);
                OldCategory = Tools.CaseInsensitive(OldCategory);                

                OldCategory = Variables.Namespaces[14] + OldCategory + "( ?\\|| ?\\]\\])";
                NewCategory = Variables.Namespaces[14] + NewCategory + "$1";

                ArticleText = Regex.Replace(ArticleText, OldCategory, NewCategory);
            }

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = RemoveCategory(strOldCat, ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string ArticleText)
        {
            //format categories properly
            ArticleText = FixCategories(ArticleText);

            strOldCat = Regex.Escape(strOldCat);
            strOldCat = Tools.CaseInsensitive(strOldCat);

            //broken into two parts to avoid removal of newline when it's not desirable
            string s = "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])\r\n";
            ArticleText = Regex.Replace(ArticleText, s, "");
            s = "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])";
            ArticleText = Regex.Replace(ArticleText, s, "");

            return ArticleText;
        }

        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public string ChangeToDefaultSort(string ArticleText, string ArticleTitle)
        {
            if (!Regex.IsMatch(ArticleText, "defaultsort", RegexOptions.IgnoreCase))
            {
                string sort = "";
                bool allsame = false;
                int matches = 0;

                //format categories properly
                ArticleText = FixCategories(ArticleText);

                string s = "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?(.*?)( ?\\]\\]| ?\\|[^\\|]*?\\]\\])";
                foreach (Match m in Regex.Matches(ArticleText, s))
                {
                    if (m.Result("$2") != "]]")
                    {
                        if (sort == "")
                            sort = m.Result("$2");
                        if (sort == m.Result("$2"))
                        {
                            allsame = true;
                            sort = m.Result("$2");
                        }
                        else
                        {
                            allsame = false;
                            break;
                        }
                        matches++;
                    }
                }
                if (allsame && matches > 1)
                {
                    if (sort.Length > 4) // So that this doesn't get confused by sort keys of "*", " ", etc.
                    {
                        foreach (Match m in Regex.Matches(ArticleText, s))
                        {
                            ArticleText = Regex.Replace(ArticleText, s, "[[" + Variables.Namespaces[14] + "$1]]");
                        }
                        if (sort.TrimStart('|').TrimEnd(']') != ArticleTitle)
                        {
                            ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + sort.TrimStart('|').TrimEnd(']') + "}}";
                        }
                    }
                }
            }
            return ArticleText;
        }

        public string LivingPeople(string ArticleText, out bool NoChange)
        {
            NoChange = true;
            testText = ArticleText;

            if (Regex.IsMatch(ArticleText, "\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase))
                return ArticleText;

            Match m = Regex.Match(ArticleText, "\\[\\[ ?Category ?:[ _]?([0-9]{4})[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase);

            if (!m.Success)
                return ArticleText;

            string birthCat = m.Value;
            int birthYear = int.Parse(m.Groups[1].Value);
            string catKey = "";

            if (birthYear < 1910)
                return ArticleText;

            if (birthCat.Contains("|"))
                catKey = Regex.Match(birthCat, "\\|.*?\\]\\]").Value;
            else
                catKey = "]]";

            ArticleText += "[[Category:Living people" + catKey;

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = Conversions(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string ArticleText)
        {
            //Use proper codes
            ArticleText = ArticleText.Replace("[[zh-tw:", "[[zh:");
            ArticleText = ArticleText.Replace("[[nb:", "[[no:");
            ArticleText = ArticleText.Replace("[[dk:", "[[da:");

            ArticleText = ArticleText.Replace("{{msg:", "{{");

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }

            return ArticleText;
        }

        /// <summary>
        /// Subst'd some user talk templates
        /// </summary>
        /// <param name="TalPageText">The wiki text of the talk page.</param>
        /// <returns>The new text.</returns>
        public string SubstUserTemplates(string TalkPageText)
        {
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(3RR[0-5]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5][ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(welcome[0-6]|welcomeip|anon|welcome-anon)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            return TalkPageText;
        }              
        
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string ArticleText, string ArticleTitle, out bool NoChange, ref string Summary)
        {
            testText = ArticleText;
            ArticleText = Tagger(ArticleText, ArticleTitle, ref Summary);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// adds/removes
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string Tagger(string ArticleText, string ArticleTitle, ref string Summary)
        {
            if (Tools.IsRedirect(ArticleText))
                return ArticleText;

            if (!Tools.IsMainSpace(ArticleTitle)) return ArticleText;

            double Length = ArticleText.Length + 1;

            double LinkCount = 1;
            double Ratio = 0;

            
            string CommentsStripped = WikiRegexes.Comments.Replace(ArticleText, "");
            int words = Tools.WordCount(CommentsStripped);

            //update by-date tags
            foreach (KeyValuePair<Regex, string> k in RegexTagger)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }

            //remove stub tags from long articles
            if (words > StubMaxWordCount && WikiRegexes.Stub.IsMatch(CommentsStripped))
            {
                MatchEvaluator stubEvaluator = new MatchEvaluator(stubChecker);
                ArticleText = WikiRegexes.Stub.Replace(ArticleText, stubEvaluator);

                ArticleText = ArticleText.Trim();
            }

            foreach (Match m in WikiRegexes.Template.Matches(ArticleText))
            {
                if (!m.Value.Contains("stub"))
                    return ArticleText;
            }

            LinkCount = Tools.LinkCount(CommentsStripped);
            Ratio = LinkCount / Length;

            string catHTML = "<div id=\"catlinks\"></div>";
            if (!WikiRegexes.Category.IsMatch(CommentsStripped))
            {
                catHTML = Tools.GetHTML(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(ArticleTitle));
            }

            if (words > 6 && (catHTML.IndexOf("<div id=\"catlinks\">") == -1) && !Regex.IsMatch(ArticleText, @"\{\{[Uu]ncategori[zs]ed"))
            {
                if (WikiRegexes.Stub.IsMatch(CommentsStripped))
                {
                    ArticleText += "\r\n\r\n{{Uncategorizedstub|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    Summary += ", added [[:Category:Uncategorized stubs|uncategorised]] tag";
                }
                else
                {
                    ArticleText += "\r\n\r\n{{Uncategorized|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    Summary += ", added [[:Category:Category needed|uncategorised]] tag";
                }
            }
            else if (LinkCount < 3 && (Ratio < 0.0025))
            {
                ArticleText = "{{Wikify|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Articles that need to be wikified|wikify]] tag";
            }
            else if (CommentsStripped.Length <= 300 && !WikiRegexes.Stub.IsMatch(CommentsStripped))
            {
                ArticleText = ArticleText + "\r\n\r\n\r\n{{stub}}";
                Summary += ", added stub tag";
            }            

            return ArticleText;
        }

        private string stubChecker(Match m)
        {// Replace each Regex cc match with the number of the occurrence.
            if (Regex.IsMatch(m.Value, Variables.SectStub))
                return m.Value;
            else
                return "";
        }

        #endregion

        #region unused

        /// <summary>
        /// Bypasses all redirects in the article
        /// </summary>
        public string BypassRedirects(string ArticleText)
        {//checks links to make them bypass redirects and (TODO) disambigs
            string link = "";
            string article = "";

            MatchCollection simple = WikiRegexes.WikiLinksOnly.Matches(ArticleText);
            MatchCollection piped = WikiRegexes.PipedWikiLink.Matches(ArticleText);

            foreach (Match m in simple)
            {
                //make link
                link = m.Value;
                article = m.Groups[1].Value;

                //get text
                string text = "";
                try
                {
                    text = Tools.GetArticleText(article);
                }
                catch
                {
                    continue;
                }

                //test if redirect
                if (Tools.IsRedirect(text))
                {
                    string directLink = Tools.RedirectTarget(text).Replace("_"," ");
                    directLink = "[[" + directLink + "|" + article + "]]";

                    ArticleText = ArticleText.Replace(link, directLink);
                }
            }
            return ArticleText;
        }

        /// <summary>
        /// Fixes minor problems, such as abbreviations and miscapitalisations
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string MinorThings(string ArticleText)
        {
            ArticleText = Regex.Replace(ArticleText, "[Aa]\\.[Kk]\\.[Aa]\\.?", "also known as");

            ArticleText = ArticleText.Replace("e.g.", "for example");
            ArticleText = ArticleText.Replace("i.e.", "that is");

            MatchCollection ma = Regex.Matches(ArticleText, "(monday|tuesday|wednesday|thursday|friday|saturday|sunday|january|february|april|june|july|august|september|october|november|december)");
            if (ma.Count > 0)
            {
                foreach (Match m in ma)
                    ArticleText = ArticleText.Replace(m.Groups[1].Value, Tools.TurnFirstToUpper(m.Groups[1].Value));
            }

            return ArticleText;
        }

        //[http://en.wikipedia.org/wiki/Dog] to [[Dog]]
        //private string ExtToInternalLinks(string ArticleText)
        //{
        //    foreach (Match m in Regex.Matches(ArticleText, "\\[http://en\\.wikipedia\\.org/wiki/.*?\\]"))
        //    {
        //        string a = HttpUtility.UrlDecode(m.ToString());

        //        if (a.Contains(" "))
        //        {
        //            int intP;
        //            //string a = n;
        //            intP = a.IndexOf(" ");

        //            string b = a.Substring(intP);
        //            a = a.Remove(intP);
        //            b = b.TrimStart();
        //            a = a.Replace("_", " ");

        //            ArticleText = ArticleText.Replace(m.ToString(), a);
        //        }
        //    }

        //    ArticleText = Regex.Replace(ArticleText, "\\[http://en\\.wikipedia\\.org/wiki/(.*?)\\]", "[[$1]]");
        //    return ArticleText;
        //}

        #endregion
    }
}
