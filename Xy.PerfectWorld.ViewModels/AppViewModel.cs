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

        #region AttachedGame
        readonly ObservableAsPropertyHelper<GameModel> attachedGame;
        public GameModel AttachedGame
        {
            get { return attachedGame.Value; }
        }
        #endregion
        #region AutoCombatEnabled
        bool autoCombatEnabled = false;
        public bool AutoCombatEnabled
        {
            get { return autoCombatEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoCombatEnabled, value); }
        }
        #endregion
        #region AutoLootEnabled
        bool autoLootEnabled = false;
        public bool AutoLootEnabled
        {
            get { return autoLootEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoLootEnabled, value); }
        }
        #endregion

        private bool lootedInLastLoop;

        private AppViewModel()
        {
            attachedGame = SettingVM.WhenAny(x => x.SelectedGame, x => x.Value)
                .ToProperty(this, x => x.AttachedGame);

            InitializeAutoCombat();
            InitializeAutoLoot();
        }

        private void InitializeAutoCombat()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(333))
                .Where(_ => (AttachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn && AutoCombatEnabled)
                .Where(_ => !lootedInLastLoop)
                .Subscribe(_ => AutoCombatPerform());
        }
        private void AutoCombatPerform()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var character = new Character(AttachedGame.Game);
                if (character.SelectedTargetID == 0)
                    AC_AcquireTarget();
                else
                    AC_InvokeAttack();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An exception occured in {nameof(AutoLootPerform)} : {e}");
            }

            Debug.WriteLine("AutoCombat elapsed: " + stopwatch.ElapsedMilliseconds);
        }
        private void AC_AcquireTarget()
        {
            var npc = new NpcContainer(AttachedGame.Game).GetItems()
                .Where(x => x.NpcType.Value == NpcType.Monster)
                .OrderBy(x => x.RelativeDistance.Value)
                .FirstOrDefault();

            if (npc != null)
            {
                npc.Target();
                AC_InvokeAttack();
            }
        }
        private void AC_InvokeAttack()
        {
            new Character(AttachedGame.Game).Attack();
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
                    .Where(x => x.CollectMethod.Value == CollectMethod.Gold)
                    .Where(x => x.RelativeDistance <= MaxLootRange)
                    .OrderBy(x => x.RelativeDistance.Value)
                    .FirstOrDefault();

                if (item != null)
                {
                    item.Loot();
                }

                lootedInLastLoop = item != null;
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
