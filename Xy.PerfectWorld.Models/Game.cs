using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class Game : Entity
    {
        public const int GameBaseAddress = 0x926FD4;

        public Core Core { get; }
        public Game(Core core)
        {
            Core = core;
            GameBase = Pointer.FromStaticAddress(core, GameBaseAddress);
        }

        [BaseAddress]
        public Pointer GameBase { get; }

        public Pointer<GameStatus> GameStatus { get { return Pointer.FromStaticAddress(Core, 0x0092AAC0); } }
        
        public Pointer DynamicBase { get { return GameBase + 0x1C; } }

        public Pointer EnvironmentBase { get { return DynamicBase + 0x8; } }
        public Pointer PlayerBase { get { return EnvironmentBase + 0x20; } } // other players
        public Pointer NpcBase { get { return EnvironmentBase + 0x24; } }
        public Pointer GroundBase { get { return EnvironmentBase + 0x28; } }

        public Pointer CharacterBase { get { return DynamicBase + 0x20; } }
        public Pointer GuildBase { get { return CharacterBase + 0x6B4; } }
        public Pointer PartyBase { get { return CharacterBase + 0x66C; } }
        public Pointer InventoryBase { get { return CharacterBase + 0xB4C; } }
        public Pointer EquipmentBase { get { return CharacterBase + 0xB50; } }
        public Pointer PetBase { get { return CharacterBase + 0xBCC; } }
        public Pointer SkillBase { get { return CharacterBase + 0xBDC; } }
        public Pointer<int> SkillCount { get { return CharacterBase + 0xBE0; } }
    }

    public enum GameStatus
    {
        Unknown = 0, Offline = 1, LoggedIn = 2
    }
}
