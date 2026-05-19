Imports Microsoft.Reporting.WinForms

''' <summary>
''' frmReport - Monthly Resource Access Summary Report (Presentation Layer)
''' Admin-only form. Uses Microsoft Report Viewer with the embedded RDLC report.
'''
''' SETUP STEPS IN VISUAL STUDIO:
''' 1. Right-click project > Manage NuGet Packages
'''    Install: Microsoft.ReportingServices.ReportViewerControl.WinForms
'''    (or add reference to Microsoft.ReportViewer.WinForms.dll manually)
''' 2. Add rptMonthlyAccess.rdlc to the project (CODE/Reports/)
'''    Set its Build Action = "Embedded Resource" in Properties panel
''' 3. Add ReportViewer to Toolbox: right-click Toolbox > Choose Items > find ReportViewer
''' </summary>
Public Class frmReport

    Private ReadOnly _reportService As New ReportService()
    Private _autoGenerateOnShown As Boolean = True

    ' ─────────────────────────────────────────────────────────────
    ' FORM LOAD
    ' ─────────────────────────────────────────────────────────────

    Private Sub frmReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not Session.IsAdmin Then
            MessageBox.Show("Reports are available to administrators only.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If
        LoadYearFilter()
        LoadMonthFilter()
        btnExportCSV.Enabled = False

        ' Pre-select current month/year
        SelectCurrentPeriod()

        ConfigureReportLayout()
        LayoutFilterPanel()
        ApplyStyles()
    End Sub

    Private Sub frmReport_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        If Not _autoGenerateOnShown Then Return
        _autoGenerateOnShown = False
        GenerateReport()
    End Sub

    Private Sub ConfigureReportLayout()
        Dim isEmbedded As Boolean = Me.Parent IsNot Nothing

        pnlHeader.Dock = DockStyle.Top
        pnlFilter.Dock = DockStyle.Top
        lblTotals.Dock = DockStyle.Bottom
        lblTotals.Padding = New Padding(8, 4, 8, 8)
        lblTotals.AutoSize = False
        lblTotals.Height = 28

        lblReportTitle.Dock = DockStyle.Top
        lblReportTitle.Padding = New Padding(12, 8, 12, 0)
        lblReportTitle.AutoSize = False
        lblReportTitle.Height = 28

        ReportViewer1.Dock = DockStyle.Fill
        ReportViewer1.Margin = New Padding(8, 0, 8, 4)

        If isEmbedded Then
            pnlHeader.Visible = False
            btnClose.Text = "Back to Dashboard"
        End If

        RemoveHandler pnlFilter.Resize, AddressOf LayoutFilterPanel
        AddHandler pnlFilter.Resize, AddressOf LayoutFilterPanel

        ' Fill control must be at the back so top/bottom panels reserve space correctly.
        Me.Controls.SetChildIndex(ReportViewer1, 0)
        Me.Controls.SetChildIndex(lblTotals, 1)
        Me.Controls.SetChildIndex(lblReportTitle, 2)
        Me.Controls.SetChildIndex(pnlFilter, 3)
        Me.Controls.SetChildIndex(pnlHeader, 4)
    End Sub

    Private _flpReportFilters As FlowLayoutPanel = Nothing

    Private Sub LayoutFilterPanel()
        ' Use FlowLayoutPanel for responsive filter layout
        If _flpReportFilters Is Nothing Then
            _flpReportFilters = New FlowLayoutPanel()
            _flpReportFilters.Dock = DockStyle.Fill
            _flpReportFilters.WrapContents = True
            _flpReportFilters.AutoSize = False
            _flpReportFilters.Padding = New Padding(8, 4, 8, 4)

            ' Remove controls from pnlFilter and add to flow panel
            pnlFilter.Controls.Clear()
            pnlFilter.Height = 52
            pnlFilter.Padding = New Padding(0)

            Dim CreateGroup = Function(ctrl As Control, lbl As Label, groupWidth As Integer) As Panel
                                  Dim grp As New Panel()
                                  grp.Size = New Size(groupWidth, 40)
                                  grp.Margin = New Padding(4, 2, 4, 2)
                                  If lbl IsNot Nothing Then
                                      lbl.AutoSize = True
                                      lbl.Location = New Point(0, 2)
                                      grp.Controls.Add(lbl)
                                      ctrl.Location = New Point(0, 18)
                                  Else
                                      ctrl.Location = New Point(0, 6)
                                  End If
                                  ctrl.Size = New Size(groupWidth, 28)
                                  grp.Controls.Add(ctrl)
                                  Return grp
                              End Function

            _flpReportFilters.Controls.Add(CreateGroup(cmbYear, lblYearLbl, 110))
            _flpReportFilters.Controls.Add(CreateGroup(cmbMonth, lblMonthLbl, 150))

            ' Button group (no labels)
            btnGenerate.Size = New Size(130, 30)
            Dim pnlGen As New Panel() With {.Size = New Size(134, 40), .Margin = New Padding(4, 2, 4, 2)}
            btnGenerate.Location = New Point(0, 6)
            pnlGen.Controls.Add(btnGenerate)
            _flpReportFilters.Controls.Add(pnlGen)

            btnExportPDF.Size = New Size(100, 30)
            Dim pnlPdf As New Panel() With {.Size = New Size(104, 40), .Margin = New Padding(4, 2, 4, 2)}
            btnExportPDF.Location = New Point(0, 6)
            pnlPdf.Controls.Add(btnExportPDF)
            _flpReportFilters.Controls.Add(pnlPdf)

            btnExportCSV.Size = New Size(100, 30)
            Dim pnlCsv As New Panel() With {.Size = New Size(104, 40), .Margin = New Padding(4, 2, 4, 2)}
            btnExportCSV.Location = New Point(0, 6)
            pnlCsv.Controls.Add(btnExportCSV)
            _flpReportFilters.Controls.Add(pnlCsv)

            btnClose.Size = New Size(90, 30)
            Dim pnlClose As New Panel() With {.Size = New Size(94, 40), .Margin = New Padding(4, 2, 4, 2)}
            btnClose.Location = New Point(0, 6)
            pnlClose.Controls.Add(btnClose)
            _flpReportFilters.Controls.Add(pnlClose)

            lblStatus.AutoSize = True
            Dim pnlStatus As New Panel() With {.Size = New Size(200, 40), .Margin = New Padding(4, 2, 4, 2)}
            lblStatus.Location = New Point(4, 12)
            pnlStatus.Controls.Add(lblStatus)
            _flpReportFilters.Controls.Add(pnlStatus)

            pnlFilter.Controls.Add(_flpReportFilters)
        End If
    End Sub

    Private Function FindHostDashboard() As frmDashboard
        Dim host As Control = Me.Parent
        While host IsNot Nothing
            If TypeOf host Is frmDashboard Then
                Return DirectCast(host, frmDashboard)
            End If
            host = host.Parent
        End While
        Return Nothing
    End Function

    Private Sub ApplyStyles()
        StyleHelper.ApplyHeaderStyle(pnlHeader)
        lblHeader.Font = StyleHelper.HeaderFont

        pnlFilter.BackColor = StyleHelper.WhiteColor
        lblYearLbl.Font = StyleHelper.SubHeaderFont
        lblMonthLbl.Font = StyleHelper.SubHeaderFont

        StyleHelper.ApplyButtonStyle(btnGenerate)
        StyleHelper.ApplyButtonStyle(btnExportPDF, isDanger:=True)
        StyleHelper.ApplyButtonStyle(btnExportCSV, isAccent:=True)
        StyleHelper.ApplyButtonStyle(btnClose)

        lblReportTitle.Font = StyleHelper.SubHeaderFont
        lblReportTitle.ForeColor = StyleHelper.PrimaryColor
        lblTotals.Font = StyleHelper.NormalFont
        lblTotals.ForeColor = StyleHelper.PrimaryColor

        Me.BackColor = StyleHelper.ContentBg
    End Sub

    Private Sub LoadYearFilter()
        cmbYear.Items.Clear()
        cmbYear.Items.Add("All Years")
        Try
            Dim years As List(Of Integer) = _reportService.GetAvailableYears()
            For Each yr As Integer In years
                cmbYear.Items.Add(yr.ToString())
            Next
        Catch
            cmbYear.Items.Add(DateTime.Now.Year.ToString())
        End Try
        cmbYear.SelectedIndex = 0
    End Sub

    Private Sub LoadMonthFilter()
        cmbMonth.Items.Clear()
        cmbMonth.Items.Add("All Months")
        Dim months As String() = {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"}
        cmbMonth.Items.AddRange(months)
        cmbMonth.SelectedIndex = 0
    End Sub

    Private Sub SelectCurrentPeriod()
        ' Select current year
        Dim curYear As String = DateTime.Now.Year.ToString()
        For i As Integer = 0 To cmbYear.Items.Count - 1
            If cmbYear.Items(i).ToString() = curYear Then
                cmbYear.SelectedIndex = i : Exit For
            End If
        Next
        ' Select current month (index 1 = January, so current month = DateTime.Now.Month)
        If DateTime.Now.Month <= cmbMonth.Items.Count - 1 Then
            cmbMonth.SelectedIndex = DateTime.Now.Month
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' REPORT GENERATION - PRIMARY: Report Viewer (RDLC)
    ' ─────────────────────────────────────────────────────────────

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
        GenerateReport()
    End Sub

    Private Sub GenerateReport()
        Try
            lblStatus.ForeColor = System.Drawing.Color.DimGray
            lblStatus.Text = "Generating report..."
            Application.DoEvents()

            ' Parse filter selections
            Dim filterYear As Integer = 0
            Dim filterMonth As Integer = 0
            If cmbYear.SelectedIndex > 0 Then Integer.TryParse(cmbYear.SelectedItem.ToString(), filterYear)
            If cmbMonth.SelectedIndex > 0 Then filterMonth = cmbMonth.SelectedIndex  ' 1=Jan … 12=Dec

            ' Get DataTable from BLL
            Dim dt As DataTable = _reportService.GetMonthlyReportDataTable(filterYear, filterMonth)

            If dt.Rows.Count = 0 Then
                lblStatus.ForeColor = System.Drawing.Color.DarkOrange
                lblStatus.Text = "⚠ No data found for the selected period."
                ReportViewer1.Reset()
                btnExportCSV.Enabled = False
                _lastDataTable = Nothing
                Return
            End If

            ' ── Bind data to Report Viewer ──
            Dim reportDataSource As New ReportDataSource()
            reportDataSource.Name = "dsMonthlyAccess"   ' Must match RDLC DataSet name exactly
            reportDataSource.Value = dt

            With ReportViewer1.LocalReport
                .ReportEmbeddedResource = "EduVault.rptMonthlyAccess.rdlc"
                .DataSources.Clear()
                .DataSources.Add(reportDataSource)

                ' Pass filter parameters to the report header label
                Dim yearLabel As String = If(filterYear > 0, filterYear.ToString(), "All Years")
                Dim monthLabel As String = If(filterMonth > 0, cmbMonth.SelectedItem.ToString(), "All Months")
                lblReportTitle.Text = $"Monthly Access Summary - {monthLabel} {yearLabel}"

                .Refresh()
            End With

            ReportViewer1.RefreshReport()
            ReportViewer1.SetDisplayMode(DisplayMode.PrintLayout)

            ' Summary totals below the viewer
            Dim totalAccesses As Integer = dt.AsEnumerable().Sum(Function(r) CInt(r("TotalAccesses")))
            Dim peakUsers As Integer = If(dt.Rows.Count > 0, dt.AsEnumerable().Max(Function(r) CInt(r("UniqueUsers"))), 0)
            lblTotals.Text = $"Total Accesses: {totalAccesses:N0}   |   Peak Unique Users: {peakUsers:N0}   |   Records: {dt.Rows.Count:N0}"

            lblStatus.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96)
            lblStatus.Text = $"[OK] Report generated - {dt.Rows.Count} record(s)."
            btnExportCSV.Enabled = True

            ' Store DataTable for CSV export
            _lastDataTable = dt

        Catch ex As Exception
            lblStatus.ForeColor = System.Drawing.Color.Red
            lblStatus.Text = $"[X] Error: {ex.Message}"
            MessageBox.Show(
                $"Report generation failed:{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.NewLine}" &
                "Ensure the RDLC file 'rptMonthlyAccess.rdlc' is added to the project with Build Action = Embedded Resource.",
                "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' EXPORT
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Exports the current report data to PDF without the Report Viewer PDF renderer.</summary>
    Private Sub btnExportPDF_Click(sender As Object, e As EventArgs) Handles btnExportPDF.Click
        If _lastDataTable Is Nothing OrElse _lastDataTable.Rows.Count = 0 Then
            MessageBox.Show("Please generate a report first.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using saveDialog As New SaveFileDialog()
            saveDialog.Title = "Export Report as PDF"
            saveDialog.Filter = "PDF Files (*.pdf)|*.pdf"
            saveDialog.FileName = $"EduVault_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Try
                    _reportService.ExportToPdf(_lastDataTable, saveDialog.FileName, lblReportTitle.Text)
                    MessageBox.Show($"[OK] PDF exported to:{Environment.NewLine}{saveDialog.FileName}",
                                    "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show(
                        $"PDF export failed:{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.NewLine}" &
                        "You can use Export CSV as an alternative.",
                        "EduVault", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    ''' <summary>Exports current data to CSV as a fallback export option.</summary>
    Private Sub btnExportCSV_Click(sender As Object, e As EventArgs) Handles btnExportCSV.Click
        If _lastDataTable Is Nothing OrElse _lastDataTable.Rows.Count = 0 Then
            MessageBox.Show("Please generate a report first.", "EduVault",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using saveDialog As New SaveFileDialog()
            saveDialog.Title = "Export Report as CSV"
            saveDialog.Filter = "CSV Files (*.csv)|*.csv"
            saveDialog.FileName = $"EduVault_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Try
                    _reportService.ExportToCsv(_lastDataTable, saveDialog.FileName)
                    MessageBox.Show($"[OK] CSV exported to:{Environment.NewLine}{saveDialog.FileName}",
                                    "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show($"CSV export failed: {ex.Message}", "EduVault",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Dim dashboard As frmDashboard = FindHostDashboard()
        If dashboard IsNot Nothing Then
            dashboard.ReturnToDashboard()
        Else
            Me.Close()
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' STATE
    ' ─────────────────────────────────────────────────────────────
    Private _lastDataTable As DataTable = Nothing

End Class
