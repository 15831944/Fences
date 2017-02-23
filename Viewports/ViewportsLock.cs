using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Viewports;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(ViewportsLock))]

namespace Viewports
{
    public class ViewportsLock
    {
        [CommandMethod("LockViewport", CommandFlags.Modal)]
        public void LockViewport()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction transaction = acCurDb.TransactionManager.StartTransaction())
            {
                PromptSelectionResult prompt = acDoc.Editor.SelectAll();

                if (prompt.Status != PromptStatus.OK) return;
                SelectionSet selectionSet = prompt.Value;

                foreach (SelectedObject obj in selectionSet)
                    if (obj.ObjectId.ObjectClass == RXObject.GetClass(typeof(Viewport)))
                    {
                        Viewport viewport = (Viewport) transaction.GetObject(obj.ObjectId, OpenMode.ForWrite);
                        viewport.Locked = true;
                    }

                transaction.Commit();
            }
        }

        [CommandMethod("UnlockViewport", CommandFlags.Modal)]
        public void UnlockViewport()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction transaction = acCurDb.TransactionManager.StartTransaction())
            {
                PromptSelectionResult prompt = acDoc.Editor.SelectAll();

                if (prompt.Status != PromptStatus.OK) return;
                SelectionSet selectionSet = prompt.Value;

                foreach (SelectedObject obj in selectionSet)
                    if (obj.ObjectId.ObjectClass == RXObject.GetClass(typeof(Viewport)))
                    {
                        Viewport viewport = (Viewport)transaction.GetObject(obj.ObjectId, OpenMode.ForWrite);
                        viewport.Locked = false;
                    }

                transaction.Commit();
            }
        }
    }
}