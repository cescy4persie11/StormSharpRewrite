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
using StormSpiritRewrite.Features.Orbwalk;
using SharpDX;
using StormSpiritRewrite.Abilities;

namespace StormSpiritRewrite.Features
{
    public class ZipAttack
    {
        private Zip zip
        {
            get
            {
                return Variables.Zip;
            }
        }

        private bool inPassive
        {
            get
            {
                return me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload");
            }
        }

        private bool inUltimate
        {
            get
            {
                return me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
            }
        }

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private Remnant remnant
        {
            get
            {
                return Variables.Remnant;
            }
        }

        private ItemUsage itemUsage;

        private ParticleEffect meToTargetParticleEffect;

        private void Update()
        {
            this.itemUsage = new ItemUsage();
        }
    
        public ZipAttack()
        {
        }

        public void Execute(Hero target)
        {
            Update();
            itemUsage.ManaEfficiency();
            if (target == null) return;
            if (target.Distance2D(me) > 1500) return;
            //if (Utils.SleepCheck("attack"))
            //{
            //    me.Attack(target);
            //    Utils.Sleep(300, "attack");
            //}
            Orbwalk.Orbwalking.Orbwalk(target, 0, 0, false, true);
            //Game.ExecuteCommand("+dota_camera_center_on_hero");
            if (remnant.CanHitEnemyWithOutPull() && remnant.CanRemnant)
            {
                if (Utils.SleepCheck("remnant"))
                {
                    remnant.Use();
                    Orbwalk.Orbwalking.Orbwalk(target, 0, 0, false, true);
                    Utils.Sleep(100, "remnant");
                }
            }

            
            //Orbwalking.Orbwalk(target, 0, 0, false, true);
            // if Out of Sight
            if (CanZipToMousePosition_TargetOutOfSight(target))
            {
                //Orbwalking.Orbwalk(target, 0, 0, false, true);
                if (Utils.SleepCheck("move"))
                {
                    me.Move(target.Position);
                    Utils.Sleep(800, "move");
                }
                if (Utils.SleepCheck("zip"))
                {
                    zip.SetZipLocation(Game.MousePosition);
                    zip.Use();
                    Utils.Sleep(800, "zip");
                }
                return;
            }
            //if in sight
            if (CanZipToMousePosition_TargetInSight(target))
            {
                if (Utils.SleepCheck("zip"))
                {
                    zip.SetZipLocation(Game.MousePosition);
                    zip.Use();                  
                    Utils.Sleep(300, "zip");
                }
                
            }
        }

        private bool MyAttackAlmostLand(Hero target)
        {
            if (target == null) return false;
            var myProjectiles = ObjectManager.TrackingProjectiles.Where(x => x.Source.Name == me.Name && x.Source.Team != me.GetEnemyTeam());
            if (myProjectiles == null) return false;
            if (!ObjectManager.GetEntities<Hero>().Any(x => x.Name == target.Name && !x.IsIllusion && x.IsAlive && x.Team != me.Team))
            {
                //target not in insight
                return false;
            }
            try {
                return myProjectiles.Any(x => target.Distance2D(x.Position) < 100);
            }
            catch
            {
                return false;
            }
        }

        private bool CanZipToMousePosition_TargetInSight(Hero target)
        {
            if (target == null) return false;
            if(!ObjectManager.GetEntities<Hero>().Any( x => x.Name == target.Name && !x.IsIllusion && x.IsAlive && x.Team != me.Team))
            {
                //target not in insight
                return false;
            }
            return zip.CanBeCast() && (!inPassive || inPassive && (MyAttackAlmostLand(target) || me.Distance2D(target) > me.AttackRange + 200));
        }
        private bool CanZipToMousePosition_TargetOutOfSight(Hero target)
        {
            if (target == null) return false;
            if (!ObjectManager.GetEntities<Hero>().Any(x => x.Name == target.Name && !x.IsIllusion && x.IsAlive && x.Team != me.Team))
            {
                //target not in insight
                // return true if zipCanBeCast();
                return zip.CanBeCast() && !inPassive;
            }
            else
            {
                return false;
            }
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

        public void PlayerExecutionDispose(Hero target, bool Cond)
        {
            if ((Cond || (target == null || !target.IsAlive || !target.IsValid)) && meToTargetParticleEffect != null)
            {
                meToTargetParticleEffect.Dispose();
                meToTargetParticleEffect = null;
            }
        }


    }
}
