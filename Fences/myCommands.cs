using System;
using System.Collections;
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
            //double[] Xcord = new double[100];
            //double[] Ycord = new double[100];
            //  Dictionary<int, double> Ycord = new Dictionary<int, double>();
            var Xcord = new ArrayList();
            var Ycord = new ArrayList();

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
                        Xcord.Add(pt.X);
                        Ycord.Add(pt.Y);
                    }
                }

                var segNum = new double[Xcord.Count];
                for (var j = 0; j < segNum.Length - 1; j++)
                {
                    segNum[j] =
                        Math.Sqrt(Math.Pow((double) Xcord[j + 1] - (double) Xcord[j], 2) +
                                  Math.Pow((double) Ycord[j + 1] - (double) Ycord[j], 2));
                    editor.WriteMessage(string.Format(segNum[j] + " "));
                    DrawBar(middlePoint((double) Xcord[j + 1], (double) Xcord[j]),
                        middlePoint((double) Ycord[j + 1], (double) Ycord[j]), AcDoc.Database);
                }

                transaction.Commit();
            }
        }

        public double middlePoint(double x1, double x2)
        {
            return (x1 + x2) / 2;
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
                /*
                using (var acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(x, y, 0);
                    acCirc.Radius = 4.25;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }
                */

                var xleft = x + 60;
                var yleft = y + 90;

                var bar = new Polyline();
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
    }
}