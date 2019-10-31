Imports System.Data.OleDb
Imports System.IO
Imports WMPLib
Imports System.Threading
Public Class IQ_L0001
    Dim Background As Control
    Dim Icon_Folder As String
    Dim controles(100, 10) As String
    Dim num_controles As Integer
    Dim num_displays As Integer
    Dim displays(100, 5) As String
    Dim tickets_actuales(100, 8) As String
    Dim Tiptick_to_call As String
    Dim Numtick_to_call As String
    Dim Punto_to_call As String
    Dim string_Pending As String
    Dim desfase_segundos As Integer
    Dim Lista_Videos(200) As String
    Dim Lista_Imagenes(200) As String
    Dim num_videos As Integer
    Dim num_imagenes As Integer
    Dim imagen_actual As Integer
    Dim NuevoVideo As New AxWMPLib.AxWindowsMediaPlayer
    Private DsPuntos As New DataSet
    Private DbPuntos As System.Data.OleDb.OleDbDataAdapter = New System.Data.OleDb.OleDbDataAdapter
    Public Sub New()
        ' Llamada necesaria para el diseñador.
        InitializeComponent()
        Me.TimerParp.Enabled = False
        Me.TimerParp.Stop()
        Me.Timer1.Enabled = True
        Me.Timer1.Start()
    End Sub
    Private Sub TimerImage_Tick(sender As Object, e As EventArgs) Handles TimerImage.Tick
        Me.TimerImage.Enabled = False
        Me.TimerImage.Stop()
        imagen_actual += 1
        If imagen_actual >= num_imagenes Then
            imagen_actual = 0
        End If
        For Each control_a_buscar As Control In Background.Controls
            If Mid(control_a_buscar.Name, 1, 6) = "ImgFld" Then
                Dim imglist As PictureBox = control_a_buscar
                Try
                    imglist.Image = System.Drawing.Image.FromFile(Lista_Imagenes(imagen_actual))
                Catch ex As Exception
                    imglist.Image = Nothing
                End Try
                Exit For
            End If
        Next
        Me.TimerImage.Enabled = True
        Me.TimerImage.Start()
    End Sub
    Private Sub Synchronize_Date_Server()
        Dim Central_Cnn As New OleDb.OleDbConnection(Cnn_Central_Server)
        Dim CmmCentral As New OleDb.OleDbCommand("", Central_Cnn)
        CmmCentral.CommandTimeout = 0
        CmmCentral.CommandType = CommandType.StoredProcedure
        CmmCentral.CommandText = "IQ_SpGetServerDate"
        CmmCentral.Parameters.Add("Fecha", OleDbType.Date).Direction = ParameterDirection.Output
        Dim Fecha_Sistema As Date
        Dim Fecha_Maquina As Date
        Try
            Central_Cnn.Open()
            CmmCentral.ExecuteNonQuery()
            Try
                Fecha_Sistema = CmmCentral.Parameters("Fecha").Value
            Catch ex As Exception
                Fecha_Sistema = DateTime.Now
                Exit Try
            End Try
            Fecha_Maquina = DateTime.Now
            desfase_segundos = DateDiff(DateInterval.Second, Fecha_Maquina, Fecha_Sistema)
            Central_Cnn.Close()
        Catch exc As Exception
            Dim Mensaje_Excepcion As String
            Mensaje_Excepcion = exc.Message
            MessageBox.Show("Error Integrado: " + Mensaje_Excepcion, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try
    End Sub
    Private Sub TimerMovil_Tick(sender As Object, e As EventArgs) Handles TimerMovil.Tick
        TimerMovil.Enabled = False
        TimerMovil.Stop()
        For Each control_a_buscar As Control In Background.Controls
            If control_a_buscar.Tag = "Texto Dinámico" Then
                control_a_buscar.Text = Mid(control_a_buscar.Text, 2, Len(control_a_buscar.Text)) & Mid(control_a_buscar.Text, 1, 1)
            End If
        Next
        TimerMovil.Enabled = True
        TimerMovil.Start()
    End Sub
    Private Sub TimerBlink_Tick(sender As Object, e As EventArgs) Handles TimerBlink.Tick
        TimerBlink.Enabled = False
        TimerBlink.Stop()
        For Each control_a_buscar As Control In Background.Controls
            If control_a_buscar.Tag = "Texto Parpadeante" Then
                If TimerBlink.Tag = True Then
                    control_a_buscar.Text = controles(control_a_buscar.TabIndex, 6)
                Else
                    control_a_buscar.Text = ""
                End If
            End If
        Next
        If TimerBlink.Tag = True Then
            TimerBlink.Tag = False
        Else
            TimerBlink.Tag = True
        End If
        TimerBlink.Enabled = True
        TimerBlink.Start()
    End Sub
    Private Sub TimerFecha_Tick(sender As Object, e As EventArgs) Handles TimerFecha.Tick
        TimerFecha.Stop()
        TimerFecha.Enabled = False
        For Each control_a_buscar As Control In Background.Controls
            If Mid(control_a_buscar.Name, 1, 5) = "Fecha" Then
                Try
                    control_a_buscar.Text = Format(DateAdd(DateInterval.Second, desfase_segundos, DateTime.Now), "dd/MM/yyyy - HH:mm:ss")
                Catch ex As Exception
                    control_a_buscar.Text = Format(DateAdd(DateInterval.Second, 0, DateTime.Now), "dd/MM/yyyy - HH:mm:ss")
                    Exit Try
                End Try
                Exit For
            End If
        Next
        TimerFecha.Enabled = True
        TimerFecha.Start()
    End Sub
    Private Sub Graba_Log(Indice As Integer, Texto As String)
        Dim instruccion_insert As String = ""
        instruccion_insert = "Insert Into IQ_LogMM Values('" + Computer_Ofic + "', '" + LCD_Code + "(" + Trim(CStr(LCD_Correl)) + ")', 'L', "
        instruccion_insert = instruccion_insert & "'" & controles(Indice, 1) & "', "
        instruccion_insert = instruccion_insert & "'" & controles(Indice, 0) & "', "
        instruccion_insert = instruccion_insert & "getdate(), "
        instruccion_insert = instruccion_insert & "'" & Texto & "')"
        Try
            Dim IQ_Cnn As New OleDb.OleDbConnection(Cnn_Central_Server)
            IQ_Cnn.Open()
            Dim IQ_Cmm As New OleDb.OleDbCommand(instruccion_insert, IQ_Cnn)
            Dim RegistrosInsertados As Long = IQ_Cmm.ExecuteNonQuery()
            IQ_Cnn.Close()
        Catch exc As Exception
            Dim Mensaje_Excepcion As String
            Mensaje_Excepcion = exc.Message
            MessageBox.Show(Mensaje_Excepcion, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            Exit Sub
        End Try
    End Sub
    Private Sub Buscar_Files(Indice As Integer)
        Dim Alcance_Recfiles(3) As String
        Alcance_Recfiles(0) = ""
        Alcance_Recfiles(1) = "L:" & LCD_Code
        Alcance_Recfiles(2) = ""
        controles(Indice, 6) = ""
        Dim encontrado As Boolean = False
        If Alcance_Recfiles(1) = "" Then GoTo busca_3
        Dim tipo_busq As String = "L"
        Dim cod_busq As String = LCD_Code
        Dim Carga_Coneccion_O2 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_O2.Open()
        Dim Carga_Comando_O2 As New OleDb.OleDbCommand("Select * from IQ_MMConfig where IQMMConfig_Oficina = '' and IQMMConfig_Area = '" & Alcance_Recfiles(1) & "' and IQMMConfig_Punto = '' And IQMMConfig_Tipo = 'L' And IQMMConfig_Nombre = '" & controles(Indice, 0) & "'", Carga_Coneccion_O2)
        Dim Carga_Reader_O2 As OleDb.OleDbDataReader = Carga_Comando_O2.ExecuteReader(CommandBehavior.CloseConnection)
        While Carga_Reader_O2.Read
            If IsDBNull(Carga_Reader_O2.GetValue(0)) = False Then
                encontrado = True
                controles(Indice, 6) = Trim(Carga_Reader_O2.GetValue(5)) & Trim(Carga_Reader_O2.GetValue(6))
            End If
        End While
        Carga_Coneccion_O2.Dispose()
        If encontrado = True Then
            GoTo Busca_Fin
        End If
        If encontrado = False Then
            Dim Carga_Coneccion_O2b As New OleDb.OleDbConnection(Cnn_Central_Server)
            Carga_Coneccion_O2b.Open()
            Dim instruc_busq As String = ""
            Select Case tipo_busq
                Case "A"
                    instruc_busq = "Select IQAreas_Oficina from IQ_Areas where IQAreas_Codigo = '" & cod_busq & "'"
                Case "K"
                    instruc_busq = "Select IQConfigOfic_Oficina from IQ_ConfigOfic where IQConfigOfic_Codigo = '" & cod_busq & "'"
                Case "L"
                    instruc_busq = "Select IQConfigOfic_Oficina from IQ_ConfigOfic where IQConfigOfic_Codigo = '" & cod_busq & "'"
            End Select
            Dim Carga_Comando_O2b As New OleDb.OleDbCommand(instruc_busq, Carga_Coneccion_O2b)
            Dim Carga_Reader_O2b As OleDb.OleDbDataReader = Carga_Comando_O2b.ExecuteReader(CommandBehavior.CloseConnection)
            While Carga_Reader_O2b.Read
                If IsDBNull(Carga_Reader_O2b.GetValue(0)) = False Then
                    Alcance_Recfiles(1) = ""
                    Alcance_Recfiles(0) = Carga_Reader_O2b.GetValue(0)
                End If
            End While
            Carga_Coneccion_O2b.Dispose()
        End If
Busca_3:
        If Alcance_Recfiles(0) = "999999" Then GoTo busca_4
        Dim consolidadora As String = ""
        Dim Carga_Coneccion_O3 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_O3.Open()
        Dim Carga_Comando_O3 As New OleDb.OleDbCommand("Select IQ_MMConfig.*, Iq_Oficinas.IQOficinas_Descripcion,  Iq_Oficinas.IQOficinas_Consolidacion from IQ_MMConfig join IQ_Oficinas on IQMMConfig_Oficina = IQOficinas_Codigo where IQMMConfig_Oficina = '" & Alcance_Recfiles(0) & "' and IQMMConfig_Area = '' and IQMMConfig_Punto = '' And IQMMConfig_Tipo = 'L'  And IQMMConfig_Nombre = '" & controles(Indice, 0) & "'", Carga_Coneccion_O3)
        Dim Carga_Reader_O3 As OleDb.OleDbDataReader = Carga_Comando_O3.ExecuteReader(CommandBehavior.CloseConnection)
        While Carga_Reader_O3.Read
            If IsDBNull(Carga_Reader_O3.GetValue(0)) = False Then
                encontrado = True
                controles(Indice, 6) = Trim(Carga_Reader_O3.GetValue(5)) & Trim(Carga_Reader_O3.GetValue(6))
            End If
        End While
        Carga_Coneccion_O3.Dispose()
        If encontrado = True Then
            GoTo Busca_Fin
        End If
        If encontrado = False Then
            Dim Carga_Coneccion_O3b As New OleDb.OleDbConnection(Cnn_Central_Server)
            Carga_Coneccion_O3b.Open()
            Dim Carga_Comando_O3b As New OleDb.OleDbCommand("Select IQOficinas_Consolidacion from IQ_Oficinas where IQOficinas_Codigo = '" & Alcance_Recfiles(0) & "'", Carga_Coneccion_O3b)
            Dim Carga_Reader_O3b As OleDb.OleDbDataReader = Carga_Comando_O3b.ExecuteReader(CommandBehavior.CloseConnection)
            While Carga_Reader_O3b.Read
                If IsDBNull(Carga_Reader_O3b.GetValue(0)) = False Then
                    consolidadora = Carga_Reader_O3b.GetValue(0)
                End If
            End While
            Carga_Coneccion_O3.Dispose()
            Alcance_Recfiles(0) = consolidadora
            GoTo busca_3
        End If
Busca_4:
        Dim Carga_Coneccion_O4 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_O4.Open()
        Dim Carga_Comando_O4 As New OleDb.OleDbCommand("Select * from IQ_MMConfig where IQMMConfig_Oficina = '999999' and IQMMConfig_Area = '' and IQMMConfig_Punto = '' And IQMMConfig_Tipo = 'L' And IQMMConfig_Nombre = '" & controles(Indice, 0) & "'", Carga_Coneccion_O4)
        Dim Carga_Reader_O4 As OleDb.OleDbDataReader = Carga_Comando_O4.ExecuteReader(CommandBehavior.CloseConnection)
        While Carga_Reader_O4.Read
            If IsDBNull(Carga_Reader_O4.GetValue(0)) = False Then
                encontrado = True
                controles(Indice, 6) = Trim(Carga_Reader_O4.GetValue(5)) & Trim(Carga_Reader_O4.GetValue(6))
            End If
        End While
        Carga_Coneccion_O4.Dispose()
        If encontrado = True Then
            GoTo Busca_Fin
        End If
Busca_Fin:
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        Timer1.Enabled = False
        TimerSearch.Enabled = False
        TimerSearch.Stop()
        TimerBlink.Enabled = False
        TimerBlink.Stop()
        TimerTicket.Enabled = False
        TimerTicket.Stop()
        TimerMovil.Enabled = False
        TimerMovil.Stop()
        TimerImage.Enabled = False
        TimerImage.Stop()
        string_Pending = ""
        num_displays = 0
        Me.PanelDesign.Visible = True
        Icon_Folder = Disco_Appl & ":\I-Q\Iconos\"
        Dim instruccion As String
        instruccion = "Select IQConfigOfic_Codigo, IQConfigOfic_Oficina, IQConfigOfic_Areas from IQ_ConfigOfic where IQConfigOfic_Ip = '" & Computer_Ip & "(" & Trim(CStr(LCD_Correl)) & ")' And IQConfigOfic_Tipo = 'L'"
        Dim Carga_Coneccion_M2b As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_M2b.Open()
        Dim Carga_Comando_M2b As New OleDb.OleDbCommand(instruccion, Carga_Coneccion_M2b)
        Dim Carga_Reader_M2b As OleDb.OleDbDataReader = Carga_Comando_M2b.ExecuteReader(CommandBehavior.CloseConnection)
        If Carga_Reader_M2b.HasRows = True Then
            While Carga_Reader_M2b.Read
                If IsDBNull(Carga_Reader_M2b.GetValue(0)) = False Then
                    Dim aux_areas As String = Carga_Reader_M2b.GetValue(2)
                    Dim pos_areas As Integer
                    Do While aux_areas <> ""
                        pos_areas = InStr(aux_areas, "|")
                        If pos_areas > 1 Then
                            If string_Pending = "" Then
                                string_Pending = "IQ_Pending.IQPending_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "'"
                            Else
                                string_Pending = string_Pending & " or IQ_Pending.IQPending_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "'"
                            End If
                            aux_areas = Mid(aux_areas, pos_areas + 1, Len(aux_areas) - pos_areas)
                        ElseIf pos_areas > 0 Then
                            aux_areas = Mid(aux_areas, 2, Len(aux_areas) - 1)
                        Else
                            If string_Pending = "" Then
                                string_Pending = "IQ_Pending.IQPending_Area = '" & aux_areas & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & aux_areas & "'"
                            Else
                                string_Pending = string_Pending & " or IQ_Pending.IQPending_Area = '" & aux_areas & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & aux_areas & "'"
                            End If
                            aux_areas = ""
                        End If
                        'If pos_areas > 1 Then
                        'If string_Pending = "" Then
                        'string_Pending = "IQ_Tickets.IQTicket_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "'"
                        'Else
                        'string_Pending = string_Pending & " or IQ_Tickets.IQTicket_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & Mid(aux_areas, 1, pos_areas - 1) & "'"
                        'End If
                        'aux_areas = Mid(aux_areas, pos_areas + 1, Len(aux_areas) - pos_areas)
                        'ElseIf pos_areas > 0 Then
                        'aux_areas = Mid(aux_areas, 2, Len(aux_areas) - 1)
                        'Else
                        'If string_Pending = "" Then
                        'string_Pending = "IQ_Tickets.IQTicket_Area = '" & aux_areas & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & aux_areas & "'"
                        'Else
                        'string_Pending = string_Pending & " or IQ_Tickets.IQTicket_Area = '" & aux_areas & "' or IQ_PuntosAtencion.IQPuntos_Area = '" & aux_areas & "'"
                        'End If
                        'aux_areas = ""
                        'End If
                    Loop
                End If
            End While
            Carga_Coneccion_M2b.Dispose()
        End If
        Synchronize_Date_Server()
        Dim cn As System.Data.OleDb.OleDbConnection = New System.Data.OleDb.OleDbConnection(Cnn_Central_Server)
        cn.Open()
        DsPuntos.Clear()
        With DbPuntos
            Dim SQLStr As String = "Select  Iq_PuntosAtencion.* from Iq_PuntosAtencion join Iq_Areas on IQ_PuntosAtencion.IQPuntos_Area = IQ_Areas.IQAreas_Codigo where IQ_Areas.IQAreas_Oficina = '" & Computer_Ofic & "'"
            .TableMappings.Add("Table", "Iq_PuntosAtencion")
            Dim cmd As System.Data.OleDb.OleDbCommand = New System.Data.OleDb.OleDbCommand(SQLStr, cn)
            cmd.CommandType = CommandType.Text
            .SelectCommand = cmd
            .Fill(DsPuntos)
            .Dispose()
            cmd.Cancel()
        End With
        cn.Close()
        Dim Alcance_recup(3) As String
        Alcance_recup(0) = ""
        Alcance_recup(1) = "L:" & LCD_Code
        Alcance_recup(2) = ""
        Dim counter As Integer
        For counter = 0 To 99
            tickets_actuales(counter, 0) = ""
            tickets_actuales(counter, 1) = ""
            tickets_actuales(counter, 2) = ""
            tickets_actuales(counter, 3) = ""
            tickets_actuales(counter, 4) = ""
            tickets_actuales(counter, 5) = ""
            tickets_actuales(counter, 6) = ""
            tickets_actuales(counter, 7) = ""
            controles(counter, 0) = Nothing
            controles(counter, 1) = Nothing
            controles(counter, 2) = Nothing
            controles(counter, 3) = Nothing
            controles(counter, 4) = Nothing
            controles(counter, 5) = Nothing
            controles(counter, 6) = Nothing
            controles(counter, 7) = Nothing
            controles(counter, 8) = Nothing
            controles(counter, 9) = Nothing
        Next
        Me.LblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString
        Dim version_ok As Boolean = False
        Do Until version_ok = True
            For indice_version = Len(Me.LblVersion.Text) To 1 Step -1
                If Mid(Me.LblVersion.Text, indice_version, 1) = "." Then
                    Me.LblVersion.Text = Mid(Me.LblVersion.Text, 1, Len(Me.LblVersion.Text) - 1)
                    version_ok = True
                    Exit For
                Else
                    Me.LblVersion.Text = Mid(Me.LblVersion.Text, 1, Len(Me.LblVersion.Text) - 1)
                End If
            Next
        Loop
        PanelDesign.Controls.Clear()
        Dim encontrado As Boolean = False
        num_controles = 0
        Dim tipo_busq As String = "L"
        Dim cod_busq As String = LCD_Code
        Dim Carga_Coneccion_O2 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_O2.Open()
        Dim Carga_Comando_O2 As New OleDb.OleDbCommand("Select * from IQ_Design where IQDesign_Oficina = '' and IQDesign_Area = '" & Alcance_recup(1) & "' and IQDEsign_Punto = '' And IQDesign_Tipo = 'L' Order by IQDesign_Orden", Carga_Coneccion_O2)
        Dim Carga_Reader_O2 As OleDb.OleDbDataReader = Carga_Comando_O2.ExecuteReader(CommandBehavior.CloseConnection)
        While Carga_Reader_O2.Read
            If IsDBNull(Carga_Reader_O2.GetValue(0)) = False Then
                encontrado = True
                controles(num_controles, 0) = Carga_Reader_O2.GetValue(5)
                controles(num_controles, 1) = Carga_Reader_O2.GetValue(4)
                controles(num_controles, 2) = CStr(Carga_Reader_O2.GetValue(6))
                controles(num_controles, 3) = CStr(Carga_Reader_O2.GetValue(7))
                controles(num_controles, 4) = CStr(Carga_Reader_O2.GetValue(8))
                controles(num_controles, 5) = CStr(Carga_Reader_O2.GetValue(9))
                num_controles += 1
            End If
        End While
        Carga_Coneccion_O2.Dispose()
        If encontrado = True Then
            GoTo Busca_Fin
        End If
        If encontrado = False Then
            Dim Carga_Coneccion_O2b As New OleDb.OleDbConnection(Cnn_Central_Server)
            Carga_Coneccion_O2b.Open()
            Dim instruc_busq As String = ""
            Select Case tipo_busq
                Case "A"
                    instruc_busq = "Select IQAreas_Oficina from IQ_Areas where IQAreas_Codigo = '" & cod_busq & "'"
                Case "K"
                    instruc_busq = "Select IQConfigOfic_Oficina from IQ_ConfigOfic where IQConfigOfic_Codigo = '" & cod_busq & "'"
                Case "L"
                    instruc_busq = "Select IQConfigOfic_Oficina from IQ_ConfigOfic where IQConfigOfic_Codigo = '" & cod_busq & "'"
            End Select
            Dim Carga_Comando_O2b As New OleDb.OleDbCommand(instruc_busq, Carga_Coneccion_O2b)
            Dim Carga_Reader_O2b As OleDb.OleDbDataReader = Carga_Comando_O2b.ExecuteReader(CommandBehavior.CloseConnection)
            While Carga_Reader_O2b.Read
                If IsDBNull(Carga_Reader_O2b.GetValue(0)) = False Then
                    Alcance_recup(1) = ""
                    Alcance_recup(0) = Carga_Reader_O2b.GetValue(0)
                End If
            End While
            Carga_Coneccion_O2b.Dispose()
        End If
Busca_3:
        If Alcance_recup(0) = "999999" Then GoTo busca_4
        Dim consolidadora As String = ""
        Dim Carga_Coneccion_O3 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_O3.Open()
        Dim Carga_Comando_O3 As New OleDb.OleDbCommand("Select IQ_Design.*, Iq_Oficinas.IQOficinas_Descripcion,  Iq_Oficinas.IQOficinas_Consolidacion from IQ_Design join IQ_Oficinas on IQDesign_Oficina = IQOficinas_Codigo where IQDesign_Oficina = '" & Alcance_recup(0) & "' and IQDesign_Area = '' and IQDEsign_Punto = '' And IQDesign_Tipo = 'L' Order by IQDesign_Orden", Carga_Coneccion_O3)
        Dim Carga_Reader_O3 As OleDb.OleDbDataReader = Carga_Comando_O3.ExecuteReader(CommandBehavior.CloseConnection)
        While Carga_Reader_O3.Read
            If IsDBNull(Carga_Reader_O3.GetValue(0)) = False Then
                encontrado = True
                controles(num_controles, 0) = Carga_Reader_O3.GetValue(5)
                controles(num_controles, 1) = Carga_Reader_O3.GetValue(4)
                controles(num_controles, 2) = CStr(Carga_Reader_O3.GetValue(6))
                controles(num_controles, 3) = CStr(Carga_Reader_O3.GetValue(7))
                controles(num_controles, 4) = CStr(Carga_Reader_O3.GetValue(8))
                controles(num_controles, 5) = CStr(Carga_Reader_O3.GetValue(9))
                num_controles += 1
            End If
        End While
        Carga_Coneccion_O3.Dispose()
        If encontrado = True Then
            GoTo Busca_Fin
        End If
        If encontrado = False Then
            Dim Carga_Coneccion_O3b As New OleDb.OleDbConnection(Cnn_Central_Server)
            Carga_Coneccion_O3b.Open()
            Dim Carga_Comando_O3b As New OleDb.OleDbCommand("Select IQOficinas_Consolidacion from IQ_Oficinas where IQOficinas_Codigo = '" & Alcance_recup(0) & "'", Carga_Coneccion_O3b)
            Dim Carga_Reader_O3b As OleDb.OleDbDataReader = Carga_Comando_O3b.ExecuteReader(CommandBehavior.CloseConnection)
            While Carga_Reader_O3b.Read
                If IsDBNull(Carga_Reader_O3b.GetValue(0)) = False Then
                    consolidadora = Carga_Reader_O3b.GetValue(0)
                End If
            End While
            Carga_Coneccion_O3.Dispose()
            Alcance_recup(0) = consolidadora
            GoTo busca_3
        End If
Busca_4:
        Dim Carga_Coneccion_O4 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Carga_Coneccion_O4.Open()
        Dim Carga_Comando_O4 As New OleDb.OleDbCommand("Select * from IQ_Design where IQDesign_Oficina = '999999' and IQDesign_Area = '' and IQDEsign_Punto = '' And IQDesign_Tipo = 'L' Order by IQDesign_Orden", Carga_Coneccion_O4)
        Dim Carga_Reader_O4 As OleDb.OleDbDataReader = Carga_Comando_O4.ExecuteReader(CommandBehavior.CloseConnection)
        While Carga_Reader_O4.Read
            If IsDBNull(Carga_Reader_O4.GetValue(0)) = False Then
                encontrado = True
                controles(num_controles, 0) = Carga_Reader_O4.GetValue(5)
                controles(num_controles, 1) = Carga_Reader_O4.GetValue(4)
                controles(num_controles, 2) = CStr(Carga_Reader_O4.GetValue(6))
                controles(num_controles, 3) = CStr(Carga_Reader_O4.GetValue(7))
                controles(num_controles, 4) = CStr(Carga_Reader_O4.GetValue(8))
                controles(num_controles, 5) = CStr(Carga_Reader_O4.GetValue(9))
                num_controles += 1
            End If
        End While
        Carga_Coneccion_O4.Dispose()
Busca_Fin:
        If num_controles > 0 Then
            Background = Nothing
            PanelDesign.Height = Me.Height
            PanelDesign.Width = Me.Width
            PanelDesign.Location = New System.Drawing.Point(0, 0)
            Dim rel_ancho As Double = Me.Width / 960
            Dim rel_alto As Double = Me.Height / 540
            For counter = 0 To num_controles - 1
                If controles(counter, 0) <> "" Then
                    Buscar_Files(counter)
                    Select Case controles(counter, 1)
                        Case "Texto Fijo"
                            Dim NuevoTexto As New Label()
                            PanelDesign.Controls.Add(NuevoTexto)
                            NuevoTexto.Name = controles(counter, 0)
                            NuevoTexto.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoTexto.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            Try
                                NuevoTexto.Text = System.IO.File.ReadAllText(controles(counter, 6))
                            Catch ex As Exception
                                NuevoTexto.Text = ""
                            End Try
                            Graba_Log(counter, NuevoTexto.Text)
                            controles(counter, 6) = NuevoTexto.Text
                            NuevoTexto.Tag = controles(counter, 1)
                            Dim alto_font As Integer
                            alto_font = (Int(CInt(controles(counter, 3)) * rel_alto)) * 0.6
                            NuevoTexto.Font = New Font("Arial", alto_font, FontStyle.Bold)
                            If Background Is Nothing Then
                                NuevoTexto.Parent = Me
                            Else
                                NuevoTexto.Parent = Background
                            End If
                            NuevoTexto.BackColor = Color.Transparent
                            NuevoTexto.ForeColor = Color.DarkBlue
                            NuevoTexto.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoTexto.TabIndex = counter
                            NuevoTexto.BringToFront()
                        Case "Texto Dinámico"
                            Dim NuevoTexto As New Label()
                            PanelDesign.Controls.Add(NuevoTexto)
                            NuevoTexto.Name = controles(counter, 0)
                            NuevoTexto.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoTexto.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            Try
                                NuevoTexto.Text = System.IO.File.ReadAllText(controles(counter, 6))
                            Catch ex As Exception
                                NuevoTexto.Text = ""
                            End Try
                            Graba_Log(counter, NuevoTexto.Text)
                            controles(counter, 6) = NuevoTexto.Text
                            NuevoTexto.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoTexto.Tag = controles(counter, 1)
                            NuevoTexto.UseCompatibleTextRendering = True
                            NuevoTexto.TabIndex = counter
                            Dim alto_font As Integer
                            alto_font = (Int(CInt(controles(counter, 3)) * rel_alto)) * 0.6
                            NuevoTexto.Font = New Font("Arial", alto_font, FontStyle.Bold)
                            If Background Is Nothing Then
                                NuevoTexto.Parent = Me
                            Else
                                NuevoTexto.Parent = Background
                            End If
                            NuevoTexto.BackColor = Color.Transparent
                            NuevoTexto.ForeColor = Color.DarkBlue
                            NuevoTexto.BringToFront()
                            If TimerMovil.Enabled = False Then
                                TimerMovil.Enabled = True
                                TimerMovil.Start()
                            End If
                        Case "Texto Parpadeante"
                            Dim NuevoTexto As New Label()
                            PanelDesign.Controls.Add(NuevoTexto)
                            NuevoTexto.Name = controles(counter, 0)
                            NuevoTexto.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoTexto.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            Try
                                NuevoTexto.Text = System.IO.File.ReadAllText(controles(counter, 6))
                            Catch ex As Exception
                                NuevoTexto.Text = ""
                            End Try
                            Graba_Log(counter, NuevoTexto.Text)
                            controles(counter, 6) = NuevoTexto.Text
                            NuevoTexto.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoTexto.Tag = controles(counter, 1)
                            NuevoTexto.TabIndex = counter
                            Dim alto_font As Integer
                            alto_font = (Int(CInt(controles(counter, 3)) * rel_alto)) * 0.6
                            NuevoTexto.Font = New Font("Arial", alto_font, FontStyle.Bold)
                            If Background Is Nothing Then
                                NuevoTexto.Parent = Me
                            Else
                                NuevoTexto.Parent = Background
                            End If
                            NuevoTexto.BackColor = Color.Transparent
                            NuevoTexto.ForeColor = Color.DarkBlue
                            NuevoTexto.BringToFront()
                            If TimerBlink.Enabled = False Then
                                TimerBlink.Enabled = True
                                TimerBlink.Start()
                            End If
                        Case "Fecha y Hora"
                            Dim NuevoTexto As New Label()
                            PanelDesign.Controls.Add(NuevoTexto)
                            NuevoTexto.Name = controles(counter, 0)
                            NuevoTexto.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoTexto.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoTexto.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoTexto.TextAlign = ContentAlignment.MiddleCenter
                            NuevoTexto.TabIndex = counter
                            Dim alto_font As Integer
                            alto_font = (Int(CInt(controles(counter, 3)) * rel_alto)) * 0.6
                            NuevoTexto.Font = New Font("Arial", alto_font, FontStyle.Bold)
                            If Background Is Nothing Then
                                NuevoTexto.Parent = Me
                            Else
                                NuevoTexto.Parent = Background
                            End If
                            NuevoTexto.BackColor = Color.Transparent
                            NuevoTexto.ForeColor = Color.DarkBlue
                            NuevoTexto.BringToFront()
                        Case "Imagen"
                            Dim NuevaImagen As New PictureBox()
                            PanelDesign.Controls.Add(NuevaImagen)
                            NuevaImagen.Name = controles(counter, 0)
                            NuevaImagen.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevaImagen.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            Try
                                NuevaImagen.Image = System.Drawing.Image.FromFile(controles(counter, 6))
                            Catch ex As Exception
                                NuevaImagen.Image = Nothing
                            End Try
                            Graba_Log(counter, controles(counter, 6))
                            NuevaImagen.SizeMode = PictureBoxSizeMode.StretchImage
                            NuevaImagen.TabIndex = counter
                            NuevaImagen.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevaImagen.BringToFront()
                            If Background Is Nothing Then
                                Background = NuevaImagen
                            Else
                                NuevaImagen.Parent = Background
                            End If
                        Case "PDF"
                            Dim NuevoPDF As New WebBrowser
                            PanelDesign.Controls.Add(NuevoPDF)
                            NuevoPDF.BackColor = Color.Black
                            NuevoPDF.Name = controles(counter, 0)
                            NuevoPDF.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoPDF.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoPDF.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoPDF.Visible = True
                            NuevoPDF.TabIndex = counter
                            If Background Is Nothing Then
                                NuevoPDF.Parent = Me
                            Else
                                NuevoPDF.Parent = Background
                            End If
                            NuevoPDF.BringToFront()
                            Graba_Log(counter, controles(counter, 6))
                            Try
                                NuevoPDF.Navigate(controles(counter, 6))
                            Catch ex As Exception
                                Exit Try
                            End Try
                        Case "Galería de Imágenes"
                            Dim NuevaImagen As New PictureBox()
                            PanelDesign.Controls.Add(NuevaImagen)
                            NuevaImagen.Name = controles(counter, 0)
                            NuevaImagen.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevaImagen.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevaImagen.Image = Nothing
                            Dim fileNames = My.Computer.FileSystem.GetFiles(controles(counter, 6), FileIO.SearchOption.SearchTopLevelOnly)
                            num_imagenes = 0
                            For Each fileName As String In fileNames
                                Dim extension As String = Mid(fileName, InStr(fileName, ".") + 1, Len(fileName) - InStr(fileName, "."))
                                If UCase(extension) = "JPG" Or UCase(extension) = "JPEG" Or UCase(extension) = "JPE" Or UCase(extension) = "GIF" Or UCase(extension) = "TIF" Or UCase(extension) = "TIFF" Or UCase(extension) = "PNG" Or UCase(extension) = "ICO" Then
                                    Lista_Imagenes(num_imagenes) = fileName
                                    num_imagenes += 1
                                End If
                            Next
                            If num_imagenes = 0 Then
                                NuevaImagen.Image = Nothing
                                Me.TimerImage.Enabled = False
                                Me.TimerImage.Stop()
                            Else
                                Try
                                    NuevaImagen.Image = System.Drawing.Image.FromFile(Lista_Imagenes(0))
                                Catch ex As Exception
                                    NuevaImagen.Image = Nothing
                                End Try
                                imagen_actual = 0
                                Me.TimerImage.Interval = 10000
                                Me.TimerImage.Enabled = True
                                Me.TimerImage.Start()
                            End If
                            Graba_Log(counter, controles(counter, 6))
                            NuevaImagen.SizeMode = PictureBoxSizeMode.StretchImage
                            NuevaImagen.TabIndex = counter
                            NuevaImagen.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            If Background Is Nothing Then
                                NuevaImagen.Parent = Me
                            Else
                                NuevaImagen.Parent = Background
                            End If
                            NuevaImagen.BringToFront()
                        Case "LiveFeed"
                            Dim NuevoFeed As New WebBrowser
                            PanelDesign.Controls.Add(NuevoFeed)
                            NuevoFeed.BackColor = Color.Black
                            NuevoFeed.Name = controles(counter, 0)
                            NuevoFeed.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoFeed.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoFeed.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoFeed.Visible = True
                            If Background Is Nothing Then
                                NuevoFeed.Parent = Me
                            Else
                                NuevoFeed.Parent = Background
                            End If
                            NuevoFeed.TabIndex = counter
                            NuevoFeed.BringToFront()
                            Graba_Log(counter, controles(counter, 6))
                            NuevoFeed.Navigate(controles(counter, 6))
                        Case "Video"
                            Dim NuevoVideo1 As New AxWMPLib.AxWindowsMediaPlayer
                            PanelDesign.Controls.Add(NuevoVideo)
                            NuevoVideo1.BackColor = Color.Black
                            NuevoVideo1.Name = controles(counter, 0)
                            NuevoVideo1.Parent = Me
                            NuevoVideo1.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoVideo1.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoVideo1.Location = New Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            Try
                                NuevoVideo1.URL = controles(counter, 6)
                            Catch ex As Exception
                                NuevoVideo1.URL = Nothing
                            End Try
                            Graba_Log(counter, NuevoVideo.URL)
                            NuevoVideo1.TabIndex = counter
                            NuevoVideo1.uiMode = "none"
                            NuevoVideo1.settings.volume = 40
                            NuevoVideo1.windowlessVideo = True
                            NuevoVideo1.settings.setMode("loop", True)
                            NuevoVideo1.Visible = True
                            NuevoVideo1.BringToFront()
                            NuevoVideo1.Ctlcontrols.play()
                        Case "Audio"
                            Dim NuevoAudio As New AxWMPLib.AxWindowsMediaPlayer
                            PanelDesign.Controls.Add(NuevoAudio)
                            NuevoAudio.BackColor = Color.Black
                            NuevoAudio.Name = controles(counter, 0)
                            NuevoAudio.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoAudio.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoAudio.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoAudio.TabIndex = counter
                            Try
                                NuevoAudio.URL = controles(counter, 6)
                            Catch ex As Exception
                                NuevoAudio.URL = Nothing
                            End Try
                            Graba_Log(counter, controles(counter, 6))
                            NuevoAudio.Visible = False
                            NuevoAudio.settings.setMode("loop", True)
                        Case "Galería de Videos"
                            Dim NuevoVideo As New AxWMPLib.AxWindowsMediaPlayer
                            PanelDesign.Controls.Add(NuevoVideo)
                            NuevoVideo.BackColor = Color.Black
                            NuevoVideo.Name = controles(counter, 0)
                            NuevoVideo.Parent = Me
                            NuevoVideo.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoVideo.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoVideo.Location = New Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoVideo.URL = Nothing
                            Dim fileNames = My.Computer.FileSystem.GetFiles(controles(counter, 6), FileIO.SearchOption.SearchTopLevelOnly)
                            num_videos = 0
                            'Dim media As WMPLib.IWMPMedia3 = NuevoVideo.mediaCollection.getAll().Item(0)
                            'Dim mediaName As String = media.name
                            'MsgBox("" & mediaName)
                            'NuevoVideo.mediaCollection.remove(media, True)

                            ' NuevoVideo.playlistCollection.remove("MyProgramMadePlaylist")
                            NuevoVideo.playlistCollection.newPlaylist("MyProgramMadePlaylist")
                            NuevoVideo.settings.setMode("loop", True)
                            NuevoVideo.currentPlaylist.clear()
                            For Each fileName As String In fileNames
                                Dim extension As String = Mid(fileName, InStr(fileName, ".") + 1, Len(fileName) - InStr(fileName, "."))
                                If UCase(extension) = "MOV" Or UCase(extension) = "MPG" Or UCase(extension) = "WMV" Or UCase(extension) = "MP4" Or UCase(extension) = "MPEG" Or UCase(extension) = "AVI" Then
                                    Lista_Videos(num_videos) = fileName
                                    num_videos += 1
                                    Dim songs = NuevoVideo.newMedia(fileName)
                                    NuevoVideo.currentPlaylist.appendItem(songs)
                                End If
                            Next
                            If num_videos = 0 Then
                                NuevoVideo.URL = Nothing
                                NuevoVideo.currentPlaylist.clear()
                            Else
                                Try
                                    If NuevoVideo.playState = WMPPlayState.wmppsReady Then
                                        NuevoVideo.Ctlcontrols.play()
                                    End If
                                Catch ex As Exception
                                    NuevoVideo.URL = Nothing
                                End Try
                            End If
                            Graba_Log(counter, controles(counter, 6))
                            NuevoVideo.TabIndex = counter
                            NuevoVideo.uiMode = "none"
                            NuevoVideo.settings.volume = 40
                            NuevoVideo.windowlessVideo = True
                            NuevoVideo.Visible = True
                            NuevoVideo.BringToFront()
                        Case "Ticket"
                            Dim NuevoTexto As New Label()
                            PanelDesign.Controls.Add(NuevoTexto)
                            NuevoTexto.Name = controles(counter, 0)
                            NuevoTexto.Width = Int(CInt(controles(counter, 2)) * rel_ancho)
                            NuevoTexto.Height = Int(CInt(controles(counter, 3)) * rel_alto)
                            NuevoTexto.BorderStyle = BorderStyle.FixedSingle
                            NuevoTexto.TabIndex = counter
                            Dim alto_font As Integer
                            alto_font = (Int(CInt(controles(counter, 3)) * rel_alto)) * 0.35
                            If alto_font > 50 Then
                                NuevoTexto.TextAlign = ContentAlignment.MiddleCenter
                                alto_font = 36
                            Else
                                alto_font = 26
                            End If
                            NuevoTexto.Font = New Font("Arial", alto_font, FontStyle.Bold)
                            NuevoTexto.TextAlign = ContentAlignment.MiddleCenter
                            NuevoTexto.Tag = controles(counter, 1)
                            If Background Is Nothing Then
                                NuevoTexto.Parent = Me
                            Else
                                NuevoTexto.Parent = Background
                            End If
                            NuevoTexto.BackColor = Color.Transparent
                            NuevoTexto.Location = New System.Drawing.Point(Int(CInt(controles(counter, 4)) * rel_ancho), Int(CInt(controles(counter, 5)) * rel_alto))
                            NuevoTexto.BringToFront()
                            '      If TimerTicket.Enabled = False Then
                            '          TimerTicket.Enabled = True
                            '          TimerTicket.Start()
                            '      End If
                            TimerTicket.Enabled = False
                            TimerTicket.Stop()
                            If num_displays = 0 Then
                                displays(0, 0) = NuevoTexto.Name
                                displays(0, 1) = controles(counter, 4)
                                displays(0, 2) = controles(counter, 5)
                                displays(0, 4) = controles(counter, 3)
                            Else
                                Dim ind_disp As Integer
                                Dim pos_disp As Integer = 999999
                                For ind_disp = 0 To num_displays - 1
                                    If CInt(controles(counter, 3)) > CInt(displays(ind_disp, 4)) Then
                                        pos_disp = ind_disp
                                        Exit For
                                    ElseIf CInt(controles(counter, 3)) = CInt(displays(ind_disp, 4)) Then
                                        If CInt(controles(counter, 5)) < CInt(displays(ind_disp, 2)) Then
                                            pos_disp = ind_disp
                                            Exit For
                                        ElseIf CInt(controles(counter, 5)) = CInt(displays(ind_disp, 2)) And CInt(controles(counter, 4)) < CInt(displays(ind_disp, 1)) Then
                                            pos_disp = ind_disp
                                            Exit For
                                        End If
                                    End If
                                Next
                                If pos_disp = 999999 Then
                                    displays(num_displays, 0) = NuevoTexto.Name
                                    displays(num_displays, 1) = controles(counter, 4)
                                    displays(num_displays, 2) = controles(counter, 5)
                                    displays(num_displays, 4) = controles(counter, 3)
                                Else
                                    For ind_disp = num_displays - 1 To pos_disp Step -1
                                        displays(ind_disp + 1, 0) = displays(ind_disp, 0)
                                        displays(ind_disp + 1, 1) = displays(ind_disp, 1)
                                        displays(ind_disp + 1, 2) = displays(ind_disp, 2)
                                        displays(ind_disp + 1, 4) = displays(ind_disp, 4)
                                    Next
                                    displays(pos_disp, 0) = NuevoTexto.Name
                                    displays(pos_disp, 1) = controles(counter, 4)
                                    displays(pos_disp, 2) = controles(counter, 5)
                                    displays(pos_disp, 4) = controles(counter, 3)
                                End If
                            End If
                            num_displays += 1
                    End Select
                End If
            Next
        End If
        Cursor.Hide()
        TimerFecha.Enabled = True
        TimerFecha.Start()
        AudioTicket2.playlistCollection.newPlaylist("MyProgramMadePlaylist")
        AudioTicket2.settings.setMode("loop", False)
        AudioTicket2.Ctlcontrols.play()
        Me.TimerParp.Enabled = True
        Me.TimerParp.Start()
        If string_Pending <> "" Then
            TimerSearch.Enabled = True
            TimerSearch.Start()
        End If
    End Sub
    Private Sub TimerTicket_Tick(sender As Object, e As EventArgs) Handles TimerTicket.Tick
        TimerTicket.Enabled = False
        TimerTicket.Stop()
        For Each control_a_buscar As Control In Background.Controls
            If control_a_buscar.Tag = "Ticket" Then
                If TimerTicket.Tag = True Then
                    control_a_buscar.Text = controles(control_a_buscar.TabIndex, 6)
                Else
                    control_a_buscar.Text = ""
                End If
            End If
        Next
        If TimerTicket.Tag = True Then
            TimerTicket.Tag = False
        Else
            TimerTicket.Tag = True
        End If
        TimerTicket.Enabled = True
        TimerTicket.Start()
    End Sub
    Private Sub TimerParp_Tick(sender As Object, e As EventArgs) Handles TimerParp.Tick
        TimerParp.Enabled = False
        TimerParp.Stop()
        For ind_rein = 0 To 1
            For Each control_a_buscar As Control In Background.Controls
                If control_a_buscar.Tag = "Ticket" Then
                    If control_a_buscar.Name = displays(ind_rein, 0) Then
                        If TimerParp.Tag = True And tickets_actuales(ind_rein, 0) <> "" Then
                            '  control_a_buscar.Text = tickets_actuales(ind_rein, 0) & " > " & tickets_actuales(ind_rein, 1)
                            control_a_buscar.Visible = False
                        Else
                            ' control_a_buscar.Text = ""
                            control_a_buscar.Visible = True
                        End If
                    End If
                End If
            Next
        Next
        If TimerParp.Tag = True Then
            TimerParp.Tag = False
        Else
            TimerParp.Tag = True
        End If
        TimerParp.Enabled = True
        TimerParp.Start()
    End Sub
    Private Sub TimerSearch_Tick(sender As Object, e As EventArgs) Handles TimerSearch.Tick
        TimerSearch.Enabled = False
        TimerSearch.Stop()
        Dim num_pendientes As Integer = 0
        Dim pendientes(100, 11) As String
        Dim Carga_Coneccion_O2 As New OleDb.OleDbConnection(Cnn_Central_Server)
        Dim ind_rein As Integer
        For ind_rein = 0 To num_displays - 1
            tickets_actuales(ind_rein, 3) = ""
        Next
        Dim dead_lock_ok As Boolean = False
        While dead_lock_ok = False
            Try
                Carga_Coneccion_O2.Open()
                Dim Carga_Comando_O2 As New OleDb.OleDbCommand("Select IQ_Pending.*, getdate() from IQ_Pending JOIN Iq_PuntosAtencion on IQ_Pending.IQPending_Codpunto = IQ_PuntosAtencion.IQPuntos_Codigo where (" & string_Pending & ") And IQ_Pending.IQPending_CodPunto <> '' Order by IQ_Pending.IQPending_Asignado Desc", Carga_Coneccion_O2)
                Dim Carga_Reader_O2 As OleDb.OleDbDataReader = Carga_Comando_O2.ExecuteReader(CommandBehavior.CloseConnection)
                dead_lock_ok = True
                While Carga_Reader_O2.Read
                    If IsDBNull(Carga_Reader_O2.GetValue(0)) = False Then
                        pendientes(num_pendientes, 10) = Carga_Reader_O2.GetValue(0)
                        pendientes(num_pendientes, 0) = Carga_Reader_O2.GetValue(1)
                        pendientes(num_pendientes, 1) = Carga_Reader_O2.GetValue(2)
                        pendientes(num_pendientes, 2) = Format(Carga_Reader_O2.GetValue(6), "yyyy/MM/dd HH:mm:ss")
                        pendientes(num_pendientes, 8) = Format(Carga_Reader_O2.GetValue(10), "yyyy/MM/dd HH:mm:ss")
                        pendientes(num_pendientes, 9) = Carga_Reader_O2.GetValue(9)
                        pendientes(num_pendientes, 3) = "0"
                        For ind_rein = 0 To num_displays - 1
                            If tickets_actuales(ind_rein, 0) <> "" Then
                                If pendientes(num_pendientes, 0) = tickets_actuales(ind_rein, 0) And pendientes(num_pendientes, 1) = tickets_actuales(ind_rein, 1) Then
                                    Dim original As Double = 0
                                    Dim actual As Double = 0
                                    Dim anio As Integer = Year(CDate(pendientes(num_pendientes, 2))) * 365
                                    Dim mes As Integer = Month(CDate(pendientes(num_pendientes, 2))) * 30
                                    Dim dia As Integer = CInt(Format(CDate(pendientes(num_pendientes, 2)), "dd"))
                                    original = ((anio + mes + dia) / 1000) * 86400
                                    original = original + CInt(Format(CDate(pendientes(num_pendientes, 2)), "HH")) * 3.6
                                    original = original + CInt(Format(CDate(pendientes(num_pendientes, 2)), "mm")) * (60 / 1000)
                                    original = original + CInt(Format(CDate(pendientes(num_pendientes, 2)), "ss")) * (1 / 1000)
                                    anio = Year(CDate(pendientes(num_pendientes, 8))) * 365
                                    mes = Month(CDate(pendientes(num_pendientes, 8))) * 30
                                    dia = CInt(Format(CDate(pendientes(num_pendientes, 8)), "dd"))
                                    actual = ((anio + mes + dia) / 1000) * 86400
                                    actual = actual + CInt(Format(CDate(pendientes(num_pendientes, 8)), "HH")) * 3.6
                                    actual = actual + CInt(Format(CDate(pendientes(num_pendientes, 8)), "mm")) * (60 / 1000)
                                    actual = actual + CInt(Format(CDate(pendientes(num_pendientes, 8)), "ss")) * (1 / 1000)
                                    actual = (actual * 1000) - (original * 1000)
                                    actual = Int(actual / 10)
                                    pendientes(num_pendientes, 3) = Trim(CStr(actual))
                                    tickets_actuales(ind_rein, 3) = "X"
                                End If
                            End If
                        Next
                        num_pendientes += 1
                    End If
                End While
                Carga_Coneccion_O2.Dispose()
            Catch ex As Exception
                Carga_Coneccion_O2.Dispose()
                dead_lock_ok = False
            End Try
        End While
        Dim num_libres As Integer = 0
        If num_pendientes > num_displays Then
            num_libres = 0
        Else
            num_libres = num_displays - num_pendientes
        End If
        Dim a_mover(num_libres, 3) As String
        For ind_rein = 0 To num_libres - 1
            a_mover(ind_rein, 0) = ""
        Next
        Dim ind_libre As Integer = 0
        If num_libres > 0 Then
            For ind_rein = 0 To num_displays - 1
                If tickets_actuales(ind_rein, 3) = "" And tickets_actuales(ind_rein, 0) <> "" And ind_libre < num_libres Then
                    Dim fecha_act As Date
                    Try
                        fecha_act = DateAdd(DateInterval.Second, desfase_segundos, DateTime.Now)
                    Catch ex As Exception
                        fecha_act = DateAdd(DateInterval.Second, 0, DateTime.Now)
                        Exit Try
                    End Try
                    If Format(CDate(tickets_actuales(ind_rein, 2)), "yyyy/MM/dd") = Format(fecha_act, "yyyy/MM/dd") Then
                        a_mover(ind_libre, 0) = tickets_actuales(ind_rein, 0)
                        a_mover(ind_libre, 1) = tickets_actuales(ind_rein, 1)
                        a_mover(ind_libre, 2) = tickets_actuales(ind_rein, 2)
                        ind_libre += 1
                    End If
                End If
            Next
        End If
        For ind_rein = 0 To num_displays - 1
            tickets_actuales(ind_rein, 0) = ""
            tickets_actuales(ind_rein, 1) = ""
            tickets_actuales(ind_rein, 2) = ""
            tickets_actuales(ind_rein, 4) = ""
            tickets_actuales(ind_rein, 5) = ""
            tickets_actuales(ind_rein, 6) = ""
            tickets_actuales(ind_rein, 7) = ""
        Next
        If num_pendientes < num_displays Then
            For ind_rein = 0 To num_pendientes - 1
                tickets_actuales(ind_rein, 0) = pendientes(ind_rein, 0)
                tickets_actuales(ind_rein, 1) = pendientes(ind_rein, 1)
                tickets_actuales(ind_rein, 2) = pendientes(ind_rein, 2)
                tickets_actuales(ind_rein, 3) = pendientes(ind_rein, 3)
                tickets_actuales(ind_rein, 4) = pendientes(ind_rein, 4)
                tickets_actuales(ind_rein, 5) = pendientes(ind_rein, 9)
                tickets_actuales(ind_rein, 6) = pendientes(ind_rein, 10)
                tickets_actuales(ind_rein, 7) = pendientes(ind_rein, 0)
            Next
            Dim ind_rein_2 As Integer
            Dim ind_rein_3 As Integer
            For ind_rein_2 = 0 To num_libres - 1
                If a_mover(ind_rein_2, 0) <> "" Then
                    For ind_rein_3 = 0 To num_displays - 1
                        If tickets_actuales(ind_rein_3, 0) = "" Then
                            tickets_actuales(ind_rein_3, 0) = a_mover(ind_rein_2, 0)
                            tickets_actuales(ind_rein_3, 1) = a_mover(ind_rein_2, 1)
                            tickets_actuales(ind_rein_3, 2) = a_mover(ind_rein_2, 2)
                            tickets_actuales(ind_rein_3, 3) = "OK"
                            Exit For
                        ElseIf CDate(a_mover(ind_rein_2, 2)) > CDate(tickets_actuales(ind_rein_3, 2)) Then
                            Dim ind_rein_4 As Integer
                            For ind_rein_4 = num_displays - 1 To ind_rein_3 + 1 Step -1
                                tickets_actuales(ind_rein_4, 0) = pendientes(ind_rein_4 - 1, 0)
                                tickets_actuales(ind_rein_4, 1) = pendientes(ind_rein_4 - 1, 1)
                                tickets_actuales(ind_rein_4, 2) = pendientes(ind_rein_4 - 1, 2)
                                tickets_actuales(ind_rein_4, 3) = pendientes(ind_rein_4 - 1, 3)
                                tickets_actuales(ind_rein_4, 4) = pendientes(ind_rein_4 - 1, 4)
                                tickets_actuales(ind_rein_4, 5) = pendientes(ind_rein_4 - 1, 5)
                                tickets_actuales(ind_rein_4, 6) = pendientes(ind_rein_4 - 1, 6)
                                tickets_actuales(ind_rein_4, 7) = pendientes(ind_rein_4 - 1, 7)
                            Next
                            tickets_actuales(ind_rein_3, 0) = a_mover(ind_rein_2, 0)
                            tickets_actuales(ind_rein_3, 1) = a_mover(ind_rein_2, 1)
                            tickets_actuales(ind_rein_3, 2) = a_mover(ind_rein_2, 2)
                            tickets_actuales(ind_rein_3, 3) = "OK"
                            Exit For
                        End If
                    Next
                End If
            Next
        Else
            For ind_rein = 0 To num_displays - 1
                tickets_actuales(ind_rein, 0) = pendientes(ind_rein, 0)
                tickets_actuales(ind_rein, 1) = pendientes(ind_rein, 1)
                tickets_actuales(ind_rein, 2) = pendientes(ind_rein, 2)
                tickets_actuales(ind_rein, 3) = pendientes(ind_rein, 3)
                tickets_actuales(ind_rein, 4) = pendientes(ind_rein, 4)
                tickets_actuales(ind_rein, 5) = pendientes(ind_rein, 9)
                tickets_actuales(ind_rein, 6) = pendientes(ind_rein, 10)
                tickets_actuales(ind_rein, 7) = pendientes(ind_rein, 0)
            Next
        End If
        If num_pendientes = 0 Then
            AudioTicket2.currentPlaylist.clear()
        End If
        For ind_rein = 0 To num_displays - 1
            For Each control_a_buscar As Control In Background.Controls
                If Mid(control_a_buscar.Name, 1, 6) = "Ticket" Then
                    If control_a_buscar.Name = displays(ind_rein, 0) Then
                        If tickets_actuales(ind_rein, 0) <> "" Then
                            Tiptick_to_call = Trim(Mid(tickets_actuales(ind_rein, 0), 1, InStr(tickets_actuales(ind_rein, 0), "-") - 1))
                            Numtick_to_call = Trim(Mid(tickets_actuales(ind_rein, 0), InStr(tickets_actuales(ind_rein, 0), "-") + 1, Len(tickets_actuales(ind_rein, 0)) - InStr(tickets_actuales(ind_rein, 0), "-")))
                            Punto_to_call = tickets_actuales(ind_rein, 1)
                            control_a_buscar.Text = tickets_actuales(ind_rein, 0) & ">" & tickets_actuales(ind_rein, 1)
                            If tickets_actuales(ind_rein, 3) = "OK" Then
                                control_a_buscar.ForeColor = Color.Green
                            ElseIf IsNumeric(tickets_actuales(ind_rein, 5)) Then
                                If CInt(tickets_actuales(ind_rein, 5)) = 1 Then
                                    control_a_buscar.ForeColor = Color.Yellow
                                ElseIf CInt(tickets_actuales(ind_rein, 5)) > 1 Then
                                    control_a_buscar.ForeColor = Color.Red
                                End If
                            ElseIf CInt(tickets_actuales(ind_rein, 3)) < 4 Then
                                control_a_buscar.ForeColor = Color.White
                            End If
                            If tickets_actuales(ind_rein, 3) <> "OK" And tickets_actuales(ind_rein, 5) <> "N" Then
                                Dim urls_audio(5) As String
                                If IsNumeric(tickets_actuales(ind_rein, 5)) Then
                                    If CInt(tickets_actuales(ind_rein, 5)) = 1 Then
                                        urls_audio(0) = Disco_Appl & ":\I-Q\Multimedia\Timbre.Wav"
                                    Else
                                        urls_audio(0) = Disco_Appl & ":\I-Q\Multimedia\Silence.Wav"
                                    End If
                                Else
                                    urls_audio(0) = Disco_Appl & ":\I-Q\Multimedia\Silence.Wav"
                                End If
                                urls_audio(1) = Disco_Appl & ":\I-Q\Multimedia\Ticket.Wav"
                                urls_audio(2) = Disco_Appl & ":\I-Q\Multimedia\" & Trim(Tiptick_to_call) & ".Wav"
                                urls_audio(3) = Disco_Appl & ":\I-Q\Multimedia\" & Trim(Numtick_to_call) & ".Wav"
                                urls_audio(4) = Disco_Appl & ":\I-Q\Multimedia\" & Trim(Punto_to_call) & ".Wav"
                                AudioTicket2.settings.volume = 1000
                                If AudioTicket2.playState = WMPPlayState.wmppsReady Then
                                    AudioTicket2.currentPlaylist.clear()
                                End If
                                Dim songs = AudioTicket2.newMedia(urls_audio(0))
                                AudioTicket2.currentPlaylist.appendItem(songs)
                                songs = AudioTicket2.newMedia(urls_audio(1))
                                AudioTicket2.currentPlaylist.appendItem(songs)
                                songs = AudioTicket2.newMedia(urls_audio(2))
                                AudioTicket2.currentPlaylist.appendItem(songs)
                                songs = AudioTicket2.newMedia(urls_audio(3))
                                AudioTicket2.currentPlaylist.appendItem(songs)
                                songs = AudioTicket2.newMedia(urls_audio(4))
                                AudioTicket2.currentPlaylist.appendItem(songs)
                                If AudioTicket2.playState = WMPPlayState.wmppsReady Then
                                    AudioTicket2.Ctlcontrols.play()
                                End If
                                Dim instruccion_insert As String = "Update Iq_Pending Set IQPending_Call = 'N' where IQPending_Area = '" & tickets_actuales(ind_rein, 6) & "' and IQPending_Ticket = '" & tickets_actuales(ind_rein, 7) & "'"
                                Try
                                    Dim IQ_Cnn As New OleDb.OleDbConnection(Cnn_Central_Server)
                                    IQ_Cnn.Open()
                                    Dim IQ_Cmm As New OleDb.OleDbCommand(instruccion_insert, IQ_Cnn)
                                    Dim RegistrosInsertados As Long = IQ_Cmm.ExecuteNonQuery()
                                    IQ_Cnn.Close()
                                Catch exc As Exception
                                    Dim Mensaje_Excepcion As String
                                    Mensaje_Excepcion = exc.Message
                                    MessageBox.Show(Mensaje_Excepcion, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                                    Exit Sub
                                End Try
                            End If
                        Else
                            control_a_buscar.Text = " "
                        End If
                    End If
                End If
            Next
        Next
        TimerSearch.Enabled = True
        TimerSearch.Start()
    End Sub
End Class