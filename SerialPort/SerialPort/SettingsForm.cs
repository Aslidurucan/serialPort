using System;
using System.Drawing;
using System.Windows.Forms;

namespace SerialPort
{
    public partial class SettingsForm : Form
    {
        public Color BackgroundColor { get; private set; }
        public Color TextColor { get; private set; }

        public SettingsForm(Color currentBackgroundColor, Color currentTextColor)
        {
            InitializeComponent();
            this.ShowIcon = false;

            this.Text = "Settings";

            BackgroundColor = currentBackgroundColor;
            TextColor = currentTextColor;

            btnBackgroundColor.Click += BtnBackgroundColor_Click;
            btnTextColor.Click += BtnTextColor_Click;
            btnApply.Click += BtnApply_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnBackgroundColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    BackgroundColor = colorDialog.Color;
                }
            }
        }

        private void BtnTextColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    TextColor = colorDialog.Color;
                }
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
