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

namespace StormSpiritRewrite.Features
{
    public class Flee
    {
        private Zip zip;

        private Unit Fountain;

        private Hero me;

        public Flee()
        {
            
        }

        private void Update()
        {
            this.zip = Variables.Zip;
            me = Variables.Hero;
        }

        public void Execute(int dist, bool Cond)
        {
            Update();
            if (!zip.CanBeCast()) return;
            var Fountain = ObjectManager.GetEntities<Unit>().Where(_x => _x.ClassID == ClassID.CDOTA_Unit_Fountain && _x.Team == me.Team).FirstOrDefault();
            if (Fountain == null) return;
            if (Utils.SleepCheck("zip"))
            {
                zip.SetZipToFountain(dist);
                if (zip.CanBeCast())
                {
                    zip.Use();
                }

                if (!Cond) return;
                var Tp = me.Inventory.Items.Any<Item>(x => x.Name == "item_tpscroll");
                var BotLvl1 = me.Inventory.Items.Any<Item>(x => x.Name == "item_travel_boots");
                var BotLvl2 = me.Inventory.Items.Any<Item>(x => x.Name == "item_travel_boots_2");
                if (Tp)
                {
                    UseTP(Fountain);
                }
                else if (BotLvl1)
                {
                    UseBotLvl1(Fountain);
                }
                else if (BotLvl2)
                {
                    UseBotLvl2(Fountain);
                }
                Utils.Sleep(1000, "zip");
            }
        }
       
        public void UseTP(Unit f)
        {
            var TP = me.FindItem("item_tpscroll");
            if (TP.CanBeCasted() && zip.CanBeCast())
            {
                if (TP == null) return;
                if (Utils.SleepCheck("TP"))
                {
                    TP.UseAbility(f.Position);
                    Utils.Sleep(100, "TP");
                }
            }
        }

        public void UseBotLvl1(Unit f)
        {
            var BoT_lv1 = me.FindItem("item_travel_boots");
            if (BoT_lv1 == null) return;
            if (BoT_lv1.CanBeCasted() && zip.CanBeCast())
            {
                if (Utils.SleepCheck("TP"))
                {
                    BoT_lv1.UseAbility(f.Position);
                    Utils.Sleep(100, "TP");
                }
            }
        }

        public void UseBotLvl2(Unit f)
        {
            var BoT_lv2 = me.FindItem("item_travel_boots_2");
            if (BoT_lv2 == null) return;
            if (BoT_lv2.CanBeCasted() && zip.CanBeCast())
            {
                if (Utils.SleepCheck("TP"))
                {
                    BoT_lv2.UseAbility(f.Position);
                    Utils.Sleep(100, "TP");
                }
            }
        }
    }
}
