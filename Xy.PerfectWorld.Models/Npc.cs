using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class NpcContainer : Entity
    {
        public NpcContainer(Game game)
        {
            NpcBase = game.NpcBase;
        }

        [BaseAddress]
        public Pointer NpcBase { get; }

        [Hexadecimal]
        public Pointer FirstItem { get { return NpcBase + 0x18; } }
        public Pointer<int> MaxSize { get { return NpcBase + 0x24; } }

        public IEnumerable<Npc> GetItems()
        {
            return Enumerable.Range(0, MaxSize)
                .Select(i => new Npc(FirstItem + i * 0x4))
                .Where(i => i.NpcBase.Value != 0)
                //.Where(i => i.ID != 0 && Enum.IsDefined(typeof(NpcType), i.NpcType.Value))
                ;
        }
    }
    public class Npc: Entity
    {
        internal Npc(Pointer npc)
        {
            NpcBase = npc + 0x4;
        }

        [BaseAddress]
        public Pointer NpcBase { get; }

        [Hexadecimal]
        public Pointer<int> ID { get { return NpcBase + 0x11C; } }
        public Pointer<int> HP { get { return NpcBase + 0x120; } }
        public Pointer<int> Level { get { return NpcBase + 0x124; } }
        public Pointer<WString> Name { get { return NpcBase + 0x230; } }

        public override string ToString()
        {
            return $"[{NpcBase.Value.ToString("X")}]" + string.Join(", "
                   , $"{nameof(ID)} -> {ID.Value.ToString("X8")}"
                   , $"{nameof(Level)} = {Level.Value.ToString("D4")}"
                   , $"{nameof(Name)} = {Name.Value}"
                );
        }
    }
}
