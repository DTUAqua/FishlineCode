using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Export
{
    public class ExportLogEntryItem : AViewModel
    {
        public static int StaticIndex = 0;

        private int _index;
        private DateTime _time;
        private string _message;
        private string _logType;
        private ExportLogState _state;
        private bool _blnShowTopBorder = true;


        #region Properties


        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged(nameof(Index));
            }
        }


        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }


        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                RaisePropertyChanged(nameof(Time));
            }
        }


        /// <summary>
        /// Failed, Passed
        /// </summary>
        public ExportLogState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged(nameof(State));
            }
        }


        /// <summary>
        /// CHECK, INFO
        /// </summary>
        public string LogType
        {
            get { return _logType ?? ""; }
            set
            {
                _logType = value;
                RaisePropertyChanged(nameof(LogType));
            }
        }


      

        public bool ShowTopBorder
        {
            get { return _blnShowTopBorder; }
            set
            {
                _blnShowTopBorder = value;
                RaisePropertyChanged(() => ShowTopBorder);
            }
        }

        #endregion



        public ExportLogEntryItem(string msg, string logType = null, ExportLogState state = ExportLogState.None)
        {
            Time = DateTime.Now;
            _message = msg;
            _logType = logType;
            _index = ++StaticIndex;
            _state = state;
        }
    }


}
