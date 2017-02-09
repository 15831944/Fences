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
                double angle;
                int gap;
                for (var j = 0; j < segNum.Length - 1; j++)
                {
                    segNum[j] =
                        Math.Sqrt(Math.Pow((double) xcord[j + 1] - (double) xcord[j], 2) +
                                  Math.Pow((double) ycord[j + 1] - (double) ycord[j], 2));
                    editor.WriteMessage(string.Format(segNum[j] + " "));

                    Point2d pt1 = new Point2d((double) xcord[j], (double)ycord[j]);
                    Point2d pt2 = new Point2d((double) xcord[j + 1], (double) ycord[j + 1]);
                    angle = pt1.GetVectorTo(pt2).Angle;

                    gap = (int) segNum[j];
                    if (gap % 10 != 0) // Надо заменить на что-нибудь 
                    {
                        gap = Round(gap);
                    }
                    int gapnum = gap / 800 + 1;

                    DrawBar(MiddlePoint((double) xcord[j + 1], (double) xcord[j]),
                        MiddlePoint((double) ycord[j + 1], (double) ycord[j]), AcDoc.Database, AcDoc, angle);
                }

                transaction.Commit();
            }
        }

        public int Round(int i)
        {
            if (i % 10 < 5)
            {
                return i - (i % 10) ;
            }
            else
            {
                return i - (i % 10) + 10;
            }
            
        }

        public double MiddlePoint(double x1, double x2)
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