using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Nuclex.Game;
using AngryTanks.Common;

namespace AngryTanks.Client
{
    class ScoreHUD
    {
        private Score myScore;

        private Dictionary<String, Score> otherScores;
        private List<KeyValuePair<string, Score>> sortedToPrint = new List<KeyValuePair<string, Score>>();
        private PlayerManager playerManager;

        public bool isActive = false;
        public bool isOpen = false;

        private KeyboardState ks, oldKs;
        private SpriteFont HUDfont;


        public ScoreHUD(PlayerManager playerManager)
        {
            myScore = new Score();

            otherScores = new Dictionary<String, Score>();
            this.playerManager = playerManager;
        }

        //Load the SpriteFont to use in the HUD
        public void LoadContent(ContentManager Content)
        {
            HUDfont = Content.Load<SpriteFont>("fonts/ConsoleFont12");
        }

        //Update the Overalls if needed
        public void Update()
        {
            if (!isActive)
                return;

            ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Tab) && oldKs.IsKeyUp(Keys.Tab))
            {
                isOpen = !isOpen;
            }
            oldKs = ks;

            //Only load and sort data if we must
            if (isOpen)
            {
                //Step #1 get the data from playerManager
                myScore = playerManager.LocalPlayer.Score;
                otherScores.Add(playerManager.LocalPlayer.Callsign, myScore);
                foreach (Player p in playerManager.RemotePlayers)
                {
                    otherScores.Add(p.Callsign, p.Score);
                }

                //Step #2 Sort the data by Overall in descending order
                List<KeyValuePair<string, Score>> unsorted = new List<KeyValuePair<string, Score>>();
                foreach (KeyValuePair<string, Score> kv in otherScores)
                {
                    unsorted.Add(kv);
                }
                var result = unsorted.OrderByDescending(score => score.Value);
                foreach (var keyPair in result)
                {
                    sortedToPrint.Add(keyPair);
                }
            }
        }

        //Draw HUD components as needed
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive)
                return;

            //Draw the local player's Overall always
            spriteBatch.DrawString(HUDfont,
                                   playerManager.LocalPlayer.Callsign + ": " + myScore.Overall.ToString(),
                                   new Vector2(3, 3),
                                   Color.Red,
                                   0.0f,
                                   Vector2.Zero,
                                   2.0f,
                                   SpriteEffects.None,
                                   0.0f);

            //If the Overallboard is Open, then draw it as well
            if (isOpen)
            {
                spriteBatch.DrawString(HUDfont,
                                   "Wins      Losses    Overall     Player",
                                   new Vector2(3, 20),
                                   Color.Tan,
                                   0.0f,
                                   Vector2.Zero,
                                   1.0f,
                                   SpriteEffects.None,
                                   0.0f);

                string scoreBoard = "";
                foreach (KeyValuePair<string, Score> kv in sortedToPrint)
                {
                    scoreBoard = kv.Value.Wins.ToString().PadRight(10, ' ') +
                                 kv.Value.Losses.ToString().PadRight(10, ' ') +
                                 kv.Value.Overall.ToString().PadRight(10, ' ') +
                                 kv.Key + "\n";

                }
                spriteBatch.DrawString(HUDfont,
                                   scoreBoard,
                                   new Vector2(3, 20),
                                   Color.Blue,
                                   0.0f,
                                   Vector2.Zero,
                                   1.0f,
                                   SpriteEffects.None,
                                   0.0f);
            }
        }
    }
}
