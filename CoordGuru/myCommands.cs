using System;
using System.Collections.Generic;
//using Autodesk.AutoCAD.Runtime;
using CoordGuru;

//[assembly: CommandClass(typeof(MyCommands))]

namespace CoordGuru
{
    public class MyCommands
    {
        private AcadProvider _acadProvider = new AcadProvider();

        //[CommandMethod("CoordGuru", CommandFlags.Modal)]
        public void CoordGuru()
        {
            string option = _acadProvider.Decision();
            IDataProvider dataProvider = DetectDataProvider(option);
            List<Point> points = dataProvider.GetData();

            Drawer drawer = new Drawer();
            drawer.CreatePolylines(points);
        }

        private IDataProvider DetectDataProvider(string option)
        {
            switch (option)
            {
                case "Web":
                    string input = "\nВведите адрес сайта с координатами: ";
                    string address = _acadProvider.StringInput(input);
                    return new GetFromRgis(address);
                case "Excel":
                    string location = _acadProvider.GetFile();
                    return new GetFromExcel(location);
                default:
                    throw new ArgumentException("Unknown option " + option);
            }
        }
    }
}