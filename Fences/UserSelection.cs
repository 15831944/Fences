using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Fences.Properties;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Fences
{
    public class UserSelection
    {
        private readonly FencesAcad _fencesAcad = new RealFencesAcad(Application.DocumentManager.MdiActiveDocument);
        private readonly FileDatabase _fileDatabase = new FileDatabase();
        private readonly TableCreator _tableCreator = new TableCreator();
        private Database _database;

        private Document _document;
        private Editor _editor;

        private int _guessnum = 1;
        private int _numbars;

        public void SelectPolyline()
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;
            ISet<ObjectId> ids = _fencesAcad.GetSelectedFenceIds();

            //TODO Have to make first layer current

            foreach (Polyline pl in _fencesAcad.GetFences(ids))
            {
                GetNumFloor();
                List<Point2d> points = new List<Point2d>();

                for (int j = 0; j < pl.NumberOfVertices; j++)
                {
                    Point2d pt = pl.GetPoint2dAt(j);
                    points.Add(pt);
                }
                Fence fence = new Fence();

                for (int i = 0; i < points.Count - 1; i++)
                {
                    int[] segments = PositionCalculator.Divide((int) points[i].GetDistanceTo(points[i + 1]), i,
                        points.Count - 1);
                    int dist = 0;
                    Point2d[] pills = new Point2d[segments.Length - 1];

                    for (int k = 0; k < segments.Length - 1; k++)
                    {
                        dist += segments[k];
                        pills[k] = MoveDist(points[i], points[i + 1], dist);
                        DrawBar(pills[k], points[i].GetVectorTo(points[i + 1]).Angle);
                    }

                    FenceEntry entry = new FenceEntry(new LineSegment2d(points[i], points[i + 1]), pills);
                    fence.AddEntry(entry);

                    _numbars += segments.Length - 1;
                }
                using (Transaction transaction = _database.TransactionManager.StartTransaction())
                {
                    Layer.ChangeLayer(transaction,
                        Layer.CreateLayer("КМ-РАЗМ", Color.FromColorIndex(ColorMethod.ByAci, 1),
                            LineWeight.LineWeight020), _database);
                    transaction.Commit();
                }

                foreach (FenceEntry entry in fence.GetEntries())
                foreach (LineSegment2d segment in entry.SplitByPills())
                    Dimension.Dim(segment);

                _fileDatabase.SaveToDB(pl.ObjectId, _guessnum, _numbars);
                Settings.Default.NumEnd += 2 * _guessnum;
                _numbars = 0;
            }
        }

        private void GetNumFloor()
        {
            PromptIntegerOptions options = new PromptIntegerOptions("");

            options.Message = "\nВведите количество этажей или ";
            options.AllowZero = false;
            options.AllowNegative = false;
            options.AllowNone = true;
            options.DefaultValue = _guessnum;

            PromptIntegerResult result = _document.Editor.GetInteger(options);
            if (result.Value != _guessnum)
                _guessnum = result.Value;
        }

        private Point2d MoveDist(Point2d p1, Point2d p2, double dist)
        {
            Vector2d p12 = p1.GetVectorTo(p2);
            return p1.Add(p12.GetNormal().MultiplyBy(dist));
        }

        private void DrawBar(Point2d p, double ang)
        {
            using (Transaction transaction = _document.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = transaction.GetObject(_database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord blockTableRecord =
                    transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as
                        BlockTableRecord;

                Layer.ChangeLayer(transaction,
                    Layer.CreateLayer("Опорная плита стойки", Color.FromColorIndex(ColorMethod.ByAci, 50),
                        LineWeight.LineWeight018), _database);

                const double w = 200;
                const double h = 120;

                Polyline bar = new Polyline();

                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, h - 35)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-w / 2, h - 35)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-w / 2, -h + 85)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, -h + 85)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, h - 35)), 0, 0, 0);

                /*
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-w / 2, h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(-w / 2, -h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, -h / 2)), 0, 0, 0);
                bar.AddVertexAt(0, p.Add(new Vector2d(w / 2, h / 2)), 0, 0, 0);
                */

                bar.Closed = true;


                Matrix3d matrix3D = _document.Editor.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUcs = matrix3D.CoordinateSystem3d;

                bar.TransformBy(Matrix3d.Rotation(ang, curUcs.Zaxis, new Point3d(p.X, p.Y, 0)));

                if (blockTableRecord != null)
                {
                    blockTableRecord.AppendEntity(bar);
                    transaction.AddNewlyCreatedDBObject(bar, true);
                }

                Layer.ChangeLayer(transaction,
                    Layer.CreateLayer("Стойки ограждений", Color.FromColorIndex(ColorMethod.ByAci, 70),
                        LineWeight.LineWeight040), _database);

                const double wr1 = 50;
                const double wr2 = 30.4;
                const double hr1 = 10.8;
                const double hr2 = 30;
                const double rad = 0.414213562373095;

                Polyline rack = new Polyline();
                rack.AddVertexAt(0, p.Add(new Vector2d(-wr1 / 2, hr1 / 2)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-wr2 / 2, hr2 / 2)), rad, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(wr2 / 2, hr2 / 2)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(wr1 / 2, hr1 / 2)), rad, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(wr1 / 2, -hr1 / 2)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(wr2 / 2, -hr2 / 2)), rad, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-wr2 / 2, -hr2 / 2)), 0, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-wr1 / 2, -hr1 / 2)), rad, 0, 0);
                rack.AddVertexAt(0, p.Add(new Vector2d(-wr1 / 2, hr1 / 2)), 0, 0, 0);

                rack.Closed = true;

                rack.TransformBy(Matrix3d.Rotation(ang + Math.PI / 2, curUcs.Zaxis, new Point3d(p.X, p.Y, 0)));

                if (blockTableRecord != null)
                {
                    blockTableRecord.AppendEntity(rack);
                    transaction.AddNewlyCreatedDBObject(rack, true);

                    DBObjectCollection acDbObjColl = rack.GetOffsetCurves(-4);

                    foreach (Entity acEnt in acDbObjColl)
                    {
                        blockTableRecord.AppendEntity(acEnt);
                        transaction.AddNewlyCreatedDBObject(acEnt, true);
                    }
                }
                transaction.Commit();
            }
        }

        public void GetDataFromSelection() //TODO Finish
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;
            _editor = _document.Editor;
            _editor.WriteMessage("Выделите секцию/секции, для которых нужно создать таблицу:");

            ISet<ObjectId> ids = _fencesAcad.GetSelectedFenceIds();

            //TODO Need to make first layer current

            List<Polyline> list = new List<Polyline>();

            foreach (Polyline pl in _fencesAcad.GetFences(ids))
                if (pl.Layer == "КМ-ОСН") //TODO: Bad solution
                    list.Add(pl);
            _fileDatabase.GetTotalNumbers(list);
            _tableCreator.Calculator(Settings.Default.CounterLength, Settings.Default.CounterPils);

            _tableCreator.CreateTable(Settings.Default.total60X30X4, Settings.Default.total40X4,
                Settings.Default.totalT10,
                Settings.Default.totalT4, Settings.Default.totalT14);
        }
    }
}