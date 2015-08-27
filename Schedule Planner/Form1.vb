Imports System.Xml
'Imports Microsoft.Office.Interop.Excel
Imports System.Net

Public Class Form1
    Dim coursesDisplay As Generic.List(Of Integer) = New Generic.List(Of Integer)
    Public coursesAdded As Generic.List(Of Integer) = New Generic.List(Of Integer)
    Public courses As Generic.List(Of Course) = New Generic.List(Of Course)
    Dim brushArray() As Brush = {Brushes.AliceBlue, Brushes.Bisque, Brushes.BlanchedAlmond, Brushes.IndianRed}

    Dim removedIndices As Generic.List(Of Integer) = New Generic.List(Of Integer)
    Dim addedIndices As Generic.List(Of Integer) = New Generic.List(Of Integer)

    Dim lecturelabIndices As Generic.List(Of Integer) = New Generic.List(Of Integer) 'of coursesAdded

    Dim schedules As Generic.List(Of Generic.List(Of Integer)) = New Generic.List(Of Generic.List(Of Integer)) 'of coursesAdded

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint
        Const timeTextEndX As Integer = 25
        Const daysTextEndY As Integer = 20
        Dim height As Integer = Convert.ToInt32((Panel1.Size.Height - daysTextEndY) / 12)
        Dim widthPanel As Integer = Panel1.Size.Width
        Dim width As Integer = (widthPanel - timeTextEndX) / 5

        'Adding days text
        Dim day As String = ""
        For i As Integer = 0 To 4
            Select Case i
                Case 0
                    day = "Monday"
                Case 1
                    day = "Tuesday"
                Case 2
                    day = "Wednesday"
                Case 3
                    day = "Thursday"
                Case 4
                    day = "Friday"
            End Select
            Dim xStart = timeTextEndX + Convert.ToInt32((widthPanel - timeTextEndX) / 5) * (i)
            e.Graphics.DrawString(day, New System.Drawing.Font("Arial", 12), Brushes.Black, xStart, 0)
        Next
        'Horizontal line for days
        e.Graphics.DrawLine(Pens.Black, 0, daysTextEndY, widthPanel, daysTextEndY)

        'Drawing vertical lines for days
        For i As Integer = 0 To 3
            Dim xStart = timeTextEndX + Convert.ToInt32((widthPanel - timeTextEndX) / 5) * (i + 1)
            e.Graphics.DrawLine(Pens.Black, xStart, 0, xStart, Panel1.Size.Height)
        Next

        'Drawing time text and lines
        For i As Integer = 1 To 12
            Dim time As Integer = (8 + i - 1) Mod 12
            If time = 0 Then
                time = 12
            End If
            'text
            e.Graphics.DrawString(Convert.ToString(time), New System.Drawing.Font("Arial", 12), Brushes.Black, 0, height * (i - 1) + daysTextEndY)

            'line
            If i <> 12 Then
                e.Graphics.DrawLine(Pens.Black, 0, height * i + daysTextEndY, widthPanel, height * i + daysTextEndY)
            End If
        Next
        'Vertical line for time text
        e.Graphics.DrawLine(Pens.Black, timeTextEndX, 0, timeTextEndX, Panel1.Size.Height)

        'Drawing courses
        Dim brushI As Integer = 0
        For Each cI As Integer In coursesDisplay
            Dim c As Course = courses(cI)
            For i As Integer = 0 To c.days.Length - 1
                If c.days(i) Then
                    e.Graphics.FillRectangle(brushArray(brushI), timeTextEndX + width * i + 2, Convert.ToInt32((c.timeStart - 8) * height) + daysTextEndY + 2, width - 4, Convert.ToInt32((c.timeEnd - c.timeStart) * height) - 4)
                    e.Graphics.DrawString(c.name & vbNewLine & c.type & " " & c.section & vbNewLine & c.location, New System.Drawing.Font("Arial", 8), Brushes.Black, timeTextEndX + width * i, (c.timeStart - 8) * height + daysTextEndY)
                End If
            Next

            If c.type <> "DIS" Then
                brushI += 1
            End If
        Next


        '' Draw a 200 by 150 pixel green rectangle.
        'e.Graphics.DrawRectangle(Pens.Green, 10, 10, 200, 150)
        '' Draw a blue square
        'e.Graphics.DrawRectangle(Pens.Blue, 30, 30, 150, 150)
        '' Draw a 150 pixel diameter red circle.
        'e.Graphics.DrawEllipse(Pens.Red, 0, 0, 150, 150)
        '' Draw a 250 by 125 pixel yellow oval.
        'e.Graphics.DrawEllipse(Pens.Yellow, 20, 20, 250, 125)

        '' Fill the circle with the same color as its border.
        'e.Graphics.FillEllipse(Brushes.Red, 0, 0, 150, 150)
        '' Fill the square with a different color.
        'e.Graphics.FillRectangle(Brushes.Aquamarine, 31, 31, 149, 149)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        'testing
        courses.Add(New Course("test", "professor", "L", "1", "MWF", 18, 20, "Kinsey"))
        courses.Add(New Course("test2", "professor2", "D", "2", "TWH", 8, 10, "PAB"))

        TextBox1.Text = "http://www.registrar.ucla.edu/schedule/detselect.aspx?termsel=15F&subareasel=STATS&idxcrs=0100A+++"
        Button1_Click(Nothing, Nothing)
        TextBox1.Text = "http://www.registrar.ucla.edu/schedule/detselect.aspx?termsel=15F&subareasel=PHYSICS&idxcrs=0004BL+++"
        Button1_Click(Nothing, Nothing)
        TextBox1.Text = "http://www.registrar.ucla.edu/schedule/detselect.aspx?termsel=15F&subareasel=ENGCOMP&idxcrs=0003++++"
        Button1_Click(Nothing, Nothing)
        TextBox1.Text = "http://www.registrar.ucla.edu/schedule/detselect.aspx?termsel=15F&subareasel=PHYSICS&idxcrs=0001C+++"
        Button1_Click(Nothing, Nothing)

        'coursesDisplay.Add(2)
        'coursesDisplay.Add(3)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sourceCode As String = DownloadString(TextBox1.Text)
        Dim htmldoc As New HtmlAgilityPack.HtmlDocument
        htmldoc.LoadHtml(sourceCode)
        Dim nameNode As HtmlAgilityPack.HtmlNode = htmldoc.DocumentNode.SelectSingleNode("//span[starts-with(@id, 'ctl00_BodyContentPlaceHolder_detselect_dgdCourseHeader')]")

        'get name
        Dim name As String = ""
        If Not nameNode Is Nothing Then
            name = nameNode.InnerText.Substring(0, 14)

            'TODO Check if class already added

            ComboBox1.Items.Add(name)
        End If

        'get courses and their attributes
        Dim nodes As HtmlAgilityPack.HtmlNodeCollection
        Dim startI As Integer = courses.Count
        For index As Integer = 0 To 4
            Select Case index
                Case 0
                    nodes = htmldoc.DocumentNode.SelectNodes("//td[@class='dgdClassDataActType']")
                Case 1
                    nodes = htmldoc.DocumentNode.SelectNodes("//td[@class='dgdClassDataSectionNumber']")
                Case 2
                    nodes = htmldoc.DocumentNode.SelectNodes("//td[@class='dgdClassDataDays']")
                Case 3
                    nodes = htmldoc.DocumentNode.SelectNodes("//td[@class='dgdClassDataTimeStart']")
                Case 4
                    nodes = htmldoc.DocumentNode.SelectNodes("//td[@class='dgdClassDataTimeEnd']")
                Case Else
                    nodes = Nothing
            End Select
            Dim i As Integer = startI
            If Not nodes Is Nothing Then
                For Each node As HtmlAgilityPack.HtmlNode In nodes
                    Select Case index
                        Case 0
                            courses.Add(New Course())
                            courses(i).type = node.InnerText.Trim
                            courses(i).name = name
                        Case 1
                            courses(i).section = node.InnerText.Trim
                        Case 2
                            courses(i).SetDays(node.InnerText.Trim)
                        Case 3
                            If node.InnerText.Trim = "" Then
                                RichTextBox1.AppendText("removed: " & courses(i).name & " " & courses(i).type & courses(i).section)
                                courses.RemoveAt(i)
                                Continue For
                            End If
                            courses(i).timeStart = Course.ConvertTime(node.InnerText.Trim)
                        Case 4
                            If node.InnerText.Trim = "" Then
                                'RichTextBox1.AppendText("removed: " & courses(i).ToString())
                                'courses.RemoveAt(i)
                                Continue For
                            End If
                            courses(i).timeEnd = Course.ConvertTime(node.InnerText.Trim)

                    End Select
                    i += 1
                    'TextBox2.Text = node.InnerText.Trim
                Next
            End If

        Next

        For i As Integer = startI To courses.Count - 1
            coursesAdded.Add(i)
        Next

        'testing
        'coursesDisplay.Add(2)
        'coursesDisplay.Add(3)
        'Panel1.Refresh()
        'Dim secNodes As HtmlAgilityPack.HtmlNodeCollection = htmldoc.DocumentNode.SelectNodes("//td[@class='dgdClassDataSectionNumber']")
        'i = startI
        'If Not secNodes Is Nothing Then
        '    For Each secNode As HtmlAgilityPack.HtmlNode In secNodes
        '        courses(i).type = secNode.InnerText.Trim
        '        i += 1
        '        TextBox2.Text = secNode.InnerText.Trim
        '    Next
        'End If

        TextBox1.Text = ""
    End Sub

    Private Function DownloadString(ByVal str) As String
        Dim sourceCode As String
        Try
            sourceCode = New System.Net.WebClient().DownloadString(str)
        Catch e As WebException
            sourceCode = New System.Net.WebClient().DownloadString(str)
        End Try
        Return sourceCode
    End Function

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        addedIndices.Clear()
        removedIndices.Clear()
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()
        For i As Integer = 0 To courses.Count - 1
            If courses(i).name = ComboBox1.Text Then
                If coursesAdded.Contains(i) Then
                    ListBox2.Items.Add(courses(i).type & " " & courses(i).section)
                    addedIndices.Add(i)
                Else
                    ListBox1.Items.Add(courses(i).type & " " & courses(i).section)
                    removedIndices.Add(i)
                End If
            End If
        Next
        'ListBox1.DataSource = removed
        'ListBox2.DataSource = added
    End Sub

    'TODO extra: to fix course added going to the end of list, you can clear the list and repopulate the list from scratch like in ComboBOx1_SelectedIndexChanged()
    'TODO [This must be fixed for optimize to work] coursesAdded will not contain the indices in ascending order still although the listbox will; so a better method may be thought of
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        While ListBox2.SelectedIndices.Count > 0
            Dim i As Integer = ListBox2.SelectedIndices(0)
            coursesAdded.Remove(addedIndices(i))
            removedIndices.Add(addedIndices(i))
            addedIndices.RemoveAt(i)
            ListBox1.Items.Add(ListBox2.Items(i))
            ListBox2.Items.RemoveAt(i)
        End While
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        For Each i As Integer In ListBox1.SelectedIndices
            coursesAdded.Add(removedIndices(i))
            addedIndices.Add(removedIndices(i))
            removedIndices.RemoveAt(i)
            ListBox2.Items.Add(ListBox1.Items(i))
            ListBox1.Items.RemoveAt(i)
        Next
    End Sub

    Private Sub Optimize()
        'Calculate max courses and lecturelabIndices

        lecturelabIndices.Clear()
        schedules.Clear()
        ComboBox2.Items.Clear()

        For i As Integer = 0 To coursesAdded.Count - 1
            If courses(coursesAdded(i)).type <> "DIS" Then
                lecturelabIndices.Add(i)
            End If
        Next
        

        'lecturelabIndices organized by course
        Dim courseLecturesLabs As Generic.List(Of Generic.List(Of Integer)) = New Generic.List(Of Generic.List(Of Integer))
        courseLecturesLabs.Add(New Generic.List(Of Integer)) 'TODO check if this code is necessary
        Dim iCourse As Integer = 0
        courseLecturesLabs(0).Add(lecturelabIndices(0))
        For i As Integer = 1 To lecturelabIndices.Count - 1
            If courses(coursesAdded(lecturelabIndices(i))).name = courses(coursesAdded(lecturelabIndices(i - 1))).name Then
                courseLecturesLabs(iCourse).Add(lecturelabIndices(i))
            Else
                courseLecturesLabs.Add(New Generic.List(Of Integer))
                iCourse += 1
                courseLecturesLabs(iCourse).Add(lecturelabIndices(i))
            End If
        Next

        Dim maxCourses As Integer = courseLecturesLabs.Count
        'For i As Integer = 0 To courseLecturesLabs.Count - 1
        '    If courses(coursesAdded(courseLecturesLabs(i)(0))).type = "LEC" And coursesAdded.Count <> courseLecturesLabs(i)(0) + 1 And
        '        courses(coursesAdded(courseLecturesLabs(i)(0) + 1)).type = "DIS" Then
        '        maxCourses += 1
        '    End If
        'Next

        Dim coursesO(courseLecturesLabs.Count - 1) As CourseOptimize
        For i As Integer = 0 To courseLecturesLabs.Count - 1
            coursesO(i) = New CourseOptimize(courseLecturesLabs(i))
        Next

        Dim coursesChosen As Stack(Of Integer) = New Stack(Of Integer) 'contains indices of coursesAdded
        Dim indexCourse As Generic.List(Of Integer) = New Generic.List(Of Integer)
        Dim indexDis As Generic.List(Of Integer) = New Generic.List(Of Integer)

        Dim index As Integer = 0
        Dim counter As Integer = 0
        While (True)
            'debug
            If index = 0 Then
                index = index
            End If
            If coursesChosen.Count = 1 Then
                index = index
            End If
            If counter = 135 Then
                counter = counter
            End If
            If counter = 122 Then
                counter = counter
            End If
            If coursesChosen(0) = 8 And coursesChosen(1) = 6 Then
                counter = counter
            End If
            
            While (index < maxCourses)
                If coursesChosen.Count = 2 And coursesChosen(0) = 3 And coursesChosen(1) = 2 Then
                    counter = counter
                End If
                Dim initialCount = coursesChosen.Count
                If Not coursesO(index).AddCourse(coursesChosen) Then
                    Exit While
                End If
                If initialCount < coursesChosen.Count Then 'If a course was added, then increment the index; otherwise (if there was a schedule conflict and a course wasn't added, do not increment the index)
                    index += 1
                End If
            End While
            If index = maxCourses Then
                'Check if valid schedule before adding
                schedules.Add(coursesChosen.ToList)
                If schedules.Count = 17 Then
                    index = index
                End If
                If schedules.Count Mod 500 = 0 Then
                    index = index
                End If
            End If
            If coursesChosen.Count = 0 Or index = 0 Then
                Exit While
            End If

            'pop [twice if dis]
            If courses(coursesAdded(coursesChosen.Pop())).type = "DIS" Then
                coursesChosen.Pop()
            End If
            index -= 1
            'debug
            If index = 0 And coursesChosen.Count = 1 Then
                index = index
            End If

            'ResetIndices
            If index < maxCourses - 1 Then
                coursesO(index + 1).ResetIndices()
            End If

            'debug
            counter += 1
        End While

        Dim count As Integer = schedules.Count - 1
        If schedules.Count > 20000 Then
            count = 20000
        End If
        For i As Integer = 0 To count
            ComboBox2.Items.Add(i + 1)
        Next
    End Sub

    'Public Function isValidSchedule(ByVal coursesChosen As Stack(Of Integer)) As Boolean
    '    For i As Integer = 
    'End Function


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Optimize()


        ''Calculate max courses and lecturelabIndices

        'lecturelabIndices.Clear()
        'For i As Integer = 0 To coursesAdded.Count
        '    If courses(coursesAdded(i)).type <> "DIS" Then
        '        lecturelabIndices.Add(i)
        '    End If
        'Next
        'Dim maxCourses As Integer = lecturelabIndices.Count
        'For i As Integer = 0 To coursesAdded.Count
        '    If courses(coursesAdded(i)).type = "LEC" Then
        '        maxCourses += 1
        '    End If
        'Next

        'Dim courseLecturesLabs As Generic.List(Of Generic.List(Of Integer)) = New Generic.List(Of Generic.List(Of Integer))
        'courseLecturesLabs.Add(New Generic.List(Of Integer)) 'TODO check if this code is necessary
        'Dim iCourse As Integer = 0
        'courseLecturesLabs(0).Add(lecturelabIndices(0))
        'For i As Integer = 1 To lecturelabIndices.Count
        '    If courses(coursesAdded(lecturelabIndices(i))).name = courses(coursesAdded(lecturelabIndices(i - 1))).name Then
        '        courseLecturesLabs(iCourse).Add(lecturelabIndices(i))
        '    Else
        '        courseLecturesLabs.Add(New Generic.List(Of Integer))
        '        iCourse += 1
        '        courseLecturesLabs(iCourse).Add(lecturelabIndices(i))
        '    End If
        'Next

        ''Dim coursesRemaining As Generic.List(Of Integer) = coursesAdded 'TODO check if this creates a copy by value (what you want) or this copies reference
        'Dim coursesChosen As Stack(Of Integer) = New Stack(Of Integer) 'contains indices of coursesAdded
        ''coursesChosen.Push(0)
        'Dim indexCourse As Generic.List(Of Integer) = New Generic.List(Of Integer)
        'Dim indexDis As Generic.List(Of Integer) = New Generic.List(Of Integer)
        ''Dim iVisited As Integer = 0
        ''Dim iVisitedDis As Integer = 0

        'Dim index As Integer = 0
        'While (True)
        '    While (coursesChosen.Count < maxCourses)
        '        If indexCourse.Count <> coursesChosen.Count + 1 Then
        '            indexCourse.Add(courseLecturesLabs(coursesChosen.Count)(0))
        '        End If
        '        'Choose lec/lab [and dis]
        '        For iLec As Integer = visited(coursesChosen.Count)

        '    End While
        'End While


        ''While (visited(0).Count <)
        ''While (ChooseNext(coursesChosen, visited(iVisited))

        ''    End While


    End Sub

    'Private Function ChooseNextSchedule(ByRef coursesChosen As Stack(Of Integer), ByRef visited As Generic.List(Of Generic.List(Of Integer)))

    '    While (coursesChosen.Count < lecturelabIndices.Count)
    '        If Not ChooseNext(coursesChosen, visited(coursesChosen.Count)) Then
    '        Else

    '        End If
    '    End While
    'End Function


    ''visited is a list of visited nodes chosen in the past and can't be chosen next
    ''Chooses next course from coursesAdded
    'Private Function ChooseNext(ByRef coursesChosen As Stack(Of Integer), ByRef visited As Generic.List(Of Integer)) As Boolean 'Returns true if there was a next course chosen
    '    'Dim visited As Generic.List(Of Integer) = New Generic.List(Of Integer)
    '    'If coursesChosen.Count = 0 Then
    '    '    coursesChosen.Push(0) 'TODO If there isn't a 0th element, then there is an error (maybe button 'optimize' pushed before courses added)
    '    '    visited.Add(0)
    '    '    Return True
    '    'End If

    '    Dim startI As Integer = 0 'index at which to start the search for next course
    '    If coursesChosen.Count <> 0 Then
    '        Dim peek As Integer = coursesChosen.Peek()
    '        Dim currentName As String = courses(coursesAdded(coursesChosen.Peek())).name
    '        Dim currentType As String = courses(coursesAdded(coursesChosen.Peek())).type

    '        If currentType = "DIS" Then 'next course must be a non-DIS
    '            'startI = 'TODO can optimize code here by using lecturelabsindices instead of looping to find next non-DIS
    '            Dim i As Integer = peek + 1
    '            While (i < coursesAdded.Count)
    '                If courses(i).type <> "DIS" And currentName <> courses(i).name Then
    '                    startI = i
    '                    Exit While
    '                End If
    '                i += 1
    '            End While
    '            If i = coursesAdded.Count Then
    '                Return False 'There is no lecture for next class after this discussion
    '            End If
    '        ElseIf currentType = "LEC" Then 'next course can be discussion or lecture (if current is lab)
    '            startI = peek + 1
    '            If startI = coursesAdded.Count Then
    '                Return False 'There is no discussion for this class
    '            End If
    '        ElseIf currentType = "LAB" Then
    '            Dim i As Integer = peek + 1
    '            While (i < coursesAdded.Count)
    '                If currentName <> courses(i).name Then
    '                    startI = i
    '                    Exit While
    '                End If
    '                i += 1
    '            End While
    '        Else
    '            MessageBox.Show("Course of unknown/not-programmed type was found")
    '        End If
    '    End If

    '    'If next course is a lecture/lab
    '    Dim typeOfPreviousCourse As String = courses(coursesAdded(coursesChosen.Peek())).type
    '    If startI = 0 Or typeOfPreviousCourse <> "LEC" Then 'TODO there may be more types than this.  Maybe not only LEC has discussions.  Maybe there is a lecture without discussions
    '        For i As Integer = lecturelabIndices.IndexOf(startI) To lecturelabIndices.Count - 1
    '            Dim index As Integer = lecturelabIndices(i)
    '            If Not visited.Contains(index) Then
    '                coursesChosen.Push(index)
    '                visited.Add(index)
    '                Return True
    '            End If
    '        Next
    '    Else 'next course is DIS
    '        For i As Integer = startI To coursesAdded.Count - 1
    '            If Not visited.Contains(i) Then
    '                coursesChosen.Push(i)
    '                visited.Add(i)
    '                Return True
    '            End If
    '        Next
    '    End If

    '    Return False
    'End Function

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Dim schedule As Generic.List(Of Integer) = schedules(Convert.ToInt32(ComboBox2.Text) - 1)
        coursesDisplay.Clear()
        For i As Integer = 0 To schedule.Count - 1
            coursesDisplay.Add(coursesAdded(schedule(i)))
        Next
        Panel1.Refresh()
    End Sub
End Class
