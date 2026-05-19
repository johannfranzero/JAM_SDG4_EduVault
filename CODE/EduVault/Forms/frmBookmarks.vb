Imports System.Windows.Forms
Imports System.Drawing

Public Class frmBookmarks
    Inherits Form
    
    Private ReadOnly _resourceService As New ResourceService()
    Private _allBookmarks As List(Of Bookmark)
    
    Private txtSearch As New TextBox()
    Private lblResultsLeft As New Label()
    Private lblSelectionCount As New Label()
    Private WithEvents dgvBookmarks As New DataGridView()
    
    Private Sub frmBookmarks_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Session.IsGuest OrElse Session.CurrentUserID <= 0 Then
            MessageBox.Show("Bookmarks are available after you sign in with your account.",
                            "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
            Return
        End If

        Me.Text = "EduVault - My Bookmarks"
        Me.Size = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent

        SetupUI()
        LoadBookmarks()
    End Sub

    Private Sub SetupUI()
        Me.BackColor = StyleHelper.ContentBg
        Dim isEmbedded As Boolean = Me.Parent IsNot Nothing
        
        ' --- HEADER ---
        Dim pnlHeader As New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconBookmark & " My Bookmarks",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 16),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)
        If isEmbedded Then pnlHeader.Visible = False
        
        ' --- RESPONSIVE FILTER BAR (TableLayoutPanel) ---
        Dim pnlSearch As New Panel() With {.Dock = DockStyle.Top, .Height = 75, .BackColor = Color.White}
        
        Dim tlpFilters As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(12, 8, 12, 8),
            .RowCount = 1,
            .ColumnCount = 4
        }
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0!)) ' Keyword
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize)) ' Search
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize)) ' Clear
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0!)) ' Spacer
        pnlSearch.Controls.Add(tlpFilters)

        Dim CreateFilterGroup = Function(labelText As String, ctrl As Control) As Panel
                                    Dim grp As New Panel() With {.Dock = DockStyle.Fill, .Margin = New Padding(4, 0, 8, 0)}
                                    Dim lbl As New Label With {
                                        .Text = labelText.ToUpper(),
                                        .Location = New Point(0, 4),
                                        .AutoSize = True,
                                        .Font = New Font("Segoe UI", 7.5, FontStyle.Bold),
                                        .ForeColor = Color.DimGray}
                                    grp.Controls.Add(lbl)
                                    ctrl.Location = New Point(0, 24)
                                    ctrl.Width = 200
                                    ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
                                    ctrl.Height = 28
                                    grp.Controls.Add(ctrl)
                                    Return grp
                                End Function

        txtSearch.Font = New Font("Segoe UI", 9.5)
        txtSearch.BorderStyle = BorderStyle.FixedSingle
        AddHandler txtSearch.KeyDown, Sub(s, e) If e.KeyCode = Keys.Enter Then ApplySearch()
        tlpFilters.Controls.Add(CreateFilterGroup("Search keyword", txtSearch), 0, 0)

        ' Search button
        Dim btnSearch As New Button() With {.Size = New Size(90, 28), .Text = StyleHelper.IconSearch & " Search", .BackColor = StyleHelper.AccentBlue, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnSearch.FlatAppearance.BorderSize = 0
        Dim pnlBtnSearch As New Panel() With {.Size = New Size(90, 56), .Margin = New Padding(4, 0, 4, 0)}
        btnSearch.Location = New Point(0, 24)
        pnlBtnSearch.Controls.Add(btnSearch)
        tlpFilters.Controls.Add(pnlBtnSearch, 1, 0)
        AddHandler btnSearch.Click, Sub() ApplySearch()

        ' Clear button
        Dim btnClear As New Button() With {.Size = New Size(70, 28), .Text = "Clear", .BackColor = Color.White, .ForeColor = Color.DimGray, .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnClear.FlatAppearance.BorderColor = Color.LightGray
        Dim pnlBtnClear As New Panel() With {.Size = New Size(70, 56), .Margin = New Padding(4, 0, 4, 0)}
        btnClear.Location = New Point(0, 24)
        pnlBtnClear.Controls.Add(btnClear)
        tlpFilters.Controls.Add(pnlBtnClear, 2, 0)
        AddHandler btnClear.Click, Sub() 
                                       txtSearch.Clear()
                                       ApplySearch()
                                   End Sub

        ' --- RESULTS INFO BAR ---
        Dim pnlResultsInfo As New Panel() With {.Dock = DockStyle.Top, .Height = 36, .BackColor = StyleHelper.ContentBg}
        lblResultsLeft.Text = StyleHelper.IconBookmark & " 0 bookmark(s) found"
        lblResultsLeft.ForeColor = Color.DimGray
        lblResultsLeft.Font = New Font("Segoe UI", 8.5)
        lblResultsLeft.Location = New Point(16, 10)
        lblResultsLeft.AutoSize = True
        pnlResultsInfo.Controls.Add(lblResultsLeft)

        Dim lblResultsRight As New Label() With {
            .Text = "Sorted by date added " & ChrW(&H2193),
            .ForeColor = Color.DimGray,
            .Font = New Font("Segoe UI", 8.5),
            .Location = New Point(pnlResultsInfo.Width - 160, 10),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
            .AutoSize = True
        }
        pnlResultsInfo.Controls.Add(lblResultsRight)

        ' --- FOOTER ACTION BAR (TableLayoutPanel) ---
        Dim pnlFooter As New Panel() With {.Dock = DockStyle.Bottom, .Height = 64, .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
        
        Dim tlpFooter As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .Padding = New Padding(12), .RowCount = 1, .ColumnCount = 3
        }
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
        pnlFooter.Controls.Add(tlpFooter)

        Dim flpLeft As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False}
        Dim flpRight As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False, .FlowDirection = FlowDirection.RightToLeft}
        tlpFooter.Controls.Add(flpLeft, 0, 0)
        tlpFooter.Controls.Add(flpRight, 2, 0)

        lblSelectionCount.Text = "0 selected"
        lblSelectionCount.ForeColor = Color.DimGray
        lblSelectionCount.Font = New Font("Segoe UI", 9)
        lblSelectionCount.Anchor = AnchorStyles.None
        lblSelectionCount.AutoSize = True
        tlpFooter.Controls.Add(lblSelectionCount, 1, 0)

        Dim BuildActionBtn = Function(txt As String, bg As Color, fg As Color, w As Integer, margin As Padding) As Button
                                 Dim b As New Button() With {.Text = txt, .BackColor = bg, .ForeColor = fg, .FlatStyle = FlatStyle.Flat, .Size = New Size(w, 32), .Margin = margin, .Font = New Font("Segoe UI", 9), .Cursor = Cursors.Hand}
                                 b.FlatAppearance.BorderSize = If(bg = Color.White, 1, 0)
                                 b.FlatAppearance.BorderColor = Color.LightGray
                                 Return b
                             End Function

        Dim btnRemove As Button = BuildActionBtn(StyleHelper.IconDelete & " Remove", Color.FromArgb(220, 38, 38), Color.White, 100, New Padding(0))
        flpLeft.Controls.Add(btnRemove)
        AddHandler btnRemove.Click, AddressOf btnRemove_Click

        Dim btnRefresh As Button = BuildActionBtn(StyleHelper.IconRefresh & " Refresh", Color.White, Color.DimGray, 90, New Padding(10, 0, 0, 0))
        Dim btnExport As Button = BuildActionBtn(StyleHelper.IconExport & " Export CSV", Color.FromArgb(52, 73, 94), Color.White, 110, New Padding(0))
        flpRight.Controls.Add(btnRefresh)
        flpRight.Controls.Add(btnExport)
        AddHandler btnRefresh.Click, Sub() LoadBookmarks()
        AddHandler btnExport.Click, AddressOf btnExport_Click

        ' --- DATAGRIDVIEW ---
        dgvBookmarks.Dock = DockStyle.Fill
        dgvBookmarks.AllowUserToAddRows = False
        dgvBookmarks.ReadOnly = True
        dgvBookmarks.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvBookmarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        
        StyleHelper.ApplyGridStyle(dgvBookmarks)
        
        dgvBookmarks.Columns.Add("colID", "ID") : dgvBookmarks.Columns("colID").Visible = False
        
        Dim colCheck As New DataGridViewCheckBoxColumn() With {.Name = "colCheck", .HeaderText = "☐"}
        colCheck.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colCheck.Width = 40
        colCheck.MinimumWidth = 40
        dgvBookmarks.Columns.Add(colCheck)

        Dim colTitle As New DataGridViewTextBoxColumn() With {.Name = "colTitle", .HeaderText = "TITLE"}
        colTitle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colTitle.FillWeight = 40
        colTitle.DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        dgvBookmarks.Columns.Add(colTitle)
        
        Dim colCat As New DataGridViewTextBoxColumn() With {.Name = "colCategory", .HeaderText = "CATEGORY"}
        colCat.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colCat.FillWeight = 20
        dgvBookmarks.Columns.Add(colCat)

        Dim colType As New DataGridViewTextBoxColumn() With {.Name = "colType", .HeaderText = "TYPE"}
        colType.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colType.FillWeight = 15
        dgvBookmarks.Columns.Add(colType)

        Dim colDate As New DataGridViewTextBoxColumn() With {.Name = "colDate", .HeaderText = "ADDED ON"}
        colDate.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colDate.FillWeight = 25
        dgvBookmarks.Columns.Add(colDate)
        
        AddHandler dgvBookmarks.CellDoubleClick, AddressOf dgvBookmarks_CellDoubleClick
        AddHandler dgvBookmarks.SelectionChanged, Sub() lblSelectionCount.Text = $"{dgvBookmarks.SelectedRows.Count} selected"
        
        Me.Controls.Add(dgvBookmarks)
        Me.Controls.Add(pnlResultsInfo)
        Me.Controls.Add(pnlSearch)
        Me.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlHeader)
        
        pnlSearch.BringToFront()
        pnlResultsInfo.BringToFront()
        dgvBookmarks.BringToFront()
    End Sub

    Private Sub LoadBookmarks()
        _allBookmarks = _resourceService.GetUserBookmarks(Session.CurrentUserID)
        ApplySearch()
    End Sub

    Private Sub ApplySearch()
        If _allBookmarks Is Nothing Then Return
        Dim keyword As String = txtSearch.Text.Trim().ToLower()
        Dim filtered As List(Of Bookmark) = _allBookmarks
        
        If Not String.IsNullOrEmpty(keyword) Then
            filtered = _allBookmarks.Where(Function(b) b.ResourceTitle.ToLower().Contains(keyword) OrElse (b.CategoryName IsNot Nothing AndAlso b.CategoryName.ToLower().Contains(keyword))).ToList()
        End If
        
        BindGrid(filtered)
    End Sub

    Private Sub BindGrid(list As List(Of Bookmark))
        dgvBookmarks.Rows.Clear()
        For Each b In list
            dgvBookmarks.Rows.Add(b.ResourceID, False, b.ResourceTitle, If(b.CategoryName, "Uncategorized"), b.ResourceType, b.DateBookmarked.ToString("MMM dd, yyyy"))
        Next
        lblResultsLeft.Text = StyleHelper.IconBookmark & $" {list.Count} bookmark(s) found"
        lblSelectionCount.Text = $"{dgvBookmarks.SelectedRows.Count} selected"
    End Sub

    Private Sub dgvBookmarks_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then Return
        Dim resID As Integer = CInt(dgvBookmarks.Rows(e.RowIndex).Cells("colID").Value)
        Dim res As Resource = _resourceService.GetResourceByID(resID)
        
        If res IsNot Nothing Then
            _resourceService.RecordAccess(Session.CurrentUserID, resID, "View")
            If Not String.IsNullOrWhiteSpace(res.URL) Then
                System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(res.URL) With {.UseShellExecute = True})
            ElseIf Not String.IsNullOrWhiteSpace(res.FilePath) Then
                System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(res.FilePath) With {.UseShellExecute = True})
            End If
        End If
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs)
        If dgvBookmarks.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a bookmark to remove.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim resID As Integer = CInt(dgvBookmarks.SelectedRows(0).Cells("colID").Value)
        Dim title As String = dgvBookmarks.SelectedRows(0).Cells("colTitle").Value.ToString()
        If MessageBox.Show($"Remove '{title}' from your bookmarks?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim err As String = String.Empty
            If _resourceService.RemoveBookmark(Session.CurrentUserID, resID, err) Then
                NotificationHelper.ShowInfo("Bookmark removed.")
                LoadBookmarks()
            Else
                MessageBox.Show(err, "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs)
        Using sfd As New SaveFileDialog()
            sfd.Filter = "CSV Files (*.csv)|*.csv"
            sfd.FileName = "MyBookmarks.csv"
            If sfd.ShowDialog() = DialogResult.OK Then
                Dim dt As DataTable = _resourceService.GetBookmarksDataTable(Session.CurrentUserID)
                Dim rpt As New ReportService()
                rpt.ExportToCsv(dt, sfd.FileName)
                NotificationHelper.ShowInfo("Bookmarks exported!")
            End If
        End Using
    End Sub
End Class
