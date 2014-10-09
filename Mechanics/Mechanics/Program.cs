﻿
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


 CClass.Id = ObjectManager.Player.BaseSkinName;
CClass.Config = Config;
var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
SimpleTs.AddToMenu(targetSelectorMenu);
Config.AddSubMenu(targetSelectorMenu);
var orbwalking = Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
CClass.Orbwalker = new Orbwalking.Orbwalker(orbwalking);
var items = Config.AddSubMenu(new Menu("Items", "Items"));
items.AddItem(new MenuItem("BOTRK", "BOTRK").SetValue(true));
items.AddItem(new MenuItem("GHOSTBLADE", "Ghostblade").SetValue(true));
items.AddItem(
new MenuItem("UseItemsMode", "Use items on").SetValue(
new StringList(new[] {"No", "Mixed mode", "Combo mode", "Both"}, 2)));
// If Champion is supported draw the extra menus
if (BaseType != CClass.GetType())
{
var combo = new Menu("Combo", "Combo");
if (CClass.ComboMenu(combo))
{
Config.AddSubMenu(combo);
}
var harass = new Menu("Harass", "Harass");
if (CClass.HarassMenu(harass))
{
Config.AddSubMenu(harass);
}
var laneclear = new Menu("LaneClear", "LaneClear");
if (CClass.LaneClearMenu(laneclear))
{
Config.AddSubMenu(laneclear);
}
var misc = new Menu("Misc", "Misc");
if (CClass.MiscMenu(misc))
{
Config.AddSubMenu(misc);
}
var drawing = new Menu("Drawings", "Drawings");
if (CClass.DrawingMenu(drawing))
{
Config.AddSubMenu(drawing);
}
}
CClass.MainMenu(Config);
Config.AddToMainMenu();
Drawing.OnDraw += Drawing_OnDraw;
Game.OnGameUpdate += Game_OnGameUpdate;
Orbwalking.AfterAttack += Orbwalking_AfterAttack;
Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
}
private static void Drawing_OnDraw(EventArgs args)
{
CClass.Drawing_OnDraw(args);
return;
var y = 10;
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
CClass.ComboActive = CClass.Config.Item("Orbwalk").GetValue<KeyBind>().Active;
CClass.HarassActive = CClass.Config.Item("Farm").GetValue<KeyBind>().Active;
CClass.LaneClearActive = CClass.Config.Item("LaneClear").GetValue<KeyBind>().Active;
CClass.Game_OnGameUpdate(args);
var useItemModes = Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;
//Items
if (
!((CClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
(useItemModes == 2 || useItemModes == 3))
||
(CClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
(useItemModes == 1 || useItemModes == 3))))
return;
var botrk = Config.Item("BOTRK").GetValue<bool>();
var ghostblade = Config.Item("GHOSTBLADE").GetValue<bool>();
var target = CClass.Orbwalker.GetTarget();
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
CClass.Orbwalking_AfterAttack(unit, target);
}
private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
{
CClass.Orbwalking_BeforeAttack(args);
}
}
}
