using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SDMessageItem : AViewModel
    {
        private SDSampleItem _sdSampleItem;
        private MessageType _type;
        private string _message;

        public SDSampleItem SampleItem
        {
            get { return _sdSampleItem; }
        }

        public MessageType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
                RaisePropertyChanged(() => TypeString);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

       

        public string TypeString
        {
            get { return _type.ToString(); }
        }

        public SDMessageItem(SDSampleItem sdSampleItem, MessageType type, string message)
        {
            _sdSampleItem = sdSampleItem;
            _type = type;
            _message = message;
        }


        public string ToCSVValues()
        {
            return string.Join(";", TypeString, SampleItem.CSVRowNumber, SampleItem.Sample.animalId, Message);
        }

        public static string GetCSVHeader()
        {
            return string.Join(";", "Type", "Line", "Animal id", "Message");
        }
    }
}
