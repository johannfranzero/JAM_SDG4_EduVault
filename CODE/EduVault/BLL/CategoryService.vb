''' <summary>
''' Business Logic Layer - Category Service.
''' Enforces rules around category creation and deletion.
''' </summary>
Public Class CategoryService

    Private ReadOnly _catRepo As New CategoryRepository()

    ''' <summary>Returns all active categories for populating ComboBox controls.</summary>
    Public Function GetAllCategories() As List(Of Category)
        Return _catRepo.GetAllCategories()
    End Function

    ''' <summary>Adds a new category after validating name uniqueness and length.</summary>
    Public Function AddCategory(name As String, description As String,
                                ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If String.IsNullOrWhiteSpace(name) Then
            errorMessage = "Category name is required." : Return False
        End If
        If name.Length > 100 Then
            errorMessage = "Category name must not exceed 100 characters." : Return False
        End If
        If _catRepo.CategoryNameExists(name.Trim()) Then
            errorMessage = $"Category '{name}' already exists." : Return False
        End If
        Try
            Dim cat As New Category() With {.CategoryName = name.Trim(), .Description = description?.Trim()}
            Return _catRepo.AddCategory(cat) > 0
        Catch ex As Exception
            errorMessage = $"Error adding category: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>Updates an existing category name and description.</summary>
    Public Function UpdateCategory(categoryID As Integer, name As String,
                                   description As String, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If categoryID <= 0 Then errorMessage = "Invalid category ID." : Return False
        If String.IsNullOrWhiteSpace(name) Then errorMessage = "Category name is required." : Return False
        If _catRepo.CategoryNameExists(name.Trim(), categoryID) Then
            errorMessage = $"Category '{name}' already exists." : Return False
        End If
        Try
            Dim cat As New Category() With {
                .CategoryID   = categoryID,
                .CategoryName = name.Trim(),
                .Description  = description?.Trim()
            }
            Return _catRepo.UpdateCategory(cat)
        Catch ex As Exception
            errorMessage = $"Error updating category: {ex.Message}"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Soft-deletes a category.
    ''' Business Rule: Cannot delete a category that still has active resources.
    ''' </summary>
    Public Function DeleteCategory(categoryID As Integer, ByRef errorMessage As String) As Boolean
        errorMessage = String.Empty
        If categoryID <= 0 Then errorMessage = "Invalid category ID." : Return False

        ' Check if any active resources use this category
        Dim cat As Category = _catRepo.GetCategoryByID(categoryID)
        If cat Is Nothing Then errorMessage = "Category not found." : Return False

        ' Re-fetch with resource count to enforce the rule
        Dim allCats As List(Of Category) = _catRepo.GetAllCategories()
        Dim target As Category = allCats.FirstOrDefault(Function(c) c.CategoryID = categoryID)
        If target IsNot Nothing AndAlso target.ResourceCount > 0 Then
            errorMessage = $"Cannot delete '{target.CategoryName}' - it has {target.ResourceCount} active resource(s). " &
                           "Please reassign or remove those resources first."
            Return False
        End If

        Try
            Return _catRepo.DeleteCategory(categoryID)
        Catch ex As Exception
            errorMessage = $"Error deleting category: {ex.Message}"
            Return False
        End Try
    End Function

End Class
