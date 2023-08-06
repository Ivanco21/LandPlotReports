
using System;
using System.Collections.Generic;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;

namespace LandPlotReports.Utils
{
    internal static class PolylineUtils
    {
        internal static List<Point3d> GetPolilyneVertices(DbPolyline pl)
        {
            List<Point3d> pLineVertex = new List<Point3d>();

            for (int i1 = 0; i1 < pl.Polyline.Vertices.Count; i1++)
            {
                Point3d Vertex = new Point3d(); double Bulge; double startWidth; double endWidth; Vector3d nrm;
                pl.Polyline.Vertices.GetVertexAt((uint)i1, out Vertex, out Bulge, out startWidth, out endWidth, out nrm);
                pLineVertex.Add(Vertex);
            }

            return pLineVertex;
        }
    }
}
