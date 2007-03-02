' This little console app can be used for debugging and converting to C#
Module Module1

    Sub Main()
        Dim InterWikiListsItem As New Regex("\[\[(?<site>simple):(?<text>.*?)\]\]", _
           RegexOptions.Compiled Or RegexOptions.IgnoreCase) ' This object is pretending to be InterWikisList[i],
        ' and goes inside your loop.
        ' The regexes in the list will have to be modified to provide <site> and <text> capture groups

        ' ArticleText is already declared in your scenario, but here's some wiki text for this little app to use:
        Dim ArticleText As String = "fication of the Royal Family]\r\n\r\n\r\n\r\n\r" & _
           "\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n[[ca:Olga de Rússia (gran duquessa de Rússia)]]\r\n" & _
           "[[de:Olga Nikolajewna Romanowa (1895–1918)]]\r\n[[es:Olga Nikolaievna Romanova]]\r\n" & _
           "[[fr:Olga Nicolaevna de Russie]]\r\n[[nl:Olga Nikolajevna van Rusland (1895-1918)]]\r\n" & _
           "[[ja:オリガ皇女]]\r\n[[pl:Olga Romanowa]]\r\n[[ru:Ольга Николаевна]]\r\n[[Si" & _
           "mple:Olga Nicolaievna Romanova]]\r\n[[sImple:Simon]][[sv:Storfurstinnan Olga]]"""

        InterWikiListsItem.Replace(ArticleText, AddressOf IWMatchEval) ' this replaces your if .match and your 
        ' if.replace. We simply match and replace at the same time (the replacement delegate won't get called 
        ' If there Then 's no match)
    End Sub

    Private Function IWMatchEval(ByVal match As Match) As String
        ' This is a delegate function. It receives a match object for each match in ArticleText and 
        ' returns the replacement text
        IWMatchEval = ("[[" & match.Groups("site").ToString.ToLower & ":" & match.Groups("text").ToString & "]]")
        Debug.Print(IWMatchEval)
    End Function

End Module
