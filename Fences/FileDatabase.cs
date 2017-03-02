using System.Diagnostics;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Fences
{
    public class FileDatabase
    {
        //private const string ExtDbPos = "TEST";
        public void SaveToDB(ObjectId objectId, int floorNum, int numBars, int numLine)
        {
            //MessageBox.Show(numLine.ToString());
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            //Editor editor = document.Editor;

            string[] numLineNames = new string[numLine - 1];
            for (int i = 0; i < numLine; i++)
            {
                numLineNames[i] = (i + 1).ToString();
            }

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                DBObject dbObject = transaction.GetObject(objectId, OpenMode.ForRead);

                ObjectId extId = dbObject.ExtensionDictionary;

                if (extId == ObjectId.Null)
                {
                    dbObject.UpgradeOpen();
                    dbObject.CreateExtensionDictionary();
                    extId = dbObject.ExtensionDictionary;
                }

                DBDictionary dbExt = (DBDictionary) transaction.GetObject(extId, OpenMode.ForRead);

                //int floornum = SimpleFences.GetFloorNum(ObjectId);
                //int pilnum = SimpleFences.GetPilNum(ObjectId);
                for (int i = 0; i < numLine; i++)
                {
                    if (!dbExt.Contains(numLineNames[i])) //TODO Add here calculation of bar/pil numbers
                    {
                        dbExt.UpgradeOpen();
                        Xrecord xRec = new Xrecord();
                        ResultBuffer rb = new ResultBuffer
                    {
                        new TypedValue((int) DxfCode.ExtendedDataAsciiString, floorNum.ToString()),
                        new TypedValue((int) DxfCode.ExtendedDataAsciiString, numBars.ToString())
                    };

                        xRec.Data = rb;

                        dbExt.SetAt(numLineNames[i], xRec);

                        transaction.AddNewlyCreatedDBObject(xRec, true);

                        ObjectId recID = dbExt.GetAt(numLineNames[i]);

                        Xrecord readBack = (Xrecord)transaction.GetObject(recID, OpenMode.ForRead);
                        foreach (TypedValue value in readBack.Data)
                        {
                            //System.Diagnostics.Debug.Print("===== OUR DATA: " + value.TypeCode.ToString() + ". " + value.Value.ToString());
                            MessageBox.Show($"{value.TypeCode}.{value.Value}");
                        }

                    }
                }
                transaction.Commit();
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
                ObjectId recId = dbExt.GetAt("1");

                Xrecord readBack = (Xrecord) tr.GetObject(recId, OpenMode.ForRead);
                /*
                foreach (TypedValue value in readBack.Data)
                    Debug.Print("===== OUR DATA: " + value.TypeCode + ". " + value.Value);
                    */
                string[] valTest = new string[readBack.Data.AsArray().Length];
                for (int i = 0; i < readBack.Data.AsArray().Length; i++)
                {
                    valTest[i] = readBack.Data.AsArray()[i].Value.ToString();
                }
            }
        }
    }
}