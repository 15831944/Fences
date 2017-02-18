using System.IO;
using System.Windows.Forms;
using Fences.Properties;

namespace Fences
{
    public class MetaInfoManager
    {
        public void CreateFenceSettings()
        {
            DialogBox m = new DialogBox();
            m.ShowDialog();
            if (m.DialogResult == DialogResult.OK)
                Settings.Default.path = DialogBox.ReturnValue ? FileCreator.CreateFile() : FileCreator.OpenFile();
            Settings.Default.Save();
        }

        public bool IsInitialized()
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