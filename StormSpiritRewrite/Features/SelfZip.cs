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
using SharpDX;

namespace StormSpiritRewrite.Utilities
{

    public class SelfZip
    {
        private Zip zip;

        private Hero me;

        private ItemUsage itemUsage;

        /*
        private TargetFind targetFind;

        private Hero Target
        {
            get
            {
                return this.targetFind.Target;
            }
        }
        */
        public SelfZip()
        {
            this.itemUsage = new ItemUsage();
           // this.targetFind = new TargetFind();
        }

        public void Update()
        {
            this.itemUsage.UpdateItems();
            this.zip = Variables.Zip;
            this.me = Variables.Hero;
            //this.targetFind.Find();
        }

        public void Execute(Hero target)
        {
            Update();
            if (!zip.CanBeCast()) return;
            //this.targetFind.Find();
            if (target == null)
            {        
                if (Utils.SleepCheck("selfzip")) {
                    zip.SetStaticSelfZipPosition();
                    zip.Use();
                    Orbwalking.Orbwalk(target, 0, 0, false, true);
                    Utils.Sleep(1000, "selfzip");
                }
                return;
            }
            else
            {
                var inUltimate = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
                var inPassive = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
                var enemyHitByMyOverload = target.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload_debuff");
                // mana efficiency
                itemUsage.ManaEfficiency();                                     
                // first self zip
                if(!inPassive && !enemyHitByMyOverload)
                {
                    if (Utils.SleepCheck("selfzip"))
                    {
                        zip.SetDynamicSelfZipPosition();
                        zip.Use();
                        Orbwalking.Orbwalk(target, 0, 0, false, true);
                        Utils.Sleep(500, "selfzip");
                        return;
                    }
                    
                }else 
                //subsequent zips                                       
                if (me.Distance2D(target) < 300)
                {
                    // orbwalk
                    if (inPassive)
                    {
                        if (Utils.SleepCheck("orbwalk"))
                        {
                            //orbwalk
                            if (target == null) return;
                            Orbwalking.Orbwalk(target, 0, 0, false, true);
                            Utils.Sleep(100, "orbwalk");
                        }
                    }
                    //if < 300, rezip after attack lands   
                    // now support Hold selfZip Attack, Cast Time * speed = 270
                    if (enemyHitByMyOverload && !inPassive)
                    {
                        if (Utils.SleepCheck("selfzip"))
                        {
                            zip.SetDynamicSelfZipPosition();
                            zip.Use();
                            Orbwalking.Orbwalk(target, 0, 0, false, true);
                            Utils.Sleep(500, "selfzip");
                        }
                    }
                }
                else //> 300
                {
                    if (me.Distance2D(target) < me.AttackRange + 100)
                    {
                        if (inPassive)
                        {
                            if (Utils.SleepCheck("orbwalk"))
                            {
                                //orbwalk
                                if (target == null) return;
                                Orbwalking.Orbwalk(target, 0, 0, false, true);
                                Utils.Sleep(100, "orbwalk");
                            }
                        }
                        try {
                            if (myAttackAlmostLand(target))
                            {
                                if (Utils.SleepCheck("selfzip"))
                                {
                                    zip.SetDynamicSelfZipPosition();
                                    zip.Use();
                                    Orbwalking.Orbwalk(target, 0, 0, false, true);
                                    Utils.Sleep(500, "selfzip");
                                }
                            }
                            else
                            {
                                // attack not dispatched, > 300
                                // if in attack range

                                //still provide the short distance case to do rezip, but only within attack range
                                if (enemyHitByMyOverload && !inPassive)
                                {
                                    if (Utils.SleepCheck("selfzip"))
                                    {
                                        zip.SetDynamicSelfZipPosition();
                                        zip.Use();
                                        Orbwalking.Orbwalk(target, 0, 0, false, true);
                                        Utils.Sleep(500, "selfzip");
                                    }
                                }

                            }
                        }
                        catch
                        {

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

        private bool myAttackAlmostLand(Hero target)
        {
            var myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == me.Name);
            if (myProjectiles == null) return false;
            if (target == null) return false;
            return myProjectiles.Any(x => target.Distance2D(x.Position) < 300);
        }

        public void SelfZipPlayerExecution()
        {
            //this.targetFind.UnlockTarget();
        }

        public void SelfZipDraw(Hero target)
        {
            //this.targetFind.Find();
            if (target == null) return;
            if (Variables.inSelfZip)
            {
               // this.targetFind.DrawTarget();
            }
        }

        public void DrawTarget(Hero target)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.65));
            var name = "materials/ensage_ui/heroes_horizontal/" + target.Name.Replace("npc_dota_hero_", "") + ".vmat";
            var size = new Vector2(50, 50);
            Drawing.DrawRect(startPos, size + new Vector2(13, -6),
                Drawing.GetTexture(name));
            Drawing.DrawRect(startPos, size + new Vector2(14, -5),
                                    new Color(0, 0, 0, 255), true);

        }
    }
}
