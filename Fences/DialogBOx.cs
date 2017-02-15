using System;
using System.Windows.Forms;

namespace Fences
{
    public partial class DialogBox : Form
    {
        public static bool ReturnValue;

        public DialogBox()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                ReturnValue = true;
            else
                ReturnValue = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void DialogBox_Load(object sender, EventArgs e)
        {
        }
    }
}