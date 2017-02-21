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
            radioButton1.Checked = true;
        }
        private void DialogBox_Load(object sender, EventArgs e)
        {
            textBox7.Text = CountFences();

            textBox1.Text = Settings.Default.pil.ToString(CultureInfo.CurrentCulture);
            textBox2.Text = Settings.Default.btm.ToString(CultureInfo.CurrentCulture);
            textBox3.Text = Settings.Default.top.ToString(CultureInfo.CurrentCulture);
            textBox4.Text = Settings.Default.pil.ToString(CultureInfo.CurrentCulture);
            textBox5.Text = Settings.Default.bar.ToString(CultureInfo.CurrentCulture);
            textBox6.Text = Settings.Default.ending.ToString(CultureInfo.CurrentCulture);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Default.path = radioButton1.Checked ? CreateFile() : OpenFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Settings.Default.pil = double.Parse(textBox1.Text);
            Settings.Default.btm = double.Parse(textBox2.Text);
            Settings.Default.top = double.Parse(textBox3.Text);
            Settings.Default.pil = double.Parse(textBox4.Text);
            Settings.Default.bar = double.Parse(textBox5.Text);
            Settings.Default.ending = double.Parse(textBox6.Text);
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
            string n = Settings.Default.Counter.ToString(CultureInfo.CurrentCulture);
            return "Учтено" + n + "ограждений";
        }
    }
}