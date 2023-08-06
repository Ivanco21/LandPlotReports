using System;
using System.Collections.Generic;
using System.Linq;
using Multicad;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Symbols.Tables;
using LandPlotReports.Models;

namespace LandPlotReports.MainWorkers
{
    internal class LandPlotReport
    {
        private McDocument nDoc;

        public McDocument NDoc { get { return nDoc; } }
        internal readonly string nameForIdenticalCadastalPlot = "Кадастровый номер, обозначение, учетный номер объекта";
        public LandPlotReport(McDocument doc)
        {
            this.nDoc = doc;
        }
        /// <summary>
        /// основной flow, сбор данных с dwg и создание отчета
        /// </summary>
        /// <param name="pLineId"></param>
        internal void DoMainFlowForLandPlotReport(McObjectId pLineId)
        {

            DbPolyline boundPl = pLineId.GetObject().Cast<DbPolyline>();
            BoundBlock bbPolyline =  boundPl.Geometry.BndBlock;

            BoundaryPline boundaryPline = new BoundaryPline(boundPl);

            // тексты в BB полилинии
            ObjectFilter filter = new ObjectFilter();
            filter.AddDoc(nDoc);
            filter.SetBound(bbPolyline);
            filter.AddType(DbText.TypeID);

            List<McObjectId> nearPl = McObjectManager.SelectObjects(filter).ToList();
            List<DbText> textsNearPl = nearPl.Select(id => id.GetObject()).Cast<DbText>().ToList();

            boundaryPline.FindBoundPointDescription(textsNearPl);

            // все блоки 
            filter = new ObjectFilter();
            filter.AddDoc(nDoc);
            filter.AddType(McBlockRef.TypeID);

            List<McObjectId> blockRefs = McObjectManager.SelectObjects(filter).ToList();
            List<McBlockRef> blocks = blockRefs.Select(id => id.GetObject()).Cast<McBlockRef>().ToList();

            List<McBlockRef> plotBlocks = blocks.Where(b => IsBlockCadastralPlot(b)).ToList();

            List<CadastralPlot> cadastralPlots = new List<CadastralPlot>();

            foreach (McBlockRef bl in plotBlocks)
            {
                CadastralPlot plot = new CadastralPlot(bl, nameForIdenticalCadastalPlot);
                cadastralPlots.Add(plot);
            }

            /*core algoritm
             * для каждой точки полилинии границы проверяется,
             * принадлежит ли она "кадастровому участку" т.е. 
             * полилиниям блоков кадастрового участка, если "да,
             * этот участок относится к точке.
             * */

            foreach (BoundPountInfo boundPtInf in boundaryPline.BoundPountInfo)
            {
                Point3d pt = boundPtInf.Point;
                List<string> names = new List<string>();
                foreach (CadastralPlot plot in cadastralPlots)
                {
                    List<Point3d> plotPts = plot.Vertices;
                    bool isInPlot = plotPts.Contains(pt); //вероятно самое узкое место

                    if (isInPlot)
                    {
                        names.Add(plot.NameOfPlot);
                    }
                }
                boundPtInf.CadastralPlotNames = names;
            }
            /* пустые удаляем (не нашлось кадастрового участка для точки)
             * и сортировка по номеру точки
             * */
            boundaryPline.BoundPountInfo = boundaryPline.BoundPountInfo
                                           .Where(inf => inf.CadastralPlotNames.Count != 0)
                                           .OrderBy(inf => inf.Description).ToList();

            // пустая таблица
            if(boundaryPline.BoundPountInfo.Count == 0)
            {
                return;
            }

            // итоговый отчет - таблица.
            GenerateLandPlotReport(boundaryPline.BoundPountInfo);
        }

        private void GenerateLandPlotReport(List<BoundPountInfo> boundPountInfo)
        {
            // таблица
            McTable tbl = new McTable();
            int rowCount = boundPountInfo.Count;
            int colCount = 2;
            tbl.Rows.AddRange(0, rowCount + 1);
            tbl.Columns.AddRange(0, colCount);

            // настройки таблицы по умолчанию
            tbl.DefaultCell.TextHeight = 2.5;
            tbl.DefaultCell.TextColor = System.Drawing.Color.Black;
            tbl.DefaultCell.VerticalTextAlign = VertTextAlign.Center;
            tbl.DefaultCell.HorizontalTextAlign = HorizTextAlign.Center;

            // текущий текстовый стиль
            string txtStyleName = McObjectManager.CurrentStyle.CurrentTextStyle;
            tbl.DefaultCell.TextStyle = txtStyleName;

            // именование столбцов
            tbl[0, 0].Value = "Номер точки";
            tbl[0, 1].Value = "Кадастровый номер(а)";
            // ширина столбцов
            tbl.Columns[0].Width = 25;
            tbl.Columns[1].Width = 30 * boundPountInfo.Max(inf => inf.CadastralPlotNames.Count);
            // ширина строк
            for (int k = 0; k < rowCount; k++)
            {
                tbl.Rows[k].Height = 5;
            }

            // основной блок данных
            // с единицы т.к. заголок 
            int iRow = 0;
            for (int i = 0; i < boundPountInfo.Count; i++)
            {
                iRow = i + 1;
                tbl[iRow, 0].Type = CellFormatEnum.Number;
                tbl[iRow, 1].Type = CellFormatEnum.String;

                string plotNames = string.Join(", ", boundPountInfo[i].CadastralPlotNames);
                tbl[iRow, 0].Value = boundPountInfo[i].Description.ToString();
                tbl[iRow, 1].Value = plotNames;
            }

            //вставка
            tbl.PlaceObject(McEntity.PlaceFlags.Silent);

        }

        internal bool IsBlockCadastralPlot(McBlockRef blockRef)
        {
            bool isCadastralPlot = false;

            McProperties blockProps = blockRef.DbEntity.GetProperties(McProperties.PropertyType.Object);

            if (blockProps != null)
            {
                List<string> propNames = blockProps.GetNames();
                bool isHaveIdentName = propNames.Exists(s => s.Contains(nameForIdenticalCadastalPlot));

                if (isHaveIdentName)
                {
                    isCadastralPlot = true;
                }
            }

            return isCadastralPlot;
        }
    }
}
