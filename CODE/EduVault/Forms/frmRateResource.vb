Imports System.Windows.Forms

Public Class frmRateResource
    Inherits Form

    Private numStars As NumericUpDown
    Private txtReview As TextBox

    Public Property SelectedStars As Integer = 5
    Public Property ReviewText As String = String.Empty

    Public Sub New(resourceTitle As String)
        Me.Text = "Rate resource"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(380, 260)

        Me.Controls.Add(New Label With {
            .Text = resourceTitle,
            .Location = New Point(16, 16),
            .Size = New Size(340, 40)
        })
        Me.Controls.Add(New Label With {.Text = "Stars (1-5)", .Location = New Point(16, 60), .AutoSize = True})
        numStars = New NumericUpDown With {.Location = New Point(16, 82), .Minimum = 1, .Maximum = 5, .Value = 5}
        Me.Controls.Add(numStars)
        Me.Controls.Add(New Label With {.Text = "Review (optional)", .Location = New Point(16, 110), .AutoSize = True})
        txtReview = New TextBox With {.Location = New Point(16, 132), .Size = New Size(330, 60), .Multiline = True}
        Me.Controls.Add(txtReview)

        Dim btnOk As New Button With {.Text = "Submit", .Location = New Point(180, 200), .Size = New Size(80, 30)}
        StyleHelper.ApplyButtonStyle(btnOk, isAccent:=True)
        AddHandler btnOk.Click,
            Sub()
                SelectedStars = CInt(numStars.Value)
                ReviewText = txtReview.Text.Trim()
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End Sub
        Me.Controls.Add(btnOk)

        Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(270, 200), .Size = New Size(80, 30)}
        StyleHelper.ApplyButtonStyle(btnCancel)
        AddHandler btnCancel.Click,
            Sub()
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End Sub
        Me.Controls.Add(btnCancel)
    End Sub

End Class
