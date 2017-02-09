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
                double angle;
                for (var j = 0; j < segNum.Length - 1; j++)
                {
                    segNum[j] =
                        Math.Sqrt(Math.Pow((double) Xcord[j + 1] - (double) Xcord[j], 2) +
                                  Math.Pow((double) Ycord[j + 1] - (double) Ycord[j], 2));
                    editor.WriteMessage(string.Format(segNum[j] + " "));

                    Point2d pt1 = new Point2d((double) Xcord[j], (double)Ycord[j]);
                    Point2d pt2 = new Point2d((double) Xcord[j + 1], (double) Ycord[j + 1]);
                    angle = pt1.GetVectorTo(pt2).Angle;

                    DrawBar(middlePoint((double) Xcord[j + 1], (double) Xcord[j]),
                        middlePoint((double) Ycord[j + 1], (double) Ycord[j]), AcDoc.Database, AcDoc, angle);
                }

                transaction.Commit();
            }
        }

        public double middlePoint(double x1, double x2)
        {
            return (x1 + x2) / 2;
        }


        public void DrawBar(double x, double y, Database d, Document doc, double ang)
        {
            using (var acTrans = d.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(d.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec =
                    acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var xleft = x + 60;
                var yleft = y + 90;

                var bar = new Polyline();
                bar.AddVertexAt(0, new Point2d(xleft, yleft), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft, yleft - 180), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft - 120, yleft - 180), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft - 120, yleft), 0, 0, 0);
                bar.AddVertexAt(0, new Point2d(xleft, yleft), 0, 0, 0);

                bar.Closed = true;

                var curUCSMatrix = doc.Editor.CurrentUserCoordinateSystem;
                var curUCS = curUCSMatrix.CoordinateSystem3d;

                bar.TransformBy(Matrix3d.Rotation(ang, curUCS.Zaxis, new Point3d(x, y, 0)));

                acBlkTblRec.AppendEntity(bar);
                acTrans.AddNewlyCreatedDBObject(bar, true);

                acTrans.Commit();
            }
        }

        /*
        [CommandMethod("AngleFromXAxis")]
        public static void AngleFromXAxis()
        {
            Point2d pt1 = new Point2d(2, 5);
            Point2d pt2 = new Point2d(5, 2);

            Application.ShowAlertDialog("Angle from XAxis: " +
                                        pt1.GetVectorTo(pt2).Angle.ToString());
        }
        */
    }
}