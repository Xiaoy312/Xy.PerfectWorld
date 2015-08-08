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
        static Core core;
        static Game game;
        static void Main(string[] args)
        {
            core = Core.Attach(Process.GetProcessesByName("elementclient").First());
            game = new Game(core);

            //DebugLoot();
            //DebugCurrentTarget();
            //DebugSkill();
            DebugNpc();
        }

        private static void DebugLoot()
        {
            var items = new GroundContainer(game).GetItems()
                .OrderBy(x => x.ItemID)
                .DistinctBy(x => x.ItemID)
                ;

            foreach (var item in items)
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
                    case 0x5DC0: // LM$ Silver

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
        private static void DebugCurrentTarget()
        {
            var character = new Character(game);
            var npcs = new NpcContainer(game);

            var id = character.SelectedTargetID.Value;
            // check if target is a mob
            if (id >= 0x80000000)
            {
                var index = id % npcs.MaxSize.Value;
                var npc = npcs[(int)index];

                if (npc.NpcBase.Value != 0)
                {

                }
            }
        }
        private static void DebugSkill()
        {
            var skillbook = new SkillBook(game);
            skillbook.DumpProperties();

            foreach (var skill in skillbook.GetItems())
            {
                Debug.WriteLine(skill);
            }
        }
        private static void DebugNpc()
        {
            var character = new Character(game);
            var npcs = new NpcContainer(game).GetItems();

            var target = new NpcContainer(game).GetItemByID(character.SelectedTargetID);
            Debug.WriteLine("*" + target);

            foreach(var npc in npcs.OrderBy(x => x.RelativeDistance))
            {
                Debug.WriteLine(npc);
            }
        }
    }
}