Imports System.Text.RegularExpressions

''' <summary>
''' Business Logic Layer - Resource Service.
''' Enforces business rules before delegating to the DAL.
''' Handles engagement stats tagging and resource access tracking.
''' </summary>
Public Class ResourceService

    Private ReadOnly _resourceRepo As New ResourceRepository()
    Private ReadOnly _logRepo      As New AccessLogRepository()
    Private ReadOnly _v2Repo       As New V2FeatureRepository()
    Private ReadOnly _userRepo     As New UserRepository()
    Private ReadOnly _logger       As ILogger
    Private ReadOnly _storagePath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EduVault_Storage")

    Public Sub New()
        Me.New(New LogRepository())
    End Sub

    Public Sub New(logger As ILogger)
        _logger = logger
        ' Ensure storage directory exists
        If Not System.IO.Directory.Exists(_storagePath) Then
            System.IO.Directory.CreateDirectory(_storagePath)
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' QUERIES
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Returns all active resources. No business rule filtering needed.</summary>
    Public Function GetAllResources() As List(Of Resource)
        Return _resourceRepo.GetAllResources()
    End Function

    ''' <summary>Returns a resource by ID, or Nothing if not found.</summary>
    Public Function GetResourceByID(resourceID As Integer) As Resource
        If resourceID <= 0 Then Return Nothing
        Return _resourceRepo.GetResourceByID(resourceID)
    End Function

    ''' <summary>
    ''' Returns filtered resources based on search parameters.
    ''' Business rule: keyword must be at least 2 characters if provided.
    ''' </summary>
    Public Function SearchResources(keyword As String, categoryID As Integer,
                                    resourceType As String, educationLevel As String,
                                    ByRef errorMessage As String) As List(Of Resource)
        errorMessage = String.Empty

        If Not String.IsNullOrWhiteSpace(keyword) AndAlso keyword.Trim().Length < 2 Then
            errorMessage = "Search keyword must be at least 2 characters."
            Return New List(Of Resource)()
        End If

        Return _resourceRepo.SearchResources(keyword?.Trim(), categoryID, resourceType, educationLevel)
    End Function

    ''' <summary>Returns the top 5 most-viewed resources for the Dashboard trending panel.</summary>
    Public Function GetTrendingResources() As List(Of Resource)
        Return _resourceRepo.GetTopResources(5)
    End Function

    Public Function GetRecentlyAdded(count As Integer) As List(Of Resource)
        Return _resourceRepo.GetRecentlyAdded(count)
    End Function

    Public Function SearchTitlesForAutocomplete(prefix As String) As List(Of String)
        If String.IsNullOrWhiteSpace(prefix) OrElse prefix.Trim().Length < 2 Then
            Return New List(Of String)()
        End If
        Return _resourceRepo.SearchTitlesForAutocomplete(prefix.Trim())
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' ADD / EDIT / DELETE
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Validates and adds a new resource.
    ''' Business rules: Title required, URL or FilePath required, CategoryID must be valid.
    ''' Returns the new ResourceID, or -1 on failure.
    ''' </summary>
    Public Function AddResource(res As Resource, ByRef errorMessage As String) As Integer
        errorMessage = String.Empty

        Dim validationError As String = ValidateResource(res)
        If validationError IsNot Nothing Then
            errorMessage = validationError
            Return -1
        End If
        ' Log addition attempt
        _logger.LogInfo($"Adding resource: {res.Title}")

        ' Ensure AddedBy is set from the current session
        If res.AddedBy <= 0 Then res.AddedBy = Session.CurrentUser.UserID

        Try
            ' --- File Storage Logic ---
            If Not String.IsNullOrWhiteSpace(res.FilePath) AndAlso System.IO.File.Exists(res.FilePath) Then
                res.FilePath = CopyToStorage(res.FilePath)
            End If

            Return _resourceRepo.AddResource(res)
        Catch ex As Exception
            errorMessage = $"Error adding resource: {ex.Message}"
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Validates and updates an existing resource.
    ''' Returns True on success; sets errorMessage on failure.
    ''' </summary>
    Public Function UpdateResource(res As Resource, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty

        If res.ResourceID <= 0 Then
            errorMessage = "Invalid resource ID."
            Return False
        End If

        Dim validationError As String = ValidateResource(res)
        If validationError IsNot Nothing Then
            errorMessage = validationError
            Return False
        End If

        Try
            ' --- File Storage Logic ---
            If Not String.IsNullOrWhiteSpace(res.FilePath) AndAlso System.IO.File.Exists(res.FilePath) Then
                res.FilePath = CopyToStorage(res.FilePath)
            End If

            Dim existing As Resource = _resourceRepo.GetResourceByID(res.ResourceID)
            Dim result As Boolean = _resourceRepo.UpdateResource(res)
            If result AndAlso existing IsNot Nothing Then
                Dim nextVer As Integer = Math.Max(1, existing.CurrentVersion) + 1
                Dim summary As String = $"Updated: {existing.Title}"
                Dim snapshot As String =
                    $"{existing.Title}|{existing.CategoryID}|{existing.ResourceType}|{existing.URL}|{existing.FilePath}"
                Dim changedBy As Integer = If(Session.IsLoggedIn, Session.CurrentUser.UserID, res.AddedBy)
                _v2Repo.SaveResourceVersion(res.ResourceID, nextVer, changedBy, summary, snapshot)
                _logger.LogInfo($"Resource updated: {res.ResourceID} (v{nextVer})")
            End If
            Return result
        Catch ex As Exception
            errorMessage = $"Error updating resource: {ex.Message}"
            _logger.LogError($"Error updating resource {res.ResourceID}: {ex.Message}")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Soft-deletes a resource. Admin-only operation (enforced at form level).
    ''' Returns True on success; sets errorMessage on failure.
    ''' </summary>
    Public Function DeleteResource(resourceID As Integer, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty

        If resourceID <= 0 Then
            errorMessage = "Invalid resource ID."
            Return False
        End If

        Try
            Return _resourceRepo.DeleteResource(resourceID)
        Catch ex As Exception
            errorMessage = $"Error deleting resource: {ex.Message}"
            Return False
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' ENGAGEMENT / ACCESS TRACKING
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Records a resource access event and increments ViewCount.
    ''' Call this whenever a student opens or views a resource link.
    ''' </summary>
    Public Sub RecordAccess(userID As Integer, resourceID As Integer, accessType As String)
        ' Guest / invalid users: still count views for analytics, but skip per-user log rows
        If userID > 0 Then
            _logRepo.LogAccess(userID, resourceID, accessType)
        End If

        If accessType = "View" Then
            _resourceRepo.IncrementViewCount(resourceID)
        ElseIf accessType = "Download" Then
            _resourceRepo.IncrementDownloadCount(resourceID)
        End If
    End Sub

    ''' <summary>Opens a resource link and records the correct access type (View vs Download).</summary>
    Public Sub OpenResource(res As Resource, userID As Integer)
        If res Is Nothing Then Return
        Dim accessType As String = If(Not String.IsNullOrWhiteSpace(res.URL), "View", "Download")
        RecordAccess(userID, res.ResourceID, accessType)
        Dim link As String = res.AccessLink
        If String.IsNullOrWhiteSpace(link) Then Return
        System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(link) With {.UseShellExecute = True})
    End Sub

    Public Function RateResource(userID As Integer, resourceID As Integer, stars As Integer,
                                reviewText As String, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If userID <= 0 Then errorMessage = "Sign in to rate resources." : Return False
        If stars < 1 OrElse stars > 5 Then errorMessage = "Rating must be between 1 and 5 stars." : Return False
        Try
            Return _v2Repo.UpsertRating(userID, resourceID, stars, reviewText)
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SubmitResourceRequest(userID As Integer, title As String, description As String,
                                          categoryID As Integer?, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If userID <= 0 Then errorMessage = "Sign in to submit a request." : Return False
        If String.IsNullOrWhiteSpace(title) Then errorMessage = "Title is required." : Return False
        Try
            Dim requestID As Integer = _v2Repo.AddResourceRequest(userID, title.Trim(), description, categoryID)
            If requestID > 0 Then
                For Each admin In _userRepo.GetAllUsers().Where(Function(u) u.IsActive AndAlso u.Role = "Admin")
                    _v2Repo.AddNotification(admin.UserID, $"New resource request: {title.Trim()}")
                Next
                Return True
            End If
            errorMessage = "Could not save request."
            Return False
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function AddToFavouritesList(userID As Integer, resourceID As Integer, listName As String,
                                        ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If userID <= 0 Then errorMessage = "Sign in to use favourites lists." : Return False
        If String.IsNullOrWhiteSpace(listName) Then listName = "My Favourites"
        Return _v2Repo.AddToFavouritesList(userID, resourceID, listName.Trim())
    End Function

    ''' <summary>
    ''' Adds a bookmark for the current user. Logs a "Bookmark" access event.
    ''' Returns True on success; sets errorMessage on failure.
    ''' </summary>
    Public Function BookmarkResource(userID As Integer, resourceID As Integer,
                                     ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If userID <= 0 Then
            errorMessage = "Sign in with your account to save bookmarks. Guest browsing is read-only."
            Return False
        End If
        Try
            Dim added As Boolean = _logRepo.AddBookmark(userID, resourceID)
            If added Then
                _logger.LogInfo($"Bookmark added for user {userID} resource {resourceID}")
                _logRepo.LogAccess(userID, resourceID, "Bookmark")
            Else
                errorMessage = "This resource is already in your bookmarks."
            End If
            Return added
        Catch ex As Exception
            errorMessage = $"Bookmark error: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>Removes a bookmark for the given user/resource pair.</summary>
    Public Function RemoveBookmark(userID As Integer, resourceID As Integer,
                                   ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        Try
            Return _logRepo.RemoveBookmark(userID, resourceID)
        Catch ex As Exception
            errorMessage = $"Remove bookmark error: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>Returns True if the given user has bookmarked the resource.</summary>
    Public Function IsBookmarked(userID As Integer, resourceID As Integer) As Boolean
        Return _logRepo.IsBookmarked(userID, resourceID)
    End Function

    ''' <summary>Returns the user's bookmark list for the My Bookmarks panel.</summary>
    Public Function GetUserBookmarks(userID As Integer) As List(Of Bookmark)
        If userID <= 0 Then Return New List(Of Bookmark)()
        Return _logRepo.GetBookmarksByUser(userID)
    End Function

    ''' <summary>
    ''' Returns the user's bookmarks as a DataTable for CSV export.
    ''' </summary>
    Public Function GetBookmarksDataTable(userID As Integer) As DataTable
        Dim bookmarks As List(Of Bookmark) = GetUserBookmarks(userID)
        Dim dt As New DataTable("MyBookmarks")
        dt.Columns.Add("ResourceID", GetType(Integer))
        dt.Columns.Add("Title", GetType(String))
        dt.Columns.Add("Category", GetType(String))
        dt.Columns.Add("Type", GetType(String))
        dt.Columns.Add("DateBookmarked", GetType(DateTime))

        For Each b In bookmarks
            dt.Rows.Add(b.ResourceID, b.ResourceTitle, b.CategoryName, b.ResourceType, b.DateBookmarked)
        Next
        Return dt
    End Function

    ''' <summary>Returns the user's recent access history (last 20 events).</summary>
    Public Function GetRecentActivity(userID As Integer) As List(Of AccessLog)
        Return _logRepo.GetUserAccessHistory(userID, 20)
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' ENGAGEMENT STATS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Business Rule: Returns engagement tag for a resource.
    ''' "HOT"     - ViewCount > 100
    ''' "POPULAR" - ViewCount > 50
    ''' "NEW"     - Added within the last 7 days
    ''' ""        - Standard (no tag)
    ''' </summary>
    Public Function GetEngagementTag(res As Resource) As String
        If res.ViewCount > 100 Then Return "HOT"
        If res.ViewCount > 50  Then Return "POPULAR"
        If (DateTime.Now - res.DateAdded).TotalDays <= 7 Then Return "NEW"
        Return String.Empty
    End Function

    ''' <summary>
    ''' Returns a list of tag strings for each resource in a list.
    ''' Used to populate a tag badge column in the DataGridView.
    ''' </summary>
    Public Function GetEngagementTags(resources As List(Of Resource)) As Dictionary(Of Integer, String)
        Dim tags As New Dictionary(Of Integer, String)()
        For Each res As Resource In resources
            tags(res.ResourceID) = GetEngagementTag(res)
        Next
        Return tags
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PRIVATE VALIDATION
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Validates required fields and business rules for a resource.
    ''' Returns an error message string, or Nothing if the resource is valid.
    ''' </summary>
    Private Function ValidateResource(res As Resource) As String
        If res Is Nothing Then Return "Resource data is missing."
        If String.IsNullOrWhiteSpace(res.Title) Then Return "Resource title is required."
        If res.Title.Length > 200 Then Return "Title must not exceed 200 characters."
        If res.CategoryID <= 0 Then Return "Please select a category."

        Dim validTypes As String() = {"E-Book", "Video", "Module", "Reference", "Article"}
        If Not validTypes.Any(Function(t) String.Equals(t, res.ResourceType, StringComparison.OrdinalIgnoreCase)) Then
            Return "Please select a valid resource type."
        End If

        ' At least one access method must be provided
        If String.IsNullOrWhiteSpace(res.URL) AndAlso String.IsNullOrWhiteSpace(res.FilePath) Then
            Return "Please provide either a URL or a file path for this resource."
        End If

        ' Validate URL format if provided
        If Not String.IsNullOrWhiteSpace(res.URL) Then
            Dim uriResult As Uri = Nothing
            If Not Uri.TryCreate(res.URL.Trim(), UriKind.Absolute, uriResult) OrElse
               (uriResult.Scheme <> Uri.UriSchemeHttp AndAlso uriResult.Scheme <> Uri.UriSchemeHttps) Then
                Return "URL must be a valid http:// or https:// address."
            End If
        End If

        If Not String.IsNullOrWhiteSpace(res.Tags) Then
            If res.Tags.Length > 500 Then Return "Tags must not exceed 500 characters."
            ' Ensure tags contain only alphanumeric characters and commas
            Dim tagPattern As String = "^[a-zA-Z0-9,\s]*$"
            If Not System.Text.RegularExpressions.Regex.IsMatch(res.Tags, tagPattern) Then
                Return "Tags may contain only letters, numbers, commas, and spaces."
            End If
        End If

        Return Nothing  ' All validations passed
    End Function

    Private Function CopyToStorage(sourcePath As String) As String
        Try
            Dim fileName As String = System.IO.Path.GetFileName(sourcePath)
            ' Add timestamp to prevent name collisions
            Dim uniqueName As String = $"{DateTime.Now:yyyyMMddHHmmss}_{fileName}"
            Dim destPath As String = System.IO.Path.Combine(_storagePath, uniqueName)
            
            System.IO.File.Copy(sourcePath, destPath, True)
            _logger.LogInfo($"File copied to storage: {uniqueName}")
            Return destPath
        Catch ex As Exception
            _logger.LogError($"Failed to copy file to storage: {ex.Message}")
            Return sourcePath ' Fallback to original path if copy fails
        End Try
    End Function

End Class
