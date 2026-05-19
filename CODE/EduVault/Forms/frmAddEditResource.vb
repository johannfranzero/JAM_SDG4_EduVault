''' <summary>
''' frmAddEditResource - Add or Edit a Resource (Presentation Layer)
''' Pass Nothing to constructor for Add mode; pass a Resource object for Edit mode.
''' </summary>
Public Class frmAddEditResource

    Private ReadOnly _resourceService As New ResourceService()
    Private ReadOnly _categoryService As New CategoryService()
    Private ReadOnly _editingResource  As Resource   ' Nothing = Add mode
    Private ReadOnly _isEditMode       As Boolean

    ''' <summary>
    ''' Constructor. Pass Nothing for Add mode, or a Resource object for Edit mode.
    ''' </summary>
    Public Sub New(resourceToEdit As Resource)
        InitializeComponent()
        _editingResource = resourceToEdit
        _isEditMode      = (resourceToEdit IsNot Nothing)
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' FORM LOAD
    ' ─────────────────────────────────────────────────────────────

    Private Sub frmAddEditResource_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCategories()

        If _isEditMode Then
            Me.Text = "EduVault - Edit Resource"
            lblFormTitle.Text = "Edit Resource"
            btnSubmit.Text = "Save Changes"
            PopulateFields(_editingResource)
        Else
            Me.Text = "EduVault - Add New Resource"
            lblFormTitle.Text = "Add New Resource"
            btnSubmit.Text = "Add Resource"
        End If
        BuildRedesignedUI()
    End Sub

    Private Sub BuildRedesignedUI()
        Me.BackColor = Color.FromArgb(248, 250, 255)
        pnlBody.BackColor = Color.Transparent
        
        ' Header
        pnlHeader.BackColor = Color.White
        pnlHeader.Height = 70
        lblFormTitle.Font = New Font("Segoe UI Variable Display", 16, FontStyle.Bold)
        lblFormTitle.ForeColor = StyleHelper.PrimaryColor
        lblFormTitle.Location = New Point(24, 20)
        If _isEditMode Then
            lblFormTitle.Text = "📄 Edit Resource"
        Else
            lblFormTitle.Text = "📄 Add New Resource"
        End If

        Dim hdrLine As New Panel With {.BackColor = Color.FromArgb(230, 235, 240), .Height = 1, .Dock = DockStyle.Bottom}
        pnlHeader.Controls.Add(hdrLine)

        ' Footer
        pnlFooter.BackColor = Color.White
        pnlFooter.Height = 70
        Dim ftrLine As New Panel With {.BackColor = Color.FromArgb(230, 235, 240), .Height = 1, .Dock = DockStyle.Top}
        pnlFooter.Controls.Add(ftrLine)

        btnSubmit.BackColor = Color.FromArgb(41, 128, 185)
        btnSubmit.ForeColor = Color.White
        btnSubmit.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
        btnSubmit.Size = New Size(140, 38)
        btnSubmit.Location = New Point(pnlFooter.Width - 160, 16)
        btnSubmit.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnSubmit.FlatStyle = FlatStyle.Flat
        btnSubmit.FlatAppearance.BorderSize = 0

        btnCancel.BackColor = Color.White
        btnCancel.ForeColor = Color.DimGray
        btnCancel.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
        btnCancel.Size = New Size(90, 38)
        btnCancel.Location = New Point(btnSubmit.Left - 100, 16)
        btnCancel.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderColor = Color.LightGray
        btnCancel.FlatAppearance.BorderSize = 1

        ' Body Styling
        pnlBody.Controls.Clear() ' Clear to rebuild cleanly
        
        Dim tlpMain As New TableLayoutPanel()
        tlpMain.ColumnCount = 2
        tlpMain.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
        tlpMain.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
        tlpMain.RowCount = 1
        tlpMain.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        tlpMain.Dock = DockStyle.Fill
        tlpMain.Padding = New Padding(20, 20, 20, 20)
        pnlBody.Controls.Add(tlpMain)

        Dim CreateCard = Function(title As String) As Panel
            Dim pnl As New Panel()
            pnl.Dock = DockStyle.Fill
            pnl.BackColor = Color.White
            pnl.BorderStyle = BorderStyle.FixedSingle
            
            Dim lblT As New Label()
            lblT.Text = title
            lblT.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lblT.ForeColor = StyleHelper.PrimaryColor
            lblT.Location = New Point(16, 16)
            lblT.AutoSize = True
            pnl.Controls.Add(lblT)

            Dim line As New Panel With {.BackColor = Color.FromArgb(240, 240, 240), .Height = 1, .Location = New Point(16, 44), .Width = pnl.Width - 32, .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right}
            pnl.Controls.Add(line)
            
            AddHandler pnl.Resize, Sub() line.Width = pnl.Width - 32
            
            Return pnl
        End Function

        Dim StyleLabel = Sub(lbl As Label)
            lbl.Font = New Font("Segoe UI", 8.5, FontStyle.Bold)
            lbl.ForeColor = Color.FromArgb(100, 115, 130)
            lbl.BackColor = Color.White
            lbl.AutoSize = True
        End Sub

        Dim StyleControl = Sub(ctrl As Control)
            ctrl.Font = New Font("Segoe UI", 9.5)
            If TypeOf ctrl Is ComboBox Then DirectCast(ctrl, ComboBox).FlatStyle = FlatStyle.Flat
            If TypeOf ctrl Is TextBox Then DirectCast(ctrl, TextBox).BorderStyle = BorderStyle.FixedSingle
            If TypeOf ctrl Is RichTextBox Then DirectCast(ctrl, RichTextBox).BorderStyle = BorderStyle.FixedSingle
        End Sub

        ' Left Card: General Information
        Dim pnlLeft = CreateCard("General Information")
        pnlLeft.Margin = New Padding(0, 0, 10, 0)
        tlpMain.Controls.Add(pnlLeft, 0, 0)

        StyleLabel(lblTitle) : pnlLeft.Controls.Add(lblTitle) : lblTitle.Location = New Point(16, 60)
        StyleControl(txtTitle) : pnlLeft.Controls.Add(txtTitle) : txtTitle.Location = New Point(16, 80)
        
        StyleLabel(lblTags) : pnlLeft.Controls.Add(lblTags) : lblTags.Location = New Point(16, 124)
        StyleControl(txtTags) : pnlLeft.Controls.Add(txtTags) : txtTags.Location = New Point(16, 144)
        pnlLeft.Controls.Add(lblTagsHint) : lblTagsHint.Location = New Point(16, 172) : lblTagsHint.BackColor = Color.White
        
        StyleLabel(lblDescription) : pnlLeft.Controls.Add(lblDescription) : lblDescription.Location = New Point(16, 210)
        StyleControl(txtDescription) : pnlLeft.Controls.Add(txtDescription) : txtDescription.Location = New Point(16, 230)
        
        AddHandler pnlLeft.Resize, Sub()
            txtTitle.Width = pnlLeft.Width - 32
            txtTags.Width = pnlLeft.Width - 32
            txtDescription.Width = pnlLeft.Width - 32
            txtDescription.Height = Math.Max(60, pnlLeft.Height - 246)
        End Sub

        ' Right Card: Classification & Access
        Dim pnlRight = CreateCard("Classification & Access")
        pnlRight.Margin = New Padding(10, 0, 0, 0)
        tlpMain.Controls.Add(pnlRight, 1, 0)

        StyleLabel(lblCategory) : pnlRight.Controls.Add(lblCategory) : lblCategory.Location = New Point(16, 60)
        StyleControl(cmbCategory) : pnlRight.Controls.Add(cmbCategory) : cmbCategory.Location = New Point(16, 80)
        
        StyleLabel(lblResourceType) : pnlRight.Controls.Add(lblResourceType)
        StyleControl(cmbResourceType) : pnlRight.Controls.Add(cmbResourceType)
        
        StyleLabel(lblLevel) : pnlRight.Controls.Add(lblLevel) : lblLevel.Location = New Point(16, 124)
        StyleControl(cmbLevel) : pnlRight.Controls.Add(cmbLevel) : cmbLevel.Location = New Point(16, 144)
        
        StyleLabel(lblSubjectArea) : pnlRight.Controls.Add(lblSubjectArea)
        StyleControl(txtSubjectArea) : pnlRight.Controls.Add(txtSubjectArea)
        
        StyleLabel(lblURL) : pnlRight.Controls.Add(lblURL) : lblURL.Location = New Point(16, 188)
        StyleControl(txtURL) : pnlRight.Controls.Add(txtURL) : txtURL.Location = New Point(16, 208)
        
        StyleLabel(lblFilePath) : pnlRight.Controls.Add(lblFilePath) : lblFilePath.Location = New Point(16, 252)
        StyleControl(txtFilePath) : pnlRight.Controls.Add(txtFilePath) : txtFilePath.Location = New Point(16, 272)
        
        AddHandler pnlRight.Resize, Sub()
            Dim halfWidth = (pnlRight.Width - 44) \ 2
            cmbCategory.Width = halfWidth
            
            cmbResourceType.Location = New Point(cmbCategory.Right + 12, 80)
            cmbResourceType.Width = halfWidth
            lblResourceType.Location = New Point(cmbResourceType.Left, 60)
            
            cmbLevel.Width = halfWidth
            
            txtSubjectArea.Location = New Point(cmbLevel.Right + 12, 144)
            txtSubjectArea.Width = halfWidth
            lblSubjectArea.Location = New Point(txtSubjectArea.Left, 124)
            
            txtURL.Width = pnlRight.Width - 32
            txtFilePath.Width = pnlRight.Width - 32
        End Sub

        ' Validation Label in Footer
        pnlFooter.Controls.Add(lblValidationError)
        lblValidationError.Location = New Point(24, 25)
        lblValidationError.AutoSize = True
        lblValidationError.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
        
        ' Expand form to comfortably fit the sections side-by-side
        Me.Width = 850
        Me.Height = 550
        Me.MinimumSize = New Size(700, 500)
    End Sub

    Private Sub LoadCategories()
        Try
            cmbCategory.Items.Clear()
            cmbCategory.Items.Add(New Category() With {.CategoryID = 0, .CategoryName = "- Select Category -"})
            For Each cat As Category In _categoryService.GetAllCategories()
                cmbCategory.Items.Add(cat)
            Next
            cmbCategory.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show($"Error loading categories: {ex.Message}", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    ''' <summary>Populates all form fields with the values from the resource being edited.</summary>
    Private Sub PopulateFields(res As Resource)
        txtTitle.Text       = res.Title
        txtDescription.Text = res.Description
        txtURL.Text         = res.URL
        txtFilePath.Text    = res.FilePath
        txtSubjectArea.Text = res.SubjectArea
        txtTags.Text        = res.Tags

        ' Match category in ComboBox by CategoryID
        For i As Integer = 0 To cmbCategory.Items.Count - 1
            Dim cat As Category = TryCast(cmbCategory.Items(i), Category)
            If cat IsNot Nothing AndAlso cat.CategoryID = res.CategoryID Then
                cmbCategory.SelectedIndex = i
                Exit For
            End If
        Next

        ' Match resource type
        For i As Integer = 0 To cmbResourceType.Items.Count - 1
            If cmbResourceType.Items(i).ToString() = res.ResourceType Then
                cmbResourceType.SelectedIndex = i
                Exit For
            End If
        Next

        ' Match education level
        For i As Integer = 0 To cmbLevel.Items.Count - 1
            If cmbLevel.Items(i).ToString() = res.EducationLevel Then
                cmbLevel.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' BUTTON EVENTS
    ' ─────────────────────────────────────────────────────────────

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        lblValidationError.Text = String.Empty

        ' Build the Resource object from form fields
        Dim res As New Resource()
        res.Title          = txtTitle.Text.Trim()
        res.Description    = txtDescription.Text.Trim()
        res.URL            = txtURL.Text.Trim()
        res.FilePath       = txtFilePath.Text.Trim()
        res.SubjectArea    = txtSubjectArea.Text.Trim()
        res.Tags           = txtTags.Text.Trim()
        res.AddedBy        = Session.CurrentUser.UserID

        ' Get CategoryID from ComboBox selection
        Dim selectedCat As Category = TryCast(cmbCategory.SelectedItem, Category)
        res.CategoryID = If(selectedCat IsNot Nothing, selectedCat.CategoryID, 0)

        ' Get ResourceType
        res.ResourceType = If(cmbResourceType.SelectedIndex > 0,
                               cmbResourceType.SelectedItem.ToString(), String.Empty)

        ' Get EducationLevel
        res.EducationLevel = If(cmbLevel.SelectedIndex > 0,
                                 cmbLevel.SelectedItem.ToString(), String.Empty)

        ' ── UI VALIDATION ──
        If String.IsNullOrWhiteSpace(res.Title) Then
            ShowValidationError("Resource title is required.")
            txtTitle.Focus()
            Return
        End If

        If cmbCategory.SelectedIndex <= 0 Then
            ShowValidationError("Please select a category.")
            cmbCategory.Focus()
            Return
        End If

        If cmbResourceType.SelectedIndex <= 0 Then
            ShowValidationError("Please select a resource type.")
            cmbResourceType.Focus()
            Return
        End If

        ' Require at least one access method
        If String.IsNullOrWhiteSpace(res.URL) AndAlso String.IsNullOrWhiteSpace(res.FilePath) Then
            ShowValidationError("Please provide either a URL or a file path.")
            txtURL.Focus()
            Return
        End If

        ' ──────────────────
        
        Dim errorMessage As String = String.Empty

        Try
            Dim success As Boolean = False

            If _isEditMode Then
                res.ResourceID = _editingResource.ResourceID
                success = _resourceService.UpdateResource(res, errorMessage)
            Else
                Dim newID As Integer = _resourceService.AddResource(res, errorMessage)
                success = (newID > 0)
            End If

            If success Then
                NotificationHelper.ShowInfo(If(_isEditMode, "Resource updated!", "Resource added!"))
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                ShowValidationError(errorMessage)
            End If

        Catch ex As Exception
            ShowValidationError($"Unexpected error: {ex.Message}")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' HELPERS
    ' ─────────────────────────────────────────────────────────────

    Private Sub ShowValidationError(message As String)
        If String.IsNullOrEmpty(message) Then
            lblValidationError.Text = String.Empty
        Else
            lblValidationError.ForeColor = Color.FromArgb(231, 76, 60) ' Modern red
            lblValidationError.Text = $"⚠️ {message}"
        End If
    End Sub

End Class
