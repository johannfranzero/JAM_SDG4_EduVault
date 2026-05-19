Imports System.IO
Imports System.Runtime.InteropServices

Namespace SqlServerTypes

    ''' <summary>
    ''' Utility methods related to CLR Types for SQL Server.
    ''' </summary>
    Public Class Utilities

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Private Shared Function LoadLibrary(libname As String) As IntPtr
        End Function

        ''' <summary>
        ''' Loads the required native assemblies for the current architecture (x86 or x64).
        ''' </summary>
        ''' <param name="rootApplicationPath">
        ''' Root path of the current application. Use AppDomain.CurrentDomain.BaseDirectory
        ''' for desktop applications.
        ''' </param>
        Public Shared Sub LoadNativeAssemblies(rootApplicationPath As String)
            Dim nativeBinaryPath As String = If(IntPtr.Size > 4,
                IO.Path.Combine(rootApplicationPath, "SqlServerTypes\x64\"),
                IO.Path.Combine(rootApplicationPath, "SqlServerTypes\x86\"))

            LoadNativeAssembly(nativeBinaryPath, "msvcr120.dll")
            LoadNativeAssembly(nativeBinaryPath, "SqlServerSpatial140.dll")
        End Sub

        Private Shared Sub LoadNativeAssembly(nativeBinaryPath As String, assemblyName As String)
            Dim filePath As String = IO.Path.Combine(nativeBinaryPath, assemblyName)
            Dim ptr As IntPtr = LoadLibrary(filePath)
            If ptr = IntPtr.Zero Then
                Throw New Exception(
                    String.Format("Error loading {0} (ErrorCode: {1})",
                                  assemblyName,
                                  Marshal.GetLastWin32Error()))
            End If
        End Sub

    End Class

End Namespace
