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
    Private pnlHeader As Panel

    Public Sub New()
        Me.Text = "Backup Database"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(700, 460)
        _schedule = _v2.GetBackupSchedule()
        BuildUI()
    End Sub

    Private Sub frmBackupSchedule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Hide our own header when embedded in the dashboard SPA host
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
    End Sub

    Private Sub BuildUI()
        Me.BackColor = StyleHelper.ContentBg

        ' ── HEADER (visible only when opened standalone) ──
        pnlHeader = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 52,
            .BackColor = StyleHelper.PrimaryColor
        }
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconBackup & "  Backup Database",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 14),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)

        ' ── FOOTER ACTION BAR ──
        Dim pnlFooter As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 60,
            .BackColor = Color.White
        }
        ' Top border line on the footer
        AddHandler pnlFooter.Paint,
            Sub(s As Object, ev As PaintEventArgs)
                Using p As New Pen(Color.FromArgb(220, 225, 235), 1)
                    ev.Graphics.DrawLine(p, 0, 0, DirectCast(s, Panel).Width, 0)
                End Using
            End Sub

        Dim tlpFooter As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16, 0, 16, 0),
            .RowCount = 1,
            .ColumnCount = 2
        }
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        pnlFooter.Controls.Add(tlpFooter)

        Dim flpLeft As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .WrapContents = False,
            .Padding = New Padding(0, 10, 0, 0)
        }
        Dim flpRight As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .WrapContents = False,
            .FlowDirection = FlowDirection.RightToLeft,
            .Padding = New Padding(0, 10, 0, 0)
        }
        tlpFooter.Controls.Add(flpLeft, 0, 0)
        tlpFooter.Controls.Add(flpRight, 1, 0)

        Dim BuildBtn = Function(txt As String, bg As Color, fg As Color, w As Integer) As Button
                           Dim b As New Button With {
                               .Text = txt,
                               .BackColor = bg,
                               .ForeColor = fg,
                               .FlatStyle = FlatStyle.Flat,
                               .Size = New Size(w, 34),
                               .Font = New Font("Segoe UI", 9),
                               .Cursor = Cursors.Hand,
                               .Margin = New Padding(0, 0, 8, 0)
                           }
                           b.FlatAppearance.BorderSize = If(bg = Color.White, 1, 0)
                           b.FlatAppearance.BorderColor = Color.FromArgb(210, 215, 225)
                           Return b
                       End Function

        Dim btnSave As Button = BuildBtn("  Save Schedule", StyleHelper.AccentBlue, Color.White, 130)
        Dim btnRun As Button = BuildBtn(StyleHelper.IconBackup & "  Run Backup Now", Color.FromArgb(22, 163, 74), Color.White, 160)
        Dim btnClose As Button = BuildBtn("Close", Color.White, Color.DimGray, 80)

        flpLeft.Controls.Add(btnSave)
        flpLeft.Controls.Add(btnRun)
        flpRight.Controls.Add(btnClose)

        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnRun.Click, AddressOf BtnRun_Click
        AddHandler btnClose.Click, Sub() Me.Close()

        ' ── MAIN CONTENT SCROLL PANEL ──
        Dim pnlScroll As New Panel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .Padding = New Padding(32, 24, 32, 0),
            .BackColor = StyleHelper.ContentBg
        }

        ' ── CARD ──
        Dim pnlCard As New Panel With {
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(24),
            .Width = 600,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }

        ' Card header row
        Dim lblCardTitle As New Label With {
            .Text = StyleHelper.IconBackup & "  Backup Configuration",
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = StyleHelper.PrimaryColor,
            .AutoSize = True,
            .Location = New Point(24, 20)
        }
        pnlCard.Controls.Add(lblCardTitle)

        ' Thin divider under card header
        Dim divCard As New Panel With {
            .BackColor = Color.FromArgb(235, 238, 245),
            .Height = 1,
            .Location = New Point(24, 50),
            .Width = 552
        }
        pnlCard.Controls.Add(divCard)

        ' ── ROW 1: Frequency ──
        Dim lblFreqCap As New Label With {
            .Text = "BACKUP FREQUENCY (DAYS)",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Bold),
            .ForeColor = Color.FromArgb(100, 116, 139),
            .AutoSize = True,
            .Location = New Point(24, 68)
        }
        numDays = New NumericUpDown With {
            .Location = New Point(24, 88),
            .Width = 120,
            .Minimum = 1,
            .Maximum = 90,
            .Value = Math.Max(1, _schedule.FrequencyDays),
            .Font = New Font("Segoe UI", 10),
            .BorderStyle = BorderStyle.FixedSingle
        }
        pnlCard.Controls.Add(lblFreqCap)
        pnlCard.Controls.Add(numDays)

        ' ── ROW 2: Path ──
        Dim lblPathCap As New Label With {
            .Text = "DEFAULT BACKUP FOLDER",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Bold),
            .ForeColor = Color.FromArgb(100, 116, 139),
            .AutoSize = True,
            .Location = New Point(24, 128)
        }
        txtPath = New TextBox With {
            .Location = New Point(24, 148),
            .Width = 468,
            .Text = _schedule.BackupPath,
            .Font = New Font("Segoe UI", 9.5),
            .BorderStyle = BorderStyle.FixedSingle
        }
        Dim btnBrowse As New Button With {
            .Text = "Browse…",
            .Location = New Point(500, 147),
            .Width = 76,
            .Height = 26,
            .Font = New Font("Segoe UI", 8.5),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .BackColor = Color.FromArgb(240, 244, 250),
            .ForeColor = StyleHelper.PrimaryColor
        }
        btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(210, 215, 225)
        AddHandler btnBrowse.Click,
            Sub()
                Using fbd As New FolderBrowserDialog()
                    fbd.SelectedPath = txtPath.Text
                    If fbd.ShowDialog() = DialogResult.OK Then
                        txtPath.Text = fbd.SelectedPath
                    End If
                End Using
            End Sub
        pnlCard.Controls.Add(lblPathCap)
        pnlCard.Controls.Add(txtPath)
        pnlCard.Controls.Add(btnBrowse)

        ' ── ROW 3: Enable toggle ──
        chkEnabled = New CheckBox With {
            .Text = " Enable automated backup schedule",
            .Location = New Point(24, 192),
            .Checked = _schedule.IsEnabled,
            .Font = New Font("Segoe UI", 9.5),
            .AutoSize = True,
            .Cursor = Cursors.Hand
        }
        pnlCard.Controls.Add(chkEnabled)

        ' ── Divider ──
        Dim divStatus As New Panel With {
            .BackColor = Color.FromArgb(235, 238, 245),
            .Height = 1,
            .Location = New Point(24, 232),
            .Width = 552
        }
        pnlCard.Controls.Add(divStatus)

        ' ── ROW 4: Status block ──
        Dim lblStatusCap As New Label With {
            .Text = "BACKUP STATUS",
            .Font = New Font("Segoe UI", 7.5, FontStyle.Bold),
            .ForeColor = Color.FromArgb(100, 116, 139),
            .AutoSize = True,
            .Location = New Point(24, 244)
        }
        lblLast = New Label With {
            .Text = "Last backup:      " & If(_schedule.LastBackupDate.HasValue, _schedule.LastBackupDate.Value.ToString("g"), "Never"),
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(71, 85, 105),
            .AutoSize = True,
            .Location = New Point(24, 264)
        }
        lblNext = New Label With {
            .Text = "Next scheduled: " & If(_schedule.NextBackupDate.HasValue, _schedule.NextBackupDate.Value.ToString("g"), "Not set"),
            .Font = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(71, 85, 105),
            .AutoSize = True,
            .Location = New Point(24, 286)
        }
        pnlCard.Controls.Add(lblStatusCap)
        pnlCard.Controls.Add(lblLast)
        pnlCard.Controls.Add(lblNext)

        ' Card needs enough height
        pnlCard.Height = 324

        ' Center card horizontally when the scroll panel resizes
        AddHandler pnlScroll.Resize,
            Sub()
                Dim x As Integer = Math.Max(0, (pnlScroll.ClientSize.Width - pnlCard.Width) \ 2)
                pnlCard.Location = New Point(x, 24)
            End Sub
        pnlCard.Location = New Point(32, 24)

        pnlScroll.Controls.Add(pnlCard)

        ' ── ASSEMBLE ──
        Me.Controls.Add(pnlScroll)
        Me.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlHeader)
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
                    lblLast.Text = "Last backup:      " & _schedule.LastBackupDate.Value.ToString("g")
                    lblNext.Text = "Next scheduled: " & If(_schedule.NextBackupDate.HasValue, _schedule.NextBackupDate.Value.ToString("g"), "Not set")
                    MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

End Class
