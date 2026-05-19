Imports System.Windows.Forms
Imports System.Drawing

Public Class frmForgotPassword
    Private ReadOnly _authService As New AuthService()
    Private _step As Integer = 1 ' 1: Username, 2: Token & Password

    Public Sub New()
        InitializeComponent()
        ApplyStyles()
    End Sub

    Private Sub ApplyStyles()
        Me.BackColor = Color.White
        Me.Text = "Reset Password"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(350, 450)

        lblTitle.Font = StyleHelper.HeaderFont
        lblTitle.ForeColor = StyleHelper.PrimaryColor
        
        StyleHelper.ApplyButtonStyle(btnAction)
        StyleHelper.ApplyModernInputStyle(txtUsername)
        StyleHelper.ApplyModernInputStyle(txtToken)
        StyleHelper.ApplyModernInputStyle(txtNewPassword)
        StyleHelper.ApplyModernInputStyle(txtConfirmPassword)
        
        StyleHelper.SetPlaceholder(txtUsername, "Enter your username")
        StyleHelper.SetPlaceholder(txtToken, "Reset code")
        StyleHelper.SetPlaceholder(txtNewPassword, "New Password")
        StyleHelper.SetPlaceholder(txtConfirmPassword, "Confirm New Password")
        
        ShowStep(1)
    End Sub

    Private Sub ShowStep(stepNumber As Integer)
        _step = stepNumber
        If _step = 1 Then
            lblTitle.Text = "Find Account"
            lblInstruction.Text = "Enter your username to receive a reset code."
            pnlStep1.Visible = True
            pnlStep2.Visible = False
            btnAction.Text = "Send Code"
        Else
            lblTitle.Text = "Reset Password"
            lblInstruction.Text = "Enter the reset code and your new password."
            pnlStep1.Visible = False
            pnlStep2.Visible = True
            btnAction.Text = "Reset Password"
        End If
    End Sub

    Private Sub btnAction_Click(sender As Object, e As EventArgs) Handles btnAction.Click
        Dim errorMsg As String = ""
        
        If _step = 1 Then
            Dim username = txtUsername.Text.Trim()
            If String.IsNullOrEmpty(username) Then
                MessageBox.Show("Please enter your username.", "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim token = _authService.GenerateResetToken(username, errorMsg)
            If token IsNot Nothing Then
                MessageBox.Show($"[MOCK EMAIL] Your reset code is:{Environment.NewLine}{Environment.NewLine}{token}",
                                "Security Code", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show(errorMsg, "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            ShowStep(2)
        Else
            If txtNewPassword.Text <> txtConfirmPassword.Text Then
                MessageBox.Show("Passwords do not match.", "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtConfirmPassword.Focus()
                Return
            End If

            If _authService.ResetPasswordWithToken(
                    txtUsername.Text.Trim(),
                    txtToken.Text.Trim(),
                    txtNewPassword.Text,
                    txtConfirmPassword.Text,
                    errorMsg) Then
                MessageBox.Show("Password reset successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Close()
            Else
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

End Class

Partial Class frmForgotPassword
    Inherits Form

    Friend WithEvents lblTitle As Label
    Friend WithEvents lblInstruction As Label
    Friend WithEvents pnlStep1 As Panel
    Friend WithEvents txtUsername As TextBox
    Friend WithEvents pnlStep2 As Panel
    Friend WithEvents txtToken As TextBox
    Friend WithEvents txtNewPassword As TextBox
    Friend WithEvents txtConfirmPassword As TextBox
    Friend WithEvents btnAction As Button

    Private Sub InitializeComponent()
        Me.lblTitle = New Label()
        Me.lblInstruction = New Label()
        Me.pnlStep1 = New Panel()
        Me.txtUsername = New TextBox()
        Me.pnlStep2 = New Panel()
        Me.txtToken = New TextBox()
        Me.txtNewPassword = New TextBox()
        Me.txtConfirmPassword = New TextBox()
        Me.btnAction = New Button()

        ' Title
        Me.lblTitle.Location = New Point(20, 20)
        Me.lblTitle.Size = New Size(300, 30)
        Me.lblTitle.TextAlign = ContentAlignment.MiddleCenter

        ' Instruction
        Me.lblInstruction.Location = New Point(20, 60)
        Me.lblInstruction.Size = New Size(300, 50)
        Me.lblInstruction.TextAlign = ContentAlignment.MiddleCenter
        Me.lblInstruction.Font = New Font("Segoe UI", 9)
        Me.lblInstruction.ForeColor = Color.DimGray

        ' Step 1 Panel
        Me.pnlStep1.Location = New Point(20, 120)
        Me.pnlStep1.Size = New Size(300, 60)
        Me.txtUsername.Location = New Point(0, 10)
        Me.txtUsername.Size = New Size(300, 30)
        Me.pnlStep1.Controls.Add(Me.txtUsername)

        ' Step 2 Panel
        Me.pnlStep2.Location = New Point(20, 120)
        Me.pnlStep2.Size = New Size(300, 200)
        Me.txtToken.Location = New Point(0, 10)
        Me.txtToken.Size = New Size(300, 30)
        Me.txtNewPassword.Location = New Point(0, 60)
        Me.txtNewPassword.Size = New Size(300, 30)
        Me.txtNewPassword.UseSystemPasswordChar = True
        Me.txtConfirmPassword.Location = New Point(0, 110)
        Me.txtConfirmPassword.Size = New Size(300, 30)
        Me.txtConfirmPassword.UseSystemPasswordChar = True
        Me.pnlStep2.Controls.Add(Me.txtToken)
        Me.pnlStep2.Controls.Add(Me.txtNewPassword)
        Me.pnlStep2.Controls.Add(Me.txtConfirmPassword)

        ' Action Button
        Me.btnAction.Location = New Point(20, 340)
        Me.btnAction.Size = New Size(300, 45)

        Me.Controls.AddRange({lblTitle, lblInstruction, pnlStep1, pnlStep2, btnAction})
    End Sub
End Class
