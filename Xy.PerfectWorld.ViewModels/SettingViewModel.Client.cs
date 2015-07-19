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
        public ReactiveList<GameModel> RunningGames { get; set; }
        public ReactiveCommand<object> AttachToGame { get; set; }

        GameModel selectedGame;
        public GameModel SelectedGame
        {
            get { return selectedGame; }
            set { this.RaiseAndSetIfChanged(ref selectedGame, value); }
        }

        partial void InitializeClient()
        {
            RunningGames = new ReactiveList<GameModel>();

            // if not attached, auto attach to a game process as soon one becomes available
            RunningGames.CountChanged
                .Where(count => count != 0 && SelectedGame == null)
                .Subscribe(_ => SelectedGame = RunningGames.FirstOrDefault());

            // keep game processes list refreshed
            Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(_ => RefreshGames());
        }

        private void RefreshGames()
        {
            // remove game processes that have exited
            var exited = RunningGames.Where(x => x.Process.HasExited).ToList();
            RunningGames.RemoveAll(exited);

            // add new game processes
            Process.GetProcessesByName("elementclient")
                .Where(p => !RunningGames.Any(g => p.Id == g.Process.Id))
                .Select(x => new GameModel(x))
                .ForEach(RunningGames.Add);
        }
    }
}
