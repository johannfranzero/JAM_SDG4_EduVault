Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' Business Logic Layer - Authentication Service.
''' Handles login, logout, password hashing, and input validation.
''' Depends on UserRepository for all DB operations.
''' </summary>
Public Class AuthService

    Private ReadOnly _userRepo As New UserRepository()

    ' ─────────────────────────────────────────────────────────────
    ' AUTHENTICATION
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Attempts to log in with the given credentials.
    ''' On success: populates Session.CurrentUser and returns True.
    ''' On failure: sets errorMessage and returns False.
    ''' </summary>
    Public Function Login(username As String, password As String,
                          ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty

        ' --- Input validation ---
        If String.IsNullOrWhiteSpace(username) Then
            errorMessage = "Username is required."
            Return False
        End If
        If String.IsNullOrWhiteSpace(password) Then
            errorMessage = "Password is required."
            Return False
        End If

        Try
            ' Retrieve the user record — returns Nothing if not found
            Dim user As User = _userRepo.GetUserByUsername(username.Trim())

            If user Is Nothing Then
                errorMessage = "Invalid username or password."
                Return False
            End If

            ' --- Check for Lockout ---
            If user.IsLockedOut Then
                Dim remaining As Integer = CInt((user.LockoutEndTime.Value - DateTime.Now).TotalMinutes)
                errorMessage = $"Your account is locked due to too many failed attempts. Try again in {remaining + 1} minutes."
                Return False
            End If

            If Not user.IsActive Then
                errorMessage = "Your account has been deactivated. Please contact the administrator."
                Return False
            End If

            ' Compare the entered password hash against the stored hash
            Dim enteredHash As String = GenerateHash(password)
            If Not String.Equals(enteredHash, user.PasswordHash, StringComparison.Ordinal) Then
                ' Increment failed attempts
                Dim attempts As Integer = _userRepo.UpdateLoginAttempts(username)
                If attempts >= 3 Then
                    _userRepo.SetLockout(user.UserID, DateTime.Now.AddMinutes(15))
                    errorMessage = "Account locked for 15 minutes due to 3 failed attempts."
                Else
                    errorMessage = $"Invalid username or password. ({3 - attempts} attempts remaining)"
                End If
                Return False
            End If

            ' --- Login successful ---
            _userRepo.ResetLoginAttempts(user.UserID)
            _userRepo.UpdateLastLogin(user.UserID)
            Session.CurrentUser = user
            Return True

        Catch ex As Exception
            errorMessage = $"Login error: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>Clears the current session. Call this on Logout button click.</summary>
    Public Sub Logout()
        Session.Logout()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' USER REGISTRATION
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Registers a new user after validating all inputs.
    ''' Returns True on success; sets errorMessage on failure.
    ''' </summary>
    Public Function RegisterUser(username As String, password As String,
                                 confirmPassword As String, fullName As String,
                                 email As String, role As String,
                                 ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty

        ' --- Validate inputs ---
        If String.IsNullOrWhiteSpace(username) Then
            errorMessage = "Username is required." : Return False
        End If
        If username.Length < 4 OrElse username.Length > 50 Then
            errorMessage = "Username must be between 4 and 50 characters." : Return False
        End If
        If Not Regex.IsMatch(username, "^[a-zA-Z0-9_]+$") Then
            errorMessage = "Username may only contain letters, numbers, and underscores." : Return False
        End If
        If String.IsNullOrWhiteSpace(fullName) Then
            errorMessage = "Full name is required." : Return False
        End If
        If Not String.IsNullOrWhiteSpace(email) AndAlso
           Not Regex.IsMatch(email, "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
            errorMessage = "Email address format is invalid." : Return False
        End If

        Dim passError As String = ValidatePasswordStrength(password)
        If passError IsNot Nothing Then
            errorMessage = passError : Return False
        End If
        If password <> confirmPassword Then
            errorMessage = "Passwords do not match." : Return False
        End If
        If role <> "Admin" AndAlso role <> "Student" Then
            errorMessage = "Role must be 'Admin' or 'Student'." : Return False
        End If

        ' --- Check for duplicate username ---
        If _userRepo.UsernameExists(username.Trim()) Then
            errorMessage = $"Username '{username}' is already taken. Please choose another." : Return False
        End If

        Try
            Dim newUser As New User()
            newUser.Username     = username.Trim()
            newUser.PasswordHash = GenerateHash(password)
            newUser.FullName     = fullName.Trim()
            newUser.Email        = email.Trim()
            newUser.Role         = role
            newUser.IsActive     = True

            Dim newID As Integer = _userRepo.AddUser(newUser)
            Return newID > 0

        Catch ex As Exception
            errorMessage = $"Registration error: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Changes a user's password after verifying the current one.
    ''' Returns True on success; sets errorMessage on failure.
    ''' </summary>
    Public Function ChangePassword(userID As Integer, currentPassword As String,
                                   newPassword As String, confirmNew As String,
                                   ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty

        Dim user As User = _userRepo.GetUserByID(userID)
        If user Is Nothing Then
            errorMessage = "User not found." : Return False
        End If

        If GenerateHash(currentPassword) <> user.PasswordHash Then
            errorMessage = "Current password is incorrect." : Return False
        End If

        Dim passError As String = ValidatePasswordStrength(newPassword)
        If passError IsNot Nothing Then
            errorMessage = passError : Return False
        End If

        If newPassword <> confirmNew Then
            errorMessage = "New passwords do not match." : Return False
        End If

        Try
            Return _userRepo.UpdatePassword(userID, GenerateHash(newPassword))
        Catch ex As Exception
            errorMessage = $"Password change error: {ex.Message}"
            Return False
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' FORGOT PASSWORD & RESET
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Generates a cryptographically secure reset token. Only a SHA-256 hash is stored in the database.
    ''' Returns the raw token for display/email, or Nothing with a generic message if the user does not exist.
    ''' </summary>
    Public Function GenerateResetToken(username As String, ByRef errorMessage As String) As String
        errorMessage = String.Empty

        If String.IsNullOrWhiteSpace(username) Then
            errorMessage = "Username is required."
            Return Nothing
        End If

        Dim user As User = _userRepo.GetUserByUsername(username.Trim())
        If user Is Nothing Then
            errorMessage = "If that username exists, a reset code has been generated."
            Return Nothing
        End If

        Dim tokenBytes(15) As Byte
        Using rng As New RNGCryptoServiceProvider()
            rng.GetBytes(tokenBytes)
        End Using
        Dim rawToken As String = BitConverter.ToString(tokenBytes).Replace("-", "").ToUpper()

        Dim tokenHash As String = HashResetToken(rawToken)
        _userRepo.SetResetToken(user.UserID, tokenHash, DateTime.Now.AddHours(1))

        Return rawToken
    End Function

    Public Function ResetPasswordWithToken(username As String, token As String,
                                           newPassword As String, confirmPassword As String,
                                           ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty

        If String.IsNullOrWhiteSpace(username) Then
            errorMessage = "Username is required." : Return False
        End If
        If String.IsNullOrWhiteSpace(token) Then
            errorMessage = "Reset code is required." : Return False
        End If
        If String.IsNullOrWhiteSpace(newPassword) Then
            errorMessage = "New password is required." : Return False
        End If
        If newPassword <> confirmPassword Then
            errorMessage = "Passwords do not match." : Return False
        End If

        Dim user As User = _userRepo.GetUserByUsername(username.Trim())
        If user Is Nothing Then
            errorMessage = "Invalid or expired reset code." : Return False
        End If

        If user.ResetTokenExpiry Is Nothing OrElse user.ResetTokenExpiry < DateTime.Now Then
            errorMessage = "Invalid or expired reset code." : Return False
        End If

        If Not IsResetTokenValid(token.Trim(), user.PasswordResetToken) Then
            errorMessage = "Invalid or expired reset code." : Return False
        End If

        Dim passError As String = ValidatePasswordStrength(newPassword)
        If passError IsNot Nothing Then
            errorMessage = passError : Return False
        End If

        Try
            If _userRepo.UpdatePassword(user.UserID, GenerateHash(newPassword)) Then
                _userRepo.SetResetToken(user.UserID, Nothing, Nothing)
                Return True
            End If
            errorMessage = "Password reset failed. Please try again."
        Catch ex As Exception
            errorMessage = "An unexpected error occurred. Please try again."
        End Try
        Return False
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PASSWORD HASHING
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Generates a SHA-256 hash of the given plain-text password.
    ''' Returns the hash as an uppercase hex string.
    ''' </summary>
    Public Shared Function GenerateHash(plainText As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(plainText)
            Dim hashBytes As Byte() = sha256.ComputeHash(bytes)
            ' Convert byte array to uppercase hex string
            Dim sb As New StringBuilder()
            For Each b As Byte In hashBytes
                sb.Append(b.ToString("X2"))
            Next
            Return sb.ToString()
        End Using
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PRIVATE HELPERS
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Validates password strength rules.
    ''' Returns an error message string, or Nothing if the password is valid.
    ''' Rules: min 8 chars, at least 1 uppercase, 1 lowercase, 1 digit, 1 special char.
    ''' </summary>
    Private Function ValidatePasswordStrength(password As String) As String
        If String.IsNullOrWhiteSpace(password) Then Return "Password is required."
        If password.Length < 8 Then Return "Password must be at least 8 characters long."
        If Not Regex.IsMatch(password, "[A-Z]") Then Return "Password must contain at least one uppercase letter."
        If Not Regex.IsMatch(password, "[a-z]") Then Return "Password must contain at least one lowercase letter."
        If Not Regex.IsMatch(password, "[0-9]") Then Return "Password must contain at least one number."
        If Not Regex.IsMatch(password, "[^a-zA-Z0-9]") Then Return "Password must contain at least one special character."
        Return Nothing  ' Password passes all rules
    End Function

    Private Shared Function HashResetToken(rawToken As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(rawToken)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToUpper()
        End Using
    End Function

    ''' <summary>Validates submitted token against stored SHA-256 hash; accepts legacy plain tokens once.</summary>
    Private Shared Function IsResetTokenValid(submittedToken As String, storedToken As String) As Boolean
        If String.IsNullOrEmpty(storedToken) Then Return False

        Dim submittedHash As String = HashResetToken(submittedToken)
        If ConstantTimeEquals(submittedHash, storedToken) Then Return True

        ' Legacy DB rows may still hold a plain token from before hashing was enabled
        Return storedToken.Length <= 64 AndAlso ConstantTimeEquals(submittedToken, storedToken)
    End Function

    Private Shared Function ConstantTimeEquals(a As String, b As String) As Boolean
        If a Is Nothing OrElse b Is Nothing Then Return False
        Dim aBytes As Byte() = Encoding.UTF8.GetBytes(a)
        Dim bBytes As Byte() = Encoding.UTF8.GetBytes(b)
        Dim diff As Integer = aBytes.Length Xor bBytes.Length
        Dim maxLen As Integer = Math.Max(aBytes.Length, bBytes.Length)
        For i As Integer = 0 To maxLen - 1
            Dim aByte As Integer = If(i < aBytes.Length, aBytes(i), 0)
            Dim bByte As Integer = If(i < bBytes.Length, bBytes(i), 0)
            diff = diff Or (aByte Xor bByte)
        Next
        Return diff = 0
    End Function

End Class
