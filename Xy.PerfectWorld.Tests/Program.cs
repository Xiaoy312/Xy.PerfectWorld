using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;
using Xy.PerfectWorld.Models;
using MoreLinq;

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

            foreach (var item in new GroundContainer(game).GetItems()
                .OrderBy(x => x.ItemID)
                .DistinctBy(x => x.ItemID)
                )
            {
                switch (item.CollectMethod.Value)
                {
                    case CollectMethod.Gold: continue;
                    case CollectMethod.Resource: continue;
                }

                switch (item.ItemID.Value)
                {
                    case 0x527E: // Martial God·Virtuous Stele
                    case 0x527F: // Martial God·Virtuous Stone
                    case 0x5280: // Martial God·Manjushri Stele
                    case 0x5281: // Martial God·Manjushri Stone
                        continue;
                }

                Debug.WriteLine($"case 0x{item.ItemID.Value.ToString("X4")}: // {item.Name.Value}");
            }
        }
    }
}