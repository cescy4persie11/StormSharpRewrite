using Ensage;
using Ensage.Common;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormSpiritRewrite.Abilities
{
    public class Remnant
    {
        private readonly Ability ability;

        private readonly DotaTexture abilityIcon;

        private readonly Sleeper sleeper;

        private readonly uint level;

        private Vector2 iconSize;

        public Remnant(Ability ability)
        {
            this.ability = ability;
            this.sleeper = new Sleeper();
            this.level = ability.Level;
            this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/storm_spirit_static_remnant");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }

        public bool CanRemnant
        {
            get
            {
                return this.ability.CanBeCasted();
            }
        }

        public bool CanHitEnemyWithOutPull()
        {
            return ObjectManager.GetEntities<Hero>()
                        .Any(
                            x =>
                                x.Team == Variables.Hero.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                                && x.Distance2D(Variables.Hero.Position) <= 100 && !x.IsMagicImmune());
        }

        public void Use()
        {
            if (Utils.SleepCheck("remnant"))
            {
                this.ability.UseAbility();
                Utils.Sleep(100, "remnant");
            }
        }




    }
}
