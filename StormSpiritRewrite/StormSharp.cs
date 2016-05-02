using StormSpiritRewrite.Abilities;
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

        private readonly ItemUsage itemUsage;

        private SelfZip selfZip;

        private ChaseZip chaseZip;

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
            this.itemUsage = new ItemUsage();
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
            Variables.PowerTreadsSwitcher = null;
        }

        public void OnDraw()
        {
            if (this.pause || Variables.Hero == null || !Variables.Hero.IsValid || !Variables.Hero.IsAlive)
            {
                return;
            }
            selfZip.SelfZipDraw();
            chaseZip.ChaseZipDraw();
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
            this.itemUsage.UpdateItems();
            this.targetFind = new TargetFind();
            this.selfZip = new SelfZip();
            this.chaseZip = new ChaseZip();
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

            selfZip.SelfZipPlayerExecution();
            chaseZip.ChaseZipPlayerExecution();

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
                selfZip.Execute();
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
                chaseZip.Execute();
                return;
            }
        }






    }
}
