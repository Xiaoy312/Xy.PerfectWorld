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
        public static void Loot(this Game game, GroundItem item)
        {
            var asm = new ASM();
            {
                asm.Pushad();
                
                asm.Push68(item.Type);
                asm.Push68(item.ID);

                asm.Mov_EDX_DWORD_Ptr(game.GameBase.Address);
                asm.Mov_ECX_DWORD_Ptr_EDX_Add(0x20);
                asm.Add_ECX(0x0EC);

                asm.Mov_EAX(0x00584510);
                asm.Call_EAX();

                asm.Popad();
                asm.Ret();
            }
            asm.Run(item.LootBase.Core);
        }
    }
}
