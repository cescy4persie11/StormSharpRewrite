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
using SharpDX;

namespace StormSpiritRewrite.Features
{
    public class InitiateCombo
    {
        private Zip zip;

        private ChaseZip chaseZip;

        private Remnant remnant;

        private Vortex vortex;

        private Hero me;

        private bool HexInitiate;

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
        public InitiateCombo()
        {
            
            //this.targetFind = new TargetFind();
            this.chaseZip = new ChaseZip();
        }

        public void Update()
        {
            this.itemUsage = new ItemUsage();
            this.zip = Variables.Zip;
            this.vortex = Variables.Vortex;
            this.remnant = Variables.Remnant;
            this.me = Variables.Hero;
            //this.targetFind.Find();
        }

        public void Execute(Hero target)
        {
            Update();
            //this.targetFind.Find();
            if (target == null) return;
            var inUltimate = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
            var inPassive = me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
            // Mana Effiency
            itemUsage.ManaEfficiency();
            itemUsage.OffensiveItem(target);
            //vortex in cooldown -> not initiate yet
            if (!vortex.inCoolDown())
            {
                //first jump
                if(vortex.OutOfRange(target))
                {
                    //first zip
                    if(zip.CanBeCast() && (!inPassive || (inPassive && myAttackAlmostLand(target)) || !me.IsAttacking()))
                    {
                        zip.SetLongZipPosition(target);
                        zip.Use();
                        if (Orbwalking.AttackOnCooldown())
                        {
                            Orbwalking.Orbwalk(target, 0, 0, false, true);
                        }
                        else
                        {
                            Orbwalking.Attack(target, true);
                        }
                        //Orbwalking.Attack(this.Target, true);
                        Utils.Sleep(100, "zip");
                    }
                }
                //pull
                if (vortex.CanbeCasted())
                {
                    if (Utils.SleepCheck("pull"))
                    {
                        vortex.UseOn(target);
                        if (Orbwalking.AttackOnCooldown())
                        {
                            Orbwalking.Orbwalk(target, 0, 0, false, true);
                        }
                        else
                        {
                            Orbwalking.Attack(target, true);
                        }
                        //Orbwalking.Attack(Target, true);
                        Utils.SleepCheck("pull");
                    }
                }
            }
            else //vortex in cooldown
            {
                if (Orbwalking.AttackOnCooldown())
                {
                    Orbwalking.Orbwalk(target, 0, 0, false, true);
                }
                else
                {
                    Orbwalking.Attack(target, true);
                }
                // first remnant land
                if (vortex.inVortex())
                {
                    
                    if (!inPassive && remnant.CanRemnant)
                    {
                        if (Utils.SleepCheck("remnant"))
                        {
                            remnant.Use();
                            Orbwalking.Attack(target, true);
                            Utils.Sleep(100, "remnant");
                        }
                    }
                }
                else
                {                  
                    chaseZip.Execute(target);
                }
            }
        }

        private bool myAttackAlmostLand(Hero target)
        {
            var myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            if (myProjectiles == null) return false;
            return myProjectiles.Any(x => target.Distance2D(x.Position) < 300);
        }

        public void InitiateComboPlayerExecution()
        {
            //this.targetFind.UnlockTarget();
        }

        public void InitiateComboDraw(Hero target)
        {
            //this.targetFind.Find();
            if (target == null) return;
            if (Variables.InInitiateZip)
            {
                //this.targetFind.DrawTarget();
            }
        }

        public void DrawTarget(Hero target)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 110, Convert.ToSingle(Drawing.Height * 0.7));
            var name = "materials/ensage_ui/heroes_horizontal/" + target.Name.Replace("npc_dota_hero_", "") + ".vmat";
            var size = new Vector2(50, 50);
            Drawing.DrawRect(startPos, size + new Vector2(13, -6),
                Drawing.GetTexture(name));
            Drawing.DrawRect(startPos, size + new Vector2(14, -5),
                                    new Color(0, 0, 0, 255), true);
        }

    }
}
