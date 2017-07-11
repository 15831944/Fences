using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace CoordGuru
{
    public class GetFromExcel : IDataProvider
    {
        public List<string> GetData()
        {
            AcadProvider acadProvider = new AcadProvider();
            string location = acadProvider.GetFile();

            return ExcelReader(location);
        }

        private List<string> ExcelReader(string path)
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

            List<string> outList = new List<string>();
            for (int i = 0; i < listY.Count; i++)
            {
                outList.Add(listX[i]);
                outList.Add(listY[i]);
            }

            return outList;
        }
    }
}