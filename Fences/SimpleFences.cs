using System.IO;
using Autodesk.AutoCAD.Runtime;
using Fences;
using Fences.Properties;

[assembly: CommandClass(typeof(SimpleFences))]

namespace Fences
{
    public class SimpleFences
    {
        /*
         * TODO Add the ability to change the unit height to DialogBox
         * TODO Add the ability to miror dimension lines 
         */

        private readonly MetaInfoManager _metaInfoManager = new MetaInfoManager();

        private readonly UserSelection _userSelection = new UserSelection();

        [CommandMethod("SimpleFenceSetting", CommandFlags.Modal)]
        public void SimpleFenceSetting()
        {
            _metaInfoManager.CreateFenceSettings();
        }

        [CommandMethod("CreateFence", CommandFlags.Modal)]
        public void CreateFence()
        {
            _metaInfoManager.InitializeIfNeeded();
            _userSelection.SelectPolyline();
        }
        [CommandMethod("SimpleFence", CommandFlags.Modal)]
        public void CreateFenceGet()
        {
            FileCreator.GetFromFile(Settings.Default.path);
            File.WriteAllText(Settings.Default.path, string.Empty);
        }

        [CommandMethod("SimpleFenceTable", CommandFlags.Modal)]
        public void CreateFenceTable()
        {
            FileCreator.CreateTable(Settings.Default.total60X30X4, Settings.Default.total40X4, Settings.Default.totalT10,
                Settings.Default.totalT4, Settings.Default.totalT14);
            Settings.Default.Counter = 0;
        }
    }
}