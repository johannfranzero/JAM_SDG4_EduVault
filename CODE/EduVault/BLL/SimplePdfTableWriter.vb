Imports System.IO
Imports System.Text
Imports System.Data
Imports System.Linq
Imports System.Globalization

''' <summary>
''' Writes a simple tabular PDF without external libraries (avoids Report Viewer PInvoke PDF bugs).
''' </summary>
Public Class SimplePdfTableWriter

    Private Const PageWidth As Double = 612
    Private Const PageHeight As Double = 792
    Private Const MarginLeft As Double = 40
    Private Const MarginTop As Double = 40
    Private Const MarginBottom As Double = 40
    Private Const LineHeight As Double = 13

    Public Sub WriteTable(filePath As String, title As String, dt As DataTable)
        If dt Is Nothing Then Throw New ArgumentNullException(NameOf(dt))

        Dim pages As New List(Of StringBuilder)()
        Dim page As StringBuilder = StartPage(pages)
        Dim y As Double = PageHeight - MarginTop

        y = DrawText(page, title, 18, True, y, "0.10 0.15 0.30 rg")
        y -= 2
        y = DrawText(page, $"Generated: {DateTime.Now:MMMM dd, yyyy  h:mm tt}", 9, False, y, "0.4 0.4 0.4 rg")
        y -= 16

        Dim formatHeader = Function(h As String) As String
                               Select Case h
                                   Case "ResourceTitle" : Return "Resource Title"
                                   Case "CategoryName" : Return "Category"
                                   Case "ResourceType" : Return "Type"
                                   Case "TotalAccesses" : Return "Accesses"
                                   Case "UniqueUsers" : Return "Users"
                                   Case Else : Return h
                               End Select
                           End Function

        Dim headers As String() = dt.Columns.Cast(Of DataColumn)().Select(Function(c) formatHeader(c.ColumnName)).ToArray()
        y = DrawRow(page, headers, True, y)

        For Each row As DataRow In dt.Rows
            If y < MarginBottom + 20 Then
                page = StartPage(pages)
                y = PageHeight - MarginTop
                y = DrawRow(page, headers, True, y)
            End If

            Dim values As String() = row.ItemArray.Select(Function(v) If(v?.ToString(), String.Empty)).ToArray()
            y = DrawRow(page, values, False, y)
        Next

        WritePdfFile(filePath, pages)
    End Sub

    Private Shared Function StartPage(pages As List(Of StringBuilder)) As StringBuilder
        Dim page As New StringBuilder()
        page.Append("BT" & vbCrLf)
        pages.Add(page)
        Return page
    End Function

    Private Shared Function DrawText(page As StringBuilder, text As String, fontSize As Integer, bold As Boolean, y As Double, color As String) As Double
        Dim font As String = If(bold, "/F2", "/F1")
        page.Append(color & vbCrLf)
        page.Append($"{font} {fontSize} Tf 1 0 0 1 {MarginLeft.ToString("0.##", CultureInfo.InvariantCulture)} {y.ToString("0.##", CultureInfo.InvariantCulture)} Tm ({EscapePdfText(text)}) Tj" & vbCrLf)
        Return y - (fontSize + 4)
    End Function

    Private Shared Function DrawRow(page As StringBuilder, values As String(), isHeader As Boolean, y As Double) As Double
        Dim colWidths As Double()
        If values.Length = 6 Then
            colWidths = New Double() {75, 175, 105, 65, 56, 56}
        Else
            Dim w As Double = 532.0 / values.Length
            colWidths = Enumerable.Repeat(w, values.Length).ToArray()
        End If
        
        Dim rowHeight As Double = If(isHeader, 26, 22)
        Dim textY As Double = y - If(isHeader, 17, 15)
        
        ' End text mode to draw paths
        page.Append("ET" & vbCrLf)
        
        If isHeader Then
            page.Append("0.96 0.97 0.98 rg" & vbCrLf) ' Light blue background
            page.Append($"{MarginLeft.ToString("0.##", CultureInfo.InvariantCulture)} {(y - rowHeight).ToString("0.##", CultureInfo.InvariantCulture)} 532 {rowHeight.ToString("0.##", CultureInfo.InvariantCulture)} re f" & vbCrLf)
            page.Append("1 w 0.85 0.88 0.92 RG" & vbCrLf)
            page.Append($"{MarginLeft.ToString("0.##", CultureInfo.InvariantCulture)} {(y - rowHeight).ToString("0.##", CultureInfo.InvariantCulture)} m {(MarginLeft + 532).ToString("0.##", CultureInfo.InvariantCulture)} {(y - rowHeight).ToString("0.##", CultureInfo.InvariantCulture)} l S" & vbCrLf)
        Else
            page.Append("0.5 w 0.92 0.92 0.92 RG" & vbCrLf)
            page.Append($"{MarginLeft.ToString("0.##", CultureInfo.InvariantCulture)} {(y - rowHeight).ToString("0.##", CultureInfo.InvariantCulture)} m {(MarginLeft + 532).ToString("0.##", CultureInfo.InvariantCulture)} {(y - rowHeight).ToString("0.##", CultureInfo.InvariantCulture)} l S" & vbCrLf)
        End If

        ' Restart text mode
        page.Append("BT" & vbCrLf)
        
        Dim font As String = If(isHeader, "/F2", "/F1")
        Dim fontSize As Integer = If(isHeader, 9, 9)
        
        If isHeader Then
            page.Append("0.2 0.25 0.35 rg" & vbCrLf)
        Else
            page.Append("0.2 0.2 0.2 rg" & vbCrLf)
        End If
        
        Dim currentX As Double = MarginLeft + 8

        For i As Integer = 0 To Math.Min(values.Length - 1, colWidths.Length - 1)
            Dim width As Double = colWidths(i)
            Dim maxLen As Integer = CInt(width / 4.8)
            Dim text As String = Truncate(values(i), maxLen)
            
            page.Append($"{font} {fontSize} Tf 1 0 0 1 {currentX.ToString("0.##", CultureInfo.InvariantCulture)} {textY.ToString("0.##", CultureInfo.InvariantCulture)} Tm ({EscapePdfText(text)}) Tj" & vbCrLf)
            currentX += width
        Next
        
        Return y - rowHeight
    End Function

    Private Shared Function Truncate(value As String, maxLen As Integer) As String
        If String.IsNullOrEmpty(value) Then Return String.Empty
        If value.Length <= maxLen Then Return value
        Return value.Substring(0, maxLen - 3) & "..."
    End Function

    Private Shared Function EscapePdfText(value As String) As String
        If String.IsNullOrEmpty(value) Then Return String.Empty
        Return value.Replace("\", "\\").Replace("(", "\(").Replace(")", "\)")
    End Function

    Private Shared Sub WritePdfFile(filePath As String, pageContents As List(Of StringBuilder))
        If pageContents.Count = 0 Then
            pageContents.Add(New StringBuilder("BT" & vbCrLf))
        End If

        Dim body As New StringBuilder()
        body.Append("%PDF-1.4" & vbCrLf)
        body.Append("%" & ChrW(&HE2) & ChrW(&HE3) & ChrW(&HCF) & ChrW(&HD3) & vbCrLf)
        Dim offsets As New List(Of Long)()

        Dim pageKids As New StringBuilder()
        For i As Integer = 0 To pageContents.Count - 1
            pageKids.Append((6 + 2 * i).ToString(CultureInfo.InvariantCulture))
            pageKids.Append(" 0 R ")
        Next

        AppendPdfObject(body, offsets, "<< /Type /Catalog /Pages 2 0 R >>")
        AppendPdfObject(body, offsets, "<< /Type /Pages /Kids [" & pageKids.ToString().Trim() & "] /Count " & pageContents.Count.ToString(CultureInfo.InvariantCulture) & " >>")
        AppendPdfObject(body, offsets, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>")
        AppendPdfObject(body, offsets, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>")

        For i As Integer = 0 To pageContents.Count - 1
            pageContents(i).Append("ET" & vbCrLf)
            Dim stream As String = pageContents(i).ToString()
            Dim streamBytes As Integer = Encoding.ASCII.GetByteCount(stream)
            Dim contentObj As String = "<< /Length " & streamBytes.ToString(CultureInfo.InvariantCulture) & " >>" & vbCrLf &
                                       "stream" & vbCrLf & stream & vbCrLf & "endstream"
            AppendPdfObject(body, offsets, contentObj)

            Dim pageObj As String = "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] " &
                                    "/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> " &
                                    "/Contents " & (5 + 2 * i).ToString(CultureInfo.InvariantCulture) & " 0 R >>"
            AppendPdfObject(body, offsets, pageObj)
        Next

        Dim xrefPos As Long = Encoding.ASCII.GetByteCount(body.ToString())
        Dim totalObjects As Integer = offsets.Count

        body.Append("xref" & vbCrLf)
        body.Append("0 " & (totalObjects + 1).ToString(CultureInfo.InvariantCulture) & vbCrLf)
        body.Append("0000000000 65535 f" & vbCrLf)
        For Each offset As Long In offsets
            body.Append(offset.ToString("D10", CultureInfo.InvariantCulture) & " 00000 n" & vbCrLf)
        Next

        body.Append("trailer" & vbCrLf)
        body.Append("<< /Size " & (totalObjects + 1).ToString(CultureInfo.InvariantCulture) & " /Root 1 0 R >>" & vbCrLf)
        body.Append("startxref" & vbCrLf)
        body.Append(xrefPos.ToString(CultureInfo.InvariantCulture) & vbCrLf)
        body.Append("%%EOF" & vbCrLf)

        File.WriteAllBytes(filePath, Encoding.ASCII.GetBytes(body.ToString()))
    End Sub

    Private Shared Sub AppendPdfObject(body As StringBuilder, offsets As List(Of Long), content As String)
        offsets.Add(Encoding.ASCII.GetByteCount(body.ToString()))
        Dim objNum As Integer = offsets.Count
        body.Append(objNum.ToString(CultureInfo.InvariantCulture) & " 0 obj" & vbCrLf)
        body.Append(content & vbCrLf)
        body.Append("endobj" & vbCrLf)
    End Sub

End Class
