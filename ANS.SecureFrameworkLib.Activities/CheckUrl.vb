Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check URL")>
Public Class CheckUrl
    Inherits SFActivity

    <Category("Input")>
    <DisplayName("End with trailing slash")>
    Public Property TrailingSlash As Boolean = False

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckUrl(Config.Get(context), Name.Get(context), IsRequired, TrailingSlash)
    End Sub

End Class