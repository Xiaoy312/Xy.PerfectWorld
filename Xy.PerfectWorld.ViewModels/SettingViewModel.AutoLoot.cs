using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Xy.PerfectWorld.Models;
using MoreLinq;
using Splat;
using Xy.PerfectWorld.Services;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Concurrent;

namespace Xy.PerfectWorld.ViewModels
{
    public partial class SettingViewModel : ReactiveObject
    {
        #region public bool LootGold
        private bool m_LootGold;
        public bool LootGold
        {
            get { return m_LootGold; }
            set { this.RaiseAndSetIfChanged(ref m_LootGold, value); }
        }
        #endregion

        public ReactiveList<LootInfo> LootInfos { get; set; }
        public ConcurrentBag<LootInfo> ConcurrentLootInfos { get; set; }
        public ReactiveList<LootInfo> NearbyLootInfos { get; set; }

        public ReactiveCommand<Unit> RefreshNearbyLootInfos { get; set; }
        public ReactiveCommand<Unit> AddLootInfo { get; set; }
        public ReactiveCommand<Unit> RemoveLootInfo { get; set; }
        public ReactiveCommand<Unit> ClearLootInfos { get; set; }
        public ReactiveCommand<Unit> ImportLootInfos { get; set; }
        public ReactiveCommand<Unit> ExportLootInfos { get; set; }

        partial void InitializeAutoLoot()
        {
            var dialogService = Locator.Current.GetService<IDialogService>();
            var game = Locator.Current.GetService<GameModel>();

            LootGold = true;

            // load the default loot table, if available
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"res\default.loots");
            if (File.Exists(path))
            {
                var serializer = new XmlSerializer(typeof(ReactiveList<LootInfo>));
                using (var stream = File.OpenRead(path))
                    LootInfos = serializer.Deserialize(stream) as ReactiveList<LootInfo>;
            }
            LootInfos = LootInfos ?? new ReactiveList<LootInfo>();
            NearbyLootInfos = new ReactiveList<LootInfo>();

            var canRefreshNearbyLootInfos = game.WhenAnyValue(x => x.Status, x => x == GameStatus.LoggedIn);
            RefreshNearbyLootInfos = ReactiveCommand.CreateAsyncTask(canRefreshNearbyLootInfos, async _ =>
            {
                using (NearbyLootInfos.SuppressChangeNotifications())
                {
                    NearbyLootInfos.Clear();
                    NearbyLootInfos.AddRange(new GroundContainer(game.Game).GetItems()
                        // filter out gold and gatherable resources
                        .Where(x => x.CollectMethod == CollectMethod.Loot)
                        .DistinctBy(x => x.ItemID.Value)
                        .Select(x => new LootInfo { ItemID = x.ItemID, Name = x.Name.Value }));
                }

                await Task.Yield();
            });

            AddLootInfo = ReactiveCommand.CreateAsyncTask(async parameter =>
            {
                var info = (LootInfo)parameter;

                lock (LootInfos)
                    if (!LootInfos.Any(x => x.ItemID == info.ItemID))
                        LootInfos.Add(info);

                await Task.Yield();
            });
            RemoveLootInfo = ReactiveCommand.CreateAsyncTask(async parameter =>
            {
                lock (LootInfos)
                    LootInfos.Remove((LootInfo)parameter);

                await Task.Yield();
            });
            ClearLootInfos = ReactiveCommand.CreateAsyncTask(async parameter =>
            {
                lock (LootInfos)
                    using (LootInfos.SuppressChangeNotifications())
                        LootInfos.Clear();

                await Task.Yield();
            });

            var canExportLootInfos = LootInfos.CountChanged.Select(x => x != 0);
            ExportLootInfos = ReactiveCommand.CreateAsyncTask(canExportLootInfos, async _ =>
            {
                var dialog = new SaveFileDialog() { Filter = Properties.Resources.LootInfoFileFilter };
                if (dialog.ShowDialog() == false) return;

                var serializer = new XmlSerializer(LootInfos.GetType());
                using (var stream = dialog.OpenFile())
                    lock (LootInfos)
                        serializer.Serialize(stream, LootInfos);

                await Task.Yield();
            });
            ExportLootInfos.ThrownExceptions.Subscribe(async e => await dialogService.DisplayExceptionAsyncOn<SettingViewModel>(e));

            ImportLootInfos = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var dialog = new OpenFileDialog() { Filter = Properties.Resources.LootInfoFileFilter };
                if (dialog.ShowDialog() == false) return;

                if (LootInfos.Any())
                {
                    var result = await dialogService.ShowMessageDialogAsyncOn<SettingViewModel>("Confirmation",
                        "Do you wish to merge the current list with this list?",
                        "Merge them",
                        "Keep new list",
                        "Cancel, keep current list");
                    switch (result)
                    {
                        case DialogResult.Affirmative:
                            break;
                        case DialogResult.Negative:
                            using (LootInfos.SuppressChangeNotifications())
                                lock (LootInfos)
                                    LootInfos.Clear();
                            break;
                        case DialogResult.FirstAuxiliary:
                            return;
                    }
                }

                var serializer = new XmlSerializer(LootInfos.GetType());
                using (var stream = dialog.OpenFile())
                    lock (LootInfos)
                    {
                        var list = serializer.Deserialize(stream) as ReactiveList<LootInfo>;
                        foreach (var item in list)
                            if (!LootInfos.Any(x => x.ItemID == item.ItemID))
                                LootInfos.Add(item);
                    }

                await Task.Yield();
            });
            ImportLootInfos.ThrownExceptions.Subscribe(async e => await dialogService.DisplayExceptionAsyncOn<SettingViewModel>(e));
        }

        public class LootInfo : ReactiveObject
        {
            #region public int ItemID
            private int m_ItemID;
            public int ItemID
            {
                get { return m_ItemID; }
                set { this.RaiseAndSetIfChanged(ref m_ItemID, value); }
            }
            #endregion
            #region public string Name
            private string m_Name;
            public string Name
            {
                get { return m_Name; }
                set { this.RaiseAndSetIfChanged(ref m_Name, value); }
            }
            #endregion
        }
    }
}
