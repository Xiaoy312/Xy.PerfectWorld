using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;
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
                asm.Push68((int)item.UniqueID.Value);

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

                asm.Push68((int)npc.UniqueID.Value);

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
        public static void Cast(Core core, int skillID)
        {
            //00492AFA    8B0D D46F9200     mov ecx,dword ptr ds:[926FD4]              ; elementc.00927630
            //00492B00    8B40 08           mov eax,dword ptr ds:[eax+8]
            //00492B03    6A FF             push -1
            //00492B05    6A 00             push 0
            //00492B07    8B51 1C           mov edx,dword ptr ds:[ecx+1C]
            //00492B0A    6A 00             push 0
            //00492B0C    50                push eax
            //00492B0D    8B4A 20           mov ecx,dword ptr ds:[edx+20]
            //00492B10    E8 BB2EFCFF       call elementc.004559D0

            var asm = new ASM();
            {
                asm.Pushad();

                asm.Push6A(-1);
                asm.Push6A(0);
                asm.Push6A(0);
                asm.Mov_EDX(skillID);
                asm.Push_EDX();

                asm.Mov_ECX_DWORD_Ptr(Game.GameBaseAddress);
                asm.Mov_ECX_DWORD_Ptr_ECX_Add(0x1C);
                asm.Mov_ECX_DWORD_Ptr_ECX_Add(0x20);

                asm.Mov_EAX(0x004559D0);
                asm.Call_EAX();

                asm.Popad();
                asm.Ret();
            };
            asm.Run(core);
        }
        public static void Cast(this Skill skill)
        {
            Cast(skill.SkillBase.Core, skill.SkillID);
        }
    }
}
