using System;
using System.IO.Ports;
using System.Windows.Forms;


namespace SerialPort
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
           
            this.Icon = null;
            this.Text = "Serial Observer";

            var ports = System.IO.Ports.SerialPort.GetPortNames();
            comboBoxCom.DataSource = ports;

            comboBoxBaude.Items.AddRange(new string[] { "300", "1200", "2400", "4800", "9600", "19200", "115200", "38400", "57600", "115200", "230400", "460800", "921600" });
            comboBoxBaude.SelectedItem = "9600";
            comboBoxBaude.DropDownStyle = ComboBoxStyle.DropDown; // kullanıcı değer girişi için

            comboBoxParity.Items.AddRange(new string[] { "none", "even", "odd" });
            comboBoxParity.SelectedItem = "none";

            comboBoxDataSize.Items.AddRange(new object[] { 7, 8 });
            comboBoxDataSize.SelectedItem = 8;

            comboBoxStopBit.Items.AddRange(new object[] { 1, 2 });
            comboBoxStopBit.SelectedItem = 1;


            LoadSettings();

            //enter tuşu için
            comboBoxCom.KeyPress += new KeyPressEventHandler(comboBox_KeyPress);
            comboBoxBaude.KeyPress += new KeyPressEventHandler(comboBox_KeyPress);

           

        }


        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
                    {
                e.Handled = true; // olay işlenmiş demek
                buttonConnect.PerformClick();
                     }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string selectedPort = comboBoxCom.SelectedItem.ToString();
            int selectedBaudRate;
            if (int.TryParse(comboBoxBaude.Text, out selectedBaudRate))
            {
                int selectedDataSize = (int)comboBoxDataSize.SelectedItem;
                Parity selectedParity = (Parity)Enum.Parse(typeof(Parity), comboBoxParity.SelectedItem.ToString() , true); // stringi enuma dönüştürme 
                StopBits selectedStopBits = (StopBits)comboBoxStopBit.SelectedItem;
                SaveSettings(selectedPort, selectedBaudRate, selectedDataSize, selectedParity, selectedStopBits);

                NewForm newForm = new NewForm(selectedPort, selectedBaudRate, selectedDataSize, selectedParity, selectedStopBits);
                newForm.Show();
                this.Hide();
            }

            else
            {
                MessageBox.Show("Lütfen geçerli bir baud rate giriniz.");
            }
        }

        private void LoadSettings()
        {
            string defaultComPort = Properties.Settings.Default.DefaultComPort;
            int defaultBaudRate = Properties.Settings.Default.DefaultBaudRate;
            int defaultDataSize = Properties.Settings.Default.DefaultDataSize;
            string defaultParity = Properties.Settings.Default.DefaultParity;
            int defaultStopBits = Properties.Settings.Default.DefaultStopBits;

            if (!string.IsNullOrEmpty(defaultComPort) && comboBoxCom.Items.Contains(defaultComPort))
            {
                comboBoxCom.SelectedItem = defaultComPort;
            }
            if (defaultBaudRate != 0 && comboBoxBaude.Items.Contains(defaultBaudRate.ToString()))
            {
                comboBoxBaude.SelectedItem = defaultBaudRate.ToString();
            }
            if (defaultDataSize != 0 && comboBoxDataSize.Items.Contains(defaultDataSize))
            {
                comboBoxDataSize.SelectedItem = defaultDataSize;
            }
            if (!string.IsNullOrEmpty(defaultParity) && comboBoxParity.Items.Contains(defaultParity))
            {
                comboBoxParity.SelectedItem = defaultParity;
            }
            if (defaultStopBits != 0 && comboBoxStopBit.Items.Contains(defaultStopBits))
            {
                comboBoxStopBit.SelectedItem = defaultStopBits;
            }

        }

        private void SaveSettings(string comPort, int baudRate , int dataSize, Parity parity, StopBits stopBits)
        {
            Properties.Settings.Default.DefaultComPort = comPort;
            Properties.Settings.Default.DefaultBaudRate = baudRate;
            Properties.Settings.Default.DefaultDataSize = dataSize;
            Properties.Settings.Default.DefaultParity = parity.ToString();
            Properties.Settings.Default.DefaultStopBits = (int)stopBits;
            Properties.Settings.Default.Save();
        }

       
    }
}


        




    

