using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis.Legacy;

namespace Xy.PerfectWorld.Models
{
    public static class Call
    {
        public static void Loot(this GroundItem item)
        {
            var asm = new ASM();
            {
                asm.Pushad();

                asm.Push68(item.ItemID);
                asm.Push68(item.UniqueID);

                asm.Mov_EDX_DWORD_Ptr(Game.GameBaseAddress);
                asm.Mov_ECX_DWORD_Ptr_EDX_Add(0x20);
                asm.Add_ECX(0x0EC);

                asm.Mov_EAX(0x00584510);
                asm.Call_EAX();

                asm.Popad();
                asm.Ret();
            }
            asm.Run(item.LootBase.Core);
        }

        public static void Target(this Npc npc)
        {
            var asm = new ASM();
            {
                asm.Pushad();

                asm.Push68(npc.UniqueID);

                asm.Mov_EAX_DWORD_Ptr(Game.GameBaseAddress);
                asm.Mov_ECX_DWORD_Ptr_EAX_Add(0x20);
                asm.Add_ECX(0xEC);

                asm.Mov_EBX(0x584580);
                asm.Call_EBX();

                asm.Popad();
                asm.Ret();
            }
            asm.Run(npc.NpcBase.Core);
        }

        public static void Attack(this Character character)
        {
            var asm = new ASM();
            {
                asm.Pushad();

                asm.Mov_EAX(0x5A80C0);
                asm.Call_EAX();

                asm.Popad();
                asm.Ret();
            }
            asm.Run(character.CharacterBase.Core);

        }
    }
}
