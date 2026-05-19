Imports System.Windows.Forms

Public Class frmCategoryManagement
    Inherits Form

    Private ReadOnly _catService As New CategoryService()
    Private dgv As DataGridView
    Private txtName As TextBox
    Private txtDesc As TextBox
    Private lblStatus As Label
    Private _selectedID As Integer = 0

    Private Sub frmCategoryManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
    End Sub

    Public Sub New()
        Me.Text = "Manage Categories"
        Me.Size = New Size(720, 480)
        Me.MinimumSize = New Size(600, 400)
        BuildUI()
        LoadCategories()
    End Sub

    Private pnlHeader As Panel

    Private Sub BuildUI()
        Me.BackColor = StyleHelper.ContentBg

        ' --- HEADER ---
        pnlHeader = New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconLibrary & " Manage Categories",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 16),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)

        ' --- TOP INPUT AREA ---
        Dim pnlInput As New Panel With {.Dock = DockStyle.Top, .Height = 90, .BackColor = StyleHelper.ContentBg}
        Dim tlpInput As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16, 16, 16, 8),
            .RowCount = 2,
            .ColumnCount = 2
        }
        tlpInput.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 240))
        tlpInput.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))

        tlpInput.Controls.Add(New Label With {.Text = "CATEGORY NAME", .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray}, 0, 0)
        tlpInput.Controls.Add(New Label With {.Text = "DESCRIPTION", .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray}, 1, 0)

        txtName = New TextBox With {.Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 9.5), .BorderStyle = BorderStyle.FixedSingle, .Margin = New Padding(0, 5, 15, 0)}
        tlpInput.Controls.Add(txtName, 0, 1)

        txtDesc = New TextBox With {.Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 9.5), .BorderStyle = BorderStyle.FixedSingle, .Margin = New Padding(0, 5, 0, 0)}
        tlpInput.Controls.Add(txtDesc, 1, 1)

        pnlInput.Controls.Add(tlpInput)

        lblStatus = New Label With {.Dock = DockStyle.Bottom, .AutoSize = False, .Height = 20, .TextAlign = ContentAlignment.MiddleLeft, .ForeColor = Color.DimGray, .Padding = New Padding(16, 0, 0, 0)}
        pnlInput.Controls.Add(lblStatus)

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

        Dim btnAdd As Button = BuildActionBtn("Add Category", StyleHelper.AccentBlue, Color.White, 120, New Padding(0, 0, 10, 0))
        Dim btnUpdate As Button = BuildActionBtn("Update Selected", Color.FromArgb(41, 128, 185), Color.White, 130, New Padding(0, 0, 10, 0))
        Dim btnDelete As Button = BuildActionBtn("Delete Selected", Color.FromArgb(192, 57, 43), Color.White, 130, New Padding(10, 0, 0, 0))
        
        flpLeft.Controls.Add(btnAdd)
        flpLeft.Controls.Add(btnUpdate)
        flpRight.Controls.Add(btnDelete)

        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnUpdate.Click, AddressOf BtnUpdate_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click

        ' --- DATAGRIDVIEW ---
        dgv = New DataGridView With {
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }
        StyleHelper.ApplyGridStyle(dgv)
        dgv.Columns.Add("colID", "ID")
        dgv.Columns.Add("colName", "Category")
        dgv.Columns.Add("colDesc", "Description")
        dgv.Columns.Add("colCount", "Resources")
        dgv.Columns("colID").Visible = False
        AddHandler dgv.SelectionChanged, AddressOf Dgv_SelectionChanged

        Me.Controls.Add(dgv)
        Me.Controls.Add(pnlInput)
        Me.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlHeader)
        
        dgv.BringToFront()
    End Sub

    Private Sub LoadCategories()
        dgv.Rows.Clear()
        For Each c In _catService.GetAllCategories()
            dgv.Rows.Add(c.CategoryID, c.CategoryName, c.Description, c.ResourceCount)
        Next
        lblStatus.Text = $"{dgv.Rows.Count} categories"
        ClearForm()
    End Sub

    Private Sub Dgv_SelectionChanged(sender As Object, e As EventArgs)
        If dgv.SelectedRows.Count = 0 Then Return
        _selectedID = CInt(dgv.SelectedRows(0).Cells("colID").Value)
        txtName.Text = dgv.SelectedRows(0).Cells("colName").Value.ToString()
        txtDesc.Text = dgv.SelectedRows(0).Cells("colDesc").Value.ToString()
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Dim err As String = String.Empty
        If _catService.AddCategory(txtName.Text, txtDesc.Text, err) Then
            MessageBox.Show("Category added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadCategories()
        Else
            lblStatus.ForeColor = Color.DarkRed
            lblStatus.Text = err
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
        If _selectedID <= 0 Then
            lblStatus.Text = "Select a category to update."
            Return
        End If
        Dim err As String = String.Empty
        If _catService.UpdateCategory(_selectedID, txtName.Text, txtDesc.Text, err) Then
            MessageBox.Show("Category updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadCategories()
        Else
            lblStatus.ForeColor = Color.DarkRed
            lblStatus.Text = err
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If _selectedID <= 0 Then Return
        Dim err As String = String.Empty
        If MessageBox.Show("Deactivate this category?", "Confirm", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
        If _catService.DeleteCategory(_selectedID, err) Then
            MessageBox.Show("Category removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadCategories()
        Else
            lblStatus.ForeColor = Color.DarkRed
            lblStatus.Text = err
        End If
    End Sub

    Private Sub ClearForm()
        _selectedID = 0
        txtName.Clear()
        txtDesc.Clear()
        lblStatus.ForeColor = Color.DimGray
    End Sub

End Class
