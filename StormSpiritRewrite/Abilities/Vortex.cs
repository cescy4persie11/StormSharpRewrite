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
    public class Vortex
    {
        private readonly Ability ability;

        private readonly DotaTexture abilityIcon;

        private readonly Sleeper sleeper;

        private readonly uint level;

        private Vector2 iconSize;

        public Vortex(Ability ability)
        {
            this.ability = ability;
            this.sleeper = new Sleeper();
            this.level = ability.Level;
            this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/storm_spirit_electric_vortex");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }

        public bool CanBePulled(Hero target)
        {
            return target != null && target.IsVisible
                   && target.Distance2D(Variables.Hero) <= this.ability.GetCastRange() + 100
                   && !target.IsLinkensProtected() && !target.IsInvul() && !target.IsMagicImmune();
        }

        public bool CanbeCasted()
        {
            return this.ability.CanBeCasted();
        }

        public float CoolDown()
        {
            return this.ability.Cooldown;
        }

        public bool inVortex()
        {
            return this.ability.Cooldown < 22 - this.ability.Level && this.ability.Cooldown > 20.5 - this.ability.Level;
        }

        public bool inCoolDown()
        {
            return this.ability.Cooldown != 0; 
        }

        public bool OutOfRange(Hero target)
        {
            return Variables.Hero.Distance2D(target) >= this.ability.CastRange + 100;
        }

        public void UseOn(Hero target)
        {
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (this.sleeper.Sleeping || !this.CanbeCasted())
            {
                return;
            }
            if (Utils.SleepCheck("pull"))
            {
                this.SwitchTread();
                if (CanBePulled(target))
                {
                    this.ability.UseAbility(target);
                }
                Utils.Sleep(100, "pull");
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
