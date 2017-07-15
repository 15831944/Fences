using System;

namespace CoordGuru
{
    public class Point
    {
        private static double EPS = 10e-8;

        private double _x;
        private double _y;
        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double GetX()
        {
            return _x;
        }

        public double GetY()
        {
            return _y;
        }

        public override bool Equals(object obj)
        {
            Point other = obj as Point;
            return Math.Abs(_x - other._x) < EPS && Math.Abs(_y - other._y) < EPS;
        }

        public override string ToString()
        {
            return "(" + _x + ", " + _y + ")";
        }
    }
}