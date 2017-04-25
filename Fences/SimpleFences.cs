using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Fences;
using Fences.Properties;
using Application = System.Windows.Forms.Application;
using Exception = System.Exception;

[assembly: CommandClass(typeof(SimpleFences))]

namespace Fences
{
    public class SimpleFences
    {
        /*
         * TODO Add the ability to change the unit height to DialogBox
         * TODO Move FirstFloor to basic class
         * TODO Add the ability to miror dimension lines 
         */
        private readonly MetaInfoManager _metaInfoManager = new MetaInfoManager();
        private readonly UserSelection _userSelection = new UserSelection();
        private FencesAcad _fencesAcad;

        // [DllImport("acad.exe", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAXH@Z")]
        // public static extern void acedDisableDefaultARXExceptionHandler(int value);

        [CommandMethod("SimpleFencesCreate", CommandFlags.Modal)]
        public void SimpleFencesCreate()
        {
            Settings.Default.CounterLength = 0;
            Settings.Default.CounterPils = 0;
            Settings.Default.total40X4 = 0;
            Settings.Default.total60X30X4 = 0;
            Settings.Default.totalT10 = 0;
            Settings.Default.totalT14 = 0;
            Settings.Default.totalT4 = 0;
            try
            {
                Application.ThreadException +=
                    delegate(object o, ThreadExceptionEventArgs args)
                    {
                        Debug.WriteLine(args.Exception.ToString());
                        MessageBox.Show("Caught using event: " + args.Exception.Message, "Exception");
                    };

               // _metaInfoManager.InitializeIfNeeded();
                _userSelection.SelectPolyline();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show("Caught using catch: " + ex.Message, "Exception");
            }
        }

        [CommandMethod("SimpleFencesGetTable", CommandFlags.Modal)]
        public void SimpleFencesGetTable()
        {
            TableCreator.CreateTable(Settings.Default.total60X30X4, Settings.Default.total40X4,
                Settings.Default.totalT10,
                Settings.Default.totalT4, Settings.Default.totalT14);
            Settings.Default.CounterLength = 0;
            Settings.Default.CounterPils = 0;
            Settings.Default.NumEnd = 0;
        }

        [CommandMethod("SimpleFencesGetFirstFloor", CommandFlags.Modal)] //TODO Not so good one
        public void SimpleFencesFirstFloor()
        {
            _userSelection.GetDataFromSelectionFirst();
        }

        [CommandMethod("SimpleFencesGet", CommandFlags.Modal)]
        public void SimpleFencesGet()
        {
            _userSelection.GetDataFromSelection();
        }

        [CommandMethod("CrearSimpleFences", CommandFlags.Modal)]
        public void CrearSimpleFences()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction transaction = acCurDb.TransactionManager.StartTransaction())
            {
                PromptSelectionResult prompt = acDoc.Editor.SelectAll();

                if (prompt.Status != PromptStatus.OK) return;
                SelectionSet selectionSet = prompt.Value;

                foreach (SelectedObject obj in selectionSet)
                {
                    DBObject databaseObject = transaction.GetObject(obj.ObjectId, OpenMode.ForRead);
                    if (!databaseObject.ExtensionDictionary.IsNull)
                    {
                        ObjectId extId = databaseObject.ExtensionDictionary;

                        DBDictionary dbExt = (DBDictionary)transaction.GetObject(extId, OpenMode.ForRead);

                        if (dbExt.Contains("CustomProp"))
                        {
                            dbExt.UpgradeOpen();
                            dbExt.Remove("CustomProp");
                        }
                    }
                }

                transaction.Commit();
            }
        }
    }
}