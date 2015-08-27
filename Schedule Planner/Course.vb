Public Class Course
    Public Property timeStart As Double
    Public Property timeEnd As Double
    Public Property name As String
    Public Property professor As String
    Public Property location As String
    Public Property days As Boolean() = New Boolean(4) {}
    Public Property type As String
    Public Property section As String

    Public Sub New()

    End Sub
    Public Sub New(name As String, professor As String, type As String, section As String, days As String, tStart As Integer, tEnd As Integer, location As String)
        timeStart = tStart
        timeEnd = tEnd
        Me.name = name
        Me.professor = professor
        Me.location = location
        Me.type = type
        Me.section = section

        For Each c As Char In days
            Select Case c
                Case "M"
                    Me.days(0) = True
                Case "T"
                    Me.days(1) = True
                Case "W"
                    Me.days(2) = True
                Case "R"
                    Me.days(3) = True
                Case "F"
                    Me.days(4) = True
            End Select
        Next
    End Sub

    Public Shared Function ConvertTime(t As String) As Double
        Dim time As Double
        Dim tokens As String() = t.Split(New Char() {":"c})
        time += Convert.ToDouble(tokens(0))
        If tokens(1)(2) = "P" And time <> 12 Then
            time += 12
        End If
        time += Convert.ToDouble(tokens(1).Substring(0, 2)) / 60
        Return time
    End Function

    Public Sub SetDays(d As String)
        For Each c As Char In d
            Select Case c
                Case "M"
                    Me.days(0) = True
                Case "T"
                    Me.days(1) = True
                Case "W"
                    Me.days(2) = True
                Case "R"
                    Me.days(3) = True
                Case "F"
                    Me.days(4) = True
            End Select
        Next
    End Sub

End Class
