using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text;
using System.Drawing;
using System.IO;
using static System.Net.WebRequestMethods;


namespace SerialPort
{
    public partial class NewForm : Form
    {
        private System.IO.Ports.SerialPort _serialPort;
        private StringBuilder _dataBuffer;
        public static bool isWriting = true;
        private int bufferSize = 10;
       
    

        public NewForm(string portName, int baudRate, int dataSize, Parity parity, StopBits stopBits)
        {
            InitializeComponent();
            this.ShowIcon = false;
            this.FormClosing += formNew_FormClosing;
            this.Text = "Serial Port Data Receiver";
            InitializeSerialPort(portName, baudRate, dataSize, parity, stopBits);
            _dataBuffer = new StringBuilder();
            LoadSettings();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(NewForm_KeyDown);

            panel1.BackColor = richTextBox1.BackColor;
            richTextBox1.BackColorChanged += RichTextBox1_BackColorChanged;

        }

        private void RichTextBox1_BackColorChanged(object sender, EventArgs e)
        {
            panel1.BackColor = richTextBox1.BackColor;
        }

        private void NewForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (this.ActiveControl is RichTextBox richTextBox)
                {
                    richTextBox1.Copy(); // Ctrl+C tuş kombinasyonu ile seçili metni kopyalama
                }
                e.Handled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                isWriting = !isWriting;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this.BackColor, this.ForeColor);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                ApplySettings(settingsForm.BackgroundColor, settingsForm.TextColor);
            }
        }

        private void ApplySettings(Color backgroundColor, Color textColor)
        {
            this.BackColor = backgroundColor;
            richTextBox1.BackColor = backgroundColor;
            richTextBox1.ForeColor = EnsureContrast(backgroundColor, textColor);

            Properties.Settings.Default.BackgroundColor = backgroundColor;
            Properties.Settings.Default.TextColor = textColor;
            Properties.Settings.Default.Save();

        }



        private void InitializeSerialPort(string portName, int baudRate, int dataSize, Parity parity, StopBits stopBits)
        {
            _serialPort = new System.IO.Ports.SerialPort
            {
                BaudRate = baudRate,
                PortName = portName,
                DataBits = dataSize,
                Parity = parity,
                StopBits = stopBits,
                Handshake = Handshake.None,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!isWriting) return;

            try
            {
                byte[] buffer = new byte[bufferSize];

                while (_serialPort.BytesToRead > 0)
                {
                    int bytesRead = _serialPort.Read(buffer, 0, bufferSize);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessData(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ProcessData(string data)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                _dataBuffer.Append(data);

                int newLineIndex;
                while ((newLineIndex = _dataBuffer.ToString().IndexOf('\n')) != -1)
                {
                    string completeMessage = _dataBuffer.ToString(0, newLineIndex + 1);
                    _dataBuffer.Remove(0, newLineIndex + 1);

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    int byteCount = System.Text.Encoding.UTF8.GetByteCount(completeMessage);

                    Invoke(new Action(() =>
                    {
                        AppendDataWithFormatting(completeMessage, timestamp, byteCount);
                    }));
                }
                // \n yoksa databufferda kalcak
            }

        }

        private void AppendDataWithFormatting(string data, string timestamp, int byteCount)
        {
            string formattedData = $"[{timestamp}] [{byteCount} bytes] {data}";

            // Kullanıcının seçtiği text renginin zıttını bul
            Color textColor = EnsureContrast(richTextBox1.BackColor, richTextBox1.ForeColor);

            if (richTextBox1.Text.EndsWith("\n"))
            {
                AppendColoredText("-> ", textColor);
            }

            AppendColoredText(formattedData, textColor);

            if (data.EndsWith("\n"))
            {
                AppendColoredText("\n-> ", textColor);
            }
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();  // Scroll en üstteki veriye çıksın
        }


        private void AppendColoredText(string text, Color color)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            richTextBox1.SelectionColor = color;
            richTextBox1.AppendText(text);
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
            }
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void formNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
            }
        }
        private void LoadSettings()
        {

           this.BackColor = Properties.Settings.Default.BackgroundColor;
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ForeColor = Color.White;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    System.IO.File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
                    MessageBox.Show("Text exported successfully.");
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                string dataToSend = tbSend.Text;
                _serialPort.WriteLine(dataToSend + "\r\n");

                // Gönderilen veri için uygun bir renk belirleme
                Color sendColor = GetVisibleColor(richTextBox1.ForeColor, richTextBox1.BackColor);

                AppendColoredText($"Sent: {dataToSend}\r\n", sendColor);
                tbSend.Clear();
            }
        }

        private Color GetContrastingColor(Color color)
        {
            // YIQ formülüne göre kontrast
            double yiq = ((color.R * 299) + (color.G * 587) + (color.B * 114)) / 1000;
            return (yiq >= 128) ? Color.Black : Color.White;
        }

        private Color GetVisibleColor(Color foreColor, Color backColor)
        {
            Color contrastColor = GetContrastingColor(foreColor);

            
            if (!IsColorVisible(contrastColor, backColor))
            {
                contrastColor = GetAlternativeColor(foreColor, backColor);
            }

            return contrastColor;
        }

        private bool IsColorVisible(Color textColor, Color backgroundColor)
        {
            double yiqText = ((textColor.R * 299) + (textColor.G * 587) + (textColor.B * 114)) / 1000;
            double yiqBackground = ((backgroundColor.R * 299) + (backgroundColor.G * 587) + (backgroundColor.B * 114)) / 1000;
            return Math.Abs(yiqBackground - yiqText) >= 128;
        }

        private Color EnsureContrast(Color backgroundColor, Color textColor)
    {
        
        double yiqBackground = ((backgroundColor.R * 299) + (backgroundColor.G * 587) + (backgroundColor.B * 114)) / 1000;
        double yiqText = ((textColor.R * 299) + (textColor.G * 587) + (textColor.B * 114)) / 1000;

        // Yeterli kontrast yoksa, zıttını al
        if (Math.Abs(yiqBackground - yiqText) < 128)
        {
            return GetContrastingColor(backgroundColor);
        }
        return textColor;
    }

        private Color GetAlternativeColor(Color foreColor, Color backColor)
        {
            // Alternatif renkler listesi
            Color[] alternativeColors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Orange, Color.Purple };

            
            foreach (var color in alternativeColors)
            {
                if (color != foreColor && color != backColor)
                {
                    return color;
                }
            }

            
            return GetContrastingColor(foreColor);
        }
    }
}
