using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Fences
{
    public class Dimension
    {
        //TODO Заставить адекватно работать 

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
                    int n = 200;
                    acRotDim.XLine1Point = new Point3d(p1.X, p1.Y, 0);
                    acRotDim.XLine2Point = new Point3d(p2.X, p2.Y, 0);
                    acRotDim.Rotation = p1.GetVectorTo(p2).Angle;
                    Vector3d vector = acRotDim.XLine2Point.GetVectorTo(acRotDim.XLine1Point).GetNormal().MultiplyBy(n);
                    acRotDim.DimLinePoint = acRotDim.XLine2Point.Add(vector.RotateBy(Math.PI / 2, new Vector3d(0, 0, 1)));


                    acRotDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    if (acBlkTblRec != null) acBlkTblRec.AppendEntity(acRotDim);
                    acTrans.AddNewlyCreatedDBObject(acRotDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
                
    }
}