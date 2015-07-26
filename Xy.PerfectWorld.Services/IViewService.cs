using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xy.PerfectWorld.Services
{
    public interface IViewService
    {
        void ShowViewFor<TViewModel>() where TViewModel : class;
    }
}
