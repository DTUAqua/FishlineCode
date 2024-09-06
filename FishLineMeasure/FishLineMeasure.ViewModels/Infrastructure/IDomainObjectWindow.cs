using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels
{
    /// <summary>
    /// Interface used in combination with Behaviors\WindoWrapper.Desktop. AViewModel in Analyzer.ViewModels implements this interface
    /// so viewmodels can request to close a popped window.
    /// </summary>
    public interface IDomainObjectWindow
    {
        /// <summary>
        /// Request to close a window event.
        /// </summary>
        event Action<IDomainObjectWindow> RequestClose;
    }
}
