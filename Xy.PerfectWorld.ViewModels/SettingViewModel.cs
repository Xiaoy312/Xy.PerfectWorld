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
    public class SettingViewModel : ReactiveObject
    {
        public ReactiveList<GameModel> RunningGames { get; }

        GameModel selectedGame;
        public GameModel SelectedGame
        {
            get { return selectedGame; }
            set { this.RaiseAndSetIfChanged(ref selectedGame, value); }
        }

        public ReactiveCommand<object> AttachToGame { get; }

        public SettingViewModel()
        {
            RunningGames = new ReactiveList<GameModel>();

            Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(_ => RefreshGames());

            RunningGames.CountChanged
                .Where(count => count != 0 && SelectedGame == null)
                .Subscribe(_ => SelectedGame = RunningGames.FirstOrDefault());
        }

        public void RefreshGames()
        {
            var exited = RunningGames.Where(x => x.Process.HasExited).ToList();
            RunningGames.RemoveAll(exited);

            Process.GetProcessesByName("elementclient")
                .Where(p => !RunningGames.Any(g => p.Id == g.Process.Id))
                .Select(x => new GameModel(x))
                .ForEach(RunningGames.Add);
        }
    }


}
