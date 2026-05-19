Imports System.Windows.Forms
Imports System.Drawing

Public Class frmRegister
    Private ReadOnly _authService As New AuthService()

    Public Sub New()
        InitializeComponent()
        ApplyStyles()
    End Sub

    Private Sub ApplyStyles()
        Me.BackColor = Color.White
        Me.Text = "Create Account"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(400, 580)

        lblTitle.Font = StyleHelper.HeaderFont
        lblTitle.ForeColor = StyleHelper.PrimaryColor
        lblTitle.Text = "Join EduVault"
        
        StyleHelper.ApplyButtonStyle(btnRegister, isAccent:=True)
        
        StyleHelper.ApplyModernInputStyle(txtFullName)
        StyleHelper.ApplyModernInputStyle(txtUsername)
        StyleHelper.ApplyModernInputStyle(txtEmail)
        StyleHelper.ApplyModernInputStyle(txtPassword)
        StyleHelper.ApplyModernInputStyle(txtConfirmPassword)

        StyleHelper.SetPlaceholder(txtFullName, "Your full name")
        StyleHelper.SetPlaceholder(txtUsername, "Choose a username")
        StyleHelper.SetPlaceholder(txtEmail, "Email address")
        StyleHelper.SetPlaceholder(txtPassword, "Password (8+ chars, with symbol)")
        StyleHelper.SetPlaceholder(txtConfirmPassword, "Confirm password")
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Dim errorMsg As String = ""
        ' Default new users to 'Student' role
        If _authService.RegisterUser(txtUsername.Text, txtPassword.Text, txtConfirmPassword.Text, 
                                     txtFullName.Text, txtEmail.Text, "Student", errorMsg) Then
            MessageBox.Show("Registration successful! You can now log in.", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
        Else
            MessageBox.Show(errorMsg, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

End Class

Partial Class frmRegister
    Inherits Form

    Friend WithEvents lblTitle As Label
    Friend WithEvents txtFullName As TextBox
    Friend WithEvents txtUsername As TextBox
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents txtConfirmPassword As TextBox
    Friend WithEvents btnRegister As Button

    Private Sub InitializeComponent()
        Me.lblTitle = New Label()
        Me.txtFullName = New TextBox()
        Me.txtUsername = New TextBox()
        Me.txtEmail = New TextBox()
        Me.txtPassword = New TextBox()
        Me.txtConfirmPassword = New TextBox()
        Me.btnRegister = New Button()

        Me.lblTitle.Location = New Point(30, 25)
        Me.lblTitle.Size = New Size(340, 40)
        Me.lblTitle.TextAlign = ContentAlignment.MiddleCenter

        ' Fields
        Me.txtFullName.Location = New Point(30, 90)
        Me.txtFullName.Size = New Size(340, 35)

        Me.txtUsername.Location = New Point(30, 160)
        Me.txtUsername.Size = New Size(340, 35)

        Me.txtEmail.Location = New Point(30, 230)
        Me.txtEmail.Size = New Size(340, 35)

        Me.txtPassword.Location = New Point(30, 300)
        Me.txtPassword.Size = New Size(340, 35)
        Me.txtPassword.UseSystemPasswordChar = True

        Me.txtConfirmPassword.Location = New Point(30, 370)
        Me.txtConfirmPassword.Size = New Size(340, 35)
        Me.txtConfirmPassword.UseSystemPasswordChar = True

        ' Button
        Me.btnRegister.Location = New Point(30, 460)
        Me.btnRegister.Size = New Size(340, 50)
        Me.btnRegister.Text = "Create Account"

        Me.Controls.AddRange({lblTitle, txtFullName, txtUsername, txtEmail, txtPassword, txtConfirmPassword, btnRegister})
    End Sub
End Class
