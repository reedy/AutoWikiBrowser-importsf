Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
    ''' <summary>
    ''' SDK Software's base class for template-manipulating AWB plugins
    ''' </summary>
    Friend MustInherit Class PluginBase
        ' Settings:
        Protected Friend MustOverride ReadOnly Property PluginShortName() As String
        Protected MustOverride ReadOnly Property InspectUnsetParameters() As Boolean
        Protected Const ForceAddition As Boolean = True ' we might want to parameterise this later
        Protected MustOverride ReadOnly Property ParameterBreak() As String
        Protected Friend Const conTemplatePlaceholder As String = "{{xxxTEMPLATExxx}}"
        Protected MustOverride ReadOnly Property OurTemplateHasAlternateNames() As Boolean
        Protected Friend MustOverride ReadOnly Property GenericSettings() As IGenericSettings
        Protected MustOverride ReadOnly Property CategoryTalkClassParm() As String
        Protected MustOverride ReadOnly Property TemplateTalkClassParm() As String
        Friend MustOverride ReadOnly Property HasSharedLogLocation() As Boolean
        Friend MustOverride ReadOnly Property SharedLogLocation() As String
        Friend MustOverride ReadOnly Property HasReqPhotoParam() As Boolean
        Friend MustOverride Sub ReqPhoto()

        ' Objects:
        Protected WithEvents OurMenuItem As ToolStripMenuItem
        Protected Article As Article
        Protected Template As Templating

        ' Enum:
        Friend Enum ProcessTalkPageMode As Integer
            Normal
            ManualAssessment
            NonStandardTalk
        End Enum

        ' Regular expressions:
        Protected MainRegex As Regex
        Protected SecondChanceRegex As Regex
        Protected Friend Const conRegexpLeft As String = "\{\{ *(?<tl>template *:)? *(?<tlname>" ' put "(?<!<nowiki[\n\r]*>\s*)" at the start to ignore nowiki, but was difficult to get secondchanceregex adapted (without becoming too strict) so I gave up. It seemed the engine was trying to do it's best to *avoid* matching this negative group
        Protected Const conRegexpRight As String = _
           ")\b\s*((\s*\|\s*([\|\s]*|(?<parm>[^}{|\s=]*)\s*)+(=\s*(?<val>[^}{|\n\r]*?)\s*)?))*\}\}*" ' put "\s(?!</nowiki[\n\r]*>)" at the end to ignore nowiki
        ' Last known good before loosened up to accept null parameters ("|||")
        '")\b\s*((\s*\|\s*(?<parm>[^}{|\s=]*)\s*)+(=\s*(?<val>[^}{|\n\r]*)\s*)?)*\}\}\s*"
        Protected Const conRegexpRightNotStrict As String = ")\b" '")\b[^}]*"
        Protected Const conRegexpOptions As RegexOptions = RegexOptions.Compiled Or RegexOptions.Multiline Or _
           RegexOptions.IgnoreCase Or RegexOptions.ExplicitCapture
        Protected PreferredTemplateNameRegex As Regex
        Protected MustOverride ReadOnly Property PreferredTemplateName() As String
        Private Shared ReadOnly StubClassTemplateRegex As New Regex(conRegexpLeft & "Stubclass" & _
           ")\s*((\s*\|\s*(?<parm>[^}{|\s=]*)\s*)+(=\s*" & _
           "(?<val>[^|\n\r]*)\s*)?)*\}\}\s*", conRegexpOptions) ' value might contain {{!}} and spaces
        Protected Shared ReadOnly BeatlesKLFSkipRegex As _
           New Regex("WPBeatles|\{\{KLF", RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        Friend Shared Function ConvertSpacesToUnderscores(ByVal Text As String) As String
            Return Text.Replace("_", " ")
        End Function
        Protected Shared Function BuildStandardRegex(ByVal RegexpMiddle As String) As Regex
            Return New Regex(conRegexpLeft & ConvertSpacesToUnderscores(RegexpMiddle) & conRegexpRight, conRegexpOptions)
        End Function
        Protected Shared Function BuildSecondChanceRegex(ByVal RegexpMiddle As String) As Regex
            Return New Regex(conRegexpLeft & ConvertSpacesToUnderscores(RegexpMiddle) & conRegexpRightNotStrict, conRegexpOptions)
        End Function
        Protected Sub BuildPreferredTemplateNameRegex(ByVal HasAlternateNames As Boolean)
            Static PreferredTemplateNameRegexCreator As _
               New Regex("^(?<first>[a-zA-Z]{1})(?<second>.*)", RegexOptions.Compiled)

            If HasAlternateNames Then
                'PreferredTemplateNameRegex = New Regex(PreferredTemplateNameRegexString, RegexOptions.Compiled)
                PreferredTemplateNameRegex = New Regex(PreferredTemplateNameRegexCreator.Replace(PreferredTemplateName, _
                   AddressOf Me.PreferredTemplateNameWikiMatchEvaluator), RegexOptions.Compiled)
            Else
                PreferredTemplateNameRegex = Nothing
            End If
        End Sub
        Protected Function PreferredTemplateNameWikiMatchEvaluator(ByVal match As Match) As String
            Return "^[" & match.Groups("first").Value.ToUpper & match.Groups("first").Value.ToLower & "]" & _
               match.Groups("second").Value & "$"
        End Function

        ' Redirects:
        Protected CheckedRedirects As Boolean
        Protected LastKnownGoodRedirects As String ' We always try to use an up-to-date list from the server, but we can at user's choice fall back to a recent list (generally from XML settings) at user's bidding
        Protected Overridable ReadOnly Property RedirectsParm() As String
            Get
                Return PreferredTemplateName.Replace(" ", "") & "Redir"
            End Get
        End Property
        Friend Sub ReadXMLRedirects(ByVal Reader As System.Xml.XmlTextReader)
            Dim Redirs As String = PluginManager.XMLReadString(Reader, RedirectsParm, LastKnownGoodRedirects)
            If Not Redirs = "" Then LastKnownGoodRedirects = ConvertSpacesToUnderscores(Redirs)
        End Sub
        Friend Sub WriteXMLRedirects(ByVal Writer As System.Xml.XmlTextWriter)
            If Not LastKnownGoodRedirects = "" Then Writer.WriteAttributeString(RedirectsParm, LastKnownGoodRedirects)
        End Sub
        Protected Overridable WriteOnly Property Regexpmiddle() As String
            Set(ByVal value As String)
                PluginManager.AWBForm.TraceManager.WriteBulletedLine("Template:" & PreferredTemplateName & " redirects: " & value, _
                   False, True, False)
                MainRegex = BuildStandardRegex(value)
                SecondChanceRegex = BuildSecondChanceRegex(value)
            End Set
        End Property
        Private Sub CheckRedirects()
            Dim redirectstring As String = ""
            Dim Redirects As List(Of WikiFunctions.Article)
            Dim gotredirects As Boolean

            Try
                Redirects = GetRedirects(PreferredTemplateName)
                gotredirects = True

                redirectstring = ConvertRedirectsToString(Redirects)

                If Redirects.Count = 0 Then
                    Regexpmiddle = PreferredTemplateName
                    BuildPreferredTemplateNameRegex(False)
                Else
                    Regexpmiddle = PreferredTemplateName & "|" & redirectstring
                    BuildPreferredTemplateNameRegex(True)
                End If

                CheckedRedirects = True
            Catch When gotredirects
                Throw
            Catch ex As Exception
                Select Case MessageBox.Show("We caught an error when attempting to get the incoming redirects for Template:" & _
                PreferredTemplateName & "." & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & "* Press Abort to stop AWB" & _
                Microsoft.VisualBasic.vbCrLf & "* Press Retry to try again" & Microsoft.VisualBasic.vbCrLf & _
                "* Press Ignore to use the default redirects list. This may be dangerous if the list is out of date. The list is:" & _
                Microsoft.VisualBasic.vbCrLf & LastKnownGoodRedirects & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
                "The error was:" & Microsoft.VisualBasic.vbCrLf & ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, _
                MessageBoxIcon.Error, MessageBoxDefaultButton.Button3)
                    Case DialogResult.Abort
                        CheckedRedirects = False
                    Case DialogResult.Retry
                        CheckRedirects()
                    Case DialogResult.Ignore
                        'Regexpmiddle = LastKnownGoodRedirects ' this should already be set; if not uncomment out
                        CheckedRedirects = True
                End Select
            End Try
        End Sub
        Protected Shared Function GetRedirects(ByVal Target As String) As List(Of WikiFunctions.Article)
            Dim message As String = "Loading redirects for Template:" & Target

            PluginManager.StatusText.Text = message
            PluginManager.AWBForm.TraceManager.WriteBulletedLine(PluginManager.conAWBPluginName & ":" & message, False, False, True)
            System.Windows.Forms.Application.DoEvents() ' the statusbar text wasn't updating without this; if happens elsewhere may need to write a small subroutine

            Try
                Return WikiFunctions.Lists.GetLists.FromRedirects(False, New String() {"Template:" & Target})
            Catch
                Throw
            Finally
                PluginManager.DefaultStatusText()
            End Try
        End Function
        Protected Shared Function ConvertRedirectsToString(ByRef Redirects As List(Of WikiFunctions.Article)) As String
            Dim tmp As New List(Of WikiFunctions.Article)
            ConvertRedirectsToString = ""

            For Each redirect As WikiFunctions.Article In Redirects
                If redirect.NameSpaceKey = 10 Then
                    ConvertRedirectsToString += redirect.Name.Remove(0, 9) & "|"
                    tmp.Add(redirect) ' hack because can't remove from a collection within a foreach block iterating through it
                End If
            Next

            Redirects = tmp

            Return ConvertRedirectsToString.Trim(New Char() {CChar("|")}) ' would .Remove be quicker? or declaring this as static?
        End Function

        ' AWB pass through:
        Protected Sub InitialiseBase()
            With OurMenuItem
                .CheckOnClick = True
                .Checked = False
                .ToolTipText = "Enable/disable the " & PluginShortName & " plugin"
            End With
            PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(OurMenuItem)

            If Not Me.IAmGeneric Then _
               PluginManager.AddItemToTextBoxInsertionContextMenu(GenericSettings.TextInsertContextMenuStripItems)
        End Sub
        Protected Friend MustOverride Sub Initialise()
        Protected Friend MustOverride Sub ReadXML(ByVal Reader As XmlTextReader)
        Protected Friend MustOverride Sub Reset()
        Protected Friend MustOverride Sub WriteXML(ByVal Writer As XmlTextWriter)
        Protected Friend Function ProcessTalkPage(ByVal A As Article, ByVal AddReqPhotoParm As Boolean) As Boolean
            Return ProcessTalkPage(A, Classification.Code, Importance.Code, False, False, False, _
               ProcessTalkPageMode.Normal, AddReqPhotoParm)
        End Function
        Protected Friend Function ProcessTalkPage(ByVal A As Article, ByVal Classification As Classification, _
        ByVal Importance As Importance, ByVal ForceNeedsInfobox As Boolean, _
        ByVal ForceNeedsAttention As Boolean, ByVal RemoveAutoStub As Boolean, _
        ByVal ProcessTalkPageMode As ProcessTalkPageMode, ByVal AddReqPhotoParm As Boolean) As Boolean

            Me.Article = A

            If SkipIfContains() Then
                A.PluginIHaveFinished(SkipResults.SkipRegex, PluginShortName)
            Else
                ' MAIN
                Dim OriginalArticleText As String = A.AlteredArticleText

                Template = New Templating
                A.AlteredArticleText = MainRegex.Replace(A.AlteredArticleText, AddressOf Me.MatchEvaluator)

                If Template.BadTemplate Then
                    GoTo BadTemplate
                ElseIf Template.FoundTemplate Then
                    ' Even if we've found a good template bizarrely the page could still contain a bad template too 
                    If SecondChanceRegex.IsMatch(A.AlteredArticleText) Then
                        GoTo BadTemplate
                    ElseIf TemplateFound() Then
                        GoTo BadTemplate ' (returns True if bad)
                    End If
                Else
                    If SecondChanceRegex.IsMatch(OriginalArticleText) Then
                        GoTo BadTemplate
                    ElseIf ForceAddition Then
                        TemplateNotFound()
                    End If
                End If

                ' OK, we're in business:
                ProcessTalkPage = True
                If Me.HasReqPhotoParam AndAlso AddReqPhotoParm Then Me.ReqPhoto()

                ProcessArticleFinish()
                If Not ProcessTalkPageMode = PluginBase.ProcessTalkPageMode.Normal Then
                    ProcessArticleFinishNonStandardMode(Classification, Importance, ForceNeedsInfobox, _
                       ForceNeedsAttention, RemoveAutoStub, ProcessTalkPageMode)
                End If

                If Article.ProcessIt Then
                    TemplateWritingAndPlacement()
                Else
                    A.AlteredArticleText = OriginalArticleText
                    A.PluginIHaveFinished(SkipResults.SkipNoChange, PluginShortName)
                End If
            End If

ExitMe:
            Article = Nothing
            Exit Function

BadTemplate:
            A.PluginIHaveFinished(SkipResults.SkipBadTag, PluginShortName) ' TODO: We could get the template placeholder here
            Article = Nothing
            Exit Function
        End Function

        ' Article processing:
        Protected MustOverride Sub InspectUnsetParameter(ByVal Param As String)
        Protected MustOverride Function SkipIfContains() As Boolean
        ''' <summary>
        ''' Send the template to the plugin for preinspection
        ''' </summary>
        ''' <returns>False if OK, TRUE IF BAD TAG</returns>
        Protected MustOverride Function TemplateFound() As Boolean
        Protected MustOverride Sub ProcessArticleFinish()
        Protected MustOverride Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
        Protected MustOverride Sub ImportanceParameter(ByVal Importance As Importance)
        Protected Function MatchEvaluator(ByVal match As Match) As String
            If Not match.Groups("parm").Captures.Count = match.Groups("val").Captures.Count Then
                Template.BadTemplate = True
            Else
                Template.FoundTemplate = True
                Article.PluginCheckTemplateCall(match.Groups("tl").Value, PluginShortName)

                If OurTemplateHasAlternateNames Then PluginCheckTemplateName(match.Groups("tlname").Value) '.Trim)

                If match.Groups("parm").Captures.Count > 0 Then
                    For i As Integer = 0 To match.Groups("parm").Captures.Count - 1

                        Dim value As String = match.Groups("val").Captures(i).Value
                        Dim parm As String = match.Groups("parm").Captures(i).Value

                        If value = "" Then
                            If InspectUnsetParameters Then InspectUnsetParameter(parm)
                        Else
                            Template.AddTemplateParmFromExistingTemplate(parm, value)
                        End If
                    Next
                End If
            End If

            Return conTemplatePlaceholder
        End Function
        Protected Sub PluginCheckTemplateName(ByVal TemplateName As String)
            If Not PreferredTemplateNameRegex Is Nothing Then
                If Not PreferredTemplateNameRegex.Match(TemplateName).Success Then
                    Article.RenamedATemplate(TemplateName, PreferredTemplateName, PluginShortName)
                    GotTemplateNotPreferredName(TemplateName)
                End If
            End If
        End Sub
        Protected MustOverride Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        Protected Overridable Sub TemplateNotFound()
            Article.ArticleHasAMajorChange()
            Template.NewTemplateParm("class", "")
            Article.TemplateAdded(PreferredTemplateName, PluginShortName)
        End Sub
        Private Sub TemplateWritingAndPlacement()
            Dim PutTemplateAtTop As Boolean
            Dim TemplateHeader As String = WriteTemplateHeader(PutTemplateAtTop)

            With Me.Article
                If Template.FoundTemplate Then
                    If Article.PageContainsShellTemplate() Then ' We're putting an existing template back into the shell where we found it
                        PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
                          "Shell template found; leaving " & PreferredTemplateName & " where we found it", PluginShortName, True)

                        TemplateHeader = Article.LineBreakRegex.Replace(TemplateHeader, "") & Template.ParametersToString("")
                        .RestoreTemplateToPlaceholderSpot(TemplateHeader)
                        .CheckLivingAndActivePolInWikiProjectBannerShell()
                    ElseIf PutTemplateAtTop Then ' moving existing tl to top
                        TemplateHeader += Template.ParametersToString(ParameterBreak)
                        .AlteredArticleText = TemplateHeader + Microsoft.VisualBasic.vbCrLf + .AlteredArticleText.Replace(conTemplatePlaceholder, "")
                    Else ' writing it back where it was
                        TemplateHeader += Template.ParametersToString(ParameterBreak)
                        .RestoreTemplateToPlaceholderSpot(TemplateHeader)
                    End If
                Else ' Our template wasn't found, write it into a shell or to the top of the page
                    .PrependTemplateOrWriteIntoShell(Template, ParameterBreak, TemplateHeader, PluginShortName)
                End If
            End With
        End Sub
        Protected Sub AddAndLogNewParamWithAYesValue(ByVal ParamName As String)
            Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, True, False, PluginName:=PluginShortName)
        End Sub
        Protected Sub AddNewParamWithAYesValue(ByVal ParamName As String)
            Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, False, False, PluginName:=PluginShortName)
        End Sub
        Protected Sub AddAndLogNewParamWithAYesValue(ByVal ParamName As String, ByVal ParamAlternativeName As String)
            Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, True, True, _
               ParamAlternativeName:=ParamAlternativeName, PluginName:=PluginShortName)
        End Sub
        Protected Sub AddAndLogEmptyParam(ByVal ParamName As String)
            If Not Template.Parameters.ContainsKey(ParamName) Then Template.NewTemplateParm(ParamName, "", True, _
            Article, PluginShortName)
        End Sub
        Protected Sub AddEmptyParam(ByVal ParamName As String)
            If Not Template.Parameters.ContainsKey(ParamName) Then Template.NewTemplateParm(ParamName, "", _
               False, Article, PluginShortName)
        End Sub
        Protected Sub ProcessArticleFinishNonStandardMode(ByVal Classification As Classification, _
        ByVal Importance As Importance, ByVal ForceNeedsInfobox As Boolean, _
        ByVal ForceNeedsAttention As Boolean, ByVal RemoveAutoStub As Boolean, _
        ByVal ProcessTalkPageMode As ProcessTalkPageMode)
            Select Case Classification
                Case Kingbotk.Classification.Code
                    If ProcessTalkPageMode = PluginBase.ProcessTalkPageMode.NonStandardTalk Then
                        Select Case Me.Article.Namespace
                            Case Namespaces.CategoryTalk
                                Template.NewOrReplaceTemplateParm( _
                                   "class", CategoryTalkClassParm, Me.Article, True, False, _
                                   PluginName:=PluginShortName)
                            Case Namespaces.TemplateTalk
                                Template.NewOrReplaceTemplateParm( _
                                   "class", TemplateTalkClassParm, Me.Article, True, False, _
                                   PluginName:=PluginShortName)
                            Case Namespaces.ImageTalk, Namespaces.PortalTalk, Namespaces.ProjectTalk
                                Template.NewOrReplaceTemplateParm( _
                                   "class", "NA", Me.Article, True, False, PluginName:=PluginShortName)
                        End Select
                    End If
                Case Kingbotk.Classification.Unassessed
                Case Else
                    Template.NewOrReplaceTemplateParm("class", Classification.ToString, Me.Article, False, False)
            End Select

            Select Case Importance
                Case Kingbotk.Importance.Code, Kingbotk.Importance.Unassessed
                Case Else
                    ImportanceParameter(Importance)
            End Select

            If ForceNeedsInfobox Then AddAndLogNewParamWithAYesValue("needs-infobox")

            If ForceNeedsAttention Then AddAndLogNewParamWithAYesValue("attention")

            If RemoveAutoStub Then
                With Me.Article
                    If Template.Parameters.ContainsKey("auto") Then
                        Template.Parameters.Remove("auto")
                        .ArticleHasAMajorChange()
                    End If

                    If StubClassTemplateRegex.IsMatch(.AlteredArticleText) Then
                        .AlteredArticleText = StubClassTemplateRegex.Replace(.AlteredArticleText, "")
                        .ArticleHasAMajorChange()
                    End If
                End With
            End If
        End Sub
        Protected Function WriteOutParameterToHeader(ByVal ParamName As String) As String
            With Template
                WriteOutParameterToHeader = "|" & ParamName & "="
                If .Parameters.ContainsKey(ParamName) Then
                    WriteOutParameterToHeader += .Parameters(ParamName).Value + ParameterBreak
                    .Parameters.Remove(ParamName)
                Else
                    WriteOutParameterToHeader += ParameterBreak
                End If
            End With
        End Function
        Protected Sub StubClass()
            If Me.Article.Namespace = Namespaces.Talk Then
                If GenericSettings.StubClass Then Template.NewOrReplaceTemplateParm("class", "Stub", Article, _
                   True, False, PluginName:=PluginShortName, DontChangeIfSet:=True)

                If GenericSettings.AutoStub _
                AndAlso Template.NewOrReplaceTemplateParm("class", "Stub", Article, True, False, _
                    PluginName:=PluginShortName, DontChangeIfSet:=True) _
                       Then AddAndLogNewParamWithAYesValue("auto")
                ' If add class=Stub (we don't change if set) add auto
            Else
                PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
                   "Ignoring Stub-Class and Auto-Stub options; not a mainspace talk page", PluginShortName, True)
            End If
        End Sub
        Protected Sub ReplaceATemplateWithAYesParameter(ByVal R As Regex, ByVal ParamName As String, _
        ByVal TemplateCall As String, Optional ByVal Replace As Boolean = True)
            With Article
                If (R.Matches(.AlteredArticleText).Count > 0) Then
                    If Replace Then .AlteredArticleText = R.Replace(.AlteredArticleText, "")
                    .DoneReplacement(TemplateCall, ParamName & "=yes", True, PluginShortName)
                    Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, False, False)
                    .ArticleHasAMinorChange()
                End If
            End With
        End Sub
        ''' <summary>
        ''' Checks if params which have two names (V8, v8) exist under both names
        ''' </summary>
        ''' <returns>True if BAD TAG</returns>
        Protected Function CheckForDoublyNamedParameters(ByVal Name1 As String, ByVal Name2 As String) As Boolean
            With Template.Parameters
                If .ContainsKey(Name1) AndAlso .ContainsKey(Name2) Then
                    If .Item(Name1).Value = .Item(Name2).Value Then
                        .Remove(Name2)
                        Article.DoneReplacement(Name2, "", True, PluginShortName)
                    Else
                        Return True
                    End If
                End If
            End With
        End Function

        ' Interraction with manager:
        Friend Property Enabled() As Boolean
            Get
                Return OurMenuItem.Checked
            End Get
            Set(ByVal IsEnabled As Boolean)
                OurMenuItem.Checked = IsEnabled
                ShowHideOurObjects(IsEnabled)
                PluginManager.PluginEnabledStateChanged(Me, IsEnabled)
            End Set
        End Property
        Protected Friend Overridable Sub BotModeChanged(ByVal BotMode As Boolean)
            If BotMode AndAlso GenericSettings.StubClass Then
                GenericSettings.AutoStub = True
                GenericSettings.StubClass = False
            End If
            GenericSettings.StubClassModeAllowed = Not BotMode
        End Sub
        Protected Friend Overridable ReadOnly Property IAmReady() As Boolean
            Get
                If Not CheckedRedirects Then ' we've not checked redirects
                    CheckRedirects() ' check them, and check the variable again
                    If CheckedRedirects Then Return True Else Throw New RedirectsException
                Else
                    Return True
                End If
            End Get
        End Property
        Protected Friend Overridable ReadOnly Property IAmGeneric() As Boolean
            Get
                Return False
            End Get
        End Property

        ' User interface:
        Protected MustOverride Sub ShowHideOurObjects(ByVal Visible As Boolean)

        ' Event handlers:
        Private Sub ourmenuitem_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles OurMenuItem.CheckedChanged
            Enabled = OurMenuItem.Checked
        End Sub

        Public Sub New(ByVal DefaultRegexpmiddle As String, ByVal HasAlternateNames As Boolean)
            LastKnownGoodRedirects = DefaultRegexpmiddle
            Regexpmiddle = DefaultRegexpmiddle
            BuildPreferredTemplateNameRegex(HasAlternateNames)
        End Sub
        Public Sub New(ByVal IAmAGenericTemplate As Boolean)
            If Not IAmAGenericTemplate Then Throw New NotSupportedException
        End Sub
    End Class

    Friend Interface IGenericSettings
        Property AutoStub() As Boolean
        Property StubClass() As Boolean
        WriteOnly Property StubClassModeAllowed() As Boolean
        ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection
        Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
        Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
        Sub XMLReset()
    End Interface

    Friend Class RedirectsException
        Inherits ApplicationException
        Friend Sub New()
        End Sub
        Friend Sub New(ByVal InnerException As Exception)
            MyBase.New("", InnerException)
        End Sub
    End Class
End Namespace