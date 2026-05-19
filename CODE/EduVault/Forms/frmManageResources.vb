''' <summary>
''' frmManageResources - Resource Browse and Management Form (Presentation Layer)
''' Admin: Full CRUD - Add, Edit, Delete, Search, Filter
''' Student: Read-only browse + Bookmark
''' </summary>
Public Class frmManageResources

    Private ReadOnly _resourceService As New ResourceService()
    Private ReadOnly _categoryService As New CategoryService()
    Private _allResources As List(Of Resource)
    Private ReadOnly _searchAutocomplete As New AutoCompleteStringCollection()

    ''' <summary>Set by the caller (e.g. frmDashboard) to pre-fill the search box on load.</summary>
    Public Property InitialSearchKeyword As String = String.Empty

    ' ─────────────────────────────────────────────────────────────
    ' FORM LOAD
    ' ─────────────────────────────────────────────────────────────

    Private Sub frmManageResources_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupUIForRole()
        BuildRedesignedUI()
        LoadFilterDropdowns()
        LoadResources()

        ' If a search keyword was passed from the dashboard, apply it immediately
        If Not String.IsNullOrWhiteSpace(InitialSearchKeyword) Then
            txtSearch.Text = InitialSearchKeyword
            ApplySearch()
        End If

        ApplyStyles()
        SetupSearchAutocomplete()
        SetupResourceGridContextMenu()
    End Sub

    Private Sub SetupResourceGridContextMenu()
        Dim menu As New ContextMenuStrip()
        Dim mnuRate As New ToolStripMenuItem("Rate resource...")
        AddHandler mnuRate.Click,
            Sub()
                Dim res = GetSelectedResource()
                If res Is Nothing Then Return
                Using dlg As New frmRateResource(res.Title)
                    If dlg.ShowDialog(Me) <> DialogResult.OK Then Return
                    Dim err As String = String.Empty
                    If _resourceService.RateResource(Session.CurrentUserID, res.ResourceID, dlg.SelectedStars, dlg.ReviewText, err) Then
                        NotificationHelper.ShowInfo("Thank you for your rating!")
                    Else
                        MessageBox.Show(err, "Rating", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Sub
        menu.Items.Add(mnuRate)

        If Not Session.IsGuest Then
            Dim mnuFav As New ToolStripMenuItem("Add to favourites list...")
            AddHandler mnuFav.Click,
                Sub()
                    Dim res = GetSelectedResource()
                    If res Is Nothing Then Return
                    Dim listName As String = InputBox("List name:", "Favourites", "My Favourites")
                    If String.IsNullOrWhiteSpace(listName) Then Return
                    Dim err As String = String.Empty
                    If _resourceService.AddToFavouritesList(Session.CurrentUserID, res.ResourceID, listName, err) Then
                        NotificationHelper.ShowInfo("Added to favourites list.")
                    Else
                        MessageBox.Show(err, "Favourites", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Sub
            menu.Items.Add(mnuFav)
        End If

        dgvResources.ContextMenuStrip = menu
    End Sub

    Private Sub SetupSearchAutocomplete()
        txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource
        txtSearch.AutoCompleteCustomSource = _searchAutocomplete
    End Sub

    Private Sub ApplyStyles()
        StyleHelper.ApplyHeaderStyle(pnlHeader)
        lblFormTitle.Font = StyleHelper.HeaderFont

        pnlSearch.BackColor = StyleHelper.WhiteColor
        lblSearchTitle.Font = StyleHelper.SubHeaderFont
        lblCat.Font = StyleHelper.SubHeaderFont
        lblType.Font = StyleHelper.SubHeaderFont
        lblLevel.Font = StyleHelper.SubHeaderFont

        StyleHelper.ApplyButtonStyle(btnSearch)
        StyleHelper.ApplyButtonStyle(btnClearSearch)
        StyleHelper.ApplyButtonStyle(btnAdd, isAccent:=True)
        StyleHelper.ApplyButtonStyle(btnEdit)
        StyleHelper.ApplyButtonStyle(btnDelete, isDanger:=True)
        StyleHelper.ApplyButtonStyle(btnBookmark, isAccent:=True)
        StyleHelper.ApplyButtonStyle(btnRefresh)
        StyleHelper.ApplyButtonStyle(btnExport)
        btnExport.BackColor = Color.FromArgb(52, 73, 94) ' Slate blue for utility

        StyleHelper.ApplyGridStyle(dgvResources)
        ConfigureResourceGridColumns()

        pnlButtons.BackColor = Color.FromArgb(240, 243, 244)
    End Sub

    Private Sub ConfigureResourceGridColumns()
        dgvResources.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvResources.ScrollBars = ScrollBars.Vertical
        dgvResources.ColumnHeadersHeight = 40
        dgvResources.RowTemplate.Height = 40

        If dgvResources.Columns.Contains("colCheck") Then
            With dgvResources.Columns("colCheck")
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Width = 40
                .MinimumWidth = 40
                .HeaderText = "☐"
            End With
        End If

        With dgvResources.Columns("colResTag")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            .Width = 72
            .MinimumWidth = 72
        End With

        With dgvResources.Columns("colResTitle")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .FillWeight = 30
            .MinimumWidth = 140
            .DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        With dgvResources.Columns("colResCat")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .FillWeight = 15
            .MinimumWidth = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        With dgvResources.Columns("colResType")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .FillWeight = 12
            .MinimumWidth = 80
        End With

        With dgvResources.Columns("colResLevel")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .FillWeight = 10
            .MinimumWidth = 88
        End With

        With dgvResources.Columns("colResSubject")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .FillWeight = 15
            .MinimumWidth = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        With dgvResources.Columns("colResViews")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            .Width = 72
            .MinimumWidth = 60
        End With

        With dgvResources.Columns("colResDate")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            .Width = 108
            .MinimumWidth = 100
        End With

        With dgvResources.Columns("colResAddedBy")
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .FillWeight = 15
            .MinimumWidth = 120
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        For Each col As DataGridViewColumn In dgvResources.Columns
            If col.Visible AndAlso col.Name <> "colCheck" Then
                col.HeaderText = col.HeaderText.ToUpper()
            End If
        Next
    End Sub

    Private Shared Function PillRect(cellBounds As Rectangle, maxWidth As Integer, Optional pillHeight As Integer = 22) As Rectangle
        Dim height As Integer = Math.Min(pillHeight, cellBounds.Height - 6)
        Dim width As Integer = Math.Min(maxWidth, cellBounds.Width - 8)
        Return New Rectangle(
            cellBounds.X + 4,
            cellBounds.Y + (cellBounds.Height - height) \ 2,
            width,
            height)
    End Function

    Private Sub pnlSearch_Paint(sender As Object, e As PaintEventArgs) Handles pnlSearch.Paint
        StyleHelper.DrawCardShadow(sender, e)
    End Sub

    Private Sub SetupUIForRole()
        If Session.IsAdmin Then
            Me.Text = "EduVault - Manage Resources (Admin)"
            btnAdd.Visible = True
            btnEdit.Visible = True
            btnDelete.Visible = True
            btnBookmark.Visible = False  ' Bookmarks are a Student feature
        ElseIf Session.IsGuest Then
            Me.Text = "EduVault - Browse Resources (Guest)"
            btnAdd.Visible = False
            btnEdit.Visible = False
            btnDelete.Visible = False
            btnBookmark.Visible = False
        Else
            Me.Text = "EduVault - Browse Resources"
            btnAdd.Visible = False
            btnEdit.Visible = False
            btnDelete.Visible = False
            btnBookmark.Visible = True
            btnBookmark.Text = "Bookmark"
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' DATA LOADING
    ' ─────────────────────────────────────────────────────────────

    Private Sub LoadFilterDropdowns()
        Try
            ' Category ComboBox
            cmbFilterCategory.Items.Clear()
            cmbFilterCategory.Items.Add(New Category() With {.CategoryID = 0, .CategoryName = "All Categories"})
            For Each cat As Category In _categoryService.GetAllCategories()
                cmbFilterCategory.Items.Add(cat)
            Next
            cmbFilterCategory.SelectedIndex = 0

            ' Type ComboBox
            cmbFilterType.Items.Clear()
            cmbFilterType.Items.Add("All Types")
            cmbFilterType.Items.AddRange({"E-Book", "Video", "Module", "Reference", "Article"})
            cmbFilterType.SelectedIndex = 0

            ' Level ComboBox
            cmbFilterLevel.Items.Clear()
            cmbFilterLevel.Items.Add("All Levels")
            cmbFilterLevel.Items.AddRange({"Beginner", "Intermediate", "Advanced"})
            cmbFilterLevel.SelectedIndex = 0

        Catch ex As Exception
            MessageBox.Show($"Error loading filters: {ex.Message}", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub LoadResources()
        Try
            _allResources = _resourceService.GetAllResources()
            BindResourcesToGrid(_allResources)
            lblCount.Text = $"{_allResources.Count} resource(s) found"
        Catch ex As Exception
            MessageBox.Show($"Error loading resources: {ex.Message}", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BindResourcesToGrid(resources As List(Of Resource))
        dgvResources.Rows.Clear()
        Dim sorted = resources.OrderByDescending(Function(r) r.DateAdded).ToList()
        For Each res As Resource In sorted
            Dim tag As String = _resourceService.GetEngagementTag(res)

            Dim hasCheckCol As Boolean = dgvResources.Columns.Contains("colCheck") AndAlso dgvResources.Columns("colCheck").Index = 0

            If hasCheckCol Then
                dgvResources.Rows.Add(
                    False,
                    res.ResourceID,
                    tag,
                    res.Title,
                    res.CategoryName,
                    res.ResourceType,
                    res.EducationLevel,
                    res.SubjectArea,
                    res.ViewCount,
                    res.DateAdded.ToString("MMM dd, yyyy"),
                    res.AddedByName
                )
            Else
                dgvResources.Rows.Add(
                    res.ResourceID,
                    tag,
                    res.Title,
                    res.CategoryName,
                    res.ResourceType,
                    res.EducationLevel,
                    res.SubjectArea,
                    res.ViewCount,
                    res.DateAdded.ToString("MMM dd, yyyy"),
                    res.AddedByName
                )
            End If
        Next
        lblCount.Text = $"{resources.Count} resource(s) found"

        ' Update SPA specific labels if they exist
        If lblResultsLeft IsNot Nothing AndAlso lblResultsLeft.Parent IsNot Nothing Then
            lblResultsLeft.Text = StyleHelper.IconLibrary & $" {resources.Count} resource(s) found"
        End If
        If lblCountText IsNot Nothing AndAlso lblCountText.Parent IsNot Nothing Then
            lblCountText.Text = $"{resources.Count} resources"
        End If
        If lblResultsRight IsNot Nothing AndAlso lblResultsRight.Parent IsNot Nothing Then
            lblResultsRight.Text = "Sorted by date added ↓"
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' SEARCH / FILTER
    ' ─────────────────────────────────────────────────────────────

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        ApplySearch()
    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Enter Then ApplySearch()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        _searchAutocomplete.Clear()
        Dim prefix As String = txtSearch.Text.Trim()
        If prefix.Length < 2 Then Return
        For Each title In _resourceService.SearchTitlesForAutocomplete(prefix)
            _searchAutocomplete.Add(title)
        Next
    End Sub

    Private Sub btnClearSearch_Click(sender As Object, e As EventArgs) Handles btnClearSearch.Click
        txtSearch.Clear()
        cmbFilterCategory.SelectedIndex = 0
        cmbFilterType.SelectedIndex = 0
        cmbFilterLevel.SelectedIndex = 0
        BindResourcesToGrid(_allResources)
    End Sub

    Private Sub ApplySearch()
        Try
            Dim keyword As String = txtSearch.Text.Trim()
            Dim selectedCat As Category = TryCast(cmbFilterCategory.SelectedItem, Category)
            Dim categoryID As Integer = If(selectedCat IsNot Nothing, selectedCat.CategoryID, 0)
            Dim resType As String = If(cmbFilterType.SelectedIndex > 0, cmbFilterType.SelectedItem.ToString(), String.Empty)
            Dim resLevel As String = If(cmbFilterLevel.SelectedIndex > 0, cmbFilterLevel.SelectedItem.ToString(), String.Empty)

            Dim errorMessage As String = String.Empty
            Dim results As List(Of Resource) = _resourceService.SearchResources(
                keyword, categoryID, resType, resLevel, errorMessage)

            If Not String.IsNullOrEmpty(errorMessage) Then
                lblCount.ForeColor = Color.Red
                lblCount.Text = $"[X] {errorMessage}"
                Return
            End If

            lblCount.ForeColor = SystemColors.ControlText
            BindResourcesToGrid(results)

        Catch ex As Exception
            MessageBox.Show($"Search error: {ex.Message}", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' CRUD BUTTON EVENTS
    ' ─────────────────────────────────────────────────────────────

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim frm As New frmAddEditResource(Nothing)
        frm.ShowDialog(Me)
        LoadResources()
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Dim selectedRes As Resource = GetSelectedResource()
        If selectedRes Is Nothing Then
            MessageBox.Show("Please select a resource to edit.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim frm As New frmAddEditResource(selectedRes)
        frm.ShowDialog(Me)
        LoadResources()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim selectedRes As Resource = GetSelectedResource()
        If selectedRes Is Nothing Then
            MessageBox.Show("Please select a resource to delete.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim confirm As DialogResult = MessageBox.Show(
            $"Are you sure you want to delete:""{Environment.NewLine}{selectedRes.Title}""{Environment.NewLine}{Environment.NewLine}This action cannot be undone.",
            "Confirm Delete - EduVault",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        )

        If confirm = DialogResult.Yes Then
            Try
                Dim errorMessage As String = String.Empty
                Dim success As Boolean = _resourceService.DeleteResource(selectedRes.ResourceID, errorMessage)

                If success Then
                    NotificationHelper.ShowInfo("Resource deleted successfully.")
                    LoadResources()
                Else
                    MessageBox.Show($"Delete failed: {errorMessage}", "EduVault",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"Unexpected error: {ex.Message}", "EduVault",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnBookmark_Click(sender As Object, e As EventArgs) Handles btnBookmark.Click
        Dim selectedRes As Resource = GetSelectedResource()
        If selectedRes Is Nothing Then
            MessageBox.Show("Please select a resource to bookmark.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Try
            Dim errorMessage As String = String.Empty
            Dim success As Boolean = _resourceService.BookmarkResource(
                Session.CurrentUserID, selectedRes.ResourceID, errorMessage)

            If success Then
                NotificationHelper.ShowInfo($"Bookmarked: {selectedRes.Title}")
            Else
                MessageBox.Show(errorMessage, "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Bookmark error: {ex.Message}", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadResources()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' GRID EVENTS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Double-click a resource to open its URL in the browser.</summary>
    Private Sub dgvResources_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvResources.CellDoubleClick
        If e.RowIndex < 0 Then Return
        Dim res As Resource = GetSelectedResource()
        If res Is Nothing Then Return

        Try
            _resourceService.OpenResource(res, Session.CurrentUserID)
        Catch ex As Exception
            MessageBox.Show($"Cannot open resource: {ex.Message}", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try

        LoadResources()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' HELPERS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Returns the Resource model for the currently selected DataGridView row using the cached list.</summary>
    Private Function GetSelectedResource() As Resource
        If dgvResources.SelectedRows.Count = 0 Then Return Nothing
        Dim resourceID As Integer = CInt(dgvResources.SelectedRows(0).Cells("colResID").Value)
        ' Use cached list instead of DB round-trip
        If _allResources IsNot Nothing Then
            Dim cached As Resource = _allResources.FirstOrDefault(Function(r) r.ResourceID = resourceID)
            If cached IsNot Nothing Then Return cached
        End If
        ' Fallback to DB if not found in cache (e.g. after search)
        Return _resourceService.GetResourceByID(resourceID)
    End Function

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfd As New SaveFileDialog()
            sfd.Title = "Export Resources to CSV"
            sfd.Filter = "CSV Files (*.csv)|*.csv"
            sfd.FileName = $"EduVault_Resources_{DateTime.Now:yyyyMMdd_HHmm}.csv"

            If sfd.ShowDialog() = DialogResult.OK Then
                Try
                    Dim resources As List(Of Resource) = _resourceService.GetAllResources()
                    Dim dt As New DataTable("Resources")
                    dt.Columns.Add("ID")
                    dt.Columns.Add("Tag")
                    dt.Columns.Add("Title")
                    dt.Columns.Add("Category")
                    dt.Columns.Add("Type")
                    dt.Columns.Add("Level")
                    dt.Columns.Add("Views")
                    dt.Columns.Add("DateAdded")

                    For Each r In resources
                        dt.Rows.Add(r.ResourceID, r.Tags, r.Title, r.CategoryName, r.ResourceType, r.EducationLevel, r.ViewCount, r.DateAdded)
                    Next

                    Dim reportSvc As New ReportService()
                    reportSvc.ExportToCsv(dt, sfd.FileName)

                    NotificationHelper.ShowInfo("Resources exported successfully!")
                Catch ex As Exception
                    MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    ' ---------------------------------------------------------------
    ' REDESIGN IMPLEMENTATION
    ' ---------------------------------------------------------------
    Private pnlTopBar As New Panel()
    Private lblTitleIcon As New Label()
    Private pnlCountBadge As New Panel()
    Private lblCountText As New Label()

    Private tlpFilters As New TableLayoutPanel()
    Private pnlResultsInfo As New Panel()
    Private lblResultsLeft As New Label()
    Private lblResultsRight As New Label()

    Private pnlFooter As New Panel()
    Private lblSelectionCount As New Label()
    Private colCheck As New DataGridViewCheckBoxColumn()

    Private Sub BuildRedesignedUI()
        Me.Controls.Clear()

        Dim isEmbedded As Boolean = Me.Parent IsNot Nothing

        ' --- TOP BAR (hidden when embedded to avoid double-header) ---
        pnlTopBar.Dock = DockStyle.Top
        pnlTopBar.Height = 52
        pnlTopBar.BackColor = StyleHelper.PrimaryColor

        lblTitleIcon.Text = StyleHelper.IconLibrary & " Browse / Manage Resources"
        lblTitleIcon.Font = New Font("Segoe UI", 12)
        lblTitleIcon.ForeColor = Color.White
        lblTitleIcon.AutoSize = True
        lblTitleIcon.Location = New Point(16, 16)

        pnlCountBadge.Size = New Size(90, 24)
        pnlCountBadge.BackColor = Color.FromArgb(40, 50, 75)
        pnlCountBadge.Location = New Point(Me.Width - 120, 14)
        pnlCountBadge.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        StyleHelper.ApplyRoundedRegion(pnlCountBadge, 12)

        lblCountText.Text = "0 resources"
        lblCountText.ForeColor = Color.FromArgb(200, 210, 230)
        lblCountText.Font = New Font("Segoe UI", 8.5)
        lblCountText.AutoSize = False
        lblCountText.Dock = DockStyle.Fill
        lblCountText.TextAlign = ContentAlignment.MiddleCenter
        pnlCountBadge.Controls.Add(lblCountText)

        pnlTopBar.Controls.Add(lblTitleIcon)
        pnlTopBar.Controls.Add(pnlCountBadge)
        Me.Controls.Add(pnlTopBar)

        ' Hide top bar when embedded (dashboard title bar provides the heading)
        If isEmbedded Then
            pnlTopBar.Visible = False
        End If

        ' --- GUEST BANNER ---
        If Session.IsGuest Then
            Dim pnlGuestBanner As New Panel()
            pnlGuestBanner.Dock = DockStyle.Top
            pnlGuestBanner.Height = 32
            pnlGuestBanner.BackColor = StyleHelper.GuestBannerBg
            Dim lblBanner As New Label()
            lblBanner.Text = ChrW(&HE7BA) & "  Browsing as guest " & ChrW(&H2014) & " bookmarks won't be saved."
            lblBanner.Font = New Font("Segoe UI", 8.5)
            lblBanner.ForeColor = StyleHelper.GuestBannerFg
            lblBanner.AutoSize = True
            lblBanner.Location = New Point(12, 7)
            pnlGuestBanner.Controls.Add(lblBanner)
            Me.Controls.Add(pnlGuestBanner)
            pnlGuestBanner.BringToFront()
        End If

        ' --- RESPONSIVE FILTER BAR (TableLayoutPanel) ---
        pnlSearch.Controls.Clear()
        pnlSearch.Dock = DockStyle.Top
        pnlSearch.Height = 75
        pnlSearch.BackColor = Color.White
        Me.Controls.Add(pnlSearch)
        pnlSearch.BringToFront()

        Dim tlpFilters As New TableLayoutPanel()
        tlpFilters.Dock = DockStyle.Fill
        tlpFilters.Padding = New Padding(12, 8, 12, 8)
        tlpFilters.RowCount = 1
        tlpFilters.ColumnCount = 7
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 28.0!)) ' Keyword
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 22.0!)) ' Category
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 18.0!)) ' Type
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 18.0!)) ' Level
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize)) ' Search Btn
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize)) ' Clear Btn
        tlpFilters.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 14.0!)) ' Spacer
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
                                    ctrl.Width = 200 ' Will be anchored
                                    ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
                                    ctrl.Height = 28
                                    grp.Controls.Add(ctrl)
                                    Return grp
                                End Function

        txtSearch.Font = New Font("Segoe UI", 9.5)
        txtSearch.BorderStyle = BorderStyle.FixedSingle
        tlpFilters.Controls.Add(CreateFilterGroup("Search keyword", txtSearch), 0, 0)
        tlpFilters.Controls.Add(CreateFilterGroup("Category", cmbFilterCategory), 1, 0)
        tlpFilters.Controls.Add(CreateFilterGroup("Resource type", cmbFilterType), 2, 0)
        tlpFilters.Controls.Add(CreateFilterGroup("Level", cmbFilterLevel), 3, 0)

        ' Search button
        btnSearch.Size = New Size(90, 28)
        btnSearch.Text = StyleHelper.IconSearch & " Search"
        btnSearch.BackColor = StyleHelper.AccentBlue
        btnSearch.ForeColor = Color.White
        btnSearch.FlatStyle = FlatStyle.Flat
        btnSearch.FlatAppearance.BorderSize = 0
        Dim pnlBtnSearch As New Panel() With {.Size = New Size(90, 56), .Margin = New Padding(4, 0, 4, 0)}
        btnSearch.Location = New Point(0, 24)
        pnlBtnSearch.Controls.Add(btnSearch)
        tlpFilters.Controls.Add(pnlBtnSearch, 4, 0)

        ' Clear button
        btnClearSearch.Size = New Size(70, 28)
        btnClearSearch.Text = "Clear"
        btnClearSearch.BackColor = Color.White
        btnClearSearch.ForeColor = Color.DimGray
        btnClearSearch.FlatStyle = FlatStyle.Flat
        btnClearSearch.FlatAppearance.BorderColor = Color.LightGray
        Dim pnlBtnClear As New Panel() With {.Size = New Size(70, 56), .Margin = New Padding(4, 0, 4, 0)}
        btnClearSearch.Location = New Point(0, 24)
        pnlBtnClear.Controls.Add(btnClearSearch)
        tlpFilters.Controls.Add(pnlBtnClear, 5, 0)

        ' --- RESULTS INFO BAR ---
        pnlResultsInfo.Dock = DockStyle.Top
        pnlResultsInfo.Height = 36
        pnlResultsInfo.BackColor = StyleHelper.ContentBg
        Me.Controls.Add(pnlResultsInfo)
        pnlResultsInfo.BringToFront()

        lblResultsLeft.Text = StyleHelper.IconLibrary & " 0 resource(s) found"
        lblResultsLeft.ForeColor = Color.DimGray
        lblResultsLeft.Font = New Font("Segoe UI", 8.5)
        lblResultsLeft.Location = New Point(16, 10)
        lblResultsLeft.AutoSize = True
        pnlResultsInfo.Controls.Add(lblResultsLeft)

        lblResultsRight.Text = "Sorted by date added " & ChrW(&H2193)
        lblResultsRight.ForeColor = Color.DimGray
        lblResultsRight.Font = New Font("Segoe UI", 8.5)
        lblResultsRight.Location = New Point(pnlResultsInfo.Width - 160, 10)
        lblResultsRight.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        lblResultsRight.AutoSize = True
        pnlResultsInfo.Controls.Add(lblResultsRight)

        ' --- FOOTER ACTION BAR (TableLayoutPanel) ---
        pnlFooter.Controls.Clear()
        pnlFooter.Dock = DockStyle.Bottom
        pnlFooter.Height = 64
        pnlFooter.BackColor = Color.White
        pnlFooter.BorderStyle = BorderStyle.FixedSingle
        Me.Controls.Add(pnlFooter)

        Dim tlpFooter As New TableLayoutPanel()
        tlpFooter.Dock = DockStyle.Fill
        tlpFooter.Padding = New Padding(12, 12, 12, 12)
        tlpFooter.RowCount = 1
        tlpFooter.ColumnCount = 3
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!)) ' Left Buttons
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize)) ' Selection Count
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!)) ' Right Buttons
        pnlFooter.Controls.Add(tlpFooter)

        Dim flpLeft As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False}
        Dim flpRight As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False, .FlowDirection = FlowDirection.RightToLeft}
        tlpFooter.Controls.Add(flpLeft, 0, 0)
        tlpFooter.Controls.Add(lblSelectionCount, 1, 0)
        tlpFooter.Controls.Add(flpRight, 2, 0)

        lblSelectionCount.Text = "0 selected"
        lblSelectionCount.ForeColor = Color.DimGray
        lblSelectionCount.Font = New Font("Segoe UI", 9)
        lblSelectionCount.Anchor = AnchorStyles.None
        lblSelectionCount.AutoSize = True

        Dim BuildActionBtn = Function(txt As String, bg As Color, fg As Color, w As Integer, margin As Padding) As Button
                                 Dim b As New Button()
                                 b.Text = txt
                                 b.BackColor = bg
                                 b.ForeColor = fg
                                 b.FlatStyle = FlatStyle.Flat
                                 b.FlatAppearance.BorderSize = If(bg = Color.White, 1, 0)
                                 b.FlatAppearance.BorderColor = Color.LightGray
                                 b.Size = New Size(w, 32)
                                 b.Margin = margin
                                 b.Font = New Font("Segoe UI", 9)
                                 b.Cursor = Cursors.Hand
                                 Return b
                             End Function

        If Session.IsAdmin Then
            Dim btnAddNew As Button = BuildActionBtn(StyleHelper.IconAdd & " Add new", Color.FromArgb(22, 163, 74), Color.White, 90, New Padding(0, 0, 10, 0))
            Dim btnEditNew As Button = BuildActionBtn(StyleHelper.IconEdit & " Edit", StyleHelper.PrimaryColor, Color.White, 90, New Padding(0, 0, 10, 0))
            Dim btnDeleteNew As Button = BuildActionBtn(StyleHelper.IconDelete & " Delete", Color.FromArgb(220, 38, 38), Color.White, 90, New Padding(0, 0, 0, 0))
            
            flpLeft.Controls.Add(btnAddNew)
            flpLeft.Controls.Add(btnEditNew)
            flpLeft.Controls.Add(btnDeleteNew)

            AddHandler btnAddNew.Click, AddressOf btnAdd_Click
            AddHandler btnEditNew.Click, AddressOf btnEdit_Click
            AddHandler btnDeleteNew.Click, AddressOf btnDelete_Click
        ElseIf Not Session.IsGuest Then
            Dim btnBookmarkNew As Button = BuildActionBtn(StyleHelper.IconBookmark & " Bookmark", Color.FromArgb(243, 156, 18), Color.White, 120, New Padding(0))
            flpLeft.Controls.Add(btnBookmarkNew)
            AddHandler btnBookmarkNew.Click, AddressOf btnBookmark_Click
        End If

        Dim btnRefreshNew As Button = BuildActionBtn(StyleHelper.IconRefresh & " Refresh", Color.White, Color.DimGray, 90, New Padding(10, 0, 0, 0))
        Dim btnExportNew As Button = BuildActionBtn(StyleHelper.IconExport & " Export CSV", Color.FromArgb(52, 73, 94), Color.White, 100, New Padding(0))
        
        flpRight.Controls.Add(btnRefreshNew)
        flpRight.Controls.Add(btnExportNew)
        
        AddHandler btnExportNew.Click, AddressOf btnExport_Click
        AddHandler btnRefreshNew.Click, AddressOf btnRefresh_Click

        ' --- DATAGRIDVIEW ---
        dgvResources.Dock = DockStyle.Fill
        dgvResources.BackgroundColor = Color.White
        dgvResources.BorderStyle = BorderStyle.None
        dgvResources.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvResources.GridColor = Color.FromArgb(235, 235, 235)
        dgvResources.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvResources.EnableHeadersVisualStyles = False
        dgvResources.RowHeadersVisible = False
        dgvResources.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        dgvResources.ColumnHeadersDefaultCellStyle.ForeColor = Color.DimGray
        dgvResources.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 245, 245)
        dgvResources.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.DimGray
        dgvResources.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 8, FontStyle.Bold)
        dgvResources.ColumnHeadersHeight = 36
        dgvResources.RowTemplate.Height = 44
        dgvResources.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvResources.DefaultCellStyle.SelectionBackColor = StyleHelper.SelectionLightGreen
        dgvResources.DefaultCellStyle.SelectionForeColor = Color.Black

        Me.Controls.Add(dgvResources)
        dgvResources.BringToFront()

        ' Setup Columns
        If Not dgvResources.Columns.Contains("colCheck") Then
            colCheck.Name = "colCheck"
            colCheck.HeaderText = "☐"
            colCheck.Width = 36
            dgvResources.Columns.Insert(0, colCheck)
        End If

        ConfigureResourceGridColumns()

        RemoveHandler dgvResources.CellPainting, AddressOf dgvResources_CellPainting
        RemoveHandler dgvResources.SelectionChanged, AddressOf dgvResources_SelectionChanged
        AddHandler dgvResources.CellPainting, AddressOf dgvResources_CellPainting
        AddHandler dgvResources.SelectionChanged, AddressOf dgvResources_SelectionChanged
        AddHandler pnlCountBadge.Paint, AddressOf pnlCountBadge_Paint
    End Sub

    Private Sub pnlCountBadge_Paint(sender As Object, e As PaintEventArgs)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Using p As New Pen(Color.FromArgb(60, 70, 95))
            e.Graphics.DrawPath(p, StyleHelper.GetRoundedPath(New Rectangle(0, 0, pnlCountBadge.Width - 1, pnlCountBadge.Height - 1), 12))
        End Using
    End Sub

    Private Sub dgvResources_SelectionChanged(sender As Object, e As EventArgs)
        lblSelectionCount.Text = $"{dgvResources.SelectedRows.Count} of {dgvResources.Rows.Count} selected"
    End Sub

    Private Sub dgvResources_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 Then Return

        Dim pillFont As New Font("Segoe UI", 7, FontStyle.Bold)

        If e.ColumnIndex = dgvResources.Columns("colResTag").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            If e.Value.ToString().Contains("NEW") Then
                StyleHelper.DrawPill(e.Graphics, PillRect(e.CellBounds, 44), "NEW", StyleHelper.PillGreenBg, StyleHelper.PillGreenText, pillFont)
            ElseIf Not String.IsNullOrWhiteSpace(e.Value.ToString()) Then
                TextRenderer.DrawText(e.Graphics, e.Value.ToString(), e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter Or TextFormatFlags.Left)
            End If
            e.Handled = True
        End If

        If e.ColumnIndex = dgvResources.Columns("colResType").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            Dim valStr As String = e.Value.ToString()
            Dim rect As Rectangle = PillRect(e.CellBounds, 68)
            If valStr = "E-Book" Then
                StyleHelper.DrawPill(e.Graphics, rect, valStr, StyleHelper.PillBlueBg, StyleHelper.PillBlueText, pillFont)
            ElseIf valStr = "Video" Then
                StyleHelper.DrawPill(e.Graphics, rect, valStr, StyleHelper.PillPinkBg, StyleHelper.PillPinkText, pillFont)
            Else
                StyleHelper.DrawPill(e.Graphics, rect, valStr, Color.FromArgb(240, 240, 240), Color.DimGray, pillFont)
            End If
            e.Handled = True
        End If

        If e.ColumnIndex = dgvResources.Columns("colResLevel").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            Dim valStr As String = e.Value.ToString()
            If Not String.IsNullOrWhiteSpace(valStr) AndAlso valStr <> "None" Then
                StyleHelper.DrawPill(e.Graphics, PillRect(e.CellBounds, 84), valStr, StyleHelper.PillGrayBg, StyleHelper.PillGrayText, pillFont)
            End If
            e.Handled = True
        End If

        If e.ColumnIndex = dgvResources.Columns("colResViews").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            Dim val As Integer = 0
            If Integer.TryParse(e.Value.ToString(), val) Then
                Dim valText As String = val.ToString()
                Dim textSize As Size = TextRenderer.MeasureText(valText, e.CellStyle.Font)
                Dim textRect As New Rectangle(e.CellBounds.X + 6, e.CellBounds.Y, textSize.Width + 4, e.CellBounds.Height)
                TextRenderer.DrawText(e.Graphics, valText, e.CellStyle.Font, textRect, e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter Or TextFormatFlags.Left)

                Dim barLeft As Integer = textRect.Right + 4
                Dim barRight As Integer = e.CellBounds.Right - 6
                If barRight > barLeft + 8 Then
                    Dim barRect As New Rectangle(barLeft, e.CellBounds.Y + (e.CellBounds.Height \ 2) - 2, barRight - barLeft, 4)
                    StyleHelper.DrawMiniBar(e.Graphics, barRect, val, 100, StyleHelper.PillBlueText)
                End If
            End If
            e.Handled = True
        End If

        If dgvResources.Columns.Contains("colResSubject") AndAlso e.ColumnIndex = dgvResources.Columns("colResSubject").Index AndAlso e.Value IsNot Nothing Then
            If String.IsNullOrWhiteSpace(e.Value.ToString()) Then
                e.PaintBackground(e.CellBounds, True)
                TextRenderer.DrawText(e.Graphics, "—", e.CellStyle.Font, e.CellBounds, Color.Gray, TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter)
                e.Handled = True
            End If
        End If
    End Sub

End Class
