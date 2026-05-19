Imports System.Data.SqlClient

''' <summary>
''' Data Access Layer for tblAccessLog and tblBookmarks.
''' Handles access event logging, bookmark management, and report data retrieval.
''' </summary>
Public Class AccessLogRepository

    ' ─────────────────────────────────────────────────────────────
    ' ACCESS LOG OPERATIONS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Logs a resource access event (View, Bookmark, or Download).</summary>
    Public Sub LogAccess(userID As Integer, resourceID As Integer, accessType As String)
        If userID <= 0 Then Return
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblAccessLog (UserID, ResourceID, AccessType)
                     VALUES (@UserID, @ResourceID, @AccessType)"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID",     SqlDbType.Int).Value          = userID
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value          = resourceID
                    cmd.Parameters.Add("@AccessType", SqlDbType.NVarChar, 20).Value = accessType
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Console.WriteLine($"Warning: Access log failed: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Returns monthly access summary filtered by year/month.
    ''' Pass 0 to skip a filter. Data sourced from vwMonthlyAccessSummary.
    ''' </summary>
    Public Function GetMonthlyAccessSummary(filterYear As Integer, filterMonth As Integer) As List(Of MonthlyAccessSummary)
        Dim summaries As New List(Of MonthlyAccessSummary)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT AccessYear, AccessMonth, MonthName, ResourceID, ResourceTitle,
                            CategoryName, ResourceType, TotalAccesses, UniqueUsers
                     FROM vwMonthlyAccessSummary WHERE 1 = 1"
                If filterYear  > 0 Then sql &= " AND AccessYear  = @Year"
                If filterMonth > 0 Then sql &= " AND AccessMonth = @Month"
                sql &= " ORDER BY AccessYear DESC, AccessMonth DESC, TotalAccesses DESC"

                Using cmd As New SqlCommand(sql, conn)
                    If filterYear  > 0 Then cmd.Parameters.Add("@Year",  SqlDbType.Int).Value = filterYear
                    If filterMonth > 0 Then cmd.Parameters.Add("@Month", SqlDbType.Int).Value = filterMonth
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            summaries.Add(MapReaderToSummary(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetMonthlyAccessSummary: {ex.Message}", ex)
        End Try
        Return summaries
    End Function

    ''' <summary>Returns recent access history records for a specific user.</summary>
    Public Function GetUserAccessHistory(userID As Integer, topN As Integer) As List(Of AccessLog)
        If userID <= 0 Then Return New List(Of AccessLog)()
        Dim logs As New List(Of AccessLog)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT TOP (@TopN) al.LogID, al.UserID, u.FullName AS UserFullName,
                            al.ResourceID,
                            COALESCE(r.Title, '[Deleted Resource]') AS ResourceTitle,
                            COALESCE(c.CategoryName, '') AS CategoryName,
                            al.AccessDate, al.AccessType
                     FROM tblAccessLog al
                     INNER JOIN tblUsers      u ON al.UserID     = u.UserID
                     LEFT  JOIN tblResources  r ON al.ResourceID = r.ResourceID
                     LEFT  JOIN tblCategories c ON r.CategoryID  = c.CategoryID
                     WHERE al.UserID = @UserID
                     ORDER BY al.AccessDate DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@TopN",   SqlDbType.Int).Value = topN
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            logs.Add(MapReaderToLog(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetUserAccessHistory: {ex.Message}", ex)
        End Try
        Return logs
    End Function

    ''' <summary>Returns total access count system-wide for admin dashboard stats.</summary>
    Public Function GetTotalAccessCount() As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT COUNT(1) FROM tblAccessLog"
                Using cmd As New SqlCommand(sql, conn)
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in GetTotalAccessCount", ex)
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' V2 REPORTING METHODS
    ' ─────────────────────────────────────────────────────────────

    Public Function GetTopActiveUsers(days As Integer) As DataTable
        Dim dt As New DataTable()
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT TOP 10 u.FullName, u.Username, COUNT(al.LogID) AS AccessCount, MAX(al.AccessDate) AS LastActive
                                     FROM tblUsers u
                                     INNER JOIN tblAccessLog al ON u.UserID = al.UserID
                                     WHERE al.AccessDate >= DATEADD(DAY, -@Days, GETDATE())
                                     GROUP BY u.FullName, u.Username
                                     ORDER BY AccessCount DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Days", days)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in GetTopActiveUsers", ex)
        End Try
        Return dt
    End Function

    Public Function GetCategoryUsageBreakdown() As DataTable
        Dim dt As New DataTable()
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT c.CategoryName, COUNT(al.LogID) AS TotalAccesses
                                     FROM tblCategories c
                                     LEFT JOIN tblResources r ON c.CategoryID = r.CategoryID
                                     LEFT JOIN tblAccessLog al ON r.ResourceID = al.ResourceID
                                     GROUP BY c.CategoryName"
                Using cmd As New SqlCommand(sql, conn)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in GetCategoryUsageBreakdown", ex)
        End Try
        Return dt
    End Function

    Public Function GetDailyAccessCounts(days As Integer) As DataTable
        Dim dt As New DataTable()
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT CAST(AccessDate AS DATE) AS AccessDay, COUNT(LogID) AS AccessCount " &
                                    "FROM tblAccessLog " &
                                    "WHERE AccessDate >= CAST(DATEADD(day, -@Days, GETDATE()) AS DATE) " &
                                    "GROUP BY CAST(AccessDate AS DATE) " &
                                    "ORDER BY AccessDay ASC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Days", days)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in GetDailyAccessCounts", ex)
        End Try
        Return dt
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' BOOKMARK OPERATIONS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Returns all bookmarks for a specific user, newest first.</summary>
    Public Function GetBookmarksByUser(userID As Integer) As List(Of Bookmark)
        Dim bookmarks As New List(Of Bookmark)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT b.BookmarkID, b.UserID, b.ResourceID,
                            r.Title AS ResourceTitle, c.CategoryName,
                            r.ResourceType, r.URL AS ResourceURL, b.DateBookmarked
                     FROM tblBookmarks b
                     INNER JOIN tblResources  r ON b.ResourceID = r.ResourceID
                     INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
                     WHERE b.UserID = @UserID AND r.IsActive = 1
                     ORDER BY b.DateBookmarked DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            bookmarks.Add(MapReaderToBookmark(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetBookmarksByUser: {ex.Message}", ex)
        End Try
        Return bookmarks
    End Function

    ''' <summary>Adds a bookmark. Returns False silently if already bookmarked (duplicate).</summary>
    Public Function AddBookmark(userID As Integer, resourceID As Integer) As Boolean
        If userID <= 0 Then Return False
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "INSERT INTO tblBookmarks (UserID, ResourceID) VALUES (@UserID, @ResourceID)"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID",     SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            If ex.Number = 2627 Then Return False  ' Unique constraint - already bookmarked
            Throw New Exception($"Database error in AddBookmark: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>Removes a bookmark. Returns True on success.</summary>
    Public Function RemoveBookmark(userID As Integer, resourceID As Integer) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "DELETE FROM tblBookmarks WHERE UserID = @UserID AND ResourceID = @ResourceID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID",     SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in RemoveBookmark: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>Returns True if the specified user has already bookmarked this resource.</summary>
    Public Function IsBookmarked(userID As Integer, resourceID As Integer) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT COUNT(1) FROM tblBookmarks WHERE UserID = @UserID AND ResourceID = @ResourceID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID",     SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceID
                    Return CInt(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in IsBookmarked: {ex.Message}", ex)
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PRIVATE HELPERS
    ' ─────────────────────────────────────────────────────────────

    Private Function MapReaderToLog(reader As SqlDataReader) As AccessLog
        Dim a As New AccessLog()
        a.LogID         = reader.GetInt32(reader.GetOrdinal("LogID"))
        a.UserID        = reader.GetInt32(reader.GetOrdinal("UserID"))
        a.UserFullName  = reader.GetString(reader.GetOrdinal("UserFullName"))
        a.ResourceID    = reader.GetInt32(reader.GetOrdinal("ResourceID"))
        a.ResourceTitle = reader.GetString(reader.GetOrdinal("ResourceTitle"))
        a.CategoryName  = reader.GetString(reader.GetOrdinal("CategoryName"))
        a.AccessDate    = reader.GetDateTime(reader.GetOrdinal("AccessDate"))
        a.AccessType    = reader.GetString(reader.GetOrdinal("AccessType"))
        Return a
    End Function

    Private Function MapReaderToSummary(reader As SqlDataReader) As MonthlyAccessSummary
        Dim s As New MonthlyAccessSummary()
        s.AccessYear    = reader.GetInt32(reader.GetOrdinal("AccessYear"))
        s.AccessMonth   = reader.GetInt32(reader.GetOrdinal("AccessMonth"))
        s.MonthName     = reader.GetString(reader.GetOrdinal("MonthName"))
        s.ResourceID    = reader.GetInt32(reader.GetOrdinal("ResourceID"))
        s.ResourceTitle = reader.GetString(reader.GetOrdinal("ResourceTitle"))
        s.CategoryName  = reader.GetString(reader.GetOrdinal("CategoryName"))
        s.ResourceType  = reader.GetString(reader.GetOrdinal("ResourceType"))
        s.TotalAccesses = reader.GetInt32(reader.GetOrdinal("TotalAccesses"))
        s.UniqueUsers   = reader.GetInt32(reader.GetOrdinal("UniqueUsers"))
        Return s
    End Function

    Private Function MapReaderToBookmark(reader As SqlDataReader) As Bookmark
        Dim b As New Bookmark()
        b.BookmarkID     = reader.GetInt32(reader.GetOrdinal("BookmarkID"))
        b.UserID         = reader.GetInt32(reader.GetOrdinal("UserID"))
        b.ResourceID     = reader.GetInt32(reader.GetOrdinal("ResourceID"))
        b.ResourceTitle  = reader.GetString(reader.GetOrdinal("ResourceTitle"))
        b.CategoryName   = reader.GetString(reader.GetOrdinal("CategoryName"))
        b.ResourceType   = reader.GetString(reader.GetOrdinal("ResourceType"))
        b.ResourceURL    = If(reader.IsDBNull(reader.GetOrdinal("ResourceURL")), String.Empty,
                              reader.GetString(reader.GetOrdinal("ResourceURL")))
        b.DateBookmarked = reader.GetDateTime(reader.GetOrdinal("DateBookmarked"))
        Return b
    End Function

End Class
