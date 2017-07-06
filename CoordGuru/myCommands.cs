using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CoordGuru;
using HtmlAgilityPack;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(MyCommands))]

namespace CoordGuru
{
    public class MyCommands
    {
        private Document _acDoc;
        private Database _acCurDb;

        [CommandMethod("CoordGuru", CommandFlags.Modal)]
        public void CoordGuru()
        {
            _acDoc = Application.DocumentManager.MdiActiveDocument;
            _acCurDb = _acDoc.Database;

            PromptStringOptions pStrOpts = new PromptStringOptions("\nВведите адрес сайта с координатами: ");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = _acDoc.Editor.GetString(pStrOpts);

            string address = pStrRes.StringResult;
            List<string> data = WebGetter(address);
            List<Point2d> points = new List<Point2d>();
            int counter = 1;
            for (int i = 0; i < data.Count; i++)
            {
                if (counter == 2)
                    points.Add(new Point2d(float.Parse(data[i + 1]), float.Parse(data[i])));
                counter++;
                if (counter == 4)
                    counter = 1;
            }
            Divider(points);
        }

        public List<string> WebGetter(string address)
        {
            WebClient webClient = new WebClient();
            string page = webClient.DownloadString(address);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var query = from table in doc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                from row in table.SelectNodes("tr").Cast<HtmlNode>()
                from cell in row.SelectNodes("th|td").Cast<HtmlNode>()
                select new {Table = table.Id, CellText = cell.InnerText};

            List<string> tableCells = new List<string>();

            foreach (var cell in query)
                tableCells.Add(cell.CellText);

            tableCells.RemoveRange(0, 3);

            return tableCells;
        }

        public void Drawer(List<Point2d> points)
        {
            using (Transaction acTrans = _acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(_acCurDb.BlockTableId,
                    OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    for (int i = 0; i < points.Count; i++)
                        acPoly.AddVertexAt(i, points[i], 0, 0, 0);

                    acBlkTblRec.AppendEntity(acPoly);
                    acPoly.Closed = true;
                    acTrans.AddNewlyCreatedDBObject(acPoly, true);
                }
                acTrans.Commit();
            }
        }

        public void Divider(List<Point2d> points)
        {
            int counter = 0;
            for (int i = 0; i < points.Count; i++)
            {
                counter++;
                if (i > 0 && points[i] == points[0])
                {
                    Drawer(points.GetRange(0, counter));
                    if (i != points.Count - 1)
                    {
                        Divider(points.GetRange(i + 1, points.Count - counter));
                    }
                }
            }
        } 
    }
}