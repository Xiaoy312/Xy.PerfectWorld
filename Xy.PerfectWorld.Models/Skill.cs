using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class SkillBook : Entity
    {
        public SkillBook(Game game)
        {
            SkillBase = game.SkillBase;
            SkillCount = game.SkillCount;
        }

        [BaseAddress]
        public Pointer SkillBase { get; }
        public Pointer<int> SkillCount { get; }

        public IEnumerable<Skill> GetItems()
        {
            var count = SkillCount.Value;
            for (int i = 0; i < count; i++)
            {
                yield return new Skill(SkillBase + i * 4);
            }
        }
    }

    public class Skill : Entity
    {
        public Skill(Pointer address)
        {
            SkillBase = address;
        }

        public Skill(Core core, int address)
        {
            SkillBase = Pointer.FromStaticAddress(core, address);
        }

        [BaseAddress]
        public Pointer SkillBase { get; }

        public Pointer<int> Level { get { return SkillBase + 0x0C; } }
        public Pointer<int> Cooldown { get { return SkillBase + 0x10; } }
        public Pointer<int> MaxCooldown { get { return SkillBase + 0x14; } }
        public Pointer<byte> State { get { return SkillBase + 0x18; } }

        public Pointer InfoBase { get { return SkillBase + 0x4 + 0x4; } }
        /// <remarks>Apparently only skill description is localized, but not this</remarks>
        public Pointer<WString> Name { get { return SkillBase + 0x4 + 0x4 + 0xC; } }
        public Pointer<int> RequiredChi { get { return SkillBase + 0x4 + 0x4 + 0x36; } }

        public override string ToString()
        {
            return $"[{SkillBase.Value.ToString("X")}]" + string.Join(", "
                   , $"{nameof(Cooldown)} -> {Cooldown.Value.ToString().PadLeft(7)}"
                   , $"{nameof(MaxCooldown)} -> {MaxCooldown.Value.ToString().PadLeft(7)}"
                   , $"{nameof(State)} -> {State.Value.ToString().PadLeft(3)}"
                   , $"{nameof(Name)} -> {Name.Value.Value}"
                );
        }
    }
}
