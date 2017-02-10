﻿using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
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

        [CommandMethod("CreateFence", CommandFlags.Modal)]
        public void CreateFence()
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;

            Editor editor = _document.Editor;
            PromptSelectionResult selAll = editor.GetSelection();
            SelectionSet selectionSet = selAll.Value;

            List<Point2d> points = new List<Point2d>();


            using (Transaction transaction = _document.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in selectionSet.GetObjectIds())
                {
                    Polyline pl = (Polyline) transaction.GetObject(id, OpenMode.ForRead);

                    for (int j = 0; j < pl.NumberOfVertices; j++)
                    {
                        Point2d pt = pl.GetPoint2dAt(j);
                        points.Add(pt);
                    }
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        int[] segments = Divide((int) points[i].GetDistanceTo(points[i + 1]), i, points.Count - 1);
                        int dist = 0;
                        for (int k = 0; k < segments.Length - 1; k++)
                        {
                            dist += segments[k];
                            Drawer(points[i], points[i + 1], dist);
                        }
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
        }

        public int Round(int i)
        {
            if (i % 10 < 5)
                return i - i % 10;
            return i - i % 10 + 10;
        }

        public void DrawBar(Point2d p, double ang)
        {
            using (Transaction acTrans = _document.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(_database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec =
                    acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


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
                acTrans.AddNewlyCreatedDBObject(bar, true);

                acTrans.Commit();
            }
        }
    }
}