'Editted on April/2019, Aug 2021
Imports System.Data.OleDb
Imports System.Linq
Public Class Custms
    Sub DispCusts()
        GetData("SELECT * FROM Customers;", CustDG)
        With CustDG
            .Columns(0).HeaderText = "كود العميل"
            .Columns(1).HeaderText = "الاسم"
            .Columns(2).HeaderText = "تليفون"
            .Columns(3).HeaderText = "موبيل"
            .Columns(4).HeaderText = "عنوان"
            .Columns(5).HeaderText = "ملاحظات"
        End With
        LblSt.Text = ("لديك عدد " & CustDG.Rows.Count & " عميل")
    End Sub
    Sub GetData(ByVal SqlStr As String, ByVal DG As DataGridView)
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Try
            Dim NewDtbl As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
            Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                MyCmdStr As New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
                CN.Open()
                Using Da As OleDbDataAdapter = New OleDbDataAdapter(MyCmdStr)
                    Da.Fill(NewDtbl)
                End Using
            End Using
            With CustDG
                .DataSource = NewDtbl
            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Sub Clearall()
        Try
            For Each Ctrl1 As Control In Me.Controls
                If TypeOf Ctrl1 Is TextBox Then
                    Ctrl1.Text = String.Empty
                End If
            Next
        Catch ex As Exception
            LblSt.Text = ("Clear All : ") & ex.Message
        End Try
    End Sub
    Private Sub MenuStrip1_MouseDown(sender As Object, e As MouseEventArgs) Handles MenuStrip1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub
    Private Sub Custms_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Clearall()
            With CustDG
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
            LblSt.Text = ("LOAD : ") & ex.Message
        End Try
    End Sub
    Private Sub _Nback_Click(sender As Object, e As EventArgs) Handles _Nback.Click
        Close()
    End Sub
    Private Sub Custms_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Try
            If e.KeyChar = ChrW(Keys.Escape) Then
                _Nback_Click(sender, e)
            End If
        Catch ex As Exception
            LblSt.Text = ("Exit : ") & ex.Message
        End Try
    End Sub
    Private Sub _CMdel_Click(sender As Object, e As EventArgs) Handles _CMdel.Click
        Try
            If Val(Tcredits.Text) > 0 Then
                MsgBox("لا يمكن حذف عميل مستحق له مبالغ")
                LblSt.Text = String.Empty
                Exit Sub
            End If
            Dim AreYouSure As String =
                MsgBox("تأكيد حذف عميل ؟",
                       MsgBoxStyle.YesNoCancel + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.Critical, "حذف")
            If AreYouSure = vbYes Then
                Dim Ops As New DataOperations
                Dim ConnectionString = Ops.GetEncryConStr
                Dim Onhy As Object = Nothing
                Dim Dsql As String = "DELETE * FROM Customers WHERE CustID=?;"
                Using newCn As OleDbConnection = New OleDbConnection(connectionstring),
            NewCmd As OleDbCommand = New OleDbCommand(Dsql, newCn) With {.CommandType = CommandType.Text}
                    With NewCmd.Parameters
                        .AddWithValue("?", CInt(CustDG(0, CustDG.CurrentCell.RowIndex).Value))
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
                DispCusts()
                _CustN.Enabled = True
                _CustS.Enabled = False
                _CMdel.Enabled = False  'Del
                _CMedit.Enabled = False
            Else
                Exit Sub
            End If
        Catch ex As Exception
            LblSt.Text = ("عملية غير صحيحة - حذف : ") & ex.Message
        End Try
    End Sub
    Private Sub Tsearch_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Tsearch.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) Then
            Try
                'Search By name using 1 letter
                Dim OsqlStr As String =
                    "SELECT * FROM Customers WHERE CustNm LIKE '%" & Tsearch.Text & "%';"
                GetData(OsqlStr, CustDG)
                With CustDG
                    .Columns(0).HeaderText = "كود العميل"
                    .Columns(0).ReadOnly = True
                    .Columns(1).HeaderText = "الاسم"
                    .Columns(2).HeaderText = "موبيل"
                    .Columns(3).HeaderText = "تليفون"
                    .Columns(4).HeaderText = "عنوان"
                    .Columns(5).HeaderText = "ملاحظات"
                End With
                _CustN.Enabled = True
                _CustS.Enabled = False
                _CMdel.Enabled = True
                _CMedit.Enabled = True
            Catch ex As Exception
                LblSt.Text = ("عملية غير صحيحة : ") & ex.Message
            End Try
        End If
    End Sub
    Private Sub _CustS_Click(sender As Object, e As EventArgs) Handles _CustS.Click
        Dim Onhe As Integer = Nothing
        If String.IsNullOrEmpty(Tname.Text) Or
            String.IsNullOrWhiteSpace(Tname.Text) Then
            MsgBox("أدخل الاسم أولا", MsgBoxStyle.Critical, "حفظ عميل")
            Exit Sub
        End If
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Dim NewSqlStr As String = "INSERT INTO Customers (CustNm,Mob,Tel,Address,Notes) VALUES (?,?,?,?,?);"
        Using newCn As OleDbConnection = New OleDbConnection(connectionstring),
                    NewCmd As OleDbCommand = New OleDbCommand(NewSqlStr, newCn) With {.CommandType = CommandType.Text}
            With NewCmd.Parameters
                .AddWithValue("?", Tname.Text)
                .AddWithValue("?", Tmob.Text)
                .AddWithValue("?", Ttel.Text)
                .AddWithValue("?", Tadd.Text)
                .AddWithValue("?", Tnotes.Text)
            End With
            Try
                newCn.Open()
                Onhe = NewCmd.ExecuteNonQuery
                If IsNothing(Onhe) Or Onhe <= 0 Then
                    LblSt.Text = "مشكلة فى حفظ عميل جديد"
                    Exit Sub
                End If
            Catch ex As OleDbException
                LblSt.Text = ex.Message
            End Try
        End Using
        DispCusts()
        LblSt.Text = ("تم حفظ عميل جديد و لديك عدد " & CustDG.Rows.Count & " عميل")
        _CustN.Enabled = True
        _CustS.Enabled = False
        _CMdel.Enabled = True
        _CMedit.Enabled = True
    End Sub
    Private Sub _CustN_Click(sender As Object, e As EventArgs) Handles _CustN.Click
        Try
            Clearall()
            _CustN.Enabled = False
            _CMdel.Enabled = False
            _CMedit.Enabled = False
            _CustS.Enabled = True
            Tname.Select()
        Catch ex As Exception
            LblSt.Text = ("عميل جديد : ") & ex.Message
        End Try
    End Sub
    Private Sub _CMedit_Click(sender As Object, e As EventArgs) Handles _CMedit.Click
        Dim Onh3 As Object = Nothing
        Dim Esql As String = "UPDATE Customers SET CustNm=?, Mob=?, Tel=?, Address=?, Notes=? WHERE CustID=?;"
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Using newCn As OleDbConnection = New OleDbConnection(connectionstring),
                    NewCmd As OleDbCommand = New OleDbCommand(Esql, newCn) With {.CommandType = CommandType.Text}
            With NewCmd.Parameters
                .AddWithValue("?", Tname.Text)
                .AddWithValue("?", Tmob.Text)
                .AddWithValue("?", Ttel.Text)
                .AddWithValue("?", Tadd.Text)
                .AddWithValue("?", Tnotes.Text)
                .AddWithValue("?", CInt(CustDG(0, CustDG.CurrentCell.RowIndex).Value))
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
        DispCusts()
        _CustN.Enabled = True
        _CustS.Enabled = False
        _CMedit.Enabled = False 'Edit
        _CMdel.Enabled = True 'Del
        LblSt.Text = ("تم تعديل عدد " & Onh3 & " عميل")
    End Sub
    Private Sub MnuDisp_Click(sender As Object, e As EventArgs) Handles MnuDisp.Click
        DispCusts()  'Display All Vendors
        CustDG.ClearSelection()
        CustDG.CurrentCell = Nothing
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
    Private Sub CustDG_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustDG.CellClick
        If e.ColumnIndex = -1 Or e.RowIndex = -1 Then Exit Sub
        If CustDG.Rows.Count < 0 Then Exit Sub
        CustDG_RowEnter(sender, e)
    End Sub
    Private Function GetallPays(ByVal CustID As Integer) As Decimal
        Dim SqlStr As String =
           "SELECT Sum(CustPaid.Payamnt) AS SumOfPayamnt FROM CustPaid GROUP BY CustPaid.CustID HAVING (((CustPaid.CustID)=?));"
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            CMD.Parameters.AddWithValue("?", CustID)
            Try
                CN.Open()
                Return CInt(CMD.ExecuteScalar())
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
    End Function
    Private Sub CustDG_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles CustDG.RowEnter
        If e.ColumnIndex = -1 Or e.RowIndex = -1 Then Exit Sub
        If IsNothing(CustDG.CurrentRow) Then Exit Sub

        Dim ThisID As Integer = CustDG(0, e.RowIndex).Value
        MnuDebts.Enabled = True
        Tcode.Text = ThisID.ToString    'Current ID
        Tname.Text = CustDG(1, e.RowIndex).Value.ToString
        Tmob.Text = CustDG(2, e.RowIndex).Value.ToString
        Ttel.Text = CustDG(3, e.RowIndex).Value.ToString
        Tadd.Text = CustDG(4, e.RowIndex).Value.ToString
        Tnotes.Text = CustDG(5, e.RowIndex).Value.ToString
        'UPDATE Vendors
        Cursor = Cursors.WaitCursor
        MnuDisp.Enabled = True
        _CustS.Enabled = False  'Save
        _CMedit.Enabled = True  'Edit
        _CMdel.Enabled = True 'Del
        _CustN.Enabled = True 'New
        'Sell Invoices Count
        Dim AllPaid As Double = GetallPays(CustDG("CustID", e.RowIndex).Value)
        TextBox3.Text = AllSellInv(CustDG("CustID", e.RowIndex).Value).ToString
        Dim numbs As Decimal() = CustDebts(CustDG("CustID", e.RowIndex).Value)
        Dim Debits As Double = numbs.First - AllPaid
        Tcredits.Text = FormatCurrency(Debits.ToString, 2)
        TextBox4.Text = FormatCurrency(numbs.Last().ToString, 2)
        CustPays.ThisCust = CInt(CustDG("CustID", CustDG.CurrentCell.RowIndex).Value)
        CustPays.TxtCustNm.Text = CustDG("CustNm", CustDG.CurrentCell.RowIndex).Value.ToString
        Cursor = Cursors.Default
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
        CustsVends.SrcFrm = "Custms"
        CustsVends.ShowDialog()
    End Sub
    Private Function AllSellInv(CustmrID As Integer) As Integer
        If CustDG.Rows.Count <= 0 Then
            Return 0
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT COUNT(POID) FROM PurOrders WHERE TranID=2 AND CustID=?;"
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            CMD.Parameters.AddWithValue("?", CustmrID)
            Try
                CN.Open()
                Dim C1 As Object = CInt(CMD.ExecuteScalar)
                If Not C1 Is DBNull.Value Or Not C1 Is Nothing Then
                    If C1 > 0 Then
                        Button1.Enabled = True
                    Else
                        Button1.Enabled = False
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
    Private Function CustDebts(CustmrID As Integer) As Decimal()
        Dim MyList As Decimal() = {0}
        If CustDG.Rows.Count <= 0 Then
            Return MyList
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT Sum(PurOrders.PORest) AS SumOfPORest, Sum(PurOrders.POTotal) AS SumOfPOTotal FROM Customers INNER JOIN PurOrders ON " &
            "Customers.CustID = PurOrders.CustID GROUP BY PurOrders.TranID, Customers.CustID HAVING (((PurOrders.TranID)=2) AND " &
            "((Customers.CustID)=?));"
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            CMD.Parameters.AddWithValue("?", CustmrID)
            Try
                CN.Open()
                Using Readr As OleDbDataReader = CMD.ExecuteReader
                    If Readr.HasRows Then
                        Button2.Enabled = True
                        Button3.Enabled = True
                        While Readr.Read
                            MyList = {Readr.GetDecimal(0), Readr.GetDecimal(1)}
                        End While
                        If MyList.Sum = 0 Then
                            Button2.Enabled = True
                            Button3.Enabled = True
                        End If
                    End If
                End Using
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
        Return MyList
    End Function
    Private Sub CustDG_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustDG.CellContentClick

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'بيان أوامر البيع
        If String.IsNullOrEmpty(TextBox3.Text) Then Exit Sub
        CustsVends.SrcFrm = "CustInv"
        Dim Onh1 As Object = Nothing, Onh2 As Object = Nothing
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Dim SqlDel As String = "DROP VIEW CustInvs;"
        Dim SqlStrCreate As String =
            "CREATE VIEW CustInvs AS SELECT PurOrders.POID, PurOrders.PODt, PayTypes.PTNm, Customers.CustNm, Customers.Mob, Customers.Tel, " &
            "Customers.Address, Customers.Notes, PurOrders.POTotal, PurOrders.PODisc, PurOrders.PONet, PurOrders.POPaid, PurOrders.PORest, " &
            "PurOrders.PONots FROM TranTypes INNER JOIN (PayTypes INNER JOIN (Customers INNER JOIN PurOrders ON Customers.CustID = PurOrders.CustID) " &
            "ON PayTypes.PTID = PurOrders.PTId) ON TranTypes.TranID = PurOrders.TranID GROUP BY PurOrders.POID, PurOrders.PODt, PayTypes.PTNm, " &
            "Customers.CustNm, Customers.Mob, Customers.Tel, Customers.Address, Customers.Notes, PurOrders.POTotal, PurOrders.PODisc, PurOrders.PONet, " &
            "PurOrders.POPaid, PurOrders.PORest, PurOrders.PONots, TranTypes.TranID, Customers.CustID HAVING (((TranTypes.TranID)=2) " &
            "AND ((Customers.CustID)=" & CInt(CustDG("CustID", CustDG.CurrentCell.RowIndex).Value.ToString) & "));"
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'بيان اجمالي المديونية
        Button1_Click(sender, e)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'بيان فواتير البيع
        Button1_Click(sender, e)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        'بيان المرتجعات

    End Sub
    Private Function GetallPays1() As Decimal
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Dim SqlStr As String =
           "SELECT Sum(CustPaid.Payamnt) AS SumOfPayamnt FROM CustPaid GROUP BY CustPaid.TranID HAVING (((CustPaid.TranID)=2));"
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
    Private Function AllCustsDebts() As Decimal
        If CustDG.Rows.Count <= 0 Then
            Return 0
            Exit Function
        End If
        Dim SqlStr As String =
            "SELECT Sum(PurOrders.PORest) AS SumOfPORest FROM PurOrders GROUP BY PurOrders.TranID HAVING (((PurOrders.TranID)=2));"
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN) With {.CommandType = CommandType.Text}
            Try
                CN.Open()
                Dim cv As Integer
                Dim queryResult = CMD.ExecuteScalar()
                If queryResult Is DBNull.Value Or IsNothing(queryResult) Then
                    ' No matching records. Do something about it.
                Else
                    cv = DirectCast(queryResult, Decimal)
                End If
                Return Decimal.Parse(cv)
            Catch ex As OleDbException
                MsgBox(ex.Message)
            End Try
        End Using
    End Function
    Private Sub MnuDebts_Click(sender As Object, e As EventArgs) Handles MnuDebts.Click
        TextBox5.Text = FormatCurrency(AllCustsDebts() - GetallPays1(), 2)
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim Ops As New DataOperations
        Dim ConnectionString = Ops.GetEncryConStr
        'بيان أوامر البيع
        If String.IsNullOrEmpty(TextBox5.Text) Then Exit Sub
        CashDetails.TargetForm1 = "AllCustsDebts"
        Dim Onh1 As Object = Nothing, Onh2 As Object = Nothing
        Dim SqlDel As String = "DROP VIEW AllCustsDebtsVals;"
        Dim SqlStrCreate As String =
            "CREATE VIEW AllCustsDebtsVals AS SELECT PurOrders.POID, PurOrders.PODt, Customers.CustNm, PurOrders.PORest, PayTypes.PTNm " &
            "FROM TranTypes INNER JOIN (PayTypes INNER JOIN (Customers INNER JOIN PurOrders ON Customers.CustID = PurOrders.CustID) ON " &
            "PayTypes.PTID = PurOrders.PTId) ON TranTypes.TranID = PurOrders.TranID GROUP BY PurOrders.POID, PurOrders.PODt, " &
            "Customers.CustNm, PurOrders.PORest, PayTypes.PTNm, PurOrders.TranID HAVING (((PurOrders.TranID)=2));"
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
    Private Sub _CstPays_Click(sender As Object, e As EventArgs) Handles _CstPays.Click
        If CustDG.Rows.Count <= 0 Then Exit Sub
        '        CustPays.Label14.Text = "اجمالي المديونية = " & Tcredits.Text
        CustPays.Show(Me)
    End Sub
End Class