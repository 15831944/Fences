using System.Diagnostics;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Fences
{
    public class FileDatabase
    {
        public static void SaveToDB()
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

                //now we will have extId...
                DBDictionary dbExt =
                    (DBDictionary) tr.GetObject(extId, OpenMode.ForRead);

                //if not present add the data
                if (!dbExt.Contains("TEST"))
                {
                    dbExt.UpgradeOpen();
                    Xrecord xRec = new Xrecord();
                    ResultBuffer rb = new ResultBuffer();
                    rb.Add(new TypedValue(
                        (int) DxfCode.ExtendedDataAsciiString, "Data"));
                    rb.Add(new TypedValue((int) DxfCode.ExtendedDataReal, new double[] {1, 2, 3}));

                    //set the data
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

        public static void GetFromDB()
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
                ObjectId recID = dbExt.GetAt("TEST");

                Xrecord readBack = (Xrecord) tr.GetObject(
                    recID, OpenMode.ForRead);
                foreach (TypedValue value in readBack.Data)
                    Debug.Print("===== OUR DATA: " + value.TypeCode + ". " + value.Value);
            }
        }
    }
}