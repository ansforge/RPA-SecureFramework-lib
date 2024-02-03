Imports System.Activities.Presentation.Metadata
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Controls

Public Class ActivityDecoratorControl
    Inherits ContentControl

    Public Shared Sub ActivityDecoratorControl()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ActivityDecoratorControl), New FrameworkPropertyMetadata(GetType(ActivityDecoratorControl)))
    End Sub

End Class

Public Class DesignerMetadata
    Implements IRegisterMetadata

    Dim Catégorie = "ANS.Secure_Framework"
    Public Sub Register() Implements IRegisterMetadata.Register

        Dim builder = New AttributeTableBuilder()

        builder.AddCustomAttributes(GetType(CheckBoolean), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckBoolean), New DesignerAttribute(GetType(CheckBooleanDesigner)))

        builder.AddCustomAttributes(GetType(CheckCredentials), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckCredentials), New DesignerAttribute(GetType(CheckCredentialsDesigner)))

        builder.AddCustomAttributes(GetType(CheckFile), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckFile), New DesignerAttribute(GetType(CheckFileDesigner)))

        builder.AddCustomAttributes(GetType(CheckFolder), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckFolder), New DesignerAttribute(GetType(CheckFolderDesigner)))

        builder.AddCustomAttributes(GetType(CheckInt), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckInt), New DesignerAttribute(GetType(CheckIntDesigner)))

        builder.AddCustomAttributes(GetType(CheckUrl), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckUrl), New DesignerAttribute(GetType(CheckUrlDesigner)))

        builder.AddCustomAttributes(GetType(CheckMailAddress), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckMailAddress), New DesignerAttribute(GetType(CheckMailAddressDesigner)))

        builder.AddCustomAttributes(GetType(CheckMailAddressCollection), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckMailAddressCollection), New DesignerAttribute(GetType(CheckMailAddressCollectionDesigner)))

        builder.AddCustomAttributes(GetType(CheckStorage), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckStorage), New DesignerAttribute(GetType(CheckStorageDesigner)))

        builder.AddCustomAttributes(GetType(CheckString), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(CheckString), New DesignerAttribute(GetType(CheckStringDesigner)))

        MetadataStore.AddAttributeTable(builder.CreateTable())

    End Sub
End Class
