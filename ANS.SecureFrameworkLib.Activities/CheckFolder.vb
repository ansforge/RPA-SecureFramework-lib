Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check folder")>
Public Class CheckFolder
    Inherits SFActivity

    <Category("Input")>
    <DisplayName("Create if not exists")>
    Public Property CreateIfNotExists As Boolean = True

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckFolder(Config.Get(context), Name.Get(context), IsRequired, CreateIfNotExists)
    End Sub

End Class