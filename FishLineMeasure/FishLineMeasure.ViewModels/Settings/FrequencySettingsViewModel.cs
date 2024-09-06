using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace FishLineMeasure.ViewModels.Settings
{
    public class FrequencySettingsViewModel : AViewModel
    {
        private int _NewLenghtFrequency;
        private int _DeleteLastEntryFrequency;
        private int _GoToNextStationFrequency;
        private int _GoToNextOrderFrequency;
        private bool _NewLenghtBeepStatus;
        private bool _DeleteLastEntryBeepStatus;
        private bool _GoToNextStationBeepStatus;
        private bool _GoToNextOrderBeepStatus;

        private int _repeatNewLength;
        private int _repeatDeleteLastEntry;
        private int _repeatNextStation;
        private int _repeatNextOrder;

        private SettingsViewModel _vmParent;


        public int RepeatNewLength
        {
            get { return _repeatNewLength; }
            set
            {
                _repeatNewLength = value;
                RaisePropertyChanged(nameof(RepeatNewLength));
            }
        }

        public int RepeatDeleteLastEntry
        {
            get { return _repeatDeleteLastEntry; }
            set
            {
                _repeatDeleteLastEntry = value;
                RaisePropertyChanged(nameof(RepeatDeleteLastEntry));
            }
        }

        public int RepeatNextStation
        {
            get { return _repeatNextStation; }
            set
            {
                _repeatNextStation = value;
                RaisePropertyChanged(nameof(RepeatNextStation));
            }
        }

        public int RepeatNextOrder
        {
            get { return _repeatNextOrder; }
            set
            {
                _repeatNextOrder = value;
                RaisePropertyChanged(nameof(RepeatNextOrder));
            }
        }


        public int NewLenghtFrequency
        {
            get { return _NewLenghtFrequency; }
            set
            {
                _NewLenghtFrequency = value;
                RaisePropertyChanged(nameof(NewLenghtFrequency));
            }
        }
        public int DeleteLastEntryFrequency
        {
            get { return _DeleteLastEntryFrequency; }
            set
            {
                if (value < 36)
                    DispatchMessageBox("Frekvensen kan ikke være under 36");
                else if (value > 32000)
                    DispatchMessageBox("Frekvensen kan ikke være over 32000");
                else
                {
                    _DeleteLastEntryFrequency = value;
                    RaisePropertyChanged(nameof(DeleteLastEntryFrequency));
                }
            }
        }
        public int GoToNextStationFrequency
        {
            get { return _GoToNextStationFrequency;}
            set
            {
                if (value < 36)
                    DispatchMessageBox("Frekvensen kan ikke være under 36");
                else if (value > 32000)
                    DispatchMessageBox("Frekvensen kan ikke være over 32000");
                else
                {
                    _GoToNextStationFrequency = value;
                    RaisePropertyChanged(nameof(GoToNextStationFrequency));
                }
            }
        }
        public int GoToNextOrderFreguency
        {
            get { return _GoToNextOrderFrequency; }
            set
            {
                if (value < 36)
                    DispatchMessageBox("Frekvensen kan ikke være under 36");
                else if (value > 32000)
                    DispatchMessageBox("Frekvensen kan ikke være over 32000");
                else
                {
                    _GoToNextOrderFrequency = value;
                    RaisePropertyChanged(nameof(GoToNextOrderFreguency));
                } 
            }
        }
        
        public bool NewLenghtBeepStatus
        {
            get { return _NewLenghtBeepStatus; }
            set
            {
                _NewLenghtBeepStatus = value;
                RaisePropertyChanged(nameof(NewLenghtBeepStatus));
            }
        }
        public bool DeleteLastEntryBeepStatus
        {
            get { return _DeleteLastEntryBeepStatus; }
            set
            {
                _DeleteLastEntryBeepStatus = value;
                RaisePropertyChanged(nameof(DeleteLastEntryBeepStatus));
                if (IsDirty != true)
                {
                    IsDirty = true;
                }
            }
        }
        public bool GoToNextStationBeepStatus
        {
            get { return _GoToNextStationBeepStatus; }
            set
            {
                _GoToNextStationBeepStatus = value;
                RaisePropertyChanged(nameof(GoToNextStationBeepStatus));
                if (IsDirty != true)
                {
                    IsDirty = true;
                }
            }
        }
        public bool GoToNextOrderBeepStatus
        {
            get { return _GoToNextOrderBeepStatus; }
            set
            {
                _GoToNextOrderBeepStatus = value;
                RaisePropertyChanged(nameof(GoToNextOrderBeepStatus));
                if (IsDirty != true)
                {
                    IsDirty = true;
                }
            }
        }

        private DelegateCommand _cmdNewLenght;
        private DelegateCommand _cmdPlayDeleteLast;
        private DelegateCommand _cmdNextStation;
        private DelegateCommand _cmdNextOrder;
        

        public FrequencySettingsViewModel(SettingsViewModel vmParent)
        {
            _vmParent = vmParent;
            NewLenghtFrequency = AppSettings.FrequencySettingForNewLenghtAdded;
            NewLenghtBeepStatus = AppSettings.FrequencySettingsStatusForNewLenght;
            DeleteLastEntryFrequency = AppSettings.FrequencySettingForDeleteLenght;
            DeleteLastEntryBeepStatus = AppSettings.FrequencySettingsStatusForDelete;
            GoToNextStationFrequency = AppSettings.FrequencySettingForNextStation;
            GoToNextStationBeepStatus = AppSettings.FrequencySettingsStatusForNextStation;
            GoToNextOrderFreguency = AppSettings.FrequencySettingForNextOrder;
            GoToNextOrderBeepStatus = AppSettings.FrequencySettingsStatusForNextOrder;

            RepeatNewLength = AppSettings.FrequencySettingRepeatForNewLenghtAdded;
            RepeatNextOrder = AppSettings.FrequencySettingRepeatForNextOrder;
            RepeatNextStation = AppSettings.FrequencySettingRepeatForNextStation;
            RepeatDeleteLastEntry = AppSettings.FrequencySettingRepeatForDeleteLenght;
            
            IsDirty = true;
        }

        public void Persist()
        {
            AppSettings.FrequencySettingForNewLenghtAdded = NewLenghtFrequency;
            AppSettings.FrequencySettingsStatusForNewLenght = NewLenghtBeepStatus;
            AppSettings.FrequencySettingForDeleteLenght = DeleteLastEntryFrequency;
            AppSettings.FrequencySettingsStatusForDelete = DeleteLastEntryBeepStatus;
            AppSettings.FrequencySettingForNextStation = GoToNextStationFrequency;
            AppSettings.FrequencySettingsStatusForNextStation = GoToNextStationBeepStatus;
            AppSettings.FrequencySettingForNextOrder = GoToNextOrderFreguency;
            AppSettings.FrequencySettingsStatusForNextOrder = GoToNextOrderBeepStatus;

            AppSettings.FrequencySettingRepeatForNewLenghtAdded = RepeatNewLength;
            AppSettings.FrequencySettingRepeatForNextOrder = RepeatNextOrder;
            AppSettings.FrequencySettingRepeatForNextStation = RepeatNextStation;
            AppSettings.FrequencySettingRepeatForDeleteLenght = RepeatDeleteLastEntry;

            IsDirty = false;
        }


        protected override string ValidateField(string strFieldName)
        {
            string error = null;

            if (!_blnValidate)
                return error;

            switch(strFieldName)
            {
                case "NewLenghtFrequency":
                    if (NewLenghtFrequency < 36)
                        error = "Lydfrekvensen for ny længde tilføjet kan ikke være under 36.";
                    else if (NewLenghtFrequency > 32000)
                        error = "Lydfrekvensen for ny længde tilføjet kan ikke være over 32000.";
                    break;

                case "RepeatNewLength":
                    if (RepeatNewLength < 1 || RepeatNewLength > 10)
                        error = "Antal afspilningsgentagelser for ny længde tilføjet skal være mellem 1 og 10.";
                    break;

                case "GoToNextOrderFreguency":
                    if (GoToNextOrderFreguency < 36)
                        error = "Lydfrekvensen for næste længdefordeling kan ikke være under 36.";
                    else if (GoToNextOrderFreguency > 32000)
                        error = "Lydfrekvensen for næste længdefordeling kan ikke være over 32000.";
                    break;

                case "RepeatNextOrder":
                    if (RepeatNextOrder < 1 || RepeatNextOrder > 10)
                        error = "Antal afspilningsgentagelser for næste længdefordeling skal være mellem 1 og 10.";
                    break;

                case "GoToNextStationFrequency":
                    if (GoToNextStationFrequency < 36)
                        error = "Lydfrekvensen for næste station kan ikke være under 36.";
                    else if (GoToNextStationFrequency > 32000)
                        error = "Lydfrekvensen for næste station kan ikke være over 32000.";
                    break;

                case "RepeatNextStation":
                    if (RepeatNextStation < 1 || RepeatNextStation > 10)
                        error = "Antal afspilningsgentagelser for næste station skal være mellem 1 og 10.";
                    break;

                case "DeleteLastEntryFrequency":
                    if (DeleteLastEntryFrequency < 36)
                        error = "Lydfrekvensen for sletning af seneste måling kan ikke være under 36.";
                    else if (DeleteLastEntryFrequency > 32000)
                        error = "Lydfrekvensen for sletning af seneste måling kan ikke være over 32000.";
                    break;

                case "RepeatDeleteLastEntry":
                    if (RepeatDeleteLastEntry < 1 || RepeatDeleteLastEntry > 10)
                        error = "Antal afspilningsgentagelser for sletning af seneste måling skal være mellem 1 og 10.";
                    break;
            }

            return error;
        }


        public DelegateCommand PlayNewLengthCommand
        {
            get { return _cmdNewLenght = new DelegateCommand(PlayLength); }
        }


        private void PlayLength()
        {
            var value = NewLenghtFrequency;

            _vmParent.ValidateAllProperties();

            if (HasErrors)
                return;

            Play(value, repeats: RepeatNewLength);
        }


        public DelegateCommand PlayDeleteLastCommand
        {
            get
            {
                return _cmdPlayDeleteLast = new DelegateCommand(PlayDelete);
            }
        }


        private void PlayDelete()
        {
            var value = DeleteLastEntryFrequency;

            _vmParent.ValidateAllProperties();

            if (HasErrors)
                return;

            Play(value, repeats: RepeatDeleteLastEntry);
        }


        public DelegateCommand PlayNextStationCommand
        {
            get
            {
                return _cmdNextStation = new DelegateCommand(PlayStation);
            }
        }

        private void PlayStation()
        {
            var value = GoToNextStationFrequency;

            _vmParent.ValidateAllProperties();

            if (HasErrors)
                return;

            Play(value, repeats: RepeatNextStation);
           
        }

        public DelegateCommand PlayNextOrderCommand
        {
            get
            {
                return _cmdNextOrder = new DelegateCommand(PlayNextOrder);
            }
        }

        private void PlayNextOrder()
        {
            var value = GoToNextOrderFreguency;

            _vmParent.ValidateAllProperties();

            if (HasErrors)
                return;

            Play(value, RepeatNextOrder);
        }

       
        public static void Play(int value, int repeats = 1, int duration = 300)
        {
            try
            {
                for (int i = 0; i < repeats; i++)
                    Console.Beep(value, duration);
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }
    }
}
