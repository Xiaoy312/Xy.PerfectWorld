using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;
using Xy.DataAnalysis;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.ViewModels
{
    public partial class SettingViewModel : ReactiveObject
    {
        private GameModel model { get; }
        private Game game { get; }
        private Core core { get; }

        public SettingViewModel()
        {
            model = Locator.Current.GetService<GameModel>();
            game = model.Game;
            core = game.Core;

            InitializeAutoCombat();
            InitializeFeatures();
        }
        
        partial void InitializeAutoCombat();
        partial void InitializeFeatures();
    }
}
