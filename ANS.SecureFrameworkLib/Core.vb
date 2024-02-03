Imports System.IO
Imports System.Net.Mail
Imports System.Globalization

Public Module Core

    ' Formats and returns a log message in the language of the .NET interpreter at runtime.
    Private Function _localization(Name As String, ParamArray Args As String()) As String
        Return String.Format(My.Resources.Core.ResourceManager.GetString(Name, CultureInfo.CurrentUICulture), Args)
    End Function
    
    ' Since Config is parsed from a spreadsheet, key-value pairs cannot be assumed to be correctly constructed.
    ' Before it can be used in the rest of the REFramework, the Config dictionary must be sanitized.
    '
    ' The following subs perform roughly the same checks, with additional adaptations to different variable types.
    ' They are all used to ensure that:
    ' 1) a Config dictionary exists
    ' 2) a correct key Name is given
    ' 3) the key Name exists in Config's keys and Config(Name) is of the appropriate type
    ' In some cases, a DefaultValue can alternatively be assigned to Config(Name) if Name is not found in Config's keys.
    '
    ' In any case, if a parameter is not marked as required and that parameter was not defined (neither in the spreadsheet nor by a default value), then the value Nothing is assigned to Config(Name).
    ' This way, in the rest of the REFramework, you can always use "Config(Name) is Nothing" to check whether a parameter is defined, and no error will be thrown since Name always exists in Config's keys.
    '
    ' Once every entry in the Config dictionary is verified using the relevant CheckXxx sub, you can use Config and Config(Name) safely across all the REFramework.
    

    ' Checks that a correct string parameter is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    '  - DefaultValue: A key-value pair <Name, DefaultValue> will be added to Config if Name is not yet defined in Config's keys
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True AND no DefaultValue was given
    Sub CheckString(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, DefaultValue As String)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrWhiteSpace(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse String.IsNullOrWhiteSpace(Config(Name)) Then
            If IsRequired And DefaultValue Is Nothing Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = DefaultValue
            End If
        End If
    End Sub
    
    ' Checks that a correct boolean parameter is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_BOOL if Config(Name) cannot be cast as a boolean
    Sub CheckBoolean(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrWhiteSpace(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else
            Dim value As Boolean
            If Not Boolean.TryParse(Config(Name), value) Then
                Throw New Exception(_localization("INVALID_BOOL", Name, Config(Name)))
            End If
        End If
    End Sub

    ' Checks that a <usr, pwd> pair is present in the credentials dictionary.
    ' Since they are retrieved from the Orchestrator through the appropriate activity, they are already under the form Tuple(Of String, SecureString).
    ' Inputs:
    '  - Credentials: the credentials dictionary
    '  - Name: the name of the <usr, pwd> pair in the Credentials dictionary
    '  - IsRequired: indicates that the credentials must exist in the dictionary
    ' Throws:
    '  - CONFIG_NOTHING if Credentials is not initialized
    '  - INVALID_ID_NAME if Name is null or white spaces
    '  - MISSING_ID if Name is not found in Credentials' keys AND IsRequired is True
    Sub CheckCredentials(Credentials As Dictionary(Of String, Tuple(Of String, Security.SecureString)), Name As String, IsRequired As Boolean)
        If Credentials Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_ID_NAME"))
        End If

        If Not Credentials.ContainsKey(Name) OrElse Credentials(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_ID", Name))
            Else
                Credentials(Name) = Nothing
            End If
        End If
    End Sub
    
    ' Checks that a correct file name parameter is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    '  - MustExist: indicates that the parameter must correspond to an existing file
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_FILE_NAME if Config(Name) is not a valid file name
    '  - MISSING_FILE if the file located by Config(Name) is not found AND MustExist is True
    Sub CheckFile(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, MustExist As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else

            Config(Name) = Environment.ExpandEnvironmentVariables(Config(Name))

            Dim info As FileInfo
            Try
                info = New FileInfo(Config(Name))
            Catch e As ArgumentException
                Throw New Exception(_localization("INVALID_FILE_NAME", Name, Config(Name)))
            End Try

            If MustExist And Not info.Exists Then
                Throw New Exception(_localization("MISSING_FILE", Name, Config(Name)))
            End If

        End If
    End Sub

    ' Checks that a correct folder name parameter is present in the config dictionary.
    ' Environment variables are expanded for free as a courtesy of the author.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    '  - CreateIfNotExists: indicates that the folder given by the parameter must be created if it is not found
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_FOLDER if Config(Name) is not a valid folder name
    '  - ERROR_FOLDER_CREATE if the folder located by Config(Name) is not found AND IsRequired is True AND CreateIfNotExists is True AND the folder creation did not succeed
    '  - MISSING_FOLDER if the folder located by Config(Name) is not found AND IsRequired is True AND CreateIfNotExists is False
    '  - ERROR_FOLDER_READONLY if the folder cannot be written AND IsRequired is True
    Sub CheckFolder(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, CreateIfNotExists As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else

            Config(Name) = Environment.ExpandEnvironmentVariables(Config(Name))

            Dim info As DirectoryInfo
            Try
                info = New DirectoryInfo(Config(Name))
            Catch e As ArgumentException
                Throw New Exception(_localization("INVALID_FOLDER", Name, Config(Name)))
            End Try

            If Not info.Exists Then
                If CreateIfNotExists Then
                    Try
                        Directory.CreateDirectory(Config(Name))
                    Catch e As Exception
                        Throw New Exception(_localization("ERROR_FOLDER_CREATE", Name, Config(Name)))
                    End Try
                Else
                    Throw New Exception(_localization("MISSING_FOLDER", Name, Config(Name)))
                End If
            End If

            Try
                Using fs As FileStream = File.Create(Path.Combine(Config(Name), Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)
                    ' Will throw a System.UnauthorizedAccessException if folder cannot be written.
                End Using
            Catch e As System.UnauthorizedAccessException
                Throw New Exception(_localization("ERROR_FOLDER_READONLY", Name, Config(Name)))
            End Try
        End If
    End Sub
    
    ' Checks that a correct integer parameter is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    '  - MinValue: optional minimum value for Config(Name)
    '  - MaxValue: optional maximum value for Config(Name)
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - ERROR_INT_RANGE if MinValue is defined AND MaxValue is defined AND MinValue is not strictly lesser than MaxValue
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_NUMBER if Config(Name) cannot be cast as a number
    '  - INVALID_INT if Config(Name) cannot be cast as an integer
    '  - ERROR_INT_MIN_VALUE if MinValue is defined AND Config(Name) is lesser than MinValue
    '  - ERROR_INT_MAX_VALUE if MaxValue is defined AND Config(Name) is bigger than MaxValue
    Sub CheckInt(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, MinValue As Integer?, MaxValue As Integer?)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If Not IsNothing(MinValue) AndAlso Not IsNothing(MaxValue) AndAlso Not (MinValue < MaxValue) Then
            Throw New Exception(_localization("ERROR_INT_RANGE", MinValue, MaxValue))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else

            If Not IsNumeric(Config(Name)) Then
                Throw New Exception(_localization("INVALID_NUMBER", Name, Config(Name)))
            End If

            Dim value As Integer
            If Not Integer.TryParse(Config(Name), value) Then
                Throw New Exception(_localization("INVALID_INT", Name, Config(Name)))
            End If

            If Not IsNothing(MinValue) AndAlso value < MinValue Then
                Throw New Exception(_localization("ERROR_INT_MIN_VALUE.", Name, Config(Name), MinValue))
            End If

            If Not IsNothing(MaxValue) AndAlso value > MaxValue Then
                Throw New Exception(_localization("ERROR_INT_MAX_VALUE", Name, Config(Name), MaxValue))
            End If
        End If
    End Sub
    
    ' Checks that a correct email address parameter is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_EMAIL if Config(Name) cannot be cast as a MailAddress
    Sub CheckMailAddress(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else

            Dim value As MailAddress
            Try
                value = New MailAddress(Config(Name))
            Catch
                Throw New Exception(_localization("INVALID_EMAIL", Name, Config(Name)))
            End Try
        End If
    End Sub
    
    ' Checks that a correct list of email addresses separated by ";" is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_EMAIL if one of the elements in Config(Name) cannot be cast as a MailAddress
    Sub CheckMailAddressCollection(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else
            Dim value As MailAddress
            For Each str As String In Config(Name).Split(";"c)
                Try
                    value = New MailAddress(str)
                Catch
                    Throw New Exception(_localization("INVALID_EMAIL", Name, str))
                End Try
            Next
        End If
    End Sub
    
    ' Checks that a correct url parameter is present in the config dictionary.
    ' Inputs:
    '  - Config: the configuration dictionary
    '  - Name: the name of the parameter in the Config dictionary
    '  - IsRequired: indicates that the parameter must exist in Config
    '  - TrailingSlash: will remove all the trailing slashes if False, or else will add one trailing slash if there isn't one already
    ' Throws:
    '  - CONFIG_NOTHING if Config is not initialized
    '  - INVALID_PARAM_NAME if Name is null or white spaces
    '  - MISSING_PARAM if Name is not found in Config's keys AND IsRequired is True
    '  - INVALID_URL if Config(Name) cannot be cast as an Uri
    Sub CheckUrl(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean, TrailingSlash As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            Else
                Config(Name) = Nothing
            End If
        Else

            Dim value As Uri
            Try
                value = New Uri(Config(Name))
            Catch
                Throw New Exception(_localization("INVALID_URL", Name, Config(Name)))
            End Try

            If TrailingSlash Then
                If Not Config(Name).EndsWith("/") Then
                    Config(Name) += "/"
                End If
            Else
                While Config(Name).EndsWith("/")
                    Config(Name) = String.Join("", Config(Name).SkipLast(1))
                End While
            End If
        End If
    End Sub
    
    Sub CheckStorage(Config As Dictionary(Of String, String), Name As String, IsRequired As Boolean)
        If Config Is Nothing Then
            Throw New Exception(_localization("CONFIG_NOTHING"))
        End If

        If String.IsNullOrEmpty(Name) Then
            Throw New Exception(_localization("INVALID_PARAM_NAME"))
        End If

        If Not Config.ContainsKey(Name) OrElse Config(Name) Is Nothing Then
            Config(Name) = Nothing
            If IsRequired Then
                Throw New Exception(_localization("MISSING_PARAM", Name))
            End If
            Return
        End If

        ' La vérification de l'existence du fichier dans le compartiment de stockage distant n'est pas encore opérationnelle.

        'Try
        '    Dim listStorageFilesActivity As ListStorageFiles = New ListStorageFiles With {
        '        .Directory = "/",
        '        .Recursive = True,
        '        .StorageBucketName = Config(Name)
        '    }
        '    Dim invoker As System.Activities.WorkflowInvoker = New WorkflowInvoker(listStorageFilesActivity)
        '    invoker.Invoke()
        'Catch e As Exception
        '    'Throw New Exception(_localization("ERROR_READONLY_BUCKET", Name, Config(Name), e.Message))
        'End Try
    End Sub

End Module
