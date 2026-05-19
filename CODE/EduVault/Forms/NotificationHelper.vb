Imports System.Windows.Forms

Public Class NotificationHelper
    Public Shared Sub ShowInfo(message As String)
        Dim toast As New frmToast(message, isError:=False)
        toast.Show()
    End Sub

    Public Shared Sub ShowError(message As String)
        Dim toast As New frmToast(message, isError:=True)
        toast.Show()
    End Sub
End Class
