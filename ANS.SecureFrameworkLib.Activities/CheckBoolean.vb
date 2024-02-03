Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check boolean")>
Public Class CheckBoolean
    Inherits SFActivity

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckBoolean(Config.Get(context), Name.Get(context), IsRequired)
    End Sub

End Class