using System.Collections.Generic;

namespace CoordGuru
{
    public interface IDataProvider
    {
        List<Point> GetData();
    }
}