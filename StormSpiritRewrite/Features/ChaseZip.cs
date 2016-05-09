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
using SharpDX;

namespace StormSpiritRewrite.Features
{
    public class ChaseZip
    {
        private Zip zip
        {
            get
            {
                return Variables.Zip;
            }
        }

        private SelfZip selfZip;

        private Remnant remnant
        {
            get
            {
                return Variables.Remnant;
            }
        }

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private ItemUsage itemUsage;

        public ChaseZip()
        {  
            //this.targetFind = new TargetFind();
            this.selfZip = new SelfZip();
        }

        public void Update()
        {
            this.itemUsage = new ItemUsage();
            //this.targetFind.Find();
        }

        public void Execute(Hero target)
        {
            Update();
            //this.targetFind.Find();
            if (target == null) return;
            var inUltimate = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
            var inPassive = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
            var enemyHitByMyOverload = target.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload_debuff");
            //Mana efficiency
            
            itemUsage.ManaEfficiency();
            if (inUltimate || inPassive)
            {
                itemUsage.OffensiveItem(target);
            }
            //var myAttackInAir = ObjectManager.TrackingProjectiles.Any(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            //always try to cast remnant if it can hit anyone
            if (remnant.CanHitEnemyWithOutPull() && remnant.CanRemnant)
            {
                if (Utils.SleepCheck("remnant"))
                {
                    remnant.Use();
                    Orbwalking.Orbwalk(target, 0, 0, false, true);
                    Utils.Sleep(100, "remnant");
                }
            }

            //Distance Closing with Long Zip
            if (me.Distance2D(target) > me.AttackRange + 100)
            {
                if (Utils.SleepCheck("orbwalk"))
                {
                    if (target == null) return;
                    Orbwalking.Orbwalk(target, 0, 0, false, true);
                    Utils.Sleep(100, "orbwalk");
                }
                if (zip.CanBeCast() && (!inPassive || (inPassive && !me.IsAttacking() && !myAttackAlmostLand(target))))
                {

                    if (Utils.SleepCheck("zip") && Prediction.StraightTime(target) > 600)
                    {
                        zip.SetLongZipPosition(target);
                        zip.Use();
                        Orbwalking.Orbwalk(target, 0, 0, false, true);
                        Utils.Sleep(100, "zip");
                    }
                }
            }
            else
            {
                //else is just selfZip
                if (zip.NoManaForZip())
                {
                    if (Utils.SleepCheck("orbwalk"))
                    {
                        Orbwalking.Orbwalk(target, 0, 0, false, true);
                        Utils.Sleep(100, "orbwalk");
                    }
                }
                else {
                    selfZip.Execute(target);
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
            var myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            if (myProjectiles == null) return false;
            return myProjectiles.Any(x => target.Distance2D(x.Position) < 300);
        }

        public void ChaseZipPlayerExecution()
        {
            //this.targetFind.UnlockTarget();
        }

        public void ChaseZipDraw(Hero target)
        {
            //this.targetFind.Find();
            if (target == null) return;
            if (Variables.InChaseZip)
            {
                //this.targetFind.DrawTarget();
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
