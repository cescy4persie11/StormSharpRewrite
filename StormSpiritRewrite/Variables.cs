using Ensage;
using StormSpiritRewrite.Abilities;
using StormSpiritRewrite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormSpiritRewrite
{
    public static class Variables
    {

        public static bool InChaseZip
        {
            get
            {
                return MenuManager.ChaseZipModeMon;
            }
        }

        public static bool InInitiateZip
        {
            get
            {
                return MenuManager.InitiateZipModeMon;
            }
        }

        public static bool inSelfZip
        {
            get
            {
                return MenuManager.SelfZipModeOn;
            }
        }

        public static bool DropItemPressed
        {
            get
            {
                return MenuManager.DropManaItemOn;
            }
        }

        public static bool FleePress
        {
            get
            {
                return MenuManager.FleeModeOn;
            }
        }

        public static Team EnemyTeam { get; set; }

        public static Hero Hero { get; set; }

        public static MenuManager MenuManager { get; set; }

        public static Remnant Remnant { get; set; }

        public static Vortex Vortex { get; set; }

        public static Zip Zip { get; set; }

        public static PowerTreadsSwitcher PowerTreadsSwitcher { get; set; }

        public static float TickCount
        {
            get
            {
                return Environment.TickCount & int.MaxValue;
            }
        }
    }
}
