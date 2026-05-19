Imports System.Data.SqlClient

''' <summary>
''' Data Access Layer for tblResources.
''' Supports full CRUD, filtered search, and view count incrementing.
''' All queries use parameterized commands - no raw SQL string concatenation.
''' </summary>
Public Class ResourceRepository

    ' ─────────────────────────────────────────────────────────────
    ' READ OPERATIONS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Returns all active resources ordered by DateAdded descending.</summary>
    Public Function GetAllResources() As List(Of Resource)
        Dim resources As New List(Of Resource)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT r.ResourceID, r.Title, r.Description, r.CategoryID, c.CategoryName,
                            r.SubjectArea, r.ResourceType, r.URL, r.FilePath, r.EducationLevel,
                            r.Tags, r.AddedBy, u.FullName AS AddedByName,
                            r.DateAdded, r.IsActive, r.ViewCount, r.DownloadCount, r.ThumbnailPath, r.CurrentVersion
                     FROM tblResources r
                     INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
                     INNER JOIN tblUsers u ON r.AddedBy = u.UserID
                     WHERE r.IsActive = 1
                     ORDER BY r.DateAdded DESC"
                Using cmd As New SqlCommand(sql, conn)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            resources.Add(MapReaderToResource(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetAllResources: {ex.Message}", ex)
        End Try
        Return resources
    End Function

    ''' <summary>Returns a single resource by ResourceID. Returns Nothing if not found.</summary>
    Public Function GetResourceByID(resourceID As Integer) As Resource
        Dim res As Resource = Nothing
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT r.ResourceID, r.Title, r.Description, r.CategoryID, c.CategoryName,
                            r.SubjectArea, r.ResourceType, r.URL, r.FilePath, r.EducationLevel,
                            r.Tags, r.AddedBy, u.FullName AS AddedByName,
                            r.DateAdded, r.IsActive, r.ViewCount, r.DownloadCount, r.ThumbnailPath, r.CurrentVersion
                     FROM tblResources r
                     INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
                     INNER JOIN tblUsers u ON r.AddedBy = u.UserID
                     WHERE r.ResourceID = @ResourceID AND r.IsActive = 1"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceID
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then res = MapReaderToResource(reader)
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetResourceByID: {ex.Message}", ex)
        End Try
        Return res
    End Function

    ''' <summary>
    ''' Searches resources by keyword, category, type, and education level.
    ''' Pass 0 or empty string to skip any individual filter.
    ''' </summary>
    Public Function SearchResources(keyword As String, categoryID As Integer,
                                    resourceType As String, educationLevel As String) As List(Of Resource)
        Dim resources As New List(Of Resource)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT r.ResourceID, r.Title, r.Description, r.CategoryID, c.CategoryName,
                            r.SubjectArea, r.ResourceType, r.URL, r.FilePath, r.EducationLevel,
                            r.Tags, r.AddedBy, u.FullName AS AddedByName,
                            r.DateAdded, r.IsActive, r.ViewCount, r.DownloadCount, r.ThumbnailPath, r.CurrentVersion
                     FROM tblResources r
                     INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
                     INNER JOIN tblUsers u ON r.AddedBy = u.UserID
                     WHERE r.IsActive = 1"

                If Not String.IsNullOrWhiteSpace(keyword) Then
                    sql &= " AND (r.Title LIKE @Keyword OR r.Description LIKE @Keyword
                                  OR r.SubjectArea LIKE @Keyword OR r.Tags LIKE @Keyword)"
                End If
                If categoryID > 0 Then sql &= " AND r.CategoryID = @CategoryID"
                If Not String.IsNullOrWhiteSpace(resourceType) Then sql &= " AND r.ResourceType = @ResourceType"
                If Not String.IsNullOrWhiteSpace(educationLevel) Then sql &= " AND r.EducationLevel = @EducationLevel"
                sql &= " ORDER BY r.ViewCount DESC, r.DateAdded DESC"

                Using cmd As New SqlCommand(sql, conn)
                    If Not String.IsNullOrWhiteSpace(keyword) Then
                        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 1000).Value = $"%{keyword}%"
                    End If
                    If categoryID > 0 Then
                        cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryID
                    End If
                    If Not String.IsNullOrWhiteSpace(resourceType) Then
                        cmd.Parameters.Add("@ResourceType", SqlDbType.NVarChar, 50).Value = resourceType
                    End If
                    If Not String.IsNullOrWhiteSpace(educationLevel) Then
                        cmd.Parameters.Add("@EducationLevel", SqlDbType.NVarChar, 50).Value = educationLevel
                    End If
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            resources.Add(MapReaderToResource(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in SearchResources: {ex.Message}", ex)
        End Try
        Return resources
    End Function

    ''' <summary>Returns the top N most-viewed resources for the Dashboard trending panel.</summary>
    Public Function GetTopResources(topN As Integer) As List(Of Resource)
        Dim resources As New List(Of Resource)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT TOP (@TopN) r.ResourceID, r.Title, r.Description, r.CategoryID, c.CategoryName,
                            r.SubjectArea, r.ResourceType, r.URL, r.FilePath, r.EducationLevel,
                            r.Tags, r.AddedBy, u.FullName AS AddedByName,
                            r.DateAdded, r.IsActive, r.ViewCount, r.DownloadCount, r.ThumbnailPath, r.CurrentVersion
                     FROM tblResources r
                     INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
                     INNER JOIN tblUsers u ON r.AddedBy = u.UserID
                     WHERE r.IsActive = 1
                     ORDER BY r.ViewCount DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = topN
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            resources.Add(MapReaderToResource(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetTopResources: {ex.Message}", ex)
        End Try
        Return resources
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' WRITE OPERATIONS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Inserts a new resource. Returns the new ResourceID.</summary>
    Public Function AddResource(res As Resource) As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblResources
                       (Title, Description, CategoryID, SubjectArea, ResourceType,
                        URL, FilePath, EducationLevel, Tags, AddedBy)
                     VALUES
                       (@Title, @Description, @CategoryID, @SubjectArea, @ResourceType,
                        @URL, @FilePath, @EducationLevel, @Tags, @AddedBy);
                     SELECT CAST(SCOPE_IDENTITY() AS INT);"
                Using cmd As New SqlCommand(sql, conn)
                    BuildResourceParams(cmd, res)
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in AddResource: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>Updates all editable fields of an existing resource. Returns True on success.</summary>
    Public Function UpdateResource(res As Resource) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblResources
                     SET Title = @Title, Description = @Description, CategoryID = @CategoryID,
                         SubjectArea = @SubjectArea, ResourceType = @ResourceType, URL = @URL,
                         FilePath = @FilePath, EducationLevel = @EducationLevel, Tags = @Tags
                     WHERE ResourceID = @ResourceID"
                Using cmd As New SqlCommand(sql, conn)
                    BuildResourceParams(cmd, res)
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value = res.ResourceID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UpdateResource: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>
    ''' Soft-deletes a resource (sets IsActive = 0).
    ''' Access log history is preserved. Returns True on success.
    ''' </summary>
    Public Function DeleteResource(resourceID As Integer) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblResources SET IsActive = 0 WHERE ResourceID = @ResourceID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in DeleteResource: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>Increments ViewCount by 1. Failures are logged silently - non-critical.</summary>
    Public Sub IncrementViewCount(resourceID As Integer)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblResources SET ViewCount = ViewCount + 1 WHERE ResourceID = @ResourceID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ResourceID", resourceID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in IncrementViewCount", ex)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' V2 ENGAGEMENT METHODS
    ' ─────────────────────────────────────────────────────────────

    Public Sub IncrementDownloadCount(resourceID As Integer)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblResources SET DownloadCount = DownloadCount + 1 WHERE ResourceID = @ResourceID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ResourceID", resourceID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in IncrementDownloadCount", ex)
        End Try
    End Sub

    Public Function GetRecentlyAdded(count As Integer) As List(Of Resource)
        Dim resources As New List(Of Resource)
        Dim topN As Integer = Math.Max(1, Math.Min(count, 50))
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT TOP (@TopN) r.ResourceID, r.Title, r.Description, r.CategoryID, c.CategoryName,
                                            r.SubjectArea, r.ResourceType, r.URL, r.FilePath, r.EducationLevel,
                                            r.Tags, r.AddedBy, u.FullName AS AddedByName, r.DateAdded, r.IsActive, r.ViewCount,
                                            r.DownloadCount, r.ThumbnailPath, r.CurrentVersion
                                     FROM tblResources r
                                     INNER JOIN tblCategories c ON r.CategoryID = c.CategoryID
                                     INNER JOIN tblUsers u ON r.AddedBy = u.UserID
                                     WHERE r.IsActive = 1
                                     ORDER BY r.DateAdded DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = topN
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            resources.Add(MapReaderToResource(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in GetRecentlyAdded", ex)
        End Try
        Return resources
    End Function

    Public Function SearchTitlesForAutocomplete(prefix As String) As List(Of String)
        Dim titles As New List(Of String)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT TOP 10 Title FROM tblResources WHERE Title LIKE @Prefix AND IsActive = 1"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Prefix", prefix & "%")
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            titles.Add(reader.GetString(0))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in SearchTitlesForAutocomplete", ex)
        End Try
        Return titles
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PRIVATE HELPERS
    ' ─────────────────────────────────────────────────────────────

    Private Function MapReaderToResource(reader As SqlDataReader) As Resource
        Dim r As New Resource()
        r.ResourceID     = reader.GetInt32(reader.GetOrdinal("ResourceID"))
        r.Title          = reader.GetString(reader.GetOrdinal("Title"))
        r.Description    = If(reader.IsDBNull(reader.GetOrdinal("Description")), String.Empty, reader.GetString(reader.GetOrdinal("Description")))
        r.CategoryID     = reader.GetInt32(reader.GetOrdinal("CategoryID"))
        r.CategoryName   = reader.GetString(reader.GetOrdinal("CategoryName"))
        r.SubjectArea    = If(reader.IsDBNull(reader.GetOrdinal("SubjectArea")), String.Empty, reader.GetString(reader.GetOrdinal("SubjectArea")))
        r.ResourceType   = reader.GetString(reader.GetOrdinal("ResourceType"))
        r.URL            = If(reader.IsDBNull(reader.GetOrdinal("URL")), String.Empty, reader.GetString(reader.GetOrdinal("URL")))
        r.FilePath       = If(reader.IsDBNull(reader.GetOrdinal("FilePath")), String.Empty, reader.GetString(reader.GetOrdinal("FilePath")))
        r.EducationLevel = If(reader.IsDBNull(reader.GetOrdinal("EducationLevel")), String.Empty, reader.GetString(reader.GetOrdinal("EducationLevel")))
        r.Tags           = If(reader.IsDBNull(reader.GetOrdinal("Tags")), String.Empty, reader.GetString(reader.GetOrdinal("Tags")))
        r.AddedBy        = reader.GetInt32(reader.GetOrdinal("AddedBy"))
        r.AddedByName    = reader.GetString(reader.GetOrdinal("AddedByName"))
        r.DateAdded      = reader.GetDateTime(reader.GetOrdinal("DateAdded"))
        r.IsActive       = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        r.ViewCount      = reader.GetInt32(reader.GetOrdinal("ViewCount"))
        r.DownloadCount  = reader.GetInt32(reader.GetOrdinal("DownloadCount"))
        r.ThumbnailPath  = If(reader.IsDBNull(reader.GetOrdinal("ThumbnailPath")), String.Empty, reader.GetString(reader.GetOrdinal("ThumbnailPath")))
        r.CurrentVersion = reader.GetInt32(reader.GetOrdinal("CurrentVersion"))
        Return r
    End Function

    Private Sub BuildResourceParams(cmd As SqlCommand, res As Resource)
        cmd.Parameters.Add("@Title",          SqlDbType.NVarChar, 200).Value  = res.Title
        cmd.Parameters.Add("@Description",    SqlDbType.NVarChar, 1000).Value = If(String.IsNullOrWhiteSpace(res.Description), DBNull.Value, CObj(res.Description))
        cmd.Parameters.Add("@CategoryID",     SqlDbType.Int).Value             = res.CategoryID
        cmd.Parameters.Add("@SubjectArea",    SqlDbType.NVarChar, 100).Value  = If(String.IsNullOrWhiteSpace(res.SubjectArea), DBNull.Value, CObj(res.SubjectArea))
        cmd.Parameters.Add("@ResourceType",   SqlDbType.NVarChar, 50).Value   = res.ResourceType
        cmd.Parameters.Add("@URL",            SqlDbType.NVarChar, 500).Value  = If(String.IsNullOrWhiteSpace(res.URL), DBNull.Value, CObj(res.URL))
        cmd.Parameters.Add("@FilePath",       SqlDbType.NVarChar, 500).Value  = If(String.IsNullOrWhiteSpace(res.FilePath), DBNull.Value, CObj(res.FilePath))
        cmd.Parameters.Add("@EducationLevel", SqlDbType.NVarChar, 50).Value   = If(String.IsNullOrWhiteSpace(res.EducationLevel), DBNull.Value, CObj(res.EducationLevel))
        cmd.Parameters.Add("@Tags",           SqlDbType.NVarChar, 500).Value  = If(String.IsNullOrWhiteSpace(res.Tags), DBNull.Value, CObj(res.Tags))
        cmd.Parameters.Add("@AddedBy",        SqlDbType.Int).Value             = res.AddedBy
    End Sub

End Class
