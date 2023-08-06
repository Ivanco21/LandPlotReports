using System;
using HostMgd.ApplicationServices;
using Multicad;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.DatabaseServices.StandardObjects;
using LandPlotReports.MainWorkers;

namespace NCadCustom
{
    public class Commands : IExtensionApplication
    {
        public void Initialize()
        {
        }
        public void Terminate()
        {
        }
       
        /// <summary>
        /// Создание отчета по точками земельных участков, у которых прописан кадастровый номер
        /// </summary>
        [CommandMethod("LandPlotReport", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void MainCreateLandPlotReport()
        {
            McDocument nDoc = McDocumentsManager.GetActiveDoc();
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            McObjectId polylineFromUser = McObjectManager.SelectObject("Выберите полилинию:");
            bool isPolyline = McObjectManager.GetObject(polylineFromUser) is DbPolyline;

            if (! isPolyline)
            {
                ed.WriteMessage("Выбрана не полилиния! Программа завершена!");
                return;
            }

            try
            {
                LandPlotReport rep = new LandPlotReport(nDoc);
                rep.DoMainFlowForLandPlotReport(polylineFromUser);
                ed.WriteMessage("Скрипт успешно завершил работу!");
            }
            catch (Exception e)
            {
                ed.WriteMessage($"Ошибка : {e}");
            }
        }
    }
}
