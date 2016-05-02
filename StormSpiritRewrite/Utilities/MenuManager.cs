using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage.Common.Menu;
using SharpDX;

namespace StormSpiritRewrite.Utilities
{
    public class MenuManager
    {
        private readonly MenuItem ChaseZipMenu;

        private readonly MenuItem InitiateZipMenu;

        private readonly MenuItem SelfZipMenu;

        private readonly Menu FleeMenu;

        private readonly MenuItem FleeHotKey;

        private readonly MenuItem FleeDistance;

        private readonly MenuItem FleeTpEnabled;

        private readonly MenuItem DropManaItemMenu;

        private readonly MenuItem AutoAttackDodgeMenu;

        public Menu Menu { get; private set; }

        public MenuManager(string heroName)
        {
            this.Menu = new Menu("LoneDruidSharp", "LoneDruidSharp", true, "npc_dota_hero_storm_spirit", true);
            this.SelfZipMenu = new MenuItem("SelfZip", "SelfZip").SetValue(new KeyBind('W', KeyBindType.Press));
            this.ChaseZipMenu = new MenuItem("ChaseZipMenu", "ChaseZipMenu").SetValue(new KeyBind('F', KeyBindType.Press));
            this.InitiateZipMenu = new MenuItem("InitiateZip", "InitiateZip").SetValue(new KeyBind('D', KeyBindType.Press));
            this.AutoAttackDodgeMenu = new MenuItem("AutoAttack Dodge", "AutoAttack Dodge").SetValue(true).SetTooltip("");
            this.DropManaItemMenu = new MenuItem("DropManaItem", "Press to Drop Mana Item").SetValue(new KeyBind('Z', KeyBindType.Press));
            this.FleeMenu = new Menu("Flee mode settings", "Flee mode settings");
            this.FleeDistance = new MenuItem("FleeDistance", "FleeDistance").SetValue(new Slider(500, 400, 1000)).SetTooltip("zip distance in flee mode");
            this.FleeHotKey = new MenuItem("FleeHotkey", "FleeHotkey").SetValue(new KeyBind('C', KeyBindType.Press)).SetTooltip("press to zip towards fountain");
            this.FleeTpEnabled = new MenuItem("FleeTpEnabled", "FleeTpEnabled").SetValue(new KeyBind('T', KeyBindType.Toggle)).SetTooltip("Enable Tp while zip-flee");
            this.FleeMenu.AddItem(this.FleeHotKey);
            this.FleeMenu.AddItem(this.FleeTpEnabled);
            this.FleeMenu.AddItem(this.FleeDistance);
            this.Menu.AddSubMenu(FleeMenu);
            this.Menu.AddItem(this.SelfZipMenu);
            this.Menu.AddItem(this.ChaseZipMenu);
            this.Menu.AddItem(this.InitiateZipMenu);
            this.Menu.AddItem(this.DropManaItemMenu);
        }

        public bool ChaseZipModeMon
        {
            get
            {
                return this.ChaseZipMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool InitiateZipModeMon
        {
            get
            {
                return this.InitiateZipMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool SelfZipModeOn
        {
            get
            {
                return this.SelfZipMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool DropManaItemOn
        {
            get
            {
                return this.DropManaItemMenu.GetValue<KeyBind>().Active;
            }
        }

        public bool FleeModeOn
        {
            get
            {
                return this.FleeHotKey.GetValue<KeyBind>().Active;
            }
        }

        public int getTpDistance
        {
            get
            {
                return this.FleeDistance.GetValue<Slider>().Value;
            }
        }

        public bool TpEnabled
        {
            get
            {
                return this.FleeTpEnabled.GetValue<KeyBind>().Active;
            }
        }
    }
}
