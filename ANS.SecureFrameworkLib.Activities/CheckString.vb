Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check string")>
Public Class CheckString
    Inherits SFActivity

    
    <Category("Input")>
    <DisplayName("Default value")>
    Public Property DefaultValue As InArgument(Of String) = Nothing


    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckString(Config.Get(context), Name.Get(context), IsRequired, DefaultValue.Get(context))
    End Sub

End Class