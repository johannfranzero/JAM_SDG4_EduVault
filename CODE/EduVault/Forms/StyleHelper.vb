Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices

Public Class StyleHelper

    ' ── COLORS (Premium Palette) ──
    Public Shared ReadOnly PrimaryColor   As Color = Color.FromArgb(28, 35, 64)    ' Modern Navy
    Public Shared ReadOnly AccentColor    As Color = Color.FromArgb(39, 174, 96)   ' Green
    Public Shared ReadOnly DangerColor    As Color = Color.FromArgb(231, 76, 60)   ' Red
    Public Shared ReadOnly WarningColor   As Color = Color.FromArgb(243, 156, 18)  ' Orange
    Public Shared ReadOnly SidebarColor   As Color = Color.FromArgb(28, 35, 64)    ' Sidebar Navy
    Public Shared ReadOnly SidebarDark    As Color = Color.FromArgb(20, 26, 48)    ' Darker Sidebar
    Public Shared ReadOnly SidebarActive  As Color = Color.FromArgb(41, 128, 185)  ' Blue for Active State
    Public Shared ReadOnly AccentBlue     As Color = Color.FromArgb(37, 99, 235)   ' Action Blue (#2563eb)
    Public Shared ReadOnly ContentBg      As Color = Color.FromArgb(240, 244, 250) ' Softer Blue-Gray
    Public Shared ReadOnly WhiteColor     As Color = Color.White
    Public Shared ReadOnly TextColor      As Color = Color.FromArgb(44, 62, 80)
    Public Shared ReadOnly TextMuted      As Color = Color.FromArgb(127, 140, 141)

    ' ── GUEST BANNER ──
    Public Shared ReadOnly GuestBannerBg  As Color = Color.FromArgb(255, 243, 205)
    Public Shared ReadOnly GuestBannerFg  As Color = Color.FromArgb(133, 100, 4)

    ' ── PILL & BADGE COLORS ──
    Public Shared ReadOnly PillGreenBg    As Color = Color.FromArgb(230, 244, 234)
    Public Shared ReadOnly PillGreenText  As Color = Color.FromArgb(19, 115, 51)
    Public Shared ReadOnly PillBlueBg     As Color = Color.FromArgb(232, 240, 254)
    Public Shared ReadOnly PillBlueText   As Color = Color.FromArgb(25, 103, 210)
    Public Shared ReadOnly PillPinkBg     As Color = Color.FromArgb(252, 232, 230)
    Public Shared ReadOnly PillPinkText   As Color = Color.FromArgb(197, 34, 31)
    Public Shared ReadOnly PillGrayBg     As Color = Color.FromArgb(241, 239, 232)
    Public Shared ReadOnly PillGrayText   As Color = Color.FromArgb(68, 68, 65)
    
    Public Shared ReadOnly SelectionLightGreen As Color = Color.FromArgb(234, 243, 222)
    
    Public Shared ReadOnly ValueGreen     As Color = Color.FromArgb(39, 174, 96)
    Public Shared ReadOnly ValueAmber     As Color = Color.FromArgb(243, 156, 18)

    ' ── FONTS ──
    Public Shared ReadOnly HeaderFont     As New Font("Segoe UI Variable Display", 14, FontStyle.Bold)
    Public Shared ReadOnly SubHeaderFont  As New Font("Segoe UI Variable Display", 10, FontStyle.Bold)
    Public Shared ReadOnly NormalFont     As New Font("Segoe UI Variable Text", 9)
    Public Shared ReadOnly SmallFont      As New Font("Segoe UI Variable Text", 8)
    Public Shared ReadOnly IconFont       As New Font("Segoe MDL2 Assets", 10)
    Public Shared ReadOnly NavIconFont    As New Font("Segoe MDL2 Assets", 11)

    ' ── SEGOE MDL2 ICON GLYPHS ──
    Public Shared ReadOnly IconHome       As String = ChrW(&HE80F)  ' Home / Dashboard
    Public Shared ReadOnly IconLibrary    As String = ChrW(&HE8F1)  ' Library / Resources
    Public Shared ReadOnly IconBookmark   As String = ChrW(&HE728)  ' Favorite Star
    Public Shared ReadOnly IconPeople     As String = ChrW(&HE77B)  ' People / Users
    Public Shared ReadOnly IconChart      As String = ChrW(&HE9F9)  ' Chart / Reports
    Public Shared ReadOnly IconSearch     As String = ChrW(&HE721)  ' Search
    Public Shared ReadOnly IconAdd        As String = ChrW(&HE710)  ' Add / Plus
    Public Shared ReadOnly IconEdit       As String = ChrW(&HE70F)  ' Edit / Pencil
    Public Shared ReadOnly IconDelete     As String = ChrW(&HE74D)  ' Delete / Trash
    Public Shared ReadOnly IconExport     As String = ChrW(&HE78C)  ' Save / Export
    Public Shared ReadOnly IconRefresh    As String = ChrW(&HE72C)  ' Refresh
    Public Shared ReadOnly IconSettings   As String = ChrW(&HE713)  ' Settings / Gear
    Public Shared ReadOnly IconBackup     As String = ChrW(&HE895)  ' Cloud / Backup
    Public Shared ReadOnly IconLogs       As String = ChrW(&HE7BA)  ' Event Log
    Public Shared ReadOnly IconLogout     As String = ChrW(&HE7E8)  ' Sign Out
    Public Shared ReadOnly IconWarning    As String = ChrW(&HE7BA)  ' Warning
    Public Shared ReadOnly IconTrending   As String = ChrW(&HE8E6)  ' Trending / Up

    ''' <summary>Styles a button for a flat, modern look.</summary>
    Public Shared Sub ApplyButtonStyle(btn As Button, Optional isAccent As Boolean = False, Optional isDanger As Boolean = False)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.Cursor = Cursors.Hand
        btn.Font = SubHeaderFont
        btn.Height = 36 ' Reduced height to fit properly in panels
        
        If isAccent Then
            btn.BackColor = AccentColor
            btn.ForeColor = WhiteColor
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 140, 76)
        ElseIf isDanger Then
            btn.BackColor = DangerColor
            btn.ForeColor = WhiteColor
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43)
        Else
            btn.BackColor = PrimaryColor
            btn.ForeColor = WhiteColor
            ' Darken by approx 10% for hover
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(12, 35, 75) 
        End If
    End Sub

    ''' <summary>Styles a header panel.</summary>
    Public Shared Sub ApplyHeaderStyle(pnl As Panel)
        pnl.BackColor = PrimaryColor
        pnl.ForeColor = WhiteColor
    End Sub

    ''' <summary>Styles a DataGridView for a clean look.</summary>
    Public Shared Sub ApplyGridStyle(dgv As DataGridView)
        dgv.BackgroundColor = WhiteColor
        dgv.BorderStyle = BorderStyle.None
        dgv.EnableHeadersVisualStyles = False
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        
        dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = WhiteColor
        dgv.ColumnHeadersDefaultCellStyle.Font = SubHeaderFont
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor
        
        dgv.DefaultCellStyle.BackColor = WhiteColor
        dgv.DefaultCellStyle.ForeColor = TextColor
        dgv.DefaultCellStyle.Font = NormalFont
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 245)
        dgv.DefaultCellStyle.SelectionForeColor = TextColor
        
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single
        dgv.GridColor = Color.FromArgb(235, 235, 235)
        
        dgv.RowHeadersVisible = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.RowTemplate.Height = 32
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253)
    End Sub

    ' ── INPUT STYLING ──

    Private Const EM_SETCUEBANNER As Integer = &H1501

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer, <MarshalAs(UnmanagedType.LPWStr)> lParam As String) As Int32
    End Function

    ''' <summary>
    ''' Sets a native placeholder (cue banner) for a TextBox.
    ''' </summary>
    Public Shared Sub SetPlaceholder(txt As TextBox, placeholder As String)
        SendMessage(txt.Handle, EM_SETCUEBANNER, 0, placeholder)
    End Sub

    ''' <summary>
    ''' Applies a modern look to a TextBox by wrapping it in a styled Panel.
    ''' This enables custom border colors and a more 'premium' focus state.
    ''' </summary>
    Public Shared Sub ApplyModernInputStyle(txt As TextBox)
        Dim parent = txt.Parent
        If parent Is Nothing Then Return

        ' Create the wrapper panel (the border)
        Dim pnlBorder As New Panel()
        pnlBorder.Size = New Size(txt.Width, 42) ' Modern airy height
        pnlBorder.Location = txt.Location
        pnlBorder.BackColor = Color.FromArgb(220, 225, 230) ' Light consistent border
        pnlBorder.Padding = New Padding(1) ' 1px border thickness
        pnlBorder.Name = "pnlBorder_" & txt.Name

        ' Adjust the TextBox (Simulate padding with Location)
        txt.BorderStyle = BorderStyle.None
        txt.Width = pnlBorder.Width - 20
        txt.Location = New Point(10, 11) ' Increased vertical and horizontal padding
        txt.Font = New Font("Segoe UI Variable Text", 10.5)
        txt.BackColor = WhiteColor

        ' Re-nest the control
        parent.Controls.Remove(txt)
        pnlBorder.Controls.Add(txt)
        parent.Controls.Add(pnlBorder)

        ' Bring to front to ensure visibility
        pnlBorder.BringToFront()
    End Sub

    ''' <summary>
    ''' Updates the border color of the wrapper panel based on focus.
    ''' </summary>
    Public Shared Sub UpdateInputFocus(txt As TextBox, hasFocus As Boolean)
        Dim pnl = TryCast(txt.Parent, Panel)
        If pnl IsNot Nothing AndAlso pnl.Name.StartsWith("pnlBorder_") Then
            If hasFocus Then
                pnl.BackColor = PrimaryColor ' Blue border
                txt.BackColor = Color.FromArgb(250, 252, 255)
            Else
                pnl.BackColor = Color.FromArgb(200, 200, 200)
                txt.BackColor = WhiteColor
            End If
        End If
    End Sub

    ''' <summary>
    ''' Applies navigation styling to a sidebar button, including active state.
    ''' </summary>
    Public Shared Sub ApplyNavStyle(btn As Button, isActive As Boolean)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.TextAlign = ContentAlignment.MiddleLeft
        btn.Padding = New Padding(14, 0, 0, 0)
        btn.Font = New Font("Segoe UI", 9.75) ' approx 13px
        
        If isActive Then
            btn.BackColor = Color.FromArgb(33, 53, 84) ' Blue tint
            btn.ForeColor = WhiteColor
        Else
            btn.BackColor = SidebarColor
            btn.ForeColor = Color.FromArgb(200, 225, 255)
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 55, 90)
        End If
    End Sub

    ''' <summary>
    ''' Draws a clean, subtle border for a panel. Use this in the Paint event handler.
    ''' </summary>
    Public Shared Sub DrawCardShadow(sender As Object, e As PaintEventArgs)
        Dim pnl As Panel = TryCast(sender, Panel)
        If pnl Is Nothing Then Return

        ' Draw crisp subtle border instead of messy shadows
        Using p As New Pen(Color.FromArgb(210, 215, 225), 1)
            e.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1)
        End Using
    End Sub

    ''' <summary>
    ''' Creates a rounded rectangle path for clipping or drawing.
    ''' </summary>
    Public Shared Function GetRoundedPath(rect As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim arcRect As New Rectangle(rect.Location, New Size(radius, radius))

        ' Top left
        path.AddArc(arcRect, 180, 90)
        
        ' Top right
        arcRect.X = rect.Right - radius
        path.AddArc(arcRect, 270, 90)
        
        ' Bottom right
        arcRect.Y = rect.Bottom - radius
        path.AddArc(arcRect, 0, 90)
        
        ' Bottom left
        arcRect.X = rect.Left
        path.AddArc(arcRect, 90, 90)

        path.CloseFigure()
        Return path
    End Function

    ''' <summary>
    ''' Draws a rounded colored pill for tags and types.
    ''' </summary>
    Public Shared Sub DrawPill(g As Graphics, rect As Rectangle, text As String, bg As Color, fg As Color, font As Font)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Using path As GraphicsPath = GetRoundedPath(rect, 10) ' Pill radius
            Using brush As New SolidBrush(bg)
                g.FillPath(brush, path)
            End Using
        End Using
        TextRenderer.DrawText(g, text, font, rect, fg, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    ''' <summary>
    ''' Draws a mini progress bar for the Views column.
    ''' </summary>
    Public Shared Sub DrawMiniBar(g As Graphics, rect As Rectangle, value As Integer, maxValue As Integer, accent As Color)
        g.SmoothingMode = SmoothingMode.None
        Dim pct As Single = If(maxValue > 0, Math.Min(1.0F, value / maxValue), 0)
        Dim barWidth As Integer = CInt(rect.Width * pct)
        
        Using bgBrush As New SolidBrush(Color.FromArgb(230, 230, 230))
            g.FillRectangle(bgBrush, rect)
        End Using
        If barWidth > 0 Then
            Using fgBrush As New SolidBrush(accent)
                g.FillRectangle(fgBrush, New Rectangle(rect.X, rect.Y, barWidth, rect.Height))
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Draws the circular Avatar with initials.
    ''' </summary>
    Public Shared Sub DrawAvatar(g As Graphics, rect As Rectangle, initials As String)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Using brush As New SolidBrush(Color.FromArgb(41, 128, 185))
            g.FillEllipse(brush, rect)
        End Using
        Using font As New Font("Segoe UI", 9, FontStyle.Bold)
            TextRenderer.DrawText(g, initials, font, rect, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Using
    End Sub

    ''' <summary>
    ''' Sets the Region of a control to be rounded.
    ''' </summary>
    Public Shared Sub ApplyRoundedRegion(ctrl As Control, radius As Integer)
        Dim rect As New Rectangle(0, 0, ctrl.Width, ctrl.Height)
        Dim path As GraphicsPath = GetRoundedPath(rect, radius)
        ctrl.Region = New Region(path)
    End Sub

End Class
