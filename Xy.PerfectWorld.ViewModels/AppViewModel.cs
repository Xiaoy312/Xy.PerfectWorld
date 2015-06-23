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
            try
            {
                var character = new Character(AttachedGame.Game);

                // reacquire closest target, before attacking
                var npc = new NpcContainer(AttachedGame.Game).GetItems()
                    .Where(x => x.NpcType.Value == NpcType.Monster)
                    .OrderBy(x => x.RelativeDistance.Value)
                    .FirstOrDefault();
                if (npc.UniqueID != character.SelectedTargetID)
                    npc.Target();

                character.Attack();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An exception occured in {nameof(AutoLootPerform)} : {e}");
            }
        }
        private void InitializeAutoLoot()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Where(_ => (AttachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn && AutoLootEnabled)
                .Subscribe(_ => AutoLootPerform());
        }
        private void AutoLootPerform()
        {
            try
            {
                const float MaxLootRange = 10.0f;
                var canLootGold = new Character(AttachedGame.Game).Gold != 200000000; 
                var ground = new GroundContainer(AttachedGame.Game);

                var item = ground.GetItems()
                    .Where(x => ShouldLoot(x, canLootGold))
                    .Where(x => x.RelativeDistance <= MaxLootRange)
                    .OrderBy(x => x.RelativeDistance)
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
        private bool ShouldLoot(GroundItem item, bool canLootGold = true)
        {
            switch (item.CollectMethod.Value)
            {
                case CollectMethod.Gold: return canLootGold;
                case CollectMethod.Resource: return false;
            }

            switch (item.ItemID.Value)
            {
                case 0x527E: // Martial God·Virtuous Stele
                case 0x527F: // Martial God·Virtuous Stone
                case 0x5280: // Martial God·Manjushri Stele
                case 0x5281: // Martial God·Manjushri Stone
                case 0x5286: // Martial God·Guanyin Stele
                case 0x5287: // Martial God·Guanyin Stone
                    return true;
            }

            return false;
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
