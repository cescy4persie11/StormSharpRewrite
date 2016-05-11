using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects.UtilityObjects;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormSpiritRewrite.Abilities
{
    public class Zip
    {
        private readonly Ability ability;

        private readonly DotaTexture abilityIcon;

        private readonly Sleeper sleeper;

        public readonly uint level;

        private Vector2 iconSize;

        private Vector3 zipPosition;

        public Vector3 zipLoc;



        public Zip(Ability ability)
        {
            this.ability = ability;
            this.sleeper = new Sleeper();
            this.level = ability.Level;
            this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/storm_spirit_static_remnant");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }

        public bool CanBeCast()
        {
            return this.ability.CanBeCasted() && !this.ability.IsInAbilityPhase
                    && !Variables.Hero.NetworkActivity.Equals(NetworkActivity.CastAbilityR)
                    && !Variables.Hero.Modifiers.Any(x => x.Name == "modifier_storm_spirit_ball_lightning");
        }

        public bool NoManaForZip()
        {
            return Variables.Hero.Mana <= 30 + 0.08 * Variables.Hero.MaximumMana;
        }

        public void SetStaticSelfZipPosition()
        {
            if (Variables.Hero.NetworkActivity.Equals(NetworkActivity.Idle))
            {
                this.zipPosition = Variables.Hero.Position;
            }
            else
            {
                SetZipFacingDirection(Variables.Hero, 50);
            }           
        }

        public void SetDodgeZipPosition(Hero me, Hero enemy, Ability spell)
        {
            var zipTarget = new Vector3(0, 0, me.Position.Z);
            this.zipLoc = me.Position;
            float distance = (float)SharpDX.Vector3.Distance(enemy.Position, me.Position);
            if (
                spell.Name == "lion_finger_of_death"
                    )
            {
                var X = me.Position.X + 200 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 200 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;

            }
            else if (spell.Name == "lina_laguna_blade")
            {
                zipTarget.X = me.Position.X - 200 * (enemy.Position.X - me.Position.X) / distance;
                zipTarget.Y = me.Position.Y - 200 * (enemy.Position.Y - me.Position.Y) / distance;
                this.zipLoc = zipTarget;
            }
            else if (spell.Name == "doom_bringer_doom")
            {
                //zipTarget.X = me.Position.X - 200 * (enemy.Position.X - me.Position.X) / distance;
                //zipTarget.Y = me.Position.Y - 200 * (enemy.Position.Y - me.Position.Y) / distance;
                zipTarget.X = me.Position.X + Convert.ToSingle(100 * Math.Cos(me.RotationRad));
                zipTarget.Y = me.Position.Y + Convert.ToSingle(100 * Math.Sin(me.RotationRad));
            }

            else if (spell.Name == "beastmaster_primal_roar")
            {
                var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
            }
            else if (spell.Name == "bane_fiends_grip")
            {var X = me.Position.X + 150 * Math.Cos(me.RotationRad);
                var Y = me.Position.Y + 150 * Math.Sin(me.RotationRad);
                zipTarget.X = Convert.ToSingle(X);
                zipTarget.Y = Convert.ToSingle(Y);
                zipTarget.Z = me.Position.Z;
                
            }
            else
            {
                zipTarget.X = me.Position.X;
                zipTarget.Y = me.Position.Y;
            }
            this.zipPosition = zipTarget;
        }

        public void SetDynamicSelfZipPosition()
        {
            SetZipFacingDirection(Variables.Hero, 50);
        } 

        public void SetLongZipPosition(Hero target)
        {

            this.zipPosition = Variables.Hero.Spellbook.SpellR.GetPrediction(target);
        }

        public void SetZipFacingDirection(Hero src, float D)
        {
            var zipTarget = new Vector3(0, 0, 0);
            double X;
            double Y;
            X = src.Position.X + D * Math.Cos(src.RotationRad);
            Y = src.Position.Y + D * Math.Sin(src.RotationRad);
            zipTarget.X = Convert.ToSingle(X);
            zipTarget.Y = Convert.ToSingle(Y);
            zipTarget.Z = src.Position.Z;
            this.zipPosition = zipTarget;
        }

        public void SetZipLocation(Vector3 position)
        {
            this.zipPosition = position;
        }

        public void Use()
        {
            if (!this.CanBeCast()) return;        
            if (Utils.SleepCheck("zip0"))
            {
                this.SwitchTread();
                this.ability.UseAbility(this.zipPosition);
                Utils.Sleep(100, "zip0");
            }
        }

        public void SwitchTread()
        {
            if (Variables.PowerTreadsSwitcher != null && Variables.PowerTreadsSwitcher.IsValid
                && Variables.Hero.Health > 300)
            {
                Variables.PowerTreadsSwitcher.SwitchTo(
                    Ensage.Attribute.Intelligence,
                    Variables.PowerTreadsSwitcher.PowerTreads.ActiveAttribute,
                    false);
            }
        }

        public void SetZipToFountain(int D)
        {
            var zipTarget = Variables.Hero.Position;
            var Fountain = ObjectManager.GetEntities<Unit>().Where(_x => _x.ClassID == ClassID.CDOTA_Unit_Fountain && _x.Team == Variables.Hero.Team).FirstOrDefault();
            if (Fountain == null)
            {
                this.zipPosition = Variables.Hero.Position;
            }
            else
            {
                zipTarget.X = Variables.Hero.Position.X + D / Variables.Hero.Distance2D(Fountain) * (Fountain.Position.X - Variables.Hero.Position.X);
                zipTarget.Y = Variables.Hero.Position.Y + D / Variables.Hero.Distance2D(Fountain) * (Fountain.Position.Y - Variables.Hero.Position.Y);
                zipTarget.Z = Variables.Hero.Position.Z;
                this.zipPosition = zipTarget;
            }
        }
    }
}
