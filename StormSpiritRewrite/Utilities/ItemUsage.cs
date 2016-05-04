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

        public void SoulRing(bool condition)
        {

        }
    }
}
