using System;
using System.Collections.Generic;
using System.Linq;
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

namespace StormSpiritRewrite.Features
{
    public class InitiateCombo
    {
        private Zip zip;

        private ChaseZip chaseZip;

        private Remnant remnant;

        private Vortex vortex;

        private Hero me;

        private readonly ItemUsage itemUsage;

        private TargetFind targetFind;

        private Hero Target
        {
            get
            {
                return this.targetFind.Target;
            }
        }

        public InitiateCombo()
        {
            this.itemUsage = new ItemUsage();
            this.targetFind = new TargetFind();
            this.chaseZip = new ChaseZip();
        }

        public void Update()
        {
            this.itemUsage.UpdateItems();
            this.zip = Variables.Zip;
            this.vortex = Variables.Vortex;
            this.remnant = Variables.Remnant;
            this.me = Variables.Hero;
            this.targetFind.Find();
        }

        public void Execute()
        {
            Update();
            this.targetFind.Find();
            if (this.Target == null) return;
            var inUltimate = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
            var inPassive = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
            //vortex in cooldown -> not initiate yet
            if (!vortex.inCoolDown())
            {
                //first jump
                if(vortex.OutOfRange(Target))
                {
                    //first zip
                    if(zip.CanBeCast() && (!inPassive || (inPassive && myAttackAlmostLand()) || !me.IsAttacking()))
                    {
                        zip.SetLongZipPosition(this.Target);
                        zip.Use();
                        Orbwalking.Attack(this.Target, true);
                        Utils.Sleep(100, "zip");
                    }
                }
                //pull
                if (vortex.CanbeCasted())
                {
                    if (Utils.SleepCheck("pull"))
                    {
                        vortex.UseOn(Target);
                        Orbwalking.Attack(Target, true);
                        Utils.SleepCheck("pull");
                    }
                }
            }
            else //vortex in cooldown
            {
                // first remnant land
                if (vortex.inVortex())
                {
                    if(!inPassive && remnant.CanRemnant)
                    {
                        if (Utils.SleepCheck("remnant"))
                        {
                            remnant.Use();
                            Orbwalking.Attack(Target, true);
                            Utils.Sleep(100, "remnant");
                        }
                    }
                }
                else
                {
                    chaseZip.Execute();
                }
            }
        }

        private bool myAttackAlmostLand()
        {
            var myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            if (myProjectiles == null) return false;
            return myProjectiles.Any(x => this.Target.Distance2D(x.Position) < 300);
        }

        public void InitiateComboPlayerExecution()
        {
            this.targetFind.UnlockTarget();
        }

        public void InitiateComboDraw()
        {
            if (Variables.InInitiateZip)
            {
                this.targetFind.DrawTarget();
            }
        }
    }
}
