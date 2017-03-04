using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace Fences
{
    public class RealFencesAcad : FencesAcad
    {
        private readonly Document _document;
        private readonly Editor _editor;

        public RealFencesAcad(Document document)
        {
            _document = document;
            _editor = document.Editor;
        }

        public ISet<ObjectId> GetSelectedFenceIds()
        {
            PromptSelectionResult selectionResult = _editor.GetSelection();
            SelectionSet selectionSet = selectionResult.Value;
            if (selectionResult.Status != PromptStatus.OK)
            {
                return new HashSet<ObjectId>();
            }
            ISet<ObjectId> fenceIds = new HashSet<ObjectId>();
            foreach (ObjectId id in selectionSet.GetObjectIds())
            {
                if (id.ObjectClass == RXObject.GetClass(typeof(Polyline)))
                {
                    fenceIds.Add(id);
                }
            }
            return fenceIds;
        }

        public ICollection<Polyline> GetFences(ISet<ObjectId> fenceIds)
        {
            using (Transaction transaction = _document.TransactionManager.StartTransaction())
            {
                ICollection<Polyline> collection = new List<Polyline>();
                foreach (ObjectId id in fenceIds)
                {
                    Polyline pl = (Polyline)transaction.GetObject(id, OpenMode.ForRead);
                    collection.Add(pl);
                }
                transaction.Commit();
                return collection;
            }
        }
    }
}