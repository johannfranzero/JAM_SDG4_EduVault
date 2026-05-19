<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLogin
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
        Me.pnlLeft          = New System.Windows.Forms.Panel()
        Me.lblBrand         = New System.Windows.Forms.Label()
        Me.lblBrandSub      = New System.Windows.Forms.Label()
        Me.lblBrandDesc     = New System.Windows.Forms.Label()
        Me.lblSdgBadge      = New System.Windows.Forms.Label()
        Me.pnlRight         = New System.Windows.Forms.Panel()
        Me.lblWelcome       = New System.Windows.Forms.Label()
        Me.lblWelcomeSub    = New System.Windows.Forms.Label()
        Me.lblUsername      = New System.Windows.Forms.Label()
        Me.txtUsername      = New System.Windows.Forms.TextBox()
        Me.lblPassword      = New System.Windows.Forms.Label()
        Me.chkShowPassword  = New System.Windows.Forms.CheckBox()
        Me.txtPassword      = New System.Windows.Forms.TextBox()
        Me.lnkForgotPassword = New System.Windows.Forms.LinkLabel()
        Me.lnkSignUp        = New System.Windows.Forms.LinkLabel()
        Me.btnLogin         = New System.Windows.Forms.Button()
        Me.lblStatus        = New System.Windows.Forms.Label()
        Me.pnlLeft.SuspendLayout()
        Me.pnlRight.SuspendLayout()
        Me.SuspendLayout()

        ' ── LEFT BRANDING PANEL ──────────────────────────────────────
        Me.pnlLeft.BackColor = System.Drawing.Color.FromArgb(19, 55, 109)
        Me.pnlLeft.Controls.Add(Me.lblBrand)
        Me.pnlLeft.Controls.Add(Me.lblBrandSub)
        Me.pnlLeft.Controls.Add(Me.lblBrandDesc)
        Me.pnlLeft.Controls.Add(Me.lblSdgBadge)
        Me.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlLeft.Width = 240
        Me.pnlLeft.Name = "pnlLeft"

        Me.lblBrand.Text = "EduVault"
        Me.lblBrand.Font = New System.Drawing.Font("Segoe UI", 26, System.Drawing.FontStyle.Bold)
        Me.lblBrand.ForeColor = System.Drawing.Color.White
        Me.lblBrand.AutoSize = False
        Me.lblBrand.Size = New System.Drawing.Size(220, 50)
        Me.lblBrand.Location = New System.Drawing.Point(20, 30)
        Me.lblBrand.Name = "lblBrand"

        Me.lblBrandSub.Text = "Open-Access" & Environment.NewLine & "Resource Library"
        Me.lblBrandSub.Font = New System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Regular)
        Me.lblBrandSub.ForeColor = System.Drawing.Color.FromArgb(160, 195, 240)
        Me.lblBrandSub.AutoSize = False
        Me.lblBrandSub.Size = New System.Drawing.Size(210, 52)
        Me.lblBrandSub.Location = New System.Drawing.Point(20, 80)
        Me.lblBrandSub.Name = "lblBrandSub"

        Me.lblBrandDesc.Text = "Supporting UN SDG 4"
        Me.lblBrandDesc.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.5, System.Drawing.FontStyle.Bold)
        Me.lblBrandDesc.ForeColor = System.Drawing.Color.White
        Me.lblBrandDesc.AutoSize = True
        Me.lblBrandDesc.Location = New System.Drawing.Point(20, 110)
        Me.lblBrandDesc.Name = "lblBrandDesc"

        Me.lblSdgBadge.Text = "Quality Education"
        Me.lblSdgBadge.Font = New System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold)
        Me.lblSdgBadge.ForeColor = System.Drawing.Color.FromArgb(26, 188, 156)
        Me.lblSdgBadge.AutoSize = False
        Me.lblSdgBadge.Size = New System.Drawing.Size(210, 40)
        Me.lblSdgBadge.Location = New System.Drawing.Point(20, 135)
        Me.lblSdgBadge.Name = "lblSdgBadge"

        ' ── RIGHT LOGIN PANEL ────────────────────────────────────────
        Me.pnlRight.BackColor = System.Drawing.Color.White
        Me.pnlRight.Controls.Add(Me.lblWelcome)
        Me.pnlRight.Controls.Add(Me.lblWelcomeSub)
        Me.pnlRight.Controls.Add(Me.lblUsername)
        Me.pnlRight.Controls.Add(Me.txtUsername)
        Me.pnlRight.Controls.Add(Me.lblPassword)
        Me.pnlRight.Controls.Add(Me.chkShowPassword)
        Me.pnlRight.Controls.Add(Me.txtPassword)
        Me.pnlRight.Controls.Add(Me.lnkForgotPassword)
        Me.pnlRight.Controls.Add(Me.lnkSignUp)
        Me.pnlRight.Controls.Add(Me.btnLogin)
        Me.pnlRight.Controls.Add(Me.lblStatus)
        Me.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlRight.Name = "pnlRight"
        Me.pnlRight.Padding = New System.Windows.Forms.Padding(32, 0, 32, 0)

        Me.lblWelcome.Text = "Welcome Back"
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold)
        Me.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(19, 55, 109)
        Me.lblWelcome.AutoSize = True
        Me.lblWelcome.Location = New System.Drawing.Point(32, 60)
        Me.lblWelcome.Name = "lblWelcome"

        Me.lblWelcomeSub.Text = "Sign in to continue to EduVault"
        Me.lblWelcomeSub.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.lblWelcomeSub.ForeColor = System.Drawing.Color.FromArgb(130, 130, 130)
        Me.lblWelcomeSub.AutoSize = True
        Me.lblWelcomeSub.Location = New System.Drawing.Point(32, 96)
        Me.lblWelcomeSub.Name = "lblWelcomeSub"

        Me.lblUsername.Text = "Username or Email"
        Me.lblUsername.Font = New System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold)
        Me.lblUsername.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60)
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Location = New System.Drawing.Point(32, 148)
        Me.lblUsername.Name = "lblUsername"

        Me.txtUsername.Font = New System.Drawing.Font("Segoe UI", 10.5)
        Me.txtUsername.Location = New System.Drawing.Point(32, 168)
        Me.txtUsername.MaxLength = 50
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(256, 28)
        Me.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtUsername.TabIndex = 0

        Me.lblPassword.Text = "Password"
        Me.lblPassword.Font = New System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold)
        Me.lblPassword.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60)
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Location = New System.Drawing.Point(32, 214)
        Me.lblPassword.Name = "lblPassword"

        Me.chkShowPassword.Text = "Show password"
        Me.chkShowPassword.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100)
        Me.chkShowPassword.AutoSize = True
        Me.chkShowPassword.Location = New System.Drawing.Point(32, 280)
        Me.chkShowPassword.Name = "chkShowPassword"
        Me.chkShowPassword.TabIndex = 2

        Me.txtPassword.Font = New System.Drawing.Font("Segoe UI", 10.5)
        Me.txtPassword.Location = New System.Drawing.Point(32, 234)
        Me.txtPassword.MaxLength = 100
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(256, 28)
        Me.txtPassword.UseSystemPasswordChar = True
        Me.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtPassword.TabIndex = 1

        Me.lnkForgotPassword.Text = "Forgot password?"
        Me.lnkForgotPassword.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.lnkForgotPassword.LinkColor = System.Drawing.Color.FromArgb(19, 55, 109)
        Me.lnkForgotPassword.AutoSize = True
        Me.lnkForgotPassword.Location = New System.Drawing.Point(175, 280)
        Me.lnkForgotPassword.Name = "lnkForgotPassword"
        Me.lnkForgotPassword.TabIndex = 3
        Me.lnkForgotPassword.TabStop = True

        Me.lnkSignUp.Text = "Don't have an account? Sign up"
        Me.lnkSignUp.Font = New System.Drawing.Font("Segoe UI Variable Text", 8.5)
        Me.lnkSignUp.LinkColor = System.Drawing.Color.FromArgb(100, 100, 100)
        Me.lnkSignUp.AutoSize = True
        Me.lnkSignUp.Location = New System.Drawing.Point(75, 375)
        Me.lnkSignUp.Name = "lnkSignUp"
        Me.lnkSignUp.TabIndex = 4
        Me.lnkSignUp.TabStop = True

        Me.btnLogin.Text = "Sign In"
        Me.btnLogin.Font = New System.Drawing.Font("Segoe UI", 10.5, System.Drawing.FontStyle.Bold)
        Me.btnLogin.ForeColor = System.Drawing.Color.White
        Me.btnLogin.BackColor = System.Drawing.Color.FromArgb(19, 55, 109)
        Me.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogin.FlatAppearance.BorderSize = 0
        Me.btnLogin.Location = New System.Drawing.Point(32, 320)
        Me.btnLogin.Size = New System.Drawing.Size(256, 42)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.TabIndex = 5
        Me.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnLogin.UseVisualStyleBackColor = False

        Me.lblStatus.Text = String.Empty
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.lblStatus.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0)
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(32, 405)
        Me.lblStatus.Name = "lblStatus"

        ' ── FORM ────────────────────────────────────────────────────
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(580, 440)
        Me.Controls.Add(Me.pnlRight)
        Me.Controls.Add(Me.pnlLeft)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = True
        Me.Name = "frmLogin"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EduVault - Sign In"

        Me.pnlLeft.ResumeLayout(False)
        Me.pnlLeft.PerformLayout()
        Me.pnlRight.ResumeLayout(False)
        Me.pnlRight.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlLeft         As System.Windows.Forms.Panel
    Friend WithEvents lblBrand        As System.Windows.Forms.Label
    Friend WithEvents lblBrandSub     As System.Windows.Forms.Label
    Friend WithEvents lblBrandDesc    As System.Windows.Forms.Label
    Friend WithEvents lblSdgBadge     As System.Windows.Forms.Label
    Friend WithEvents pnlRight        As System.Windows.Forms.Panel
    Friend WithEvents lblWelcome      As System.Windows.Forms.Label
    Friend WithEvents lblWelcomeSub   As System.Windows.Forms.Label
    Friend WithEvents lblUsername     As System.Windows.Forms.Label
    Friend WithEvents txtUsername     As System.Windows.Forms.TextBox
    Friend WithEvents lblPassword     As System.Windows.Forms.Label
    Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
    Friend WithEvents txtPassword     As System.Windows.Forms.TextBox
    Friend WithEvents lnkForgotPassword As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkSignUp        As System.Windows.Forms.LinkLabel
    Friend WithEvents btnLogin        As System.Windows.Forms.Button
    Friend WithEvents lblStatus       As System.Windows.Forms.Label

End Class
