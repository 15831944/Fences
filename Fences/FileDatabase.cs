using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Fences
{
    public class FileDatabase
    {
        //Creates a custom property in object contains number of floors and bars
        public void SaveToDB(ObjectId objectId, int floorNum, int numBars)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;

            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                DBObject databaseObject = transaction.GetObject(objectId, OpenMode.ForRead);

                ObjectId extId = databaseObject.ExtensionDictionary;

                if (extId == ObjectId.Null)
                {
                    databaseObject.UpgradeOpen();
                    databaseObject.CreateExtensionDictionary();
                    extId = databaseObject.ExtensionDictionary;
                }

                DBDictionary dbExt = (DBDictionary) transaction.GetObject(extId, OpenMode.ForRead);

                if (!dbExt.Contains("CustomProp"))
                {
                    dbExt.UpgradeOpen();
                    Xrecord xRec = new Xrecord();
                    ResultBuffer rb = new ResultBuffer
                    {
                        new TypedValue((int) DxfCode.ExtendedDataAsciiString, floorNum.ToString()),
                        new TypedValue((int) DxfCode.ExtendedDataAsciiString, numBars.ToString())
                    };

                    xRec.Data = rb;
                    dbExt.SetAt("CustomProp", xRec);
                    transaction.AddNewlyCreatedDBObject(xRec, true);
                }

                transaction.Commit();
            }
        }

        public void GetTotalNumbers(List<Polyline> list)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;

            for (int i = 0; i < list.Count; i++)
            {
                int numfloor = 0;
                int numbars = 0;
                using (Transaction transaction = database.TransactionManager.StartTransaction())
                {
                    DBObject databaseObject = transaction.GetObject(list[i].ObjectId, OpenMode.ForRead);
                    ObjectId extId = databaseObject.ExtensionDictionary;
                    DBDictionary dbExt = (DBDictionary)transaction.GetObject(extId, OpenMode.ForRead);

                    if (dbExt.Contains("CustomProp"))
                    {
                        ObjectId recID = dbExt.GetAt("CustomProp");
                        Xrecord readBack = (Xrecord)transaction.GetObject(recID, OpenMode.ForRead);
                        numfloor = int.Parse(readBack.Data.AsArray()[0].Value.ToString());
                        numbars = int.Parse(readBack.Data.AsArray()[1].Value.ToString());
                    }
                    transaction.Commit();
                }
                Properties.Settings.Default.CounterLength += list[i].Length * numfloor;
                Properties.Settings.Default.CounterPils += numfloor * numbars;
            }
        }
    }
}