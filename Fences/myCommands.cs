using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Fences;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(MyCommands))]

namespace Fences
{
    public class MyCommands
    {
        private const string vert = "pl{0}.AddVertexAt({1}, new Point2d({2}, {3})," + "{4}, {5}, {6});\r\n";

        public Document AcDoc
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }

        [CommandMethod("CreateFence", CommandFlags.Modal)]
        public void CreateFence()
        {
            var editor = AcDoc.Editor;
            var selAll = editor.GetSelection();
            var selectionSet = selAll.Value;

            using (var transaction = AcDoc.TransactionManager.StartTransaction())
            {
                foreach (var id in selectionSet.GetObjectIds())
                {
                    var pl = (Polyline) transaction.GetObject(id, OpenMode.ForRead);


                    for (var j = 0; j < pl.NumberOfVertices; j++)
                    {
                        var pt = pl.GetPoint2dAt(j);
                        double bul = pl.GetBulgeAt(j),
                            sWid = pl.GetStartWidthAt(j),
                            eWid = pl.GetEndWidthAt(j);
                        editor.WriteMessage(string.Format(vert, id, j, pt.X, pt.Y, bul, sWid, eWid));
                        DrawCircle(pt.X, pt.Y, AcDoc.Database);
                    }
                }
                transaction.Commit();
            }
        }

        public void DrawCircle(double x, double y, Database d)
        {
            using (var acTrans = d.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(d.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec =
                    acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (var acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(x, y, 0);
                    acCirc.Radius = 4.25;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }
                acTrans.Commit();
            }
        }
    }
}