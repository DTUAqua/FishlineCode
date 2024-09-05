using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Babelfisk.Warehouse;
using Anchor.Core;

namespace Babelfisk.ViewModels.Export
{
    public class DWMessagesViewModel : AViewModel
    {
        private ObservableCollection<DWMessage> _lstMessages;


        public ObservableCollection<DWMessage> Messages
        {
            get { return _lstMessages; }
            set
            {
                _lstMessages = value;
                RaisePropertyChanged(() => Messages);
            }
        }


        private DWMessagesViewModel() : base() { }

        public DWMessagesViewModel(List<DWMessage> lstMessages) : this()
        {
            _lstMessages = lstMessages.ToObservableCollection();

            WindowWidth = 700;
            WindowHeight = 400;
            WindowTitle = "Resultat af eksportering - fejl og beskeder";
        }
    }
}
