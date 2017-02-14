namespace Fences
{
    public class Dimension
    {
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
    }
}