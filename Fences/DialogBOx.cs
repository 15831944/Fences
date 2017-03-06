using System;
using System.Globalization;
using System.Windows.Forms;
using Fences.Properties;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Fences
{
    public partial class DialogBox : Form
    {

        public DialogBox()
        {
            InitializeComponent();
            createFileRadioButton.Checked = true;
        }
        private void DialogBox_Load(object sender, EventArgs e)
        {
            counter.Text = CountFences();

            pilBox.Text = Settings.Default.pil.ToString(CultureInfo.CurrentCulture);
            btmBox.Text = Settings.Default.btm.ToString(CultureInfo.CurrentCulture);
            topBox.Text = Settings.Default.top.ToString(CultureInfo.CurrentCulture);
            dwnPilBox.Text = Settings.Default.pil.ToString(CultureInfo.CurrentCulture);
            barBox.Text = Settings.Default.bar.ToString(CultureInfo.CurrentCulture);
            endBox.Text = Settings.Default.ending.ToString(CultureInfo.CurrentCulture);
        }


        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void chooseFileButton_click(object sender, EventArgs e)
        {
            Settings.Default.path = createFileRadioButton.Checked ? CreateFile() : OpenFile();
        }

        private void changeMassButton_Click(object sender, EventArgs e)
        {
            Settings.Default.pil = double.Parse(pilBox.Text);
            Settings.Default.btm = double.Parse(btmBox.Text);
            Settings.Default.top = double.Parse(topBox.Text);
            Settings.Default.pil = double.Parse(dwnPilBox.Text);
            Settings.Default.bar = double.Parse(barBox.Text);
            Settings.Default.ending = double.Parse(endBox.Text);
        }

        private string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Текстовые файлы (*.txt) | *.txt" };
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        private string CreateFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog { Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" };
            saveFileDialog1.ShowDialog();
            return saveFileDialog1.FileName;
        }

        private string CountFences()
        {
            string n = Settings.Default.CounterLength.ToString(CultureInfo.CurrentCulture);
            return "Учтено" + n + "ограждений";
        }
    }
}