Imports System.Activities
Imports System.ComponentModel

Public MustInherit Class SFActivity
    Inherits CodeActivity
    
    <Category("Input/Output")>
    <DisplayName("Config dictionary")>
    <RequiredArgument()>
    Public Property Config As InOutArgument(Of Dictionary(Of String, String))

    <Category("Input")>
    <DisplayName("Name")>
    <RequiredArgument()>
    Public Property Name As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Is required")>
    Public Property IsRequired As Boolean = True

End Class
