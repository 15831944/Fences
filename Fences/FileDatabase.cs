using System.Diagnostics;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Fences
{
    public class FileDatabase
    {
        public void SaveToDB()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityResult ers = ed.GetEntity("Select entity to add" +
                                                  " extension dictionary ");
            if (ers.Status != PromptStatus.OK)
                return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBObject dbObj = tr.GetObject(ers.ObjectId,
                    OpenMode.ForRead);

                ObjectId extId = dbObj.ExtensionDictionary;

                if (extId == ObjectId.Null)
                {
                    dbObj.UpgradeOpen();
                    dbObj.CreateExtensionDictionary();
                    extId = dbObj.ExtensionDictionary;
                }

                DBDictionary dbExt =
                    (DBDictionary) tr.GetObject(extId, OpenMode.ForRead);

                if (!dbExt.Contains("TEST"))
                {
                    dbExt.UpgradeOpen();
                    Xrecord xRec = new Xrecord();
                    ResultBuffer rb = new ResultBuffer
                    {
                        new TypedValue(
                            (int) DxfCode.ExtendedDataAsciiString, "Data"),
                        new TypedValue((int) DxfCode.ExtendedDataReal, new double[] {1, 2, 3})
                    };

                    xRec.Data = rb;

                    dbExt.SetAt("TEST", xRec);
                    tr.AddNewlyCreatedDBObject(xRec, true);
                }
                else
                {
                    ed.WriteMessage("entity contains the TEST data\n");
                }


                tr.Commit();
            }
        }
        
        public void GetFromDB()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityResult ers = ed.GetEntity("Select entity to add" +
                                                  " extension dictionary ");
            if (ers.Status != PromptStatus.OK)
                return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBObject dbObj = tr.GetObject(ers.ObjectId,
                    OpenMode.ForRead);

                ObjectId extId = dbObj.ExtensionDictionary;

                if (extId == ObjectId.Null)
                    ed.WriteMessage("У этого объекта нет сохраненных данных");

                DBDictionary dbExt = (DBDictionary) tr.GetObject(extId, OpenMode.ForRead);
                ObjectId recId = dbExt.GetAt("TEST");

                Xrecord readBack = (Xrecord) tr.GetObject(
                    recId, OpenMode.ForRead);
                foreach (TypedValue value in readBack.Data)
                    Debug.Print("===== OUR DATA: " + value.TypeCode + ". " + value.Value);
            }
        }
    }
}