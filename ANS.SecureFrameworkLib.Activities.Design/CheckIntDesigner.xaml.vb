Imports System.Activities.Presentation
Imports System.Activities.Presentation.View
Imports System.Windows

Public Class CheckIntDesigner

    Public Sub SetExpressionType_NullableInt(sender As ExpressionTextBox, e As RoutedEventArgs)
        sender.ExpressionType = GetType(Integer?)
    End Sub

End Class

