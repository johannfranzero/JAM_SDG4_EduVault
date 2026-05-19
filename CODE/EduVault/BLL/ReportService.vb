''' <summary>
''' Business Logic Layer - Report Service.
''' Aggregates data from AccessLogRepository and formats it for reporting.
''' Returns DataTable objects ready to bind to Report Viewer or Crystal Reports.
''' </summary>
Public Class ReportService

    Private ReadOnly _logRepo As New AccessLogRepository()
    Private ReadOnly _resourceRepo As New ResourceRepository()

    ' ---------------------------------------------------------------
    ' MONTHLY ACCESS SUMMARY REPORT
    ' ---------------------------------------------------------------

    ''' <summary>
    ''' Returns a DataTable of the monthly access summary report.
    ''' Suitable for binding to Microsoft Report Viewer (RDLC).
    ''' Optionally filtered by year and month (pass 0 to include all).
    ''' </summary>
    Public Function GetMonthlyReportDataTable(filterYear As Integer,
                                              filterMonth As Integer) As DataTable
        Dim dt As New DataTable("MonthlyAccessReport")

        ' Define columns matching the report layout
        dt.Columns.Add("Period", GetType(String))
        dt.Columns.Add("ResourceTitle", GetType(String))
        dt.Columns.Add("CategoryName", GetType(String))
        dt.Columns.Add("ResourceType", GetType(String))
        dt.Columns.Add("TotalAccesses", GetType(Integer))
        dt.Columns.Add("UniqueUsers", GetType(Integer))

        Try
            Dim summaries As List(Of MonthlyAccessSummary) =
                _logRepo.GetMonthlyAccessSummary(filterYear, filterMonth)

            For Each s As MonthlyAccessSummary In summaries
                Dim row As DataRow = dt.NewRow()
                row("Period") = s.Period
                row("ResourceTitle") = s.ResourceTitle
                row("CategoryName") = s.CategoryName
                row("ResourceType") = s.ResourceType
                row("TotalAccesses") = s.TotalAccesses
                row("UniqueUsers") = s.UniqueUsers
                dt.Rows.Add(row)
            Next

        Catch ex As Exception
            Throw New Exception($"Report generation error: {ex.Message}", ex)
        End Try

        Return dt
    End Function

    ''' <summary>
    ''' Returns a raw list of MonthlyAccessSummary objects.
    ''' Use this for custom binding or export to CSV/Excel.
    ''' </summary>
    Public Function GetMonthlyAccessSummary(filterYear As Integer,
                                            filterMonth As Integer) As List(Of MonthlyAccessSummary)
        Return _logRepo.GetMonthlyAccessSummary(filterYear, filterMonth)
    End Function

    ' ---------------------------------------------------------------
    ' DASHBOARD STATS
    ' ---------------------------------------------------------------

    ''' <summary>
    ''' Returns key statistics for the admin dashboard summary panel.
    ''' Returns a Dictionary with named stat keys.
    ''' </summary>
    Public Function GetDashboardStats() As Dictionary(Of String, Integer)
        Dim stats As New Dictionary(Of String, Integer)()
        Try
            Dim allResources As List(Of Resource) = _resourceRepo.GetAllResources()

            stats("TotalResources") = allResources.Count
            stats("TotalEbooks") = allResources.Where(Function(r) r.ResourceType = "E-Book").Count
            stats("TotalVideos") = allResources.Where(Function(r) r.ResourceType = "Video").Count
            stats("TotalModules") = allResources.Where(Function(r) r.ResourceType = "Module").Count
            stats("TotalAccesses") = _logRepo.GetTotalAccessCount()
            stats("PopularResources") = allResources.Where(Function(r) r.IsPopular).Count
        Catch ex As Exception
            ' Return empty stats rather than crashing the dashboard
            stats.Clear()
            stats("TotalResources") = 0
            stats("TotalEbooks") = 0
            stats("TotalVideos") = 0
            stats("TotalModules") = 0
            stats("TotalAccesses") = 0
            stats("PopularResources") = 0
        End Try
        Return stats
    End Function

    ''' <summary>
    ''' Returns a DataTable with daily access counts for the past N days.
    ''' Used for the Admin Dashboard line chart.
    ''' </summary>
    Public Function GetDailyAccessCounts(days As Integer) As DataTable
        Return _logRepo.GetDailyAccessCounts(days)
    End Function

    ' ---------------------------------------------------------------
    ' EXPORT HELPERS
    ' ---------------------------------------------------------------

    ''' <summary>
    ''' Exports a DataTable to a CSV file at the specified file path.
    ''' Used as an alternative export when Crystal Reports is unavailable.
    ''' </summary>
    ''' <summary>Exports a DataTable to a simple PDF file (no Report Viewer renderer).</summary>
    Public Sub ExportToPdf(dt As DataTable, filePath As String, title As String)
        Try
            Dim writer As New SimplePdfTableWriter()
            writer.WriteTable(filePath, title, dt)
        Catch ex As Exception
            Throw New Exception($"PDF export error: {ex.Message}", ex)
        End Try
    End Sub

    Public Sub ExportToCsv(dt As DataTable, filePath As String)
        Try
            Dim sb As New System.Text.StringBuilder()

            ' Write header row
            Dim headers As String() = dt.Columns.Cast(Of DataColumn)() _
                                        .Select(Function(c) c.ColumnName).ToArray()
            sb.AppendLine(String.Join(",", headers))

            ' Write data rows -- wrap values in quotes to handle commas in data
            For Each row As DataRow In dt.Rows
                Dim values As String() = row.ItemArray _
                    .Select(Function(v) $"""{v}""").ToArray()
                sb.AppendLine(String.Join(",", values))
            Next

            System.IO.File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8)

        Catch ex As Exception
            Throw New Exception($"CSV export error: {ex.Message}", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Returns a list of all years for which there is access log data.
    ''' Used to populate the Year filter ComboBox on the report form.
    ''' </summary>
    Public Function GetAvailableYears() As List(Of Integer)
        Dim years As New List(Of Integer)()
        Dim summaries As List(Of MonthlyAccessSummary) =
            _logRepo.GetMonthlyAccessSummary(0, 0)
        For Each s As MonthlyAccessSummary In summaries
            If Not years.Contains(s.AccessYear) Then
                years.Add(s.AccessYear)
            End If
        Next
        years.Sort()
        years.Reverse()  ' Newest year first
        Return years
    End Function

    ''' <summary>
    ''' Triggers a database backup to the specified path.
    ''' </summary>
    Public Function BackupDatabase(filePath As String) As Boolean
        Dim backupRepo As New BackupRepository()
        Return backupRepo.BackupDatabase(filePath)
    End Function

End Class
