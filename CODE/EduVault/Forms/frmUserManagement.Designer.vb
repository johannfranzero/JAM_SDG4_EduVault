<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmUserManagement
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlHeader       = New System.Windows.Forms.Panel()
        Me.lblHeader       = New System.Windows.Forms.Label()
        Me.pnlToolbar      = New System.Windows.Forms.Panel()
        Me.btnAddUser      = New System.Windows.Forms.Button()
        Me.btnEditUser     = New System.Windows.Forms.Button()
        Me.btnDeactivate   = New System.Windows.Forms.Button()
        Me.btnReactivate   = New System.Windows.Forms.Button()
        Me.btnRefreshUsers = New System.Windows.Forms.Button()
        Me.btnClose        = New System.Windows.Forms.Button()
        Me.lblUserCount    = New System.Windows.Forms.Label()
        Me.dgvUsers        = New System.Windows.Forms.DataGridView()
        Me.colUserID       = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUsername     = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colFullName     = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colEmail        = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colRole         = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colStatus       = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDateCreated  = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlAddUser      = New System.Windows.Forms.Panel()
        Me.lblAddUserTitle = New System.Windows.Forms.Label()
        Me.lblNewUsername  = New System.Windows.Forms.Label()
        Me.txtNewUsername  = New System.Windows.Forms.TextBox()
        Me.lblNewFullName  = New System.Windows.Forms.Label()
        Me.txtNewFullName  = New System.Windows.Forms.TextBox()
        Me.lblNewEmail     = New System.Windows.Forms.Label()
        Me.txtNewEmail     = New System.Windows.Forms.TextBox()
        Me.lblNewRole      = New System.Windows.Forms.Label()
        Me.cmbNewRole      = New System.Windows.Forms.ComboBox()
        Me.lblNewPwd       = New System.Windows.Forms.Label()
        Me.txtNewPassword  = New System.Windows.Forms.TextBox()
        Me.lblNewConfirm   = New System.Windows.Forms.Label()
        Me.txtNewConfirmPwd = New System.Windows.Forms.TextBox()
        Me.btnSaveNewUser  = New System.Windows.Forms.Button()
        Me.btnCancelAdd    = New System.Windows.Forms.Button()
        Me.lblAddError     = New System.Windows.Forms.Label()
        Me.pnlHeader.SuspendLayout()
        Me.pnlToolbar.SuspendLayout()
        CType(Me.dgvUsers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlAddUser.SuspendLayout()
        Me.SuspendLayout()

        ' ── pnlHeader ──
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.pnlHeader.Controls.Add(Me.lblHeader)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top : Me.pnlHeader.Height = 52 : Me.pnlHeader.Name = "pnlHeader"
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
        Me.lblHeader.ForeColor = System.Drawing.Color.White
        Me.lblHeader.Location = New System.Drawing.Point(12, 12) : Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Text = "User Management"

        ' ── pnlToolbar ──
        Me.pnlToolbar.BackColor = System.Drawing.Color.White
        Me.pnlToolbar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlToolbar.Controls.AddRange(New System.Windows.Forms.Control() {
            Me.btnAddUser, Me.btnEditUser, Me.btnDeactivate, Me.btnReactivate, Me.btnRefreshUsers, Me.btnClose, Me.lblUserCount})
        Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top : Me.pnlToolbar.Height = 50 : Me.pnlToolbar.Name = "pnlToolbar"

        Dim btnFont As New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)

        Me.btnAddUser.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnAddUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnAddUser.FlatAppearance.BorderSize = 0
        Me.btnAddUser.Font = btnFont : Me.btnAddUser.ForeColor = System.Drawing.Color.White
        Me.btnAddUser.Location = New System.Drawing.Point(8, 10) : Me.btnAddUser.Size = New System.Drawing.Size(120, 30)
        Me.btnAddUser.Name = "btnAddUser" : Me.btnAddUser.Text = "Add User" : Me.btnAddUser.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnEditUser.BackColor = System.Drawing.Color.FromArgb(41, 128, 185)
        Me.btnEditUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnEditUser.FlatAppearance.BorderSize = 0
        Me.btnEditUser.Font = btnFont : Me.btnEditUser.ForeColor = System.Drawing.Color.White
        Me.btnEditUser.Location = New System.Drawing.Point(136, 10) : Me.btnEditUser.Size = New System.Drawing.Size(90, 30)
        Me.btnEditUser.Name = "btnEditUser" : Me.btnEditUser.Text = "Edit User" : Me.btnEditUser.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnDeactivate.BackColor = System.Drawing.Color.FromArgb(192, 57, 43)
        Me.btnDeactivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnDeactivate.FlatAppearance.BorderSize = 0
        Me.btnDeactivate.Font = btnFont : Me.btnDeactivate.ForeColor = System.Drawing.Color.White
        Me.btnDeactivate.Location = New System.Drawing.Point(234, 10) : Me.btnDeactivate.Size = New System.Drawing.Size(110, 30)
        Me.btnDeactivate.Name = "btnDeactivate" : Me.btnDeactivate.Text = "Deactivate" : Me.btnDeactivate.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnReactivate.BackColor = System.Drawing.Color.FromArgb(41, 128, 185)
        Me.btnReactivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnReactivate.FlatAppearance.BorderSize = 0
        Me.btnReactivate.Font = btnFont : Me.btnReactivate.ForeColor = System.Drawing.Color.White
        Me.btnReactivate.Location = New System.Drawing.Point(352, 10) : Me.btnReactivate.Size = New System.Drawing.Size(100, 30)
        Me.btnReactivate.Name = "btnReactivate" : Me.btnReactivate.Text = "Reactivate" : Me.btnReactivate.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnRefreshUsers.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnRefreshUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnRefreshUsers.FlatAppearance.BorderSize = 0
        Me.btnRefreshUsers.Font = btnFont : Me.btnRefreshUsers.ForeColor = System.Drawing.Color.White
        Me.btnRefreshUsers.Location = New System.Drawing.Point(460, 10) : Me.btnRefreshUsers.Size = New System.Drawing.Size(90, 30)
        Me.btnRefreshUsers.Name = "btnRefreshUsers" : Me.btnRefreshUsers.Text = "Refresh" : Me.btnRefreshUsers.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnClose.BackColor = System.Drawing.Color.FromArgb(52, 73, 94)
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.Font = btnFont : Me.btnClose.ForeColor = System.Drawing.Color.White
        Me.btnClose.Location = New System.Drawing.Point(558, 10) : Me.btnClose.Size = New System.Drawing.Size(80, 30)
        Me.btnClose.Name = "btnClose" : Me.btnClose.Text = "Close" : Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand

        Me.lblUserCount.AutoSize = True : Me.lblUserCount.Font = New System.Drawing.Font("Segoe UI", 8)
        Me.lblUserCount.ForeColor = System.Drawing.Color.DimGray
        Me.lblUserCount.Location = New System.Drawing.Point(618, 18) : Me.lblUserCount.Name = "lblUserCount" : Me.lblUserCount.Text = "0 users"

        ' ── dgvUsers ──
        Me.dgvUsers.AllowUserToAddRows = False : Me.dgvUsers.AllowUserToDeleteRows = False
        Me.dgvUsers.BackgroundColor = System.Drawing.Color.White
        Me.dgvUsers.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(25, 55, 109)
        Me.dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White
        Me.dgvUsers.ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Me.dgvUsers.ColumnHeadersHeight = 30
        Me.dgvUsers.DefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 248, 255)
        Me.dgvUsers.Location = New System.Drawing.Point(0, 102)
        Me.dgvUsers.Name = "dgvUsers" : Me.dgvUsers.ReadOnly = True : Me.dgvUsers.RowHeadersVisible = False
        Me.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvUsers.MultiSelect = False
        Me.dgvUsers.Size = New System.Drawing.Size(960, 468)
        Me.dgvUsers.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right

        Me.colUserID.Name = "colUserID"         : Me.colUserID.HeaderText = "ID"           : Me.colUserID.Width = 40    : Me.colUserID.Visible = False
        Me.colUsername.Name = "colUsername"       : Me.colUsername.HeaderText = "Username"   : Me.colUsername.Width = 120
        Me.colFullName.Name = "colFullName"       : Me.colFullName.HeaderText = "Full Name"  : Me.colFullName.Width = 160
        Me.colEmail.Name = "colEmail"             : Me.colEmail.HeaderText = "Email"         : Me.colEmail.Width = 160
        Me.colRole.Name = "colRole"               : Me.colRole.HeaderText = "Role"           : Me.colRole.Width = 80
        Me.colStatus.Name = "colStatus"           : Me.colStatus.HeaderText = "Status"       : Me.colStatus.Width = 90
        Me.colDateCreated.Name = "colDateCreated" : Me.colDateCreated.HeaderText = "Joined"  : Me.colDateCreated.Width = 110
        Me.dgvUsers.Columns.AddRange(Me.colUserID, Me.colUsername, Me.colFullName,
                                     Me.colEmail, Me.colRole, Me.colStatus, Me.colDateCreated)

        ' ── pnlAddUser (inline Add User panel, hidden by default) ──
        Me.pnlAddUser.BackColor = System.Drawing.Color.White
        Me.pnlAddUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlAddUser.Controls.AddRange(New System.Windows.Forms.Control() {
            Me.lblAddUserTitle,
            Me.lblNewUsername, Me.txtNewUsername, Me.lblNewFullName, Me.txtNewFullName,
            Me.lblNewEmail, Me.txtNewEmail, Me.lblNewRole, Me.cmbNewRole,
            Me.lblNewPwd, Me.txtNewPassword, Me.lblNewConfirm, Me.txtNewConfirmPwd,
            Me.btnSaveNewUser, Me.btnCancelAdd, Me.lblAddError})
        Me.pnlAddUser.Location = New System.Drawing.Point(610, 102)
        Me.pnlAddUser.Name = "pnlAddUser" : Me.pnlAddUser.Size = New System.Drawing.Size(340, 468)
        Me.pnlAddUser.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right Or System.Windows.Forms.AnchorStyles.Bottom
        Me.pnlAddUser.Visible = False

        Me.lblAddUserTitle.Text = "New User Account" : Me.lblAddUserTitle.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.lblAddUserTitle.ForeColor = System.Drawing.Color.FromArgb(25, 55, 109) : Me.lblAddUserTitle.AutoSize = True
        Me.lblAddUserTitle.Location = New System.Drawing.Point(10, 10) : Me.lblAddUserTitle.Name = "lblAddUserTitle"

        Dim lf As New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Dim tf As New System.Drawing.Font("Segoe UI", 9)
        Dim lc As System.Drawing.Color = System.Drawing.Color.FromArgb(50, 50, 50)

        Me.lblNewUsername.Text = "Username *" : Me.lblNewUsername.Font = lf : Me.lblNewUsername.ForeColor = lc
        Me.lblNewUsername.AutoSize = True : Me.lblNewUsername.Location = New System.Drawing.Point(10, 40) : Me.lblNewUsername.Name = "lblNewUsername"
        Me.txtNewUsername.Font = tf : Me.txtNewUsername.Location = New System.Drawing.Point(10, 58) : Me.txtNewUsername.Size = New System.Drawing.Size(316, 26) : Me.txtNewUsername.MaxLength = 50 : Me.txtNewUsername.Name = "txtNewUsername"

        Me.lblNewFullName.Text = "Full Name *" : Me.lblNewFullName.Font = lf : Me.lblNewFullName.ForeColor = lc
        Me.lblNewFullName.AutoSize = True : Me.lblNewFullName.Location = New System.Drawing.Point(10, 92) : Me.lblNewFullName.Name = "lblNewFullName"
        Me.txtNewFullName.Font = tf : Me.txtNewFullName.Location = New System.Drawing.Point(10, 110) : Me.txtNewFullName.Size = New System.Drawing.Size(316, 26) : Me.txtNewFullName.MaxLength = 100 : Me.txtNewFullName.Name = "txtNewFullName"

        Me.lblNewEmail.Text = "Email" : Me.lblNewEmail.Font = lf : Me.lblNewEmail.ForeColor = lc
        Me.lblNewEmail.AutoSize = True : Me.lblNewEmail.Location = New System.Drawing.Point(10, 144) : Me.lblNewEmail.Name = "lblNewEmail"
        Me.txtNewEmail.Font = tf : Me.txtNewEmail.Location = New System.Drawing.Point(10, 162) : Me.txtNewEmail.Size = New System.Drawing.Size(316, 26) : Me.txtNewEmail.MaxLength = 100 : Me.txtNewEmail.Name = "txtNewEmail"

        Me.lblNewRole.Text = "Role *" : Me.lblNewRole.Font = lf : Me.lblNewRole.ForeColor = lc
        Me.lblNewRole.AutoSize = True : Me.lblNewRole.Location = New System.Drawing.Point(10, 196) : Me.lblNewRole.Name = "lblNewRole"
        Me.cmbNewRole.Font = tf : Me.cmbNewRole.Location = New System.Drawing.Point(10, 214) : Me.cmbNewRole.Size = New System.Drawing.Size(316, 26)
        Me.cmbNewRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNewRole.Items.AddRange({"- Select Role -", "Student", "Admin"}) : Me.cmbNewRole.SelectedIndex = 1
        Me.cmbNewRole.Name = "cmbNewRole"

        Me.lblNewPwd.Text = "Password *" : Me.lblNewPwd.Font = lf : Me.lblNewPwd.ForeColor = lc
        Me.lblNewPwd.AutoSize = True : Me.lblNewPwd.Location = New System.Drawing.Point(10, 250) : Me.lblNewPwd.Name = "lblNewPwd"
        Me.txtNewPassword.Font = tf : Me.txtNewPassword.Location = New System.Drawing.Point(10, 268) : Me.txtNewPassword.Size = New System.Drawing.Size(316, 26)
        Me.txtNewPassword.UseSystemPasswordChar = True : Me.txtNewPassword.MaxLength = 100 : Me.txtNewPassword.Name = "txtNewPassword"

        Me.lblNewConfirm.Text = "Confirm Password *" : Me.lblNewConfirm.Font = lf : Me.lblNewConfirm.ForeColor = lc
        Me.lblNewConfirm.AutoSize = True : Me.lblNewConfirm.Location = New System.Drawing.Point(10, 302) : Me.lblNewConfirm.Name = "lblNewConfirm"
        Me.txtNewConfirmPwd.Font = tf : Me.txtNewConfirmPwd.Location = New System.Drawing.Point(10, 320) : Me.txtNewConfirmPwd.Size = New System.Drawing.Size(316, 26)
        Me.txtNewConfirmPwd.UseSystemPasswordChar = True : Me.txtNewConfirmPwd.MaxLength = 100 : Me.txtNewConfirmPwd.Name = "txtNewConfirmPwd"

        Me.btnSaveNewUser.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnSaveNewUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnSaveNewUser.FlatAppearance.BorderSize = 0
        Me.btnSaveNewUser.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) : Me.btnSaveNewUser.ForeColor = System.Drawing.Color.White
        Me.btnSaveNewUser.Location = New System.Drawing.Point(10, 360) : Me.btnSaveNewUser.Size = New System.Drawing.Size(150, 32)
        Me.btnSaveNewUser.Name = "btnSaveNewUser" : Me.btnSaveNewUser.Text = "Create Account" : Me.btnSaveNewUser.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnCancelAdd.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnCancelAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnCancelAdd.FlatAppearance.BorderSize = 0
        Me.btnCancelAdd.Font = New System.Drawing.Font("Segoe UI", 9) : Me.btnCancelAdd.ForeColor = System.Drawing.Color.White
        Me.btnCancelAdd.Location = New System.Drawing.Point(170, 360) : Me.btnCancelAdd.Size = New System.Drawing.Size(80, 32)
        Me.btnCancelAdd.Name = "btnCancelAdd" : Me.btnCancelAdd.Text = "Cancel" : Me.btnCancelAdd.Cursor = System.Windows.Forms.Cursors.Hand

        Me.lblAddError.AutoSize = True : Me.lblAddError.Font = New System.Drawing.Font("Segoe UI", 8)
        Me.lblAddError.ForeColor = System.Drawing.Color.Red : Me.lblAddError.MaximumSize = New System.Drawing.Size(316, 0)
        Me.lblAddError.Location = New System.Drawing.Point(10, 400) : Me.lblAddError.Name = "lblAddError" : Me.lblAddError.Text = String.Empty

        ' ── frmUserManagement ──
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.FromArgb(240, 244, 250)
        Me.ClientSize = New System.Drawing.Size(960, 620)
        Me.MinimumSize = New System.Drawing.Size(800, 500)
        Me.Controls.Add(Me.dgvUsers) : Me.Controls.Add(Me.pnlAddUser)
        Me.Controls.Add(Me.pnlToolbar) : Me.Controls.Add(Me.pnlHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MaximizeBox = True : Me.Name = "frmUserManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "EduVault - User Management"

        Me.pnlHeader.ResumeLayout(False) : Me.pnlHeader.PerformLayout()
        Me.pnlToolbar.ResumeLayout(False) : Me.pnlToolbar.PerformLayout()
        CType(Me.dgvUsers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlAddUser.ResumeLayout(False) : Me.pnlAddUser.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlHeader        As System.Windows.Forms.Panel
    Friend WithEvents lblHeader        As System.Windows.Forms.Label
    Friend WithEvents pnlToolbar       As System.Windows.Forms.Panel
    Friend WithEvents btnAddUser       As System.Windows.Forms.Button
    Friend WithEvents btnEditUser      As System.Windows.Forms.Button
    Friend WithEvents btnDeactivate    As System.Windows.Forms.Button
    Friend WithEvents btnReactivate    As System.Windows.Forms.Button
    Friend WithEvents btnRefreshUsers  As System.Windows.Forms.Button
    Friend WithEvents btnClose         As System.Windows.Forms.Button
    Friend WithEvents lblUserCount     As System.Windows.Forms.Label
    Friend WithEvents dgvUsers         As System.Windows.Forms.DataGridView
    Friend WithEvents colUserID        As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colUsername      As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colFullName      As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colEmail         As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colRole          As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colStatus        As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colDateCreated   As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents pnlAddUser       As System.Windows.Forms.Panel
    Friend WithEvents lblAddUserTitle  As System.Windows.Forms.Label
    Friend WithEvents lblNewUsername   As System.Windows.Forms.Label
    Friend WithEvents txtNewUsername   As System.Windows.Forms.TextBox
    Friend WithEvents lblNewFullName   As System.Windows.Forms.Label
    Friend WithEvents txtNewFullName   As System.Windows.Forms.TextBox
    Friend WithEvents lblNewEmail      As System.Windows.Forms.Label
    Friend WithEvents txtNewEmail      As System.Windows.Forms.TextBox
    Friend WithEvents lblNewRole       As System.Windows.Forms.Label
    Friend WithEvents cmbNewRole       As System.Windows.Forms.ComboBox
    Friend WithEvents lblNewPwd        As System.Windows.Forms.Label
    Friend WithEvents txtNewPassword   As System.Windows.Forms.TextBox
    Friend WithEvents lblNewConfirm    As System.Windows.Forms.Label
    Friend WithEvents txtNewConfirmPwd As System.Windows.Forms.TextBox
    Friend WithEvents btnSaveNewUser   As System.Windows.Forms.Button
    Friend WithEvents btnCancelAdd     As System.Windows.Forms.Button
    Friend WithEvents lblAddError      As System.Windows.Forms.Label

End Class
