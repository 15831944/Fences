using System;
using System.Collections;
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
    public class MyCommands
    {

        private Database database;
        private Document document;

        [CommandMethod("CreateFence", CommandFlags.Modal)]
        public void CreateFence()
        {
            document = Application.DocumentManager.MdiActiveDocument;
            database = document.Database;

            var editor = document.Editor;
            var selAll = editor.GetSelection();
            var selectionSet = selAll.Value;

            //var xcord = new ArrayList();
            //var ycord = new ArrayList();
            List<Point2d> points = new List<Point2d>();  
            

            using (var transaction = document.TransactionManager.StartTransaction())
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
                        points.Add(pt);
                    }
                }

                var segNum = new double[points.Count];
                var check = new bool[segNum.Length - 1];
                for (var j = 0; j < segNum.Length - 1; j++)
                    check[j] = false;
                double angle;
                int gap;
                for (var j = 0; j < segNum.Length - 1; j++)
                { 
                    segNum[j] =points[j].GetDistanceTo(points[j+1]);
                    editor.WriteMessage(string.Format(segNum[j] + " "));

                    angle = points[j].GetVectorTo(points[j+1]).Angle + Math.PI / 2;

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
                                double px2 = MoveDist(px1, py1, (double) xcord[j + 1], (double) ycord[j + 1], 100).Item1;
                                double py2 = MoveDist(px1, py1, (double) xcord[j + 1], (double) ycord[j + 1], 100).Item2;

                                for (int z = 0; z < div; z++)
                                {
                                    editor.WriteMessage(string.Format(dd[z] + " "));
                                    Drawer(px1, py1, px2, py2, dd[z], AcDoc.Database, AcDoc, angle);
                               //     DrawBar(MoveDist(px1, py1, px2, py2, segments + 10).Item1, MoveDist(px1, py1, px2, py2, segments + 10).Item2, AcDoc.Database, AcDoc, angle);
                                    px1 = MoveDist(px1, py1, px2, py2, dd[z]).Item1;
                                    py1 = MoveDist(px1, py1, px2, py2, dd[z]).Item2;
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

        public Point2d MoveDist(Point2d p1, Point2d p2, double dist)
        {
            Vector2d p12 = p1.GetVectorTo(p2);
            return p1.Add(p12.GetNormal().MultiplyBy(dist));
        }

        public void Drawer(Point2d p1, Point2d p2, double dist)
        {
            DrawBar(MoveDist(p1, p2, dist), p1.GetVectorTo(p2).Angle);
        }

        public int Round(int i)
        {
            if (i % 10 < 5)
                return i - i % 10;
            return i - i % 10 + 10;
        }

        public void DrawBar(Point2d p, double ang)
        {
            using (var acTrans = document.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec =
                    acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //var xleft = x + 60;
        //        double w = 180;
          //      double h = 120;
                Point2d right = p.Add(new Vector2d(120, 180));

                Rectangle3d rect = new Rectangle3d();
                var bar = new Polyline();
                bar.AddVertexAt(0, right, 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-120, 180)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-120, -180)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(120, -180)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(120, 180)), 0, 0, 0);

                bar.Closed = true;

                var curUcsMatrix = document.Editor.CurrentUserCoordinateSystem;
                var curUcs = curUcsMatrix.CoordinateSystem3d;

                bar.TransformBy(Matrix3d.Rotation(ang, curUcs.Zaxis, new Point3d(x, y, 0)));

                acBlkTblRec.AppendEntity(bar);
                acTrans.AddNewlyCreatedDBObject(bar, true);

                acTrans.Commit();
            }
        }
    }
}