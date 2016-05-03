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

namespace StormSpiritRewrite.Utilities
{

    public class SelfZip
    {
        private Zip zip;

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

        public SelfZip()
        {
            this.itemUsage = new ItemUsage();
            this.targetFind = new TargetFind();
        }

        public void Update()
        {
            this.itemUsage.UpdateItems();
            this.zip = Variables.Zip;
            this.me = Variables.Hero;
            this.targetFind.Find();
        }

        public void Execute()
        {
            Update();
            if (!zip.CanBeCast()) return;
            this.targetFind.Find();      
            if (!this.AnyEnemyNearBy(1000))
            {        
                if (Utils.SleepCheck("selfzip")) {
                    zip.SetStaticSelfZipPosition();
                    zip.Use();
                    Utils.Sleep(1000, "selfzip");
                }
            }
            else
            {
                if (this.Target == null) return;
                var inUltimate = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
                var inPassive = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
                var enemyHitByMyOverload = this.Target.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload_debuff"); 
                // first self zip
                if(!inPassive && !enemyHitByMyOverload)
                {
                    if (Utils.SleepCheck("selfzip"))
                    {
                        zip.SetDynamicSelfZipPosition();
                        zip.Use();
                        Orbwalking.Attack(this.Target, true);
                        Utils.Sleep(100, "selfzip");
                        return;
                    }
                    
                }else 
                //subsequent zips                                       
                if (me.Distance2D(this.Target) < 300)
                {
                    //if < 250, rezip after attack lands   
                    // now support Hold selfZip Attack, Cast Time * speed = 270
                    if (enemyHitByMyOverload && !inPassive)
                    {
                        if (Utils.SleepCheck("selfzip"))
                        {
                            zip.SetDynamicSelfZipPosition();
                            zip.Use();
                            Orbwalking.Attack(this.Target, true);
                            Utils.Sleep(100, "selfzip");
                        }
                    }
                }
                else
                {
                    if (myAttackAlmostLand())
                    {
                        if (Utils.SleepCheck("selfzip"))
                        {
                            zip.SetDynamicSelfZipPosition();
                            zip.Use();
                            Orbwalking.Attack(this.Target, true);
                            Utils.Sleep(100, "selfzip");
                        }
                    }
                    else
                    {
                        //still provide the short distance case to do rezip
                        if (enemyHitByMyOverload && !inPassive)
                        {
                            if (Utils.SleepCheck("selfzip"))
                            {
                                zip.SetDynamicSelfZipPosition();
                                zip.Use();
                                Orbwalking.Attack(this.Target, true);
                                Utils.Sleep(100, "selfzip");
                            }
                        }
                    }
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

        public void SelfZipPlayerExecution()
        {
            this.targetFind.UnlockTarget();
        }

        public void SelfZipDraw()
        {
            if (Variables.inSelfZip)
            {
                this.targetFind.DrawTarget();
            }
        }
    }
}
