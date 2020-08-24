﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoG.GrindScript;

namespace SoG.ChaosMod
{
    public class ChaosMod : BaseScript
    {
        private bool questTaken = false;
        private bool questFinished = false;
        

        public ChaosMod()
        {
            Console.WriteLine("Hello World from Chaosmod!");
        }

        public override void OnDraw()
        {

            if (!questTaken)
                return;


           /* var font = GetFont(FontType.Verdana8);


            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            SpriteBatch.DrawString(font, "Current Floor: " + LocalGame.GetCurrentFloor() + "/" + "5", new Vector2(400, 5), Color.Black);
            SpriteBatch.End();
            */
        }

        public override void OnPlayerDamaged(ref int damage, ref byte type)
        {
            damage = (3 * LocalGame.GetCurrentFloor()) * damage; //e.g 300%, 600%, 900%... dmg
        }

        public override void OnPlayerKilled()
        {
            if(LocalGame.GetCurrentFloor() < 5)
                Dialogue.AddDialogueLineTo(LocalGame,"I am not going to lie, but it's not looking good...");
            if (LocalGame.GetCurrentFloor() >= 5)
            {
                Dialogue.AddDialogueLineTo(LocalGame,
                    "Looking but I am sorry to tell you, it's floor 10 now....just joking" + Environment.NewLine +
                    "Grab your reward!");

                questFinished = true;
            }
        }

        public override void OnEnemyDamaged(Enemy enemy, ref int damage, ref byte type)
        {
            var currentFloor = (double) LocalGame.GetCurrentFloor();
            var factor = 1 - 0.15 * currentFloor;

            damage = (int)Math.Floor(damage * factor);
        }

        public override void OnNPCDamaged(NPC npc, ref int damage, ref byte type)
        {
            Console.WriteLine("NPC damaged...");
            Console.WriteLine(damage + "::" + type.ToString());
        }

        public override void OnArcadiaLoad()
        {
            Console.WriteLine("Arcadia loaded....!");
            NPC npc = NPC.AddNPCTo(LocalGame, NPCTypes.Teddy, new Vector2(1000, 250));

            npc.LookAtPlayerOnInteraction = true;
        }

        public override void OnNPCInteraction(NPC npc)
        {
            if (npc.GetNPCType() == NPCTypes.Teddy)
            {
                if (!questTaken)
                {
                    Dialogue.AddDialogueLineTo(LocalGame, "Nice to meet you " + LocalPlayer.Name +
                                                          ", I hope you are doing well..."
                                                          + Environment.NewLine +
                                                          "Unfortunately you seem a bit too good for this game, therefore... here is your nerf..."
                                                          + Environment.NewLine +
                                                          "Reach Floor 5 and you will get a special reward!");
                    questTaken = true;

                    
                }
                else if(questFinished)
                {
                    Dialogue.AddDialogueLineTo(LocalGame,"You managed to reach floor 5! Here is your reward!" + Environment.NewLine + "*Proceeds to give you one gold coin*");
                    LocalPlayer.Inventory.AddMoney(1);
                }
                else if (questTaken)
                {
                    Dialogue.AddDialogueLineTo(LocalGame, "'If the quest is too hard to take..." + Environment.NewLine + "You are just too weak...'" + Environment.NewLine + "~5Head");
                }
            }
        }
    }


}
