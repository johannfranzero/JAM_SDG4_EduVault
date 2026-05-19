<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmDashboard
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlSidebar        = New System.Windows.Forms.Panel()
        Me.lblSideTitle      = New System.Windows.Forms.Label()
        Me.lblSideSubtitle   = New System.Windows.Forms.Label()
        Me.pnlSideDivider    = New System.Windows.Forms.Panel()
        Me.btnNavResources   = New System.Windows.Forms.Button()
        Me.btnNavUsers       = New System.Windows.Forms.Button()
        Me.btnNavReports     = New System.Windows.Forms.Button()
        Me.btnMyBookmarks    = New System.Windows.Forms.Button()
        Me.btnSystemLogs     = New System.Windows.Forms.Button()
        Me.btnBackup         = New System.Windows.Forms.Button()
        Me.btnNavLogout      = New System.Windows.Forms.Button()
        Me.btnSettings       = New System.Windows.Forms.Button()
        Me.btnNewResource    = New System.Windows.Forms.Button()
        Me.pnlTopBar         = New System.Windows.Forms.Panel()
        Me.lblDashboardTitle = New System.Windows.Forms.Label()
        Me.txtTopSearch      = New System.Windows.Forms.TextBox()
        Me.lblWelcome        = New System.Windows.Forms.Label()
        Me.lblRole           = New System.Windows.Forms.Label()
        Me.pnlStats          = New System.Windows.Forms.Panel()
        Me.pnlStatRes        = New System.Windows.Forms.Panel()
        Me.lblStatResLbl     = New System.Windows.Forms.Label()
        Me.lblStatResources  = New System.Windows.Forms.Label()
        Me.pnlStatAcc        = New System.Windows.Forms.Panel()
        Me.lblStatAccLbl     = New System.Windows.Forms.Label()
        Me.lblStatAccesses   = New System.Windows.Forms.Label()
        Me.pnlStatEb         = New System.Windows.Forms.Panel()
        Me.lblStatEbLbl      = New System.Windows.Forms.Label()
        Me.lblStatEbooks     = New System.Windows.Forms.Label()
        Me.pnlStatVid        = New System.Windows.Forms.Panel()
        Me.lblStatVidLbl     = New System.Windows.Forms.Label()
        Me.lblStatVideos     = New System.Windows.Forms.Label()
        Me.pnlStatPop        = New System.Windows.Forms.Panel()
        Me.lblStatPopLbl     = New System.Windows.Forms.Label()
        Me.lblStatPopular    = New System.Windows.Forms.Label()
        Me.pnlContent        = New System.Windows.Forms.Panel()
        Me.lblTrendTitle     = New System.Windows.Forms.Label()
        Me.dgvTrending       = New System.Windows.Forms.DataGridView()
        Me.colTrendID        = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTrendTag       = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTrendTitle     = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTrendCat       = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTrendType      = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTrendViews     = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTrendDate      = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.lblActivityTitle  = New System.Windows.Forms.Label()
        Me.lstActivity       = New System.Windows.Forms.ListBox()
        Me.lnkViewAll        = New System.Windows.Forms.LinkLabel()
        Me.stsBar            = New System.Windows.Forms.StatusStrip()
        Me.tsslInfo          = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pnlSidebar.SuspendLayout()
        Me.pnlTopBar.SuspendLayout()
        Me.pnlStats.SuspendLayout()
        Me.pnlContent.SuspendLayout()
        CType(Me.dgvTrending, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.stsBar.SuspendLayout()
        Me.SuspendLayout()

        ' ── SIDEBAR ─────────────────────────────────────────────────
        Dim sideNavy As System.Drawing.Color = System.Drawing.Color.FromArgb(28, 35, 64)
        Dim sideDark As System.Drawing.Color = System.Drawing.Color.FromArgb(20, 26, 48)

        Me.pnlSidebar.BackColor = sideNavy
        Me.pnlSidebar.Controls.Add(Me.lblSideTitle)
        Me.pnlSidebar.Controls.Add(Me.lblSideSubtitle)
        Me.pnlSidebar.Controls.Add(Me.pnlSideDivider)
        Me.pnlSidebar.Controls.Add(Me.btnNavResources)
        Me.pnlSidebar.Controls.Add(Me.btnNavUsers)
        Me.pnlSidebar.Controls.Add(Me.btnNavReports)
        Me.pnlSidebar.Controls.Add(Me.btnMyBookmarks)
        Me.pnlSidebar.Controls.Add(Me.btnSystemLogs)
        Me.pnlSidebar.Controls.Add(Me.btnBackup)
        Me.pnlSidebar.Controls.Add(Me.btnNavLogout)
        Me.pnlSidebar.Controls.Add(Me.btnSettings)
        Me.pnlSidebar.Controls.Add(Me.btnNewResource)
        Me.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlSidebar.Width = 200
        Me.pnlSidebar.Name = "pnlSidebar"

        Me.lblSideTitle.Text = "EduVault"
        Me.lblSideTitle.Font = New System.Drawing.Font("Segoe UI", 17, System.Drawing.FontStyle.Bold)
        Me.lblSideTitle.ForeColor = System.Drawing.Color.White
        Me.lblSideTitle.AutoSize = True
        Me.lblSideTitle.Location = New System.Drawing.Point(16, 22)
        Me.lblSideTitle.Name = "lblSideTitle"

        Me.lblSideSubtitle.Text = "Resource Library"
        Me.lblSideSubtitle.Font = New System.Drawing.Font("Segoe UI", 8)
        Me.lblSideSubtitle.ForeColor = System.Drawing.Color.FromArgb(120, 160, 210)
        Me.lblSideSubtitle.AutoSize = True
        Me.lblSideSubtitle.Location = New System.Drawing.Point(18, 54)
        Me.lblSideSubtitle.Name = "lblSideSubtitle"

        Me.pnlSideDivider.BackColor = System.Drawing.Color.FromArgb(45, 55, 90)
        Me.pnlSideDivider.Location = New System.Drawing.Point(0, 80)
        Me.pnlSideDivider.Size = New System.Drawing.Size(200, 1)
        Me.pnlSideDivider.Name = "pnlSideDivider"

        ' Nav button helper
        Dim SetNav As Action(Of Button, String, Integer) =
            Sub(btn As Button, txt As String, y As Integer)
                btn.Text = txt
                btn.Font = New System.Drawing.Font("Segoe UI", 9.5)
                btn.ForeColor = System.Drawing.Color.FromArgb(200, 225, 255)
                btn.BackColor = sideNavy
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                btn.FlatAppearance.BorderSize = 0
                btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(30, 70, 130)
                btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                btn.Padding = New System.Windows.Forms.Padding(14, 0, 0, 0)
                btn.Location = New System.Drawing.Point(0, y)
                btn.Size = New System.Drawing.Size(200, 42)
                btn.Cursor = System.Windows.Forms.Cursors.Hand
            End Sub

        SetNav(Me.btnNavResources, "  Browse Resources", 100)
        Me.btnNavResources.Name = "btnNavResources"

        SetNav(Me.btnNavUsers, "  Manage Users", 144)
        Me.btnNavUsers.Name = "btnNavUsers"

        SetNav(Me.btnNavReports, "  Reports", 188)
        Me.btnNavReports.Name = "btnNavReports"

        SetNav(Me.btnMyBookmarks, "  My Bookmarks", 232)
        Me.btnMyBookmarks.Name = "btnMyBookmarks"

        SetNav(Me.btnSystemLogs, "  System Logs", 276)
        Me.btnSystemLogs.Name = "btnSystemLogs"

        SetNav(Me.btnBackup, "  Backup Database", 320)
        Me.btnBackup.Name = "btnBackup"

        ' Logout — anchored to bottom, red accent
        Me.btnNavLogout.Text = "  Logout"
        Me.btnNavLogout.Font = New System.Drawing.Font("Segoe UI", 9.5, System.Drawing.FontStyle.Bold)
        Me.btnNavLogout.ForeColor = System.Drawing.Color.FromArgb(255, 120, 100)
        Me.btnNavLogout.BackColor = sideDark
        Me.btnNavLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNavLogout.FlatAppearance.BorderSize = 0
        Me.btnNavLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(150, 30, 20)
        Me.btnNavLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNavLogout.Padding = New System.Windows.Forms.Padding(14, 0, 0, 0)
        Me.btnNavLogout.Location = New System.Drawing.Point(0, 560)
        Me.btnNavLogout.Size = New System.Drawing.Size(200, 42)
        Me.btnNavLogout.Name = "btnNavLogout"
        Me.btnNavLogout.Cursor = System.Windows.Forms.Cursors.Hand

        ' New Resource button (green, near bottom of sidebar)
        Me.btnNewResource.Text = "➕ New Resource"
        Me.btnNewResource.Font = New System.Drawing.Font("Segoe UI", 9.0, System.Drawing.FontStyle.Bold)
        Me.btnNewResource.ForeColor = System.Drawing.Color.White
        Me.btnNewResource.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnNewResource.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNewResource.FlatAppearance.BorderSize = 0
        Me.btnNewResource.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(30, 140, 76)
        Me.btnNewResource.Location = New System.Drawing.Point(480, 10)
        Me.btnNewResource.Size = New System.Drawing.Size(140, 34)
        Me.btnNewResource.Name = "btnNewResource"
        Me.btnNewResource.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNewResource.Cursor = System.Windows.Forms.Cursors.Hand

        ' Settings button (above New Resource)
        Me.btnSettings.Text = "Settings"
        Me.btnSettings.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.btnSettings.ForeColor = System.Drawing.Color.FromArgb(180, 190, 210)
        Me.btnSettings.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSettings.FlatAppearance.BorderSize = 0
        Me.btnSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(30, 60, 110)
        Me.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSettings.Padding = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.btnSettings.Location = New System.Drawing.Point(0, 470)
        Me.btnSettings.Size = New System.Drawing.Size(200, 34)
        Me.btnSettings.Name = "btnSettings"
        Me.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand

        ' ── TOP BAR ─────────────────────────────────────────────────
        Me.pnlTopBar.BackColor = System.Drawing.Color.White
        Me.pnlTopBar.Controls.Add(Me.lblDashboardTitle)
        Me.pnlTopBar.Controls.Add(Me.txtTopSearch)
        Me.pnlTopBar.Controls.Add(Me.lblWelcome)
        Me.pnlTopBar.Controls.Add(Me.lblRole)
        Me.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTopBar.Height = 56
        Me.pnlTopBar.Name = "pnlTopBar"

        Me.lblDashboardTitle.Text = "Admin Dashboard"
        Me.lblDashboardTitle.Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
        Me.lblDashboardTitle.ForeColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.lblDashboardTitle.AutoSize = True
        Me.lblDashboardTitle.Location = New System.Drawing.Point(16, 14)
        Me.lblDashboardTitle.Name = "lblDashboardTitle"

        ' Search box in top bar
        Me.txtTopSearch.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.txtTopSearch.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120)
        Me.txtTopSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtTopSearch.Location = New System.Drawing.Point(260, 16)
        Me.txtTopSearch.Size = New System.Drawing.Size(210, 24)
        Me.txtTopSearch.Name = "txtTopSearch"
        Me.txtTopSearch.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left

        Me.lblWelcome.Text = "Welcome, User"
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80)
        Me.lblWelcome.AutoSize = True
        Me.lblWelcome.Location = New System.Drawing.Point(640, 10)
        Me.lblWelcome.Name = "lblWelcome"
        Me.lblWelcome.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right

        Me.lblRole.Text = "Role: Student"
        Me.lblRole.Font = New System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold)
        Me.lblRole.ForeColor = System.Drawing.Color.FromArgb(26, 140, 100)
        Me.lblRole.AutoSize = True
        Me.lblRole.Location = New System.Drawing.Point(640, 32)
        Me.lblRole.Name = "lblRole"
        Me.lblRole.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right

        ' ── STAT CARDS ──────────────────────────────────────────────
        Me.pnlStats.BackColor = System.Drawing.Color.FromArgb(245, 247, 252)
        Me.pnlStats.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlStats.Height = 88
        Me.pnlStats.Name = "pnlStats"
        Me.pnlStats.Padding = New System.Windows.Forms.Padding(12, 12, 12, 0)

        Dim BuildCard As Action(Of Panel, Label, Label, String, String, System.Drawing.Color) =
            Sub(card As Panel, lbl As Label, val As Label,
                labelText As String, initVal As String, accent As System.Drawing.Color)
                card.BackColor = System.Drawing.Color.White
                card.Size = New System.Drawing.Size(148, 64)
                lbl.Text = labelText
                lbl.Font = New System.Drawing.Font("Segoe UI", 7.5)
                lbl.ForeColor = System.Drawing.Color.DimGray
                lbl.AutoSize = True
                lbl.Location = New System.Drawing.Point(10, 8)
                card.Controls.Add(lbl)
                val.Text = initVal
                val.Font = New System.Drawing.Font("Segoe UI", 20, System.Drawing.FontStyle.Bold)
                val.ForeColor = accent
                val.AutoSize = True
                val.Location = New System.Drawing.Point(10, 26)
                card.Controls.Add(val)
            End Sub

        Me.pnlStatRes.Location = New System.Drawing.Point(12, 12)
        Me.pnlStatRes.Name = "pnlStatRes"
        BuildCard(Me.pnlStatRes, Me.lblStatResLbl, Me.lblStatResources, "Total Resources", "0", System.Drawing.Color.FromArgb(28, 35, 64))
        Me.lblStatResLbl.Name = "lblStatResLbl" : Me.lblStatResources.Name = "lblStatResources"
        Me.pnlStats.Controls.Add(Me.pnlStatRes)

        Me.pnlStatAcc.Location = New System.Drawing.Point(170, 12)
        Me.pnlStatAcc.Name = "pnlStatAcc"
        BuildCard(Me.pnlStatAcc, Me.lblStatAccLbl, Me.lblStatAccesses, "Total Accesses", "0", System.Drawing.Color.FromArgb(26, 140, 100))
        Me.lblStatAccLbl.Name = "lblStatAccLbl" : Me.lblStatAccesses.Name = "lblStatAccesses"
        Me.pnlStats.Controls.Add(Me.pnlStatAcc)

        Me.pnlStatEb.Location = New System.Drawing.Point(328, 12)
        Me.pnlStatEb.Name = "pnlStatEb"
        BuildCard(Me.pnlStatEb, Me.lblStatEbLbl, Me.lblStatEbooks, "E-Books", "0", System.Drawing.Color.FromArgb(100, 60, 180))
        Me.lblStatEbLbl.Name = "lblStatEbLbl" : Me.lblStatEbooks.Name = "lblStatEbooks"
        Me.pnlStats.Controls.Add(Me.pnlStatEb)

        Me.pnlStatVid.Location = New System.Drawing.Point(486, 12)
        Me.pnlStatVid.Name = "pnlStatVid"
        BuildCard(Me.pnlStatVid, Me.lblStatVidLbl, Me.lblStatVideos, "Videos", "0", System.Drawing.Color.FromArgb(200, 80, 20))
        Me.lblStatVidLbl.Name = "lblStatVidLbl" : Me.lblStatVideos.Name = "lblStatVideos"
        Me.pnlStats.Controls.Add(Me.pnlStatVid)

        Me.pnlStatPop.Location = New System.Drawing.Point(644, 12)
        Me.pnlStatPop.Name = "pnlStatPop"
        BuildCard(Me.pnlStatPop, Me.lblStatPopLbl, Me.lblStatPopular, "Popular (>50 views)", "0", System.Drawing.Color.FromArgb(180, 40, 40))
        Me.lblStatPopLbl.Name = "lblStatPopLbl" : Me.lblStatPopular.Name = "lblStatPopular"
        Me.pnlStats.Controls.Add(Me.pnlStatPop)

        ' ── CONTENT AREA ────────────────────────────────────────────
        Me.pnlContent.BackColor = System.Drawing.Color.FromArgb(245, 247, 252)
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Padding = New System.Windows.Forms.Padding(16, 12, 16, 12)
        Me.pnlContent.Controls.Add(Me.lblTrendTitle)
        Me.pnlContent.Controls.Add(Me.lnkViewAll)
        Me.pnlContent.Controls.Add(Me.dgvTrending)
        Me.pnlContent.Controls.Add(Me.lblActivityTitle)
        Me.pnlContent.Controls.Add(Me.lstActivity)

        Me.lblTrendTitle.Text = "Trending Resources (Top 5)"
        Me.lblTrendTitle.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.lblTrendTitle.ForeColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.lblTrendTitle.AutoSize = True
        Me.lblTrendTitle.Location = New System.Drawing.Point(16, 12)
        Me.lblTrendTitle.Name = "lblTrendTitle"
        Me.lblTrendTitle.Text = "🔥 Trending Resources (Top 5)"

        Me.lnkViewAll.Text = "View All →"
        Me.lnkViewAll.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.lnkViewAll.Location = New System.Drawing.Point(490, 14)
        Me.lnkViewAll.AutoSize = True
        Me.lnkViewAll.Name = "lnkViewAll"
        Me.lnkViewAll.LinkColor = System.Drawing.Color.FromArgb(41, 128, 185)
        Me.lnkViewAll.ActiveLinkColor = System.Drawing.Color.FromArgb(28, 35, 64)

        Me.dgvTrending.AllowUserToAddRows = False
        Me.dgvTrending.AllowUserToDeleteRows = False
        Me.dgvTrending.BackgroundColor = System.Drawing.Color.White
        Me.dgvTrending.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvTrending.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.dgvTrending.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White
        Me.dgvTrending.ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Me.dgvTrending.ColumnHeadersHeight = 36
        Me.dgvTrending.DefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.dgvTrending.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(210, 225, 245)
        Me.dgvTrending.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black
        Me.dgvTrending.RowTemplate.Height = 38
        Me.dgvTrending.Location = New System.Drawing.Point(16, 42)
        Me.dgvTrending.Name = "dgvTrending"
        Me.dgvTrending.ReadOnly = True
        Me.dgvTrending.RowHeadersVisible = False
        Me.dgvTrending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvTrending.Size = New System.Drawing.Size(560, 280)
        Me.dgvTrending.TabIndex = 0
        Me.dgvTrending.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right

        Me.colTrendID.Name = "colTrendID" : Me.colTrendID.HeaderText = "ID" : Me.colTrendID.Width = 40 : Me.colTrendID.Visible = False
        Me.colTrendTag.Name = "colTrendTag" : Me.colTrendTag.HeaderText = "Tag" : Me.colTrendTag.Width = 70
        Me.colTrendTitle.Name = "colTrendTitle" : Me.colTrendTitle.HeaderText = "Title" : Me.colTrendTitle.Width = 200
        Me.colTrendCat.Name = "colTrendCat" : Me.colTrendCat.HeaderText = "Category" : Me.colTrendCat.Width = 110
        Me.colTrendType.Name = "colTrendType" : Me.colTrendType.HeaderText = "Type" : Me.colTrendType.Width = 70
        Me.colTrendViews.Name = "colTrendViews" : Me.colTrendViews.HeaderText = "Views" : Me.colTrendViews.Width = 55
        Me.colTrendDate.Name = "colTrendDate" : Me.colTrendDate.HeaderText = "Date Added" : Me.colTrendDate.Width = 110
        Me.dgvTrending.Columns.AddRange(Me.colTrendID, Me.colTrendTag, Me.colTrendTitle,
                                        Me.colTrendCat, Me.colTrendType, Me.colTrendViews, Me.colTrendDate)

        Me.lblActivityTitle.Text = "Recent Activity"
        Me.lblActivityTitle.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.lblActivityTitle.ForeColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.lblActivityTitle.AutoSize = True
        Me.lblActivityTitle.Location = New System.Drawing.Point(600, 12)
        Me.lblActivityTitle.Name = "lblActivityTitle"
        Me.lblActivityTitle.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right

        Me.lstActivity.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.lstActivity.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lstActivity.Location = New System.Drawing.Point(600, 42)
        Me.lstActivity.Name = "lstActivity"
        Me.lstActivity.Size = New System.Drawing.Size(228, 280)
        Me.lstActivity.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right Or System.Windows.Forms.AnchorStyles.Bottom

        ' ── STATUS BAR ──────────────────────────────────────────────
        Me.stsBar.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.stsBar.Items.Add(Me.tsslInfo)
        Me.tsslInfo.ForeColor = System.Drawing.Color.FromArgb(160, 200, 255)
        Me.tsslInfo.Name = "tsslInfo"
        Me.tsslInfo.Text = "EduVault  -  UN SDG 4: Quality Education  -  Double-click a trending resource to open it."
        Me.stsBar.Name = "stsBar"

        ' ── FORM ────────────────────────────────────────────────────
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1024, 740)
        Me.Controls.Add(Me.pnlContent)
        Me.Controls.Add(Me.pnlStats)
        Me.Controls.Add(Me.pnlTopBar)
        Me.Controls.Add(Me.pnlSidebar)
        Me.Controls.Add(Me.stsBar)
        Me.MinimumSize = New System.Drawing.Size(900, 600)
        Me.Name = "frmDashboard"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EduVault"

        Me.pnlSidebar.ResumeLayout(False)
        Me.pnlSidebar.PerformLayout()
        Me.pnlTopBar.ResumeLayout(False)
        Me.pnlTopBar.PerformLayout()
        Me.pnlStats.ResumeLayout(False)
        Me.pnlContent.ResumeLayout(False)
        Me.pnlContent.PerformLayout()
        CType(Me.dgvTrending, System.ComponentModel.ISupportInitialize).EndInit()
        Me.stsBar.ResumeLayout(False)
        Me.stsBar.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents pnlSidebar        As System.Windows.Forms.Panel
    Friend WithEvents lblSideTitle      As System.Windows.Forms.Label
    Friend WithEvents lblSideSubtitle   As System.Windows.Forms.Label
    Friend WithEvents pnlSideDivider    As System.Windows.Forms.Panel
    Friend WithEvents btnNavResources   As System.Windows.Forms.Button
    Friend WithEvents btnNavUsers       As System.Windows.Forms.Button
    Friend WithEvents btnNavReports     As System.Windows.Forms.Button
    Friend WithEvents btnMyBookmarks    As System.Windows.Forms.Button
    Friend WithEvents btnSystemLogs     As System.Windows.Forms.Button
    Friend WithEvents btnBackup         As System.Windows.Forms.Button
    Friend WithEvents btnNavLogout      As System.Windows.Forms.Button
    Friend WithEvents btnSettings       As System.Windows.Forms.Button
    Friend WithEvents btnNewResource    As System.Windows.Forms.Button
    Friend WithEvents pnlTopBar         As System.Windows.Forms.Panel
    Friend WithEvents lblDashboardTitle As System.Windows.Forms.Label
    Friend WithEvents txtTopSearch      As System.Windows.Forms.TextBox
    Friend WithEvents lblWelcome        As System.Windows.Forms.Label
    Friend WithEvents lblRole           As System.Windows.Forms.Label
    Friend WithEvents pnlStats          As System.Windows.Forms.Panel
    Friend WithEvents pnlStatRes        As System.Windows.Forms.Panel
    Friend WithEvents lblStatResLbl     As System.Windows.Forms.Label
    Friend WithEvents lblStatResources  As System.Windows.Forms.Label
    Friend WithEvents pnlStatAcc        As System.Windows.Forms.Panel
    Friend WithEvents lblStatAccLbl     As System.Windows.Forms.Label
    Friend WithEvents lblStatAccesses   As System.Windows.Forms.Label
    Friend WithEvents pnlStatEb         As System.Windows.Forms.Panel
    Friend WithEvents lblStatEbLbl      As System.Windows.Forms.Label
    Friend WithEvents lblStatEbooks     As System.Windows.Forms.Label
    Friend WithEvents pnlStatVid        As System.Windows.Forms.Panel
    Friend WithEvents lblStatVidLbl     As System.Windows.Forms.Label
    Friend WithEvents lblStatVideos     As System.Windows.Forms.Label
    Friend WithEvents pnlStatPop        As System.Windows.Forms.Panel
    Friend WithEvents lblStatPopLbl     As System.Windows.Forms.Label
    Friend WithEvents lblStatPopular    As System.Windows.Forms.Label
    Friend WithEvents pnlContent        As System.Windows.Forms.Panel
    Friend WithEvents lblTrendTitle     As System.Windows.Forms.Label
    Friend WithEvents dgvTrending       As System.Windows.Forms.DataGridView
    Friend WithEvents colTrendID        As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTrendTag       As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTrendTitle     As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTrendCat       As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTrendType      As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTrendViews     As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTrendDate      As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents lblActivityTitle  As System.Windows.Forms.Label
    Friend WithEvents lstActivity       As System.Windows.Forms.ListBox
    Friend WithEvents lnkViewAll        As System.Windows.Forms.LinkLabel
    Friend WithEvents stsBar            As System.Windows.Forms.StatusStrip
    Friend WithEvents tsslInfo          As System.Windows.Forms.ToolStripStatusLabel

End Class
