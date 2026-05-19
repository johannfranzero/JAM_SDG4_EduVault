<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmReport
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
        Me.pnlHeader      = New System.Windows.Forms.Panel()
        Me.lblHeader      = New System.Windows.Forms.Label()
        Me.pnlFilter      = New System.Windows.Forms.Panel()
        Me.lblYearLbl     = New System.Windows.Forms.Label()
        Me.cmbYear        = New System.Windows.Forms.ComboBox()
        Me.lblMonthLbl    = New System.Windows.Forms.Label()
        Me.cmbMonth       = New System.Windows.Forms.ComboBox()
        Me.btnGenerate    = New System.Windows.Forms.Button()
        Me.btnExportPDF   = New System.Windows.Forms.Button()
        Me.btnExportCSV   = New System.Windows.Forms.Button()
        Me.btnClose       = New System.Windows.Forms.Button()
        Me.lblStatus      = New System.Windows.Forms.Label()
        Me.lblReportTitle = New System.Windows.Forms.Label()
        ' ── ReportViewer ──
        ' NOTE: Add ReportViewer to Toolbox first:
        '   Right-click Toolbox -> Choose Items -> find Microsoft.Reporting.WinForms.ReportViewer
        '   Then drag it onto this form and rename it to ReportViewer1.
        '   If unavailable, install NuGet: Microsoft.ReportingServices.ReportViewerControl.WinForms
        Me.ReportViewer1  = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.lblTotals      = New System.Windows.Forms.Label()
        Me.pnlHeader.SuspendLayout()
        Me.pnlFilter.SuspendLayout()
        ' ReportViewer (NuGet) does not implement ISupportInitialize - BeginInit removed
        Me.SuspendLayout()

        ' ── pnlHeader ──
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(28, 35, 64)
        Me.pnlHeader.Controls.Add(Me.lblHeader)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top : Me.pnlHeader.Height = 52 : Me.pnlHeader.Name = "pnlHeader"
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
        Me.lblHeader.ForeColor = System.Drawing.Color.White
        Me.lblHeader.Location = New System.Drawing.Point(12, 12) : Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Text = "Monthly Resource Access Summary Report"

        ' ── pnlFilter ──
        Me.pnlFilter.BackColor = System.Drawing.Color.White
        Me.pnlFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlFilter.Controls.AddRange(New System.Windows.Forms.Control() {
            Me.lblYearLbl, Me.cmbYear, Me.lblMonthLbl, Me.cmbMonth,
            Me.btnGenerate, Me.btnExportPDF, Me.btnExportCSV, Me.btnClose, Me.lblStatus})
        Me.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top : Me.pnlFilter.Height = 50 : Me.pnlFilter.Name = "pnlFilter"

        Me.lblYearLbl.Text = "Year:" : Me.lblYearLbl.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Me.lblYearLbl.AutoSize = True : Me.lblYearLbl.Location = New System.Drawing.Point(12, 15) : Me.lblYearLbl.Name = "lblYearLbl"

        Me.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbYear.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.cmbYear.Location = New System.Drawing.Point(52, 11) : Me.cmbYear.Size = New System.Drawing.Size(100, 28) : Me.cmbYear.Name = "cmbYear"

        Me.lblMonthLbl.Text = "Month:" : Me.lblMonthLbl.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Me.lblMonthLbl.AutoSize = True : Me.lblMonthLbl.Location = New System.Drawing.Point(176, 15) : Me.lblMonthLbl.Name = "lblMonthLbl"

        Me.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMonth.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.cmbMonth.Location = New System.Drawing.Point(232, 11) : Me.cmbMonth.Size = New System.Drawing.Size(140, 28) : Me.cmbMonth.Name = "cmbMonth"

        ' Generate button
        Me.btnGenerate.BackColor = System.Drawing.Color.FromArgb(25, 55, 109)
        Me.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnGenerate.FlatAppearance.BorderSize = 0
        Me.btnGenerate.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) : Me.btnGenerate.ForeColor = System.Drawing.Color.White
        Me.btnGenerate.Location = New System.Drawing.Point(352, 9) : Me.btnGenerate.Size = New System.Drawing.Size(140, 30)
        Me.btnGenerate.Name = "btnGenerate" : Me.btnGenerate.Text = "Generate Report" : Me.btnGenerate.Cursor = System.Windows.Forms.Cursors.Hand

        ' Export PDF button
        Me.btnExportPDF.BackColor = System.Drawing.Color.FromArgb(192, 57, 43)
        Me.btnExportPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnExportPDF.FlatAppearance.BorderSize = 0
        Me.btnExportPDF.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) : Me.btnExportPDF.ForeColor = System.Drawing.Color.White
        Me.btnExportPDF.Location = New System.Drawing.Point(500, 9) : Me.btnExportPDF.Size = New System.Drawing.Size(110, 30)
        Me.btnExportPDF.Name = "btnExportPDF" : Me.btnExportPDF.Text = "Export PDF" : Me.btnExportPDF.Cursor = System.Windows.Forms.Cursors.Hand

        ' Export CSV button
        Me.btnExportCSV.BackColor = System.Drawing.Color.FromArgb(39, 174, 96)
        Me.btnExportCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnExportCSV.FlatAppearance.BorderSize = 0
        Me.btnExportCSV.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) : Me.btnExportCSV.ForeColor = System.Drawing.Color.White
        Me.btnExportCSV.Location = New System.Drawing.Point(618, 9) : Me.btnExportCSV.Size = New System.Drawing.Size(110, 30)
        Me.btnExportCSV.Name = "btnExportCSV" : Me.btnExportCSV.Text = "Export CSV"
        Me.btnExportCSV.Cursor = System.Windows.Forms.Cursors.Hand : Me.btnExportCSV.Enabled = False

        ' Close button
        Me.btnClose.BackColor = System.Drawing.Color.FromArgb(127, 140, 141)
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat : Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.Font = New System.Drawing.Font("Segoe UI", 9) : Me.btnClose.ForeColor = System.Drawing.Color.White
        Me.btnClose.Location = New System.Drawing.Point(736, 9) : Me.btnClose.Size = New System.Drawing.Size(80, 30)
        Me.btnClose.Name = "btnClose" : Me.btnClose.Text = "Close" : Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand

        Me.lblStatus.AutoSize = True : Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 8.5)
        Me.lblStatus.ForeColor = System.Drawing.Color.DimGray
        Me.lblStatus.Location = New System.Drawing.Point(826, 16) : Me.lblStatus.Name = "lblStatus" : Me.lblStatus.Text = String.Empty

        ' ── lblReportTitle ──
        Me.lblReportTitle.AutoSize = True
        Me.lblReportTitle.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.lblReportTitle.ForeColor = System.Drawing.Color.FromArgb(25, 55, 109)
        Me.lblReportTitle.Location = New System.Drawing.Point(4, 108)
        Me.lblReportTitle.Name = "lblReportTitle" : Me.lblReportTitle.Text = "Monthly Access Summary"

        ' ── ReportViewer1 ──
        ' ProcessingMode = Local means the RDLC runs in-process (no SSRS server needed)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "EduVault.rptMonthlyAccess.rdlc"
        Me.ReportViewer1.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
        Me.ReportViewer1.Location = New System.Drawing.Point(4, 128)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.Size = New System.Drawing.Size(1000, 490)
        Me.ReportViewer1.TabIndex = 0
        Me.ReportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth

        ' ── lblTotals ──
        Me.lblTotals.AutoSize = True
        Me.lblTotals.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Me.lblTotals.ForeColor = System.Drawing.Color.FromArgb(25, 55, 109)
        Me.lblTotals.Location = New System.Drawing.Point(4, 628)
        Me.lblTotals.Name = "lblTotals" : Me.lblTotals.Text = String.Empty

        ' ── frmReport ──
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.FromArgb(240, 244, 250)
        Me.ClientSize = New System.Drawing.Size(1010, 650)
        Me.Controls.Add(Me.lblReportTitle)
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.lblTotals)
        Me.Controls.Add(Me.pnlFilter)
        Me.Controls.Add(Me.pnlHeader)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False : Me.Name = "frmReport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "EduVault - Monthly Access Report"

        Me.pnlHeader.ResumeLayout(False) : Me.pnlHeader.PerformLayout()
        Me.pnlFilter.ResumeLayout(False) : Me.pnlFilter.PerformLayout()
        ' ReportViewer (NuGet) does not implement ISupportInitialize - EndInit removed
        Me.ResumeLayout(False) : Me.PerformLayout()
    End Sub

    Friend WithEvents pnlHeader      As System.Windows.Forms.Panel
    Friend WithEvents lblHeader      As System.Windows.Forms.Label
    Friend WithEvents pnlFilter      As System.Windows.Forms.Panel
    Friend WithEvents lblYearLbl     As System.Windows.Forms.Label
    Friend WithEvents cmbYear        As System.Windows.Forms.ComboBox
    Friend WithEvents lblMonthLbl    As System.Windows.Forms.Label
    Friend WithEvents cmbMonth       As System.Windows.Forms.ComboBox
    Friend WithEvents btnGenerate    As System.Windows.Forms.Button
    Friend WithEvents btnExportPDF   As System.Windows.Forms.Button
    Friend WithEvents btnExportCSV   As System.Windows.Forms.Button
    Friend WithEvents btnClose       As System.Windows.Forms.Button
    Friend WithEvents lblStatus      As System.Windows.Forms.Label
    Friend WithEvents lblReportTitle As System.Windows.Forms.Label
    Friend WithEvents ReportViewer1  As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents lblTotals      As System.Windows.Forms.Label

End Class
