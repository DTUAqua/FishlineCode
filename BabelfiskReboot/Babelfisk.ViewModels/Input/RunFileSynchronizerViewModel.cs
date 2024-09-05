using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;

namespace Babelfisk.ViewModels.Input
{
    public class RunFileSynchronizerViewModel : AViewModel
    {
        public static readonly RunFileSynchronizerViewModel Instance = new RunFileSynchronizerViewModel();

        private Task _t;

        /*
            -- Below scripts makes sure the server has xp_cmdshell enabled
            -- To allow advanced options to be changed.
            EXEC sp_configure 'show advanced options', 1
            GO
            -- To update the currently configured value for advanced options.
            RECONFIGURE
            GO
            -- To enable the feature.
            EXEC sp_configure 'xp_cmdshell', 1
            GO
            -- To update the currently configured value for this feature.
            RECONFIGURE
            GO
         */


        private RunFileSynchronizerViewModel()
        {
        }


        public Task RunSynchronizerAsync()
        {
            if (IsLoading)
                return null;

            IsLoading = true;

            _t = Task.Factory.StartNew(RunSynchronizer).ContinueWith(t => new Action(() => { _t = null; IsLoading = false; }).Dispatch());
            return _t;
        }


        private void RunSynchronizer()
        {
            var man = new Babelfisk.BusinessLogic.DataInput.DataInputManager();

            try
            {
                var res = man.RunFileSynchronizer();

                if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }

    }
}
