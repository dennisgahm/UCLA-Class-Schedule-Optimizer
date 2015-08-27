Public Class CourseOptimize
    Dim leclabIndices As Generic.List(Of Integer)
    Dim hasDis As Boolean
    Dim indexLab As Integer = 0
    Dim indexDis As Integer = 0
    Public Sub New(leclabIndices As Generic.List(Of Integer))
        Me.leclabIndices = leclabIndices
        'If lec, then get dis (may not be necessary)
        'hasDis = Form1.courses(Form1.coursesAdded(leclabIndices(0))).type = "LEC" And Form1.coursesAdded.Count <> leclabIndices(0) + 1 And
        '        Form1.courses(Form1.coursesAdded(leclabIndices(0) + 1)).type = "DIS" 'TODO check if this works
        hasDis = Form1.courses(Form1.coursesAdded(leclabIndices(0))).type = "LEC"
        If Form1.coursesAdded.Count <> leclabIndices(0) + 1 Then
            hasDis = hasDis And Form1.courses(Form1.coursesAdded(leclabIndices(0) + 1)).type = "DIS" 'TODO check if this works
        Else
            hasDis = False
        End If

    End Sub

    Public Function AddCourse(ByRef coursesChosen As Stack(Of Integer)) As Boolean
        If hasDis Then 'Add lec and dis
            'debug
            If indexLab = leclabIndices.Count Then
                'indexLab = indexLab
                Return False
            End If
            If leclabIndices(0) = 0 And indexLab = 1 Then
                indexLab = indexLab
            End If
            Dim dis As Integer = leclabIndices(indexLab) + 1 + indexDis
            If dis = Form1.coursesAdded.Count Then 'last lecture and no more dis
                Return False
            ElseIf Form1.courses(Form1.coursesAdded(dis)).type <> "DIS" Then 'Adding next lecture and first dis
                indexLab += 1
                indexDis = 0
                If indexLab = leclabIndices.Count Then
                    Return False
                Else 'if there is a next lecture in this course
                    If Not isValidSchedule(coursesChosen, leclabIndices(indexLab)) Then
                        indexLab += 1
                        indexDis = 0
                        Return True
                    ElseIf Not isValidSchedule(coursesChosen, leclabIndices(indexLab) + 1 + indexDis) Then
                        indexDis += 1
                        Return True
                    End If
                    coursesChosen.Push(leclabIndices(indexLab))
                    coursesChosen.Push(leclabIndices(indexLab) + 1 + indexDis) 'Assumes that discussion always follows a lecture
                    indexDis += 1
                End If
            Else 'Add next dis
                If Not isValidSchedule(coursesChosen, leclabIndices(indexLab)) Or
                        Not isValidSchedule(coursesChosen, leclabIndices(indexLab) + 1 + indexDis) Then 'Lecture must be valid at this point though (may remove the isValid for lecture)
                    indexDis += 1
                    Return True
                End If
                coursesChosen.Push(leclabIndices(indexLab))
                coursesChosen.Push(leclabIndices(indexLab) + 1 + indexDis) 'Assumes that discussion always follows a lecture
                indexDis += 1
            End If

        Else 'Add single lab
            If indexLab = leclabIndices.Count Then
                Return False
            End If
            If Not isValidSchedule(coursesChosen, leclabIndices(indexLab)) Then
                indexLab += 1
                Return True
            End If
            coursesChosen.Push(leclabIndices(indexLab))
            indexLab += 1
        End If
        Return True
    End Function

    ''alternate method
    'Public Function isValidSchedule(ByRef coursesChosen As Stack(Of Integer), ByVal index As Integer) As Boolean
    '    Return True
    'End Function

    'index is of coursesAdded
    Public Function isValidSchedule(ByRef coursesChosen As Stack(Of Integer), ByVal index As Integer) As Boolean
        Dim course As Course = Form1.courses(Form1.coursesAdded(index))
        Dim tStart As Double = course.timeStart
        Dim tEnd As Double = course.timeEnd
        Dim curStart As Double
        Dim curEnd As Double
        For i As Integer = 0 To coursesChosen.Count - 1
            Dim curCourse As Course = Form1.courses(Form1.coursesAdded(coursesChosen(i)))
            For iDay As Integer = 0 To 4
                If course.days(iDay) And curCourse.days(iDay) Then
                    curStart = curCourse.timeStart
                    curEnd = curCourse.timeEnd
                    If (tStart >= curStart And tStart <= curEnd) Or (tEnd >= curStart And tEnd <= curEnd) Or
                        (curStart >= tStart And curEnd <= tEnd) Or (curEnd >= tStart And curEnd <= tEnd) Then
                        Return False
                    End If
                End If
            Next
        Next
        Return True
    End Function

    Public Sub ResetIndices()
        indexLab = 0
        indexDis = 0
    End Sub
End Class
