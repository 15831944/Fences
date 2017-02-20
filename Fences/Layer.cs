using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace Fences
{
    public class Layer
    {
        public static LayerTableRecord CreateLayer(string name, Color color, LineWeight weight)
        {
            LayerTableRecord layer = new LayerTableRecord
            {
                Name = name,
                Color = color,
                LineWeight = weight
            };
            return layer;
        }

        public static void ChangeLayer(Transaction acTrans, LayerTableRecord ltr, Database database)
        {
            LayerTable lt = (LayerTable)acTrans.GetObject(database.LayerTableId, OpenMode.ForRead);
            if (lt.Has(ltr.Name))
            {
                database.Clayer = lt[ltr.Name];
            }
            else
            {
                lt.UpgradeOpen();
                ObjectId ltId = lt.Add(ltr);
                acTrans.AddNewlyCreatedDBObject(ltr, true);
                database.Clayer = ltId;
            }
        }
    }
}