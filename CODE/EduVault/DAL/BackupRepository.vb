Imports System.Data.SqlClient

''' <summary>
''' Data Access Layer for system maintenance and backups.
''' </summary>
Public Class BackupRepository

    ''' <summary>
    ''' Performs a full SQL Server database backup to the specified path.
    ''' Requires appropriate SQL Server permissions.
    ''' </summary>
    Public Function BackupDatabase(backupPath As String) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                ' We need to switch to 'master' database or use fully qualified names for the BACKUP command
                ' but usually, running it from the current connection is fine if permissions allow.
                Dim dbName As String = conn.Database
                Dim sql As String = $"BACKUP DATABASE [{dbName}] TO DISK = @Path WITH FORMAT, MEDIANAME = 'EduVaultBackup', NAME = 'Full Backup of EduVaultDB';"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@Path", backupPath)
                    ' Backups can take a while
                    cmd.CommandTimeout = 300 
                    cmd.ExecuteNonQuery()
                    Return True
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"SQL Backup Error: {ex.Message}", ex)
        End Try
    End Function

End Class
