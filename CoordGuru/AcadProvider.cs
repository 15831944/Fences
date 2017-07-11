using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CoordGuru
{
    public class AcadProvider
    {
        private Document _document;
        private Database _database;

        public string Decision()
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;

            PromptKeywordOptions options = new PromptKeywordOptions("");
            options.Message = "\nВыберите источник данных";
            options.Keywords.Add("Excel");
            options.Keywords.Add("Web");
            options.AllowNone = false;

            return _document.Editor.GetKeywords(options).StringResult;
        }

        public string StringInput(string input)
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;

            PromptStringOptions options = new PromptStringOptions(input) {AllowSpaces = true};
            PromptResult result = _document.Editor.GetString(options);

            return result.StringResult;
        }

        public void Draw(List<Point2d> points)
        {
            _document = Application.DocumentManager.MdiActiveDocument;
            _database = _document.Database;

            using (Transaction transaction = _database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = transaction.GetObject(_database.BlockTableId,
                    OpenMode.ForRead) as BlockTable;

                BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline polyline = new Polyline())
                {
                    for (int i = 0; i < points.Count; i++)
                        polyline.AddVertexAt(i, points[i], 0, 0, 0);

                    if (blockTableRecord != null) blockTableRecord.AppendEntity(polyline);
                    polyline.Closed = true;
                    transaction.AddNewlyCreatedDBObject(polyline, true);
                }
                transaction.Commit();
            }
        }

        public string GetFile()
        {
            OpenFileDialog dialog = new OpenFileDialog("Выберите файл координат",
                null,
                "xls; xlsx",
                "ExcelCoordinates",
                OpenFileDialog.OpenFileDialogFlags.DoNotTransferRemoteFiles);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.Filename;
            }
            return null;
        }
    }
}