using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.AbilityInfo;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using Ensage.Items;
namespace StormSpiritRewrite.Utilities
{
    public class ItemUsage
    {
        private List<Item> items;

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        public ItemUsage()
        {
            this.UpdateItems();
        }

        public void UpdateItems()
        {
            if (Variables.Hero == null || !Variables.Hero.IsValid)
            {
                return;
            }
            this.items = Variables.Hero.Inventory.Items.ToList();
            var powerTreads = this.items.FirstOrDefault(x => x.StoredName() == "item_power_treads");
            if (powerTreads != null)
            {
                Variables.PowerTreadsSwitcher = new PowerTreadsSwitcher(powerTreads as PowerTreads);
            }
        }

        public void UseSoulRing()
        {
            var soulRing = me.FindItem("item_soul_ring");
            if (soulRing == null) return;
            bool SoulRingCond = me.Health / me.MaximumHealth > 0.3 && soulRing.CanBeCasted();
            if (!SoulRingCond) return;
            if (Utils.SleepCheck("soulring"))
            {
                soulRing.UseAbility();
                Utils.Sleep(100, "soulring");
            }
        }

        public void UseBottle()
        {
            var bottle = me.FindItem("item_bottle") as Bottle;
            if (bottle == null) return;
            bool inBottle = me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration");
            bool BottleCond = bottle.CurrentCharges > 0 && !inBottle && bottle.CanBeCasted();
            if (!BottleCond) return;
            if(Utils.SleepCheck("bottle"))
            {
                bottle.UseAbility();
                Utils.Sleep(300, "bottle");
            }
        }

        public void UseStick()
        {
            var magicStick = me.FindItem("item_magic_stick");
            if (magicStick == null) return;
            bool StickCond = me.IsAttacking() && me.ClosestToMouseTarget(500) != null
                            && magicStick.CanBeCasted() && magicStick.CurrentCharges > 0
                            && me.Mana / me.MaximumMana < 0.2;
            if (!StickCond) return;
            if(Utils.SleepCheck("magic_stick"))
            {

                magicStick.UseAbility();
                Utils.Sleep(300, "magic_stick");
            }
        }

        public void UseWand()
        {
            var magicWand = me.FindItem("item_magic_wand");
            if (magicWand == null) return;
            bool WandCond = me.IsAttacking() && me.ClosestToMouseTarget(500) != null
                            && magicWand.CanBeCasted() && magicWand.CurrentCharges > 0
                            && me.Mana / me.MaximumMana < 0.2;
            if (!WandCond) return;
            if (Utils.SleepCheck("magicWand"))
            {
                magicWand.UseAbility();
                Utils.Sleep(300, "magicWand");
            }
        }

        public void UseVeil(Hero target)
        {
            Item Veil = me.FindItem("item_veil_of_discord");
            if (Veil == null) return;
            bool VeilCond = target.Distance2D(me) <= Veil.CastRange + 100 && Veil.CanBeCasted();
            if (!VeilCond) return;
            if (Utils.SleepCheck("Veil"))
            {
                Veil.UseAbility(target.Position);
                Utils.Sleep(100, "Veil");
            }
        }

        public void Orchid(Hero target)
        {
            Item Orchid = me.FindItem("item_orchid");
            if (Orchid == null) return;
            bool OrchidCond = !target.IsMagicImmune() && target.Distance2D(me) <= Orchid.CastRange + 100 && Orchid.CanBeCasted();
            if (!OrchidCond) return;
            if (Utils.SleepCheck("Orchid"))
            {
                Orchid.UseAbility(target);
                Utils.Sleep(100, "Orchid");
            }
        }

        public void Bloodthorn(Unit target)
        {
            Item Bloodthorn = me.FindItem("item_bloodthorn");
            if (Bloodthorn == null) return;
            bool BloodThornCond = !target.IsMagicImmune() && target.Distance2D(me) <= Bloodthorn.CastRange + 100 && Bloodthorn.CanBeCasted();
            if (!BloodThornCond) return;
            if (Utils.SleepCheck("Bloodthorn"))
            {
                Bloodthorn.UseAbility(target);
                Utils.Sleep(100, "Bloodthorn");
            }
        }

        public void SheepStick(Unit target)
        {
            Item Sheep = me.FindItem("item_sheepstick");
            var underPulled = target.Modifiers.Any(x => x.Name == "modifier_storm_spirit_electric_vortex_pull");
            if (!underPulled) return;
            var pulledModifier = target.FindModifier("modifier_storm_spirit_electric_vortex_pull");
            if (pulledModifier == null) return;
            bool SheepCond = Sheep.CanBeCasted() && me.Distance2D(target) <= Sheep.CastRange + 100;
            if (!SheepCond) return;
            if(pulledModifier.RemainingTime < 0.1)
            {
                if (Utils.SleepCheck("Sheep"))
                {
                    Sheep.UseAbility(target);
                    Utils.Sleep(100, "Sheep");
                }
            }

        }



        public void ManaEfficiency()
        {
            this.UpdateItems();
            UseBottle();
            UseSoulRing();
            UseStick();
            UseWand();
        }

        public void OffensiveItem(Hero target)
        {
            UseVeil(target);
            SheepStick(target);
            Orchid(target);
            Bloodthorn(target);

        }


    }
}
