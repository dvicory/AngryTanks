using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    class ScoreHUD
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Score myScore;

        private PlayerManager playerManager;
        private List<Player> players;

        public bool isActive = false;
        public bool isOpen = false;

        private KeyboardState ks, oldKs;

        private SpriteFont scoreFont;
        private SpriteFont scoreboardFont;

        public ScoreHUD(PlayerManager playerManager)
        {
            this.playerManager = playerManager;

            myScore = new Score();
            players = new List<Player>(ProtocolInformation.DummySlot);
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
                // step 1: get the players from player manager
                players.Clear(); // first clear it out

                // add all remote players
                players.AddRange(playerManager.RemotePlayers.Cast<Player>());

                // add local player, if we can
                if (playerManager.LocalPlayer != null)
                    players.Add(playerManager.LocalPlayer);

                // step 2: sort players in descending order
                // first by wins, then by overall score
                players = players.OrderByDescending(p => p.Score.Wins).ThenByDescending(p => p.Score.Overall).ToList();
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
                String header = String.Format("{0,-6} ({1,-6} - {2,6}) {3}",
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
                foreach (Player p in players)
                {
                    // draw an individual scoreboard entry
                    String scoreboardEntry;

                    // this player has no tag
                    if (p.Tag.Length == 0)
                        scoreboardEntry = String.Format("{0,-6} ({1,-6} - {2,6}) {3}",
                                                        p.Score.Overall, p.Score.Wins, p.Score.Losses, p.Callsign);
                    else
                        scoreboardEntry = String.Format("{0,-6} ({1,-6} - {2,6}) {3} ({4})",
                                                        p.Score.Overall, p.Score.Wins, p.Score.Losses, p.Callsign, p.Tag);

                    // TODO draw with the same color as the player's team
                    spriteBatch.DrawString(scoreboardFont,
                                           scoreboardEntry,
                                           new Vector2(5, 120 + (int)(i * scoreboardFont.MeasureString(scoreboardEntry).Y)),
                                           ProtocolHelpers.TeamTypeToColor(p.Team),
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
