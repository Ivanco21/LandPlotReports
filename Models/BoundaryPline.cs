using System;
using System.Collections.Generic;
using System.Linq;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;
using LandPlotReports.Utils;

namespace LandPlotReports.Models
{
    internal class BoundaryPline
    {
        private DbPolyline _boundPolyline;
        private List<BoundPountInfo> _boundPountInfo;

        public BoundaryPline(DbPolyline boundPolyline)
        {
            _boundPolyline = boundPolyline;
        }
        /// <summary>
        /// геометрия граничной линии
        /// </summary>
        internal DbPolyline BoundPolyline { get => _boundPolyline; set => _boundPolyline = value; }

        /// <summary>
        /// информация о граничной точке
        /// </summary>
        internal List<BoundPountInfo> BoundPountInfo
        {
            get => _boundPountInfo;
            set => _boundPountInfo = value;
        }

        internal void FindBoundPointDescription(List<DbText> allTexts)
        {
            // все вершины полинии
            List<Point3d> pLineVertex = PolylineUtils.GetPolilyneVertices(BoundPolyline);

            var boundInfoAll = new List<BoundPountInfo>();

            foreach (Point3d pt in pLineVertex)
            {
                // только один текст. если будет два в выборку не попадет
                DbText eqPts = allTexts.Where(ptText => ptText.Text.Origin.IsEqualTo(pt)).FirstOrDefault();

                if (null != eqPts)
                {
                    BoundPountInfo bInf = new BoundPountInfo();
                    string txtValue = eqPts.Text.Text;
                    double numValue = 0;

                    // важно.только точки, тексты которых можно спарсить как цифру,
                    // добавляются в выборку
                    if (Double.TryParse(txtValue,out numValue))
                    {
                        bInf.Point = pt;

                        bInf.Description = numValue;
                        boundInfoAll.Add(bInf);
                    }
                }
            }

            BoundPountInfo = boundInfoAll;
        }
    }

    internal class BoundPountInfo
    {
        private Point3d _point;
        private double _description;
        private List<string> _cadastralPlotNames;

        public Point3d Point { get => _point; set => _point = value; }
        public double Description { get => _description; set => _description = value; }
        public List<string> CadastralPlotNames { get => _cadastralPlotNames; set => _cadastralPlotNames = value; }
    }
}
