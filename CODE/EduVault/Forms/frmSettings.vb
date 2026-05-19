Imports System.Windows.Forms

Public Class frmSettings
    Inherits Form

    Private ReadOnly _userService As New UserService()

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
    End Sub

    Public Sub New()
        Me.Text = "Settings"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(400, 280)
        BuildUI()
    End Sub

    Private pnlHeader As Panel

    Private Sub BuildUI()
        Me.BackColor = StyleHelper.ContentBg

        ' --- HEADER ---
        pnlHeader = New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconSettings & " Settings & Preferences",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 16),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)
        
        ' --- MAIN CONTENT ---
        Dim flpMain As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        
        ' Account Security Section
        Dim pnlSecurity As New Panel With {.Width = 500, .Height = 60, .Margin = New Padding(0)}
        pnlSecurity.Controls.Add(New Label With {.Text = "ACCOUNT SECURITY", .Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray})
        
        Dim btnPwd As New Button() With {
            .Text = "Change Password...", 
            .BackColor = StyleHelper.PrimaryColor, 
            .ForeColor = Color.White, 
            .FlatStyle = FlatStyle.Flat, 
            .Size = New Size(160, 32), 
            .Location = New Point(0, 20),
            .Font = New Font("Segoe UI", 9), 
            .Cursor = Cursors.Hand
        }
        btnPwd.FlatAppearance.BorderSize = 0
        AddHandler btnPwd.Click, Sub()
                                      Using f As New frmChangePassword()
                                          f.ShowDialog(Me)
                                      End Using
                                  End Sub
        pnlSecurity.Controls.Add(btnPwd)
        flpMain.Controls.Add(pnlSecurity)

        ' --- FOOTER ACTION BAR ---
        Dim pnlFooter As New Panel() With {.Dock = DockStyle.Bottom, .Height = 64, .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
        Dim tlpFooter As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .Padding = New Padding(12), .RowCount = 1, .ColumnCount = 2
        }
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        pnlFooter.Controls.Add(tlpFooter)

        Dim flpLeft As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False}
        Dim flpRight As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False, .FlowDirection = FlowDirection.RightToLeft}
        tlpFooter.Controls.Add(flpLeft, 0, 0)
        tlpFooter.Controls.Add(flpRight, 1, 0)
        
        Dim BuildActionBtn = Function(txt As String, bg As Color, fg As Color, w As Integer, icon As String) As Button
                                 Dim b As New Button() With {.Text = If(icon = "", txt, icon & " " & txt), .BackColor = bg, .ForeColor = fg, .FlatStyle = FlatStyle.Flat, .Size = New Size(w, 40), .Margin = New Padding(0, 0, 0, 16), .Font = New Font("Segoe UI", 9), .Cursor = Cursors.Hand}
                                 b.FlatAppearance.BorderSize = If(bg = Color.White, 1, 0)
                                 b.FlatAppearance.BorderColor = Color.LightGray
                                 Return b
                             End Function
        
        Dim btnClose As Button = BuildActionBtn("Close", Color.White, Color.DimGray, 80, "")
        btnClose.Height = 32
        btnClose.Margin = New Padding(10, 0, 0, 0)
        
        flpRight.Controls.Add(btnClose)

        AddHandler btnClose.Click, Sub() Me.Close()

        Dim pnlCard As New Panel With {
            .Width = 500,
            .Height = 200,
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        ' Center the card when the form resizes
        AddHandler Me.Resize, Sub()
                                  pnlCard.Location = New Point((Me.ClientSize.Width - pnlCard.Width) \ 2, (Me.ClientSize.Height - pnlCard.Height) \ 2)
                              End Sub

        pnlCard.Controls.Add(flpMain)
        pnlCard.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlCard)
        Me.Controls.Add(pnlHeader)
        
        Me.Size = New Size(420, 220)
    End Sub

End Class
