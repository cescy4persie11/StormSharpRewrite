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

        private readonly uint level;

        private Vector2 iconSize;

        private Vector3 zipPosition;

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

        public void testAbility()
        {
            Console.WriteLine("zip is ability behavior " + this.ability.AbilityBehavior);
            Console.WriteLine("I am in " + Variables.Hero.NetworkActivity);
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

        public void Use()
        {
            if (!this.CanBeCast()) return;        
            if (Utils.SleepCheck("zip"))
            {
                this.SwitchTread();
                this.ability.UseAbility(this.zipPosition);
                Utils.Sleep(100, "zip");
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
    }
}
