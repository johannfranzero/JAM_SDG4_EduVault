Imports System.Data.SqlClient
Imports System.Configuration

''' <summary>
''' Centralized database connection factory for the EduVault DAL.
''' All repository classes use this helper to obtain SqlConnection objects.
''' Connection string is read from App.config - never hardcoded.
''' </summary>
Public Class DatabaseHelper

    ''' <summary>Cached connection string read once from App.config.</summary>
    Private Shared ReadOnly _connectionString As String =
        ConfigurationManager.ConnectionStrings("EduVaultDB").ConnectionString

    ''' <summary>
    ''' Creates and returns a new, already-opened SqlConnection.
    ''' The caller is responsible for closing/disposing the connection.
    ''' Always wrap usage in a Using block to guarantee disposal.
    ''' </summary>
    ''' <returns>An open SqlConnection ready for use.</returns>
    ''' <exception cref="SqlException">Thrown if the connection cannot be established.</exception>
    Public Shared Function GetConnection() As SqlConnection
        Dim conn As New SqlConnection(_connectionString)
        conn.Open()
        Return conn
    End Function

    ''' <summary>
    ''' Tests the database connection and returns True if successful.
    ''' Used on application startup to verify connectivity.
    ''' </summary>
    Public Shared Function TestConnection() As Boolean
        Try
            Using conn As SqlConnection = GetConnection()
                Return conn.State = ConnectionState.Open
            End Using
        Catch
            Return False
        End Try
    End Function

End Class
