Imports System.Windows.Forms

Public Class frmBackupSchedule
    Inherits Form

    Private ReadOnly _v2 As New V2FeatureRepository()
    Private ReadOnly _backupRepo As New BackupRepository()
    Private ReadOnly _reportService As New ReportService()
    Private _schedule As BackupScheduleInfo

    Private numDays As NumericUpDown
    Private txtPath As TextBox
    Private chkEnabled As CheckBox
    Private lblLast As Label
    Private lblNext As Label

    Public Sub New()
        Me.Text = "Backup schedule"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(480, 300)
        _schedule = _v2.GetBackupSchedule()
        BuildUI()
    End Sub

    Private Sub frmBackupSchedule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
    End Sub

    Private pnlHeader As Panel

    Private Sub BuildUI()
        Me.BackColor = StyleHelper.ContentBg

        ' --- HEADER ---
        pnlHeader = New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconBackup & " Backup Schedule",
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
        
        ' Frequency
        Dim pnlFreq As New Panel With {.Width = 500, .Height = 60, .Margin = New Padding(0, 0, 0, 15)}
        pnlFreq.Controls.Add(New Label With {.Text = "FREQUENCY (DAYS)", .Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray})
        numDays = New NumericUpDown With {.Location = New Point(0, 20), .Width = 100, .Minimum = 1, .Maximum = 90, .Value = Math.Max(1, _schedule.FrequencyDays), .Font = New Font("Segoe UI", 9.5)}
        pnlFreq.Controls.Add(numDays)
        flpMain.Controls.Add(pnlFreq)
        
        ' Path
        Dim pnlPath As New Panel With {.Width = 500, .Height = 60, .Margin = New Padding(0, 0, 0, 15)}
        pnlPath.Controls.Add(New Label With {.Text = "DEFAULT BACKUP FOLDER", .Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray})
        txtPath = New TextBox With {.Location = New Point(0, 20), .Width = 400, .Text = _schedule.BackupPath, .Font = New Font("Segoe UI", 9.5), .BorderStyle = BorderStyle.FixedSingle}
        pnlPath.Controls.Add(txtPath)
        flpMain.Controls.Add(pnlPath)
        
        ' Enable Checkbox
        Dim pnlEnable As New Panel With {.Width = 500, .Height = 40, .Margin = New Padding(0, 0, 0, 15)}
        chkEnabled = New CheckBox With {.Text = " Enable automated backup schedule", .Location = New Point(0, 0), .Checked = _schedule.IsEnabled, .Font = New Font("Segoe UI", 9), .AutoSize = True}
        pnlEnable.Controls.Add(chkEnabled)
        flpMain.Controls.Add(pnlEnable)
        
        ' Status Labels
        Dim pnlStatus As New Panel With {.Width = 500, .Height = 60, .Margin = New Padding(0)}
        lblLast = New Label With {.Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8.5), .ForeColor = Color.DimGray, .Text = $"Last backup: {If(_schedule.LastBackupDate.HasValue, _schedule.LastBackupDate.Value.ToString("g"), "Never")}"}
        lblNext = New Label With {.Location = New Point(0, 20), .AutoSize = True, .Font = New Font("Segoe UI", 8.5), .ForeColor = Color.DimGray, .Text = $"Next scheduled: {If(_schedule.NextBackupDate.HasValue, _schedule.NextBackupDate.Value.ToString("g"), "Not set")}"}
        pnlStatus.Controls.Add(lblLast)
        pnlStatus.Controls.Add(lblNext)
        flpMain.Controls.Add(pnlStatus)

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
        
        Dim BuildActionBtn = Function(txt As String, bg As Color, fg As Color, w As Integer, margin As Padding) As Button
                                 Dim b As New Button() With {.Text = txt, .BackColor = bg, .ForeColor = fg, .FlatStyle = FlatStyle.Flat, .Size = New Size(w, 32), .Margin = margin, .Font = New Font("Segoe UI", 9), .Cursor = Cursors.Hand}
                                 b.FlatAppearance.BorderSize = If(bg = Color.White, 1, 0)
                                 b.FlatAppearance.BorderColor = Color.LightGray
                                 Return b
                             End Function

        Dim btnSave As Button = BuildActionBtn("Save schedule", StyleHelper.AccentBlue, Color.White, 110, New Padding(0, 0, 10, 0))
        Dim btnRun As Button = BuildActionBtn(StyleHelper.IconBackup & " Run backup now", Color.FromArgb(22, 163, 74), Color.White, 140, New Padding(0, 0, 10, 0))
        Dim btnClose As Button = BuildActionBtn("Close", Color.White, Color.DimGray, 80, New Padding(10, 0, 0, 0))
        
        flpLeft.Controls.Add(btnSave)
        flpLeft.Controls.Add(btnRun)
        flpRight.Controls.Add(btnClose)

        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnRun.Click, AddressOf BtnRun_Click
        AddHandler btnClose.Click, Sub() Me.Close()

        Dim pnlCard As New Panel With {
            .Width = 600,
            .Height = 400,
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
        
        Me.Size = New Size(540, 420)
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If _v2.UpdateBackupSchedule(_schedule.ScheduleID, CInt(numDays.Value), txtPath.Text.Trim(), chkEnabled.Checked) Then
            MessageBox.Show("Backup schedule saved successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information)
            _schedule = _v2.GetBackupSchedule()
        End If
    End Sub

    Private Sub BtnRun_Click(sender As Object, e As EventArgs)
        Using sfd As New SaveFileDialog()
            sfd.Filter = "SQL Backup (*.bak)|*.bak"
            sfd.FileName = $"EduVaultDB_{DateTime.Now:yyyyMMdd_HHmm}.bak"
            If sfd.ShowDialog() <> DialogResult.OK Then Return
            Try
                If _reportService.BackupDatabase(sfd.FileName) Then
                    _v2.RecordBackupRun(_schedule.ScheduleID)
                    _schedule = _v2.GetBackupSchedule()
                    lblLast.Text = $"Last backup: {_schedule.LastBackupDate.Value:g}"
                    lblNext.Text = $"Next scheduled: {If(_schedule.NextBackupDate.HasValue, _schedule.NextBackupDate.Value.ToString("g"), "Not set")}"
                    MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Backup", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

End Class
