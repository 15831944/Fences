using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;

namespace CoordGuru
{
    public class Drawer
    {
        public void CreatePolylines(List<string> pointsCoord)
        {
            List<Point2d> points = new List<Point2d>();
            for (int i = 0; i < pointsCoord.Count; i += 2)
            {
                points.Add(new Point2d(float.Parse(pointsCoord[i]), float.Parse(pointsCoord[i + 1])));
            }
            FinalDrawing(points);
        }

        private void FinalDrawing(List<Point2d> points)
        {
            int counter = 0;
            AcadProvider acadProvider = new AcadProvider();
            for (int i = 0; i < points.Count; i++)
            {
                counter++;
                if (i > 0 && points[i] == points[0])
                {
                    acadProvider.Draw(points.GetRange(0, counter));
                    if (i != points.Count - 1)
                    {
                        FinalDrawing(points.GetRange(i + 1, points.Count - counter));
                    }
                }
            }
        }
    }
}