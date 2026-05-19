Imports System.Data.SqlClient

''' <summary>
''' Data Access Layer for tblCategories.
''' Provides CRUD operations for resource categories.
''' </summary>
Public Class CategoryRepository

    ''' <summary>Returns all active categories ordered by CategoryName.</summary>
    Public Function GetAllCategories() As List(Of Category)
        Dim cats As New List(Of Category)
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT c.CategoryID, c.CategoryName, c.Description, c.IsActive, c.DateCreated,
                            COUNT(r.ResourceID) AS ResourceCount
                     FROM tblCategories c
                     LEFT JOIN tblResources r ON c.CategoryID = r.CategoryID AND r.IsActive = 1
                     WHERE c.IsActive = 1
                     GROUP BY c.CategoryID, c.CategoryName, c.Description, c.IsActive, c.DateCreated
                     ORDER BY c.CategoryName ASC"
                Using cmd As New SqlCommand(sql, conn)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            cats.Add(MapReaderToCategory(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetAllCategories: {ex.Message}", ex)
        End Try
        Return cats
    End Function

    ''' <summary>Returns a single category by CategoryID. Returns Nothing if not found.</summary>
    Public Function GetCategoryByID(categoryID As Integer) As Category
        Dim cat As Category = Nothing
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT CategoryID, CategoryName, Description, IsActive, DateCreated, 0 AS ResourceCount
                     FROM tblCategories
                     WHERE CategoryID = @CategoryID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryID
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then cat = MapReaderToCategory(reader)
                    End Using
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in GetCategoryByID: {ex.Message}", ex)
        End Try
        Return cat
    End Function

    ''' <summary>Returns True if a category with the given name already exists.</summary>
    Public Function CategoryNameExists(name As String, Optional excludeID As Integer = 0) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "SELECT COUNT(1) FROM tblCategories
                     WHERE CategoryName = @Name AND CategoryID <> @ExcludeID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@Name",      SqlDbType.NVarChar, 100).Value = name
                    cmd.Parameters.Add("@ExcludeID", SqlDbType.Int).Value            = excludeID
                    Return CInt(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in CategoryNameExists: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>Inserts a new category. Returns the new CategoryID.</summary>
    Public Function AddCategory(cat As Category) As Integer
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "INSERT INTO tblCategories (CategoryName, Description)
                     VALUES (@CategoryName, @Description);
                     SELECT CAST(SCOPE_IDENTITY() AS INT);"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = cat.CategoryName
                    cmd.Parameters.Add("@Description",  SqlDbType.NVarChar, 500).Value =
                        If(String.IsNullOrWhiteSpace(cat.Description), DBNull.Value, CObj(cat.Description))
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in AddCategory: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>Updates the name and description of an existing category. Returns True on success.</summary>
    Public Function UpdateCategory(cat As Category) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String =
                    "UPDATE tblCategories
                     SET CategoryName = @CategoryName, Description = @Description
                     WHERE CategoryID = @CategoryID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = cat.CategoryName
                    cmd.Parameters.Add("@Description",  SqlDbType.NVarChar, 500).Value =
                        If(String.IsNullOrWhiteSpace(cat.Description), DBNull.Value, CObj(cat.Description))
                    cmd.Parameters.Add("@CategoryID",   SqlDbType.Int).Value            = cat.CategoryID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in UpdateCategory: {ex.Message}", ex)
        End Try
    End Function

    ''' <summary>
    ''' Soft-deletes a category (sets IsActive = 0).
    ''' Only allowed if the category has no active resources (enforced in BLL).
    ''' </summary>
    Public Function DeleteCategory(categoryID As Integer) As Boolean
        Try
            Using conn As SqlConnection = DatabaseHelper.GetConnection()
                Dim sql As String = "UPDATE tblCategories SET IsActive = 0 WHERE CategoryID = @CategoryID"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As SqlException
            Throw New Exception($"Database error in DeleteCategory: {ex.Message}", ex)
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' PRIVATE HELPERS
    ' ─────────────────────────────────────────────────────────────

    Private Function MapReaderToCategory(reader As SqlDataReader) As Category
        Dim c As New Category()
        c.CategoryID    = reader.GetInt32(reader.GetOrdinal("CategoryID"))
        c.CategoryName  = reader.GetString(reader.GetOrdinal("CategoryName"))
        c.Description   = If(reader.IsDBNull(reader.GetOrdinal("Description")), String.Empty,
                             reader.GetString(reader.GetOrdinal("Description")))
        c.IsActive      = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        c.DateCreated   = reader.GetDateTime(reader.GetOrdinal("DateCreated"))
        c.ResourceCount = reader.GetInt32(reader.GetOrdinal("ResourceCount"))
        Return c
    End Function

End Class
