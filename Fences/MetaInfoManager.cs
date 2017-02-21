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
    }
}