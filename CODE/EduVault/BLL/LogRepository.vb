Imports System.Data.SqlClient

Public Class LogRepository
    Implements ILogger

    Public Sub LogInfo(message As String) Implements ILogger.LogInfo
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "INSERT INTO tblLog (LogLevel, Message, LogDate) VALUES (@Level, @Message, GETDATE())"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Level", SqlDbType.NVarChar, 10).Value = "Info"
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 500).Value = message
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Console.WriteLine($"LogInfo failed: {ex.Message}")
        End Try
    End Sub

    Public Sub LogError(message As String) Implements ILogger.LogError
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "INSERT INTO tblLog (LogLevel, Message, LogDate) VALUES (@Level, @Message, GETDATE())"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Level", SqlDbType.NVarChar, 10).Value = "Error"
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 500).Value = message
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Console.WriteLine($"LogError failed: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Retrieves all system logs for the Admin audit viewer.
    ''' </summary>
    Public Function GetSystemLogs() As DataTable
        Dim dt As New DataTable("SystemLogs")
        dt.Columns.Add("LogID", GetType(Integer))
        dt.Columns.Add("LogLevel", GetType(String))
        dt.Columns.Add("Message", GetType(String))
        dt.Columns.Add("LogDate", GetType(DateTime))

        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "SELECT LogID, LogLevel, Message, LogDate FROM tblLog ORDER BY LogDate DESC"
                Using cmd As New SqlCommand(sql, conn)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            dt.Rows.Add(
                                reader("LogID"),
                                reader("LogLevel"),
                                reader("Message"),
                                reader("LogDate")
                            )
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Console.WriteLine($"GetSystemLogs failed: {ex.Message}")
        End Try
        Return dt
    End Function

End Class
