Imports System.Data.SqlClient

''' <summary>Data access for v2 migration tables (notifications, backup schedule, versions, ratings, requests, favourites).</summary>
Public Class V2FeatureRepository

    ' ── Backup schedule ─────────────────────────────────────────

    Public Function GetBackupSchedule() As BackupScheduleInfo
        Dim info As New BackupScheduleInfo()
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT TOP 1 ScheduleID, FrequencyDays, BackupPath, LastBackupDate, NextBackupDate, IsEnabled
                     FROM tblBackupSchedule ORDER BY ScheduleID"
                Using cmd As New SqlCommand(sql, conn)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            info.ScheduleID = reader.GetInt32(0)
                            info.FrequencyDays = reader.GetInt32(1)
                            info.BackupPath = reader.GetString(2)
                            info.LastBackupDate = If(reader.IsDBNull(3), Nothing, CType(reader.GetDateTime(3), DateTime?))
                            info.NextBackupDate = If(reader.IsDBNull(4), Nothing, CType(reader.GetDateTime(4), DateTime?))
                            info.IsEnabled = reader.GetBoolean(5)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetBackupSchedule: {ex.Message}", ex)
        End Try
        Return info
    End Function

    Public Function UpdateBackupSchedule(scheduleID As Integer, frequencyDays As Integer,
                                         backupPath As String, isEnabled As Boolean) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblBackupSchedule SET FrequencyDays = @Days, BackupPath = @Path, IsEnabled = @Enabled
                     WHERE ScheduleID = @ID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Days", SqlDbType.Int).Value = frequencyDays
                    cmd.Parameters.Add("@Path", SqlDbType.NVarChar, 500).Value = backupPath
                    cmd.Parameters.Add("@Enabled", SqlDbType.Bit).Value = isEnabled
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scheduleID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UpdateBackupSchedule: {ex.Message}", ex)
        End Try
    End Function

    Public Sub RecordBackupRun(scheduleID As Integer)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblBackupSchedule
                     SET LastBackupDate = GETDATE(),
                         NextBackupDate = DATEADD(DAY, FrequencyDays, GETDATE())
                     WHERE ScheduleID = @ID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = scheduleID
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in RecordBackupRun: {ex.Message}", ex)
        End Try
    End Sub

    ' ── Notifications ───────────────────────────────────────────

    Public Function GetNotificationsForUser(userID As Integer, Optional unreadOnly As Boolean = False) As List(Of AppNotification)
        Dim list As New List(Of AppNotification)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT NotificationID, UserID, Message, IsRead, DateCreated, RelatedResourceID
                     FROM tblNotifications WHERE UserID = @UserID"
                If unreadOnly Then sql &= " AND IsRead = 0"
                sql &= " ORDER BY DateCreated DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New AppNotification With {
                                .NotificationID = reader.GetInt32(0),
                                .UserID = reader.GetInt32(1),
                                .Message = reader.GetString(2),
                                .IsRead = reader.GetBoolean(3),
                                .DateCreated = reader.GetDateTime(4),
                                .RelatedResourceID = If(reader.IsDBNull(5), Nothing, CType(reader.GetInt32(5), Integer?))
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetNotificationsForUser: {ex.Message}", ex)
        End Try
        Return list
    End Function

    Public Function GetUnreadNotificationCount(userID As Integer) As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT COUNT(1) FROM tblNotifications WHERE UserID = @UserID AND IsRead = 0"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Return 0
        End Try
    End Function

    Public Sub MarkNotificationRead(notificationID As Integer)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblNotifications SET IsRead = 1 WHERE NotificationID = @ID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = notificationID
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in MarkNotificationRead: {ex.Message}", ex)
        End Try
    End Sub

    Public Sub AddNotification(userID As Integer, message As String, Optional relatedResourceID As Integer? = Nothing)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblNotifications (UserID, Message, RelatedResourceID) VALUES (@UserID, @Msg, @ResID)"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 500).Value = message
                    cmd.Parameters.Add("@ResID", SqlDbType.Int).Value =
                        If(relatedResourceID.HasValue, CObj(relatedResourceID.Value), DBNull.Value)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            ' Non-critical
        End Try
    End Sub

    ' ── Resource versions ───────────────────────────────────────

    Public Sub SaveResourceVersion(resourceID As Integer, versionNumber As Integer,
                                   changedBy As Integer, changeSummary As String, snapshotJson As String)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblResourceVersions (ResourceID, VersionNumber, ChangeSummary, ChangedBy, SnapshotJSON)
                     VALUES (@ResID, @Ver, @Summary, @By, @Json);
                     UPDATE tblResources SET CurrentVersion = @Ver WHERE ResourceID = @ResID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ResID", SqlDbType.Int).Value = resourceID
                    cmd.Parameters.Add("@Ver", SqlDbType.Int).Value = versionNumber
                    cmd.Parameters.Add("@Summary", SqlDbType.NVarChar, 500).Value =
                        If(String.IsNullOrWhiteSpace(changeSummary), DBNull.Value, CObj(changeSummary.Trim()))
                    cmd.Parameters.Add("@By", SqlDbType.Int).Value = changedBy
                    cmd.Parameters.Add("@Json", SqlDbType.NVarChar).Value =
                        If(String.IsNullOrWhiteSpace(snapshotJson), DBNull.Value, CObj(snapshotJson))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in SaveResourceVersion: {ex.Message}", ex)
        End Try
    End Sub

    Public Function GetVersionHistory(resourceID As Integer) As List(Of ResourceVersionInfo)
        Dim list As New List(Of ResourceVersionInfo)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT VersionID, ResourceID, VersionNumber, ChangeSummary, ChangedBy, ChangeDate
                     FROM tblResourceVersions WHERE ResourceID = @ResID ORDER BY VersionNumber DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ResID", SqlDbType.Int).Value = resourceID
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New ResourceVersionInfo With {
                                .VersionID = reader.GetInt32(0),
                                .ResourceID = reader.GetInt32(1),
                                .VersionNumber = reader.GetInt32(2),
                                .ChangeSummary = If(reader.IsDBNull(3), String.Empty, reader.GetString(3)),
                                .ChangedBy = reader.GetInt32(4),
                                .ChangeDate = reader.GetDateTime(5)
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetVersionHistory: {ex.Message}", ex)
        End Try
        Return list
    End Function

    ' ── Ratings ─────────────────────────────────────────────────

    Public Function UpsertRating(userID As Integer, resourceID As Integer, stars As Integer, reviewText As String) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "IF EXISTS (SELECT 1 FROM tblRatings WHERE UserID = @UserID AND ResourceID = @ResID)
                        UPDATE tblRatings SET Stars = @Stars, ReviewText = @Review, DateRated = GETDATE()
                        WHERE UserID = @UserID AND ResourceID = @ResID
                     ELSE
                        INSERT INTO tblRatings (UserID, ResourceID, Stars, ReviewText) VALUES (@UserID, @ResID, @Stars, @Review)"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@ResID", SqlDbType.Int).Value = resourceID
                    cmd.Parameters.Add("@Stars", SqlDbType.TinyInt).Value = CByte(stars)
                    cmd.Parameters.Add("@Review", SqlDbType.NVarChar, 1000).Value =
                        If(String.IsNullOrWhiteSpace(reviewText), DBNull.Value, CObj(reviewText.Trim()))
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UpsertRating: {ex.Message}", ex)
        End Try
    End Function

    Public Function GetAverageRating(resourceID As Integer) As Double
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT AVG(CAST(Stars AS FLOAT)) FROM tblRatings WHERE ResourceID = @ResID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ResID", SqlDbType.Int).Value = resourceID
                    Dim o = cmd.ExecuteScalar()
                    If o Is Nothing OrElse o Is DBNull.Value Then Return 0
                    Return CDbl(o)
                End Using
            End Using
        Catch ex As SqlException
            Return 0
        End Try
    End Function

    ' ── Resource requests ───────────────────────────────────────

    Public Function AddResourceRequest(userID As Integer, title As String, description As String,
                                       categoryID As Integer?) As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblResourceRequests (UserID, Title, Description, CategoryID)
                     VALUES (@UserID, @Title, @Desc, @CatID);
                     SELECT CAST(SCOPE_IDENTITY() AS INT);"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = title
                    cmd.Parameters.Add("@Desc", SqlDbType.NVarChar, 1000).Value =
                        If(String.IsNullOrWhiteSpace(description), DBNull.Value, CObj(description))
                    cmd.Parameters.Add("@CatID", SqlDbType.Int).Value =
                        If(categoryID.HasValue AndAlso categoryID.Value > 0, CObj(categoryID.Value), DBNull.Value)
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in AddResourceRequest: {ex.Message}", ex)
        End Try
    End Function

    Public Function UpdateResourceRequestStatus(requestID As Integer, status As String, adminNotes As String) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblResourceRequests SET Status = @Status, AdminNotes = @Notes WHERE RequestID = @ID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = status
                    cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value =
                        If(String.IsNullOrWhiteSpace(adminNotes), DBNull.Value, CObj(adminNotes))
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = requestID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Return False
        End Try
    End Function

    Public Function GetPendingResourceRequests() As List(Of ResourceRequestInfo)
        Dim list As New List(Of ResourceRequestInfo)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT r.RequestID, r.UserID, u.FullName, r.Title, r.Description, r.Status, r.DateRequested
                     FROM tblResourceRequests r
                     INNER JOIN tblUsers u ON r.UserID = u.UserID
                     WHERE r.Status = 'Pending' ORDER BY r.DateRequested DESC"
                Using cmd As New SqlCommand(sql, conn)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New ResourceRequestInfo With {
                                .RequestID = reader.GetInt32(0),
                                .UserID = reader.GetInt32(1),
                                .RequesterName = reader.GetString(2),
                                .Title = reader.GetString(3),
                                .Description = If(reader.IsDBNull(4), String.Empty, reader.GetString(4)),
                                .Status = reader.GetString(5),
                                .DateRequested = reader.GetDateTime(6)
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetPendingResourceRequests: {ex.Message}", ex)
        End Try
        Return list
    End Function

    ' ── Favourites (curated lists) ──────────────────────────────

    Public Function AddToFavouritesList(userID As Integer, resourceID As Integer, listName As String) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "IF NOT EXISTS (SELECT 1 FROM tblFavourites WHERE UserID=@U AND ResourceID=@R AND ListName=@L)
                     INSERT INTO tblFavourites (UserID, ResourceID, ListName) VALUES (@U, @R, @L)"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@U", SqlDbType.Int).Value = userID
                    cmd.Parameters.Add("@R", SqlDbType.Int).Value = resourceID
                    cmd.Parameters.Add("@L", SqlDbType.NVarChar, 100).Value = listName
                    cmd.ExecuteNonQuery()
                    Return True
                End Using
            End Using
        Catch ex As SqlException
            Return False
        End Try
    End Function

End Class

Public Class BackupScheduleInfo
    Public Property ScheduleID As Integer
    Public Property FrequencyDays As Integer = 7
    Public Property BackupPath As String = "C:\EduVaultBackups\"
    Public Property LastBackupDate As DateTime?
    Public Property NextBackupDate As DateTime?
    Public Property IsEnabled As Boolean = True
End Class

Public Class AppNotification
    Public Property NotificationID As Integer
    Public Property UserID As Integer
    Public Property Message As String
    Public Property IsRead As Boolean
    Public Property DateCreated As DateTime
    Public Property RelatedResourceID As Integer?
End Class

Public Class ResourceVersionInfo
    Public Property VersionID As Integer
    Public Property ResourceID As Integer
    Public Property VersionNumber As Integer
    Public Property ChangeSummary As String
    Public Property ChangedBy As Integer
    Public Property ChangeDate As DateTime
End Class

Public Class ResourceRequestInfo
    Public Property RequestID As Integer
    Public Property UserID As Integer
    Public Property RequesterName As String
    Public Property Title As String
    Public Property Description As String
    Public Property Status As String
    Public Property DateRequested As DateTime
End Class
