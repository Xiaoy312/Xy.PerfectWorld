using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Models;
using Xy.PerfectWorld.Services;

namespace Xy.PerfectWorld.ViewModels
{
    public partial class SettingViewModel : ReactiveObject
    {
        #region public bool? Unfreeze
        private bool? m_Unfreeze;
        public bool? Unfreeze
        {
            get { return m_Unfreeze; }
            private set { this.RaiseAndSetIfChanged(ref m_Unfreeze, value); }
        }
        #endregion

        public ReactiveCommand<object> ToggleUnfreeze { get; private set; }

        partial void InitializeFeatures()
        {
            // !(bool?) is same as !bool, with null still being null
            ToggleUnfreeze = ReactiveCommand.Create();
            ToggleUnfreeze.Subscribe(_ => Unfreeze = UpdateUnfreeze(!Unfreeze));

            // refresh current state without toggling
            Unfreeze = UpdateUnfreeze(null);
        }

        /// <summary>
        /// Update the unfreeze AOB
        /// </summary>
        /// <param name="value">Enabled unfreeze when true; reset to original state if false; do nothing if null</param>
        /// <returns>Whether or not unfreeze has been enabled based on AOB, null when the AOB is corrupted or the offset has changed</returns>
        private bool? UpdateUnfreeze(bool? value)
        {
            const int UnfreezeAddress = 0x0042BA4C;
            // 0042BA4C - 88 85 18040000  - mov [ebp+00000418],al; update [IsClientFocused] on alt-tab
            var pIsClientFocused = game.GameBase + 0x418;
            var disabledAOB = new byte[] { 0x88, 0x85, 0x18, 0x04, 0x00, 0x00 };
            var enabledAOB = Enumerable.Repeat<byte>(0x90, disabledAOB.Length).ToArray();

            // check if aob are correct
            var aob = core.ReadBytes(UnfreezeAddress, disabledAOB.Length);
            if (!aob.SequenceEqual(disabledAOB) && !aob.SequenceEqual(enabledAOB))
                return null;

            // update the aob if there is a value
            if (value.HasValue)
            {
                core.WriteBytes(UnfreezeAddress, value.Value ? enabledAOB : disabledAOB);
                core.WriteBytes(pIsClientFocused.Address, new [] { (byte)(value.Value ? 1 : 0) });
            }

            // re-read the aob and report the result
            aob = core.ReadBytes(UnfreezeAddress, disabledAOB.Length);
            if (aob.SequenceEqual(enabledAOB))
                return true;
            else if (aob.SequenceEqual(disabledAOB))
                return false;
            else return null; // just in case
        }
    }
}
