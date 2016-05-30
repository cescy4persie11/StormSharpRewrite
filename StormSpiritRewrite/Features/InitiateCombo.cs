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
        private Zip zip
        {
            get
            {
                return Variables.Zip;
            }
        }

        private ChaseZip chaseZip;

        private Remnant remnant
        {
            get
            {
                return Variables.Remnant;
            }
        }

        private Vortex vortex
        {
            get
            {
                return Variables.Vortex;

            }
        }

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private bool HexInitiate;

        private ItemUsage itemUsage;

        private ParticleEffect meToTargetParticleEffect;
        
        public InitiateCombo()
        {
            
            //this.targetFind = new TargetFind();
            this.chaseZip = new ChaseZip();
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
            // Mana Effiency
            
            itemUsage.ManaEfficiency();
            itemUsage.OffensiveItem(target);
            if (inUltimate || (inPassive && (remnant.isInCoolDown || vortex.inCoolDown())))
            {
                itemUsage.OffensiveItem(target);
            }
            if (!vortex.isLearnt())
            {
                chaseZip.Execute(target);
                return;
            }
            //vortex in cooldown -> not initiate yet
            if (!vortex.inCoolDown())
            {
                //first jump
                if(vortex.OutOfRange(target))
                {
                    //first zip
                    if(zip.CanBeCast() && (!inPassive || (inPassive && myAttackAlmostLand(target)) || !me.IsAttacking()))
                    {
                        if (Utils.SleepCheck("longzip") && Prediction.StraightTime(target) > 600)
                        {
                            zip.SetLongZipPosition(target);
                            zip.Use();
                            Utils.Sleep(100, "longzip");
                        }
                    }
                }
                if (inUltimate)
                {
                    if (Utils.SleepCheck("attack"))
                    {
                        me.Attack(target);
                        Utils.Sleep(100, "attack");
                    }
                }
                //pull
                if (vortex.CanbeCasted())
                {
                    if (Utils.SleepCheck("pull"))
                    {
                        vortex.UseOn(target);                   
                        Utils.Sleep(100, "pull");
                    }
                    Orbwalking.Orbwalk(target, 0, 0, false, true);
                }
            }
            else //vortex in cooldown
            {
                // first remnant land
                if (vortex.inVortex())
                {
                    if (!inPassive && remnant.CanRemnant)
                    {
                        if (Utils.SleepCheck("remnant"))
                        {
                            remnant.Use();
                            Utils.Sleep(100, "remnant");
                        }
                    }

                    if (Utils.SleepCheck("Remant attack"))
                    {
                        me.Attack(target);
                        Utils.Sleep(800, "Remant attack");
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
            var textPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.67));
            var text = "Combo Target:";
            Drawing.DrawText(text, textPos, new Vector2(20), Color.Yellow, FontFlags.AntiAlias);
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 110, Convert.ToSingle(Drawing.Height * 0.7));
            var name = "materials/ensage_ui/heroes_horizontal/" + target.Name.Replace("npc_dota_hero_", "") + ".vmat";
            var size = new Vector2(50, 50);
            Drawing.DrawRect(startPos, size + new Vector2(13, -6),
                Drawing.GetTexture(name));
            Drawing.DrawRect(startPos, size + new Vector2(14, -5),
                                    new Color(0, 0, 0, 255), true);
        }

        public void DrawParticleEffect(Hero target)
        {
            if (target == null) return;
            if (meToTargetParticleEffect == null)
            {
                meToTargetParticleEffect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);     //target inditcator
                meToTargetParticleEffect.SetControlPoint(2, new Vector3(Variables.Hero.Position.X, Variables.Hero.Position.Y, Variables.Hero.Position.Z));             //start point XYZ
                meToTargetParticleEffect.SetControlPoint(6, new Vector3(1, 0, 0));                                                    // 1 means the particle is visible
                meToTargetParticleEffect.SetControlPoint(7, new Vector3(target.Position.X, target.Position.Y, target.Position.Z)); //end point XYZ
            }
            else //updating positions
            {
                meToTargetParticleEffect.SetControlPoint(2, new Vector3(Variables.Hero.Position.X, Variables.Hero.Position.Y, Variables.Hero.Position.Z));
                meToTargetParticleEffect.SetControlPoint(6, new Vector3(1, 0, 0));
                meToTargetParticleEffect.SetControlPoint(7, new Vector3(target.Position.X, target.Position.Y, target.Position.Z));
            }
        }

        public void PlayerExecution(Hero target)
        {
            if ((target == null || !target.IsAlive || !target.IsValid) && meToTargetParticleEffect != null)
            {
                meToTargetParticleEffect.Dispose();
                meToTargetParticleEffect = null;
            }
        }

        public void DisableParticleEffect()
        {
            if (meToTargetParticleEffect != null)
            {
                meToTargetParticleEffect.Dispose();
                meToTargetParticleEffect = null;
            }
        }


    }
}
