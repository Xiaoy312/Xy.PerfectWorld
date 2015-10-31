using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Models;
using Xy.PerfectWorld.Services;

namespace Xy.PerfectWorld.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private SettingViewModel settingVM = Locator.Current.GetService<SettingViewModel>();
        private GameModel attachedGame;

        #region AutoCombatEnabled
        bool autoCombatEnabled = false;
        public bool AutoCombatEnabled
        {
            get { return autoCombatEnabled; }
            set { this.RaiseAndSetIfChanged(ref autoCombatEnabled, value); }
        }
        #endregion
        #region public bool AutoSparkEnabled
        private bool m_AutoSparkEnabled;
        public bool AutoSparkEnabled
        {
            get { return m_AutoSparkEnabled; }
            set { this.RaiseAndSetIfChanged(ref m_AutoSparkEnabled, value); }
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
        #region TargetInfo
        string targetInfo;
        public string TargetInfo
        {
            get { return targetInfo; }
            set { this.RaiseAndSetIfChanged(ref targetInfo, value); }
        }
        #endregion

        private bool lootedInLastLoop;
        private Npc currentTarget;

        public ReactiveCommand<object> ShowSetting { get; private set; }

        public MainViewModel()
        {
            attachedGame = Locator.Current.GetService<GameModel>();

            ShowSetting = ReactiveCommand.Create();
            ShowSetting.Subscribe(_ =>
            {
                Locator.Current.GetService<IViewService>().ShowViewFor<SettingViewModel>();
            });

            InitializeAutoCombat();
            InitializeAutoLoot();
            InitializeStatusBar();
        }

        private void InitializeAutoCombat()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(333))
                .Where(_ => (attachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn && AutoCombatEnabled)
                .Where(_ => !lootedInLastLoop)
                .Subscribe(_ => AutoCombatPerform());
        }
        private void AutoCombatPerform()
        {
            try
            {
                var character = new Character(attachedGame.Game);
                var mobs = new NpcContainer(attachedGame.Game).GetItems()
                    .Where(x => x.NpcType.Value == NpcType.Monster);
                IEnumerable<Npc> targets = null;

                switch (settingVM.SearchBehavior)
                {
                    case AutoCombatSearchBehavior.SearchAndDestroy:
                        targets = mobs;
                        break;

                    case AutoCombatSearchBehavior.SelfDefence:
                        targets = Enumerable.Empty<Npc>();
                        break;

                    case AutoCombatSearchBehavior.Custom:
                        targets = mobs.Where(x => settingVM.TargetList.Contains(x.Name.Value.Value));
                        break;
                }

                // reacquire closest target, before attacking
                var mob = targets
                    .OrderBy(x => x.RelativeDistance.Value)
                    .FirstOrDefault();
                if (mob != null && mob.UniqueID != character.SelectedTargetID)
                    mob.Target();

                if (character.SelectedTargetID != 0)
                {
                    if (AutoSparkEnabled && character.Chi > 300)
                    {
                        var sparkBurstID = new SkillBook(attachedGame.Game).GetItems()
                            .Select(x => x.SkillID.Value)
                            .FirstOrDefault(x => 0x16A <= x && x <= 0x175); // skill id for holy/demonic spark burst

                        if (sparkBurstID != default(int))
                        {
                            Call.Cast(attachedGame.Game.Core, sparkBurstID);
                            return;
                        }
                    }

                    character.Attack();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An exception occured in {nameof(AutoLootPerform)} : {e}");
            }
        }

        private void InitializeAutoLoot()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Where(_ => (attachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn && AutoLootEnabled)
                .Subscribe(_ => AutoLootPerform());
        }
        private void AutoLootPerform()
        {
            try
            {
                const float MaxLootRange = 10.0f;
                var canLootGold = new Character(attachedGame.Game).Gold != 200000000;
                var ground = new GroundContainer(attachedGame.Game);

                GroundItem item;
                lock (settingVM.LootInfos)
                    item = ground.GetItems()
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
                case CollectMethod.Gold: return settingVM.LootGold && canLootGold;
                case CollectMethod.Resource: return false;
            }

            return settingVM.LootInfos.Any(x => x.ItemID == item.ItemID);
        }

        private void InitializeStatusBar()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Where(_ => (attachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn)
                .Subscribe(_ => UpdateTarget());
            Observable.Interval(TimeSpan.FromMilliseconds(125))
                .Where(_ => (attachedGame?.Status ?? GameStatus.Offline) == GameStatus.LoggedIn)
                .Subscribe(_ => UpdateTargetInfo());
        }
        private void UpdateTarget()
        {
            var character = new Character(attachedGame.Game);
            var npcs = new NpcContainer(attachedGame.Game);

            currentTarget = npcs.GetItemByID(character.SelectedTargetID);
        }
        private void UpdateTargetInfo()
        {
            // save a temp instance to prevent race condition from UpdateTarget()
            var target = currentTarget;

            if (target == null || target.NpcBase.Value == 0)
            {
                TargetInfo = "Target: null";
                return;
            }

            var maxHP = target.MaxHP;

            TargetInfo = string.Format("Target: {0:P2} {1:N0}",
                maxHP != 0 ? (double)target.HP.Value / target.MaxHP.Value : 0,
                target.HP.Value);
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
