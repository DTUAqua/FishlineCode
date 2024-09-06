using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Menu
{
    public class BluetoothHelperViewModel : AViewModel
    {
        private DelegateCommand _Close;
        public BluetoothHelperViewModel()
        {
            WindowWidth = 800;
            WindowHeight = 525;
            MinWindowHeight = 400;
            MinWindowWidth = 500;

            base.AdjustWindowWidthHeightToScreen();
        }

        public DelegateCommand CloseCommand
        {
            get { return _Close ?? (_Close = new DelegateCommand(CloseThis)); }
        }

        private void CloseThis()
        {
            this.Close();
        }
    }
}
