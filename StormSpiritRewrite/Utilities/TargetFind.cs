using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;

using SharpDX;

namespace StormSpiritRewrite.Utilities
{
    public class TargetFind
    {
        private readonly DotaTexture heroIcon;

        private readonly Sleeper sleeper;

        private Vector2 iconSize;

        private bool locked;

        private bool zipAttackLocked;

        public Hero zipAttackTarget { get; private set; }

        public TargetFind()
        {
            this.sleeper = new Sleeper();
            this.heroIcon = Drawing.GetTexture("materials/ensage_ui/miniheroes/storm_spirit");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }

        public Hero Target { get; private set; }

        public void DrawTarget()
        {
            if (this.Target == null || !this.Target.IsVisible || !this.Target.IsAlive)
            {
                return;
            }
            Vector2 screenPosition;
            if (
                !Drawing.WorldToScreen(
                    this.Target.Position + new Vector3(0, 0, this.Target.HealthBarOffset / 3),
                    out screenPosition))
            {
                return;
            }
            screenPosition += new Vector2(-this.iconSize.X, 0);
            Drawing.DrawRect(screenPosition, this.iconSize, this.heroIcon);

            if (this.locked)
            {
                Drawing.DrawText(
                    "LOCKED",
                    screenPosition + new Vector2(this.iconSize.X + 2, 1),
                    new Vector2((float)(this.iconSize.X * 0.85)),
                    new Color(255, 150, 110),
                    FontFlags.AntiAlias | FontFlags.Additive);
            }
        }

        public void DrawZipAttackTargetOnMyHealthBar()
        {
            if (this.zipAttackTarget == null || !this.zipAttackTarget.IsVisible || !this.zipAttackTarget.IsAlive)
            {
                return;
            }
            Vector2 screenPosition;
            if (
                !Drawing.WorldToScreen(
                    Variables.Hero.Position + new Vector3(0, 0, this.zipAttackTarget.HealthBarOffset / 3),
                    out screenPosition))
            {
                return;
            }
            var targetIcon = Drawing.GetTexture("materials/ensage_ui/miniheroes/" + zipAttackTarget.Name.Replace("npc_dota_hero_", ""));
            //Console.WriteLine("name " + zipAttackTarget.Name.Replace("npc_dota_hero", ""));
            screenPosition += new Vector2(-this.iconSize.X, 0);
            Drawing.DrawRect(screenPosition, this.iconSize, targetIcon);   
                
        }

        public void DrawZipAttackTarget()
        {
            if (this.zipAttackTarget == null || !this.zipAttackTarget.IsVisible || !this.zipAttackTarget.IsAlive)
            {
                return;
            }
            Vector2 screenPosition;
            if (
                !Drawing.WorldToScreen(
                    this.zipAttackTarget.Position + new Vector3(0, 0, this.zipAttackTarget.HealthBarOffset / 3),
                    out screenPosition))
            {
                return;
            }
            screenPosition += new Vector2(-this.iconSize.X, 0);
            Drawing.DrawRect(screenPosition, this.iconSize, this.heroIcon);

            if (this.locked)
            {
                Drawing.DrawText(
                    "LOCKED",
                    screenPosition + new Vector2(this.iconSize.X + 2, 1),
                    new Vector2((float)(this.iconSize.X * 0.85)),
                    new Color(255, 150, 110),
                    FontFlags.AntiAlias | FontFlags.Additive);
            }
        }

        public void zipAttackFind()
        {
            if (this.sleeper.Sleeping)
            {
                return;
            }
            if (this.zipAttackLocked && this.zipAttackTarget != null && this.zipAttackTarget.IsAlive)
            {
                return;
            }
            this.UnlockZipAttackTarget();
            this.zipAttackTarget =
                Heroes.GetByTeam(Variables.EnemyTeam)
                    .Where(
                        x =>
                        x.IsValid && x.IsAlive && !x.IsIllusion && x.IsVisible
                        && x.Distance2D(Game.MousePosition) < 1000)
                    .MinOrDefault(x => x.Distance2D(Game.MousePosition));
            LockZipAttackTarget();
            this.sleeper.Sleep(100);

        }

        public void Find()
        {
            if (this.sleeper.Sleeping)
            {
                return;
            }

            if (this.locked && this.Target != null && this.Target.IsAlive)
            {
                return;
            }
            this.UnlockTarget();
            this.Target =
                Heroes.GetByTeam(Variables.EnemyTeam)
                    .Where(
                        x =>
                        x.IsValid && x.IsAlive && !x.IsIllusion && x.IsVisible
                        && x.Distance2D(Game.MousePosition) < 1000)
                    .MinOrDefault(x => x.Distance2D(Game.MousePosition));
            LockTarget();
            this.sleeper.Sleep(100);
        }

        public void LockTarget()
        {
            this.locked = true;
        }

        public void UnlockTarget()
        {
            this.locked = false;
        }

        public void LockZipAttackTarget()
        {
            this.zipAttackLocked = true;
        }

        public void UnlockZipAttackTarget()
        {
            this.zipAttackLocked = false;
        }
    }
}
