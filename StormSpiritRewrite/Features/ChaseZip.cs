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
    public class ChaseZip
    {
        private Zip zip;

        private SelfZip selfZip;

        private Remnant remnant;

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

        public ChaseZip()
        {
            this.itemUsage = new ItemUsage();
            this.targetFind = new TargetFind();
            this.selfZip = new SelfZip();
        }

        public void Update()
        {
            this.itemUsage.UpdateItems();
            this.zip = Variables.Zip;
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
            var enemyHitByMyOverload = this.Target.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload_debuff");
            //var myAttackInAir = ObjectManager.TrackingProjectiles.Any(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            //always try to cast remnant if it can hit anyone
            if (remnant.CanHitEnemyWithOutPull() && remnant.CanRemnant)
            {
                if (Utils.SleepCheck("remnant"))
                {
                    remnant.Use();
                    Orbwalking.Attack(this.Target, true);
                    Utils.Sleep(100, "remnant");
                }
            }

            //Distance Closing with Long Zip
            if (me.Distance2D(this.Target) > me.AttackRange + 100)
            {
                if (!myAttackAlmostLand() && zip.CanBeCast() && (!inPassive || (inPassive && myAttackAlmostLand()) || !me.IsAttacking()))
                {

                    if (Utils.SleepCheck("zip") && Prediction.StraightTime(this.Target) > 600)
                    {
                        zip.SetLongZipPosition(this.Target);
                        zip.Use();
                        Orbwalking.Attack(this.Target, true);
                        Utils.Sleep(100, "zip");
                    }
                }
            }
            else
            {
                //else is just selfZip
                if (zip.NoManaForZip())
                {
                    if (Orbwalking.AttackOnCooldown())
                    {
                        Orbwalking.Orbwalk(this.Target, 0, 0, false, true);
                    }
                    else
                    {
                        Orbwalking.Attack(this.Target, true);
                    }
                }
                else {
                    selfZip.Execute();
                }
            }

        }

        private bool AnyEnemyNearBy(int range)
        {
            return ObjectManager.GetEntities<Hero>()
                    .Any(
                        x =>
                            x.Team == Variables.Hero.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(Variables.Hero.Position) <= range);
        }

        private bool myAttackAlmostLand()
        {
            var myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            if (myProjectiles == null) return false;
            return myProjectiles.Any(x => this.Target.Distance2D(x.Position) < 300);
        }

        public void ChaseZipPlayerExecution()
        {
            this.targetFind.UnlockTarget();
        }

        public void ChaseZipDraw()
        {
            if (Variables.InChaseZip)
            {
                this.targetFind.DrawTarget();
            }
        }

        public bool AnyHeroCanBeHitByRemnant()
        {
            return
                    ObjectManager.GetEntities<Hero>()
                        .Any(
                            x =>
                                x.Team == me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                                && x.Distance2D(me.Position) <= 100 && !x.IsMagicImmune());
        }
    }
}
