'Editted on April/2019, Aug 2021
Imports System.Data.OleDb
Imports System.Linq

Public Class Vndrs
    Private Ops As New DataOperations
    Private ConnectionString = Ops.GetEncryConStr
    Function GetData(ByVal SqlStr As String)
        Dim NewDtbl As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
        Try
            Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                MyCmdStr As New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
                CN.Open()
                Using reader As OleDbDataReader = MyCmdStr.ExecuteReader
                    NewDtbl.Load(reader)
                End Using
            End Using
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return NewDtbl
    End Function
    Sub Clearall()
        Try
            For Each Ctrl1 As Control In Me.Controls
                If TypeOf Ctrl1 Is TextBox Then
                    Ctrl1.Text = String.Empty
                End If
            Next
        Catch ex As Exception
            lblSt.Text = ("Clear All : ") & ex.Message
        End Try
    End Sub
    Sub DispVen()
        With VendG
            .DataSource = GetData("SELECT * FROM Vendors;")
            .Columns(0).HeaderText = "كود المورد"
            .Columns(1).HeaderText = "الاسم"
            .Columns(2).HeaderText = "تليفون"
            .Columns(3).HeaderText = "موبيل"
            .Columns(4).HeaderText = "عنوان"
            .Columns(5).HeaderText = "ملاحظات"
        End With
        lblSt.Text = ("لديك عدد " & VendG.Rows.Count & " مورد")
    End Sub
    Private Sub _Vadd_Click(sender As System.Object, e As System.EventArgs) Handles _Vadd.Click
        Try
            Clearall()
            _Vadd.Enabled = False
            _Vdel.Enabled = False
            _Vedit.Enabled = False
            _Vsave.Enabled = True
            Tname.Select()
        Catch ex As Exception
            lblSt.Text = ("مورد جديد : ") & ex.Message
        End Try
    End Sub
    Private Sub _Vsave_Click(sender As System.Object, e As System.EventArgs) Handles _Vsave.Click
        Dim Onhe As Integer = Nothing
        If String.IsNullOrEmpty(Tname.Text) Or
            String.IsNullOrWhiteSpace(Tname.Text) Then
            MsgBox("أدخل الاسم أولا", MsgBoxStyle.Critical, "حفظ مورد")
            Exit Sub
        End If
        Dim NewSqlStr As String = "INSERT INTO Vendors (VendNm,Tel,Mob,Address,Notes) VALUES (?,?,?,?,?);"
        Using newCn As OleDbConnection = New OleDbConnection(connectionstring),
                    NewCmd As OleDbCommand = New OleDbCommand(NewSqlStr, newCn) With {.CommandType = CommandType.Text}
            With NewCmd.Parameters
                .AddWithValue("?", Tname.Text)
                .AddWithValue("?", Ttel.Text)
                .AddWithValue("?", Tmob.Text)
                .AddWithValue("?", Tadd.Text)
                .AddWithValue("?", Tnotes.Text)
            End With
            Try
                newCn.Open()
                Onhe = NewCmd.ExecuteNonQuery
                If IsNothing(Onhe) Or Onhe <= 0 Then
                    lblSt.Text = "مشكلة فى حفظ مورد جديد"
                    Exit Sub
                End If
            Catch ex As OleDbException
                lblSt.Text = ex.Message
            End Try
        End Using
        DispVen()
        lblSt.Text = ("تم حفظ المورد و لديك عدد " & VendG.Rows.Count & " مورد")
        _Vadd.Enabled = True
        _Vsave.Enabled = False
        _Vdel.Enabled = True
        _Vedit.Enabled = True
    End Sub
    Private Sub _nmExit_Click(sender As System.Object, e As System.EventArgs) Handles _nmExit.Click
        Try
            Close()
        Catch ex As Exception
            lblSt.Text = ("Exit : ") & ex.Message
        End Try
    End Sub
    Private Sub VendG_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles VendG.CellContentClick
        Try

        Catch ex As Exception

        End Try
    End Sub
    Private Sub _Vedit_Click(sender As System.Object, e As System.EventArgs) Handles _Vedit.Click
        If _Vedit.Enabled = True Then
            Dim Onh3 As Object = Nothing
            Dim Esql As String = "UPDATE Vendors SET VendNm=?, Mob=?, Tel=?, Address=?, Notes=? WHERE VenID=?;"
            Using newCn As OleDbConnection = New OleDbConnection(connectionstring),
                        NewCmd As OleDbCommand = New OleDbCommand(Esql, newCn) With {.CommandType = CommandType.Text}
                With NewCmd.Parameters
                    .AddWithValue("?", Tname.Text)
                    .AddWithValue("?", Tmob.Text)
                    .AddWithValue("?", Ttel.Text)
                    .AddWithValue("?", Tadd.Text)
                    .AddWithValue("?", Tnotes.Text)
                    .AddWithValue("?", CInt(VendG(0, VendG.CurrentCell.RowIndex).Value))
                End With
                Try
                    newCn.Open()
                    Onh3 = NewCmd.ExecuteNonQuery
                    If Onh3 Is DBNull.Value And IsNothing(Onh3) Then
                        MsgBox("خطأ فالتعديل")
                        newCn.Close()
                        Exit Sub
                    End If
                Catch ex As OleDbException
                    MsgBox(ex.Message)
                    Exit Sub
                End Try
            End Using
            DispVen()
            _Vadd.Enabled = True
            _Vsave.Enabled = False
            _Vedit.Enabled = False 'Edit
            _Vdel.Enabled = True 'Del
            lblSt.Text = ("تم تعديل عدد " & Onh3 & " مورد")
        End If
    End Sub
    Private Sub _Vdel_Click(sender As System.Object, e As System.EventArgs) Handles _Vdel.Click
        Try
            If _Vdel.Enabled = True Then
                If Val(TextBox1.Text) > 0 Then
                    MsgBox("لا يمكن حذف مورد مستحق له مبالغ")
                    lblSt.Text = String.Empty
                    Exit Sub
                End If
                Dim AreYouSure As String =
                    MsgBox("تأكيد حذف مورد ؟",
                           MsgBoxStyle.YesNoCancel + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.Critical, "حذف")
                If AreYouSure = vbYes Then
                    Dim Onhy As Object = Nothing
                    Dim Dsql As String = "DELETE * FROM Vendors WHERE VenID=?;"
                    Using newCn As OleDbConnection = New OleDbConnection(connectionstring),
                NewCmd As OleDbCommand = New OleDbCommand(Dsql, newCn) With {.CommandType = CommandType.Text}
                        With NewCmd.Parameters
                            .AddWithValue("?", CInt(VendG(0, VendG.CurrentCell.RowIndex).Value))
                        End With
                        Try
                            newCn.Open()
                            Onhy = NewCmd.ExecuteNonQuery
                            If Onhy Is DBNull.Value And IsNothing(Onhy) Then
                                MsgBox("خطأ فالحذف")
                                newCn.Close()
                                Exit Sub
                            End If
                        Catch ex As OleDbException
                            MsgBox(ex.Message)
                        End Try
                    End Using
                    MsgBox("تم حذف عدد " & CInt(Onhy) & " مورد")
                    DispVen()
                    _Vadd.Enabled = True
                    _Vsave.Enabled = False
                    _Vdel.Enabled = False  'Del
                    _Vedit.Enabled = False
                Else
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            lblSt.Text = ("Del Error : ") & ex.Message
        End Try
    End Sub
    Private Sub Vndrs_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        Try
            If e.KeyChar = ChrW(Keys.Escape) Then
                _nmExit_Click(sender, e)
            End If
        Catch ex As Exception
            lblSt.Text = ("Exit : ") & ex.Message
        End Try
    End Sub
    Private Sub Vndrs_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            Clearall()
            With VendG
                .EnableHeadersVisualStyles = False
                .AutoGenerateColumns = True
                .AllowUserToAddRows = False
                .ReadOnly = True
                .RightToLeft = RightToLeft.Inherit
                .RowsDefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .RowsDefaultCellStyle.Font = New Font("Arial Black", 8.25, FontStyle.Regular)
                .RowHeadersWidth = 30
                .RowHeadersDefaultCellStyle.BackColor = Color.DarkSlateGray
                .AlternatingRowsDefaultCellStyle.BackColor = Color.Silver
                .ColumnHeadersDefaultCellStyle.BackColor = Color.DimGray
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
                .ColumnHeadersDefaultCellStyle.Font = New Font("Times New Roman", 13.25, FontStyle.Bold)
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            End With
        Catch ex As Exception
            lblSt.Text = ("LOAD : ") & ex.Message
        End Try
    End Sub
    Private Function AllVendsDebts() As Decimal
        If VendG.Rows.Count <= 0 Then
            Return 0
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT SUM(PORest) FROM PurOrders WHERE TranID=1;"
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            Try
                CN.Open()
                Dim C1 As Object = CMD.ExecuteScalar()
                If Not IsDBNull(C1) Then
                    Button5.Enabled = True
                    Return Decimal.Parse(C1.ToString)
                Else
                    Button5.Enabled = False
                    Return 0
                    Exit Function
                End If
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
    End Function
    Private Sub _Disp_Click(sender As System.Object, e As System.EventArgs) Handles _Disp.Click
        If VendG.Rows.Count <= 0 Then Exit Sub
        VendPays.Show(Me)
    End Sub
    Private Function AllVendsPays() As Decimal
        If VendG.Rows.Count <= 0 Then
            Return 0
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT Sum(CustPaid.Payamnt) AS SumOfPayamnt FROM CustPaid GROUP BY CustPaid.TranID HAVING (((CustPaid.TranID)=1));"
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            Try
                CN.Open()
                Return CInt(CMD.ExecuteScalar())
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
    End Function
    Private Sub _DispUp_Click(sender As System.Object, e As System.EventArgs) Handles _DispUp.Click
        TextBox5.Text = FormatCurrency(AllVendsDebts() - AllVendsPays(), 2)
    End Sub
    Private Sub Tname_Enter(sender As Object, e As System.EventArgs) Handles Tname.Enter
        Tname.SelectAll()
    End Sub
    Private Sub Tmob_Enter(sender As Object, e As System.EventArgs) Handles Tmob.Enter
        Tmob.SelectAll()
    End Sub
    Private Sub Ttel_Enter(sender As Object, e As System.EventArgs) Handles Ttel.Enter
        Ttel.SelectAll()
    End Sub
    Private Sub Tsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles Tsearch.KeyDown

    End Sub
    Private Sub VendG_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles VendG.CellClick
        If e.ColumnIndex = -1 Or e.RowIndex = -1 Then Exit Sub
        If VendG.Rows.Count < 0 Then Exit Sub
        If IsNothing(VendG.CurrentRow) Then Exit Sub
        Dim ThisID As Integer = VendG.Rows(e.RowIndex).Cells(0).Value
        Tcode.Text = ThisID.ToString    'Current ID
        Tname.Text = VendG(1, e.RowIndex).Value.ToString
        Ttel.Text = VendG(2, e.RowIndex).Value.ToString
        Tmob.Text = VendG(3, e.RowIndex).Value.ToString
        Tadd.Text = VendG(4, e.RowIndex).Value.ToString
        Tnotes.Text = VendG(5, e.RowIndex).Value.ToString

        'Sell Invoices Count
        TextBox3.Text = AllPOInv(VendG("VenID", e.RowIndex).Value).ToString
        Dim numbs As Decimal() = VenDebts(VendG("VenID", e.RowIndex).Value)
        TextBox1.Text = FormatCurrency(numbs.First().ToString, 2)
        TextBox4.Text = FormatCurrency(numbs.Last().ToString, 2)
        VendPays.ThisCust = CInt(VendG("VenID", VendG.CurrentCell.RowIndex).Value)
        VendPays.TxtCustNm.Text = VendG("VendNm", VendG.CurrentCell.RowIndex).Value.ToString
    End Sub
    Private Sub _MV_MouseDown(sender As Object, e As MouseEventArgs) Handles _MV.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub
    Private Sub MnuShow_Click(sender As Object, e As EventArgs) Handles MnuShow.Click
        DispVen()  'Display All Vendors
        VendG.ClearSelection()
    End Sub
    Private Sub Tsearch_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Tsearch.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) Then
            Try
                'Search By name using 1 letter
                Dim OsqlStr As String =
                    "SELECT * FROM Vendors WHERE VendNm LIKE '%" & Tsearch.Text & "%';"
                VendG.DataSource = GetData(OsqlStr)
                VendG.Columns(0).HeaderText = "كود المورد"
                VendG.Columns(0).ReadOnly = True
                VendG.Columns(1).HeaderText = "الاسم"
                VendG.Columns(2).HeaderText = "تليفون"
                VendG.Columns(3).HeaderText = "موبيل"
                VendG.Columns(4).HeaderText = "عنوان"
                VendG.Columns(5).HeaderText = "ملاحظات"
                _Vadd.Enabled = True
                _Vsave.Enabled = False
                _Vdel.Enabled = True
                _Vedit.Enabled = True
            Catch ex As Exception
                lblSt.Text = ("Search Name error : ") & ex.Message
            End Try
        End If
    End Sub
    Private Sub Label17_Click(sender As Object, e As EventArgs) Handles Label17.Click
        Location = New Point(0, 0)
        If WindowState = FormWindowState.Maximized Then
            WindowState = FormWindowState.Normal
            Exit Sub
        End If
        WindowState = FormWindowState.Maximized
    End Sub
    Private Sub MnuPrint_Click(sender As Object, e As EventArgs) Handles MnuPrint.Click
        CustsVends.SrcFrm = "Vndrs"
        CustsVends.ShowDialog()
    End Sub
    Private Function AllPOInv(VndrID As Integer) As Integer
        If VendG.Rows.Count <= 0 Then
            Return 0
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT COUNT(POID) FROM PurOrders WHERE TranID=1 AND VenID=?;"
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            CMD.Parameters.AddWithValue("?", VndrID)
            Try
                CN.Open()
                Dim C1 As Object = CInt(CMD.ExecuteScalar)
                If Not C1 Is DBNull.Value Or Not C1 Is Nothing Then
                    If C1 > 0 Then
                        Button3.Enabled = True
                    Else
                        Button3.Enabled = False
                    End If
                    Return C1
                Else
                    Return 0
                    Exit Function
                End If
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'بيان أوامر الشراء
        If String.IsNullOrEmpty(TextBox3.Text) Then Exit Sub
        CustsVends.SrcFrm = "VendInv"
        Dim Onh1 As Object = Nothing, Onh2 As Object = Nothing
        Dim SqlDel As String = "DROP VIEW VendInvs;"
        Dim SqlStrCreate As String =
            "CREATE VIEW VendInvs AS SELECT PurOrders.POID, PurOrders.PODt, PayTypes.PTNm, Vendors.VendNm, Vendors.Tel, Vendors.Mob, " &
            "Vendors.Address, Vendors.Notes, Sum(PurOrders.POTotal) AS SumOfPOTotal, Sum(PurOrders.PODisc) AS SumOfPODisc, " &
            "Sum(PurOrders.PONet) AS SumOfPONet, Sum(PurOrders.POPaid) AS SumOfPOPaid, Sum(PurOrders.PORest) AS SumOfPORest, " &
            "PurOrders.PONots FROM Vendors INNER JOIN (PayTypes INNER JOIN (TranTypes INNER JOIN PurOrders ON TranTypes.TranID = " &
            "PurOrders.TranID) ON PayTypes.PTID = PurOrders.PTId) ON Vendors.VenID=PurOrders.[VenID] GROUP BY PurOrders.POID, " &
            "PurOrders.PODt, PayTypes.PTNm, Vendors.VendNm, Vendors.Tel, Vendors.Mob, Vendors.Address, Vendors.Notes, PurOrders.PONots, " &
            "PurOrders.[VenID], TranTypes.TranID " &
            "HAVING (((PurOrders.[VenID])=" & CInt(VendG("VenID", VendG.CurrentCell.RowIndex).Value.ToString) & ") " &
            "And ((TranTypes.TranID)=1));"
        Using CN12 As OleDbConnection = New OleDbConnection(connectionstring),
                CmdVcash As OleDbCommand = New OleDbCommand(SqlDel, CN12) With {.CommandType = CommandType.Text},
                CmdVcash1 As OleDbCommand = New OleDbCommand(SqlStrCreate, CN12) With {.CommandType = CommandType.Text}
            CN12.Open()
            Try
                Onh1 = CmdVcash.ExecuteNonQuery
                Onh2 = CmdVcash1.ExecuteNonQuery
            Catch ex As OleDbException
                Onh2 = CmdVcash1.ExecuteNonQuery
            Finally
                CustsVends.ShowDialog()
            End Try
        End Using
    End Sub
    Private Function VenDebts(VendID As Integer) As Decimal()
        Dim MyList As Decimal() = {0}
        If VendG.Rows.Count <= 0 Then
            Return MyList
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT Sum(PurOrders.PORest) AS SumOfPORest, Sum(PurOrders.POTotal) AS SumOfPOTotal FROM Vendors INNER JOIN PurOrders ON " &
            "Vendors.VenID = PurOrders.VenID GROUP BY PurOrders.TranID, Vendors.VenID HAVING (((PurOrders.TranID)=1) AND " &
            "((Vendors.VenID)=?));"
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            CMD.Parameters.AddWithValue("?", VendID)
            Try
                CN.Open()
                Using Readr As OleDbDataReader = CMD.ExecuteReader
                    If Readr.HasRows Then
                        Button1.Enabled = True
                        Button2.Enabled = True
                        While Readr.Read
                            MyList = {Readr.GetDecimal(0), Readr.GetDecimal(1)}
                        End While
                        If MyList.Sum = 0 Then
                            Button1.Enabled = True
                            Button2.Enabled = True
                        End If
                    End If
                End Using
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
        Return MyList
    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Button3_Click(sender, e)
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button3_Click(sender, e)
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        'بيان أوامر الشراء
        If String.IsNullOrEmpty(TextBox5.Text) Then Exit Sub
        CashDetails.TargetForm1 = "AllVendsDebts"
        Dim Onh1 As Object = Nothing, Onh2 As Object = Nothing
        Dim SqlDel As String = "DROP VIEW AllVendsDebtsVals;"
        Dim SqlStrCreate As String =
            "CREATE VIEW AllVendsDebtsVals AS SELECT PurOrders.POID, PurOrders.PODt, PurOrders.PORest, PayTypes.PTNm, Vendors.VendNm " &
            "FROM (TranTypes INNER JOIN (PayTypes INNER JOIN PurOrders ON PayTypes.PTID = PurOrders.PTId) ON TranTypes.TranID = " &
            "PurOrders.TranID) INNER JOIN Vendors ON PurOrders.VenID = Vendors.VenID GROUP BY PurOrders.POID, PurOrders.PODt, " &
            "PurOrders.PORest, PayTypes.PTNm, PurOrders.TranID, Vendors.VendNm HAVING (((PurOrders.TranID)=1));"
        Using CN12 As OleDbConnection = New OleDbConnection(connectionstring),
                CmdVcash As OleDbCommand = New OleDbCommand(SqlDel, CN12) With {.CommandType = CommandType.Text},
                CmdVcash1 As OleDbCommand = New OleDbCommand(SqlStrCreate, CN12) With {.CommandType = CommandType.Text}
            CN12.Open()
            Try
                Onh1 = CmdVcash.ExecuteNonQuery
                Onh2 = CmdVcash1.ExecuteNonQuery
            Catch ex As OleDbException
                Onh2 = CmdVcash1.ExecuteNonQuery
            Finally
                CashDetails.ShowDialog()
            End Try
        End Using
    End Sub
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If Val(TextBox1.Text) > 0 Then
            _Disp.Enabled = True
        Else
            _Disp.Enabled = False
        End If
    End Sub
End Class