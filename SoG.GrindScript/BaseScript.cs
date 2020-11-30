﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SoG.GrindScript
{
    public partial class BaseScript
    {
        // Collection of important data for mods and the mod loader
        internal static List<ModItem> CustomItems = new List<ModItem>();

        private readonly dynamic _game;

        protected ContentManager ModContent;

        public LocalGame LocalGame { get; }

        public Player LocalPlayer { get; }

        public SpriteBatch SpriteBatch { get; }

        protected BaseScript() 
        {
            Utils.Initialize(AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Secrets Of Grindea"));

            #region ModContent

            string assetPath = "ModContent/" + this.GetType().Name;

            ModContent = new ContentManager(Utils.GetTheGame().Content.ServiceProvider, assetPath);

            ModLibrary.ModContentManagers.Add(this.GetType().Name, ModContent);

            Console.WriteLine(this.GetType().Name + " ContentManager path set as " + ModContent.RootDirectory);

            #endregion

            _game = Utils.GetGameType("SoG.Program").GetMethod("GetTheGame")?.Invoke(null, null);

            LocalGame = new LocalGame(_game);

            SpriteBatch = (SpriteBatch)Utils.GetGameType("SoG.Game1").
                GetField("spriteBatch", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(_game);

            LocalPlayer = new Player(_game.xLocalPlayer);
        }

        protected SpriteFont GetFont(FontType font)
        {
            return (SpriteFont)Utils.GetGameType("SoG.FontManager").GetMethod("GetFont")?.Invoke(null, new object[] { (int)font });
        }

        public virtual void OnDraw()
        {
            return;
        }

        public virtual void OnPlayerDamaged(ref int damage, ref byte type)
        {
            return;
        }

        public virtual void OnPlayerKilled()
        {
            return;
        }

        public virtual void PostPlayerLevelUp(Player player)
        {
            return;
        }

        public virtual void OnEnemyDamaged(Enemy enemy, ref int damage, ref byte type)
        {
            return;
        }

        public virtual void OnNPCDamaged(NPC enemy, ref int damage, ref byte type)
        {
            return;
        }

        public virtual void OnNPCInteraction(NPC npc)
        {
            return;
        }

        public virtual void OnArcadiaLoad()
        {
            return;
        }

        public virtual void OnCustomContentLoad()
        {
            return;
        }
    }
}
