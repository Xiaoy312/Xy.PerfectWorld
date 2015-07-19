using ReactiveUI;
using System;
using MoreLinq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.ViewModels
{
    public partial class SettingViewModel : ReactiveObject
    {
        public SettingViewModel()
        {
            InitializeClient();
            InitializeAutoCombat();
        }

        partial void InitializeClient();
        partial void InitializeAutoCombat();
    }


}
