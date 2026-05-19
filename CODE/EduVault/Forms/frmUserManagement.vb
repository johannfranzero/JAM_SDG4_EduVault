''' <summary>
''' frmUserManagement - Admin User Management Form (Presentation Layer)
''' Allows Admins to view all users, add new users, edit roles/details, and deactivate accounts.
''' </summary>
Public Class frmUserManagement

    Private ReadOnly _userRepo   As New UserRepository()
    Private ReadOnly _authService As New AuthService()
    Private _allUsers As List(Of User)
    
    Private txtSearch As New TextBox()
    Private lblResultsLeft As New Label()
    Private lblSelectionCount As New Label()

    ' ─────────────────────────────────────────────────────────────
    ' FORM LOAD
    ' ─────────────────────────────────────────────────────────────

    Private Sub frmUserManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Session.IsAdmin Then
            MessageBox.Show("Access denied. Admin only.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If
        Me.Text = "EduVault - User Management (Admin)"
        SetupUI()
        LoadUsers()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' MODERN UI SETUP
    ' ─────────────────────────────────────────────────────────────

    Private Sub SetupUI()
        Me.BackColor = StyleHelper.ContentBg
        Dim isEmbedded As Boolean = Me.Parent IsNot Nothing
        
        ' Hide designer header if embedded
        If isEmbedded Then pnlHeader.Visible = False

        ' Convert existing pnlAddUser to a modern docked side panel
        pnlAddUser.Dock = DockStyle.Right
        pnlAddUser.Width = 350
        pnlAddUser.BackColor = Color.White
        pnlAddUser.Padding = New Padding(20)
        pnlAddUser.Visible = False
        StyleHelper.ApplyButtonStyle(btnSaveNewUser, isAccent:=True)
        btnCancelAdd.FlatStyle = FlatStyle.Flat
        btnCancelAdd.FlatAppearance.BorderSize = 0

        ' Hide designer toolbar
        pnlToolbar.Visible = False

        ' --- RESPONSIVE FILTER BAR (TableLayoutPanel) ---
        Dim pnlSearch As New Panel() With {.Dock = DockStyle.Top, .Height = 75, .BackColor = Color.White}
        
        Dim tlpFilters As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .Padding = New Padding(12, 8, 12, 8), .RowCount = 1, .ColumnCount = 4
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
        tlpFilters.Controls.Add(CreateFilterGroup("Search users", txtSearch), 0, 0)

        Dim btnSearchAction As New Button() With {.Size = New Size(90, 28), .Text = StyleHelper.IconSearch & " Search", .BackColor = StyleHelper.AccentBlue, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnSearchAction.FlatAppearance.BorderSize = 0
        Dim pnlBtnSearch As New Panel() With {.Size = New Size(90, 56), .Margin = New Padding(4, 0, 4, 0)}
        btnSearchAction.Location = New Point(0, 24)
        pnlBtnSearch.Controls.Add(btnSearchAction)
        tlpFilters.Controls.Add(pnlBtnSearch, 1, 0)
        AddHandler btnSearchAction.Click, Sub() ApplySearch()

        Dim btnClearAction As New Button() With {.Size = New Size(70, 28), .Text = "Clear", .BackColor = Color.White, .ForeColor = Color.DimGray, .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        btnClearAction.FlatAppearance.BorderColor = Color.LightGray
        Dim pnlBtnClear As New Panel() With {.Size = New Size(70, 56), .Margin = New Padding(4, 0, 4, 0)}
        btnClearAction.Location = New Point(0, 24)
        pnlBtnClear.Controls.Add(btnClearAction)
        tlpFilters.Controls.Add(pnlBtnClear, 2, 0)
        AddHandler btnClearAction.Click, Sub() 
                                       txtSearch.Clear()
                                       ApplySearch()
                                   End Sub

        ' --- RESULTS INFO BAR ---
        Dim pnlResultsInfo As New Panel() With {.Dock = DockStyle.Top, .Height = 36, .BackColor = StyleHelper.ContentBg}
        lblResultsLeft.Text = StyleHelper.IconPeople & " 0 user(s) found"
        lblResultsLeft.ForeColor = Color.DimGray
        lblResultsLeft.Font = New Font("Segoe UI", 8.5)
        lblResultsLeft.Location = New Point(16, 10)
        lblResultsLeft.AutoSize = True
        pnlResultsInfo.Controls.Add(lblResultsLeft)

        Dim lblResultsRight As New Label() With {
            .Text = "Sorted by date joined " & ChrW(&H2193),
            .ForeColor = Color.DimGray, .Font = New Font("Segoe UI", 8.5),
            .Location = New Point(pnlResultsInfo.Width - 160, 10),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right, .AutoSize = True
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

        Dim btnAddNew As Button = BuildActionBtn(StyleHelper.IconAdd & " Add User", Color.FromArgb(22, 163, 74), Color.White, 95, New Padding(0, 0, 10, 0))
        Dim btnEditNew As Button = BuildActionBtn(StyleHelper.IconEdit & " Edit", StyleHelper.PrimaryColor, Color.White, 80, New Padding(0, 0, 10, 0))
        Dim btnDeactivateNew As Button = BuildActionBtn("Deactivate", Color.FromArgb(220, 38, 38), Color.White, 90, New Padding(0, 0, 10, 0))
        Dim btnReactivateNew As Button = BuildActionBtn("Reactivate", StyleHelper.AccentBlue, Color.White, 90, New Padding(0, 0, 10, 0))
        
        flpLeft.Controls.Add(btnAddNew)
        flpLeft.Controls.Add(btnEditNew)
        flpLeft.Controls.Add(btnDeactivateNew)
        flpLeft.Controls.Add(btnReactivateNew)

        AddHandler btnAddNew.Click, AddressOf btnAddUser_Click
        AddHandler btnEditNew.Click, AddressOf btnEditUser_Click
        AddHandler btnDeactivateNew.Click, AddressOf btnDeactivate_Click
        AddHandler btnReactivateNew.Click, AddressOf btnReactivate_Click

        Dim btnRefreshNew As Button = BuildActionBtn(StyleHelper.IconRefresh & " Refresh", Color.White, Color.DimGray, 90, New Padding(10, 0, 0, 0))
        flpRight.Controls.Add(btnRefreshNew)
        AddHandler btnRefreshNew.Click, Sub() LoadUsers()

        ' --- DATAGRIDVIEW ---
        dgvUsers.Dock = DockStyle.Fill
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        
        StyleHelper.ApplyGridStyle(dgvUsers)
        
        If Not dgvUsers.Columns.Contains("colCheck") Then
            Dim colCheck As New DataGridViewCheckBoxColumn() With {.Name = "colCheck", .HeaderText = "☐"}
            colCheck.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            colCheck.Width = 40
            colCheck.MinimumWidth = 40
            dgvUsers.Columns.Insert(0, colCheck)
        End If
        
        colUsername.FillWeight = 20
        colFullName.FillWeight = 25
        colEmail.FillWeight = 25
        colRole.FillWeight = 10
        colStatus.FillWeight = 10
        colDateCreated.FillWeight = 10
        
        colUsername.DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        
        AddHandler dgvUsers.SelectionChanged, Sub() lblSelectionCount.Text = $"{dgvUsers.SelectedRows.Count} selected"
        
        Me.Controls.Add(dgvUsers)
        Me.Controls.Add(pnlResultsInfo)
        Me.Controls.Add(pnlSearch)
        Me.Controls.Add(pnlFooter)
        
        pnlSearch.BringToFront()
        pnlResultsInfo.BringToFront()
        pnlAddUser.BringToFront()
        dgvUsers.BringToFront()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' DATA LOADING & SEARCH
    ' ─────────────────────────────────────────────────────────────

    Private Sub LoadUsers()
        Try
            _allUsers = _userRepo.GetAllUsers()
            ApplySearch()
        Catch ex As Exception
            MessageBox.Show($"Error loading users: {ex.Message}", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ApplySearch()
        If _allUsers Is Nothing Then Return
        Dim keyword As String = txtSearch.Text.Trim().ToLower()
        Dim filtered As List(Of User) = _allUsers
        
        If Not String.IsNullOrEmpty(keyword) Then
            filtered = _allUsers.Where(Function(u) u.Username.ToLower().Contains(keyword) OrElse 
                                                   u.FullName.ToLower().Contains(keyword) OrElse 
                                                   u.Email.ToLower().Contains(keyword)).ToList()
        End If
        
        BindUsersToGrid(filtered)
    End Sub

    Private Sub BindUsersToGrid(users As List(Of User))
        dgvUsers.Rows.Clear()
        For Each u As User In users
            dgvUsers.Rows.Add(
                False,
                u.UserID,
                u.Username,
                u.FullName,
                u.Email,
                u.Role,
                If(u.IsActive, "[OK] Active", "[X] Inactive"),
                u.DateCreated.ToString("MMM dd, yyyy")
            )
        Next
        lblResultsLeft.Text = StyleHelper.IconPeople & $" {users.Count} user(s) found"
        lblSelectionCount.Text = $"{dgvUsers.SelectedRows.Count} selected"
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' BUTTON EVENTS
    ' ─────────────────────────────────────────────────────────────

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs)
        ShowAddUserPanel(True)
        ClearAddUserFields()
    End Sub

    Private Sub btnEditUser_Click(sender As Object, e As EventArgs)
        Dim selectedUser As User = GetSelectedUser()
        If selectedUser Is Nothing Then
            MessageBox.Show("Select a user to edit.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Using dlg As New frmEditUser(selectedUser)
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                LoadUsers()
            End If
        End Using
    End Sub

    Private Sub btnSaveNewUser_Click(sender As Object, e As EventArgs) Handles btnSaveNewUser.Click
        lblAddError.Text = String.Empty

        Dim errorMessage As String = String.Empty
        Dim success As Boolean = _authService.RegisterUser(
            txtNewUsername.Text,
            txtNewPassword.Text,
            txtNewConfirmPwd.Text,
            txtNewFullName.Text,
            txtNewEmail.Text,
            If(cmbNewRole.SelectedIndex > 0, cmbNewRole.SelectedItem.ToString(), String.Empty),
            errorMessage
        )

        If success Then
            MessageBox.Show("[OK] User created successfully!", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ShowAddUserPanel(False)
            LoadUsers()
        Else
            lblAddError.ForeColor = Color.FromArgb(192, 0, 0)
            lblAddError.Text = $"[X] {errorMessage}"
        End If
    End Sub

    Private Sub btnCancelAdd_Click(sender As Object, e As EventArgs) Handles btnCancelAdd.Click
        ShowAddUserPanel(False)
        ClearAddUserFields()
    End Sub

    Private Sub btnDeactivate_Click(sender As Object, e As EventArgs)
        Dim selectedUser As User = GetSelectedUser()
        If selectedUser Is Nothing Then
            MessageBox.Show("Please select a user to deactivate.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If selectedUser.UserID = Session.CurrentUser.UserID Then
            MessageBox.Show("You cannot deactivate your own account.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not selectedUser.IsActive Then
            MessageBox.Show("This user is already inactive.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim confirm As DialogResult = MessageBox.Show(
            $"Deactivate account for '{selectedUser.FullName}' ({selectedUser.Username})?{Environment.NewLine}They will no longer be able to log in.",
            "Confirm Deactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Try
                If _userRepo.DeactivateUser(selectedUser.UserID) Then
                    MessageBox.Show("[OK] User deactivated.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    LoadUsers()
                End If
            Catch ex As Exception
                MessageBox.Show($"Error: {ex.Message}", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnReactivate_Click(sender As Object, e As EventArgs)
        Dim selectedUser As User = GetSelectedUser()
        If selectedUser Is Nothing Then
            MessageBox.Show("Please select a user to reactivate.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If selectedUser.IsActive Then
            MessageBox.Show("This user is already active.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim confirm As DialogResult = MessageBox.Show(
            $"Reactivate account for '{selectedUser.FullName}' ({selectedUser.Username})?{Environment.NewLine}They will be able to log in again.",
            "Confirm Reactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            Try
                If _userRepo.ReactivateUser(selectedUser.UserID) Then
                    MessageBox.Show("[OK] User reactivated.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    LoadUsers()
                End If
            Catch ex As Exception
                MessageBox.Show($"Error: {ex.Message}", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' HELPERS
    ' ─────────────────────────────────────────────────────────────

    Private Function GetSelectedUser() As User
        If dgvUsers.SelectedRows.Count = 0 Then Return Nothing
        Dim userID As Integer = CInt(dgvUsers.SelectedRows(0).Cells("colUserID").Value)
        Return _allUsers.FirstOrDefault(Function(u) u.UserID = userID)
    End Function

    Private Sub ShowAddUserPanel(show As Boolean)
        pnlAddUser.Visible = show
    End Sub

    Private Sub ClearAddUserFields()
        txtNewUsername.Clear()
        txtNewFullName.Clear()
        txtNewEmail.Clear()
        txtNewPassword.Clear()
        txtNewConfirmPwd.Clear()
        cmbNewRole.SelectedIndex = 1  ' Default to Student
        lblAddError.Text = String.Empty
    End Sub

End Class
