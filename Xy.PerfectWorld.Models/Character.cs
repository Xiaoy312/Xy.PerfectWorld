using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class Character : Entity
    {
        public Character(Game game)
        {
            CharacterBase = game.CharacterBase;
        }

        [BaseAddress]
        public Pointer CharacterBase { get; }

        [Hexadecimal]
        public Pointer<int> ID { get { return CharacterBase + 0x43C; } }
        public Pointer<int> Level { get { return CharacterBase + 0x448; } }
        public Pointer<Cultivation> Cultivation { get { return CharacterBase + 0x44C; } }

        public Pointer<int> CurrentHP { get { return CharacterBase + 0x450; } }
        public Pointer<int> CurrentMP { get { return CharacterBase + 0x454; } }
        public Pointer<int> Experience { get { return CharacterBase + 0x458; } }
        public Pointer<int> Spirit { get { return CharacterBase + 0x45C; } }
        public Pointer<int> AssignablePoint { get { return CharacterBase + 0x460; } }
        public Pointer<int> Chi { get { return CharacterBase + 0x464; } }
        
        public Pointer<int> Vitality { get { return CharacterBase + 0x468; } }
        public Pointer<int> Magic { get { return CharacterBase + 0x46C; } }
        public Pointer<int> Strength { get { return CharacterBase + 0x470; } }
        public Pointer<int> Dexterity { get { return CharacterBase + 0x474; } }

        public Pointer<int> MaxHP { get { return CharacterBase + 0x478; } }
        public Pointer<int> MaxMP { get { return CharacterBase + 0x47C; } }

        public Pointer<int> Accuracy { get { return CharacterBase + 0x498; } }
        public Pointer<int> MinPhyAttack { get { return CharacterBase + 0x49C; } }
        public Pointer<int> MaxPhyAttack { get { return CharacterBase + 0x4A0; } }
        public Pointer<int> MinMagAttack { get { return CharacterBase + 0x4D4; } }
        public Pointer<int> MaxMagAttack { get { return CharacterBase + 0x4D8; } }
        public Pointer<int> MetalResistance { get { return CharacterBase + 0x4DC; } }
        public Pointer<int> WoodResistance { get { return CharacterBase + 0x4E0; } }
        public Pointer<int> WaterResistance { get { return CharacterBase + 0x4E4; } }
        public Pointer<int> FireResistance { get { return CharacterBase + 0x4E8; } }
        public Pointer<int> EarthResistance { get { return CharacterBase + 0x4EC; } }
        public Pointer<int> PhysicalDefence { get { return CharacterBase + 0x4F0; } }
        public Pointer<int> Dodge { get { return CharacterBase + 0x4F4; } }
        public Pointer<int> Gold { get { return CharacterBase + 0x4FC; } }
        public Pointer<int> Reputation { get { return CharacterBase + 0x55C; } }
        public Pointer<WString> Name { get { return CharacterBase + 0x5CC; } }
        public Pointer<Class> Class { get { return CharacterBase + 0x5D4; } }

        public Pointer<int> GuildRank { get { return CharacterBase + 0x608; } }
        public Pointer<MoveMethod> MoveMethod { get { return CharacterBase + 0x64C; } }

        [Hexadecimal]
        public Pointer<int> SelectedTargetID { get { return CharacterBase + 0xA18; } }
        [Hexadecimal]
        public Pointer<int> MouseOverTargetID { get { return CharacterBase + 0xA30; } }
    }

    public enum Cultivation
    {
        筑基, 灵虚, 和合, 元婴, 空冥, 履霜, 渡劫, 寂灭, 大乘
    }
    public enum Class
    {
        WR = 0, // Warrior
        MG = 1, //Mage, Wizard
        /* unused */
        WF = 3, //Werefox, Venomancer
        WB = 4, //Werebeast, Barbarian
        /* unused */
        EA = 6, //Elf Archer, Archer
        EP = 7, //Elf Priest, Cleric
    }
    public enum MoveMethod
    {
        Walk, Swim, Fly
    }
}
