''' <summary>
''' Singleton session holder for the currently authenticated user.
''' Stores the logged-in user object and provides role-check helpers.
''' All forms read from this class to determine what the current user can do.
''' </summary>
Public Module Session

    ''' <summary>
    ''' The currently logged-in user.
    ''' Set by AuthService.Login() on successful authentication.
    ''' Nothing if no user is logged in.
    ''' </summary>
    Public CurrentUser As User = Nothing

    ''' <summary>Returns True if any user is currently logged in.</summary>
    Public ReadOnly Property IsLoggedIn As Boolean
        Get
            Return CurrentUser IsNot Nothing
        End Get
    End Property

    ''' <summary>
    ''' True when browsing without a database account (guest login).
    ''' Guest sessions must not write to tblAccessLog or tblBookmarks.
    ''' </summary>
    Public ReadOnly Property IsGuest As Boolean
        Get
            Return IsLoggedIn AndAlso CurrentUser.UserID <= 0
        End Get
    End Property

    ''' <summary>Database UserID for the current session, or 0 for guest / not logged in.</summary>
    Public ReadOnly Property CurrentUserID As Integer
        Get
            If Not IsLoggedIn OrElse IsGuest Then Return 0
            Return CurrentUser.UserID
        End Get
    End Property

    ''' <summary>Returns True if the current user has the Admin role.</summary>
    Public ReadOnly Property IsAdmin As Boolean
        Get
            Return IsLoggedIn AndAlso Not IsGuest AndAlso CurrentUser.IsAdmin
        End Get
    End Property

    ''' <summary>Returns the current user's display name, or an empty string.</summary>
    Public ReadOnly Property DisplayName As String
        Get
            If IsLoggedIn Then Return CurrentUser.FullName
            Return String.Empty
        End Get
    End Property

    ''' <summary>
    ''' Clears the current session - called on logout.
    ''' Sets CurrentUser to Nothing so no further privileged actions can be taken.
    ''' </summary>
    Public Sub Logout()
        CurrentUser = Nothing
    End Sub

End Module
