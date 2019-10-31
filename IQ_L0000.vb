Public Class IQ_L000

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()
        LCD_Correl = 1
        For Each argument As String In My.Application.CommandLineArgs
            If IsNumeric(Mid(argument, 2, Len(argument) - 1)) Then
                LCD_Correl = CInt(Mid(argument, 2, Len(argument) - 1))
            End If
        Next
        LCD_Output = 0
        Dim Ips_Prov(50) As String
        Dim Indice_Ips As Integer = 0
        Dim strHostName As String = System.Net.Dns.GetHostName()
        Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(strHostName)
        For Each ipheal As System.Net.IPAddress In iphe.AddressList
            If ipheal.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                Ips_Prov(Indice_Ips) = ipheal.ToString()
                Indice_Ips += 1
            End If
        Next
        Indice_Ips = 1
        Disco_Appl = GetSetting("I-Queue", "Appl", "Disco", "D")
        If Disco_Appl Is Nothing Then
            Disco_Appl = "C"
        End If
        Server_Name = GetSetting("I-Queue", "Appl", "ServerName", "")
        Server_Ip = GetSetting("I-Queue", "Appl", "ServerIp", "")
        Server_User = GetSetting("I-Queue", "Appl", "ServerUser", "")
        Server_Pwd = GetSetting("I-Queue", "Appl", "ServerPwd", "")
        Server_Collation = GetSetting("I-Queue", "Appl", "ServerCollation", "ymd")
        If Server_Name = "" Or Server_Ip = "" Or Server_User = "" Then
            MessageBox.Show("EQUIPO NO CONFIGURADO PARA I-Q", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If
        Computer_Ofic = Nothing
        LCD_Code = Nothing
        Computer_Ip = ""
        For Indice_Ips = 0 To 49
            If Ips_Prov(Indice_Ips) = Nothing Then
                Exit For
            End If
            Cnn_Central_Server = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=" & Server_User & ";Password=" & Server_Pwd & ";Data Source=" & Server_Name & ";Initial Catalog=IQData;Use Procedure for Prepare=1;Auto Translate=True;Packet Size=4096;Workstation ID=RIESGO2;Use Encryption for Data=False;Tag with column collation when possible=False"
            Dim instruccion As String
            instruccion = "Select IQConfigOfic_Codigo, IQConfigOfic_Oficina, IQConfigOfic_Output from IQ_ConfigOfic where IQConfigOfic_Ip = '" & Ips_Prov(Indice_Ips) & "(" & Trim(CStr(LCD_Correl)) & ")' And IQConfigOfic_Tipo = 'L'"
            Dim Carga_Coneccion_M2b As New OleDb.OleDbConnection(Cnn_Central_Server)
            Carga_Coneccion_M2b.Open()
            Dim Carga_Comando_M2b As New OleDb.OleDbCommand(instruccion, Carga_Coneccion_M2b)
            Dim Carga_Reader_M2b As OleDb.OleDbDataReader = Carga_Comando_M2b.ExecuteReader(CommandBehavior.CloseConnection)
            If Carga_Reader_M2b.HasRows = True Then
                While Carga_Reader_M2b.Read
                    If IsDBNull(Carga_Reader_M2b.GetValue(0)) = False Then
                        Computer_Ip = Ips_Prov(Indice_Ips)
                        LCD_Code = Carga_Reader_M2b.GetValue(0)
                        Computer_Ofic = Carga_Reader_M2b.GetValue(1)
                        LCD_Output = CInt(Carga_Reader_M2b.GetValue(2))
                    End If
                End While
                Carga_Coneccion_M2b.Dispose()
                If Computer_Ip <> "" Then
                    Exit For
                End If
            End If
        Next
        If Computer_Ip = "" Then
            MessageBox.Show("PANTALLA NO CONFIGURADA EN EL SERVIDOR DE I-Q", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If

        Dim screen As Screen = screen.AllScreens(LCD_Output)
        Dim screen0 As Screen = screen.AllScreens(0)
        Dim nuevo_y As Integer = (screen0.Bounds.Y - screen.Bounds.Y) * -1
        Dim doc As IQ_L0001 = New IQ_L0001
        doc.StartPosition = FormStartPosition.Manual
        doc.Location = New Point(Screen.Bounds.X, nuevo_y)
        doc.Height = Screen.AllScreens(LCD_Output).WorkingArea.Height
        doc.Width = Screen.AllScreens(LCD_Output).WorkingArea.Width
        doc.WindowState = FormWindowState.Normal
        doc.ShowDialog()
    End Sub
End Class