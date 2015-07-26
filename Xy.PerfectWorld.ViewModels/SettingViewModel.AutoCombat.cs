using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Models;
using Xy.PerfectWorld.Services;

namespace Xy.PerfectWorld.ViewModels
{
    public partial class SettingViewModel : ReactiveObject
    {
        #region public AutoCombatSearchBehavior SearchBehavior
        AutoCombatSearchBehavior searchBehavior;
        public AutoCombatSearchBehavior SearchBehavior
        {
            get { return searchBehavior; }
            set { this.RaiseAndSetIfChanged(ref searchBehavior, value); }
        }
        #endregion
        #region public string MonsterName
        string monsterName;
        public string MonsterName
        {
            get { return monsterName; }
            set { this.RaiseAndSetIfChanged(ref monsterName, value); }
        }
        #endregion
        #region public string SelectedTarget
        string selectedTarget;
        public string SelectedTarget
        {
            get { return selectedTarget; }
            set { this.RaiseAndSetIfChanged(ref selectedTarget, value); }
        }
        #endregion
        public ReactiveList<string> NearbyMonsters { get; set; }
        public ReactiveList<string> TargetList { get; set; }

        public ReactiveCommand<IEnumerable<string>> RefreshNearbyMonsters { get; set; }
        public ReactiveCommand<object> AddToTargetList { get; set; }
        public ReactiveCommand<object> ClearTargetList { get; set; }
        public ReactiveCommand<object> RemoveSelectedTarget { get; set; }
        public ReactiveCommand<Unit> ExportTargetList { get; set; }
        public ReactiveCommand<Unit> ImportTargetList { get; set; }

        partial void InitializeAutoCombat()
        {
            SearchBehavior = AutoCombatSearchBehavior.SearchAndDestroy;
            NearbyMonsters = new ReactiveList<string>();
            TargetList = new ReactiveList<string>();
            TargetList.CountChanged.Subscribe(_ => TargetList.Sort());

            var canRefreshNearbyMonsters = this.WhenAnyValue(x => x.SelectedGame, (GameModel x) => x != null);
            RefreshNearbyMonsters = ReactiveCommand.CreateAsyncTask(canRefreshNearbyMonsters, async _ =>
                {
                    var names = await Task.Run(() =>
                    {
                        return new NpcContainer(SelectedGame.Game).GetItems()
                            .Where(x => x.NpcType.Value == NpcType.Monster)
                            .Select(x => x.Name.Value.Value)
                            .Distinct();
                    });

                    return names;
                });
            RefreshNearbyMonsters.Subscribe(results =>
            {
                NearbyMonsters.Clear();
                foreach (var item in results)
                    NearbyMonsters.Add(item);
            });

            var canAddToTarget = this.WhenAnyValue(x => x.MonsterName, x => !string.IsNullOrWhiteSpace(x));
            AddToTargetList = ReactiveCommand.Create(canAddToTarget);
            AddToTargetList.Subscribe(_ =>
            {
                if (!TargetList.Contains(MonsterName))
                    TargetList.Add(MonsterName);
            });

            var canClearTargetList = TargetList.CountChanged.Select(x => x != 0);
            ClearTargetList = ReactiveCommand.Create(canClearTargetList);
            ClearTargetList.Subscribe(_ => TargetList.Clear());

            var canRemoveSelectedTarget = this.WhenAnyValue(x => x.SelectedTarget, x => !string.IsNullOrWhiteSpace(x));
            RemoveSelectedTarget = ReactiveCommand.Create(canRemoveSelectedTarget);
            RemoveSelectedTarget.Subscribe(_ => TargetList.Remove(selectedTarget));

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var dialogService = Locator.Current.GetService<IDialogService>();
            var canExportTargetList = TargetList.CountChanged.Select(x => x != 0);
            ExportTargetList = ReactiveCommand.CreateAsyncTask(canExportTargetList, async _ =>
            {
                var dialog = new SaveFileDialog()  { Filter = TargetListFileFormat };
                if (dialog.ShowDialog() == false) return;

                var serializer = new XmlSerializer(typeof(List<string>));
                using (var stream = dialog.OpenFile())
                    serializer.Serialize(stream, TargetList.ToList());
            });
            ExportTargetList.ThrownExceptions.Subscribe(dialogService.DisplayExceptionOn<SettingViewModel>);

            ImportTargetList = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var dialog = new OpenFileDialog() { Filter = TargetListFileFormat };
                if (dialog.ShowDialog() == false) return;

                if (TargetList.Any())
                {
                    // FIXME: Lord forgive me, for I've broken MVVM
                    var result = await dialogService.ShowMessageDialogOnAsync<SettingViewModel>("Confirmation",
                        "Do you wish to merge the current list with this list?",
                        "Merge them",
                        "Keep new list",
                        "Cancel, keep current list");
                    switch (result)
                    {
                        case DialogResult.Affirmative:
                            break;
                        case DialogResult.Negative:
                            TargetList.Clear();
                            break;
                        case DialogResult.FirstAuxiliary:
                            return;
                    }
                }

                var serializer = new XmlSerializer(typeof(List<string>));
                using (var stream = dialog.OpenFile())
                {
                    var list = serializer.Deserialize(stream) as List<string>;
                    foreach (var item in list)
                        if (!TargetList.Contains(item))
                            TargetList.Add(item);
                }
            });
            ImportTargetList.ThrownExceptions.Subscribe(dialogService.DisplayExceptionOn<SettingViewModel>);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        private const string TargetListFileFormat = "Target List|*.targets";
    }

    public enum AutoCombatSearchBehavior
    {
        SelfDefence, SearchAndDestroy, Custom
    }
}
