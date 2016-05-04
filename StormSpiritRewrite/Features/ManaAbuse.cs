using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects.UtilityObjects;

namespace StormSpiritRewrite.Features
{
    public class ManaAbuse
    {
        private Hero me;

        private static List<string> ManaStackItem = new List<string>
                        {
                            "item_branches", "item_mantle", "item_circlet", "item_robe",
                            "item_staff_of_wizardry", "item_null_talisman", "item_orchid",
                            "item_point_booster", "item_arcane_boots", "item_cyclone",
                            "item_oblivion_staff", "item_energy_booster", "item_bloodstone",
                            "item_soul_booster", "item_veil_of_discord", "item_energy_booster",
                            "item_wraith_band", "item_ring_of_aquila"
                        };

        private bool AbuseDroppedItem;

        private Item soulRing;

        private Dictionary<ItemSlot, Item> ItemSlots = new Dictionary<ItemSlot, Item>();

        public bool isManaItem(Item item)
        {
            return ManaStackItem.Exists(x => x == item.Name);
        }

        public ManaAbuse()
        {
            this.ItemSlots = new Dictionary<ItemSlot, Item>();
        }

        public void Update()
        {
            this.me = Variables.Hero;
            if (hasItem("item_soul_ring"))
            {
                this.soulRing = me.FindItem("item_soul_ring");
            }
        }

        private bool AnyEnemyNearBy(int range)
        {
            return ObjectManager.GetEntities<Hero>()
                    .Any(
                        x =>
                            x.Team == me.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(me.Position) <= range);
        }

        private void SaveItemSlot(Item item)
        {
            var me = ObjectManager.LocalHero;
            for (var i = 0; i < 6; i++)
            {
                var currentSlot = (ItemSlot)i;
                var currentItem = me.Inventory.GetItem(currentSlot);
                if (currentItem == null || !currentItem.Equals(item) || ItemSlots.ContainsKey(currentSlot)) continue;
                ItemSlots.Add(currentSlot, item);
                break;
            }
        }

        private void PickUpItemsOnMove(ExecuteOrderEventArgs args)
        {
            args.Process = false;
            PickUpItems();
        }

        private void PickUpItems()
        {
            var droppedItems =
                ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(me) < 250).Reverse().ToList();

            var count = droppedItems.Count;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                    me.PickUpItem(droppedItems[i], i != 0);
                foreach (var itemSlot in ItemSlots)
                    itemSlot.Value.MoveItem(itemSlot.Key);
                ItemSlots.Clear();
            }
        }

        private void DropItems(Hero me)
        {
            var items = me.Inventory.Items;
            if (items.Where(x => !x.Equals("null") && isManaItem(x)) == null) return;
            foreach (var item in items.Where(x => !x.Equals("null") && isManaItem(x)))
            {
                SaveItemSlot(item);
                me.DropItem(item, me.NetworkPosition, true);
            }
        }

        private bool hasItem(String itemName)
        {
            return me.Inventory.Items.Any<Item>(x => x.Name == itemName);
        }

        private bool hasDroppedAllManaItem()
        {
            return !me.Inventory.Items.Any<Item>(x => ManaStackItem.Exists(y => y == x.Name));
        }

        public void Execute()
        {
            Update();
            if (!AnyEnemyNearBy(500))
            {
                AbuseDroppedItem = true;
                if (!me.NetworkActivity.Equals(NetworkActivity.Idle))
                {
                    me.Hold();
                }
                if (Utils.SleepCheck("drop"))
                {
                    DropItems(me);
                    Utils.Sleep(100, "drop");
                }
                if (soulRing.CanBeCasted() && hasDroppedAllManaItem())
                {
                    if (Utils.SleepCheck("soulring"))
                    {
                        soulRing.UseAbility();
                        Utils.Sleep(100, "soulring");
                    }
                }
            }
        }

        public void ManaAbusePlayerExecution(ExecuteOrderEventArgs args)
        {
            if (!AbuseDroppedItem) return;
            if (args.Order == Order.MoveLocation)
            {
                if (ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(me) < 250).Reverse().ToList().Count() != 0
                            && AbuseDroppedItem == true)
                {
                    PickUpItems();                    
                }
                if(ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(me) < 250).Reverse().ToList().Count() == 0)
                {
                    AbuseDroppedItem = false;
                }
            }
        }



    }
}
