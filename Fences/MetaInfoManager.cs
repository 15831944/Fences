using System.IO;
using Fences.Properties;

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