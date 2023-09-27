Imports System.Data.OleDb
Imports System.IO

Module Module1
    Public PreForm As Form
    Public ThisBackPath As String = Nothing
    Public MyPass As String = My.Settings.DatabasePass
    Public connectionstring As String = "Provider=Microsoft.ACE.OLEDB.16.0;" &
                                        "Data Source=" & Path.Combine(Application.StartupPath, My.Settings.DBName) & ";" &
                                        "Persist Security Info = false;" &
                                        "Jet OLEDB:Database Password='" & MyPass & "';"
    Public FromFrm As String = String.Empty
    '====================================
    Public Cname_ As String = String.Empty
    Public ItmBar As String = String.Empty
    Public AdminUname As String = String.Empty
#Region "Form Move"
    Public Const WM_NCLBUTTONDOWN As Integer = &HA1
    Public Const HT_CAPTION As Integer = &H2
    <Runtime.InteropServices.DllImportAttribute("user32.dll")>
    Public Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function
    <Runtime.InteropServices.DllImportAttribute("user32.dll")>
    Public Function ReleaseCapture() As Boolean
    End Function
#End Region
    Public Function CompRepairJET4() As Boolean
        'This will Compact & Repair MSAccess2007 Database to the same location with the same name.
        Dim Result As Boolean = False
        Dim Compactedfil As String = Path.Combine(Application.StartupPath, "CompactedFile.accdb")
        Dim OriginalDB As String = Path.Combine(Application.StartupPath, My.Settings.DBName)
        Dim jro As JRO.JetEngine
        jro = New JRO.JetEngine()
        Dim a = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source=" & OriginalDB & "; Jet OLEDB:Database Password=01002851540;"
        Dim b = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source=" & Compactedfil &
            "; Jet OLEDB: Engine Type=5; Jet OLEDB:Database Password=01002851540;"
        Try
            jro.CompactDatabase(a, b)
            Result = True
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
            Return False
            Exit Function
        End Try
        DelRename(OriginalDB, Compactedfil)
        Return Result
    End Function
    Private Function DelRename(ByVal OriginalNM As String, ByVal DestNm As String) As Boolean
        Dim Fil As FileInfo = New FileInfo(OriginalNM)
        Try
            If Fil.Exists Then
                Fil.Delete()
                My.Computer.FileSystem.RenameFile(DestNm, My.Settings.DBName)
                Fil.Refresh()
                Return True
            End If
            While Fil.Exists
                Threading.Thread.Sleep(1000)
                Fil.Refresh()
            End While
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function
    Public Sub DisplayDGView(ByVal MySqlStr As String, DGI As DataGridView)
        Dim BindingSource1 As New BindingSource
        Try
            With DGI
                .DataSource = BindingSource1
                .Refresh()
            End With
            Using CNStr As OleDbConnection = New OleDbConnection(connectionstring),
                MyCmdStr As OleDbCommand = New OleDbCommand(MySqlStr, CNStr) With {.CommandType = CommandType.Text}
                Using Da As OleDbDataAdapter = New OleDbDataAdapter(MyCmdStr)
                    Using NewTable As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
                        CNStr.Open()
                        Da.Fill(NewTable)
                        BindingSource1.DataSource = NewTable
                    End Using
                End Using
            End Using
            DGI.DataSource = BindingSource1
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Function RandomString() As String
        Dim r As Random = New Random(36)
        Dim s As String = Guid.NewGuid.ToString
        Try
            Return s.Substring(r.Next(1, s.Length - 1), 8)
        Catch ex As Exception
            Return s.Substring(r.Next(2, s.Length - 2), 9)
        End Try
    End Function
    Public Function GetMSProvider() As List(Of String)
        Dim Providers = New List(Of String)
        Dim provider = String.Empty
        Dim reader = OleDbEnumerator.GetRootEnumerator()
        Dim list = New List(Of String)
        While reader.Read()
            For I As Integer = 0 To reader.FieldCount - 1
                'Debug.WriteLine(reader.GetName(i))
                If reader.GetName(I) = "SOURCES_NAME" Then
                    list.Add(reader.GetValue(I).ToString())
                    'Debug.WriteLine(reader.GetValue(i).ToString())
                End If
            Next
        End While
        reader.Close()
        For Each provider In list
            Debug.WriteLine(provider)
            If provider.StartsWith("Microsoft") Then
                Providers.Add(("Provider=") & provider & ";Data Source=")
            End If
        Next
        Return Providers
    End Function
    Public Function CompRepair(ByVal OriginalDB As String, ByVal BackUpFile As String) As Boolean
        'This will Compact & Repair MSAccess2007 Database to the same location with the same name.
        Dim Result As Boolean = False
        Cursor.Current = Cursors.WaitCursor
        'Compact & Repair needs the Database File (*.accdb) to be closed.
        Dim MyAccDB As New Microsoft.Office.Interop.Access.Dao.DBEngine
        Try
            MyAccDB.CompactDatabase(OriginalDB,
                                    BackUpFile, , ,
                                    ";pwd=" & My.Settings.DatabasePass)
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
            Return False
            Exit Function
        End Try
        Try
            My.Computer.FileSystem.DeleteFile(OriginalDB)
            Result = True
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
            Return Result
            Exit Function
        End Try
        Try
            Rename(BackUpFile, OriginalDB)
            Result = True
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
            Return Result
            Exit Function
        End Try
        Application.DoEvents()
        Return Result
        Cursor.Current = Cursors.Default
    End Function
End Module
