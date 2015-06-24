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
            character.DumpProperties();
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

                var filter = new List<Predicate<string>>
                {
                    x => x.Contains("Restoration Powder"),
                    x => x.Contains("Aloeswood Bolus"),
                };
                if (filter.Any(x => x(item.Name.Value.Value)))
                    continue;

                switch (item.ItemID.Value)
                {
                    case 0x527A: // Martial God·Ksitigarbha Stele
                    case 0x527B: // Martial God·Ksitigarbha Stone
                    case 0x527C: // Martial God·Steel Stele
                    
                    case 0x527E: // Martial God·Virtuous Stele
                    case 0x527F: // Martial God·Virtuous Stone
                    case 0x5280: // Martial God·Manjushri Stele
                    case 0x5281: // Martial God·Manjushri Stone
                    case 0x5282: // Martial God·Hollow Stele
                    case 0x5283: // Martial God·Hollow Stone
                    case 0x5284: // Martial God·Removal Stele
                    case 0x5285: // Martial God·Removal Stone
                    case 0x5286: // Martial God·Guanyin Stele
                    case 0x5287: // Martial God·Guanyin Stone

                    case 0xD6D9: // g17 Ember 
                    case 0xD6DA: // g17 Pearl 
                        continue;
                }

                Debug.WriteLine($"case 0x{item.ItemID.Value.ToString("X4")}: // {item.Name.Value}");
            }
        }
    }
}