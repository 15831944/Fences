using System.Collections.Generic;
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
    public abstract class MyCommands
    {
        //private const string vert = "pl{0}.AddVertexAt({1}, new Point2d({2}, {3})," + "{4}, {5}, {6});\r\n";

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
            Dictionary<double, double> cord = new Dictionary<double, double>();

            using (var transaction = AcDoc.TransactionManager.StartTransaction())
            {
                foreach (var id in selectionSet.GetObjectIds())
                {

                    var pl = (Polyline) transaction.GetObject(id, OpenMode.ForRead);

                    for (var j = 0; j < pl.NumberOfVertices; j++)
                    {
                        var pt = pl.GetPoint2dAt(j);
                    /*    double bul = pl.GetBulgeAt(j),
                            sWid = pl.GetStartWidthAt(j),
                            eWid = pl.GetEndWidthAt(j);
                        editor.WriteMessage(string.Format(vert, id, j, pt.X, pt.Y, bul, sWid, eWid));
                    */
                     //   DrawBar(pt.X, pt.Y, AcDoc.Database);
                        cord.Add(pt.X, pt.Y);
                    }
                }
                transaction.Commit();
            }
        }

        public void DrawBar(double x, double y, Database d)
        {
            using (var acTrans = d.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(d.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec =
                    acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                /*using (var acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(x, y, 0);
                    acCirc.Radius = 4.25;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }*/

                double xleft = x + 60;
                double yleft = y + 90;

                Polyline bar = new Polyline();
                bar.AddVertexAt(0, new Point2d(xleft, yleft), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft, yleft - 180), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft - 120, yleft - 180), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft - 120, yleft), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft, yleft), 0, 0, 0);

                acBlkTblRec.AppendEntity(bar);
                acTrans.AddNewlyCreatedDBObject(bar, true);

                acTrans.Commit();
            }
        }

        public bool isHorizontal(double x1, double x2)
        {
            return true;
        }
        /*
        public static Dictionary<double, double> Xcord(int i)
        {
            Dictionary<double, double> cord = new Dictionary<double, double>();
            for (int j = 0; j < i; j++)
            {
                
            }
            return cord;
        }
        */

    }
}