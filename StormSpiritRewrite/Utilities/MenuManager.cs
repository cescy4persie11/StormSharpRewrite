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
        public readonly MenuItem ChaseZipMenu;

        public readonly MenuItem InitiateZipMenu;

        public readonly MenuItem SelfZipMenu;

        private readonly MenuItem LegacyOrQwerMenu;

        public readonly Menu FleeMenu;

        public readonly MenuItem FleeHotKey;

        private readonly MenuItem FleeDistance;

        public readonly MenuItem FleeTpEnabled;

        private readonly MenuItem DropManaItemMenu;

        private readonly MenuItem AutoAttackDodgeMenu;

        public Menu Menu { get; private set; }

        public MenuManager(string heroName)
        {
            this.Menu = new Menu("StormSharp", "StormSharp", true, "npc_dota_hero_storm_spirit", true);
            this.LegacyOrQwerMenu = new MenuItem("Hotkey Setting", "Do you use LegacyHotkey?").SetValue(true).SetTooltip("Enable/Disable : Legacy/QWER");
            this.SelfZipMenu = new MenuItem("SelfZip", "SelfZip").SetValue(new KeyBind('F', KeyBindType.Press));
            this.ChaseZipMenu = new MenuItem("ChaseZipMenu", "ChaseZipMenu").SetValue(new KeyBind('W', KeyBindType.Press));
            this.InitiateZipMenu = new MenuItem("InitiateZip", "InitiateZip").SetValue(new KeyBind('D', KeyBindType.Press));
            this.AutoAttackDodgeMenu = new MenuItem("AutoAttack Dodge", "AutoAttack Dodge").SetValue(true).SetTooltip("");
            this.DropManaItemMenu = new MenuItem("DropManaItem", "Press to Drop Mana Item").SetValue(new KeyBind('Z', KeyBindType.Press));
            this.FleeMenu = new Menu("Flee mode settings", "Flee mode settings");
            this.FleeDistance = new MenuItem("FleeDistance", "FleeDistance").SetValue(new Slider(450, 400, 1000)).SetTooltip("zip distance in flee mode");
            this.FleeHotKey = new MenuItem("FleeHotkey", "FleeHotkey").SetValue(new KeyBind('C', KeyBindType.Press)).SetTooltip("press to zip towards fountain");
            this.FleeTpEnabled = new MenuItem("FleeTpEnabled", "FleeTpEnabled").SetValue(new KeyBind('Y', KeyBindType.Toggle)).SetTooltip("Enable Tp while zip-flee");
            this.FleeMenu.AddItem(this.FleeHotKey);
            this.FleeMenu.AddItem(this.FleeTpEnabled);
            this.FleeMenu.AddItem(this.FleeDistance);
            this.Menu.AddSubMenu(FleeMenu);
            this.Menu.AddItem(this.LegacyOrQwerMenu);
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

        public bool Legacy
        {
            get
            {
                return this.LegacyOrQwerMenu.GetValue<bool>();
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

        public int TpDistance
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
