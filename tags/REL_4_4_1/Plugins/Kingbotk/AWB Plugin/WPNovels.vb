'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins

    Friend NotInheritable Class WPNovelSettings
        Implements IGenericSettings

        Private Const conAutoStubParm As String = "NovelsAutoStub"
        Private Const conStubClassParm As String = "NovelsStubClass"
        Private Const conOldPeerReviewParm As String = "NovelsOldPeerReview"
        Private Const conCrimeWGParm As String = "NovelsCrimeWG"
        Private Const conShortStoryWGParm As String = "NovelsShortStoryWG"
        Private Const conSFWGParm As String = "NovelsSFWG"
        Private Const conAusParm As String = "NovelsAusWG"
        Private Const conFanParm As String = "NovelsFantWG"
        Private Const con19thParm As String = "Novels19thWG"

#Region "XML interface"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
            CrimeWG = PluginManager.XMLReadBoolean(Reader, conCrimeWGParm, CrimeWG)
            ShortStoryWG = PluginManager.XMLReadBoolean(Reader, conShortStoryWGParm, ShortStoryWG)
            SFWG = PluginManager.XMLReadBoolean(Reader, conSFWGParm, SFWG)
            AusWG = PluginManager.XMLReadBoolean(Reader, conAusParm, AusWG)
            FantWG = PluginManager.XMLReadBoolean(Reader, conFanParm, FantWG)
            NineteenthCWG = PluginManager.XMLReadBoolean(Reader, con19thParm, NineteenthCWG)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conCrimeWGParm, CrimeWG.ToString)
                .WriteAttributeString(conShortStoryWGParm, ShortStoryWG.ToString)
                .WriteAttributeString(conSFWGParm, SFWG.ToString)
                .WriteAttributeString(conAutoStubParm, AutoStub.ToString)
                .WriteAttributeString(conStubClassParm, StubClass.ToString)
                .WriteAttributeString(conAusParm, AusWG.ToString())
                .WriteAttributeString(conFanParm, FantWG.ToString())
                .WriteAttributeString(con19thParm, NineteenthCWG.ToString())
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset

        End Sub
#End Region

#Region "Properties"
        Friend Property CrimeWG() As Boolean
            Get
                Return CrimeCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CrimeCheckBox.Checked = value
            End Set
        End Property
        Friend Property ShortStoryWG() As Boolean
            Get
                Return ShortStoryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ShortStoryCheckBox.Checked = value
            End Set
        End Property
        Friend Property SFWG() As Boolean
            Get
                Return SFCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SFCheckBox.Checked = value
            End Set
        End Property
        Friend Property AusWG() As Boolean
            Get
                Return AustralianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AustralianCheckBox.Checked = value
            End Set
        End Property
        Friend Property FantWG() As Boolean
            Get
                Return FantasyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                FantasyCheckBox.Checked = value
            End Set
        End Property
        Friend Property NineteenthCWG() As Boolean
            Get
                Return NineteenthCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NineteenthCheckBox.Checked = value
            End Set
        End Property
        Friend Property AutoStub() As Boolean Implements IGenericSettings.AutoStub
            Get
                Return AutoStubCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AutoStubCheckBox.Checked = value
            End Set
        End Property
        Friend Property StubClass() As Boolean Implements IGenericSettings.StubClass
            Get
                Return StubClassCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                StubClassCheckBox.Checked = value
            End Set
        End Property
        WriteOnly Property StubClassModeAllowed() As Boolean Implements IGenericSettings.StubClassModeAllowed
            Set(ByVal value As Boolean)
                StubClassCheckBox.Enabled = value
            End Set
        End Property
        Friend ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
        Implements IGenericSettings.TextInsertContextMenuStripItems
            Get
                Return TextInsertContextMenuStrip.Items
            End Get
        End Property
#End Region

        ' Event handlers:
        Private Sub LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
            Tools.OpenENArticleInBrowser("Template:NovelsWikiProject", False)
        End Sub
        Private Sub NovelsWikiProjectToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NovelsWikiProjectToolStripMenuItem.Click
            PluginManager.EditBoxInsert("{{NovelsWikiProject}}")
        End Sub
        Private Sub NovelinfoboxneededToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NovelinfoboxneededToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("needs-infobox")
        End Sub
        Private Sub NovelinfoboxincompToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NovelinfoboxincompToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("incomp-infobox")
        End Sub
        Private Sub NovelsClassListToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ClassListToolStripMenuItem.Click
            PluginManager.EditBoxInsert("|class=List")
        End Sub
        Private Sub CoverNeededToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles CoverNeededToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("needs-infobox-cover")
        End Sub
        Private Sub StCoverNeededToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StCoverNeededToolStripMenuItem.Click
            PluginManager.EditBoxInsert("|needs-infobox-cover=1st")
        End Sub
        Private Sub ShortStoriesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShortStoriesToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("short-story-task-force")
        End Sub
        Private Sub CrimeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CrimeToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("crime-task-force")
        End Sub
        Private Sub ScienceFictionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScienceFictionToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("sf-task-force")
        End Sub
        Private Sub NeedsAttentionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NeedsAttentionToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("attention")
        End Sub
        Private Sub CollaborationCandidateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CollaborationCandidateToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("collaboration-candidate")
        End Sub
        Private Sub PastCollaborationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PastCollaborationToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("past-collaboration")
        End Sub
        Private Sub PeerReviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PeerReviewToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("peer-review")
        End Sub
        Private Sub OldPeerReviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OldPeerReviewToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("old-peer-review")
        End Sub
        Private Sub AutotaggedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutotaggedToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("auto")
        End Sub
        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub
        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub
        Private Sub AustralianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AustralianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("australian-task-force")
        End Sub
        Private Sub FantasyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            PluginManager.EditBoxInsertYesParam("peer-review")
        End Sub
        Private Sub FantasyToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FantasyToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("fantasy-task-force")
        End Sub
        Private Sub ThCenturyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThCenturyToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("19thC-task-force")
        End Sub
    End Class

    Friend NotInheritable Class WPNovels
        Inherits PluginBase

        ' Regular expressions:
        Private Shared ReadOnly InfoboxIncompleteRegex As New Regex(TemplatePrefix & "Novelinfoboxincomp\s*\}\}[\s\n\r]*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)
        Private Shared ReadOnly InfoboxNeededRegex As New Regex(TemplatePrefix & "Novelinfoboxneeded\s*\}\}[\s\n\r]*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)

        ' Settings:
        Private OurTab As New TabPage("Novels")
        Private WithEvents OurSettingsControl As New WPNovelSettings
        Private Const conEnabled As String = "NovEnabled"
        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return "Novels"
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return "NovelsWikiProject"
            End Get
        End Property
        Protected Overrides ReadOnly Property ParameterBreak() As String
            Get
                Return Microsoft.VisualBasic.vbCrLf
            End Get
        End Property
        Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
            Template.NewOrReplaceTemplateParm("importance", Importance.ToString, Me.Article, False, False)
        End Sub
        Protected Friend Overrides ReadOnly Property GenericSettings() As IGenericSettings
            Get
                Return OurSettingsControl
            End Get
        End Property
        Protected Overrides ReadOnly Property CategoryTalkClassParm() As String
            Get
                Return "Cat"
            End Get
        End Property
        Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
            Get
                Return "Template"
            End Get
        End Property
        Friend Overrides ReadOnly Property HasSharedLogLocation() As Boolean
            Get
                Return True
            End Get
        End Property
        Friend Overrides ReadOnly Property SharedLogLocation() As String
            Get
                Return "Wikipedia:WikiProject Novels/Automation/Logs"
            End Get
        End Property
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return True
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
            AddNewParamWithAYesValue("needs-infobox-cover")
        End Sub
        'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
        '    Get
        '        Return "^[Nn]ovelsWikiProject$"
        '    End Get
        'End Property

        ' Initialisation:
        Friend Sub New(ByVal Manager As PluginManager)
            MyBase.New("Novels|WPNovels") ' Specify alternate names only
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Novels Plugin")
            MyBase.InitialiseBase() ' must set menu item object first
            OurTab.UseVisualStyleBackColor = True
            OurTab.Controls.Add(OurSettingsControl)
        End Sub

        ' Article processing:
        Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
            Get
                Return False
            End Get
        End Property
        Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
        End Sub
        Protected Overrides Function SkipIfContains() As Boolean
            ' None
        End Function
        Protected Overrides Sub ProcessArticleFinish()
            ReplaceATemplateWithAYesParameter(InfoboxIncompleteRegex, "incomp-infobox", _
               "{{[[Template:Novelinfoboxincomp|Novelinfoboxincomp]]}}", True)
            ReplaceATemplateWithAYesParameter(InfoboxNeededRegex, "needs-infobox", _
               "{{[[Template:Novelinfoboxneeded|Novelinfoboxneeded]]}}", True)

            StubClass()
            With OurSettingsControl
                If .CrimeWG Then AddAndLogNewParamWithAYesValue("crime-task-force")
                If .ShortStoryWG Then AddAndLogNewParamWithAYesValue("short-story-task-force")
                If .SFWG Then AddAndLogNewParamWithAYesValue("sf-task-force")
                If .AusWG Then AddAndLogNewParamWithAYesValue("australian-task-force")
                If .FantWG Then AddAndLogNewParamWithAYesValue("fantasy-task-force")
                If .NineteenthCWG Then AddAndLogNewParamWithAYesValue("19thC-task-force")
            End With
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
            ' Currently only WPBio does anything here (if {{musician}} add to musician-work-group)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            WriteTemplateHeader = "{{NovelsWikiProject" & Microsoft.VisualBasic.vbCrLf

            With Template
                WriteTemplateHeader += WriteOutParameterToHeader("class") & _
                   WriteOutParameterToHeader("importance")
            End With
        End Function

        'User interface:
        Protected Overrides Sub ShowHideOurObjects(ByVal Visible As Boolean)
            PluginManager.ShowHidePluginTab(OurTab, Visible)
        End Sub

        ' XML settings:
        Protected Friend Overrides Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
            Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, conEnabled, Enabled)
            If Not blnNewVal = Enabled Then Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs
            OurSettingsControl.ReadXML(Reader)
        End Sub
        Protected Friend Overrides Sub Reset()
            OurSettingsControl.Reset()
        End Sub
        Protected Friend Overrides Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Writer.WriteAttributeString(conEnabled, Enabled.ToString)
            OurSettingsControl.WriteXML(Writer)
        End Sub
    End Class
End Namespace