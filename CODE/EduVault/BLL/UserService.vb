Imports System.Text.RegularExpressions

''' <summary>Business rules for user profile updates (admin edit user).</summary>
Public Class UserService

    Private ReadOnly _userRepo As New UserRepository()

    Public Function UpdateUserProfile(user As User, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If user Is Nothing OrElse user.UserID <= 0 Then
            errorMessage = "Invalid user." : Return False
        End If
        If String.IsNullOrWhiteSpace(user.FullName) Then
            errorMessage = "Full name is required." : Return False
        End If
        If Not String.IsNullOrWhiteSpace(user.Email) AndAlso
           Not Regex.IsMatch(user.Email, "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
            errorMessage = "Email address format is invalid." : Return False
        End If
        If user.Role <> "Admin" AndAlso user.Role <> "Student" Then
            errorMessage = "Role must be Admin or Student." : Return False
        End If
        Try
            Return _userRepo.UpdateUser(user)
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function SetDarkMode(userID As Integer, darkMode As Boolean) As Boolean
        If userID <= 0 Then Return False
        Return _userRepo.UpdateDarkMode(userID, darkMode)
    End Function

    Public Function LoadDarkMode(userID As Integer) As Boolean
        If userID <= 0 Then Return False
        Dim u As User = _userRepo.GetUserByID(userID)
        Return u IsNot Nothing AndAlso u.DarkMode
    End Function

End Class
