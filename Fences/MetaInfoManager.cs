using System.IO;
using System.Windows.Forms;
using Fences.Properties;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Fences
{
    public class MetaInfoManager
    {
        public void CreateFenceSettings()
        {
            DialogBox m = new DialogBox();
            m.ShowDialog();
            if (m.DialogResult == DialogResult.OK)
                Settings.Default.path = DialogBox.ReturnValue ? CreateFile() : OpenFile();
            Settings.Default.Save();
        }

        private bool IsInitialized()
        {
            return Settings.Default.path != null && File.Exists(Settings.Default.path);
        }

        public void InitializeIfNeeded()
        {
            if (!IsInitialized())
            {
                CreateFenceSettings();
            }
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
    }
}