using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (radioButton1.Checked == true)
            {
                DialogBox.ReturnValue = true;
            }
            else
            {
                DialogBox.ReturnValue = false;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
