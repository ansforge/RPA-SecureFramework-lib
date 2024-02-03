Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check storage")>
Public Class CheckStorage
    Inherits SFActivity

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckStorage(Config.Get(context), Name.Get(context), IsRequired)
    End Sub

End Class