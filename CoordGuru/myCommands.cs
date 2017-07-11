using System.Collections.Generic;
using Autodesk.AutoCAD.Runtime;
using CoordGuru;

[assembly: CommandClass(typeof(MyCommands))]

namespace CoordGuru
{
    public class MyCommands
    {
        [CommandMethod("CoordGuru", CommandFlags.Modal)]
        public void CoordGuru()
        {
            AcadProvider provider = new AcadProvider();
            string option = provider.Decision();
            List<string> points = new List<string>();
            if (option == "Web")
            {
                GetFromRgis rgis = new GetFromRgis();
                points = rgis.GetData();
            }
            if (option == "Excel")
            {
                GetFromExcel excel = new GetFromExcel();
                points = excel.GetData();
            }

            Drawer drawer = new Drawer();
            drawer.CreatePolylines(points);
        }
    }
}