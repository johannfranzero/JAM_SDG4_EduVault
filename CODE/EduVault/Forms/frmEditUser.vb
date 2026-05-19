Imports System.Windows.Forms

Public Class frmEditUser
    Inherits Form

    Private ReadOnly _userService As New UserService()
    Private ReadOnly _editingUser As User

    Private txtFullName As TextBox
    Private txtEmail As TextBox
    Private cmbRole As ComboBox
    Private chkActive As CheckBox
    Private lblUsername As Label
    Private lblError As Label

    Public Sub New(user As User)
        _editingUser = user
        InitializeDialog()
    End Sub

    Private Sub InitializeDialog()
        Me.Text = "Edit User"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(420, 320)
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Dim y As Integer = 16
        lblUsername = New Label With {.Location = New Point(16, y), .AutoSize = True,
            .Text = $"Username: {_editingUser.Username}"}
        Me.Controls.Add(lblUsername)
        y += 28

        Me.Controls.Add(New Label With {.Text = "Full name", .Location = New Point(16, y), .AutoSize = True})
        y += 20
        txtFullName = New TextBox With {.Location = New Point(16, y), .Size = New Size(370, 26), .Text = _editingUser.FullName}
        Me.Controls.Add(txtFullName)
        y += 36

        Me.Controls.Add(New Label With {.Text = "Email", .Location = New Point(16, y), .AutoSize = True})
        y += 20
        txtEmail = New TextBox With {.Location = New Point(16, y), .Size = New Size(370, 26), .Text = If(_editingUser.Email, String.Empty)}
        Me.Controls.Add(txtEmail)
        y += 36

        Me.Controls.Add(New Label With {.Text = "Role", .Location = New Point(16, y), .AutoSize = True})
        y += 20
        cmbRole = New ComboBox With {.Location = New Point(16, y), .Size = New Size(180, 26), .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbRole.Items.AddRange(New Object() {"Admin", "Student"})
        cmbRole.SelectedItem = _editingUser.Role
        Me.Controls.Add(cmbRole)
        y += 36

        chkActive = New CheckBox With {.Text = "Account active", .Location = New Point(16, y), .Checked = _editingUser.IsActive}
        Me.Controls.Add(chkActive)
        y += 32

        lblError = New Label With {.Location = New Point(16, y), .Size = New Size(370, 40), .ForeColor = Color.DarkRed, .Visible = False}
        Me.Controls.Add(lblError)
        y += 44

        Dim btnSave As New Button With {.Text = "Save", .Location = New Point(206, y), .Size = New Size(85, 32)}
        StyleHelper.ApplyButtonStyle(btnSave, isAccent:=True)
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        Me.Controls.Add(btnSave)

        Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(301, y), .Size = New Size(85, 32)}
        StyleHelper.ApplyButtonStyle(btnCancel)
        AddHandler btnCancel.Click,
            Sub()
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End Sub
        Me.Controls.Add(btnCancel)
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        _editingUser.FullName = txtFullName.Text.Trim()
        _editingUser.Email = txtEmail.Text.Trim()
        _editingUser.Role = cmbRole.SelectedItem.ToString()
        _editingUser.IsActive = chkActive.Checked

        Dim err As String = String.Empty
        If _userService.UpdateUserProfile(_editingUser, err) Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            lblError.Visible = True
            lblError.Text = err
        End If
    End Sub

End Class
