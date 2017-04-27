using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Fences.Properties;

namespace Fences
{
    public class Cleaner
    {
        public void CleanAllPoly()
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

        public void CleanSettings()
        {
            Settings.Default.CounterLength = 0;
            Settings.Default.CounterPils = 0;
            Settings.Default.total40X4 = 0;
            Settings.Default.total60X30X4 = 0;
            Settings.Default.totalT10 = 0;
            Settings.Default.totalT14 = 0;
            Settings.Default.totalT4 = 0;
        }
    }
}