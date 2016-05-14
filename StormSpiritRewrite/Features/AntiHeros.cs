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
using SharpDX;
using StormSpiritRewrite.Abilities;

namespace StormSpiritRewrite.Features
{
    public class AntiHeros
    {
        private Zip zip
        {
            get
            {
                return Variables.Zip;
            }
        }

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private ItemUsage itemUsage;

        private int fleeDistance;

        private Item blink;

        public AntiHeros()
        {
            this.itemUsage = new ItemUsage();   
        }



        public void Blademail_AntiAntiMage()
        {
            Hero Antimage = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Antimage == null) return;
            Item BladeMail = me.FindItem("item_blade_mail");
            if (BladeMail == null) return;
            var ManaVoid = Antimage.Spellbook.Spell4;
            bool Cond = ManaVoid.IsInAbilityPhase && BladeMail.CanBeCasted() && Antimage.GetTurnTime(me) <= 0.2 && Antimage.Distance2D(me) <= ManaVoid.CastRange + 100;
            if (!Cond) return;
            if (Utils.SleepCheck("blademail"))
            {
                BladeMail.UseAbility();
                Utils.Sleep(100, "blademail");
            } 
        }

        public void Emuls_AntiAntiMage()
        {
            Hero Antimage = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Antimage == null) return;
            Item Euls = me.FindItem("item_cyclone");
            if (Euls == null) return;
            var ManaVoid = Antimage.Spellbook.Spell4;
            bool Cond = Euls.CanBeCasted() && Antimage.GetTurnTime(me) <= 0.2 && Antimage.Distance2D(me) <= ManaVoid.CastRange + 100;
            if (!Cond) return;
            if (Utils.SleepCheck("Euls"))
            {
                if (ManaVoid.IsInAbilityPhase)
                {
                    DelayAction.Add(100, () => {
                        Euls.UseAbility(me);
                    });
                    Utils.Sleep(100, "Euls");
                }
            }
        }

        public void Execute()
        {
            if (!zip.CanBeCast()) return;
            var AllEnemyHeroes = ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.Team == Variables.Hero.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(Variables.Hero.Position) <= 1000);
            if (AllEnemyHeroes == null) return;
            //Blademail_AntiAntiMage();
            //Emuls_AntiAntiMage();
            EvadeAntiMage();
            Blademail_AntiAntiMage();
            EvadeAxe();
            EvadeSlardar();
        }

        
        public void EvadeAntiMage()
        {
            Hero Antimage = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Antimage == null) return;
            Ability Blink = Antimage.Spellbook.Spell2;
            if (!zip.CanBeCast()) return;
            if (EnemyJumpToMe(Antimage, me, 200, 400, null, Blink) && Antimage.GetTurnTime(me) < 0.1)
            {
                if (Utils.SleepCheck("zip"))
                {
                    zip.SetZipToFountain(this.fleeDistance);
                    zip.Use();
                    Utils.Sleep(1000, "zip");
                }
            }
        }

        public void EvadeSlardar()
        {
            Hero Slardar = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Slardar && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Slardar == null) return;
            var Crush = Slardar.Spellbook.Spell2;
            if (Crush == null) return;
            if (!hasBlink(Slardar)) return;
            this.blink = Slardar.FindItem("item_blink");
            if (!zip.CanBeCast()) return;
            bool CrushCond = Crush.CanBeCasted();
            if (!CrushCond) return;
            if (!EnemyJumpToMe(Slardar, me, 350 + 50, 400, this.blink, null)) return;
            if (Utils.SleepCheck("zip"))
            {
                zip.SetZipToFountain(this.fleeDistance);
                zip.Use();
                Utils.Sleep(1000, "zip");
            }
        }       

        public void EvadeAxe()
        {
            Hero Axe = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Axe && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Axe == null) return;
            var Call = Axe.Spellbook.Spell1;
            if (Call == null) return;
            if (!hasBlink(Axe)) return;
            this.blink = Axe.FindItem("item_blink");
            if (!zip.CanBeCast()) return;
            bool CallCond = Call.CanBeCasted();
            // axe jumps to me, within alert range, has blink just used
            if (!EnemyJumpToMe(Axe, me, 300 + 50, 300, this.blink, null)) return;
            if (!CallCond) return;
            if (Utils.SleepCheck("zip"))
            {
                zip.SetZipToFountain(this.fleeDistance);
                zip.Use();
                Utils.Sleep(1000, "zip");
            }
        }

        public bool hasBlink(Hero hero)
        {
            return hero.HasItem(ClassID.CDOTA_Item_BlinkDagger);
        }

        public bool JustUsedAblity(Ability ability)
        {
            return !ability.CanBeCasted() && ability.Cooldown > ability.CooldownLength - 0.5;
        }

        public bool JustUsed(Item item)
        {
            if (item == null) return false;
            return item.Cooldown > 12 - 0.5 && !item.CanBeCasted();
        }

        private bool AnyEnemyNearBy(int range)
        {
            return ObjectManager.GetEntities<Hero>()
                    .Any(
                        x =>
                            x.Team == Variables.Hero.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(Variables.Hero.Position) <= range);
        }

        private bool EnemyJumpToMe(Hero Enemy, Hero me, uint alertRange, int fleeDist, Item blink = null, Ability ability = null)
        {
            if (Enemy == null) return false;
            this.fleeDistance = fleeDist;
            bool EnemyInOkState = Enemy.CanAttack() && !Enemy.IsStunned() && !Enemy.IsSilenced() && (double)(Enemy.Health) / (double)(Enemy.MaximumHealth) > 0.2;
            bool ImInOkState = !me.IsStunned() && !me.IsSilenced();
            //Console.WriteLine("Ememny state " + EnemyInOkState + " used blink " + JustUsed(blink) + " dist " + (Enemy.Distance2D(me) <= alertRange));
            
            if (ability == null)
            {
                if (blink == null)
                {
                    return false;
                }
                else
                {
                    return JustUsed(blink) && Enemy.Distance2D(me) <= alertRange && EnemyInOkState && ImInOkState;
                }
            }
            else
            {
                if (blink == null)
                {
                    return JustUsedAblity(ability) && (Enemy.Distance2D(me) <= alertRange) && EnemyInOkState && ImInOkState;
                }
                else
                {
                    return (JustUsedAblity(ability) || JustUsed(blink)) && Enemy.Distance2D(me) <= alertRange && EnemyInOkState & ImInOkState;
                }
            }
        }
    }
}
