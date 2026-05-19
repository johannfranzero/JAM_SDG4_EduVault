Imports System.Windows.Forms
Imports System.Drawing

Public Class frmLogViewer
    Inherits Form
    Private ReadOnly _logRepo As New LogRepository()

    Private Sub frmLogViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "EduVault - System Log Audit"
        Me.Size = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        
        SetupUI()
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
        RefreshLogs()
    End Sub

    Private lblResultsLeft As New Label()

    Private pnlHeader As Panel

    Private Sub SetupUI()
        Me.BackColor = StyleHelper.ContentBg
        
        ' --- HEADER ---
        pnlHeader = New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconLogs & " System Audit Logs",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 16),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)

        ' --- RESULTS INFO BAR ---
        Dim pnlResultsInfo As New Panel() With {.Dock = DockStyle.Top, .Height = 36, .BackColor = StyleHelper.ContentBg}
        lblResultsLeft.Text = StyleHelper.IconLogs & " 0 log(s) found"
        lblResultsLeft.ForeColor = Color.DimGray
        lblResultsLeft.Font = New Font("Segoe UI", 8.5)
        lblResultsLeft.Location = New Point(16, 10)
        lblResultsLeft.AutoSize = True
        pnlResultsInfo.Controls.Add(lblResultsLeft)

        ' --- FOOTER ACTION BAR (TableLayoutPanel) ---
        Dim pnlFooter As New Panel() With {.Dock = DockStyle.Bottom, .Height = 64, .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
        Dim tlpFooter As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .Padding = New Padding(12), .RowCount = 1, .ColumnCount = 2
        }
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        pnlFooter.Controls.Add(tlpFooter)

        Dim flpRight As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False, .FlowDirection = FlowDirection.RightToLeft}
        tlpFooter.Controls.Add(flpRight, 1, 0)

        Dim btnRefresh As New Button With {
            .Text = StyleHelper.IconRefresh & " Refresh Logs",
            .Size = New Size(130, 32),
            .Margin = New Padding(10, 0, 0, 0),
            .BackColor = Color.White,
            .ForeColor = Color.DimGray,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .Font = New Font("Segoe UI", 9)
        }
        btnRefresh.FlatAppearance.BorderSize = 1
        btnRefresh.FlatAppearance.BorderColor = Color.LightGray
        AddHandler btnRefresh.Click, Sub() RefreshLogs()
        flpRight.Controls.Add(btnRefresh)
        
        ' --- DATAGRIDVIEW ---
        Dim dgv As New DataGridView With {
            .Name = "dgvLogs",
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }
        StyleHelper.ApplyGridStyle(dgv)
        
        Me.Controls.Add(dgv)
        Me.Controls.Add(pnlResultsInfo)
        Me.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlHeader)
        
        pnlResultsInfo.BringToFront()
        dgv.BringToFront()
    End Sub

    Private Sub RefreshLogs()
        Dim dgv As DataGridView = DirectCast(Me.Controls("dgvLogs"), DataGridView)
        dgv.DataSource = _logRepo.GetSystemLogs()
        lblResultsLeft.Text = StyleHelper.IconLogs & $" {dgv.Rows.Count} log(s) found"
        
        ' Format columns if needed
        If dgv.Columns.Count > 0 Then
            dgv.Columns("LogID").Width = 60
            dgv.Columns("LogLevel").Width = 80
            dgv.Columns("LogDate").DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"
            dgv.Columns("Message").FillWeight = 200
        End If
    End Sub
End Class
