using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace Fences
{
    interface FencesAcad
    {
        ISet<ObjectId> GetSelectedFenceIds();

        ICollection<Polyline> GetFences(ISet<ObjectId> FenceIds);
    }
}
