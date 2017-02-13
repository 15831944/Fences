using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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
        private Database _database;
        private Document _document;
        // private Editor _editor;

        [CommandMethod("CreateFence", CommandFlags.Modal)]
        public void CreateFence()
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;

            Editor editor = _document.Editor;
            PromptSelectionResult selAll = editor.GetSelection();
            SelectionSet selectionSet = selAll.Value;

            List<Point2d> points = new List<Point2d>();

            if (selAll.Status == PromptStatus.OK)
                using (Transaction transaction = _document.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in selectionSet.GetObjectIds())
                        //TODO Сейчас при выделении нескольких полилиний, получается ерунда
                    {
                        Polyline pl = (Polyline) transaction.GetObject(id, OpenMode.ForRead);
                            //TODO Добавить случай для линии

                        for (int j = 0; j < pl.NumberOfVertices; j++)
                        {
                            Point2d pt = pl.GetPoint2dAt(j);
                            points.Add(pt);
                        }
                        for (int i = 0; i < points.Count - 1; i++)
                        {
                            int[] segments = Divide((int) points[i].GetDistanceTo(points[i + 1]), i, points.Count - 1);
                            int dist = 0;
                            int barnum = 0;
                            for (int k = 0; k < segments.Length - 1; k++)
                            {
                                dist += segments[k];
                                Drawer(points[i], points[i + 1], dist);
                                barnum++;
                            }
                            ToFile(id.ToString(), points[i].GetDistanceTo(points[i + 1]), barnum);
                        }
                    }
                    transaction.Commit();
                }
        }

        public static int[] Divide(int lenght, int index, int n)
        {
            if (lenght < 200)
                throw new ArgumentException("Такой длины не бывает: " + lenght);
            int firstLen = 150;
            int lastLen = 150;

            if (index == 0)
                firstLen = 100;
            if (index == n - 1)
                lastLen = 100;

            if (lenght < firstLen + 190 + lastLen)
            {
                if (index == 0)
                    return new[] {firstLen, lenght - firstLen};
                return new[] {lenght - lastLen, lastLen};
            }
            int middleLen = lenght - firstLen - lastLen;
            int numSeg = middleLen % 900 == 0 ? middleLen / 900 : middleLen / 900 + 1;
            int minSegLenght = middleLen / numSeg / 10 * 10;
            int rest = middleLen - numSeg * minSegLenght;
            int[] result = new int[numSeg + 2];
            result[0] = firstLen;
            result[result.Length - 1] = lastLen;
            for (int i = 1; i < result.Length - 1; i++)
            {
                result[i] = minSegLenght;
                int curRest = Math.Min(rest, 10);
                result[i] += curRest;
                rest -= curRest;
            }
            return result;
        }

        public Point2d MoveDist(Point2d p1, Point2d p2, double dist)
        {
            Vector2d p12 = p1.GetVectorTo(p2);
            return p1.Add(p12.GetNormal().MultiplyBy(dist));
        }

        public void Drawer(Point2d p1, Point2d p2, double dist)
        {
            DrawBar(MoveDist(p1, p2, dist), p1.GetVectorTo(p2).Angle);
            // Dim(p1, p2);
        }

        public int Round(int i)
        {
            if (i % 10 < 5)
                return i - i % 10;
            return i - i % 10 + 10;
        }

        public void DrawBar(Point2d p, double ang)
        {
            using (Transaction transaction = _document.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = transaction.GetObject(_database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec =
                    transaction.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                ChangeLayer(transaction, "Опорная плита стойки");

                double w = 180;
                double h = 120;

                Polyline bar = new Polyline();
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-w / 2, h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-w / 2, -h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, -h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, h / 2)), 0, 0, 0);

                bar.Closed = true;


                Matrix3d curUcsMatrix = _document.Editor.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUcs = curUcsMatrix.CoordinateSystem3d;

                bar.TransformBy(Matrix3d.Rotation(ang, curUcs.Zaxis, new Point3d(p.X, p.Y, 0)));

                acBlkTblRec.AppendEntity(bar);
                transaction.AddNewlyCreatedDBObject(bar, true);

                ChangeLayer(transaction, "Стойки ограждений");

                Polyline rack = new Polyline();
                rack.AddVertexAt(0, p.Add(new Vector2d(-16, 10.4)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-10.4, 16)), 0.414213562373095, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(10.4, 16)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(16, 10.4)), 0.414213562373095, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(16, -10.4)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(10.4, -16)), 0.414213562373095, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-10.4, -16)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-16, -10.4)), 0.414213562373095, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-16, 10.4)), 0, 0, 0);

                rack.Closed = true;

                rack.TransformBy(Matrix3d.Rotation(ang, curUcs.Zaxis, new Point3d(p.X, p.Y, 0)));

                acBlkTblRec.AppendEntity(rack);
                transaction.AddNewlyCreatedDBObject(rack, true);

                DBObjectCollection acDbObjColl = rack.GetOffsetCurves(4);

                foreach (Entity acEnt in acDbObjColl)
                {
                    acBlkTblRec.AppendEntity(acEnt);
                    transaction.AddNewlyCreatedDBObject(acEnt, true);
                }

                transaction.Commit();
            }
        }

        public void ChangeLayer(Transaction acTrans, string sLayerName)
        {
            LayerTable lt = (LayerTable) acTrans.GetObject(_database.LayerTableId, OpenMode.ForRead);
            if (lt.Has(sLayerName))
            {
                _database.Clayer = lt[sLayerName];
            }
            else
            {
                LayerTableRecord ltr = new LayerTableRecord();
                ltr.Name = sLayerName;

                if (sLayerName == "Опорная плита стойки")
                {
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 50);
                    ltr.LineWeight = LineWeight.LineWeight018;
                }

                if (sLayerName == "Стойки ограждений")
                {
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 70);
                    ltr.LineWeight = LineWeight.LineWeight040;
                }

                lt.UpgradeOpen();
                ObjectId ltId = lt.Add(ltr);
                acTrans.AddNewlyCreatedDBObject(ltr, true);

                _database.Clayer = ltId;
            }
        }

        //TODO Заставить адекватно работать 
/*
        public void Dim(Point2d p1, Point2d p2)
        {
            using (Transaction acTrans = _database.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(_database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (RotatedDimension acRotDim = new RotatedDimension())
                {
                    acRotDim.XLine1Point = new Point3d(p1.X, p1.Y, 0);
                    acRotDim.XLine2Point = new Point3d(p2.X, p2.Y, 0);
                    acRotDim.Rotation = p1.GetVectorTo(p2).Angle;
                    acRotDim.Annotative = AnnotativeStates.True;

                 //   acRotDim.DimLinePoint = new Point3d(20,20,0);
                    acRotDim.DimensionStyle = _database.Dimstyle;

                    acBlkTblRec.AppendEntity(acRotDim);
                    acTrans.AddNewlyCreatedDBObject(acRotDim, true);
                }

                acTrans.Commit();
            }
        }
        */

        public void ToFile(string id, double length, int pilnum) // HACK Временный говнокод, нужно улучшить
        {
            int barnum = (int) Math.Ceiling(length / 100 - pilnum);

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string path = @"C:\ToFile\table.xls";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("#\tID\tLength\tNumber of pillars\tNumber of bars");
                    sw.WriteLine(1 + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
                }
            }
            else
            {
                string x;

                string text = File.ReadLines(path).Last();
                string[] bits = text.Split('\t');


                x = bits[0];
                ed.WriteMessage(x);

                int num = int.Parse(x);
                if ("id" + id != bits[1])
                    num++;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(num + "\tid" + id + "\t" + length + "\t" + pilnum + "\t" + barnum);
                }
            }
        }

        //TODO Запилить класс, делающий расчеты
    }
}