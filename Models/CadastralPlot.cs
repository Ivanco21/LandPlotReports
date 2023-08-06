using System;
using System.Collections.Generic;
using System.Linq;
using LandPlotReports.Utils;
using Multicad;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;

namespace LandPlotReports.Models
{
    internal class CadastralPlot
    {
        private List<Point3d> _verticesList;
        private string _nameOfPlot;

        /// <summary>
        /// вершины кадастрового участка
        /// <para>со всех полилиний в блоке</para>
        /// </summary>
        public List<Point3d> Vertices { get => _verticesList; set => _verticesList = value; }
        /// <summary>
        /// кадастровый номер/имя
        /// </summary>
        public string NameOfPlot { get => _nameOfPlot; set => _nameOfPlot = value; }

        internal CadastralPlot(McBlockRef blockRef, string nameOfCustomProp)
        {
            NameOfPlot = SetNameOfPlot(blockRef, nameOfCustomProp);
            Vertices = FindPlVertices(blockRef);
        }

        /// <summary>
        /// получение кода/имени кадастрового участка
        /// <para> уже проверено что оно есть</para>
        /// </summary>
        /// <param name="blockRef"></param>
        /// <param name="nameOfCustomProp">свойство из которого берется имя</param>
        /// <returns></returns>
        private string SetNameOfPlot(McBlockRef blockRef, string nameOfCustomProp)
        {
            McProperties blockProps = blockRef.DbEntity.GetProperties(McProperties.PropertyType.Object);
            McProperty cadastrProp = blockProps.GetProperty(nameOfCustomProp);

            string name = (string)cadastrProp.GetValue();
            return name;
        }

        private List<Point3d> FindPlVertices(McBlockRef blockRef)
        {
            List<Point3d> pts = new List<Point3d>();

            List<EntityGeometry> blockObjs = blockRef.DbEntity.Explode();

            List<EntityGeometry> plsInBlock = blockObjs.Where(obj => obj.GeometryType == EntityGeomType.kPolyline).ToList();
            IEnumerable<Polyline3d> pls = plsInBlock.Select(p => p.Polyline);  

            foreach (Polyline3d pl in pls)
            {
                pts.AddRange(PolylineUtils.GetPolilyneVertices(pl));
            }

            return pts;
        }
    }
}
