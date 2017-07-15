using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace CoordGuru
{
    public class GetFromExcel : IDataProvider
    {
        private string _location;

        public GetFromExcel(string location)
        {
            this._location = location;
        }

        public List<Point> GetData()
        {
            return ReadExcel(_location);
        }

        private List<Point> ReadExcel(string path)
        {
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path, ReadOnly: true);
            Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets.Item[1];
            Excel.Range range = sheet.UsedRange;

            List<string> listY = new List<string>();
            List<string> listX = new List<string>();

            object[,] colY = (object[,])range.Columns[1, Type.Missing].Value;
            for (int i = 1; i <= colY.Length; i++)
            {
                listY.Add(colY[i, 1].ToString());
            }
            object[,] colX = (object[,])range.Columns[2, Type.Missing].Value;
            for (int i = 1; i <= colX.Length; i++)
            {
                listX.Add(colX[i, 1].ToString());
            }

            List<Point> outList = new List<Point>();
            for (int i = 0; i < listY.Count; i++)
            {
                Point point = new Point(double.Parse(listX[i]), double.Parse(listY[i]));
                outList.Add(point);
            }

            return outList;
        }
    }
}