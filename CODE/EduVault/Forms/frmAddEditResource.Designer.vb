<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmAddEditResource
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
        Me.pnlHeader          = New System.Windows.Forms.Panel()
        Me.lblFormTitle       = New System.Windows.Forms.Label()
        Me.pnlBody            = New System.Windows.Forms.Panel()
        Me.lblTitle           = New System.Windows.Forms.Label()
        Me.txtTitle           = New System.Windows.Forms.TextBox()
        Me.lblDescription     = New System.Windows.Forms.Label()
        Me.txtDescription     = New System.Windows.Forms.RichTextBox()
        Me.lblCategory        = New System.Windows.Forms.Label()
        Me.cmbCategory        = New System.Windows.Forms.ComboBox()
        Me.lblResourceType    = New System.Windows.Forms.Label()
        Me.cmbResourceType    = New System.Windows.Forms.ComboBox()
        Me.lblLevel           = New System.Windows.Forms.Label()
        Me.cmbLevel           = New System.Windows.Forms.ComboBox()
        Me.lblSubjectArea     = New System.Windows.Forms.Label()
        Me.txtSubjectArea     = New System.Windows.Forms.TextBox()
        Me.lblURL             = New System.Windows.Forms.Label()
        Me.txtURL             = New System.Windows.Forms.TextBox()
        Me.lblFilePath        = New System.Windows.Forms.Label()
        Me.txtFilePath        = New System.Windows.Forms.TextBox()
        Me.lblTags            = New System.Windows.Forms.Label()
        Me.txtTags            = New System.Windows.Forms.TextBox()
        Me.lblTagsHint        = New System.Windows.Forms.Label()
        Me.lblValidationError = New System.Windows.Forms.Label()
        Me.pnlFooter          = New System.Windows.Forms.Panel()
        Me.btnSubmit          = New System.Windows.Forms.Button()
        Me.btnCancel          = New System.Windows.Forms.Button()
        Me.pnlHeader.SuspendLayout()
        Me.pnlBody.SuspendLayout()
        Me.pnlFooter.SuspendLayout()
        Me.SuspendLayout()

        ' ── pnlHeader ──
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.pnlHeader.Controls.Add(Me.lblFormTitle)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Height = 52
        Me.pnlHeader.Name = "pnlHeader"

        Me.lblFormTitle.AutoSize = True
        Me.lblFormTitle.Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
        Me.lblFormTitle.ForeColor = System.Drawing.Color.White
        Me.lblFormTitle.Location = New System.Drawing.Point(12, 12)
        Me.lblFormTitle.Name = "lblFormTitle"
        Me.lblFormTitle.Text = "+ Add New Resource"

        ' ── pnlBody ──
        Me.pnlBody.AutoScroll = True
        Me.pnlBody.BackColor = System.Drawing.Color.FromArgb(248, 250, 255)
        Me.pnlBody.Controls.AddRange(New System.Windows.Forms.Control() {
            Me.lblTitle, Me.txtTitle,
            Me.lblDescription, Me.txtDescription,
            Me.lblCategory, Me.cmbCategory,
            Me.lblResourceType, Me.cmbResourceType,
            Me.lblLevel, Me.cmbLevel,
            Me.lblSubjectArea, Me.txtSubjectArea,
            Me.lblURL, Me.txtURL,
            Me.lblFilePath, Me.txtFilePath,
            Me.lblTags, Me.txtTags, Me.lblTagsHint,
            Me.lblValidationError})
        Me.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBody.Name = "pnlBody"
        Me.pnlBody.Padding = New System.Windows.Forms.Padding(16, 12, 16, 8)

        Dim lbFont As New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Dim lbColor As System.Drawing.Color = System.Drawing.Color.FromArgb(50, 50, 50)
        Dim tbFont  As New System.Drawing.Font("Segoe UI", 9.5)
        Dim col1 As Integer = 16
        Dim col2 As Integer = 330
        Dim tbW  As Integer = 290

        ' Row 1: Title (full width)
        Me.lblTitle.Text = "Resource Title *" : Me.lblTitle.Font = lbFont : Me.lblTitle.ForeColor = lbColor
        Me.lblTitle.AutoSize = True : Me.lblTitle.Location = New System.Drawing.Point(col1, 20)
        Me.lblTitle.Name = "lblTitle"
        Me.txtTitle.Font = tbFont : Me.txtTitle.Location = New System.Drawing.Point(col1, 40)
        Me.txtTitle.Size = New System.Drawing.Size(604, 28) : Me.txtTitle.MaxLength = 200
        Me.txtTitle.Name = "txtTitle"

        ' Row 2: Description
        Me.lblDescription.Text = "Description" : Me.lblDescription.Font = lbFont : Me.lblDescription.ForeColor = lbColor
        Me.lblDescription.AutoSize = True : Me.lblDescription.Location = New System.Drawing.Point(col1, 80)
        Me.lblDescription.Name = "lblDescription"
        Me.txtDescription.Font = tbFont : Me.txtDescription.Location = New System.Drawing.Point(col1, 100)
        Me.txtDescription.Size = New System.Drawing.Size(604, 72) : Me.txtDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.txtDescription.Name = "txtDescription"

        ' Row 3: Category | Type
        Me.lblCategory.Text = "Category *" : Me.lblCategory.Font = lbFont : Me.lblCategory.ForeColor = lbColor
        Me.lblCategory.AutoSize = True : Me.lblCategory.Location = New System.Drawing.Point(col1, 188)
        Me.lblCategory.Name = "lblCategory"
        Me.cmbCategory.Font = tbFont : Me.cmbCategory.Location = New System.Drawing.Point(col1, 208)
        Me.cmbCategory.Size = New System.Drawing.Size(tbW, 28) : Me.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCategory.Name = "cmbCategory"

        Me.lblResourceType.Text = "Resource Type *" : Me.lblResourceType.Font = lbFont : Me.lblResourceType.ForeColor = lbColor
        Me.lblResourceType.AutoSize = True : Me.lblResourceType.Location = New System.Drawing.Point(col2, 188)
        Me.lblResourceType.Name = "lblResourceType"
        Me.cmbResourceType.Font = tbFont : Me.cmbResourceType.Location = New System.Drawing.Point(col2, 208)
        Me.cmbResourceType.Size = New System.Drawing.Size(tbW, 28) : Me.cmbResourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbResourceType.Items.AddRange({"- Select Type -", "E-Book", "Video", "Module", "Reference", "Article"})
        Me.cmbResourceType.SelectedIndex = 0 : Me.cmbResourceType.Name = "cmbResourceType"

        ' Row 4: Level | Subject Area
        Me.lblLevel.Text = "Education Level" : Me.lblLevel.Font = lbFont : Me.lblLevel.ForeColor = lbColor
        Me.lblLevel.AutoSize = True : Me.lblLevel.Location = New System.Drawing.Point(col1, 248)
        Me.lblLevel.Name = "lblLevel"
        Me.cmbLevel.Font = tbFont : Me.cmbLevel.Location = New System.Drawing.Point(col1, 268)
        Me.cmbLevel.Size = New System.Drawing.Size(tbW, 28) : Me.cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbLevel.Items.AddRange({"- Select Level -", "Beginner", "Intermediate", "Advanced"})
        Me.cmbLevel.SelectedIndex = 0 : Me.cmbLevel.Name = "cmbLevel"

        Me.lblSubjectArea.Text = "Subject Area" : Me.lblSubjectArea.Font = lbFont : Me.lblSubjectArea.ForeColor = lbColor
        Me.lblSubjectArea.AutoSize = True : Me.lblSubjectArea.Location = New System.Drawing.Point(col2, 248)
        Me.lblSubjectArea.Name = "lblSubjectArea"
        Me.txtSubjectArea.Font = tbFont : Me.txtSubjectArea.Location = New System.Drawing.Point(col2, 268)
        Me.txtSubjectArea.Size = New System.Drawing.Size(tbW, 28) : Me.txtSubjectArea.MaxLength = 100
        Me.txtSubjectArea.Name = "txtSubjectArea"

        ' Row 5: URL
        Me.lblURL.Text = "Resource URL (http:// or https://)" : Me.lblURL.Font = lbFont : Me.lblURL.ForeColor = lbColor
        Me.lblURL.AutoSize = True : Me.lblURL.Location = New System.Drawing.Point(col1, 308)
        Me.lblURL.Name = "lblURL"
        Me.txtURL.Font = tbFont : Me.txtURL.Location = New System.Drawing.Point(col1, 328)
        Me.txtURL.Size = New System.Drawing.Size(604, 28) : Me.txtURL.MaxLength = 500
        Me.txtURL.Name = "txtURL"

        ' Row 6: File Path
        Me.lblFilePath.Text = "File Path (if not a web resource)" : Me.lblFilePath.Font = lbFont : Me.lblFilePath.ForeColor = lbColor
        Me.lblFilePath.AutoSize = True : Me.lblFilePath.Location = New System.Drawing.Point(col1, 368)
        Me.lblFilePath.Name = "lblFilePath"
        Me.txtFilePath.Font = tbFont : Me.txtFilePath.Location = New System.Drawing.Point(col1, 388)
        Me.txtFilePath.Size = New System.Drawing.Size(604, 28) : Me.txtFilePath.MaxLength = 500
        Me.txtFilePath.Name = "txtFilePath"

        ' Row 7: Tags
        Me.lblTags.Text = "Tags (comma-separated keywords)" : Me.lblTags.Font = lbFont : Me.lblTags.ForeColor = lbColor
        Me.lblTags.AutoSize = True : Me.lblTags.Location = New System.Drawing.Point(col1, 428)
        Me.lblTags.Name = "lblTags"
        Me.txtTags.Font = tbFont : Me.txtTags.Location = New System.Drawing.Point(col1, 448)
        Me.txtTags.Size = New System.Drawing.Size(604, 28) : Me.txtTags.MaxLength = 500
        Me.txtTags.Name = "txtTags"
        Me.lblTagsHint.Text = "e.g.  python, beginner, programming, free" : Me.lblTagsHint.Font = New System.Drawing.Font("Segoe UI", 8)
        Me.lblTagsHint.ForeColor = System.Drawing.Color.DimGray : Me.lblTagsHint.AutoSize = True
        Me.lblTagsHint.Location = New System.Drawing.Point(col1, 480) : Me.lblTagsHint.Name = "lblTagsHint"

        ' Validation error label
        Me.lblValidationError.AutoSize = True : Me.lblValidationError.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.lblValidationError.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0)
        Me.lblValidationError.Location = New System.Drawing.Point(col1, 504) : Me.lblValidationError.MaximumSize = New System.Drawing.Size(604, 0)
        Me.lblValidationError.Name = "lblValidationError" : Me.lblValidationError.Text = String.Empty

        ' ── pnlFooter ──
        Me.pnlFooter.BackColor = System.Drawing.Color.FromArgb(240, 244, 250)
        Me.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlFooter.Controls.Add(Me.btnSubmit)
        Me.pnlFooter.Controls.Add(Me.btnCancel)
        Me.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlFooter.Height = 52 : Me.pnlFooter.Name = "pnlFooter"

        Me.btnSubmit.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnSubmit.FlatAppearance.BorderSize = 0
        Me.btnSubmit.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.btnSubmit.ForeColor = System.Drawing.Color.White
        Me.btnSubmit.Location = New System.Drawing.Point(392, 10) : Me.btnSubmit.Size = New System.Drawing.Size(150, 34)
        Me.btnSubmit.Name = "btnSubmit" : Me.btnSubmit.Text = "Add Resource" : Me.btnSubmit.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnCancel.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnCancel.FlatAppearance.BorderSize = 0
        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI", 10)
        Me.btnCancel.ForeColor = System.Drawing.Color.White
        Me.btnCancel.Location = New System.Drawing.Point(552, 10) : Me.btnCancel.Size = New System.Drawing.Size(90, 34)
        Me.btnCancel.Name = "btnCancel" : Me.btnCancel.Text = "Cancel" : Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand

        ' ── frmAddEditResource ──
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(660, 620)
        Me.Controls.Add(Me.pnlBody)
        Me.Controls.Add(Me.pnlFooter)
        Me.Controls.Add(Me.pnlHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False : Me.MinimizeBox = False
        Me.Name = "frmAddEditResource"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "EduVault - Add Resource"

        Me.pnlHeader.ResumeLayout(False) : Me.pnlHeader.PerformLayout()
        Me.pnlBody.ResumeLayout(False) : Me.pnlBody.PerformLayout()
        Me.pnlFooter.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlHeader          As System.Windows.Forms.Panel
    Friend WithEvents lblFormTitle       As System.Windows.Forms.Label
    Friend WithEvents pnlBody            As System.Windows.Forms.Panel
    Friend WithEvents lblTitle           As System.Windows.Forms.Label
    Friend WithEvents txtTitle           As System.Windows.Forms.TextBox
    Friend WithEvents lblDescription     As System.Windows.Forms.Label
    Friend WithEvents txtDescription     As System.Windows.Forms.RichTextBox
    Friend WithEvents lblCategory        As System.Windows.Forms.Label
    Friend WithEvents cmbCategory        As System.Windows.Forms.ComboBox
    Friend WithEvents lblResourceType    As System.Windows.Forms.Label
    Friend WithEvents cmbResourceType    As System.Windows.Forms.ComboBox
    Friend WithEvents lblLevel           As System.Windows.Forms.Label
    Friend WithEvents cmbLevel           As System.Windows.Forms.ComboBox
    Friend WithEvents lblSubjectArea     As System.Windows.Forms.Label
    Friend WithEvents txtSubjectArea     As System.Windows.Forms.TextBox
    Friend WithEvents lblURL             As System.Windows.Forms.Label
    Friend WithEvents txtURL             As System.Windows.Forms.TextBox
    Friend WithEvents lblFilePath        As System.Windows.Forms.Label
    Friend WithEvents txtFilePath        As System.Windows.Forms.TextBox
    Friend WithEvents lblTags            As System.Windows.Forms.Label
    Friend WithEvents txtTags            As System.Windows.Forms.TextBox
    Friend WithEvents lblTagsHint        As System.Windows.Forms.Label
    Friend WithEvents lblValidationError As System.Windows.Forms.Label
    Friend WithEvents pnlFooter          As System.Windows.Forms.Panel
    Friend WithEvents btnSubmit          As System.Windows.Forms.Button
    Friend WithEvents btnCancel          As System.Windows.Forms.Button

End Class
