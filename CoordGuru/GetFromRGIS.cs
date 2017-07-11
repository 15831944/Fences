using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace CoordGuru
{
    public class GetFromRgis : IDataProvider
    {
        public List<string> GetData()
        {
            AcadProvider provider = new AcadProvider();
            string input = "\nВведите адрес сайта с координатами: ";
            List<string> webdata = WebGetter(provider.StringInput(input));
            return FilterListRGIS(webdata);
        }

        private List<string> WebGetter(string address)
        {
            WebClient webClient = new WebClient();
            string page = webClient.DownloadString(address);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var query = from table in doc.DocumentNode.SelectNodes("//table")
                from row in table.SelectNodes("tr")
                from cell in row.SelectNodes("th|td")
                select new { Table = table.Id, CellText = cell.InnerText };

            List<string> tableCells = new List<string>();

            foreach (var cell in query)
                tableCells.Add(cell.CellText);

            tableCells.RemoveRange(0, 3);

            return tableCells;
        }

        private List<string> FilterListRGIS(List<string> inputList)
        {
            List<string> ouputList = new List<string>();
            for (int i = 0; i < inputList.Count - 2; i++)
            {
                if (i % 3 != 0) continue;
                ouputList.Add(inputList[i + 2]);
                ouputList.Add(inputList[i + 1]);
            }
            return ouputList;
        }
    }
}