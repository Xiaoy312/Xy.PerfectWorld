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
            var core = NpcBase.Core;
            var size = MaxSize.Value;
            var first = FirstItem.Value;

            for (int i = 0; i < size; i++)
            {
                var iterator = core.ReadInt(first + i * 4);
                if (iterator == 0) continue;
                var pitem = core.ReadInt(iterator + 4);
                if (pitem == 0) continue;

                yield return new Npc(core, iterator + 4);
            }
        }
    }
    public class Npc : Entity
    {
        internal Npc(Pointer npc)
        {
            NpcBase = npc + 0x4;
        }
        internal Npc(Core core, int address)
        {
            NpcBase = Pointer.FromStaticAddress(core, address);
        }

        [BaseAddress]
        public Pointer NpcBase { get; }
        
        public Pointer<NpcType> NpcType { get { return NpcBase + 0x0B4; } }
        [Hexadecimal]
        public Pointer<uint> UniqueID { get { return NpcBase + 0x11C; } }
        public Pointer<int> NpcID { get { return NpcBase + 0x120; } }
        public Pointer<int> Level { get { return NpcBase + 0x124; } }
        public Pointer<int> HP { get { return NpcBase + 0x12C; } }
        public Pointer<int> MaxHP { get { return NpcBase + 0x154; } }
        public Pointer<WString> Name { get { return NpcBase + 0x230; } }
        public Pointer<float> RelativeDistance { get { return NpcBase + 0x254; } }

        public override string ToString()
        {
            return $"[{NpcBase.Value.ToString("X")}]" + string.Join(", "
                   , $"{nameof(UniqueID)} -> {UniqueID.Value.ToString("X8")}"
                   , $"{nameof(NpcID)} -> {NpcID.Value.ToString("X4")}"
                   , $"{nameof(Level)} = {Level.Value.ToString().PadLeft(3)}"
                   , $"{nameof(Name)} = {Name.Value}"
                   , $"{nameof(Name)} = {Name.Value}"
                );
        }
    }

    public enum NpcType
    {
        Monster = 6, Npc = 7, Pet = 9, GM = 10
    }
}
