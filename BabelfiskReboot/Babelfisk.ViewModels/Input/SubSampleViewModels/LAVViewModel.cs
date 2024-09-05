using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using System.Windows.Input;

namespace Babelfisk.ViewModels.Input
{
    public class LAVViewModel : ALavSFViewModel
    {
        private bool _blnIsAutoLength;


        private System.Windows.Controls.DataGridCellInfo _objCurrentCell;

        public override string LavSFType
        {
            get { return "LAV"; }
        }


        public bool IsAutoLength
        {
            get { return _blnIsAutoLength; }
            set
            {
                _blnIsAutoLength = value;
                RaisePropertyChanged(() => IsAutoLength);
            }
        }


        public System.Windows.Controls.DataGridCellInfo CurrentCell
        {
            get { return _objCurrentCell; }
            set
            {
                _objCurrentCell = value;
                RaisePropertyChanged(() => CurrentCell);
            }
        }


        public bool IsAutoLengthDecrement
        {
            get { return BusinessLogic.Settings.Settings.Instance.LavAutoDecrement; }
            set
            {
                BusinessLogic.Settings.Settings.Instance.LavAutoDecrement = value;
                RaisePropertyChanged(() => IsAutoLengthDecrement);
            }
        }



        public LAVViewModel(SubSampleViewModel parent)
            : base(parent)
        {
            _enmSubSampleType = Entities.SubSampleType.LAVRep;
            RegisterToKeyDown();
        }


        protected override void SelectedAnimalItemChanged(AnimalItem oldItem, AnimalItem newItem)
        {
            try
            {
                base.SelectedAnimalItemChanged(oldItem, newItem);

                //Only perform auto increment if enabled
                if (newItem == null || !IsAutoLength || Items == null ||
                    //Onlt perform auto increment when cursor (active cell) is at Length and Number columns
                    _objCurrentCell == null || _objCurrentCell.Column == null || !(_objCurrentCell.Column.SortMemberPath.Equals("Length", StringComparison.InvariantCultureIgnoreCase) || _objCurrentCell.Column.SortMemberPath.Equals("Number", StringComparison.InvariantCultureIgnoreCase)))
                    return;

                int intIndex = Items.IndexOf(newItem);

                int? intLength = null;
                AnimalItem prevItem = null;

                if (intIndex > 0)
                {
                    prevItem = Items[intIndex - 1];
                    intLength = prevItem.Length.HasValue ? prevItem.Length + (IsAutoLengthDecrement ? -1 : 1) : null;
                }

                if (intLength.HasValue)
                    newItem.Length = intLength.Value;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            SyncNewRows();
        }


        public override void Initialize(SubSample ss)
        {
            base.Initialize(ss);
        }

        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == System.Windows.Input.Key.N && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.N || e.Key == Key.N) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
            {
                e.Handled = true;
                ScrollTo("NewItem");
            }
        }
    }
}
