using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.CustomControls
{
    public class CheckBoxControlViewModel<T> : AViewModel
    {
        public event Action<CheckBoxControlViewModel<T>, bool, bool> OnCheckedChanged;
       
        private Func<T, string> _funcDisplayMember;
        private Func<T, string> _funcGroupMember;

        private bool _isChecked;


        private T _entity;


        #region Properties


        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                /*var old = _isChecked;
                if (old && value)
                    _isChecked = false;
                else
                    _isChecked = value;*/

                var val = value;

                if (_isChecked && val)
                    val = false;

                if (_isChecked != val)
                {
                    var old = _isChecked;
                    _isChecked = val;
                    RaisePropertyChanged(nameof(IsChecked));

                    var evt = OnCheckedChanged;
                    if (evt != null)
                        evt(this, old, val);
                }
            }
        }


        public string UIDisplay
        {
           get { return _funcDisplayMember == null ? "" : _funcDisplayMember(_entity); }
        }


        public string GroupName
        {
            get { return _funcGroupMember == null ? "" : _funcGroupMember(_entity); }
        }

        public T Entity
        {
            get { return _entity; }
        }


        #endregion




        public CheckBoxControlViewModel(T entity, Func<T, string> funcDisplayMember, Func<T, string> funcGroupMember)
        {
            _entity = entity;
            _funcDisplayMember = funcDisplayMember;
            _funcGroupMember = funcGroupMember;
           
        }
    }
}
