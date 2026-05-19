Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class frmResourceRequest
    Inherits Form

    Private ReadOnly _resourceService As New ResourceService()
    Private ReadOnly _catService As New CategoryService()

    ' ── Controls referenced across methods ──
    Private _txtTitle    As TextBox
    Private _txtDesc     As TextBox
    Private _cmbCat      As ComboBox
    Private _pnlTopBar   As Panel

    Public Sub New()
        Me.Text = "Request a Resource"
        Me.Size = New Size(780, 580)
        Me.MinimumSize = New Size(680, 520)
        Me.BackColor = StyleHelper.ContentBg
        Me.Font = New Font("Segoe UI", 9)
        BuildUI()
    End Sub

    Private Sub frmResourceRequest_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Hide top bar when embedded inside the dashboard panel
        If Not Me.TopLevel AndAlso _pnlTopBar IsNot Nothing Then
            _pnlTopBar.Visible = False
        End If
    End Sub

    ' ════════════════════════════════════════════════════
    ' BUILD UI
    ' ════════════════════════════════════════════════════
    Private Sub BuildUI()
        Me.Controls.Clear()

        ' ── TOP BAR ──────────────────────────────────────
        _pnlTopBar = New Panel With {
            .Dock    = DockStyle.Top,
            .Height  = 52,
            .BackColor = StyleHelper.PrimaryColor
        }

        Dim lblTitle As New Label With {
            .Text      = StyleHelper.IconAdd & "  Request a Resource",
            .Font      = New Font("Segoe UI", 12),
            .ForeColor = Color.White,
            .Location  = New Point(16, 14),
            .AutoSize  = True
        }
        _pnlTopBar.Controls.Add(lblTitle)
        Me.Controls.Add(_pnlTopBar)

        ' ── FOOTER ACTION BAR ─────────────────────────────
        Dim pnlFooter As New Panel With {
            .Dock        = DockStyle.Bottom,
            .Height      = 64,
            .BackColor   = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        Dim tlpFooter As New TableLayoutPanel With {
            .Dock        = DockStyle.Fill,
            .Padding     = New Padding(16, 12, 16, 12),
            .RowCount    = 1,
            .ColumnCount = 2
        }
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
        tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))

        Dim flpLeft  As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .WrapContents = False}
        Dim flpRight As New FlowLayoutPanel With {
            .Dock          = DockStyle.Fill,
            .WrapContents  = False,
            .FlowDirection = FlowDirection.RightToLeft
        }
        tlpFooter.Controls.Add(flpLeft,  0, 0)
        tlpFooter.Controls.Add(flpRight, 1, 0)

        Dim btnSubmit As Button = MakeBtn(StyleHelper.IconAdd & "  Submit Request",
                                          StyleHelper.AccentBlue, Color.White, 148)
        Dim btnCancel As Button = MakeBtn("Cancel", Color.White, Color.DimGray, 80,
                                          borderColor:=Color.FromArgb(200, 200, 200))

        flpLeft.Controls.Add(btnSubmit)
        flpRight.Controls.Add(btnCancel)

        pnlFooter.Controls.Add(tlpFooter)
        Me.Controls.Add(pnlFooter)

        ' ── SCROLLABLE CONTENT AREA ───────────────────────
        Dim pnlScroll As New Panel With {
            .Dock       = DockStyle.Fill,
            .AutoScroll = True,
            .BackColor  = StyleHelper.ContentBg,
            .Padding    = New Padding(32, 24, 32, 16)
        }
        Me.Controls.Add(pnlScroll)

        ' ── HERO INTRO CARD ──────────────────────────────
        Dim pnlHero As New Panel With {
            .Dock      = DockStyle.Top,
            .Height    = 74,
            .BackColor = StyleHelper.PrimaryColor,
            .Margin    = New Padding(0, 0, 0, 20)
        }
        ApplyRoundedRegion(pnlHero, 8)

        Dim lblHeroTitle As New Label With {
            .Text      = "Can't find what you need?",
            .Font      = New Font("Segoe UI", 13, FontStyle.Bold),
            .ForeColor = Color.White,
            .Location  = New Point(20, 12),
            .AutoSize  = True
        }
        Dim lblHeroSub As New Label With {
            .Text      = "Submit a request and an admin will review it. We'll try to source it for you.",
            .Font      = New Font("Segoe UI", 9),
            .ForeColor = Color.FromArgb(180, 200, 230),
            .Location  = New Point(22, 40),
            .AutoSize  = True
        }
        pnlHero.Controls.Add(lblHeroTitle)
        pnlHero.Controls.Add(lblHeroSub)
        pnlScroll.Controls.Add(pnlHero)
        pnlHero.BringToFront()

        ' ── FORM CARD ────────────────────────────────────
        Dim pnlCard As New Panel With {
            .Dock        = DockStyle.Top,
            .Height      = 370,
            .BackColor   = Color.White,
            .Padding     = New Padding(28, 24, 28, 20)
        }
        ApplyRoundedRegion(pnlCard, 8)
        DrawCardBorder(pnlCard)

        ' TITLE field
        Dim grpTitle As Panel = BuildFieldGroup(
            pnlCard,
            icon      := StyleHelper.IconEdit,
            labelText := "RESOURCE TITLE",
            topOffset := 24,
            height    := 78
        )
        _txtTitle = New TextBox With {
            .BorderStyle = BorderStyle.None,
            .Font        = New Font("Segoe UI", 10.5),
            .Dock        = DockStyle.Fill,
            .BackColor   = Color.White
        }
        StyleHelper.SetPlaceholder(_txtTitle, "e.g.  Introduction to Data Structures – MIT OpenCourseWare")
        Dim txtTitleWrap As Panel = WrapInput(_txtTitle, grpTitle, topOffset:=30, heightPx:=38)
        HookFocusColor(_txtTitle, txtTitleWrap)

        ' REASON field
        Dim grpReason As Panel = BuildFieldGroup(
            pnlCard,
            icon      := StyleHelper.IconLibrary,
            labelText := "WHY DO YOU NEED THIS?",
            topOffset := 120,
            height    := 130
        )
        _txtDesc = New TextBox With {
            .BorderStyle = BorderStyle.None,
            .Font        = New Font("Segoe UI", 10),
            .Multiline   = True,
            .Dock        = DockStyle.Fill,
            .BackColor   = Color.White
        }
        StyleHelper.SetPlaceholder(_txtDesc, "Describe the resource and why it would be useful to other students…")
        Dim txtDescWrap As Panel = WrapInput(_txtDesc, grpReason, topOffset:=30, heightPx:=88)
        HookFocusColor(_txtDesc, txtDescWrap)

        ' CATEGORY field
        Dim grpCat As Panel = BuildFieldGroup(
            pnlCard,
            icon      := StyleHelper.IconBookmark,
            labelText := "CATEGORY  (OPTIONAL)",
            topOffset := 268,
            height    := 68
        )
        _cmbCat = New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font          = New Font("Segoe UI", 10),
            .FlatStyle     = FlatStyle.Flat,
            .BackColor     = Color.White,
            .Height        = 28,
            .Width         = 320,
            .Location      = New Point(0, 28)
        }
        _cmbCat.Items.Add("(Any category)")
        For Each c In _catService.GetAllCategories()
            _cmbCat.Items.Add(c)
        Next
        _cmbCat.DisplayMember = "CategoryName"
        _cmbCat.SelectedIndex = 0
        grpCat.Controls.Add(_cmbCat)

        pnlScroll.Controls.Add(pnlCard)
        pnlCard.BringToFront()

        ' ── SPACER BETWEEN HERO AND CARD ─────────────────
        Dim pnlSpacer As New Panel With {.Dock = DockStyle.Top, .Height = 16, .BackColor = StyleHelper.ContentBg}
        pnlScroll.Controls.Add(pnlSpacer)
        pnlSpacer.BringToFront()

        ' ── WIRE BUTTONS ─────────────────────────────────
        AddHandler btnSubmit.Click, AddressOf OnSubmit
        AddHandler btnCancel.Click, Sub() Me.Close()

        ' Hover effects
        HookHover(btnSubmit, StyleHelper.AccentBlue, Color.FromArgb(29, 78, 216))
        HookHover(btnCancel, Color.White, Color.FromArgb(235, 235, 235))
    End Sub

    ' ════════════════════════════════════════════════════
    ' SUBMIT HANDLER
    ' ════════════════════════════════════════════════════
    Private Sub OnSubmit(sender As Object, e As EventArgs)
        Dim catID As Integer? = Nothing
        If _cmbCat.SelectedIndex > 0 Then
            catID = DirectCast(_cmbCat.SelectedItem, Category).CategoryID
        End If

        Dim err As String = String.Empty
        If _resourceService.SubmitResourceRequest(
                Session.CurrentUserID, _txtTitle.Text, _txtDesc.Text, catID, err) Then
            MessageBox.Show("Request submitted. An admin will review it.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            MessageBox.Show(err, "Request Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' ════════════════════════════════════════════════════
    ' HELPERS
    ' ════════════════════════════════════════════════════

    ''' <summary>Builds an icon + label group inside a card panel.</summary>
    Private Shared Function BuildFieldGroup(card As Panel,
                                             icon As String,
                                             labelText As String,
                                             topOffset As Integer,
                                             height As Integer) As Panel
        Dim grp As New Panel With {
            .Left       = card.Padding.Left,
            .Top        = topOffset,
            .Width      = card.Width - card.Padding.Horizontal,
            .Height     = height,
            .Anchor     = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right,
            .BackColor  = Color.Transparent
        }

        Dim lblIcon As New Label With {
            .Text      = icon,
            .Font      = New Font("Segoe MDL2 Assets", 9),
            .ForeColor = StyleHelper.AccentBlue,
            .Location  = New Point(0, 1),
            .AutoSize  = True
        }
        Dim lbl As New Label With {
            .Text      = labelText,
            .Font      = New Font("Segoe UI", 7.5, FontStyle.Bold),
            .ForeColor = Color.FromArgb(100, 116, 139),
            .Location  = New Point(22, 2),
            .AutoSize  = True
        }
        grp.Controls.Add(lblIcon)
        grp.Controls.Add(lbl)
        card.Controls.Add(grp)
        Return grp
    End Function

    ''' <summary>Wraps a TextBox in a styled border panel inside a group.</summary>
    Private Shared Function WrapInput(txt As TextBox,
                                       grp As Panel,
                                       topOffset As Integer,
                                       heightPx As Integer) As Panel
        Dim wrap As New Panel With {
            .Left       = 0,
            .Top        = topOffset,
            .Width      = grp.Width,
            .Height     = heightPx,
            .Anchor     = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right,
            .BackColor  = Color.FromArgb(220, 225, 234),
            .Padding    = New Padding(1)
        }
        Dim inner As New Panel With {
            .Dock      = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding   = New Padding(10, 6, 10, 6)
        }
        inner.Controls.Add(txt)
        wrap.Controls.Add(inner)
        grp.Controls.Add(wrap)
        Return wrap
    End Function

    ''' <summary>Changes border colour on TextBox focus/blur.</summary>
    Private Shared Sub HookFocusColor(txt As TextBox, wrap As Panel)
        AddHandler txt.Enter, Sub()
                                   wrap.BackColor = StyleHelper.AccentBlue
                                   wrap.Padding = New Padding(2)
                               End Sub
        AddHandler txt.Leave, Sub()
                                   wrap.BackColor = Color.FromArgb(220, 225, 234)
                                   wrap.Padding = New Padding(1)
                               End Sub
    End Sub

    ''' <summary>Creates a flat, modern button.</summary>
    Private Shared Function MakeBtn(text As String,
                                     bg As Color,
                                     fg As Color,
                                     width As Integer,
                                     Optional borderColor As Color = Nothing) As Button
        Dim b As New Button With {
            .Text      = text,
            .BackColor = bg,
            .ForeColor = fg,
            .FlatStyle = FlatStyle.Flat,
            .Size      = New Size(width, 36),
            .Font      = New Font("Segoe UI", 9, FontStyle.Regular),
            .Cursor    = Cursors.Hand,
            .Margin    = New Padding(0, 0, 8, 0)
        }
        b.FlatAppearance.BorderSize = If(borderColor = Nothing OrElse borderColor = Color.Empty, 0, 1)
        If borderColor <> Nothing AndAlso borderColor <> Color.Empty Then
            b.FlatAppearance.BorderColor = borderColor
        End If
        Return b
    End Function

    ''' <summary>Adds mouse-over colour swap to a button.</summary>
    Private Shared Sub HookHover(btn As Button, normal As Color, hover As Color)
        btn.FlatAppearance.MouseOverBackColor = hover
    End Sub

    ''' <summary>Applies a subtle 8-px rounded clip region to a panel.</summary>
    Private Shared Sub ApplyRoundedRegion(pnl As Panel, radius As Integer)
        Dim rect As New Rectangle(0, 0, pnl.Width, pnl.Height)
        Dim path As GraphicsPath = StyleHelper.GetRoundedPath(rect, radius)
        pnl.Region = New Region(path)
        ' Re-apply on resize so the clip stays correct
        AddHandler pnl.Resize,
            Sub()
                Dim r As New Rectangle(0, 0, pnl.Width, pnl.Height)
                pnl.Region = New Region(StyleHelper.GetRoundedPath(r, radius))
            End Sub
    End Sub

    ''' <summary>Registers a Paint handler that draws a 1-px border on a card.</summary>
    Private Shared Sub DrawCardBorder(pnl As Panel)
        AddHandler pnl.Paint,
            Sub(s As Object, ev As PaintEventArgs)
                Using pen As New Pen(Color.FromArgb(210, 218, 230), 1)
                    ev.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1)
                End Using
            End Sub
    End Sub

End Class
