<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmManageResources
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
        Me.pnlHeader           = New System.Windows.Forms.Panel()
        Me.lblFormTitle        = New System.Windows.Forms.Label()
        Me.pnlSearch           = New System.Windows.Forms.Panel()
        Me.lblSearchTitle      = New System.Windows.Forms.Label()
        Me.txtSearch           = New System.Windows.Forms.TextBox()
        Me.lblCat              = New System.Windows.Forms.Label()
        Me.cmbFilterCategory   = New System.Windows.Forms.ComboBox()
        Me.lblType             = New System.Windows.Forms.Label()
        Me.cmbFilterType       = New System.Windows.Forms.ComboBox()
        Me.lblLevel            = New System.Windows.Forms.Label()
        Me.cmbFilterLevel      = New System.Windows.Forms.ComboBox()
        Me.btnSearch           = New System.Windows.Forms.Button()
        Me.btnClearSearch      = New System.Windows.Forms.Button()
        Me.lblCount            = New System.Windows.Forms.Label()
        Me.dgvResources        = New System.Windows.Forms.DataGridView()
        Me.colResID            = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResTag           = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResTitle         = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResCat           = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResType          = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResLevel         = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResSubject       = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResViews         = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResDate          = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResAddedBy       = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlButtons          = New System.Windows.Forms.Panel()
        Me.btnAdd              = New System.Windows.Forms.Button()
        Me.btnEdit             = New System.Windows.Forms.Button()
        Me.btnDelete           = New System.Windows.Forms.Button()
        Me.btnBookmark         = New System.Windows.Forms.Button()
        Me.btnRefresh          = New System.Windows.Forms.Button()
        Me.btnExport           = New System.Windows.Forms.Button()
        Me.pnlHeader.SuspendLayout()
        Me.pnlSearch.SuspendLayout()
        CType(Me.dgvResources, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()

        ' ── Header ──────────────────────────────────────────────────
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.pnlHeader.Controls.Add(Me.lblFormTitle)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Height = 52
        Me.pnlHeader.Name = "pnlHeader"

        Me.lblFormTitle.Text = "Browse / Manage Resources"
        Me.lblFormTitle.Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
        Me.lblFormTitle.ForeColor = System.Drawing.Color.White
        Me.lblFormTitle.AutoSize = True
        Me.lblFormTitle.Location = New System.Drawing.Point(14, 12)
        Me.lblFormTitle.Name = "lblFormTitle"

        ' pnlSearch
        Me.pnlSearch.BackColor = System.Drawing.Color.White
        Me.pnlSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.pnlSearch.Controls.Add(Me.lblSearchTitle)
        Me.pnlSearch.Controls.Add(Me.txtSearch)
        Me.pnlSearch.Controls.Add(Me.lblCat)
        Me.pnlSearch.Controls.Add(Me.cmbFilterCategory)
        Me.pnlSearch.Controls.Add(Me.lblType)
        Me.pnlSearch.Controls.Add(Me.cmbFilterType)
        Me.pnlSearch.Controls.Add(Me.lblLevel)
        Me.pnlSearch.Controls.Add(Me.cmbFilterLevel)
        Me.pnlSearch.Controls.Add(Me.btnSearch)
        Me.pnlSearch.Controls.Add(Me.btnClearSearch)
        Me.pnlSearch.Controls.Add(Me.lblCount)
        Me.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlSearch.Height = 100
        Me.pnlSearch.Name = "pnlSearch"

        Me.lblSearchTitle.AutoSize = True
        Me.lblSearchTitle.Font = New System.Drawing.Font("Segoe UI", 9.0, System.Drawing.FontStyle.Bold)
        Me.lblSearchTitle.ForeColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.lblSearchTitle.Location = New System.Drawing.Point(12, 12)
        Me.lblSearchTitle.Name = "lblSearchTitle"
        Me.lblSearchTitle.Text = "🔍 Search Keyword:"

        Me.txtSearch.Font = New System.Drawing.Font("Segoe UI", 10)
        Me.txtSearch.Location = New System.Drawing.Point(12, 34)
        Me.txtSearch.Size = New System.Drawing.Size(300, 26)
        Me.txtSearch.Name = "txtSearch"

        Me.lblCat.AutoSize = True
        Me.lblCat.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Me.lblCat.Location = New System.Drawing.Point(325, 12)
        Me.lblCat.Name = "lblCat"
        Me.lblCat.Text = "Category"

        Me.cmbFilterCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFilterCategory.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.cmbFilterCategory.Location = New System.Drawing.Point(325, 34)
        Me.cmbFilterCategory.Size = New System.Drawing.Size(180, 24)
        Me.cmbFilterCategory.Name = "cmbFilterCategory"

        Me.lblType.AutoSize = True
        Me.lblType.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Me.lblType.Location = New System.Drawing.Point(515, 12)
        Me.lblType.Name = "lblType"
        Me.lblType.Text = "Resource Type"

        Me.cmbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFilterType.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.cmbFilterType.Location = New System.Drawing.Point(515, 34)
        Me.cmbFilterType.Size = New System.Drawing.Size(140, 24)
        Me.cmbFilterType.Name = "cmbFilterType"

        Me.lblLevel.AutoSize = True
        Me.lblLevel.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Me.lblLevel.Location = New System.Drawing.Point(665, 12)
        Me.lblLevel.Name = "lblLevel"
        Me.lblLevel.Text = "Level"

        Me.cmbFilterLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFilterLevel.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.cmbFilterLevel.Location = New System.Drawing.Point(665, 34)
        Me.cmbFilterLevel.Size = New System.Drawing.Size(130, 24)
        Me.cmbFilterLevel.Name = "cmbFilterLevel"

        Me.btnSearch.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSearch.FlatAppearance.BorderSize = 0
        Me.btnSearch.Font = New System.Drawing.Font("Segoe UI", 9.0, System.Drawing.FontStyle.Bold)
        Me.btnSearch.ForeColor = System.Drawing.Color.White
        Me.btnSearch.Location = New System.Drawing.Point(805, 32)
        Me.btnSearch.Size = New System.Drawing.Size(100, 30)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Text = "🔍 Search"
        Me.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnClearSearch.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClearSearch.FlatAppearance.BorderSize = 0
        Me.btnClearSearch.Font = New System.Drawing.Font("Segoe UI", 9.0)
        Me.btnClearSearch.ForeColor = System.Drawing.Color.White
        Me.btnClearSearch.Location = New System.Drawing.Point(915, 32)
        Me.btnClearSearch.Size = New System.Drawing.Size(70, 30)
        Me.btnClearSearch.Name = "btnClearSearch"
        Me.btnClearSearch.Text = "Clear"
        Me.btnClearSearch.Cursor = System.Windows.Forms.Cursors.Hand

        Me.lblCount.AutoSize = True
        Me.lblCount.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Italic)
        Me.lblCount.ForeColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.lblCount.Location = New System.Drawing.Point(12, 72)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Text = "0 resource(s) found"

        ' dgvResources
        Me.dgvResources.AllowUserToAddRows = False
        Me.dgvResources.AllowUserToDeleteRows = False
        Me.dgvResources.BackgroundColor = System.Drawing.Color.White
        Me.dgvResources.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvResources.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.dgvResources.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White
        Me.dgvResources.ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 8.5, System.Drawing.FontStyle.Bold)
        Me.dgvResources.ColumnHeadersHeight = 40
        Me.dgvResources.DefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.dgvResources.RowTemplate.Height = 36
        Me.dgvResources.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 248, 255)
        Me.dgvResources.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvResources.Name = "dgvResources"
        Me.dgvResources.ReadOnly = True
        Me.dgvResources.RowHeadersVisible = False
        Me.dgvResources.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvResources.MultiSelect = False

        Me.colResID.Name = "colResID"         : Me.colResID.HeaderText = "ID"        : Me.colResID.Width = 40  : Me.colResID.Visible = False
        Me.colResTag.Name = "colResTag"       : Me.colResTag.HeaderText = "Tag"       : Me.colResTag.Width = 72
        Me.colResTitle.Name = "colResTitle"   : Me.colResTitle.HeaderText = "Title"   : Me.colResTitle.Width = 200 : Me.colResTitle.MinimumWidth = 140
        Me.colResCat.Name = "colResCat"       : Me.colResCat.HeaderText = "Category" : Me.colResCat.Width = 120 : Me.colResCat.MinimumWidth = 100
        Me.colResType.Name = "colResType"     : Me.colResType.HeaderText = "Type"    : Me.colResType.Width = 88 : Me.colResType.MinimumWidth = 80
        Me.colResLevel.Name = "colResLevel"   : Me.colResLevel.HeaderText = "Level"  : Me.colResLevel.Width = 96 : Me.colResLevel.MinimumWidth = 88
        Me.colResSubject.Name = "colResSubject" : Me.colResSubject.HeaderText = "Subject" : Me.colResSubject.Width = 100 : Me.colResSubject.MinimumWidth = 80
        Me.colResViews.Name = "colResViews"   : Me.colResViews.HeaderText = "Views"  : Me.colResViews.Width = 72 : Me.colResViews.MinimumWidth = 60
        Me.colResDate.Name = "colResDate"     : Me.colResDate.HeaderText = "Date Added" : Me.colResDate.Width = 108 : Me.colResDate.MinimumWidth = 100
        Me.colResAddedBy.Name = "colResAddedBy" : Me.colResAddedBy.HeaderText = "Added By" : Me.colResAddedBy.Width = 140 : Me.colResAddedBy.MinimumWidth = 120
        Me.dgvResources.Columns.AddRange(Me.colResID, Me.colResTag, Me.colResTitle, Me.colResCat,
                                         Me.colResType, Me.colResLevel, Me.colResSubject,
                                         Me.colResViews, Me.colResDate, Me.colResAddedBy)

        ' pnlButtons
        Me.pnlButtons.BackColor = System.Drawing.Color.FromArgb(245, 248, 252)
        Me.pnlButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlButtons.Controls.Add(Me.btnAdd)
        Me.pnlButtons.Controls.Add(Me.btnEdit)
        Me.pnlButtons.Controls.Add(Me.btnDelete)
        Me.pnlButtons.Controls.Add(Me.btnBookmark)
        Me.pnlButtons.Controls.Add(Me.btnRefresh)
        Me.pnlButtons.Controls.Add(Me.btnExport)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Height = 46
        Me.pnlButtons.Name = "pnlButtons"

        Dim btnFont As New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)

        Me.btnAdd.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAdd.FlatAppearance.BorderSize = 0
        Me.btnAdd.Font = btnFont : Me.btnAdd.ForeColor = System.Drawing.Color.White
        Me.btnAdd.Location = New System.Drawing.Point(8, 8)
        Me.btnAdd.Size = New System.Drawing.Size(110, 30)
        Me.btnAdd.Name = "btnAdd" : Me.btnAdd.Text = "Add New"
        Me.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnEdit.BackColor = System.Drawing.Color.FromArgb(41, 128, 185)
        Me.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnEdit.FlatAppearance.BorderSize = 0
        Me.btnEdit.Font = btnFont : Me.btnEdit.ForeColor = System.Drawing.Color.White
        Me.btnEdit.Location = New System.Drawing.Point(126, 8)
        Me.btnEdit.Size = New System.Drawing.Size(110, 30)
        Me.btnEdit.Name = "btnEdit" : Me.btnEdit.Text = "Edit"
        Me.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnDelete.BackColor = System.Drawing.Color.FromArgb(192, 57, 43)
        Me.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDelete.FlatAppearance.BorderSize = 0
        Me.btnDelete.Font = btnFont : Me.btnDelete.ForeColor = System.Drawing.Color.White
        Me.btnDelete.Location = New System.Drawing.Point(244, 8)
        Me.btnDelete.Size = New System.Drawing.Size(110, 30)
        Me.btnDelete.Name = "btnDelete" : Me.btnDelete.Text = "Delete"
        Me.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnBookmark.BackColor = System.Drawing.Color.FromArgb(243, 156, 18)
        Me.btnBookmark.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBookmark.FlatAppearance.BorderSize = 0
        Me.btnBookmark.Font = btnFont : Me.btnBookmark.ForeColor = System.Drawing.Color.White
        Me.btnBookmark.Location = New System.Drawing.Point(362, 8)
        Me.btnBookmark.Size = New System.Drawing.Size(120, 30)
        Me.btnBookmark.Name = "btnBookmark" : Me.btnBookmark.Text = "Bookmark"
        Me.btnBookmark.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnRefresh.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRefresh.FlatAppearance.BorderSize = 0
        Me.btnRefresh.Font = btnFont : Me.btnRefresh.ForeColor = System.Drawing.Color.White
        Me.btnRefresh.Location = New System.Drawing.Point(494, 8)
        Me.btnRefresh.Size = New System.Drawing.Size(100, 30)
        Me.btnRefresh.Name = "btnRefresh" : Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand

        Me.btnExport.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExport.FlatAppearance.BorderSize = 0
        Me.btnExport.Font = btnFont : Me.btnExport.ForeColor = System.Drawing.Color.White
        Me.btnExport.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right
        Me.btnExport.Location = New System.Drawing.Point(820, 8)
        Me.btnExport.Size = New System.Drawing.Size(120, 30)
        Me.btnExport.Name = "btnExport" : Me.btnExport.Text = "Export All (CSV)"
        Me.btnExport.Cursor = System.Windows.Forms.Cursors.Hand

        ' frmManageResources
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(960, 640)
        Me.MinimumSize = New System.Drawing.Size(800, 500)
        Me.Controls.Add(Me.dgvResources)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.pnlSearch)
        Me.Controls.Add(Me.pnlHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.Name = "frmManageResources"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "EduVault - Browse Resources"

        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.pnlSearch.ResumeLayout(False)
        Me.pnlSearch.PerformLayout()
        CType(Me.dgvResources, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlHeader          As System.Windows.Forms.Panel
    Friend WithEvents lblFormTitle       As System.Windows.Forms.Label
    Friend WithEvents pnlSearch          As System.Windows.Forms.Panel
    Friend WithEvents lblSearchTitle     As System.Windows.Forms.Label
    Friend WithEvents txtSearch          As System.Windows.Forms.TextBox
    Friend WithEvents lblCat             As System.Windows.Forms.Label
    Friend WithEvents cmbFilterCategory  As System.Windows.Forms.ComboBox
    Friend WithEvents lblType            As System.Windows.Forms.Label
    Friend WithEvents cmbFilterType      As System.Windows.Forms.ComboBox
    Friend WithEvents lblLevel           As System.Windows.Forms.Label
    Friend WithEvents cmbFilterLevel     As System.Windows.Forms.ComboBox
    Friend WithEvents btnSearch          As System.Windows.Forms.Button
    Friend WithEvents btnClearSearch     As System.Windows.Forms.Button
    Friend WithEvents lblCount           As System.Windows.Forms.Label
    Friend WithEvents dgvResources       As System.Windows.Forms.DataGridView
    Friend WithEvents colResID           As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResTag          As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResTitle        As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResCat          As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResType         As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResLevel        As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResSubject      As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResViews        As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResDate         As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colResAddedBy      As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents pnlButtons         As System.Windows.Forms.Panel
    Friend WithEvents btnAdd             As System.Windows.Forms.Button
    Friend WithEvents btnEdit            As System.Windows.Forms.Button
    Friend WithEvents btnDelete          As System.Windows.Forms.Button
    Friend WithEvents btnBookmark        As System.Windows.Forms.Button
    Friend WithEvents btnRefresh         As System.Windows.Forms.Button
    Friend WithEvents btnExport          As System.Windows.Forms.Button

End Class
