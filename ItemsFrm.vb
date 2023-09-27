Imports System.Data.OleDb
Public Class ItemsFrm
    Private Ops As New DataOperations
    Private ConnectionString = Ops.GetEncryConStr
    Private WithEvents DGReady As DataGridView, DT As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
    Private ItmID As Integer
    Private Sub ItemsFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        KeyPreview = True
        GroupBox1.Visible = False
        DGReady = New DataGridView With
        {
            .EnableHeadersVisualStyles = False,
            .EditMode = DataGridViewEditMode.EditOnEnter,
            .RightToLeft = RightToLeft.Yes,
            .Name = "DGReady",
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .ScrollBars = ScrollBars.Both,
            .RowHeadersWidth = 20,
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }
        With DGReady
            .RowHeadersDefaultCellStyle.BackColor = Color.DarkSlateGray
            .RowsDefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .RowsDefaultCellStyle.Font = New Font("Arial Black", 9.25, FontStyle.Regular)
            .AlternatingRowsDefaultCellStyle.BackColor = Color.Silver
            .ColumnHeadersDefaultCellStyle.BackColor = Color.DarkGray
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .ColumnHeadersDefaultCellStyle.Font = New Font("Times New Roman", 13.25, FontStyle.Bold)
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        End With
        AddHandler DGReady.CellFormatting, AddressOf DGReady_CellFormatting
        AddHandler DGReady.CellClick, AddressOf DGReady_CellClick
        GroupBox2.Controls.Add(DGReady)
    End Sub
    Private Sub DGReady_CellClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs)
        If e.ColumnIndex = -1 Or e.ColumnIndex = -1 Then Exit Sub
        Dim i As Integer
        With DGReady
            If e.RowIndex >= 0 Then
                i = .CurrentRow.Index
                ItmID = .Rows(i).Cells("PID").Value.ToString
                TextBox2.Text = .Rows(i).Cells("Pname").Value.ToString
                TextBox4.Text = .Rows(i).Cells("BarCode").Value.ToString
                TextBox5.Text = .Rows(i).Cells("MinQ").Value.ToString
                TextBox6.Text = FormatCurrency(.Rows(i).Cells("Pcost").Value.ToString)
                TextBox3.Text = .Rows(i).Cells("Pdesc").Value.ToString
                MnuEdit.Enabled = True
                MnuDel.Enabled = True
                MnuSave.Enabled = False
            End If
        End With
    End Sub
    Private Sub DGReady_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If DGReady.Columns(e.ColumnIndex).Name = "Pcost" And IsNumeric(e.Value) Then
            e.Value = FormatCurrency(e.Value, 2)
        End If
    End Sub
    Private Sub ItemsFrm_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = ChrW(Keys.Escape) Then Close()
        If e.KeyChar = ChrW(Keys.F5) Then
            DisplayAll()
        End If
    End Sub
    Private Sub MnuSave_Click(sender As Object, e As EventArgs) Handles MnuSave.Click
        If TextBox2.Text.Length <= 0 Or
                TextBox4.Text.Length <= 0 Then
            MsgBox("أدخل اسم الصنف و الباركود أولا",
                   MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.Critical, "برنامج المبيعات و المشتريات")
            Exit Sub
        End If
        Dim AreYouSure As MsgBoxResult =
            MsgBox("تأكيد حفظ أصناف جديدة من ",
                   MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo,
                   "برنامج المبيعات و المشتريات")
        If AreYouSure = MsgBoxResult.Yes Then
            If DT.Columns.Count <= 0 Then
                'DT.Columns.Add("PID", GetType(Integer))
                DT.Columns.Add("Pname", GetType(String))
                DT.Columns.Add("Pdesc", GetType(String))
                DT.Columns.Add("Pcost", GetType(Double))
                DT.Columns.Add("MinQ", GetType(Integer))
                DT.Columns.Add("BarCode", GetType(String))
            End If
            Dim R As DataRow = DT.NewRow
            R("Pname") = TextBox2.Text
            R("Pdesc") = TextBox3.Text
            R("Pcost") = CDbl(TextBox6.Text)
            R("MinQ") = CInt(TextBox5.Text)
            R("BarCode") = TextBox4.Text
            DT.Rows.Add(R)
            Dim Onh As Integer = 0, Onh1 As Object = Nothing
            Dim SqlStr As String =
            "INSERT INTO Products (Pname,Pdesc,Pcost,MinQ,BarCode) VALUES (?,?,?,?,?)"
            Using CN As OleDbConnection = New OleDbConnection(connectionstring),
                CMD As OleDbCommand = New OleDbCommand(SqlStr, CN),
                CMD1 As OleDbCommand = New OleDbCommand("SELECT COUNT(PID) From Products;", CN)
                With CMD.Parameters
                    .AddWithValue("?", R("Pname"))
                    .AddWithValue("?", R("Pdesc"))
                    .AddWithValue("?", R("Pcost"))
                    .AddWithValue("?", R("MinQ"))
                    .AddWithValue("?", R("BarCode"))
                End With
                Try
                    CN.Open()
                    Onh = CMD.ExecuteNonQuery
                    Onh1 = CInt(CMD1.ExecuteScalar)
                    MsgBox("تم الحفظ",
                           MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.Critical,
                           "برنامج المبيعات و المشتريات")
                    TextBox1.Text = Onh1.ToString
                Catch ex As OleDbException
                    MsgBox("عملية غير صحيحة" & vbCrLf & ex.Message,
                           MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.Critical,
                           "برنامج المبيعات و المشتريات")
                Finally
                    CN.Close()
                End Try
            End Using
            With DGReady
                .DataSource = New BindingSource(DT, Nothing)
                .Columns("Pname").HeaderText = "اسم الصنف"
                .Columns("Pdesc").HeaderText = "الوصف"
                .Columns("Pcost").HeaderText = "التكلفة"
                .Columns("MinQ").HeaderText = "أقل كمية"
                .Columns("BarCode").HeaderText = "باركود"
                .Refresh()
            End With
            NewItem()
            DisplayAll()
        End If
    End Sub
    Private Sub NewItem()
        TextBox2.Select()
        TextBox2.Text = String.Empty
        TextBox5.Text = 1
        TextBox6.Text = 0
        TextBox3.Text = "لا يوجد"
        TextBox4.Text = RandomString()
    End Sub
    Private Sub _MPback_Click(sender As Object, e As EventArgs) Handles _MPback.Click
        Close()
    End Sub
    Private Sub MnuNew_Click(sender As Object, e As EventArgs) Handles MnuNew.Click
        MnuSave.Enabled = True
        GroupBox1.Visible = True
        GroupBox2.Visible = True
        If DGReady.Columns.Count > 0 Then
            DGReady.Columns.Clear()
        End If
        NewItem()
    End Sub
    Private Sub MenuStrip1_MouseDown(sender As Object, e As MouseEventArgs) Handles MenuStrip1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub
    Private Sub MnuPOrdrs_Click(sender As Object, e As EventArgs) Handles MnuPOrdrs.Click
        Close()
        PurOrdrs.Show()
    End Sub
    Private Sub MnuUnt_Click(sender As Object, e As EventArgs) Handles MnuUnt.Click
        Basics.TargetForm = "Units"
        Basics.ShowDialog()
    End Sub
    Private Sub MnuStr_Click(sender As Object, e As EventArgs) Handles MnuStr.Click
        Basics.TargetForm = "Stores"
        Basics.ShowDialog()
    End Sub
    Private Sub MnuKnd_Click(sender As Object, e As EventArgs) Handles MnuKnd.Click
        Basics.TargetForm = "Kinds"
        Basics.ShowDialog()
    End Sub
    Private Sub ItemsFrm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler DGReady.CellFormatting, AddressOf DGReady_CellFormatting
        RemoveHandler DGReady.CellClick, AddressOf DGReady_CellClick
    End Sub
    Private Sub Label18_Click(sender As Object, e As EventArgs) Handles Label18.Click
        Location = New Point(0, 0)
        If Height = MainF.Height Then
            Height = MainF.Height / 2
            Exit Sub
        End If
        Height = MainF.Height
    End Sub
    Private Sub Label17_Click(sender As Object, e As EventArgs) Handles Label17.Click
        Location = New Point(0, 0)
        If WindowState = FormWindowState.Maximized Then
            WindowState = FormWindowState.Normal
            Exit Sub
        End If
        WindowState = FormWindowState.Maximized
    End Sub
    Private Sub TextBox6_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox6.KeyPress
        e.Handled = Not (Char.IsDigit(e.KeyChar) Or e.KeyChar = "." Or (e.KeyChar) = ChrW(Keys.Back))
        If e.KeyChar = ChrW(Keys.Enter) Then
            If String.IsNullOrEmpty(TextBox6.Text) Then TextBox6.Text = Val(0)
            Dim DisPO = TextBox6.Text
            TextBox6.Text = FormatCurrency(DisPO, 2)
        End If
    End Sub
    Private Sub MnuDisp_Click(sender As Object, e As EventArgs) Handles MnuDisp.Click
        MnuSave.Enabled = False
        DisplayAll()
    End Sub
    Private Sub DisplayAll()
        If MnuDisp.Enabled = True Then
            With DGReady
                .DataSource = New BindingSource(GetData("SELECT * FROM Products;"), Nothing)
                .Columns("PID").HeaderText = "كود الصنف"
                .Columns("Pname").HeaderText = "اسم الصنف"
                .Columns("Pdesc").HeaderText = "الوصف"
                .Columns("Pcost").HeaderText = "التكلفة"
                .Columns("MinQ").HeaderText = "أقل كمية"
                .Columns("BarCode").HeaderText = "باركود"
                .Refresh()
            End With
        End If
    End Sub
    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) And Not String.IsNullOrEmpty(TextBox2.Text) Then
            'search for name
            Dim Onh As Object
            Using CN As New OleDbConnection(connectionstring),
            CMD As New OleDbCommand("SELECT Count(Products.PID) AS CountOfPID FROM Products HAVING (([products].[pname]=?));", CN)
                CMD.Parameters.AddWithValue("?", TextBox2.Text)
                Try
                    CN.Open()
                    Onh = CInt(CMD.ExecuteScalar)
                    If Onh Is DBNull.Value Then
                        Exit Sub
                    ElseIf Onh > 0 Then
                        Dim AreUsURE As MsgBoxResult =
                            MsgBox("اسم الصنف مسجل من قبل .. اذا اخترت Yes سيتم انشاء صنف جديد بنفس الاسم",
                                   MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.YesNoCancel + MsgBoxStyle.Information + MsgBoxStyle.MsgBoxRight,
                                   "برنامج المبيعات و المشتريات")
                        If AreUsURE <> MsgBoxResult.Yes Then
                            Exit Sub
                        End If
                    End If
                Catch ex As OleDbException
                Finally
                    CN.Close()
                End Try
            End Using
        End If
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub
    Private Sub TextBox2_Click(sender As Object, e As EventArgs) Handles TextBox2.Click
        TextBox2.SelectAll()
    End Sub
    Private Sub TextBox5_Click(sender As Object, e As EventArgs) Handles TextBox5.Click
        TextBox5.SelectAll()
    End Sub
    Private Sub TextBox6_Click(sender As Object, e As EventArgs) Handles TextBox6.Click
        TextBox6.SelectAll()
    End Sub
    Private Sub MnuFNm_Click(sender As Object, e As EventArgs) Handles MnuFNm.Click
        'search ItmName..
        Dim AreuSure As MsgBoxResult
        If DGReady.Rows.Count > 0 Then
            AreuSure =
                MsgBox("هل ترغب فى الغاء العملية الحالية و البدء فى البحث ؟",
                       MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.YesNoCancel + MsgBoxStyle.Critical,
                       "برنامج المبيعات و المشتريات")
            If AreuSure = MsgBoxResult.Yes Then
                Dim SearchRslt As String =
                        InputBox("أدخل اسم الصنف", "بحث - برنامج المبيعات و المشتريات")
                If String.IsNullOrEmpty(SearchRslt) Or
                                String.IsNullOrWhiteSpace(SearchRslt) OrElse SearchRslt.Length <= 0 Then Exit Sub
                Dim Newtble As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
                Newtble = GetData("SELECT * FROM Products WHERE Pname LIKE '%" & SearchRslt & "%';")
                If Newtble.Rows.Count <= 0 Then
                    MnuEdit.Enabled = False
                    MnuDel.Enabled = False
                    MnuSave.Enabled = True
                    MsgBox("لا يوجد صنف بهذا الاسم",, "برنامج المبيعات و المشتريات")
                    Exit Sub
                End If
                MnuEdit.Enabled = True
                MnuDel.Enabled = True
                MnuSave.Enabled = False
                With DGReady
                    .DataSource = New BindingSource(Newtble, Nothing)
                    .Columns("PID").HeaderText = "كود الصنف"
                    .Columns("Pname").HeaderText = "اسم الصنف"
                    .Columns("Pdesc").HeaderText = "الوصف"
                    .Columns("Pcost").HeaderText = "التكلفة"
                    .Columns("MinQ").HeaderText = "أقل كمية"
                    .Columns("BarCode").HeaderText = "باركود"
                    .Refresh()
                End With
            Else
                Exit Sub
            End If
        Else
            Dim SearchRslt As String =
                        InputBox("أدخل اسم الصنف", "بحث - برنامج المبيعات و المشتريات")
            If String.IsNullOrEmpty(SearchRslt) Or
                                String.IsNullOrWhiteSpace(SearchRslt) OrElse SearchRslt.Length <= 0 Then Exit Sub
            Dim Newtble As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
            Newtble = GetData("SELECT * FROM Products WHERE Pname LIKE '%" & SearchRslt & "%';")
            If Newtble.Rows.Count <= 0 Then
                MnuEdit.Enabled = False
                MnuDel.Enabled = False
                MnuSave.Enabled = True
                MsgBox("لا يوجد صنف بهذا الاسم",, "برنامج المبيعات و المشتريات")
                Exit Sub
            End If
            MnuEdit.Enabled = True
            MnuDel.Enabled = True
            MnuSave.Enabled = False
            With DGReady
                .DataSource = New BindingSource(Newtble, Nothing)
                .Columns("PID").HeaderText = "كود الصنف"
                .Columns("Pname").HeaderText = "اسم الصنف"
                .Columns("Pdesc").HeaderText = "الوصف"
                .Columns("Pcost").HeaderText = "التكلفة"
                .Columns("MinQ").HeaderText = "أقل كمية"
                .Columns("BarCode").HeaderText = "باركود"
                .Refresh()
            End With
        End If
    End Sub
    Private Sub MnuFBa_Click(sender As Object, e As EventArgs) Handles MnuFBa.Click
        'search Barcode..
        Dim AreuSure As MsgBoxResult
        If DGReady.Rows.Count > 0 Then
            AreuSure =
                MsgBox("هل ترغب فى الغاء العملية الحالية و البدء فى البحث ؟",
                       MsgBoxStyle.MsgBoxRtlReading + MsgBoxStyle.MsgBoxRight + MsgBoxStyle.YesNoCancel + MsgBoxStyle.Critical,
                       "برنامج المبيعات و المشتريات")
            If AreuSure = MsgBoxResult.Yes Then
                Dim SearchRslt As String =
                        InputBox("أدخل رقم الباركود", "بحث - برنامج المبيعات و المشتريات")
                If String.IsNullOrEmpty(SearchRslt) Or
                                String.IsNullOrWhiteSpace(SearchRslt) OrElse SearchRslt.Length <= 0 Then Exit Sub
                Dim Newtble As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
                Newtble = GetData("SELECT * FROM Products WHERE BarCode = '" & SearchRslt & "';")
                If Newtble.Rows.Count <= 0 Then
                    MnuEdit.Enabled = False
                    MnuDel.Enabled = False
                    MnuSave.Enabled = True
                    MsgBox("رقم الباركود غير موجود",, "برنامج المبيعات و المشتريات")
                    Exit Sub
                End If
                MnuEdit.Enabled = True
                MnuDel.Enabled = True
                MnuSave.Enabled = False
                With DGReady
                    .DataSource = New BindingSource(Newtble, Nothing)
                    .Columns("PID").HeaderText = "كود الصنف"
                    .Columns("Pname").HeaderText = "اسم الصنف"
                    .Columns("Pdesc").HeaderText = "الوصف"
                    .Columns("Pcost").HeaderText = "التكلفة"
                    .Columns("MinQ").HeaderText = "أقل كمية"
                    .Columns("BarCode").HeaderText = "باركود"
                    .Refresh()
                End With
            Else
                Exit Sub
            End If
        Else
            Dim SearchRslt As String =
                        InputBox("أدخل رقم الباركود", "بحث - برنامج المبيعات و المشتريات")
            If String.IsNullOrEmpty(SearchRslt) Or
                                String.IsNullOrWhiteSpace(SearchRslt) OrElse SearchRslt.Length <= 0 Then Exit Sub
            Dim Newtble As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
            Newtble = GetData("SELECT * FROM Products WHERE BarCode = '" & SearchRslt & "';")
            If Newtble.Rows.Count <= 0 Then
                MnuEdit.Enabled = False
                MnuDel.Enabled = False
                MnuSave.Enabled = True
                MsgBox("رقم الباركود غير موجود",, "برنامج المبيعات و المشتريات")
                Exit Sub
            End If
            MnuEdit.Enabled = True
            MnuDel.Enabled = True
            MnuSave.Enabled = False
            With DGReady
                .DataSource = New BindingSource(Newtble, Nothing)
                .Columns("PID").HeaderText = "كود الصنف"
                .Columns("Pname").HeaderText = "اسم الصنف"
                .Columns("Pdesc").HeaderText = "الوصف"
                .Columns("Pcost").HeaderText = "التكلفة"
                .Columns("MinQ").HeaderText = "أقل كمية"
                .Columns("BarCode").HeaderText = "باركود"
                .Refresh()
            End With
        End If
    End Sub
    Private Sub DelItem()
        If Not MnuDel.Enabled = False And Not IsNothing(ItmID) Then
            Dim NewTble As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
            NewTble = GetData("SELECT * FROM PODetails WHERE PID=" & ItmID & ";")
            If NewTble.Rows.Count > 0 Then
                MsgBox("لا يمكن حذف هذا الصنف", MsgBoxStyle.MsgBoxRight + MsgBoxStyle.MsgBoxRtlReading +
                                   MsgBoxStyle.Critical)
                Exit Sub
            Else
                Dim QUERY As String = "DELETE * FROM Products WHERE PID=?;"
                Using CN As New OleDbConnection(connectionstring),
                    CMD As New OleDbCommand(QUERY, CN) With {.CommandType = CommandType.Text}
                    CMD.Parameters.AddWithValue("?", ItmID)
                    Try
                        CN.Open()
                        Dim Onh As Object = CMD.ExecuteNonQuery
                        If Onh Is DBNull.Value Or CInt(Onh) = 0 Then
                            MsgBox("عملية غير صحيحة - لم يتم الحذف", MsgBoxStyle.MsgBoxRight + MsgBoxStyle.MsgBoxRtlReading +
                                   MsgBoxStyle.Critical, "برنامج المبيعات و المشتريات")
                        Else
                            MsgBox("تم حذف الصنف بنجاح", MsgBoxStyle.MsgBoxRight + MsgBoxStyle.MsgBoxRtlReading +
                                   MsgBoxStyle.Information, "برنامج المبيعات و المشتريات")
                            DGReady.DataSource = New BindingSource(GetData("SELECT * FROM Products WHERE PID=" & Onh & ";"), Nothing)
                            DGReady.Refresh()
                        End If
                    Catch ex As OleDbException
                        MsgBox("عملية غير صحيحة - لم يتم الحذف" & ex.Message)
                    Finally
                        CN.Close()
                    End Try
                End Using
            End If
        End If
    End Sub
    Private Sub EditItem()
        If Not MnuEdit.Enabled = False And Not IsNothing(ItmID) Then
            Dim NewTble As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
            NewTble = GetData("SELECT * FROM PODetails WHERE PID=" & ItmID & ";")
            If NewTble.Rows.Count > 0 Then
                MsgBox("لا يمكن تعديل هذا الصنف", MsgBoxStyle.MsgBoxRight + MsgBoxStyle.MsgBoxRtlReading +
                                   MsgBoxStyle.Critical)
                Exit Sub
            Else
                Dim QUERY As String = "UPDATE Products SET Pname=?, Pdesc=?, Pcost=?, MinQ=? WHERE PID=?;"
                Using CN As New OleDbConnection(connectionstring),
                    CMD As New OleDbCommand(QUERY, CN) With {.CommandType = CommandType.Text}
                    CMD.Parameters.AddWithValue("?", TextBox2.Text)
                    CMD.Parameters.AddWithValue("?", TextBox3.Text)
                    CMD.Parameters.AddWithValue("?", CDbl(TextBox6.Text))
                    CMD.Parameters.AddWithValue("?", CInt(TextBox5.Text))
                    CMD.Parameters.AddWithValue("?", ItmID)
                    Try
                        CN.Open()
                        Dim Onh As Object = CMD.ExecuteNonQuery
                        If Onh Is DBNull.Value Or CInt(Onh) = 0 Then
                            MsgBox("عملية غير صحيحة - لم يتم التعديل", MsgBoxStyle.MsgBoxRight + MsgBoxStyle.MsgBoxRtlReading +
                                   MsgBoxStyle.Critical, "برنامج المبيعات و المشتريات")
                        Else
                            MsgBox("تم تعديل الصنف بنجاح", MsgBoxStyle.MsgBoxRight + MsgBoxStyle.MsgBoxRtlReading +
                                   MsgBoxStyle.Information, "برنامج المبيعات و المشتريات")
                            DGReady.DataSource = New BindingSource(GetData("SELECT * FROM Products WHERE PID=" & Onh & ";"), Nothing)
                            DGReady.Refresh()
                        End If
                    Catch ex As OleDbException
                        MsgBox("عملية غير صحيحة - لم يتم التعديل" & ex.Message)
                    Finally
                        CN.Close()
                    End Try
                End Using
            End If
        End If
    End Sub
    Private Sub MnuDel_Click(sender As Object, e As EventArgs) Handles MnuDel.Click
        DelItem()
    End Sub
    Private Sub MnuEdit_Click(sender As Object, e As EventArgs) Handles MnuEdit.Click
        EditItem()
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

    End Sub

    Private Function GetData(query As String) As DataTable
        Using CN As New OleDbConnection(connectionstring),
            CMD As New OleDbCommand(query, CN),
            Sda As New OleDbDataAdapter(CMD),
            MyTable As DataTable = New DataTable With {.Locale = Globalization.CultureInfo.InvariantCulture}
            CN.Open()
            Sda.Fill(MyTable)
            Return MyTable
        End Using
    End Function
End Class