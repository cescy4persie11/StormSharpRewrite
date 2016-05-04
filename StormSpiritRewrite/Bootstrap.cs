using Ensage;
using Ensage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormSpiritRewrite
{
    class Bootstrap
    {
        private readonly StormSharp stormsharp;
        public Bootstrap()
        {
            this.stormsharp = new StormSharp();
        }

        public void SubscribeEvents()
        {
            Events.OnLoad += this.Events_Onload;
            //Events.OnClose += this.Events_OnClose;
            Game.OnUpdate += this.Game_OnUpdate;
            Game.OnWndProc += this.Game_OnWndProc;
            Drawing.OnDraw += this.Drawing_OnDraw;
            Drawing.OnPreReset += this.Drawing_OnPreReset;
            Drawing.OnPostReset += this.Drawing_OnPostReset;
            Drawing.OnEndScene += this.Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += this.CurrentDomain_DomainUnload;
            Player.OnExecuteOrder += this.Player_OnExecuteOrder;
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            this.stormsharp.CurrentDomain_DomainUnload();
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            this.stormsharp.Drawing_OnPreReset(args);
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            this.stormsharp.Drawing_OnPostReset(args);
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            this.stormsharp.Drawing_OnEndScene(args);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            this.stormsharp.OnDraw();
        }

        private void Events_Onload(object sender, EventArgs e)
        {
            this.stormsharp.OnLoad();
        }

        private void Events_OnClose(object sender, EventArgs e)
        {
            this.stormsharp.OnClose();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            this.stormsharp.OnUpdate_SelfZip();
            this.stormsharp.OnUpdate_ChaseZip();
            this.stormsharp.OnUpdate_InitiateCombo();
            this.stormsharp.OnUpdate_ManaAbuse();
            this.stormsharp.OnUpdate_ZipDodge();
            this.stormsharp.OnUpdate_Flee();
            this.stormsharp.OnUpdate();
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            this.stormsharp.OnWndProc(args);
        }

        private void Player_OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (sender.Equals(ObjectManager.LocalPlayer))
            {
                this.stormsharp.Player_OnExecuteOrder(args);
                
            }
        }
    }
}
