Imports System.Windows.Forms

Public Class frmPendingRequests
    Inherits Form

    Private ReadOnly _v2 As New V2FeatureRepository()
    Private dgv As DataGridView

    Private lblResultsLeft As Label
    
    Private Sub frmPendingRequests_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
    End Sub

    Public Sub New()
        Me.Text = "Pending resource requests"
        Me.Size = New Size(640, 400)
        BuildUI()
    End Sub

    Private pnlHeader As Panel

    Private Sub BuildUI()
        Me.BackColor = StyleHelper.ContentBg

        ' --- HEADER ---
        pnlHeader = New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconPeople & " Pending Resource Requests",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 16),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)

        ' --- RESULTS INFO BAR ---
        Dim pnlResultsInfo As New Panel() With {.Dock = DockStyle.Top, .Height = 36, .BackColor = StyleHelper.ContentBg}
        lblResultsLeft = New Label With {
            .Text = StyleHelper.IconPeople & " 0 pending request(s)",
            .ForeColor = Color.DimGray,
            .Font = New Font("Segoe UI", 8.5),
            .Location = New Point(16, 10),
            .AutoSize = True
        }
        pnlResultsInfo.Controls.Add(lblResultsLeft)

        ' --- FOOTER ACTION BAR ---
        Dim pnlFooter As New Panel() With {.Dock = DockStyle.Bottom, .Height = 64, .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
        Dim tlpFooter As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill, .Padding = New Padding(12), .RowCount = 1, .ColumnCount = 2
        }
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        pnlFooter.Controls.Add(tlpFooter)

        Dim flpLeft As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .WrapContents = False}
        tlpFooter.Controls.Add(flpLeft, 0, 0)

        Dim btnApprove As New Button With {.Text = "Approve Selected", .BackColor = StyleHelper.AccentBlue, .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(140, 32), .Margin = New Padding(0, 0, 10, 0), .Font = New Font("Segoe UI", 9), .Cursor = Cursors.Hand}
        btnApprove.FlatAppearance.BorderSize = 0
        
        Dim btnReject As New Button With {.Text = "Reject Selected", .BackColor = Color.FromArgb(192, 57, 43), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(130, 32), .Margin = New Padding(0, 0, 10, 0), .Font = New Font("Segoe UI", 9), .Cursor = Cursors.Hand}
        btnReject.FlatAppearance.BorderSize = 0

        AddHandler btnApprove.Click, Sub() SetStatus("Approved")
        AddHandler btnReject.Click, Sub() SetStatus("Rejected")
        flpLeft.Controls.AddRange({btnApprove, btnReject})

        ' --- DATAGRIDVIEW ---
        dgv = New DataGridView With {
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }
        StyleHelper.ApplyGridStyle(dgv)
        dgv.Columns.Add("colID", "ID")
        dgv.Columns.Add("colUser", "Requested by")
        dgv.Columns.Add("colTitle", "Title")
        dgv.Columns.Add("colDate", "Date")
        dgv.Columns("colID").Visible = False

        Me.Controls.Add(dgv)
        Me.Controls.Add(pnlResultsInfo)
        Me.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlHeader)
        
        dgv.BringToFront()
        
        LoadData()
    End Sub

    Private Sub LoadData()
        dgv.Rows.Clear()
        For Each r In _v2.GetPendingResourceRequests()
            dgv.Rows.Add(r.RequestID, r.RequesterName, r.Title, r.DateRequested.ToString("g"))
        Next
        lblResultsLeft.Text = StyleHelper.IconPeople & $" {dgv.Rows.Count} pending request(s)"
    End Sub

    Private Sub SetStatus(status As String)
        If dgv.SelectedRows.Count = 0 Then Return
        Dim id As Integer = CInt(dgv.SelectedRows(0).Cells("colID").Value)
        If _v2.UpdateResourceRequestStatus(id, status, status) Then
            MessageBox.Show($"Request marked {status}.", "Status Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadData()
        End If
    End Sub

End Class
