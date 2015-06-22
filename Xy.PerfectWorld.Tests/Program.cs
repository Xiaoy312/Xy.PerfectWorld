using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var core = Core.Attach(Process.GetProcessesByName("elementclient").First());

            var game = new Game(core);
            //game.DumpProperties();

            var character = new Character(game);
            //new NpcContainer(game).DumpProperties();
            //new GroundContainer(game).DumpProperties();

            //new GroundContainer(game).GetItems();

            foreach (var item in new NpcContainer(game).GetItems()
                .Where(x => x.NpcType.Value == NpcType.Monster).Take(1))
            {
                //if (item.UniqueID != character.SelectedTargetID) continue;

                Debug.WriteLine(item);
                item.DumpProperties();

                item.Target();
                character.Attack();
            }
        }
    }
}