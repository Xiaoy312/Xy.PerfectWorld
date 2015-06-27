using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class GroundContainer : Entity
    {
        public GroundContainer(Game game)
        {
            GroundBase = game.GroundBase;
        }

        [BaseAddress]
        public Pointer GroundBase { get; }

        [Hexadecimal]
        public Pointer FirstItem { get { return GroundBase + 0x18; } }
        [Hexadecimal]
        public Pointer LastItem { get { return GroundBase + 0x1C; } }

        public Pointer<int> MaxSize { get { return GroundBase + 0x20; } }

        public IEnumerable<GroundItem> GetItems()
        {
            var core = GroundBase.Core;
            var size = MaxSize.Value;
            var first = FirstItem.Value;

            for (int i = 0; i < size; i++)
            {
                var iterator = core.ReadInt(first + i * 4);
                if (iterator == 0) continue;
                var pitem = core.ReadInt(iterator + 4);
                if (pitem == 0) continue;

                yield return new GroundItem(core, iterator + 4);
            }
        }
    }

    public class GroundItem : Entity
    {
        internal GroundItem(Pointer address)
        {
            LootBase = address + 0x4;
        }
        internal GroundItem(Core core, int address)
        {
            LootBase = Pointer.FromStaticAddress(core, address);
        }

        [BaseAddress]
        public Pointer LootBase { get; }

        public Pointer<float> X { get { return LootBase + 0x03C; } }
        public Pointer<float> Y { get { return LootBase + 0x040; } }
        public Pointer<float> Z { get { return LootBase + 0x044; } }

        [Hexadecimal]
        public Pointer<uint> UniqueID { get { return LootBase + 0x010C; } }
        public Pointer<int> ItemID { get { return LootBase + 0x0110; } }

        public Pointer<CollectMethod> CollectMethod { get { return LootBase + 0x014C; } }

        public Pointer<float> RelativeDistance { get { return LootBase + 0x154; } }
        /// <summary>
        /// Relative distance, using only x and y axis, ignoring z axis(height)
        /// </summary>
        public Pointer<float> PlanarDistance { get { return LootBase + 0x158; } }
        /// <summary>
        /// Distance from camera
        /// </summary>
        public Pointer<float> VisualDistance { get { return LootBase + 0x15C; } }
        
        public Pointer<GB2312> ModelPath { get { return LootBase + 0x160; } }
        public Pointer<WString> Name { get { return LootBase + 0x0164; } }
        public Pointer<byte> IsSelected { get { return LootBase + 0x0169; } }

        public override string ToString()
        {
            return $"[{LootBase.Value.ToString("X")}]" + string.Join(", "
                   , $"{nameof(UniqueID)} -> {UniqueID.Value.ToString("X8")}"
                   , $"{nameof(RelativeDistance)} = {RelativeDistance.Value.ToString("0.000")}"
                   , $"{nameof(ItemID)} = {ItemID.Value.ToString("X4")}"
                   , $"{nameof(Name)} = {Name.Value}"
                );
        }
    }

    public enum CollectMethod
    {
        Loot = 1, Resource = 2, Gold = 3
    }
}
