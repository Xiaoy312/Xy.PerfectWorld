using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using MoreLinq;
using Xy.PerfectWorld.Models;
using Splat;

namespace Xy.PerfectWorld.ViewModels
{
    public class ClientSelectorViewModel : ReactiveObject
    {
        public ReactiveList<GameModel> GameClients { get; }
        public ReactiveCommand<GameModel> Attach { get; }

        public ClientSelectorViewModel()
        {
            GameClients = new ReactiveList<GameModel>();

            Attach = ReactiveCommand.CreateAsyncTask(x => Task.Run(() => (GameModel)x));
            Attach.Subscribe(model =>
            {
                Locator.CurrentMutable.RegisterConstant(model, typeof(GameModel));
            });

            Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(_ => RefreshGameClients());
        }

        private void RefreshGameClients()
        {
            // remove game processes that have exited
            var exited = GameClients.Where(x => x.Process.HasExited).ToList();
            GameClients.RemoveAll(exited);

            // add new game processes
            Process.GetProcessesByName("elementclient")
                .Where(p => !GameClients.Any(g => p.Id == g.Process.Id))
                .Select(x => new GameModel(x))
                .ForEach(GameClients.Add);

            if (GameClients.Count == 1)
                Attach.ExecuteAsync(GameClients.First());
        }
    }
}
