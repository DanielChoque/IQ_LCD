<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IQ_L0001
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IQ_L0001))
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.TimerFecha = New System.Windows.Forms.Timer(Me.components)
        Me.PanelDesign = New System.Windows.Forms.Panel()
        Me.AudioTicket2 = New AxWMPLib.AxWindowsMediaPlayer()
        Me.TimerBlink = New System.Windows.Forms.Timer(Me.components)
        Me.TimerMovil = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSearch = New System.Windows.Forms.Timer(Me.components)
        Me.TimerTicket = New System.Windows.Forms.Timer(Me.components)
        Me.TimerAudio = New System.Windows.Forms.Timer(Me.components)
        Me.TimerImage = New System.Windows.Forms.Timer(Me.components)
        Me.LblVersion = New System.Windows.Forms.Label()
        Me.TimerParp = New System.Windows.Forms.Timer(Me.components)
        Me.PanelDesign.SuspendLayout()
        CType(Me.AudioTicket2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Timer1
        '
        '
        'TimerFecha
        '
        '
        'PanelDesign
        '
        Me.PanelDesign.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelDesign.BackColor = System.Drawing.Color.Black
        Me.PanelDesign.Controls.Add(Me.AudioTicket2)
        Me.PanelDesign.ForeColor = System.Drawing.Color.Red
        Me.PanelDesign.Location = New System.Drawing.Point(2, 0)
        Me.PanelDesign.Name = "PanelDesign"
        Me.PanelDesign.Size = New System.Drawing.Size(948, 434)
        Me.PanelDesign.TabIndex = 1
        '
        'AudioTicket2
        '
        Me.AudioTicket2.Enabled = True
        Me.AudioTicket2.Location = New System.Drawing.Point(368, 68)
        Me.AudioTicket2.Name = "AudioTicket2"
        Me.AudioTicket2.OcxState = CType(resources.GetObject("AudioTicket2.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AudioTicket2.Size = New System.Drawing.Size(75, 23)
        Me.AudioTicket2.TabIndex = 0
        '
        'TimerBlink
        '
        Me.TimerBlink.Interval = 400
        '
        'TimerMovil
        '
        Me.TimerMovil.Interval = 400
        '
        'TimerSearch
        '
        Me.TimerSearch.Interval = 1000
        '
        'TimerTicket
        '
        Me.TimerTicket.Interval = 2000
        '
        'TimerAudio
        '
        Me.TimerAudio.Interval = 400
        '
        'TimerImage
        '
        '
        'LblVersion
        '
        Me.LblVersion.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblVersion.AutoSize = True
        Me.LblVersion.BackColor = System.Drawing.Color.Transparent
        Me.LblVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblVersion.Location = New System.Drawing.Point(915, 0)
        Me.LblVersion.Name = "LblVersion"
        Me.LblVersion.Size = New System.Drawing.Size(33, 9)
        Me.LblVersion.TabIndex = 3
        Me.LblVersion.Text = "Label1"
        '
        'TimerParp
        '
        Me.TimerParp.Interval = 500
        '
        'IQ_L0001
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(950, 434)
        Me.ControlBox = False
        Me.Controls.Add(Me.LblVersion)
        Me.Controls.Add(Me.PanelDesign)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IQ_L0001"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.PanelDesign.ResumeLayout(False)
        CType(Me.AudioTicket2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents TimerFecha As System.Windows.Forms.Timer
    Friend WithEvents PanelDesign As System.Windows.Forms.Panel
    Friend WithEvents TimerBlink As System.Windows.Forms.Timer
    Friend WithEvents TimerMovil As System.Windows.Forms.Timer
    Friend WithEvents TimerSearch As System.Windows.Forms.Timer
    Friend WithEvents TimerTicket As System.Windows.Forms.Timer
    Friend WithEvents TimerAudio As System.Windows.Forms.Timer
    Friend WithEvents AudioTicket2 As AxWMPLib.AxWindowsMediaPlayer
    Friend WithEvents TimerImage As System.Windows.Forms.Timer
    Friend WithEvents LblVersion As System.Windows.Forms.Label
    Friend WithEvents TimerParp As System.Windows.Forms.Timer
End Class
