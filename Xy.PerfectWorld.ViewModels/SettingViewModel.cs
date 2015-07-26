using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;

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
