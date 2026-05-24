''' <summary>
''' frmLogin - EduVault Login Form (Presentation Layer)
''' Authenticates users and routes them to the dashboard based on their role.
''' Controls: txtUsername, txtPassword, btnLogin, lblStatus, chkShowPassword
''' </summary>
Public Class frmLogin

    Private ReadOnly _authService As New AuthService()

    ' ─────────────────────────────────────────────────────────────
    ' FORM EVENTS
    ' ─────────────────────────────────────────────────────────────

    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "EduVault - Login"
        lblStatus.Text = String.Empty

        ' Verify DB connectivity on startup
        If Not DatabaseHelper.TestConnection() Then
            lblStatus.ForeColor = Color.Red
            lblStatus.Text = "[!] Cannot connect to database. Check App.config and SQL Server."
            btnLogin.Enabled = False
        End If

        BuildRedesignedUI()
        Me.ActiveControl = Nothing
    End Sub

    Private Sub ApplyStyles()
        pnlLeft.BackColor = StyleHelper.SidebarColor
        lblBrand.Font     = New Font("Segoe UI Variable Display", 26, FontStyle.Bold)
        lblBrandSub.Font  = StyleHelper.SubHeaderFont
        lblWelcome.Font    = StyleHelper.HeaderFont
        lblWelcome.ForeColor = StyleHelper.PrimaryColor
        lblWelcomeSub.Font = StyleHelper.NormalFont
        
        StyleHelper.ApplyButtonStyle(btnLogin)
        lblStatus.Font     = StyleHelper.SmallFont

        ' Apply Modern Input Styles & Placeholders
        StyleHelper.ApplyModernInputStyle(txtUsername)
        StyleHelper.ApplyModernInputStyle(txtPassword)
        StyleHelper.SetPlaceholder(txtUsername, "Enter your username")
        StyleHelper.SetPlaceholder(txtPassword, "Enter your password")
    End Sub

    Private Sub frmLogin_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        ' When the form is re-shown after a logout, clear old credentials
        If Me.Visible Then
            txtUsername.Clear()
            txtPassword.Clear()
            lblStatus.Text = String.Empty
            txtUsername.Focus()
        End If
    End Sub

    Private Sub frmLogin_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' Allow pressing Enter from anywhere on the form to trigger login
        If e.KeyCode = Keys.Enter Then PerformLogin()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' CONTROL EVENTS
    ' ─────────────────────────────────────────────────────────────

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        PerformLogin()
    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
        ' Toggle password masking
        If chkShowPassword.Checked Then
            txtPassword.UseSystemPasswordChar = False
            txtPassword.PasswordChar = Chr(0)
        Else
            txtPassword.UseSystemPasswordChar = False
            txtPassword.PasswordChar = "•"c
        End If
    End Sub

    Private Sub lnkForgotPassword_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Dim forgotForm As New frmForgotPassword()
        forgotForm.ShowDialog()
    End Sub

    Private Sub lnkSignUp_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Dim signUpForm As New frmRegister()
        signUpForm.ShowDialog()
    End Sub

    Private Sub txtUsername_TextChanged(sender As Object, e As EventArgs) Handles txtUsername.TextChanged
        ClearStatus()
    End Sub

    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged
        ClearStatus()
    End Sub

    ' ── FOCUS STATES ──

    Private Sub picEye_MouseUp(sender As Object, e As MouseEventArgs)
        txtPassword.UseSystemPasswordChar = True
    End Sub

    ' ---------------------------------------------------------------
    ' REDESIGN IMPLEMENTATION
    ' ---------------------------------------------------------------
    Private pnlLeftNew As New Panel()
    Private pnlRightNew As New Panel()
    Private webIllustration As New WebBrowser()
    
    Private pnlUserWrap As New Panel()
    Private pnlPassWrap As New Panel()
    Private btnGuest As New Button()
    
    Private Sub BuildRedesignedUI()
        Me.Controls.Clear()
        
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.ClientSize = New Size(600, 460)
        Me.BackColor = Color.White
        
        ' --- LEFT PANEL ---
        pnlLeftNew.Width = 210
        pnlLeftNew.Dock = DockStyle.Left
        pnlLeftNew.BackColor = Color.FromArgb(17, 24, 39) ' #111827
        Me.Controls.Add(pnlLeftNew)
        
        webIllustration.Dock = DockStyle.Fill
        webIllustration.ScrollBarsEnabled = False
        webIllustration.IsWebBrowserContextMenuEnabled = False
        webIllustration.AllowNavigation = False
        webIllustration.AllowWebBrowserDrop = False
        webIllustration.ScriptErrorsSuppressed = True
        pnlLeftNew.Controls.Add(webIllustration)
        
        LoadIllustration()
        
        ' --- RIGHT PANEL ---
        pnlRightNew.Dock = DockStyle.Fill
        pnlRightNew.BackColor = Color.White
        Me.Controls.Add(pnlRightNew)
        pnlRightNew.BringToFront()
        
        Dim lblWelcomeNew As New Label() With {.Text = "Welcome back", .Font = New Font("Segoe UI", 18, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 40, 40), .Location = New Point(32, 32), .AutoSize = True}
        Dim lblWelcomeSubNew As New Label() With {.Text = "Sign in to continue to EduVault", .Font = New Font("Segoe UI", 11), .ForeColor = Color.Gray, .Location = New Point(32, 64), .AutoSize = True}
        
        pnlRightNew.Controls.Add(lblWelcomeNew)
        pnlRightNew.Controls.Add(lblWelcomeSubNew)
        
        ' --- USERNAME FIELD ---
        Dim lblUserTitle As New Label() With {.Text = "USERNAME OR EMAIL", .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .ForeColor = Color.DimGray, .Location = New Point(32, 110), .AutoSize = True}
        pnlRightNew.Controls.Add(lblUserTitle)
        
        pnlUserWrap.Location = New Point(32, 130)
        pnlUserWrap.Size = New Size(320, 34)
        pnlUserWrap.BackColor = Color.FromArgb(245, 245, 245)
        pnlUserWrap.BorderStyle = BorderStyle.FixedSingle
        pnlRightNew.Controls.Add(pnlUserWrap)
        
        Dim lblUserIcon As New Label() With {.Text = "👤", .Font = New Font("Segoe UI Emoji", 10), .ForeColor = Color.Gray, .Location = New Point(8, 7), .AutoSize = True}
        pnlUserWrap.Controls.Add(lblUserIcon)
        
        txtUsername.Location = New Point(36, 6)
        txtUsername.Size = New Size(274, 20)
        txtUsername.BorderStyle = BorderStyle.None
        txtUsername.BackColor = Color.FromArgb(245, 245, 245)
        txtUsername.Font = New Font("Segoe UI", 10.5)
        StyleHelper.SetPlaceholder(txtUsername, "Enter your username")
        pnlUserWrap.Controls.Add(txtUsername)
        
        ' --- PASSWORD FIELD ---
        Dim lblPassTitle As New Label() With {.Text = "PASSWORD", .Font = New Font("Segoe UI", 9.5, FontStyle.Bold), .ForeColor = Color.DimGray, .Location = New Point(32, 174), .AutoSize = True}
        pnlRightNew.Controls.Add(lblPassTitle)
        
        pnlPassWrap.Location = New Point(32, 194)
        pnlPassWrap.Size = New Size(320, 34)
        pnlPassWrap.BackColor = Color.FromArgb(245, 245, 245)
        pnlPassWrap.BorderStyle = BorderStyle.FixedSingle
        pnlRightNew.Controls.Add(pnlPassWrap)
        
        Dim lblPassIcon As New Label() With {.Text = "🔒", .Font = New Font("Segoe UI Emoji", 10), .ForeColor = Color.Gray, .Location = New Point(8, 7), .AutoSize = True}
        pnlPassWrap.Controls.Add(lblPassIcon)
        
        txtPassword.Location = New Point(36, 6)
        txtPassword.Size = New Size(274, 20)
        txtPassword.BorderStyle = BorderStyle.None
        txtPassword.BackColor = Color.FromArgb(245, 245, 245)
        txtPassword.Font = New Font("Segoe UI", 10.5)
        txtPassword.UseSystemPasswordChar = False
        txtPassword.PasswordChar = "•"c
        StyleHelper.SetPlaceholder(txtPassword, "Enter your password")
        pnlPassWrap.Controls.Add(txtPassword)
        
        ' --- OPTIONS ROW ---
        chkShowPassword.Location = New Point(32, 238)
        chkShowPassword.AutoSize = True
        chkShowPassword.Font = New Font("Segoe UI", 9.5)
        chkShowPassword.ForeColor = Color.DimGray
        pnlRightNew.Controls.Add(chkShowPassword)
        
        lnkForgotPassword.Location = New Point(236, 240)
        lnkForgotPassword.AutoSize = True
        lnkForgotPassword.Font = New Font("Segoe UI", 9.5)
        lnkForgotPassword.LinkArea = New LinkArea(0, lnkForgotPassword.Text.Length)
        lnkForgotPassword.Cursor = Cursors.Hand
        AddHandler lnkForgotPassword.LinkClicked, AddressOf lnkForgotPassword_LinkClicked
        pnlRightNew.Controls.Add(lnkForgotPassword)
        
        ' --- BUTTONS ---
        btnLogin.Location = New Point(32, 270)
        btnLogin.Size = New Size(320, 36)
        btnLogin.Text = "Login →"
        btnLogin.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        pnlRightNew.Controls.Add(btnLogin)
        
        lblStatus.Location = New Point(32, 310)
        lblStatus.Size = New Size(320, 36)
        lblStatus.AutoSize = False
        pnlRightNew.Controls.Add(lblStatus)
        
        Dim pnlDiv As New FlowLayoutPanel() With {.Location = New Point(32, 356), .Size = New Size(320, 16), .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False}
        Dim l1 As New Label With {.Text = "───────────────", .ForeColor = Color.LightGray, .AutoSize = True, .Margin = New Padding(0, 0, 0, 0)}
        Dim l2 As New Label With {.Text = " or ", .ForeColor = Color.Gray, .AutoSize = True, .Font = New Font("Segoe UI", 9), .Margin = New Padding(0, -2, 0, 0)}
        Dim l3 As New Label With {.Text = "───────────────", .ForeColor = Color.LightGray, .AutoSize = True, .Margin = New Padding(0, 0, 0, 0)}
        pnlDiv.Controls.Add(l1)
        pnlDiv.Controls.Add(l2)
        pnlDiv.Controls.Add(l3)
        pnlRightNew.Controls.Add(pnlDiv)
        
        btnGuest.Location = New Point(32, 382)
        btnGuest.Size = New Size(320, 32)
        btnGuest.Text = "👁 Browse as Guest"
        btnGuest.FlatStyle = FlatStyle.Flat
        btnGuest.FlatAppearance.BorderColor = Color.LightGray
        btnGuest.BackColor = Color.Transparent
        btnGuest.ForeColor = Color.DimGray
        btnGuest.Font = New Font("Segoe UI", 10)
        btnGuest.Cursor = Cursors.Hand
        AddHandler btnGuest.Click, AddressOf btnGuest_Click
        pnlRightNew.Controls.Add(btnGuest)
        
        lnkSignUp.Location = New Point(100, 426)
        lnkSignUp.Font = New Font("Segoe UI", 9.5)
        lnkSignUp.AutoSize = True
        lnkSignUp.LinkArea = New LinkArea(0, lnkSignUp.Text.Length)
        lnkSignUp.Cursor = Cursors.Hand
        AddHandler lnkSignUp.LinkClicked, AddressOf lnkSignUp_LinkClicked
        pnlRightNew.Controls.Add(lnkSignUp)
        

        
        ' Hook focus events for styling
        AddHandler txtUsername.Enter, AddressOf txtUsername_EnterNew
        AddHandler txtUsername.Leave, AddressOf txtUsername_LeaveNew
        
        AddHandler txtPassword.Enter, AddressOf txtPassword_EnterNew
        AddHandler txtPassword.Leave, AddressOf txtPassword_LeaveNew
        
        AddHandler pnlUserWrap.Paint, AddressOf pnlWrapper_Paint
        AddHandler pnlPassWrap.Paint, AddressOf pnlWrapper_Paint
    End Sub

    Private Sub txtUsername_EnterNew(sender As Object, e As EventArgs)
        pnlUserWrap.BorderStyle = BorderStyle.None
        pnlUserWrap.BackColor = Color.White
        txtUsername.BackColor = Color.White
        pnlUserWrap.Invalidate()
    End Sub

    Private Sub txtUsername_LeaveNew(sender As Object, e As EventArgs)
        pnlUserWrap.BorderStyle = BorderStyle.FixedSingle
        pnlUserWrap.BackColor = Color.FromArgb(245, 245, 245)
        txtUsername.BackColor = Color.FromArgb(245, 245, 245)
    End Sub

    Private Sub txtPassword_EnterNew(sender As Object, e As EventArgs)
        pnlPassWrap.BorderStyle = BorderStyle.None
        pnlPassWrap.BackColor = Color.White
        txtPassword.BackColor = Color.White
        pnlPassWrap.Invalidate()
    End Sub

    Private Sub txtPassword_LeaveNew(sender As Object, e As EventArgs)
        pnlPassWrap.BorderStyle = BorderStyle.FixedSingle
        pnlPassWrap.BackColor = Color.FromArgb(245, 245, 245)
        txtPassword.BackColor = Color.FromArgb(245, 245, 245)
        pnlPassWrap.BackColor = Color.FromArgb(249, 250, 251)
        txtPassword.BackColor = Color.FromArgb(249, 250, 251)
    End Sub
    
    Private Sub pnlWrapper_Paint(sender As Object, e As PaintEventArgs)
        Dim pnl As Panel = DirectCast(sender, Panel)
        If pnl.BackColor = Color.White Then ' Focused state
            e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Using p As New Pen(Color.FromArgb(37, 99, 235), 1.5F)
                e.Graphics.DrawRectangle(p, 1, 1, pnl.Width - 2, pnl.Height - 2)
            End Using
        End If
    End Sub

    Private Sub btnGuest_Click(sender As Object, e As EventArgs)
        Session.CurrentUser = New User() With {
            .UserID = -1,
            .Username = "guest",
            .FullName = "Guest User",
            .Role = "Student"
        }
        Dim dash As New frmDashboard()
        dash.Show()
        Me.Hide()
    End Sub

    Private Sub LoadIllustration()
        Dim html As New System.Text.StringBuilder()
        html.AppendLine("<!DOCTYPE html><html><head><meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" /><style>")
        html.AppendLine("* { margin:0; padding:0; box-sizing:border-box; }")
        html.AppendLine("html, body { width:100%; height:100%; background:#111827; overflow:hidden; }")
        html.AppendLine("svg { width:100%; height:100%; display:block; }")
        html.AppendLine("</style></head><body>")
        html.AppendLine("<svg viewBox=""0 0 210 420"" xmlns=""http://www.w3.org/2000/svg"">")
        html.AppendLine("<rect width=""210"" height=""420"" fill=""#111827""/>")
        html.AppendLine("<circle cx=""105"" cy=""160"" r=""90"" fill=""#151f30""/>")
        html.AppendLine("<circle cx=""105"" cy=""160"" r=""68"" fill=""none"" stroke=""rgba(93,202,165,0.08)"" stroke-width=""0.5""/>")
        html.AppendLine("<circle cx=""105"" cy=""160"" r=""50"" fill=""none"" stroke=""rgba(93,202,165,0.12)"" stroke-width=""0.5""/>")
        html.AppendLine("<circle cx=""105"" cy=""160"" r=""32"" fill=""none"" stroke=""rgba(93,202,165,0.18)"" stroke-width=""0.5""/>")
        html.AppendLine("<ellipse cx=""105"" cy=""160"" rx=""90"" ry=""26"" fill=""none"" stroke=""rgba(55,138,221,0.15)"" stroke-width=""0.5""/>")
        html.AppendLine("<ellipse cx=""105"" cy=""160"" rx=""68"" ry=""19"" fill=""none"" stroke=""rgba(55,138,221,0.10)"" stroke-width=""0.5""/>")
        html.AppendLine("<circle cx=""105"" cy=""92""  r=""5""   fill=""#5DCAA5""/>")
        html.AppendLine("<circle cx=""170"" cy=""178"" r=""3.5"" fill=""#378ADD""/>")
        html.AppendLine("<circle cx=""40""  cy=""178"" r=""3.5"" fill=""#378ADD""/>")
        html.AppendLine("<circle cx=""152"" cy=""103"" r=""3""   fill=""rgba(93,202,165,0.5)""/>")
        html.AppendLine("<circle cx=""58""  cy=""103"" r=""3""   fill=""rgba(93,202,165,0.5)""/>")
        html.AppendLine("<circle cx=""160"" cy=""220"" r=""2.5"" fill=""rgba(55,138,221,0.5)""/>")
        html.AppendLine("<circle cx=""50""  cy=""220"" r=""2.5"" fill=""rgba(55,138,221,0.5)""/>")
        html.AppendLine("<circle cx=""105"" cy=""160"" r=""22"" fill=""#1a2640"" stroke=""rgba(93,202,165,0.3)"" stroke-width=""1""/>")
        html.AppendLine("<circle cx=""105"" cy=""160"" r=""14"" fill=""#1e2d4a"" stroke=""rgba(93,202,165,0.2)"" stroke-width=""0.5""/>")
        html.AppendLine("<path d=""M97 163 L103 155 L109 161 L115 151 L121 159"" fill=""none"" stroke=""#5DCAA5"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round""/>")
        html.AppendLine("<circle cx=""97""  cy=""163"" r=""1.8"" fill=""#5DCAA5""/>")
        html.AppendLine("<circle cx=""121"" cy=""159"" r=""1.8"" fill=""#5DCAA5""/>")
        html.AppendLine("<rect x=""60""  y=""244"" width=""14"" height=""44"" rx=""2"" fill=""#085041""/>")
        html.AppendLine("<rect x=""60""  y=""244"" width=""14"" height=""6""  rx=""1"" fill=""#0F6E56""/>")
        html.AppendLine("<rect x=""62""  y=""253"" width=""8""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.20)""/>")
        html.AppendLine("<rect x=""62""  y=""257"" width=""6""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.15)""/>")
        html.AppendLine("<rect x=""76""  y=""236"" width=""13"" height=""52"" rx=""2"" fill=""#0C447C""/>")
        html.AppendLine("<rect x=""76""  y=""236"" width=""13"" height=""6""  rx=""1"" fill=""#185FA5""/>")
        html.AppendLine("<rect x=""78""  y=""245"" width=""7""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.20)""/>")
        html.AppendLine("<rect x=""78""  y=""249"" width=""9""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.15)""/>")
        html.AppendLine("<rect x=""91""  y=""228"" width=""14"" height=""60"" rx=""2"" fill=""#633806""/>")
        html.AppendLine("<rect x=""91""  y=""228"" width=""14"" height=""6""  rx=""1"" fill=""#854F0B""/>")
        html.AppendLine("<rect x=""93""  y=""237"" width=""8""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.20)""/>")
        html.AppendLine("<rect x=""93""  y=""241"" width=""6""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.15)""/>")
        html.AppendLine("<rect x=""93""  y=""245"" width=""9""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.12)""/>")
        html.AppendLine("<rect x=""107"" y=""240"" width=""13"" height=""48"" rx=""2"" fill=""#4B1528""/>")
        html.AppendLine("<rect x=""107"" y=""240"" width=""13"" height=""6""  rx=""1"" fill=""#72243E""/>")
        html.AppendLine("<rect x=""109"" y=""249"" width=""7""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.20)""/>")
        html.AppendLine("<rect x=""109"" y=""253"" width=""5""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.15)""/>")
        html.AppendLine("<rect x=""122"" y=""248"" width=""12"" height=""40"" rx=""2"" fill=""#26215C""/>")
        html.AppendLine("<rect x=""122"" y=""248"" width=""12"" height=""5""  rx=""1"" fill=""#3C3489""/>")
        html.AppendLine("<rect x=""124"" y=""256"" width=""6""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.20)""/>")
        html.AppendLine("<rect x=""124"" y=""260"" width=""8""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.15)""/>")
        html.AppendLine("<rect x=""136"" y=""255"" width=""11"" height=""33"" rx=""2"" fill=""#27500A""/>")
        html.AppendLine("<rect x=""136"" y=""255"" width=""11"" height=""5""  rx=""1"" fill=""#3B6D11""/>")
        html.AppendLine("<rect x=""138"" y=""262"" width=""5""  height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.20)""/>")
        html.AppendLine("<rect x=""56"" y=""286"" width=""94"" height=""3""  rx=""1.5"" fill=""#1e2d4a""/>")
        html.AppendLine("<rect x=""52"" y=""289"" width=""102"" height=""4"" rx=""2""   fill=""#243050""/>")
        html.AppendLine("<g transform=""rotate(-14 52 178)"">")
        html.AppendLine("  <rect x=""30"" y=""162"" width=""44"" height=""58"" rx=""3"" fill=""#1a2640"" stroke=""rgba(93,202,165,0.2)"" stroke-width=""0.5""/>")
        html.AppendLine("  <rect x=""34"" y=""167"" width=""36"" height=""4"" rx=""1"" fill=""rgba(255,255,255,0.08)""/>")
        html.AppendLine("  <rect x=""34"" y=""174"" width=""28"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.06)""/>")
        html.AppendLine("  <rect x=""34"" y=""178"" width=""32"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.06)""/>")
        html.AppendLine("  <rect x=""34"" y=""182"" width=""24"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.06)""/>")
        html.AppendLine("  <rect x=""34"" y=""186"" width=""30"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.05)""/>")
        html.AppendLine("  <circle cx=""52"" cy=""207"" r=""6"" fill=""none"" stroke=""#5DCAA5"" stroke-width=""0.8""/>")
        html.AppendLine("  <path d=""M49.5 207 L51.5 209.5 L55.5 204.5"" fill=""none"" stroke=""#5DCAA5"" stroke-width=""1"" stroke-linecap=""round"" stroke-linejoin=""round""/>")
        html.AppendLine("</g>")
        html.AppendLine("<g transform=""rotate(12 162 155)"">")
        html.AppendLine("  <rect x=""140"" y=""138"" width=""44"" height=""56"" rx=""3"" fill=""#1a2640"" stroke=""rgba(55,138,221,0.2)"" stroke-width=""0.5""/>")
        html.AppendLine("  <rect x=""144"" y=""143"" width=""36"" height=""4"" rx=""1"" fill=""rgba(255,255,255,0.08)""/>")
        html.AppendLine("  <rect x=""144"" y=""150"" width=""24"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.06)""/>")
        html.AppendLine("  <rect x=""144"" y=""154"" width=""30"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.06)""/>")
        html.AppendLine("  <rect x=""144"" y=""158"" width=""20"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.06)""/>")
        html.AppendLine("  <rect x=""144"" y=""162"" width=""28"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.05)""/>")
        html.AppendLine("  <rect x=""144"" y=""166"" width=""22"" height=""1.5"" rx=""0.5"" fill=""rgba(255,255,255,0.05)""/>")
        html.AppendLine("  <rect x=""148"" y=""175"" width=""24"" height=""12"" rx=""2"" fill=""rgba(55,138,221,0.15)"" stroke=""rgba(55,138,221,0.3)"" stroke-width=""0.5""/>")
        html.AppendLine("  <text x=""160"" y=""184"" text-anchor=""middle"" fill=""#378ADD"" font-size=""7"" font-family=""sans-serif"" font-weight=""500"">PDF</text>")
        html.AppendLine("</g>")
        html.AppendLine("<circle cx=""34""  cy=""72""  r=""2""   fill=""rgba(93,202,165,0.4)""/>")
        html.AppendLine("<circle cx=""176"" cy=""58""  r=""1.5"" fill=""rgba(255,255,255,0.2)""/>")
        html.AppendLine("<circle cx=""188"" cy=""88""  r=""2.5"" fill=""rgba(55,138,221,0.35)""/>")
        html.AppendLine("<circle cx=""22""  cy=""105"" r=""1.5"" fill=""rgba(255,255,255,0.15)""/>")
        html.AppendLine("<circle cx=""192"" cy=""300"" r=""1.5"" fill=""rgba(93,202,165,0.3)""/>")
        html.AppendLine("<circle cx=""18""  cy=""320"" r=""2""   fill=""rgba(55,138,221,0.25)""/>")
        html.AppendLine("<line x1=""105"" y1=""92""  x2=""105"" y2=""108"" stroke=""rgba(93,202,165,0.25)"" stroke-width=""0.5"" stroke-dasharray=""2,2""/>")
        html.AppendLine("<line x1=""170"" y1=""178"" x2=""157"" y2=""178"" stroke=""rgba(55,138,221,0.25)"" stroke-width=""0.5"" stroke-dasharray=""2,2""/>")
        html.AppendLine("<line x1=""40""  y1=""178"" x2=""53""  y2=""178"" stroke=""rgba(55,138,221,0.25)"" stroke-width=""0.5"" stroke-dasharray=""2,2""/>")
        html.AppendLine("<rect x=""20"" y=""50""  width=""170"" height=""0.5"" fill=""rgba(255,255,255,0.04)""/>")
        html.AppendLine("<rect x=""20"" y=""370"" width=""170"" height=""0.5"" fill=""rgba(255,255,255,0.04)""/>")
        html.AppendLine("<text x=""105"" y=""342"" text-anchor=""middle"" fill=""#e8edf5"" font-size=""15"" font-weight=""500"" font-family=""sans-serif"" letter-spacing=""0.02em"">EduVault</text>")
        html.AppendLine("<text x=""105"" y=""358"" text-anchor=""middle"" fill=""rgba(232,237,245,0.38)"" font-size=""8.5"" font-family=""sans-serif"" letter-spacing=""0.04em"">OPEN-ACCESS RESOURCE LIBRARY</text>")
        html.AppendLine("<line x1=""60""  y1=""368"" x2=""92""  y2=""368"" stroke=""rgba(93,202,165,0.25)"" stroke-width=""0.5""/>")
        html.AppendLine("<circle cx=""105"" cy=""368"" r=""2"" fill=""#5DCAA5"" opacity=""0.7""/>")
        html.AppendLine("<line x1=""118"" y1=""368"" x2=""150"" y2=""368"" stroke=""rgba(93,202,165,0.25)"" stroke-width=""0.5""/>")
        html.AppendLine("<text x=""105"" y=""385"" text-anchor=""middle"" fill=""#5DCAA5"" font-size=""8"" font-family=""sans-serif"" letter-spacing=""0.06em"">QUALITY EDUCATION</text>")
        html.AppendLine("</svg></body></html>")

        With webIllustration
            .ScrollBarsEnabled = False
            .IsWebBrowserContextMenuEnabled = False
            .AllowNavigation = False
            .AllowWebBrowserDrop = False
            .ScriptErrorsSuppressed = True
            .DocumentText = html.ToString()
        End With
    End Sub

    Private Sub Input_Enter(sender As Object, e As EventArgs) Handles txtUsername.Enter, txtPassword.Enter
        Dim txt As TextBox = DirectCast(sender, TextBox)
        StyleHelper.UpdateInputFocus(txt, True)
    End Sub

    Private Sub Input_Leave(sender As Object, e As EventArgs) Handles txtUsername.Leave, txtPassword.Leave
        Dim txt As TextBox = DirectCast(sender, TextBox)
        StyleHelper.UpdateInputFocus(txt, False)
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' CORE LOGIN LOGIC
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Attempts login using the entered credentials.
    ''' On success, opens the Dashboard and closes this form.
    ''' </summary>
    Private Sub PerformLogin()
        ' Basic UI-level empty check before calling the service
        If String.IsNullOrWhiteSpace(txtUsername.Text) Then
            ShowError("Please enter your username.")
            txtUsername.Focus()
            Return
        End If
        If String.IsNullOrWhiteSpace(txtPassword.Text) Then
            ShowError("Please enter your password.")
            txtPassword.Focus()
            Return
        End If

        ' Disable button to prevent double-click
        btnLogin.Enabled = False
        lblStatus.ForeColor = Color.DimGray
        lblStatus.Text = "Authenticating..."
        Application.DoEvents()

        Try
            Dim errorMessage As String = String.Empty
            Dim success As Boolean = _authService.Login(
                txtUsername.Text.Trim(),
                txtPassword.Text,
                errorMessage
            )

            If success Then
                ' Open the dashboard. We MUST use Me.Hide() — not Me.Close() —
                ' because frmLogin is the Application Framework startup form.
                ' Closing it would trigger automatic app shutdown.
                Dim dashboard As New frmDashboard()
                dashboard.Show()
                Me.Hide()
            Else
                ShowError(errorMessage)
                txtPassword.Clear()
                txtPassword.Focus()
            End If

        Catch ex As Exception
            ShowError($"Unexpected error: {ex.Message}")
        Finally
            btnLogin.Enabled = True
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' HELPERS
    ' ─────────────────────────────────────────────────────────────

    Private Sub ShowError(message As String)
        lblStatus.ForeColor = Color.FromArgb(192, 0, 0)
        lblStatus.Text = $"[X] {message}"
        ' Also show a MessageBox so the error is impossible to miss
        MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    Private Sub ClearStatus()
        lblStatus.Text = String.Empty
    End Sub

End Class
