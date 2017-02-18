using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;

namespace Fences
{
    public class FenceEntry
    {
        private Point2d[] _pills;
        private LineSegment2d _segment;

        public FenceEntry(LineSegment2d segment, Point2d[] pills)
        {
            _segment = segment;
            _pills = pills;
        }

        public LineSegment2d Segment
        {
            get { return _segment; }
        }

        public Point2d[] GetPills()
        {
            return _pills;
        }

        public List<LineSegment2d> SplitByPills()
        {
            List<LineSegment2d> result = new List<LineSegment2d>(_pills.Length + 1);
            result.Add(new LineSegment2d(_segment.StartPoint, _pills[0]));
            for (int i = 0; i < _pills.Length-1; i++)
            {
                result.Add(new LineSegment2d(_pills[i], _pills[i+1]));
            }
            result.Add(new LineSegment2d(_pills[_pills.Length-1], _segment.EndPoint));
            return result;
        }

        public SteelMassStat ComputeSteelMass()
        {
            return new SteelMassStat();
        }


    }
}