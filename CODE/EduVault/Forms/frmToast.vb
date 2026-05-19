Imports System.Windows.Forms
Imports System.Drawing

Public Class frmToast
    Inherits Form

    Private WithEvents _timer As New Timer()
    Private _opacityStep As Double = 0.05

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Public Sub New(message As String, Optional isError As Boolean = False)
        ' Form Setup
        Me.FormBorderStyle = FormBorderStyle.None
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.Size = New Size(300, 60)
        Me.BackColor = If(isError, Color.FromArgb(231, 76, 60), Color.FromArgb(46, 204, 113))
        Me.Opacity = 0

        Dim lbl As New Label()
        lbl.Text = message
        lbl.ForeColor = Color.White
        lbl.Font = New Font("Segoe UI Variable Text", 9.5, FontStyle.Bold)
        lbl.TextAlign = ContentAlignment.MiddleCenter
        lbl.Dock = DockStyle.Fill
        Me.Controls.Add(lbl)

        ' Position at bottom right
        Dim wa As Rectangle = Screen.PrimaryScreen.WorkingArea
        Me.Location = New Point(wa.Right - Me.Width - 20, wa.Bottom - Me.Height - 20)

        _timer.Interval = 30
        _timer.Start()
    End Sub

    Private Sub _timer_Tick(sender As Object, e As EventArgs) Handles _timer.Tick
        If Me.Opacity < 1 AndAlso _opacityStep > 0 Then
            Me.Opacity += _opacityStep
        ElseIf Me.Opacity >= 1 Then
            _timer.Interval = 2500 ' Show for 2.5 seconds
            _opacityStep = -0.05
            Me.Opacity = 0.99 ' Trigger next phase
        ElseIf Me.Opacity > 0 AndAlso _opacityStep < 0 Then
            Me.Opacity += _opacityStep
        Else
            _timer.Stop()
            Me.Close()
        End If
    End Sub
End Class
