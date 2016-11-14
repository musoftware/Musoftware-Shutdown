Imports Microsoft.Win32
Imports System.IO

Public Class Frmmain


    Private IsFormBeingDragged As Boolean = False
    Private MouseDownX As Integer
    Private MouseDownY As Integer

    Private Sub Frmmain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        hkr.Unregister(0)
    End Sub

    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseDown, ToolStrip1.MouseDown

        If e.Button = MouseButtons.Left Then
            IsFormBeingDragged = True
            MouseDownX = e.X
            MouseDownY = e.Y
        End If
    End Sub

    Private Sub Form1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseUp, ToolStrip1.MouseUp

        If e.Button = MouseButtons.Left Then
            IsFormBeingDragged = False
        End If
    End Sub

    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseMove, ToolStrip1.MouseMove

        If IsFormBeingDragged Then
            Dim temp As Point = New Point()

            temp.X = Me.Location.X + (e.X - MouseDownX)
            temp.Y = Me.Location.Y + (e.Y - MouseDownY)
            Me.Location = temp
            temp = Nothing
        End If
    End Sub



    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim Clock() As String '= Split(My.Settings.DateString, ":")

        Do
            Clock = Split(My.Settings.DateString, ":")
            If (Hour(Now) = 14) And (Minute(Now) = 49) And (Second(Now) = 5) And (My.Settings.DateString = "") Then
                System.Diagnostics.Process.Start("ShutDown", "-s -f")
            ElseIf My.Settings.DateString <> "" Then
                If (Hour(Now) = CInt(Clock(0))) And (Minute(Now) = CInt(Clock(1))) And (Second(Now) = CInt(Clock(2))) Then
                    System.Diagnostics.Process.Start("ShutDown", "-s -f")
                End If
            End If
            '    BackgroundWorker1.ReportProgress(0, DateAndTime.TimeString)
            Threading.Thread.Sleep(700)
        Loop
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If MessageBox.Show("R U Sure to make Restart", "Sure Message", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then

            System.Diagnostics.Process.Start("ShutDown", "-r -f")

        End If
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If MessageBox.Show("R U Sure to make Shutdown", "Sure Message", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then

            System.Diagnostics.Process.Start("ShutDown", "-s -f")

        End If
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Me.Visible = False
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Label1.Text = e.UserState
    End Sub
    Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
    Private Sub Frmmain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MaskedTextBox1.Text = My.Settings.DateString
        hkr.Register(HotKeyRegistryClass.Modifiers.MOD_ALT, Keys.N).ToString()


        BackgroundWorker1.RunWorkerAsync()
        'NotifyIcon1.

        Dim MyKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\run", True)
        Dim filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\MSD\MSD.exe"
        Dim Dirname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\MSD"
        Dim PDirname = Path.GetDirectoryName(Application.ExecutablePath)

        Try
            If filename <> Application.ExecutablePath Then
                If Directory.Exists(Dirname) = False Then

                    Directory.CreateDirectory(Dirname)
                    File.Copy(Application.ExecutablePath, filename)

                    MyKey.SetValue("MSD", filename + " /hide")
                Else
                    For Each I As Process In Process.GetProcessesByName("MSD") : I.Kill() : Next
                    File.Delete(filename)
                    File.Copy(Application.ExecutablePath, filename)

                End If
            End If
        Catch ex As Exception
            Try
                MyKey.SetValue("MSD", Application.ExecutablePath + " /hide")
            Catch ex2 As Exception
                MessageBox.Show(ex.Message & vbCrLf & ex2.Message)
            End Try
        End Try





    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowToolStripMenuItem.Click
        Me.Visible = True
    End Sub

    Private Sub Frmmain_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        If Not CommandLineArgs Is Nothing Then
            For i As Integer = 0 To CommandLineArgs.Count - 1
                If (CommandLineArgs(i) = "/hide") Then
                    Me.Visible = False
                    Me.Hide()
                End If
            Next
        End If

        CommandLineArgs = Nothing

    End Sub
    Dim HH, MM, SS As Integer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Clock() As String = Split(MaskedTextBox1.Text, ":")
        Try
            If CInt(Clock(0)) <= 23 Then
                If CInt(Clock(1)) <= 59 Then
                    If CInt(Clock(3)) <= 59 Then
                        My.Settings.DateString = MaskedTextBox1.Text
                        My.Settings.Save()
                    Else
                        Throw New ArgumentException("Check Date as 14:45:59")
                    End If
                Else
                    Throw New ArgumentException("Check Date as 14:59:05")
                End If
            Else
                Throw New ArgumentException("Check Date as 23:45:05")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Dim hkr As New HotKeyRegistryClass(Me.Handle)

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = HotKeyRegistryClass.Messages.WM_HOTKEY Then
            Dim ID As String = m.WParam.ToString()
            Select Case ID
                Case 0
                    Me.Show()
                    Me.Visible = True
            End Select
        End If
        MyBase.WndProc(m)
    End Sub

End Class
