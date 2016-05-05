﻿using StormSpiritRewrite.Abilities;
using StormSpiritRewrite.Utilities;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using StormSpiritRewrite.Features;

namespace StormSpiritRewrite
{
    public class StormSharp
    {
        private readonly Sleeper chaseSleeper;

        private readonly Sleeper initiateSleeper;

        private readonly Sleeper fleeSleeper;

        private readonly Sleeper dropItemSleeper;

        private ItemUsage itemUsage;

        private SelfZip selfZip;

        private ChaseZip chaseZip;

        private InitiateCombo initiateCombo;

        private ZipDodge zipDodge;

        private Flee flee;

        private DrawText drawText;

        private ManaAbuse manaAbuse;

        private ManaDisplay manaDisplay;

        private TargetFind targetFind;

        private Remnant remnant;

        private Vortex vortex;

        private Zip zip;

        private bool pause;

        public StormSharp()
        {
            this.chaseSleeper = new Sleeper();
            this.initiateSleeper = new Sleeper();
            this.fleeSleeper = new Sleeper();
            this.dropItemSleeper = new Sleeper();
            this.targetFind = new TargetFind();
        }

        private static Hero Me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private Hero Target
        {
            get
            {
                return this.targetFind.Target;
            }
        }

        public void OnClose()
        {
            this.pause = true;
            if (Variables.MenuManager != null)
            {
                Variables.MenuManager.Menu.RemoveFromMainMenu();
            }

            Variables.PowerTreadsSwitcher = null;
        }

        public void OnDraw()
        {
            if (Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            //selfZip.SelfZipDraw(Target);
            //chaseZip.ChaseZipDraw(Target);
            //initiateCombo.InitiateComboDraw(Target);
            

            drawText.DrawTextChaseZip(Variables.InChaseZip);
            drawText.DrawTextFlee(Variables.FleePress);
            drawText.DrawTextSelfZip(Variables.inSelfZip);
            drawText.DrawTextTpEnabled(Variables.TpEnabled);
            drawText.DrawTextInitiate(Variables.InInitiateZip);
            if (this.Target == null) return;
            if(Variables.InChaseZip)
            {
                chaseZip.DrawTarget(Target);
            }else if (Variables.inSelfZip)
            {
                selfZip.DrawTarget(Target);
            }else if (Variables.InInitiateZip)
            {
                initiateCombo.DrawTarget(Target);
            }
            
            if (Variables.InInitiateZip || Variables.InChaseZip || Variables.inSelfZip)
            {
                this.targetFind.DrawTarget();
            }
        }

        public void OnLoad()
        {
            Variables.Hero = ObjectManager.LocalHero;
            this.pause = Variables.Hero.ClassID != ClassID.CDOTA_Unit_Hero_StormSpirit;
            if (this.pause)
            {
                return;
            }
            Variables.Hero = ObjectManager.LocalHero;
            Variables.EnemyTeam = Me.GetEnemyTeam();
            Variables.MenuManager = new MenuManager(Me.Name);
            Variables.MenuManager.Menu.AddToMainMenu();
            Variables.Remnant = new Remnant(Me.Spellbook.Spell1);
            Variables.Vortex = new Vortex(Me.Spellbook.Spell2);
            Variables.Zip = new Zip(Me.Spellbook.Spell4);
            this.itemUsage = new ItemUsage();
            this.targetFind = new TargetFind();
            this.selfZip = new SelfZip();
            this.chaseZip = new ChaseZip();
            this.initiateCombo = new InitiateCombo();
            this.manaAbuse = new ManaAbuse();
            this.zipDodge = new ZipDodge();
            this.flee = new Flee();
            this.drawText = new DrawText();
            this.manaDisplay = new ManaDisplay();
            //this.constants = new Constants();


            Game.PrintMessage(
                "Storm Spirit" + " v " + Assembly.GetExecutingAssembly().GetName().Version + " loaded",
                MessageType.LogMessage);
        }

        public void Player_OnExecuteOrder(ExecuteOrderEventArgs args)
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }

            //if (!Variables.inSelfZip && !Variables.InInitiateZip && !Variables.InChaseZip)
            //{
            //selfZip.SelfZipPlayerExecution();
            //chaseZip.ChaseZipPlayerExecution();
            //initiateCombo.InitiateComboPlayerExecution();
            //}
            this.targetFind.UnlockTarget();
            manaAbuse.ManaAbusePlayerExecution(args);
            manaDisplay.PlayerExecution_ManaDisplay();

        }

        public void OnWndProc(WndEventArgs args)
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            manaDisplay.OnWndProc(args);
            
        }

        public void OnUpdate()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            this.targetFind.Find();
            manaDisplay.Execute();
        }

        public void OnUpdate_SelfZip()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var CanAction = !Me.IsChanneling();
            
            if (Variables.inSelfZip && CanAction)
            {         
                selfZip.Execute(Target);
                return;
            }
        }

        public void OnUpdate_ChaseZip()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var CanAction = !Me.IsChanneling();

            if (Variables.InChaseZip && CanAction)
            {
                chaseZip.Execute(Target);
                return;
            }
        }

        public void OnUpdate_InitiateCombo()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var CanAction = !Me.IsChanneling();

            if (Variables.InInitiateZip && CanAction)
            {
                initiateCombo.Execute(Target);
                return;
            }
        }

        public void OnUpdate_ManaAbuse()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var CanAction = !Me.IsChanneling();
            if (Variables.DropItemPressed && CanAction)
            {
                manaAbuse.Execute();
            }
        }

        public void OnUpdate_ZipDodge()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            var CanAction = !Me.IsChanneling();
            if (CanAction)
            {
                zipDodge.SelfZipDodgeExecute();
                zipDodge.SpecialZipDodgeExecute();
            }
        }

        public void OnUpdate_Flee()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            if (!Variables.FleePress) return;
            bool TpEnabled = Variables.MenuManager.TpEnabled;
            int dist = Variables.MenuManager.TpDistance;
            flee.Execute(dist, TpEnabled);
        }

        public void Drawing_OnPreReset(EventArgs args)
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            manaDisplay.Drawing_OnPreReset(args);
        }

        public void Drawing_OnPostReset(EventArgs args)
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            manaDisplay.Drawing_OnPostReset(args);
        }

        public void Drawing_OnEndScene(EventArgs args)
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            manaDisplay.Drawing_OnEndScene(args);
        }

        public void CurrentDomain_DomainUnload()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            manaDisplay.CurrentDomain_DomainUnload();
        }








    }
}
