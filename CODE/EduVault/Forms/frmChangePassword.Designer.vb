<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmChangePassword
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
        Me.pnlHeader       = New System.Windows.Forms.Panel()
        Me.lblHeader       = New System.Windows.Forms.Label()
        Me.lblCurrent      = New System.Windows.Forms.Label()
        Me.txtCurrent      = New System.Windows.Forms.TextBox()
        Me.lblNew          = New System.Windows.Forms.Label()
        Me.txtNew          = New System.Windows.Forms.TextBox()
        Me.lblConfirm      = New System.Windows.Forms.Label()
        Me.txtConfirm      = New System.Windows.Forms.TextBox()
        Me.lblRules        = New System.Windows.Forms.Label()
        Me.btnSave         = New System.Windows.Forms.Button()
        Me.btnCancel       = New System.Windows.Forms.Button()
        Me.lblStatus       = New System.Windows.Forms.Label()
        Me.pnlHeader.SuspendLayout()
        Me.SuspendLayout()

        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.pnlHeader.Controls.Add(Me.lblHeader)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Height = 52
        Me.pnlHeader.Name = "pnlHeader"

        Me.lblHeader.Text = "Change Password"
        Me.lblHeader.Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
        Me.lblHeader.ForeColor = System.Drawing.Color.White
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Location = New System.Drawing.Point(14, 12)
        Me.lblHeader.Name = "lblHeader"

        Dim lbFont As New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Dim tbFont As New System.Drawing.Font("Segoe UI", 10)

        Me.lblCurrent.Text = "Current Password"
        Me.lblCurrent.Font = lbFont : Me.lblCurrent.AutoSize = True
        Me.lblCurrent.Location = New System.Drawing.Point(30, 74)
        Me.lblCurrent.Name = "lblCurrent"

        Me.txtCurrent.Font = tbFont : Me.txtCurrent.UseSystemPasswordChar = True
        Me.txtCurrent.Location = New System.Drawing.Point(30, 96)
        Me.txtCurrent.Size = New System.Drawing.Size(320, 28)
        Me.txtCurrent.Name = "txtCurrent" : Me.txtCurrent.TabIndex = 0

        Me.lblNew.Text = "New Password"
        Me.lblNew.Font = lbFont : Me.lblNew.AutoSize = True
        Me.lblNew.Location = New System.Drawing.Point(30, 140)
        Me.lblNew.Name = "lblNew"

        Me.txtNew.Font = tbFont : Me.txtNew.UseSystemPasswordChar = True
        Me.txtNew.Location = New System.Drawing.Point(30, 162)
        Me.txtNew.Size = New System.Drawing.Size(320, 28)
        Me.txtNew.Name = "txtNew" : Me.txtNew.TabIndex = 1

        Me.lblConfirm.Text = "Confirm New Password"
        Me.lblConfirm.Font = lbFont : Me.lblConfirm.AutoSize = True
        Me.lblConfirm.Location = New System.Drawing.Point(30, 206)
        Me.lblConfirm.Name = "lblConfirm"

        Me.txtConfirm.Font = tbFont : Me.txtConfirm.UseSystemPasswordChar = True
        Me.txtConfirm.Location = New System.Drawing.Point(30, 228)
        Me.txtConfirm.Size = New System.Drawing.Size(320, 28)
        Me.txtConfirm.Name = "txtConfirm" : Me.txtConfirm.TabIndex = 2

        Me.lblRules.Text = "Min 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special char"
        Me.lblRules.Font = New System.Drawing.Font("Segoe UI", 7.5)
        Me.lblRules.ForeColor = System.Drawing.Color.DimGray
        Me.lblRules.AutoSize = True
        Me.lblRules.Location = New System.Drawing.Point(30, 268)
        Me.lblRules.Name = "lblRules"

        Me.btnSave.Text = "Save"
        Me.btnSave.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.btnSave.ForeColor = System.Drawing.Color.White
        Me.btnSave.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.FlatAppearance.BorderSize = 0
        Me.btnSave.Location = New System.Drawing.Point(30, 300)
        Me.btnSave.Size = New System.Drawing.Size(150, 36)
        Me.btnSave.Name = "btnSave" : Me.btnSave.TabIndex = 3
        Me.btnSave.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI", 10)
        Me.btnCancel.ForeColor = System.Drawing.Color.White
        Me.btnCancel.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.FlatAppearance.BorderSize = 0
        Me.btnCancel.Location = New System.Drawing.Point(200, 300)
        Me.btnCancel.Size = New System.Drawing.Size(150, 36)
        Me.btnCancel.Name = "btnCancel" : Me.btnCancel.TabIndex = 4
        Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand

        Me.lblStatus.Text = String.Empty
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.lblStatus.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0)
        Me.lblStatus.AutoSize = True
        Me.lblStatus.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblStatus.Location = New System.Drawing.Point(30, 348)
        Me.lblStatus.Name = "lblStatus"

        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(380, 390)
        Me.Controls.Add(Me.pnlHeader)
        Me.Controls.Add(Me.lblCurrent) : Me.Controls.Add(Me.txtCurrent)
        Me.Controls.Add(Me.lblNew) : Me.Controls.Add(Me.txtNew)
        Me.Controls.Add(Me.lblConfirm) : Me.Controls.Add(Me.txtConfirm)
        Me.Controls.Add(Me.lblRules)
        Me.Controls.Add(Me.btnSave) : Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lblStatus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False : Me.MinimizeBox = False
        Me.Name = "frmChangePassword"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "EduVault - Change Password"
        Me.pnlHeader.ResumeLayout(False) : Me.pnlHeader.PerformLayout()
        Me.ResumeLayout(False) : Me.PerformLayout()
    End Sub

    Friend WithEvents pnlHeader  As System.Windows.Forms.Panel
    Friend WithEvents lblHeader  As System.Windows.Forms.Label
    Friend WithEvents lblCurrent As System.Windows.Forms.Label
    Friend WithEvents txtCurrent As System.Windows.Forms.TextBox
    Friend WithEvents lblNew     As System.Windows.Forms.Label
    Friend WithEvents txtNew     As System.Windows.Forms.TextBox
    Friend WithEvents lblConfirm As System.Windows.Forms.Label
    Friend WithEvents txtConfirm As System.Windows.Forms.TextBox
    Friend WithEvents lblRules   As System.Windows.Forms.Label
    Friend WithEvents btnSave    As System.Windows.Forms.Button
    Friend WithEvents btnCancel  As System.Windows.Forms.Button
    Friend WithEvents lblStatus  As System.Windows.Forms.Label

End Class
