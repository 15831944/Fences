using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Fences
{
    interface FencesAcad
    {
        ISet<ObjectId> GetSelectedFenceIds();

        ICollection<Polyline> GetFences(ISet<ObjectId> FenceIds);
    }
}
