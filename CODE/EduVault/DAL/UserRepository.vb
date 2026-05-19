Imports System.Data.SqlClient

''' <summary>
''' Data Access Layer for tblUsers.
''' All queries use parameterized commands - no raw SQL string concatenation.
''' </summary>
Public Class UserRepository

    ' ─────────────────────────────────────────────────────────────
    ' READ OPERATIONS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Retrieves all users from the database, ordered by FullName.
    ''' </summary>
    Public Function GetAllUsers() As List(Of User)
        Dim users As New List(Of User)

        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT UserID, Username, PasswordHash, FullName, Email, Role, IsActive, DateCreated,
                            FailedLoginCount, LockoutEndTime, LastLoginDate, PasswordResetToken,
                            ResetTokenExpiry, EmailVerified, FavouriteCategory, DarkMode
                     FROM tblUsers
                     ORDER BY FullName ASC"

                Using cmd As New SqlCommand(sql, conn)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            users.Add(MapReaderToUser(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetAllUsers: {ex.Message}", ex)
        End Try

        Return users
    End Function

    ''' <summary>
    ''' Retrieves a single user by their UserID.
    ''' Returns Nothing if not found.
    ''' </summary>
    Public Function GetUserByID(userID As Integer) As User
        Dim user As User = Nothing

        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT UserID, Username, PasswordHash, FullName, Email, Role, IsActive, DateCreated,
                            FailedLoginCount, LockoutEndTime, LastLoginDate, PasswordResetToken,
                            ResetTokenExpiry, EmailVerified, FavouriteCategory, DarkMode
                     FROM tblUsers
                     WHERE UserID = @UserID"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@UserID", userID)

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            user = MapReaderToUser(reader)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetUserByID: {ex.Message}", ex)
        End Try

        Return user
    End Function

    ''' <summary>
    ''' Retrieves a user by their unique username.
    ''' Used during the login process.
    ''' Returns Nothing if no match is found.
    ''' </summary>
    Public Function GetUserByUsername(username As String) As User
        Dim user As User = Nothing

        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT UserID, Username, PasswordHash, FullName, Email, Role, IsActive, DateCreated,
                            FailedLoginCount, LockoutEndTime, LastLoginDate, PasswordResetToken,
                            ResetTokenExpiry, EmailVerified, FavouriteCategory, DarkMode
                     FROM tblUsers
                     WHERE Username = @Username"

                Using cmd As New SqlCommand(sql, conn)
                    ' Use NVarChar to match the column type exactly
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            user = MapReaderToUser(reader)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetUserByUsername: {ex.Message}", ex)
        End Try

        Return user
    End Function

    ''' <summary>
    ''' Returns True if the given username already exists in the database.
    ''' Used during new user registration to enforce uniqueness.
    ''' </summary>
    Public Function UsernameExists(username As String) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT COUNT(1) FROM tblUsers WHERE Username = @Username"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username
                    Return CInt(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UsernameExists: {ex.Message}", ex)
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' WRITE OPERATIONS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Inserts a new user record into tblUsers.
    ''' Returns the new UserID assigned by SQL Server IDENTITY.
    ''' </summary>
    Public Function AddUser(user As User) As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblUsers (Username, PasswordHash, FullName, Email, Role, IsActive)
                     VALUES (@Username, @PasswordHash, @FullName, @Email, @Role, @IsActive);
                     SELECT CAST(SCOPE_IDENTITY() AS INT);"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Username",     SqlDbType.NVarChar, 50).Value  = user.Username
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = user.PasswordHash
                    cmd.Parameters.Add("@FullName",     SqlDbType.NVarChar, 100).Value = user.FullName
                    cmd.Parameters.Add("@Email",        SqlDbType.NVarChar, 100).Value =
                        If(String.IsNullOrWhiteSpace(user.Email), DBNull.Value, CObj(user.Email))
                    cmd.Parameters.Add("@Role",         SqlDbType.NVarChar, 20).Value  = user.Role
                    cmd.Parameters.Add("@IsActive",     SqlDbType.Bit).Value            = user.IsActive

                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in AddUser: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>
    ''' Updates the editable fields of an existing user (not password).
    ''' Returns True if the row was successfully updated.
    ''' </summary>
    Public Function UpdateUser(user As User) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblUsers
                     SET FullName = @FullName,
                         Email    = @Email,
                         Role     = @Role,
                         IsActive = @IsActive
                     WHERE UserID = @UserID"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = user.FullName
                    cmd.Parameters.Add("@Email",    SqlDbType.NVarChar, 100).Value =
                        If(String.IsNullOrWhiteSpace(user.Email), DBNull.Value, CObj(user.Email))
                    cmd.Parameters.Add("@Role",     SqlDbType.NVarChar, 20).Value  = user.Role
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value            = user.IsActive
                    cmd.Parameters.Add("@UserID",   SqlDbType.Int).Value            = user.UserID

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UpdateUser: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>
    ''' Updates only the password hash for a user.
    ''' Called from the Change Password feature.
    ''' </summary>
    Public Function UpdatePassword(userID As Integer, newPasswordHash As String) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblUsers SET PasswordHash = @PasswordHash WHERE UserID = @UserID"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = newPasswordHash
                    cmd.Parameters.Add("@UserID",       SqlDbType.Int).Value            = userID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UpdatePassword: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>
    ''' Soft-deletes a user by setting IsActive = 0.
    ''' Records are never physically deleted to preserve referential integrity
    ''' with tblAccessLog and tblBookmarks.
    ''' </summary>
    Public Function DeactivateUser(userID As Integer) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET IsActive = 0 WHERE UserID = @UserID"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in DeactivateUser: {ex.Message}", ex)
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' V2 SECURITY METHODS
    ' ─────────────────────────────────────────────────────────────

    Public Function UpdateLoginAttempts(username As String) As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET FailedLoginCount = FailedLoginCount + 1 
                                     OUTPUT INSERTED.FailedLoginCount
                                     WHERE Username = @Username"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Username", username)
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in UpdateLoginAttempts", ex)
        End Try
    End Function

    Public Sub ResetLoginAttempts(userID As Integer)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET FailedLoginCount = 0 WHERE UserID = @UserID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in ResetLoginAttempts", ex)
        End Try
    End Sub

    Public Sub SetLockout(userID As Integer, lockoutEnd As DateTime?)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET LockoutEndTime = @LockoutEnd WHERE UserID = @UserID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@LockoutEnd", If(lockoutEnd.HasValue, CObj(lockoutEnd.Value), DBNull.Value))
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in SetLockout", ex)
        End Try
    End Sub

    Public Sub SetResetToken(userID As Integer, token As String, expiry As DateTime?)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET PasswordResetToken = @Token, ResetTokenExpiry = @Expiry WHERE UserID = @UserID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Token", If(String.IsNullOrEmpty(token), DBNull.Value, CObj(token)))
                    cmd.Parameters.AddWithValue("@Expiry", If(expiry.HasValue, CObj(expiry.Value), DBNull.Value))
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in SetResetToken", ex)
        End Try
    End Sub

    Public Sub UpdateLastLogin(userID As Integer)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET LastLoginDate = GETDATE() WHERE UserID = @UserID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in UpdateLastLogin", ex)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' V2 PREFERENCES
    ' ─────────────────────────────────────────────────────────────

    Public Function UpdateDarkMode(userID As Integer, darkMode As Boolean) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET DarkMode = @DarkMode WHERE UserID = @UserID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@DarkMode", darkMode)
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception("Database error in UpdateDarkMode", ex)
        End Try
    End Function

    ''' <summary>
    ''' Reactivates a previously deactivated user by setting IsActive = 1.
    ''' Allows the user to log in again.
    ''' </summary>
    Public Function ReactivateUser(userID As Integer) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblUsers SET IsActive = 1 WHERE UserID = @UserID"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in ReactivateUser: {ex.Message}", ex)
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PRIVATE HELPERS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>Maps a SqlDataReader row to a User model object.</summary>
    Private Function MapReaderToUser(reader As SqlDataReader) As User
        Dim u As New User()
        u.UserID       = reader.GetInt32(reader.GetOrdinal("UserID"))
        u.Username     = reader.GetString(reader.GetOrdinal("Username"))
        u.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
        u.FullName     = reader.GetString(reader.GetOrdinal("FullName"))
        u.Email        = If(reader.IsDBNull(reader.GetOrdinal("Email")), String.Empty,
                            reader.GetString(reader.GetOrdinal("Email")))
        u.Role         = reader.GetString(reader.GetOrdinal("Role"))
        u.IsActive     = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        u.DateCreated  = reader.GetDateTime(reader.GetOrdinal("DateCreated"))

        ' V2 Properties
        u.FailedLoginCount = reader.GetInt32(reader.GetOrdinal("FailedLoginCount"))
        u.LockoutEndTime = If(reader.IsDBNull(reader.GetOrdinal("LockoutEndTime")), Nothing, CType(reader.GetDateTime(reader.GetOrdinal("LockoutEndTime")), DateTime?))
        u.LastLoginDate = If(reader.IsDBNull(reader.GetOrdinal("LastLoginDate")), Nothing, CType(reader.GetDateTime(reader.GetOrdinal("LastLoginDate")), DateTime?))
        u.PasswordResetToken = If(reader.IsDBNull(reader.GetOrdinal("PasswordResetToken")), Nothing, reader.GetString(reader.GetOrdinal("PasswordResetToken")))
        u.ResetTokenExpiry = If(reader.IsDBNull(reader.GetOrdinal("ResetTokenExpiry")), Nothing, CType(reader.GetDateTime(reader.GetOrdinal("ResetTokenExpiry")), DateTime?))
        u.EmailVerified = reader.GetBoolean(reader.GetOrdinal("EmailVerified"))
        u.FavouriteCategory = If(reader.IsDBNull(reader.GetOrdinal("FavouriteCategory")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("FavouriteCategory")), Integer?))
        u.DarkMode = reader.GetBoolean(reader.GetOrdinal("DarkMode"))

        Return u
    End Function

End Class
