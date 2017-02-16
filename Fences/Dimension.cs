using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Fences
{
    public class Dimension
    {
        //TODO Заставить адекватно работать 
        private static Database _database;
                /*public static void Dim(Point2d p1, Point2d p2)
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
                            //acRotDim.Annotative = AnnotativeStates.True;

                         //   acRotDim.DimLinePoint = new Point3d(20,20,0);
                            //acRotDim.DimensionStyle = _database.Dimstyle;

                            acBlkTblRec.AppendEntity(acRotDim);
                            acTrans.AddNewlyCreatedDBObject(acRotDim, true);
                        }

                        acTrans.Commit();
                    }
                }*/

        public static void Dim(Point2d p1, Point2d p2)
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the rotated dimension
                using (RotatedDimension acRotDim = new RotatedDimension())
                {
                    acRotDim.XLine1Point = new Point3d(p1.X, p1.Y, 0);
                    acRotDim.XLine2Point = new Point3d(p2.X, p2.Y, 0);
                    acRotDim.Rotation = p1.GetVectorTo(p2).Angle;
                    acRotDim.DimLinePoint = acRotDim.XLine2Point.RotateBy(Math.PI / 10, acRotDim.XLine1Point.GetVectorTo(acRotDim.XLine2Point), acRotDim.XLine1Point);

                   // acRotDim.DimLinePoint = new Point3d(0, 5, 0);
                    acRotDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acRotDim);
                    acTrans.AddNewlyCreatedDBObject(acRotDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
                
    }
}