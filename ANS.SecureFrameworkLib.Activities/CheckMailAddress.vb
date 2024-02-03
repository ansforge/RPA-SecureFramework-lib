Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check mail address")>
Public Class CheckMailAddress
    Inherits SFActivity

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckMailAddress(Config.Get(context), Name.Get(context), IsRequired)
    End Sub

End Class