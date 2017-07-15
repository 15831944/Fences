using NUnit.Framework;
using CoordGuru;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoordGuruTests.Properties;

namespace CoordGuru.Tests
{
    [TestFixture()]
    public class GetFromExcelTests
    {
        [Test()]
        public void GetExcelDataTest()
        {
            string path = @"C:\Users\krami\Source\Repos\Fences\CoordGuruTests\Resources\testCoord.xls";
            CoordGuru.IDataProvider excel = new GetFromExcel(path);
            List<Point> points = excel.GetData();
            Assert.AreEqual(new Point(109807.97, 106042.55), points[0]);
            Assert.AreEqual(new Point(110073.69, 105985.32), points[1]);
        }

        [Test()]
        public void GetRgisDataTest()
        {
            string address = @"http://www.rgis.spb.ru/map/GeomCoordinatesTable.aspx?alias=ZKP_LAND_06&id=149503201&description=%CE%E1%FA%E5%EA%F2%20%F1%EB%EE%FF%20%27%D3%F7%F2%B8%ED%ED%FB%E5%20%C7%D3%27";
            CoordGuru.IDataProvider rgis = new GetFromRgis(address);
            List<Point> points = rgis.GetData();
            Assert.AreEqual(new Point(109355.00, 91052.41), points[0]);
            Assert.AreEqual(new Point(109354.39, 91051.30), points[1]);
        }
    }
}