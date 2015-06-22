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

        bool autoCombatEnabled = false;
        public bool AutoCombatEnabled
        {
            get { return autoCombatEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoCombatEnabled, value); }
        }
        bool autoLootEnabled = false;
        public bool AutoLootEnabled
        {
            get { return autoLootEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoLootEnabled, value); }
        }

        private AppViewModel()
        {
            attachedGame = SettingVM.WhenAny(x => x.SelectedGame, x => x.Value)
                .ToProperty(this, x => x.AttachedGame);

            InitializeAutoCombat();
            InitializeAutoLoot();
        }

        private void InitializeAutoCombat()
        {

        }

        private void InitializeAutoLoot()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(333))
                .Where(_ => (AttachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn && AutoLootEnabled)
                .Subscribe(_ => AutoLootPerform());
        }
        private void AutoLootPerform()
        {
            try
            {
                const float MaxLootRange = 10.0f;

                var ground = new GroundContainer(AttachedGame.Game);
                var item = ground.GetItems()
                    .Where(x => x.CollectMethod == CollectMethod.Gold)
                    .Where(x => x.RelativeDistance <= MaxLootRange)
                    .OrderBy(x => x.RelativeDistance.Value)
                    .FirstOrDefault();

                if (item != null)
                    AttachedGame.Game.Loot(item);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An exception occured in {nameof(AutoLootPerform)} : {e}");
            }
        }
    }

    public static class DumpExtension
    {
        public static IEnumerable<T> Dump<T>(this IEnumerable<T> source, string header = "Dumped : ")
        {
            foreach (var item in source)
            {
                Debug.WriteLine(header + item);
            }
            return source;
        }
    }
}
