''' <summary>
''' frmChangePassword - Allows any logged-in user to change their password.
''' Uses AuthService.ChangePassword which validates current password and strength rules.
''' </summary>
Public Class frmChangePassword

    Private ReadOnly _authService As New AuthService()

    Private Sub frmChangePassword_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Session.IsLoggedIn Then
            Me.Close()
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        lblStatus.Text = String.Empty

        If String.IsNullOrWhiteSpace(txtCurrent.Text) Then
            ShowError("Please enter your current password.")
            Return
        End If

        Dim errorMessage As String = String.Empty
        Dim success As Boolean = _authService.ChangePassword(
            Session.CurrentUser.UserID,
            txtCurrent.Text,
            txtNew.Text,
            txtConfirm.Text,
            errorMessage
        )

        If success Then
            MessageBox.Show("Password changed successfully!", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            ShowError(errorMessage)
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ShowError(msg As String)
        lblStatus.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0)
        lblStatus.Text = msg
    End Sub

End Class
