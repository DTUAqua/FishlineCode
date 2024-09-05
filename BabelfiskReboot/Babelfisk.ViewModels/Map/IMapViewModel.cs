using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.ViewModels.Map
{
    public interface IMapViewModel
    {

        bool IsWindow
        {
            get;
        }


        IAppRegionManager AppRegionManager
        {
            get;
        }

        void Dispose();

        List<MapPoint> Points
        {
            get;
            set;
        }

        bool IsHidden
        {
            get;
            set;
        }

        bool ShowWebBrowser
        {
            get;
            set;
        }

        string WindowTitle
        {
            get;
            set;
        }

        Task RefreshAsync();
    }
}
