using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Mechanics
{
    internal class Tristana : Champion
    {
        public Spell Q;
        public Spell W;
        public Spell E;
        public Spell R;

        public Tristana()
        {
            MecanicaUtil.PrintMessage("Tristana Loaded", MecanicaUtil.HexColor.Yellow);

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 600);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPosibleToInterrupt += Interrupter_OnPosibleToInterrupt;
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range))
                R.CastOnUnit(gapcloser.Sender);
        }

        public void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (R.IsReady() && unit.IsValidTarget(R.Range))
                R.CastOnUnit(unit);
        }

        public override void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if ((ComboActive || HarassActive) && unit.IsMe && (target is Obj_AI_Hero))
            {
                var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
                var useE = GetValue<bool>("UseE" + (ComboActive ? "C" : "H"));

                if (useQ && Q.IsReady())
                    Q.CastOnUnit(ObjectManager.Player);

                if (useE && E.IsReady())
                    E.CastOnUnit(target);
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = { W, E };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            Q.Range = 550 + 9 * (ObjectManager.Player.Level - 1);
            E.Range = 550 + 9 * (ObjectManager.Player.Level - 1);
            R.Range = 550 + 9 * (ObjectManager.Player.Level - 1);
            float aaRange = 550 + 9 * (ObjectManager.Player.Level - 1);

            if (Orbwalking.CanMove(100) && (ComboActive || HarassActive))
            {
                var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));
                if (useW)
                {
                    var wTarget = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
                    if (W.IsReady() && wTarget.IsValidTarget())
                        W.CastIfHitchanceEquals(wTarget, HitChance.Medium, false);
                }
                var useE = GetValue<bool>("UseE" + (ComboActive ? "C" : "H"));
                if (useE)
                {
                    var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

                    if (E.IsReady() && eTarget.IsValidTarget())
                        E.CastOnUnit(eTarget);
                    //var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
                    //if (E.IsReady() && eTarget.IsValidTarget())
                    //    E.CastOnUnit(eTarget);
                }

                if (GetValue<bool>("UseWM") && ComboActive)
                {
                    foreach (
                        var hero in 
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    hero => 
                                        hero.IsValidTarget(W.Range + aaRange) &&
                                        ObjectManager.Player.GetSpellDamage(hero, SpellSlot.W) > hero.Health))
                        W.Cast(hero.ServerPosition, true);
                }
            }

            if (!ComboActive || !GetValue<bool>("UseRM") || !R.IsReady()) return;
            foreach (
                var hero in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            hero =>
                                hero.IsValidTarget(R.Range) &&
                                ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R) - 20 > hero.Health))
               R.CastOnUnit(hero);
        }

        public override void ComboMenu(Menu menu)
        {
            menu.AddItem(new MenuItem("UseQC" + Id, "Use Q").SetValue(true));
            menu.AddItem(new MenuItem("UseWC" + Id, "Use W").SetValue(true));
            menu.AddItem(new MenuItem("UseEC" + Id, "Use E").SetValue(true));
        }

        public override void HarassMenu(Menu menu)
        {
            menu.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(false));
            menu.AddItem(new MenuItem("UseEH" + Id, "Use E").SetValue(true));
        }

        public override void DrawingMenu(Menu menu)
        {
            menu.AddItem(
                new MenuItem("DrawW" + Id, "W range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            menu.AddItem(
                new MenuItem("DrawE" + Id, "E range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));

        }

        public override void MiscMenu(Menu menu)
        {

            menu.AddItem(new MenuItem("UseWM" + Id, "W to gap close").SetValue(false));
            menu.AddItem(new MenuItem("UseRM" + Id, "Use R").SetValue(true));
            
        }
    }
}
