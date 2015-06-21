using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class GroundContainer : SingletonEntity<GroundContainer>
    {
        [BaseAddress]
        public Pointer GroundBase { get; } = Environment.Instance.EnvironmentBase + 0x28;

        [Hexadecimal]
        public Pointer FirstItem { get { return GroundBase + 0x18; } }
        [Hexadecimal]
        public Pointer LastItem { get { return GroundBase + 0x1C; } }

        public Pointer<int> MaxSize { get { return GroundBase + 0x20; } }

        public IEnumerable<GroundItem> GetItems()
        {
            return Enumerable.Range(0, MaxSize)
                .Select(i => new GroundItem(FirstItem + i * 0x4))
                .Where(i => i.LootBase.Value != 0)
                //.Where(i => i.ID != 0 && Enum.IsDefined(typeof(NpcType), i.NpcType.Value))
                ;
        }
    }

    public class GroundItem : Entity
    {
        internal GroundItem(Pointer npc)
        {
            LootBase = npc + 0x4;
        }

        [BaseAddress]
        public Pointer LootBase { get; }

        public Pointer<float> X { get { return LootBase + 0x03C; } }
        public Pointer<float> Y { get { return LootBase + 0x040; } }
        public Pointer<float> Z { get { return LootBase + 0x044; } }

        [Hexadecimal]
        public Pointer<int> ID { get { return LootBase + 0x010C; } }
        public Pointer<int> Type { get { return LootBase + 0x0110; } }

        public Pointer<GroundItemType> Asd { get { return LootBase + 0x014C; } }
        public Pointer<int> QWe2 { get { return LootBase + 0x0110; } }

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

        public override string ToString()
        {
            return $"[{LootBase.Value.ToString("X")}]" + string.Join(",",
                   $"{nameof(ID)} -> {ID.Value.ToString("X8")}",
                   $"{nameof(Type)} = {Type.Value.ToString("X4")}",
                   $"{nameof(Name)} = {Name.Value}"
                );
        }
    }

    public enum GroundItemType
    {
        Loot = 1, Resource = 2, Gold = 3
    }
    public enum CollectMethod
    {
        None = 0,
    }
}
