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

        private SpriteFont scoreFont;
        private SpriteFont scoreboardFont;

        public ScoreHUD(PlayerManager playerManager)
        {
            myScore = new Score();

            otherScores = new Dictionary<String, Score>();
            this.playerManager = playerManager;
        }

        public void LoadContent(ContentManager Content)
        {
            // load the SpriteFonts to use in the HUD
            scoreFont = Content.Load<SpriteFont>("fonts/BoldConsoleFont14");
            scoreboardFont = Content.Load<SpriteFont>("fonts/ConsoleFont12");
        }

        public void Update()
        {
            if (!isActive)
                return;

            // first update the local player's score
            myScore = playerManager.LocalPlayer.Score;            

            ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Tab) && oldKs.IsKeyUp(Keys.Tab))
            {
                isOpen = !isOpen;
            }
            oldKs = ks;

            // only load and sort data if we must
            if (isOpen)
            {
                // step 1: get the data from playerManager
                otherScores.Clear(); // flush for new sorting process
                sortedToPrint.Clear();
                otherScores.Add(playerManager.LocalPlayer.Callsign, myScore);
                foreach (Player p in playerManager.RemotePlayers)
                {
                    otherScores.Add(p.Callsign, p.Score);
                }

                // step 2: sort the data by overall in descending order
                // TODO use container that sorts for us
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive)
                return;

            // draw the local player's overall score always
            spriteBatch.DrawString(scoreFont,
                                   playerManager.LocalPlayer.Callsign + ": " + myScore.Overall.ToString(),
                                   new Vector2(5, 5),
                                   Color.White,
                                   0.0f,
                                   Vector2.Zero,
                                   1.0f,
                                   SpriteEffects.None,
                                   0.0f);


            // if we're open, then we draw the whole scoreboard too
            if (isOpen)
            {
                String header = String.Format("{0,-6} ({1,-6} - {2,6}) {3}\n",
                                              "Score", "Wins", "Losses", "Player");

                spriteBatch.DrawString(scoreboardFont,
                                   header,
                                   new Vector2(5, 100),
                                   Color.White,
                                   0.0f,
                                   Vector2.Zero,
                                   1.0f,
                                   SpriteEffects.None,
                                   0.0f);

                int i = 0;
                foreach (KeyValuePair<string, Score> kv in sortedToPrint)
                {
                    // draw an individual scoreboard entry
                    String scoreboardEntry = String.Format("{0,-6} ({1,-6} - {2,6}) {3}",
                                                           kv.Value.Overall, kv.Value.Wins, kv.Value.Losses, kv.Key);

                    // TODO draw with the same color as the player's team
                    spriteBatch.DrawString(scoreboardFont,
                                           scoreboardEntry,
                                           new Vector2(5, 120 + (int)(i * scoreboardFont.MeasureString(scoreboardEntry).Y)),
                                           Color.Blue,
                                           0.0f,
                                           Vector2.Zero,
                                           1.0f,
                                           SpriteEffects.None,
                                           0.0f);

                    ++i;
                }
            }
        }
    }
}
