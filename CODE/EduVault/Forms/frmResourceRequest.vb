Imports System.Windows.Forms

Public Class frmResourceRequest
    Inherits Form

    Private ReadOnly _resourceService As New ResourceService()
    Private ReadOnly _catService As New CategoryService()

    Private Sub frmResourceRequest_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Me.TopLevel AndAlso pnlHeader IsNot Nothing Then
            pnlHeader.Visible = False
        End If
    End Sub

    Public Sub New()
        Me.Text = "Request a resource"
        Me.Size = New Size(540, 480)
        BuildUI()
    End Sub

    Private pnlHeader As Panel

    Private Sub BuildUI()
        Me.BackColor = StyleHelper.ContentBg

        ' --- HEADER ---
        pnlHeader = New Panel With {.Dock = DockStyle.Top, .Height = 52, .BackColor = StyleHelper.PrimaryColor}
        Dim lblTitle As New Label With {
            .Text = StyleHelper.IconAdd & " Request a Resource",
            .Font = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location = New Point(16, 16),
            .AutoSize = True
        }
        pnlHeader.Controls.Add(lblTitle)

        Dim flpMain As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }

        Dim txtTitle As New TextBox With {.Font = New Font("Segoe UI", 9.5), .BorderStyle = BorderStyle.FixedSingle, .Margin = New Padding(0, 5, 0, 15)}
        Dim txtDesc As New TextBox With {.Font = New Font("Segoe UI", 9.5), .BorderStyle = BorderStyle.FixedSingle, .Multiline = True, .Height = 100, .Margin = New Padding(0, 5, 0, 15)}
        Dim cmbCat As New ComboBox With {.Width = 300, .Font = New Font("Segoe UI", 9.5), .DropDownStyle = ComboBoxStyle.DropDownList, .Margin = New Padding(0, 5, 0, 15)}
        
        cmbCat.Items.Add("(Any category)")
        For Each c In _catService.GetAllCategories()
            cmbCat.Items.Add(c)
        Next
        cmbCat.DisplayMember = "CategoryName"
        cmbCat.SelectedIndex = 0

        Dim pnlTitle As New Panel With {.Width = 500, .Height = 60, .Margin = New Padding(0, 0, 0, 15)}
        pnlTitle.Controls.Add(New Label With {.Text = "TITLE", .Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray})
        txtTitle.Location = New Point(0, 20)
        txtTitle.Width = 460
        pnlTitle.Controls.Add(txtTitle)
        flpMain.Controls.Add(pnlTitle)

        Dim pnlDesc As New Panel With {.Width = 500, .Height = 140, .Margin = New Padding(0, 0, 0, 15)}
        pnlDesc.Controls.Add(New Label With {.Text = "WHY DO YOU NEED THIS?", .Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray})
        txtDesc.Location = New Point(0, 20)
        txtDesc.Width = 460
        pnlDesc.Controls.Add(txtDesc)
        flpMain.Controls.Add(pnlDesc)

        Dim pnlCat As New Panel With {.Width = 500, .Height = 60, .Margin = New Padding(0)}
        pnlCat.Controls.Add(New Label With {.Text = "CATEGORY (OPTIONAL)", .Location = New Point(0, 0), .AutoSize = True, .Font = New Font("Segoe UI", 8, FontStyle.Bold), .ForeColor = Color.DimGray})
        cmbCat.Location = New Point(0, 20)
        pnlCat.Controls.Add(cmbCat)
        flpMain.Controls.Add(pnlCat)

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

        Dim btnSubmit As Button = BuildActionBtn("Submit Request", StyleHelper.AccentBlue, Color.White, 120, New Padding(0, 0, 10, 0))
        Dim btnCancel As Button = BuildActionBtn("Cancel", Color.White, Color.DimGray, 80, New Padding(10, 0, 0, 0))
        
        flpLeft.Controls.Add(btnSubmit)
        flpRight.Controls.Add(btnCancel)

        AddHandler btnSubmit.Click,
            Sub()
                Dim catID As Integer? = Nothing
                If cmbCat.SelectedIndex > 0 Then catID = DirectCast(cmbCat.SelectedItem, Category).CategoryID
                Dim err As String = String.Empty
                If _resourceService.SubmitResourceRequest(Session.CurrentUserID, txtTitle.Text, txtDesc.Text, catID, err) Then
                    MessageBox.Show("Request submitted. An admin will review it.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                Else
                    MessageBox.Show(err, "Request Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End Sub
            
        AddHandler btnCancel.Click, Sub() Me.Close()

        Dim pnlCard As New Panel With {
            .Width = 600,
            .Height = 400,
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        ' Center the card when the form resizes
        AddHandler Me.Resize, Sub()
                                  pnlCard.Location = New Point((Me.ClientSize.Width - pnlCard.Width) \ 2, (Me.ClientSize.Height - pnlCard.Height) \ 2)
                              End Sub

        pnlCard.Controls.Add(flpMain)
        pnlCard.Controls.Add(pnlFooter)
        Me.Controls.Add(pnlCard)
        Me.Controls.Add(pnlHeader)
    End Sub

End Class
