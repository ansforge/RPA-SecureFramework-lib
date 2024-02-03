Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check file")>
Public Class CheckFile
    Inherits SFActivity

    <Category("Input")>
    <DisplayName("File must exist")>
    Public Property MustExist As Boolean
    
    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckFile(Config.Get(context), Name.Get(context), IsRequired, MustExist)
    End Sub

End Class