Imports System.Windows.Forms

Public Class frmNotifications
    Inherits Form

    Private ReadOnly _v2 As New V2FeatureRepository()
    Private dgv As DataGridView

    Public Sub New()
        Me.Text = "Notifications"
        Me.Size = New Size(560, 400)
        dgv = New DataGridView With {.Dock = DockStyle.Fill}
        StyleHelper.ApplyGridStyle(dgv)
        dgv.ReadOnly = True
        dgv.Columns.Add("colID", "ID")
        dgv.Columns.Add("colMsg", "Message")
        dgv.Columns.Add("colDate", "Date")
        dgv.Columns.Add("colRead", "Read")
        dgv.Columns("colID").Visible = False
        dgv.Columns("colMsg").Width = 320
        AddHandler dgv.CellDoubleClick, AddressOf MarkRead

        Dim btnMark As New Button With {.Text = "Mark selected read", .Dock = DockStyle.Bottom, .Height = 36}
        StyleHelper.ApplyButtonStyle(btnMark)
        AddHandler btnMark.Click, AddressOf MarkRead

        Me.Controls.Add(dgv)
        Me.Controls.Add(btnMark)
        LoadData()
    End Sub

    Private Sub LoadData()
        dgv.Rows.Clear()
        If Session.IsGuest Then Return
        For Each n In _v2.GetNotificationsForUser(Session.CurrentUserID)
            dgv.Rows.Add(n.NotificationID, n.Message, n.DateCreated.ToString("g"), If(n.IsRead, "Yes", "No"))
        Next
    End Sub

    Private Sub MarkRead(sender As Object, e As EventArgs)
        If dgv.SelectedRows.Count = 0 Then Return
        Dim id As Integer = CInt(dgv.SelectedRows(0).Cells("colID").Value)
        _v2.MarkNotificationRead(id)
        LoadData()
    End Sub

End Class
