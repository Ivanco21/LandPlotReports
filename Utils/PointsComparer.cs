using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Multicad.Geometry;

namespace LandPlotReports.Utils
{
    internal class PointsComparer : IEqualityComparer<Point3d>
    {
        public bool Equals(Point3d pt1, Point3d pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y && pt1.Z == pt2.Z;
        }

        public int GetHashCode([DisallowNull] Point3d obj)
        {
            int xHash = obj.X.GetHashCode();
            int yHash = obj.Y.GetHashCode();
            int zHash = obj.Z.GetHashCode();
            return xHash + yHash + zHash;
        }
    }
}
