using StormSpiritRewrite.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects.UtilityObjects;
using StormSpiritRewrite.Utilities;
using SharpDX.Direct3D9;
using SharpDX;
using System.Windows.Input;

namespace StormSpiritRewrite.Features
{
    public class ManaDisplay
    {
        private Zip zip;

        private Hero me;

        private bool bDraw;

        private bool ManaDisplayEn;

        private int remainingMana;

        private static bool legacy
        {
            get
            {
                return Variables.LegacyOrQwer;
            }
        }  
 
        private Font _Font;

        public ManaDisplay()
        {
            _Font = new SharpDX.Direct3D9.Font(
            Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 33,
                Weight = FontWeight.Bold,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
        }

        public void Update()
        {
            this.me = Variables.Hero;
            this.zip = Variables.Zip;
        }

        public void Execute()
        {
            Update();
            var zipLvl = zip.level;
            int distance = (int)SharpDX.Vector3.Distance(Game.MousePosition, me.Position);
            var travelSpeed = (int)(625 * (zipLvl + 1));
            var startManaCost = 30 + me.MaximumMana * 0.08;
            var costPerUnit = (12 + me.MaximumMana * 0.007) / 100.0;
            int totalCost = (int)(startManaCost + costPerUnit * (int)Math.Floor((decimal)distance / 100) * 100);
            var travelTime = distance / travelSpeed;
            remainingMana = (int)(me.Mana - totalCost + (me.ManaRegeneration * (travelTime + 1)));
            if (remainingMana > me.MaximumMana) return;
            if (ManaDisplayEn)
            {
                bDraw = true;
            }
            else
            {
                bDraw = false;
            }
        }

        public void PlayerExecution_ManaDisplay()
        {
            ManaDisplayEn = false;
        }

        public void OnWndProc(WndEventArgs args)
        {
            
            if (args.Msg != (ulong)Utils.WindowsMessages.WM_KEYDOWN)
            {
                return;
            }
            if(legacy)
            {
                if (Game.IsKeyDown(Key.G) && !Game.IsChatOpen)
                {
                    ManaDisplayEn = true;
                }
            }
            else
            {
                if (Game.IsKeyDown(Key.R) && !Game.IsChatOpen)
                {
                    ManaDisplayEn = true;
                }
            }
        }

        public void OnDraw()
        {

        }

        public void Drawing_OnPreReset(EventArgs args)
        {
            _Font.OnLostDevice();
        }

        public void Drawing_OnPostReset(EventArgs args)
        {
            _Font.OnResetDevice();
        }

        public void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;
            //if (!Utils.SleepCheck("ShowMana")) {
            if (!bDraw) return;
            //
            _Font.DrawText(null, (remainingMana < 0 ? 0 : remainingMana).ToString(), (int)Game.MouseScreenPosition.X + 25, (int)Game.MouseScreenPosition.Y - 20, Color.Red);
        }

        public void CurrentDomain_DomainUnload()
        {
            _Font.Dispose();
        }


    }
}
