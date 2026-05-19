Imports System.Windows.Forms.DataVisualization.Charting

''' <summary>
''' frmDashboard - EduVault Main Dashboard (Presentation Layer)
''' Central hub for all navigation. Adapts UI based on user role:
'''   Admin  - sees stats panel, manage resources/users, reports menu
'''   Student - sees resource browser, bookmarks, recent activity
''' </summary>
Public Class frmDashboard

    Private ReadOnly _resourceService As New ResourceService()
    Private ReadOnly _reportService As New ReportService()
    Private ReadOnly _logRepo As New AccessLogRepository()
    Private ReadOnly _userRepo As New UserRepository()
    Private ReadOnly _catService As New CategoryService()
    Private ReadOnly _v2Repo As New V2FeatureRepository()
    Private ReadOnly _searchAutocomplete As New AutoCompleteStringCollection()
    Private _isLoggingOut As Boolean = False
    Private _middleWidgetReady As Boolean = False

    Private Const TopSearchPlaceholder As String = "resources, keywords..."
    Private Const DefaultStatusHint As String = "EduVault  -  UN SDG 4: Quality Education"
    Private _defaultDashboardTitle As String = "Dashboard"

    Private Sub ConfigureTopSearchBox()
        txtTopSearch.Text = String.Empty
        txtTopSearch.ForeColor = Color.Black
        If txtTopSearch.IsHandleCreated Then
            StyleHelper.SetPlaceholder(txtTopSearch, TopSearchPlaceholder)
        Else
            AddHandler txtTopSearch.HandleCreated,
                Sub()
                    StyleHelper.SetPlaceholder(txtTopSearch, TopSearchPlaceholder)
                End Sub
        End If
    End Sub

    Private Sub PositionTopBarControls()
        lblDashboardTitle.Location = New Point(16, 22)
        lblDashboardTitle.AutoSize = True
    End Sub

    ' ---------------------------------------------------------------
    ' FORM EVENTS
    ' ---------------------------------------------------------------

    Private Sub frmDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Session.IsLoggedIn Then
            Me.Close()
            Return
        End If

        BuildRedesignedUI()
        SetupUIForRole()
        ConfigureTopSearchBox()
        ApplyStyles()
        UpdateNavActiveState(btnNavDashboard) ' Set Dashboard active
        LoadDashboardData()
        SetupTopSearchAutocomplete()
        RefreshNotificationBadge()
    End Sub

    Private Sub frmDashboard_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        _middleWidgetReady = True
        LoadMiddleWidget()
    End Sub

    Private Sub SetupTopSearchAutocomplete()
        txtTopSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        txtTopSearch.AutoCompleteSource = AutoCompleteSource.CustomSource
        txtTopSearch.AutoCompleteCustomSource = _searchAutocomplete
    End Sub

    Private Sub txtTopSearch_TextChanged(sender As Object, e As EventArgs) Handles txtTopSearch.TextChanged
        _searchAutocomplete.Clear()
        Dim prefix As String = txtTopSearch.Text.Trim()
        If prefix.Length < 2 Then Return
        For Each title In _resourceService.SearchTitlesForAutocomplete(prefix)
            _searchAutocomplete.Add(title)
        Next
    End Sub

    Private Sub ApplyStyles()
        ' Sidebar
        pnlSidebar.BackColor = StyleHelper.SidebarColor
        lblSideTitle.Font = New Font("Segoe UI Variable Display", 18, FontStyle.Bold)
        lblSideSubtitle.Font = StyleHelper.SmallFont

        ' Top Bar
        pnlTopBar.BackColor = StyleHelper.WhiteColor
        lblDashboardTitle.Font = StyleHelper.HeaderFont
        lblDashboardTitle.ForeColor = StyleHelper.PrimaryColor
        lblWelcome.Font = StyleHelper.NormalFont
        lblRole.Font = New Font("Segoe UI Variable Text", 8, FontStyle.Bold)

        ' Grid
        StyleHelper.ApplyGridStyle(dgvTrending)
        dgvTrending.RowTemplate.Height = 38

        ' Navigation Buttons
        UpdateNavActiveState(Nothing) ' Initialize styles

        StyleHelper.ApplyButtonStyle(btnNewResource, isAccent:=True)
        btnNewResource.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnNewResource.Height = 36

        ' Set background
        Me.BackColor = StyleHelper.ContentBg
        pnlContent.BackColor = StyleHelper.ContentBg
        pnlStats.BackColor = StyleHelper.ContentBg
    End Sub

    ''' <summary>Highlights the active sidebar nav panel and resets others.</summary>
    Private Sub UpdateNavActiveState(activeBtn As Button)
        Dim navButtons As Button() = {btnNavDashboard, btnNavResources, btnMyBookmarks, btnRequestResource, btnNavUsers, btnNavReports, btnManageCategories, btnPendingRequests, btnSystemLogs, btnBackup, btnSettings}

        For Each btn In navButtons
            If _navPanelMap.ContainsKey(btn) Then
                Dim pnl As Panel = _navPanelMap(btn)
                Dim isActive As Boolean = (btn Is activeBtn)
                If isActive Then
                    pnl.BackColor = Color.FromArgb(38, 48, 78)
                    ' Highlight icon and text
                    For Each ctrl As Control In pnl.Controls
                        If TypeOf ctrl Is Label Then
                            Dim lbl As Label = DirectCast(ctrl, Label)
                            If lbl.Font.FontFamily.Name = "Segoe MDL2 Assets" Then
                                lbl.ForeColor = StyleHelper.AccentBlue
                            Else
                                lbl.ForeColor = Color.White
                            End If
                        End If
                    Next
                Else
                    pnl.BackColor = Color.Transparent
                    For Each ctrl As Control In pnl.Controls
                        If TypeOf ctrl Is Label Then
                            Dim lbl As Label = DirectCast(ctrl, Label)
                            If lbl.Font.FontFamily.Name = "Segoe MDL2 Assets" Then
                                lbl.ForeColor = Color.FromArgb(140, 160, 195)
                            Else
                                lbl.ForeColor = Color.FromArgb(195, 205, 225)
                            End If
                        End If
                    Next
                End If
            End If
        Next

        pnlSidebar.Invalidate()
    End Sub

    Private Sub frmDashboard_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        RepositionSidebarButtons()
    End Sub

    Private Sub RepositionSidebarButtons()
        Dim h As Integer = pnlSidebar.ClientSize.Height
        ' Reposition logout panel and divider to bottom
        If _pnlLogout IsNot Nothing Then _pnlLogout.Location = New Point(0, h - 42)
        If _divLogout IsNot Nothing Then _divLogout.Location = New Point(16, h - 52)
    End Sub

    Private Sub frmDashboard_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' If this is a logout, the login form is already visible — don't exit the app.
        ' If the user clicked the X button (not logout), exit the entire application.
        If Not _isLoggingOut Then
            Application.Exit()
        End If
    End Sub

    ' ---------------------------------------------------------------
    ' ROLE-BASED UI SETUP
    ' ---------------------------------------------------------------

    ''' <summary>Configures the dashboard controls based on the logged-in user's role.</summary>
    Private Sub SetupUIForRole()
        lblWelcome.Text = "Welcome, " & Session.CurrentUser.FullName
        If Session.IsGuest Then
            lblRole.Text = "Guest — browse only (sign in to bookmark)"
            Me.Text = "EduVault - Guest"
            SetNavItemVisible(btnMyBookmarks, False)
            SetNavItemVisible(btnRequestResource, False)
        Else
            lblRole.Text = "Role: " & Session.CurrentUser.Role
            Me.Text = "EduVault - " & Session.CurrentUser.FullName
            SetNavItemVisible(btnMyBookmarks, True)
            SetNavItemVisible(btnRequestResource, True)
        End If

        If Session.IsAdmin Then
            pnlStats.Visible = True
            SetNavItemVisible(btnNavUsers, True)
            SetNavItemVisible(btnNavReports, True)
            SetNavItemVisible(btnManageCategories, True)
            SetNavItemVisible(btnPendingRequests, True)
            SetNavItemVisible(btnSystemLogs, True)
            SetNavItemVisible(btnBackup, True)
            SetNavItemVisible(btnRequestResource, False)
            btnNewResource.Visible = True
            lblNavSectionAdmin.Visible = True
            lblNavSectionSystem.Visible = True
            If _divAdmin IsNot Nothing Then _divAdmin.Visible = True
            If _divSystem IsNot Nothing Then _divSystem.Visible = True
            _defaultDashboardTitle = "Admin Dashboard"
        Else
            pnlStats.Visible = False
            SetNavItemVisible(btnNavUsers, False)
            SetNavItemVisible(btnNavReports, False)
            SetNavItemVisible(btnManageCategories, False)
            SetNavItemVisible(btnPendingRequests, False)
            SetNavItemVisible(btnSystemLogs, False)
            SetNavItemVisible(btnBackup, False)
            SetNavItemVisible(btnRequestResource, True)
            btnNewResource.Visible = False
            lblNavSectionAdmin.Visible = False
            lblNavSectionSystem.Visible = False
            If _divAdmin IsNot Nothing Then _divAdmin.Visible = False
            If _divSystem IsNot Nothing Then _divSystem.Visible = False
            _defaultDashboardTitle = "Dashboard"
        End If

        lblDashboardTitle.Text = _defaultDashboardTitle
        PositionTopBarControls()
    End Sub

    ' ---------------------------------------------------------------
    ' NAV PANEL HELPERS
    ' ---------------------------------------------------------------

    ''' <summary>Shows or hides the nav panel associated with a button.</summary>
    Private Sub SetNavItemVisible(btn As Button, visible As Boolean)
        If _navPanelMap.ContainsKey(btn) Then
            _navPanelMap(btn).Visible = visible
        End If
    End Sub

    ' ---------------------------------------------------------------
    ' DATA LOADING
    ' ---------------------------------------------------------------

    Private Sub LoadDashboardData()
        Try
            LoadStatsPanel()
            LoadTrendingResources()
            If _middleWidgetReady Then LoadMiddleWidget()
            LoadCategoryBreakdown()
            LoadRecentActivity()
        Catch ex As Exception
            MessageBox.Show("Error loading dashboard data:" & Environment.NewLine & ex.Message,
                            "Dashboard Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    ''' <summary>Populates the admin stats panel with system-wide counts.</summary>
    Private Sub LoadStatsPanel()
        If Not Session.IsAdmin Then Return
        If _statCardValues Is Nothing OrElse _statCardValues.Length < 5 Then Return

        Dim stats As Dictionary(Of String, Integer) = _reportService.GetDashboardStats()

        Dim totalResources As Integer = If(stats.ContainsKey("TotalResources"), stats("TotalResources"), 0)
        Dim totalAccesses As Integer = If(stats.ContainsKey("TotalAccesses"), stats("TotalAccesses"), 0)
        Dim totalEbooks As Integer = If(stats.ContainsKey("TotalEbooks"), stats("TotalEbooks"), 0)
        Dim totalVideos As Integer = If(stats.ContainsKey("TotalVideos"), stats("TotalVideos"), 0)
        Dim popularCount As Integer = If(stats.ContainsKey("PopularResources"), stats("PopularResources"), 0)

        _statCardValues(0).Text = totalResources.ToString("N0")
        _statCardSubs(0).Text = If(totalResources = 1, "1 resource in library", $"{totalResources} resources in library")
        _statCardSubs(0).ForeColor = Color.DimGray

        _statCardValues(1).Text = totalAccesses.ToString("N0")
        _statCardSubs(1).Text = "All time"
        _statCardSubs(1).ForeColor = Color.DimGray

        _statCardValues(2).Text = totalEbooks.ToString("N0")
        _statCardSubs(2).Text = If(totalResources > 0, $"of {totalResources} resources", "No resources yet")
        _statCardSubs(2).ForeColor = StyleHelper.ValueGreen

        _statCardValues(3).Text = totalVideos.ToString("N0")
        _statCardSubs(3).Text = If(totalResources > 0, $"of {totalResources} resources", "No resources yet")
        _statCardSubs(3).ForeColor = StyleHelper.ValueAmber

        _statCardValues(4).Text = popularCount.ToString("N0")
        If popularCount = 0 Then
            _statCardSubs(4).Text = "No popular yet"
            _statCardSubs(4).ForeColor = Color.DimGray
        Else
            _statCardSubs(4).Text = If(popularCount = 1, "1 resource with 50+ views", $"{popularCount} resources with 50+ views")
            _statCardSubs(4).ForeColor = StyleHelper.ValueGreen
        End If
    End Sub

    ''' <summary>Loads the top 5 most-viewed resources into the trending grid.</summary>
    Private Sub LoadTrendingResources()
        Dim trending As List(Of Resource) = _resourceService.GetTrendingResources()

        dgvTrending.DataSource = Nothing
        dgvTrending.Rows.Clear()

        For Each res As Resource In trending
            Dim tag As String = _resourceService.GetEngagementTag(res)
            dgvTrending.Rows.Add(
                res.ResourceID,
                tag,
                res.Title,
                res.CategoryName,
                res.ResourceType,
                res.ViewCount,
                res.DateAdded.ToString("MMM dd, yyyy")
            )
        Next
    End Sub

    ''' <summary>Loads the data for the middle widget (Chart for Admin, Recent Grid for Student).</summary>
    Private Sub LoadMiddleWidget()
        If Not _middleWidgetReady Then Return
        If pnlMiddleWidget Is Nothing OrElse pnlMiddleWidget.Parent Is Nothing Then Return
        If pnlHost IsNot Nothing AndAlso pnlHost.Visible Then Return

        pnlMiddleWidget.Controls.Clear()

        If Session.IsAdmin Then
            ' Build Admin Chart
            Dim chart As New Chart()
            chart.Dock = DockStyle.Fill
            chart.BackColor = Color.White
            
            Dim area As New ChartArea()
            area.BackColor = Color.White
            area.AxisX.MajorGrid.LineColor = Color.LightGray
            area.AxisY.MajorGrid.LineColor = Color.LightGray
            area.AxisX.LabelStyle.Font = New Font("Segoe UI", 8)
            area.AxisY.LabelStyle.Font = New Font("Segoe UI", 8)
            chart.ChartAreas.Add(area)

            Dim series As New Series("Accesses")
            series.ChartType = SeriesChartType.Line
            series.BorderWidth = 3
            series.Color = StyleHelper.PrimaryColor
            series.MarkerStyle = MarkerStyle.Circle
            series.MarkerSize = 8
            
            Dim dt As DataTable = _reportService.GetDailyAccessCounts(7)
            If dt.Rows.Count = 0 Then
                series.Points.AddXY(DateTime.Today.ToString("MMM dd"), 0)
            Else
                For Each row As DataRow In dt.Rows
                    Dim day As Date = CDate(row("AccessDay"))
                    Dim count As Integer = CInt(row("AccessCount"))
                    series.Points.AddXY(day.ToString("MMM dd"), count)
                Next
            End If

            chart.Series.Add(series)
            
            Dim title As New Title("Accesses Over Last 7 Days")
            title.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            title.ForeColor = Color.DimGray
            title.Alignment = ContentAlignment.TopLeft
            chart.Titles.Add(title)

            pnlMiddleWidget.Controls.Add(chart)
        ElseIf Session.IsGuest Then
            Dim lblGuest As New Label() With {
                .Text = "You are browsing as a guest." & vbCrLf & vbCrLf &
                        "You can search and open resources, but bookmarks and personal history require signing in.",
                .Font = New Font("Segoe UI", 9.5),
                .ForeColor = Color.DimGray,
                .Location = New Point(16, 16),
                .Size = New Size(pnlMiddleWidget.Width - 32, pnlMiddleWidget.Height - 32),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
            }
            pnlMiddleWidget.Controls.Add(lblGuest)
        Else
            ' Build student panel: recently added library items
            Dim lblTitle As New Label()
            lblTitle.Text = "Recently added resources"
            lblTitle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lblTitle.ForeColor = StyleHelper.PrimaryColor
            lblTitle.Location = New Point(12, 12)
            lblTitle.AutoSize = True
            
            Dim dgv As New DataGridView()
            dgv.Location = New Point(12, 40)
            dgv.Size = New Size(pnlMiddleWidget.Width - 24, pnlMiddleWidget.Height - 50)
            dgv.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Bottom Or AnchorStyles.Right
            StyleHelper.ApplyGridStyle(dgv)
            dgv.RowTemplate.Height = 35
            dgv.ReadOnly = True
            dgv.AllowUserToAddRows = False
            dgv.RowHeadersVisible = False
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            
            dgv.Columns.Add("colTitle", "Title")
            dgv.Columns.Add("colCat", "Category")
            dgv.Columns.Add("colDate", "Added")

            Dim recent = _resourceService.GetRecentlyAdded(8)
            For Each res In recent
                dgv.Rows.Add(res.Title, res.CategoryName, res.DateAdded.ToString("MMM dd, yyyy"))
            Next
            
            pnlMiddleWidget.Controls.Add(dgv)
            pnlMiddleWidget.Controls.Add(lblTitle)
        End If
    End Sub

    ''' <summary>Loads recent access activity and sets live data onto the Activity Panel label.</summary>
    Private Sub LoadRecentActivity()
        ' Get total users
        Dim userCount As Integer = 0
        Try
            userCount = _userRepo.GetAllUsers().Count
        Catch
        End Try

        ' Get last resource added
        Dim lastAdded As String = "None"
        Try
            Dim allRes = _resourceService.GetAllResources()
            If allRes.Count > 0 Then
                lastAdded = allRes.OrderByDescending(Function(r) r.DateAdded).First().DateAdded.ToString("MMM dd, yyyy")
            End If
        Catch
        End Try

        ' Get last access (signed-in users only)
        Dim lastAccess As String = "None"
        If Not Session.IsGuest Then
            Try
                Dim history = _logRepo.GetUserAccessHistory(Session.CurrentUserID, 1)
                If history.Count > 0 Then
                    lastAccess = history.First().AccessDate.ToString("MMM dd, yyyy")
                End If
            Catch
            End Try
        End If

        lblActivityData.Text = $"Last resource added           {lastAdded}" & vbCrLf & vbCrLf &
                               $"Last access                        {lastAccess}" & vbCrLf & vbCrLf &
                               $"Total users                        {userCount}"
    End Sub

    ''' <summary>Loads live category counts and sets them onto the Category Breakdown label.</summary>
    Private Sub LoadCategoryBreakdown()
        Try
            Dim categories = _catService.GetAllCategories()
            
            ' Sort categories by resource count (descending) and take top 4
            Dim topCats = categories.OrderByDescending(Function(c) c.ResourceCount).Take(4).ToList()

            Dim sb As New Text.StringBuilder()
            For Each c In topCats
                ' Pad string to align nicely
                Dim catName = c.CategoryName
                If catName.Length > 20 Then catName = catName.Substring(0, 17) & "..."
                Dim paddedName = catName.PadRight(25)
                sb.AppendLine($"{paddedName} {c.ResourceCount} resource(s)")
                sb.AppendLine()
            Next

            If categories.Count > 4 Then
                sb.AppendLine("...")
            End If

            If sb.Length = 0 Then
                sb.AppendLine("No categories found.")
            End If

            lblCatBreakdown.Text = sb.ToString()
        Catch ex As Exception
            lblCatBreakdown.Text = "Error loading categories."
        End Try
    End Sub

    ' ---------------------------------------------------------------
    ' MENU NAVIGATION
    ' ---------------------------------------------------------------

    Private Sub btnNavResources_Click(sender As Object, e As EventArgs) Handles btnNavResources.Click
        UpdateNavActiveState(btnNavResources)
        ShowEmbeddedForm(New frmManageResources(), "Browse Resources",
                         "Double-click a resource row to open it in your browser.")
    End Sub

    Private Sub btnNewResource_Click(sender As Object, e As EventArgs) Handles btnNewResource.Click
        If Not Session.IsAdmin Then
            MessageBox.Show("Only Admins can add resources.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim frm As New frmAddEditResource(Nothing)
        If frm.ShowDialog(Me) = DialogResult.OK Then LoadDashboardData()
    End Sub

    Private Sub txtTopSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtTopSearch.KeyDown
        If e.KeyCode <> Keys.Enter Then Return

        Dim keyword As String = txtTopSearch.Text.Trim()
        If String.IsNullOrWhiteSpace(keyword) Then Return

        Dim frm As New frmManageResources()
        frm.InitialSearchKeyword = keyword
        UpdateNavActiveState(btnNavResources)
        ShowEmbeddedForm(frm, "Browse Resources",
                         "Double-click a resource row to open it in your browser.")
    End Sub

    Private Sub btnNavUsers_Click(sender As Object, e As EventArgs) Handles btnNavUsers.Click
        UpdateNavActiveState(btnNavUsers)
        If Not Session.IsAdmin Then
            MessageBox.Show("Access denied.", "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        ShowEmbeddedForm(New frmUserManagement(), "Manage Users",
                         "Select a user to edit account details or reset access.")
    End Sub

    Private Sub btnNavReports_Click(sender As Object, e As EventArgs) Handles btnNavReports.Click
        UpdateNavActiveState(btnNavReports)
        If Not Session.IsAdmin Then
            MessageBox.Show("Reports are available to Admins only.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        ShowEmbeddedForm(New frmReport(), "Reports",
                         "Choose a year and month, then click Generate Report to view access data.")
    End Sub

    Private Sub btnNavLogout_Click(sender As Object, e As EventArgs) Handles btnNavLogout.Click
        Dim result As DialogResult = MessageBox.Show(
            "Are you sure you want to log out?",
            "Logout - EduVault", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            _isLoggingOut = True

            Dim authService As New AuthService()
            authService.Logout()

            ' Re-show the original hidden login form (startup form) instead of
            ' creating a new instance, which would cause duplicate forms.
            For Each frm As Form In Application.OpenForms
                If TypeOf frm Is frmLogin Then
                    ' Clear any leftover credentials and re-show
                    Dim login As frmLogin = DirectCast(frm, frmLogin)
                    login.Show()
                    Exit For
                End If
            Next

            Me.Close()
        End If
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click
        UpdateNavActiveState(btnSettings)
        ShowEmbeddedForm(New frmSettings(), "Settings", "Manage your account security and preferences.")
    End Sub

    Private Sub pnlBell_Click(sender As Object, e As EventArgs)
        If Session.IsGuest Then
            MessageBox.Show("Sign in to view notifications.", "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Using frm As New frmNotifications()
            frm.ShowDialog(Me)
        End Using
        RefreshNotificationBadge()
    End Sub

    Private Sub RefreshNotificationBadge()
        If Session.IsGuest OrElse pnlBell Is Nothing Then Return
        Dim count As Integer = _v2Repo.GetUnreadNotificationCount(Session.CurrentUserID)
        pnlBell.Tag = count
        pnlBell.Invalidate()
    End Sub

    ' ---------------------------------------------------------------
    ' TRENDING GRID EVENTS
    ' ---------------------------------------------------------------

    ''' <summary>
    ''' Double-clicking a trending resource opens it in the default browser
    ''' and records a View access event.
    ''' </summary>
    Private Sub dgvTrending_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTrending.CellDoubleClick
        If e.RowIndex < 0 Then Return

        Try
            Dim resourceID As Integer = CInt(dgvTrending.Rows(e.RowIndex).Cells("colTrendID").Value)
            Dim res As Resource = _resourceService.GetResourceByID(resourceID)

            If res Is Nothing Then Return

            _resourceService.OpenResource(res, Session.CurrentUserID)
            LoadTrendingResources()
            If _middleWidgetReady Then LoadMiddleWidget()

        Catch ex As Exception
            MessageBox.Show("Error opening resource: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnBackup_Click(sender As Object, e As EventArgs) Handles btnBackup.Click
        UpdateNavActiveState(btnBackup)
        Using frm As New frmBackupSchedule()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnMyBookmarks_Click(sender As Object, e As EventArgs) Handles btnMyBookmarks.Click
        UpdateNavActiveState(btnMyBookmarks)
        ShowEmbeddedForm(New frmBookmarks(), "My Bookmarks",
                         "Double-click a bookmark to open the resource.")
    End Sub

    Private Sub btnSystemLogs_Click(sender As Object, e As EventArgs) Handles btnSystemLogs.Click
        UpdateNavActiveState(btnSystemLogs)
        ShowEmbeddedForm(New frmLogViewer(), "System Logs",
                         "Review recent sign-in and resource access activity.")
    End Sub

    ' ---------------------------------------------------------------
    ' CARD SHADOWS & POLISH
    ' ---------------------------------------------------------------

    Private Sub pnlStat_Paint(sender As Object, e As PaintEventArgs) Handles _
        pnlStatRes.Paint, pnlStatAcc.Paint, pnlStatEb.Paint, pnlStatVid.Paint, pnlStatPop.Paint
        StyleHelper.DrawCardShadow(sender, e)
    End Sub

    Private Sub lnkViewAll_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkViewAll.LinkClicked
        btnNavResources.PerformClick()
    End Sub

    ' ---------------------------------------------------------------
    ' REDESIGN IMPLEMENTATION
    ' ---------------------------------------------------------------
    Private lblNavSectionLibrary As New Label()
    Private lblNavSectionAdmin As New Label()
    Private lblNavSectionSystem As New Label()
    Private WithEvents btnNavDashboard As New Button()
    Private _navPanelMap As New Dictionary(Of Button, Panel)()  ' Maps hidden buttons to their visible nav panels
    Private _pnlLogout As Panel = Nothing
    Private _divLogout As Panel = Nothing
    Private _divAdmin As Panel = Nothing
    Private _divSystem As Panel = Nothing
    Private WithEvents pnlAvatar As New Panel()
    Private WithEvents pnlBell As New Panel()
    Private tlpStats As New TableLayoutPanel()
    Private _statCardValues As Label()
    Private _statCardSubs As Label()
    Private tlpBottom As New TableLayoutPanel()
    Private pnlCategoryBreakdown As New Panel()
    Private pnlRecentActivity As New Panel()
    Private lblCategoryTitle As New Label()
    Private lblActivityTitle2 As New Label()
    Private lblCatBreakdown As New Label()
    Private lblActivityData As New Label()

    Private pnlMiddleWidget As New Panel()

    ' SPA Variables
    Private pnlHost As New Panel()
    Private _currentEmbeddedForm As Form = Nothing

    Private WithEvents btnManageCategories As New Button()
    Private WithEvents btnPendingRequests As New Button()
    Private WithEvents btnRequestResource As New Button()

    Private Sub BuildRedesignedUI()
        pnlSidebar.Controls.Clear()
        _navPanelMap.Clear()

        pnlSidebar.BackColor = StyleHelper.SidebarColor

        ' ── BRAND HEADER ──
        Dim pnlBrand As New Panel With {
            .Location = New Point(0, 0),
            .Size = New Size(200, 80),
            .BackColor = Color.Transparent}

        Dim lblBrandIcon As New Label With {
            .Text = StyleHelper.IconLibrary,
            .Font = New Font("Segoe MDL2 Assets", 18),
            .ForeColor = StyleHelper.AccentBlue,
            .AutoSize = True,
            .Location = New Point(16, 18)}
        pnlBrand.Controls.Add(lblBrandIcon)

        lblSideTitle.Text = "EduVault"
        lblSideTitle.Font = New Font("Segoe UI", 13, FontStyle.Bold)
        lblSideTitle.ForeColor = Color.White
        lblSideTitle.Location = New Point(50, 16)
        lblSideTitle.AutoSize = True
        pnlBrand.Controls.Add(lblSideTitle)

        lblSideSubtitle.Text = "Resource Library"
        lblSideSubtitle.Font = New Font("Segoe UI", 7.5)
        lblSideSubtitle.ForeColor = Color.FromArgb(120, 140, 170)
        lblSideSubtitle.Location = New Point(52, 42)
        lblSideSubtitle.AutoSize = True
        pnlBrand.Controls.Add(lblSideSubtitle)

        ' Brand divider
        Dim divBrand As New Panel With {
            .Location = New Point(16, 68),
            .Size = New Size(168, 1),
            .BackColor = Color.FromArgb(50, 60, 90)}
        pnlBrand.Controls.Add(divBrand)

        pnlSidebar.Controls.Add(pnlBrand)

        ' ── SECTION HEADERS + DIVIDERS ──
        Dim CreateSection = Function(txt As String, y As Integer) As Object()
                                ' Thin divider above section
                                Dim div As New Panel With {
                                    .Location = New Point(16, y - 8),
                                    .Size = New Size(168, 1),
                                    .BackColor = Color.FromArgb(40, 50, 78)}
                                pnlSidebar.Controls.Add(div)

                                Dim lbl As New Label()
                                lbl.Text = txt
                                lbl.Font = New Font("Segoe UI", 7, FontStyle.Bold)
                                lbl.ForeColor = Color.FromArgb(90, 105, 135)
                                lbl.AutoSize = True
                                lbl.Location = New Point(16, y)
                                pnlSidebar.Controls.Add(lbl)
                                Return New Object() {lbl, div}
                            End Function

        Dim libResult = CreateSection("LIBRARY", 88)
        lblNavSectionLibrary = DirectCast(libResult(0), Label)
        Dim adminResult = CreateSection("ADMIN", 264)
        lblNavSectionAdmin = DirectCast(adminResult(0), Label)
        _divAdmin = DirectCast(adminResult(1), Panel)
        Dim sysResult = CreateSection("SYSTEM", 426)
        lblNavSectionSystem = DirectCast(sysResult(0), Label)
        _divSystem = DirectCast(sysResult(1), Panel)

        ' ── NAV ITEMS (Icon Label + Text Button in Panel) ──
        Dim navHoverBg As Color = Color.FromArgb(38, 48, 78)
        Dim navDefaultBg As Color = Color.Transparent

        Dim SetBtn = Sub(btn As Button, icon As String, txt As String, y As Integer)
                         ' Create a container panel for proper layout
                         Dim pnlNav As New Panel With {
                             .Location = New Point(0, y),
                             .Size = New Size(200, 36),
                             .BackColor = navDefaultBg,
                             .Cursor = Cursors.Hand}

                         ' Icon label — uses MDL2 font
                         Dim lblIcon As New Label With {
                             .Text = icon,
                             .Font = New Font("Segoe MDL2 Assets", 10),
                             .ForeColor = Color.FromArgb(140, 160, 195),
                             .AutoSize = False,
                             .Size = New Size(24, 24),
                             .Location = New Point(20, 7),
                             .TextAlign = ContentAlignment.MiddleCenter}
                         pnlNav.Controls.Add(lblIcon)

                         ' Text label — uses Segoe UI
                         Dim lblText As New Label With {
                             .Text = txt,
                             .Font = New Font("Segoe UI", 9.25),
                             .ForeColor = Color.FromArgb(195, 205, 225),
                             .AutoSize = True,
                             .Location = New Point(50, 9),
                             .Cursor = Cursors.Hand}
                         pnlNav.Controls.Add(lblText)

                         ' Forward clicks from the panel/labels to the real button
                         Dim onClick = Sub(s As Object, ev As EventArgs)
                                           btn.PerformClick()
                                       End Sub
                         AddHandler pnlNav.Click, onClick
                         AddHandler lblIcon.Click, onClick
                         AddHandler lblText.Click, onClick

                         ' Hover effect on panel + children
                         Dim onEnter = Sub(s As Object, ev As EventArgs)
                                           pnlNav.BackColor = navHoverBg
                                           lblText.ForeColor = Color.White
                                           lblIcon.ForeColor = StyleHelper.AccentBlue
                                       End Sub
                         Dim onLeave = Sub(s As Object, ev As EventArgs)
                                           pnlNav.BackColor = navDefaultBg
                                           lblText.ForeColor = Color.FromArgb(195, 205, 225)
                                           lblIcon.ForeColor = Color.FromArgb(140, 160, 195)
                                       End Sub
                         AddHandler pnlNav.MouseEnter, onEnter
                         AddHandler pnlNav.MouseLeave, onLeave
                         AddHandler lblIcon.MouseEnter, onEnter
                         AddHandler lblIcon.MouseLeave, onLeave
                         AddHandler lblText.MouseEnter, onEnter
                         AddHandler lblText.MouseLeave, onLeave

                         ' Keep button off-screen but visible (PerformClick needs Visible=True)
                         btn.Size = New Size(1, 1)
                         btn.Location = New Point(-10, -10)
                         pnlSidebar.Controls.Add(btn)
                         pnlSidebar.Controls.Add(pnlNav)

                         ' Register in nav panel map for active-state + visibility management
                         _navPanelMap(btn) = pnlNav
                     End Sub

        ' Library section
        SetBtn(btnNavDashboard, StyleHelper.IconHome, "Dashboard", 106)
        SetBtn(btnNavResources, StyleHelper.IconLibrary, "Browse Resources", 142)
        SetBtn(btnMyBookmarks, StyleHelper.IconBookmark, "My Bookmarks", 178)
        SetBtn(btnRequestResource, StyleHelper.IconAdd, "Request Resource", 214)

        ' Admin section
        SetBtn(btnNavUsers, StyleHelper.IconPeople, "Manage Users", 282)
        SetBtn(btnManageCategories, StyleHelper.IconLibrary, "Manage Categories", 318)
        SetBtn(btnPendingRequests, StyleHelper.IconPeople, "Pending Requests", 354)
        SetBtn(btnNavReports, StyleHelper.IconChart, "Reports", 390)

        ' System section
        SetBtn(btnBackup, StyleHelper.IconBackup, "Backup Database", 444)
        SetBtn(btnSystemLogs, StyleHelper.IconLogs, "System Logs", 480)
        SetBtn(btnSettings, StyleHelper.IconSettings, "Settings", 516)

        ' Logout — at the bottom with danger color
        _divLogout = New Panel With {
            .Location = New Point(16, 560),
            .Size = New Size(168, 1),
            .BackColor = Color.FromArgb(50, 60, 90)}
        pnlSidebar.Controls.Add(_divLogout)

        _pnlLogout = New Panel With {
            .Location = New Point(0, 570),
            .Size = New Size(200, 36),
            .BackColor = Color.Transparent,
            .Cursor = Cursors.Hand}

        Dim lblLogoutIcon As New Label With {
            .Text = StyleHelper.IconLogout,
            .Font = New Font("Segoe MDL2 Assets", 10),
            .ForeColor = Color.FromArgb(231, 76, 60),
            .AutoSize = False,
            .Size = New Size(24, 24),
            .Location = New Point(20, 7),
            .TextAlign = ContentAlignment.MiddleCenter}
        _pnlLogout.Controls.Add(lblLogoutIcon)

        Dim lblLogoutText As New Label With {
            .Text = "Logout",
            .Font = New Font("Segoe UI", 9.25),
            .ForeColor = Color.FromArgb(231, 76, 60),
            .AutoSize = True,
            .Location = New Point(50, 9),
            .Cursor = Cursors.Hand}
        _pnlLogout.Controls.Add(lblLogoutText)

        Dim onLogoutClick = Sub(s As Object, ev As EventArgs)
                                btnNavLogout.PerformClick()
                            End Sub
        AddHandler _pnlLogout.Click, onLogoutClick
        AddHandler lblLogoutIcon.Click, onLogoutClick
        AddHandler lblLogoutText.Click, onLogoutClick

        Dim logoutHoverBg As Color = Color.FromArgb(60, 30, 30)
        AddHandler _pnlLogout.MouseEnter, Sub() _pnlLogout.BackColor = logoutHoverBg
        AddHandler _pnlLogout.MouseLeave, Sub() _pnlLogout.BackColor = Color.Transparent
        AddHandler lblLogoutIcon.MouseEnter, Sub() _pnlLogout.BackColor = logoutHoverBg
        AddHandler lblLogoutIcon.MouseLeave, Sub() _pnlLogout.BackColor = Color.Transparent
        AddHandler lblLogoutText.MouseEnter, Sub() _pnlLogout.BackColor = logoutHoverBg
        AddHandler lblLogoutText.MouseLeave, Sub() _pnlLogout.BackColor = Color.Transparent

        btnNavLogout.Size = New Size(1, 1)
        btnNavLogout.Location = New Point(-10, -10)
        pnlSidebar.Controls.Add(btnNavLogout)
        pnlSidebar.Controls.Add(_pnlLogout)

        pnlTopBar.BackColor = StyleHelper.ContentBg
        pnlTopBar.Height = 64
        lblDashboardTitle.Text = "Dashboard"
        lblDashboardTitle.Font = New Font("Segoe UI", 14)
        lblDashboardTitle.ForeColor = Color.FromArgb(40, 40, 40)
        lblDashboardTitle.Location = New Point(20, 20)

        ' Modern Search Box wrapper
        Dim pnlSearch As New Panel()
        pnlSearch.Size = New Size(260, 30)
        pnlSearch.Location = New Point(pnlTopBar.Width - 380, 18)
        pnlSearch.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        pnlSearch.BackColor = Color.FromArgb(245, 247, 252)
        pnlSearch.Cursor = Cursors.IBeam
        
        Dim lblSearchIcon As New Label()
        lblSearchIcon.Text = StyleHelper.IconSearch
        lblSearchIcon.Font = New Font("Segoe MDL2 Assets", 10)
        lblSearchIcon.ForeColor = Color.FromArgb(140, 150, 170)
        lblSearchIcon.Location = New Point(8, 7)
        lblSearchIcon.AutoSize = True
        lblSearchIcon.Cursor = Cursors.IBeam

        Dim lblSearchText As New Label()
        lblSearchText.Text = "Search"
        lblSearchText.Font = New Font("Segoe UI", 9)
        lblSearchText.ForeColor = Color.FromArgb(140, 150, 170)
        lblSearchText.Location = New Point(lblSearchIcon.Right, 6)
        lblSearchText.AutoSize = True
        lblSearchText.Cursor = Cursors.IBeam

        pnlTopBar.Controls.Remove(txtTopSearch)
        txtTopSearch.BorderStyle = BorderStyle.None
        txtTopSearch.BackColor = pnlSearch.BackColor
        txtTopSearch.Location = New Point(lblSearchText.Right + 4, 7)
        txtTopSearch.Width = pnlSearch.Width - txtTopSearch.Left - 8
        txtTopSearch.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

        pnlSearch.Controls.Add(lblSearchIcon)
        pnlSearch.Controls.Add(lblSearchText)
        pnlSearch.Controls.Add(txtTopSearch)
        pnlTopBar.Controls.Add(pnlSearch)
        
        AddHandler pnlSearch.Paint, Sub(s, ev)
            ControlPaint.DrawBorder(ev.Graphics, pnlSearch.ClientRectangle, Color.FromArgb(220, 225, 235), ButtonBorderStyle.Solid)
        End Sub
        
        Dim focusAction = Sub() txtTopSearch.Focus()
        AddHandler pnlSearch.Click, focusAction
        AddHandler lblSearchIcon.Click, focusAction
        AddHandler lblSearchText.Click, focusAction

        lblWelcome.Visible = False
        lblRole.Visible = False

        pnlBell.Size = New Size(28, 28)
        pnlBell.Location = New Point(pnlTopBar.Width - 88, 18)
        pnlBell.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        pnlBell.Cursor = Cursors.Hand
        pnlTopBar.Controls.Add(pnlBell)

        pnlAvatar.Size = New Size(32, 32)
        pnlAvatar.Location = New Point(pnlTopBar.Width - 50, 16)
        pnlAvatar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        pnlAvatar.Cursor = Cursors.Hand
        pnlTopBar.Controls.Add(pnlAvatar)

        pnlStats.Controls.Clear()
        pnlStats.Height = 120
        pnlStats.Padding = New Padding(16, 16, 16, 0)
        tlpStats.Dock = DockStyle.Fill
        tlpStats.ColumnCount = 5
        tlpStats.RowCount = 1
        For i = 0 To 4
            tlpStats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0!))
        Next
        pnlStats.Controls.Add(tlpStats)

        _statCardValues = New Label(4) {}
        _statCardSubs = New Label(4) {}

        Dim BuildNewCard = Sub(col As Integer, title As String)
                               Dim rightMargin As Integer = If(col = 4, 0, 12)
                               Dim card As New Panel() With {.Dock = DockStyle.Fill, .Margin = New Padding(0, 0, rightMargin, 0), .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
                               Dim lblT As New Label() With {.Text = title, .ForeColor = Color.DimGray, .Font = New Font("Segoe UI", 8.5), .AutoSize = True, .Location = New Point(12, 12)}
                               Dim lblV As New Label() With {.Text = "—", .ForeColor = StyleHelper.PrimaryColor, .Font = New Font("Segoe UI", 24), .AutoSize = True, .Location = New Point(10, 32)}
                               Dim lblS As New Label() With {.Text = String.Empty, .ForeColor = Color.DimGray, .Font = New Font("Segoe UI", 8), .AutoSize = True, .Location = New Point(14, 76)}
                               card.Controls.Add(lblT)
                               card.Controls.Add(lblV)
                               card.Controls.Add(lblS)
                               tlpStats.Controls.Add(card, col, 0)
                               _statCardValues(col) = lblV
                               _statCardSubs(col) = lblS
                           End Sub

        BuildNewCard(0, "Total Resources")
        BuildNewCard(1, "Total Accesses")
        BuildNewCard(2, "E-Books")
        BuildNewCard(3, "Videos")
        BuildNewCard(4, "Popular (>50 views)")

        LoadStatsPanel()

        lblTrendTitle.Text = StyleHelper.IconTrending & " Trending Resources"
        lblTrendTitle.Font = New Font("Segoe UI", 11)
        lnkViewAll.Location = New Point(dgvTrending.Right - lnkViewAll.Width, lblTrendTitle.Top)

        Dim lblTop5 As New Label() With {.Text = "Top 5", .BackColor = StyleHelper.PillBlueBg, .ForeColor = StyleHelper.PillBlueText, .Font = New Font("Segoe UI", 7, FontStyle.Bold), .Location = New Point(lblTrendTitle.Right + 10, lblTrendTitle.Top + 2), .AutoSize = True}
        pnlContent.Controls.Add(lblTop5)

        dgvTrending.BackgroundColor = Color.White
        dgvTrending.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvTrending.GridColor = Color.FromArgb(240, 240, 240)
        dgvTrending.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvTrending.EnableHeadersVisualStyles = False
        dgvTrending.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        dgvTrending.ColumnHeadersDefaultCellStyle.ForeColor = Color.DimGray
        dgvTrending.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 8, FontStyle.Bold)
        dgvTrending.AlternatingRowsDefaultCellStyle.BackColor = Color.White

        If dgvTrending.Columns.Contains("colActions") Then
            dgvTrending.Columns.Remove("colActions")
        End If

        For Each col As DataGridViewColumn In dgvTrending.Columns
            col.HeaderText = col.HeaderText.ToUpper()
        Next

        tlpBottom.ColumnCount = 2
        tlpBottom.RowCount = 1
        tlpBottom.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
        tlpBottom.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
        tlpBottom.Dock = DockStyle.Bottom
        tlpBottom.Height = 130
        tlpBottom.Padding = New Padding(16, 10, 16, 10)

        pnlCategoryBreakdown.Dock = DockStyle.Fill
        pnlCategoryBreakdown.Margin = New Padding(0, 0, 8, 0)
        pnlCategoryBreakdown.BackColor = Color.White
        pnlCategoryBreakdown.BorderStyle = BorderStyle.FixedSingle

        lblCategoryTitle.Text = StyleHelper.IconLibrary & " Category Breakdown"
        lblCategoryTitle.Font = New Font("Segoe UI", 10)
        lblCategoryTitle.Location = New Point(16, 16)
        lblCategoryTitle.AutoSize = True

        lblCatBreakdown.Text = "Mathematics           1 resource" & vbCrLf & vbCrLf & "Social Sciences        1 resource" & vbCrLf & vbCrLf & vbCrLf & "More categories will appear as resources are added."
        lblCatBreakdown.Location = New Point(16, 50)
        lblCatBreakdown.AutoSize = True
        lblCatBreakdown.Font = New Font("Segoe UI", 9)

        pnlCategoryBreakdown.Controls.Add(lblCategoryTitle)
        pnlCategoryBreakdown.Controls.Add(lblCatBreakdown)

        pnlRecentActivity.Dock = DockStyle.Fill
        pnlRecentActivity.Margin = New Padding(8, 0, 0, 0)
        pnlRecentActivity.BackColor = Color.White
        pnlRecentActivity.BorderStyle = BorderStyle.FixedSingle

        lblActivityTitle2.Text = StyleHelper.IconLogs & " Recent Activity"
        lblActivityTitle2.Font = New Font("Segoe UI", 10)
        lblActivityTitle2.Location = New Point(16, 16)
        lblActivityTitle2.AutoSize = True

        lblActivityData.Text = "Last resource added           May 06, 2026" & vbCrLf & vbCrLf & "Last access                        May 06, 2026" & vbCrLf & vbCrLf & "Total users                        —"
        lblActivityData.Location = New Point(16, 50)
        lblActivityData.AutoSize = True
        lblActivityData.Font = New Font("Segoe UI", 9)

        pnlRecentActivity.Controls.Add(lblActivityTitle2)
        pnlRecentActivity.Controls.Add(lblActivityData)

        tlpBottom.Controls.Add(pnlCategoryBreakdown, 0, 0)
        tlpBottom.Controls.Add(pnlRecentActivity, 1, 0)

        pnlContent.Controls.Add(tlpBottom)

        pnlContent.Controls.Remove(lblActivityTitle)
        pnlContent.Controls.Remove(lstActivity)

        ' Create a TableLayoutPanel for the middle section (Trending + Chart)
        Dim tlpMiddle As New TableLayoutPanel()
        tlpMiddle.ColumnCount = 2
        tlpMiddle.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 55.0!))
        tlpMiddle.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 45.0!))
        tlpMiddle.RowCount = 1
        tlpMiddle.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        tlpMiddle.Dock = DockStyle.Fill
        tlpMiddle.Padding = New Padding(16, 12, 16, 12)

        ' Create the Trending Box
        Dim pnlTrendingBox As New Panel()
        pnlTrendingBox.Dock = DockStyle.Fill
        pnlTrendingBox.Margin = New Padding(0, 0, 8, 0)
        pnlTrendingBox.BackColor = Color.White
        pnlTrendingBox.BorderStyle = BorderStyle.FixedSingle

        ' Move existing controls to pnlTrendingBox
        pnlContent.Controls.Remove(lblTrendTitle)
        pnlContent.Controls.Remove(lblTop5)
        pnlContent.Controls.Remove(lnkViewAll)
        pnlContent.Controls.Remove(dgvTrending)

        pnlTrendingBox.Controls.Add(lblTrendTitle)
        pnlTrendingBox.Controls.Add(lblTop5)
        pnlTrendingBox.Controls.Add(lnkViewAll)
        pnlTrendingBox.Controls.Add(dgvTrending)

        lblTrendTitle.Location = New Point(16, 16)
        lblTop5.Location = New Point(lblTrendTitle.Right + 10, 18)

        btnNewResource.Text = "➕ New Resource"
        btnNewResource.BackColor = StyleHelper.ValueGreen
        btnNewResource.ForeColor = Color.White
        btnNewResource.FlatStyle = FlatStyle.Flat
        btnNewResource.FlatAppearance.BorderSize = 0
        btnNewResource.TextAlign = ContentAlignment.MiddleCenter
        btnNewResource.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnNewResource.Size = New Size(140, 32)
        btnNewResource.Cursor = Cursors.Hand
        pnlTrendingBox.Controls.Add(btnNewResource)
        
        AddHandler pnlTrendingBox.Resize,
            Sub()
                btnNewResource.Location = New Point(pnlTrendingBox.Width - 156, 12)
                lnkViewAll.Location = New Point(btnNewResource.Left - lnkViewAll.Width - 16, 20)
                dgvTrending.Location = New Point(16, 56)
                dgvTrending.Size = New Size(pnlTrendingBox.Width - 32, pnlTrendingBox.Height - 72)
            End Sub

        dgvTrending.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        dgvTrending.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        ' Setup Middle Widget Panel (Chart/Recently Added)
        pnlMiddleWidget.Dock = DockStyle.Fill
        pnlMiddleWidget.Margin = New Padding(8, 0, 0, 0)
        pnlMiddleWidget.BackColor = Color.White
        pnlMiddleWidget.BorderStyle = BorderStyle.FixedSingle

        tlpMiddle.Controls.Add(pnlTrendingBox, 0, 0)
        tlpMiddle.Controls.Add(pnlMiddleWidget, 1, 0)

        pnlContent.Controls.Add(tlpMiddle)
        tlpMiddle.BringToFront()

        ' ── GUEST BANNER ──
        If Session.IsGuest Then
            Dim pnlGuestBanner As New Panel()
            pnlGuestBanner.Dock = DockStyle.Top
            pnlGuestBanner.Height = 36
            pnlGuestBanner.BackColor = StyleHelper.GuestBannerBg
            Dim lblBanner As New Label()
            lblBanner.Text = ChrW(&HE7BA) & "  Browsing as guest " & ChrW(&H2014) & " bookmarks won't be saved. Sign in for full access."
            lblBanner.Font = New Font("Segoe UI", 9)
            lblBanner.ForeColor = StyleHelper.GuestBannerFg
            lblBanner.AutoSize = True
            lblBanner.Location = New Point(16, 9)
            pnlGuestBanner.Controls.Add(lblBanner)
            pnlContent.Controls.Add(pnlGuestBanner)
            pnlGuestBanner.BringToFront()
        End If

        ' SPA Host Panel
        pnlHost.Dock = DockStyle.Fill
        pnlHost.BackColor = StyleHelper.ContentBg
        pnlHost.Visible = False
        Me.Controls.Add(pnlHost)
        pnlHost.BringToFront()

        pnlHost.BackColor = StyleHelper.ContentBg

        PositionTopBarControls()

        AddHandler pnlBell.Paint, AddressOf pnlBell_Paint
        AddHandler pnlAvatar.Paint, AddressOf pnlAvatar_Paint
        AddHandler pnlAvatar.Click, AddressOf pnlAvatar_Click
        AddHandler pnlContent.Resize,
            Sub()
                If _middleWidgetReady AndAlso pnlHost IsNot Nothing AndAlso Not pnlHost.Visible Then
                    LoadMiddleWidget()
                End If
            End Sub
        AddHandler dgvTrending.CellPainting, AddressOf dgvTrending_CellPainting
        AddHandler btnNavDashboard.Click, AddressOf ShowDashboardHome
    End Sub

    Private Sub ShowDashboardHome()
        UpdateNavActiveState(btnNavDashboard)
        ReturnToDashboard()
    End Sub

    Private Sub btnManageCategories_Click(sender As Object, e As EventArgs) Handles btnManageCategories.Click
        UpdateNavActiveState(btnManageCategories)
        If Not Session.IsAdmin Then Return
        ShowEmbeddedForm(New frmCategoryManagement(), "Category Management", "Add, edit, or remove resource categories.")
    End Sub

    Private Sub btnPendingRequests_Click(sender As Object, e As EventArgs) Handles btnPendingRequests.Click
        UpdateNavActiveState(btnPendingRequests)
        If Not Session.IsAdmin Then Return
        ShowEmbeddedForm(New frmPendingRequests(), "Pending Requests", "Review user requests for new resources.")
    End Sub

    Private Sub btnRequestResource_Click(sender As Object, e As EventArgs) Handles btnRequestResource.Click
        UpdateNavActiveState(btnRequestResource)
        If Session.IsGuest Then Return
        ShowEmbeddedForm(New frmResourceRequest(), "Request a Resource", "Submit a request to admins for a new library item.")
    End Sub

    Private Sub pnlBell_Paint(sender As Object, e As PaintEventArgs)
        e.Graphics.DrawString("🔔", New Font("Segoe UI", 12), Brushes.DimGray, 2, 4)
        Dim count As Integer = 0
        If pnlBell.Tag IsNot Nothing Then Integer.TryParse(pnlBell.Tag.ToString(), count)
        If count > 0 Then
            Dim badge As String = If(count > 9, "9+", count.ToString())
            e.Graphics.FillEllipse(Brushes.IndianRed, 14, 0, 14, 14)
            e.Graphics.DrawString(badge, New Font("Segoe UI", 7, FontStyle.Bold), Brushes.White, 15, 0)
        End If
    End Sub

    Private Sub pnlAvatar_Paint(sender As Object, e As PaintEventArgs)
        Dim initials As String = "U"
        If Session.CurrentUser IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(Session.CurrentUser.FullName) Then
            Dim parts = Session.CurrentUser.FullName.Split(" "c)
            If parts.Length > 1 Then
                initials = (parts(0)(0).ToString() & parts(parts.Length - 1)(0).ToString()).ToUpper()
            Else
                initials = parts(0).Substring(0, Math.Min(2, parts(0).Length)).ToUpper()
            End If
        End If
        StyleHelper.DrawAvatar(e.Graphics, pnlAvatar.ClientRectangle, initials)
    End Sub

    Private Sub pnlAvatar_Click(sender As Object, e As EventArgs)
        btnSettings.PerformClick()
    End Sub

    Private Sub dgvTrending_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 Then Return

        If e.ColumnIndex = dgvTrending.Columns("colTrendTag").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            Dim rect As New Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 8, Math.Min(50, e.CellBounds.Width - 8), e.CellBounds.Height - 16)
            If e.Value.ToString() = "NEW" Then
                StyleHelper.DrawPill(e.Graphics, rect, "NEW", StyleHelper.PillGreenBg, StyleHelper.PillGreenText, New Font("Segoe UI", 7, FontStyle.Bold))
            Else
                TextRenderer.DrawText(e.Graphics, e.Value.ToString(), e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter)
            End If
            e.Handled = True
        End If

        If e.ColumnIndex = dgvTrending.Columns("colTrendType").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            Dim rect As New Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 8, Math.Min(60, e.CellBounds.Width - 8), e.CellBounds.Height - 16)
            Dim valStr As String = e.Value.ToString()
            If valStr = "E-Book" Then
                StyleHelper.DrawPill(e.Graphics, rect, valStr, StyleHelper.PillBlueBg, StyleHelper.PillBlueText, New Font("Segoe UI", 7, FontStyle.Bold))
            ElseIf valStr = "Video" Then
                StyleHelper.DrawPill(e.Graphics, rect, valStr, StyleHelper.PillPinkBg, StyleHelper.PillPinkText, New Font("Segoe UI", 7, FontStyle.Bold))
            Else
                StyleHelper.DrawPill(e.Graphics, rect, valStr, Color.FromArgb(240, 240, 240), Color.DimGray, New Font("Segoe UI", 7, FontStyle.Bold))
            End If
            e.Handled = True
        End If

        If e.ColumnIndex = dgvTrending.Columns("colTrendViews").Index AndAlso e.Value IsNot Nothing Then
            e.PaintBackground(e.CellBounds, True)
            Dim val As Integer = 0
            If Integer.TryParse(e.Value.ToString(), val) Then
                TextRenderer.DrawText(e.Graphics, val.ToString(), e.CellStyle.Font, New Rectangle(e.CellBounds.X, e.CellBounds.Y, 20, e.CellBounds.Height), e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter Or TextFormatFlags.Left)
                Dim barRect As New Rectangle(e.CellBounds.X + 25, e.CellBounds.Y + (e.CellBounds.Height \ 2) - 2, 30, 4)
                StyleHelper.DrawMiniBar(e.Graphics, barRect, val, 10, StyleHelper.PillBlueText)
            End If
            e.Handled = True
        End If

    End Sub

    Private Sub pnlSidebar_Paint(sender As Object, e As PaintEventArgs) Handles pnlSidebar.Paint
        For Each ctrl As Control In pnlSidebar.Controls
            If TypeOf ctrl Is Button AndAlso ctrl.BackColor = Color.FromArgb(33, 53, 84) Then
                ' Active button tint border
                Using brush As New SolidBrush(Color.FromArgb(41, 128, 185))
                    e.Graphics.FillRectangle(brush, New Rectangle(ctrl.Left, ctrl.Top, 4, ctrl.Height))
                End Using
            End If
        Next
    End Sub

    ' ---------------------------------------------------------------
    ' SPA ENGINE
    ' ---------------------------------------------------------------

    Public Sub ShowEmbeddedForm(frm As Form,
                                Optional pageTitle As String = Nothing,
                                Optional statusHint As String = Nothing)
        ' Hide home widgets
        pnlStats.Visible = False
        pnlContent.Visible = False

        If Not String.IsNullOrWhiteSpace(pageTitle) Then
            lblDashboardTitle.Text = pageTitle
            PositionTopBarControls()
        End If

        If Not String.IsNullOrWhiteSpace(statusHint) Then
            tsslInfo.Text = $"{DefaultStatusHint}  -  {statusHint}"
        End If

        If _currentEmbeddedForm IsNot Nothing Then
            _currentEmbeddedForm.Close()
            _currentEmbeddedForm.Dispose()
        End If

        frm.TopLevel = False
        frm.FormBorderStyle = FormBorderStyle.None
        frm.Dock = DockStyle.Fill
        
        pnlHost.Visible = True
        pnlHost.Controls.Add(frm)
        frm.Show()
        _currentEmbeddedForm = frm

        ' ── EMBEDDED CHROME FIX ──
        ' Hide the embedded form's own header to prevent double-header effect
        Dim embeddedHeader As Panel = TryCast(frm.Controls("pnlHeader"), Panel)
        If embeddedHeader IsNot Nothing Then
            embeddedHeader.Visible = False
        End If
    End Sub

    Public Sub ReturnToDashboard()
        If _currentEmbeddedForm IsNot Nothing Then
            _currentEmbeddedForm.Close()
            _currentEmbeddedForm.Dispose()
            _currentEmbeddedForm = Nothing
        End If
        pnlHost.Visible = False
        lblDashboardTitle.Text = _defaultDashboardTitle
        PositionTopBarControls()
        tsslInfo.Text = $"{DefaultStatusHint}  -  Double-click a trending resource to open it."
        pnlStats.Visible = Session.IsAdmin
        pnlContent.Visible = True
        LoadDashboardData()
    End Sub

End Class
