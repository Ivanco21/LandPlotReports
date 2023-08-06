using System;
using System.IO;
using LandPlotReports.ViewModels.Base;
using Multicad.DatabaseServices.StandardObjects;

namespace LandPlotReports.ViewModels
{
    internal class MainWindowViewModel: ViewModel
    {
        private readonly string _title = "Таблица кадастрового отчета";
        private string _appState;
        //private DbPolyline _boundPolyLine;
        private FileInfo _fInfo;
        public string Title => _title;

        /// <summary> Состояние приложения</summary>
        public string AppState { get => _appState; set => Set(ref _appState, value); }

        /// <summary> Полилиния (граница участка)</summary>
        public FileInfo BoundPolyLine { get => _fInfo; set => Set(ref _fInfo, value); }

    }
}
