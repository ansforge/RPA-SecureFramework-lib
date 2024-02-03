Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check mail address collection")>
Public Class CheckMailAddressCollection
    Inherits SFActivity
    
    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckMailAddressCollection(Config.Get(context), Name.Get(context), IsRequired)
    End Sub

End Class