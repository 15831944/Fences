using System.Collections.Generic;
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
        [CommandMethod("CoordGuru", CommandFlags.Modal)]
        public void CoordGuru()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            PromptStringOptions pStrOpts = new PromptStringOptions("\nВведите адрес сайта с координатами: ");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);

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

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
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
    }
}