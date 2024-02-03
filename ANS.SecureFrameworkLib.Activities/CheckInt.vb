Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check int")>
Public Class CheckInt
    Inherits SFActivity

    <Category("Input")>
    <DisplayName("Minimum value")>
    Public Property MinValue As InArgument(Of Integer?)

    <Category("Input")>
    <DisplayName("Maximum value")>
    Public Property MaxValue As InArgument(Of Integer?)


    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckInt(Config.Get(context), Name.Get(context), IsRequired, MinValue.Get(Context), MaxValue.Get(context))
    End Sub

End Class