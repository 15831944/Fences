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

            var xcord = new ArrayList();
            var ycord = new ArrayList();

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
                        xcord.Add(pt.X);
                        ycord.Add(pt.Y);
                    }
                }

                var segNum = new double[xcord.Count];
                var check = new bool[segNum.Length - 1];
                for (var j = 0; j < segNum.Length - 1; j++)
                    check[j] = false;
                double angle;
                int gap;
                for (var j = 0; j < segNum.Length - 1; j++)
                {
                    segNum[j] =
                        Math.Sqrt(Math.Pow((double) xcord[j + 1] - (double) xcord[j], 2) +
                                  Math.Pow((double) ycord[j + 1] - (double) ycord[j], 2));
                    editor.WriteMessage(string.Format(segNum[j] + " "));

                    var pt1 = new Point2d((double) xcord[j], (double) ycord[j]);
                    var pt2 = new Point2d((double) xcord[j + 1], (double) ycord[j + 1]);
                    angle = pt1.GetVectorTo(pt2).Angle + Math.PI / 2;

                    if (j == 0 && check[j] == false)
                    {
                        Drawer((double) xcord[j], (double) ycord[j], (double) xcord[j + 1],
                            (double) ycord[j + 1], 100, AcDoc.Database, AcDoc, angle);

                        if (segNum[j] >= 430)
                        {
                            Drawer((double) xcord[j], (double) ycord[j], (double) xcord[j + 1],
                                (double) ycord[j + 1], segNum[j] - 150, AcDoc.Database, AcDoc, angle);

                            if (segNum[j] - 250 > 900)
                            {
                                int div = ((int) segNum[j] - 250) / 900; //Делим на 900 и округляем вверх
                                if ((int) segNum[j] / 900 != 0)
                                    div++; //div - это количество отрезков

                                int[] dd = new int[div];
                                for (int k = 0; k < div - 1; k++)
                                {
                                    dd[k] = ((int) segNum[j] - 250) / div;
                                }
                                int m = 0;
                                for (int t = (((int)segNum[j] - 250) / div) / 10; t == 0; t--)
                                {
                                    m++;
                                    if (m == div - 1)
                                    {
                                        m = 0;

                                    }
                                    dd[m] = dd[m] + 10;
                                }

                              //  Drawer((double) xcord[j], (double) ycord[j], (double) xcord[j + 1],
                           //         (double) ycord[j + 1], 100, AcDoc.Database, AcDoc, angle);
                                double px1 = (double) xcord[j];

                                double py1 = (double) ycord[j];
                                double px2 = GetPoint(px1, py1, (double) xcord[j + 1], (double) ycord[j + 1], 100).Item1;
                                double py2 = GetPoint(px1, py1, (double) xcord[j + 1], (double) ycord[j + 1], 100).Item2;

                                for (int z = 0; z < div; z++)
                                {
                                    editor.WriteMessage(string.Format(dd[z] + " "));
                                    Drawer(px1, py1, px2, py2, dd[z], AcDoc.Database, AcDoc, angle);
                               //     DrawBar(GetPoint(px1, py1, px2, py2, segments + 10).Item1, GetPoint(px1, py1, px2, py2, segments + 10).Item2, AcDoc.Database, AcDoc, angle);
                                    px1 = GetPoint(px1, py1, px2, py2, dd[z]).Item1;
                                    py1 = GetPoint(px1, py1, px2, py2, dd[z]).Item2;
                                }
                            }
                        }

                        check[j] = true;
                    }
                    else
                    {
                        if (j == segNum.Length - 2 && segNum[j] < 430 && check[j] == false)
                        {
                            Drawer((double) xcord[j], (double) ycord[j], (double) xcord[j + 1],
                                (double) ycord[j + 1], segNum[segNum.Length - 2] - 100, AcDoc.Database, AcDoc, angle);
                            check[j] = true;
                        }
                    }
               }

                transaction.Commit();
            }
        }

        public static Tuple<double, double> GetPoint(double px1, double py1, double px2, double py2, double dist)
        {
            var lmin = length(px1, py1, px2, py2);
            var lx = pointA(px1, px2, lmin, dist);
            var ly = pointA(py1, py2, lmin, dist);
            return Tuple.Create(lx, ly);
        }

        public static void Drawer(double px1, double py1, double px2, double py2, double dist, Database d, Document doc,
            double ang)
        {
            var ax = GetPoint(px1, py1, px2, py2, dist).Item1;
            var ay = GetPoint(px1, py1, px2, py2, dist).Item2;
            DrawBar(ax, ay, d, doc, ang);
        }

        public int Round(int i)
        {
            if (i % 10 < 5)
                return i - i % 10;
            return i - i % 10 + 10;
        }

        public static double length(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public static double pointA(double x1, double x2, double l, double a)
        {
            var xa = a * (x2 - x1) / l + x1;

            return xa;
        }


        public static void DrawBar(double x, double y, Database d, Document doc, double ang)
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

                var curUcsMatrix = doc.Editor.CurrentUserCoordinateSystem;
                var curUcs = curUcsMatrix.CoordinateSystem3d;

                bar.TransformBy(Matrix3d.Rotation(ang, curUcs.Zaxis, new Point3d(x, y, 0)));

                acBlkTblRec.AppendEntity(bar);
                acTrans.AddNewlyCreatedDBObject(bar, true);

                acTrans.Commit();
            }
        }
    }
}