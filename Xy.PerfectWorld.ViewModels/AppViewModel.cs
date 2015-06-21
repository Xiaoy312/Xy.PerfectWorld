using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.ViewModels
{
    public class AppViewModel : ReactiveObject
    {
        public static AppViewModel Instance = new AppViewModel();
        public SettingViewModel SettingVM = new SettingViewModel();

        readonly ObservableAsPropertyHelper<GameModel> attachedGame;
        public GameModel AttachedGame
        {
            get { return attachedGame.Value; }
        }

        bool autoCombatEnabled = true;
        public bool AutoCombatEnabled
        {
            get { return autoCombatEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoCombatEnabled, value); }
        }

        private AppViewModel()
        {
            attachedGame = SettingVM.WhenAny(x => x.SelectedGame, x => x.Value)
                .ToProperty(this, x => x.AttachedGame);

            InitializeAutoLoot();
        }


        #region property AutoLootEnabled
        bool autoLootEnabled = true;
        public bool AutoLootEnabled
        {
            get { return autoLootEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoLootEnabled, value); }
        } 
        #endregion
        

        private void InitializeAutoLoot()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(_ => (AttachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn && AutoLootEnabled)
                .Subscribe(_ =>
                {
                    var ground = new GroundContainer(AttachedGame.Game);
                    //ground.GetItems()
                });
        }
    }
}
