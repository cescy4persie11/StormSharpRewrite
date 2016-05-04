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
using SharpDX;
using System.Diagnostics;

namespace StormSpiritRewrite.Features
{
    public class ZipDodge
    {
        private Zip zip;

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }
        }

        private static List<string> SpellsList_StartDodgeWhenInAbilityPhase = new List<string>
                        {
                            "morphling_adaptive_strike", "windrunner_shackleshot", "broodmother_spawn_spiderlings",
                            "phantom_lancer_spirit_lance", "naga_siren_ensnare", "dazzle_poison_touch",
                            "viper_viper_strike", "tidehunter_gush", "bounty_hunter_shuriken_toss", "chaos_knight_chaos_bolt",
                            "vengefulspirit_magic_missile", "sven_storm_bolt", "skeleton_king_hellfire_blast",
                            "obsidian_destroyer_arcane_orb", "ogre_magi_fireblast",
                            "doom_bringer_doom", "beastmaster_primal_roar", "bane_fiends_grip"
                        };

        private static List<string> SpellsList_ZipDodgeForward = new List<string>
                        {
                             "doom_bringer_doom", "beastmaster_primal_roar", "bane_fiends_grip"
                        };


        public ZipDodge()
        {

        }

        private void Update()
        {
            this.zip = Variables.Zip;
        }

        public void SelfZipDodgeExecute()
        {
            Update();
            if (!zip.CanBeCast()) return;
            var AllEnemyHeroes = ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.Team == Variables.Hero.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(Variables.Hero.Position) <= 1000);
            if (AllEnemyHeroes == null) return;

            var AnyHeroCastingDodgeableSpell = AllEnemyHeroes.Any<Hero>(hero => hero.Spellbook.Spells.Any<Ability>(x => x.IsInAbilityPhase && SpellsList_StartDodgeWhenInAbilityPhase.Exists(spell => spell == x.Name)));
            if (!AnyHeroCastingDodgeableSpell) return;

            var CastingHero = AllEnemyHeroes.Where(hero => hero.Spellbook.Spells.Any<Ability>(x => x.IsInAbilityPhase && SpellsList_StartDodgeWhenInAbilityPhase.Exists(spell => spell == x.Name)));
            if (CastingHero == null) return;
            var CastedAbility = CastingHero.FirstOrDefault().Spellbook.Spells.Where<Ability>(x => x.IsInAbilityPhase && SpellsList_StartDodgeWhenInAbilityPhase.Exists(spell => spell == x.Name));
            if (CastedAbility == null) return;
            var EnemyFacingto = ObjectManager.GetEntities<Hero>()
                                    .Where(x => x.Distance2D(CastingHero.FirstOrDefault()) < CastedAbility.FirstOrDefault().CastRange + 150 && x.Team == me.Team) // Valid targets within range (over slightly to account for movement during abilityphase)
                                    .OrderBy(x => RadiansToFace(CastingHero.FirstOrDefault(), x))  // Orders targets based on radians required to be facing exactly
                                    .FirstOrDefault();
            if (EnemyFacingto == null) return;

            if(EnemyFacingto == me && zip.CanBeCast())
            {
                //special dodge position
                if (SpellsList_ZipDodgeForward.Exists(x => x == CastedAbility.FirstOrDefault().Name))
                {
                    if (Utils.SleepCheck("zip"))
                    {
                        zip.SetDodgeZipPosition(me, CastingHero.FirstOrDefault(), CastedAbility.FirstOrDefault());
                        zip.Use();
                        Utils.Sleep(100, "zip");
                    }
                }
                else {
                    if (Utils.SleepCheck("zip"))
                    {
                        zip.SetDynamicSelfZipPosition();
                        zip.Use();
                        Utils.Sleep(100, "zip");
                    }
                }
            }
        }

        public void SpecialZipDodgeExecute()
        {
            Centaur();
            DeathProphet();
            Riki();
            ShadowFiend();
            Lina();
            Lion();
        }

        public void Centaur()
        {
            Hero Centaur = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Centaur && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Centaur == null) return;
            Ability HoofStomp = Centaur.Spellbook.Spell1;
            bool cond = HoofStomp.IsInAbilityPhase && Centaur.Distance2D(me) <= 350 - me.HullRadius && zip.CanBeCast();
            if (!cond) return;
            if (Utils.SleepCheck("zip"))
            {
                zip.SetZipFacingDirection(me, 200);
                zip.Use();
                Utils.Sleep(100, "zip");
            }
        }

        public void DeathProphet()
        {
            Hero DP = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_DeathProphet && x.Distance2D(me) < 1000).FirstOrDefault();
            if (DP == null) return;
            var Silence = DP.Spellbook.SpellW;
            var CirclePoint = me.Position;
            CirclePoint.X = DP.Position.X + Convert.ToSingle(450 * Math.Cos(DP.RotationRad));
            CirclePoint.Y = DP.Position.Y + Convert.ToSingle(450 * Math.Sin(DP.RotationRad));
            bool cond = Silence.IsInAbilityPhase && me.Distance2D(CirclePoint) <= 500 && zip.CanBeCast();
            if (!cond) return;
            if (Utils.SleepCheck("zip"))
            {
                zip.SetZipFacingDirection(me, 150);
                zip.Use();
                Utils.Sleep(100, "zip");
            }
        }

        public void Riki()
        {
            Hero Riki = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Riki && x.Distance2D(me) < 1000).FirstOrDefault();
            if (Riki == null) return;
            var Smoke = Riki.Spellbook.Spell1;
            var SmokePosition = me.Position;
            var Smokelvl = Smoke.Level;
            SmokePosition.X = Riki.Position.X + Convert.ToSingle(200 * Math.Cos(Riki.Rotation));
            SmokePosition.Y = Riki.Position.Y + Convert.ToSingle(200 * Math.Sin(Riki.Rotation));
            var cond = IsFacing(Riki, me) && Riki.IsAttacking() && Smoke.Cooldown == 0 && me.Distance2D(Riki) <= 250 + 25 * Smokelvl + 50 && zip.CanBeCast();
            if (!cond) return;
            if (Utils.SleepCheck("zip"))
            {
                zip.SetZipFacingDirection(me, 350 + 25 * Smokelvl);
                zip.Use();
                Utils.Sleep(100, "zip");
            }
        }

        public void ShadowFiend()
        {
            Hero SF = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Nevermore && x.Distance2D(me) < 1000).FirstOrDefault();
            if (SF == null) return;
            Ability ZRaze = SF.Spellbook.Spell1;
            Ability XRaze = SF.Spellbook.Spell2;
            Ability CRaze = SF.Spellbook.Spell3;
            var ZRazeTarget = new Vector3(Convert.ToSingle(SF.Position.X + Math.Cos(SF.RotationRad) * (200 + SF.HullRadius)), Convert.ToSingle(SF.Position.Y + Math.Sin(SF.RotationRad) * (200 + SF.HullRadius)), Convert.ToSingle(SF.Position.Z));
            var XRazeTarget = new Vector3(Convert.ToSingle(SF.Position.X + Math.Cos(SF.RotationRad) * (450 + SF.HullRadius)), Convert.ToSingle(SF.Position.Y + Math.Sin(SF.RotationRad) * (450 + SF.HullRadius)), Convert.ToSingle(SF.Position.Z));
            var CRazeTarget = new Vector3(Convert.ToSingle(SF.Position.X + Math.Cos(SF.RotationRad) * (700 + SF.HullRadius)), Convert.ToSingle(SF.Position.Y + Math.Sin(SF.RotationRad) * (700 + SF.HullRadius)), Convert.ToSingle(SF.Position.Z));

            bool Zcond = ZRaze.IsInAbilityPhase && ZRazeTarget.Distance2D(me) < 270 && zip.CanBeCast();
            bool Xcond = XRaze.IsInAbilityPhase && XRazeTarget.Distance2D(me) < 270 && zip.CanBeCast();
            bool Ccond = CRaze.IsInAbilityPhase && CRazeTarget.Distance2D(me) < 270 && zip.CanBeCast();
            if (Zcond || Xcond || Ccond)
            {
                DelayAction.Add(250 - 2 * Game.Ping,
                    () => {
                        if (Utils.SleepCheck("zip"))
                        {
                            zip.SetZipFacingDirection(me, 150);
                            zip.Use();
                            Utils.Sleep(200, "zip");
                        }
                    }
                );
            }
            
            


        }

        public void Lina()
        {
            Hero Lina = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Lina && x.Distance2D(me) < 1500).FirstOrDefault();
            if (Lina == null) return;
            Ability Laguna = Lina.Spellbook.Spell4;
            var cond = Laguna.IsInAbilityPhase && IsFacing(Lina, me) && zip.CanBeCast();
            if (cond)
            {
                DelayAction.Add(250 - 2 * Game.Ping - (int)Math.Round((me.GetTurnTime(zip.zipLoc) * 1000)),
                    () => {
                        if (Utils.SleepCheck("zip"))
                        {
                            zip.SetDodgeZipPosition(me, Lina, Laguna);
                            zip.Use();
                            Utils.Sleep(200, "zip");
                        }
                    }
                );
            }

        }

        public void Lion()
        {
            Hero Lion = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Lion && x.Distance2D(me) <= 2000).FirstOrDefault();
            if (Lion == null) return;
            Ability Finger = Lion.Spellbook.Spell4;
            var cond = Finger.IsInAbilityPhase && IsFacing(Lion, me) && zip.CanBeCast();
            if (cond)
            {
                DelayAction.Add(90 - 2 * Game.Ping - (int)Math.Round((me.GetTurnTime(zip.zipLoc) * 1000)),
                    () => {
                        if (Utils.SleepCheck("zip"))
                        {
                            zip.SetDodgeZipPosition(me, Lion, Finger);
                            zip.Use();
                            Utils.Sleep(200, "zip");
                        }
                    }
                );
            }
        }

        private static bool IsFacing(Unit StartUnit, dynamic Target)
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("IsFacing => INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            float n1 = (float)Math.Sin(StartUnit.RotationRad - angle);
            float n2 = (float)Math.Cos(StartUnit.RotationRad - angle);

            return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.15;
        }

        private float RadiansToFace(Unit StartUnit, dynamic Target)
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("RadiansToFace -> INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            return (float)(Math.PI - Math.Abs(Math.Atan2(Math.Sin(StartUnit.RotationRad - angle), Math.Cos(StartUnit.RotationRad - angle))));
        }

        private bool AnyEnemyNearBy(int range)
        {
            return ObjectManager.GetEntities<Hero>()
                    .Any(
                        x =>
                            x.Team == Variables.Hero.GetEnemyTeam() && !x.IsIllusion && x.IsAlive && x.IsVisible
                            && x.Distance2D(Variables.Hero.Position) <= range);
        }
    }
}
