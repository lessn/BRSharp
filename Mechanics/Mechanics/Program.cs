
#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;

#endregion


namespace Mechanics
{
    internal class Program
    {
        public static Menu Menu;
        public static Champion Champion;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Menu = new Menu("Mechanics", "Mechanics", true);
            Champion = new Champion();
            var BaseType = Champion.GetType();

            var championName = ObjectManager.Player.ChampionName;

            switch (championName)
            {
                case "Caitlyn":
                    Champion = new Caitlyn();
                    break;
                case "Corki":
                    Champion = new Corki();
                    break;
                case "Draven":
                    Champion = new Draven();
                    break;
                case "Ezreal":
                    Champion = new Ezreal();
                    break;
                case "Graves":
                    Champion = new Graves();
                    break;
                case "Jinx":
                    Champion = new Jinx();
                    break;
                case "KogMaw":
                    Champion = new Kogmaw();
                    break;
                case "Lucian":
                    Champion = new Lucian();
                    break;
                case "Quinn":
                    Champion = new Quinn();
                    break;
                case "Sivir":
                    Champion = new Sivir();
                    break;
                case "Teemo":
                    Champion = new Teemo();
                    break;
                case "Tristana":
                    Champion = new Tristana();
                    break;
                case "Twitch":
                    Champion = new Twitch();
                    break;
                case "Vayne":
                    Champion = new Vayne();
                    break;
                case "Kennen":
                    Champion = new Kennen();
                    break;
                case "Riven":
                    Champion = new Riven();
                    break;
            }


            Champion.Id = ObjectManager.Player.BaseSkinName;
            Champion.Menu = Menu;

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            var orbwalking = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Champion.Orbwalker = new Orbwalking.Orbwalker(orbwalking);

            var items = Menu.AddSubMenu(new Menu("Items", "Items"));
            items.AddItem(new MenuItem("BOTRK", "BOTRK").SetValue(true));
            items.AddItem(new MenuItem("GHOSTBLADE", "Ghostblade").SetValue(true));
            items.AddItem(
                new MenuItem("UseItemsMode", "Use items on").SetValue(
                    new StringList(new[] { "No", "Mixed mode", "Combo mode", "Both" }, 2)));

            // If Champion is supported draw the extra menus
            if (BaseType != Champion.GetType())
            {
                var combo = Menu.AddSubMenu(new Menu("Combo", "Combo"));
                Champion.ComboMenu(combo);

                var harass = Menu.AddSubMenu(new Menu("Harass", "Harass"));
                Champion.HarassMenu(harass);

                var laneclear = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                Champion.LaneClearMenu(laneclear);

                var misc = Menu.AddSubMenu(new Menu("Misc", "Misc"));
                Champion.MiscMenu(misc);

                var drawing = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
                Champion.DrawingMenu(drawing);
            }

            Champion.MainMenu(Menu);

            Menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Champion.Drawing_OnDraw(args);
            return;

            var y = 1;

            foreach (
                var t in
                    ObjectManager.Player.Buffs.Select(
                        b => b.DisplayName + " - " + b.IsActive + " - " + (b.EndTime > Game.Time) + " - " + b.IsPositive)
                )
            {
                Drawing.DrawText(0, y, System.Drawing.Color.Wheat, t);
                y = y + 16;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Update the combo and harass values.
            Champion.ComboActive = Champion.Menu.Item("Orbwalk").GetValue<KeyBind>().Active;
            Champion.HarassActive = Champion.Menu.Item("Farm").GetValue<KeyBind>().Active;
            Champion.LaneClearActive = Champion.Menu.Item("LaneClear").GetValue<KeyBind>().Active;
            Champion.Game_OnGameUpdate(args);


            var useItemModes = Menu.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;
            
            if (
                !((Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                   (useItemModes == 2 || useItemModes == 3))
                  ||
                  (Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                   (useItemModes == 1 || useItemModes == 3))))
                return;

            var botrk = Menu.Item("BOTRK").GetValue<bool>();
            var ghostblade = Menu.Item("GHOSTBLADE").GetValue<bool>();
            var target = Champion.Orbwalker.GetTarget();

            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                    target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass || ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }
            }

            if (ghostblade && target != null && target.Type == ObjectManager.Player.Type &&
                Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3142);
        }

        private static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            Champion.Orbwalking_AfterAttack(unit, target);
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            Champion.Orbwalking_BeforeAttack(args);
        }

    }
}
