using Autodesk.AutoCAD.Runtime;
using Fences;
using Fences.Properties;

[assembly: CommandClass(typeof(SimpleFences))]

namespace Fences
{
    public class SimpleFences
    {
        private readonly UserSelection _userSelection = new UserSelection();
        /*
         * TODO Add the ability to change the unit height to DialogBox
         * TODO Move FirstFloor to basic class
         * TODO Add the ability to miror dimension lines 
         */
        private readonly MetaInfoManager _metaInfoManager = new MetaInfoManager();

        [CommandMethod("SimpleFencesSettings", CommandFlags.Modal)]
        public void SimpleFencesSettings()
        {
            _metaInfoManager.CreateFenceSettings();
        }

        [CommandMethod("SimpleFencesCreate", CommandFlags.Modal)]
        public void SimpleFencesCreate()
        {
            _metaInfoManager.InitializeIfNeeded();
            _userSelection.SelectPolyline();
        }

        [CommandMethod("SimpleFencesGetTable", CommandFlags.Modal)]
        public void SimpleFencesGetTable()
        {
            TableCreator.CreateTable(Settings.Default.total60X30X4, Settings.Default.total40X4, Settings.Default.totalT10,
                Settings.Default.totalT4, Settings.Default.totalT14);
            Settings.Default.Counter = 0;
        }
    }
}
