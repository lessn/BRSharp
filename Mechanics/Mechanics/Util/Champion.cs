using System;
using LeagueSharp;
using LeagueSharp.Common;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SharpDX;

namespace Mechanics
{
    internal class Champion
    {
        public bool ComboActive;
        public Menu Menu;
        public bool HarassActive;
        public string Id = "";
        public bool LaneClearActive;
        public Orbwalking.Orbwalker Orbwalker;

        public T GetValue<T>(string item)
        {
            return Menu.Item(item + Id).GetValue<T>();
        }

        public virtual void ComboMenu(Menu menu)
        {
        }

        public bool Packets()
        {
            return Program.Menu.Item("usePackets").GetValue<bool>();
        }

        public virtual void HarassMenu(Menu menu)
        {
        }

        public virtual void LaneClearMenu(Menu menu)
        {
        }

        public virtual void MiscMenu(Menu menu)
        {
        }

        public virtual void DrawingMenu(Menu menu)
        {
        }

        public virtual void MainMenu(Menu menu)
        {
        }

        public virtual void Drawing_OnDraw(EventArgs args)
        {
        }

        public virtual void Game_OnGameUpdate(EventArgs args)
        {
        }

        public virtual void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
        }

        public virtual void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
        }

        public void Cast_BasicCircleSkillshot_Enemy(Spell spell, SimpleTs.DamageType damageType = SimpleTs.DamageType.Physical, float extrarange = 0)
        {
            if (!spell.IsReady())
                return;
            var target = SimpleTs.GetTarget(spell.Range + extrarange, damageType);
            if (target == null)
                return;
            if (target.IsValidTarget(spell.Range + extrarange) && spell.GetPrediction(target).Hitchance >= HitChance.High)
                spell.Cast(target, Packets());
        }

        public bool Cast_IfEnemys_inRange(Spell spell, int count = 1, float extrarange = 0)
        {
            if (!spell.IsReady())
                return false;
            if (Utility.CountEnemysInRange((int)spell.Range + (int)extrarange) < count)
                return false;
            spell.Cast();
            return true;
        }

        public void Cast_BasicCircleSkillshot_AOE_Farm(Spell spell, int extrawidth = 0)
        {
            if (!spell.IsReady())
                return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, spell.Range + ((spell.Width + extrawidth) / 2), MinionTypes.All, MinionTeam.NotAlly);
            if (minions.Count == 0)
                return;
            var castPostion = MinionManager.GetBestCircularFarmLocation(minions.Select(minion => minion.ServerPosition.To2D()).ToList(), spell.Width + extrawidth, spell.Range);
            spell.Cast(castPostion.Position, Packets());
        }

    }
}
