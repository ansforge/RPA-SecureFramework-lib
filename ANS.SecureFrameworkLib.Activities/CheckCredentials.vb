Imports System.Activities
Imports System.ComponentModel

<DisplayName("Check credentials")>
Public Class CheckCredentials
    Inherits CodeActivity

    <Category("Input/Output")>
    <DisplayName("Credentials dictionary")>
    <RequiredArgument()>
    Public Property Credentials As InOutArgument(Of Dictionary(Of String, Tuple(Of String, Security.SecureString)))
    
    <Category("Input")>
    <DisplayName("Name")>
    <RequiredArgument()>
    Public Property Name As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Is required")>
    Public Property IsRequired As Boolean = True
    
    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.CheckCredentials(Credentials.Get(context), Name.Get(context), IsRequired)
    End Sub

End Class