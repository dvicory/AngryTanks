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

using Lidgren.Network;
using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Extensions.DictionaryExtensions;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public abstract class Player : DynamicSprite, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Player Properties

        protected PlayerInformation playerInfo;

        public PlayerInformation PlayerInfo
        {
            get { return playerInfo; }
        }

        public Byte Slot
        {
            get { return PlayerInfo.Slot; }
        }

        public String Callsign
        {
            get { return PlayerInfo.Callsign; }
        }

        public String Tag
        {
            get { return PlayerInfo.Tag; }
        }

        public TeamType Team
        {
            get { return PlayerInfo.Team; }
        }

        protected PlayerState state = PlayerState.None;

        public PlayerState State
        {
            get { return state; }
        }

        public Score Score
        {
            get;
            set;
        }

        protected Dictionary<Byte, Shot> shots = new Dictionary<Byte, Shot>();

        public Dictionary<Byte, Shot> Shots
        {
            get { return shots; }
        }

        public List<Shot> ActiveShots
        {
            get { return shots.Values.Where(s => s.State != ShotState.None).ToList();}
        }

        #endregion

        public Player(World world, PlayerInformation playerInfo)
            : base(world, GetTexture(world, playerInfo), Vector2.Zero, GetTankSize(world, playerInfo), 0)
        {
            this.playerInfo = playerInfo;
            this.Score = new Score(); //Gives scoreHUD a valid reference to start with

            World.ServerLink.MessageReceivedEvent += HandleReceivedMessage;
        }

        ~Player()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            World.ServerLink.MessageReceivedEvent -= HandleReceivedMessage;
        }

        protected static Texture2D GetTexture(World world, PlayerInformation playerInfo)
        {
            try
            {
                return world.Content.Load<Texture2D>(String.Format("textures/tank_{0}", ProtocolHelpers.TeamTypeToName(playerInfo.Team)));
            }
            catch (ContentLoadException e)
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                return world.Content.Load<Texture2D>("textures/tank_white");
            }
        }

        protected static Vector2 GetTankSize(World world, PlayerInformation playerInfo)
        {
            return new Vector2((Single)world.VarDB["tankWidth"].Value,
                               (Single)world.VarDB["tankLength"].Value);
        }

        public override void Update(GameTime gameTime)
        {
            // move straight into dead state if we're exploding since we have no logic for exploding yet
            if (State == PlayerState.Exploding)
                state = PlayerState.Dead;

            // update all our shots
            foreach (Shot shot in Shots.Values)
            {
                shot.Update(gameTime);
            }

            // remove all shots that are done
            Shots.RemoveAll(kvp => kvp.Value.State == ShotState.None);

            base.Update(gameTime);
        }

        protected virtual void HandleReceivedMessage(object sender, ServerLinkMessageEvent message)
        {
            switch (message.MessageType)
            {
                case MessageType.MsgSpawn:
                    {
                        MsgSpawnPacket packet = (MsgSpawnPacket)message.MessageData;

                        // only interested in it if it's us that is spawning
                        if (packet.Slot == this.Slot)
                        {
                            Spawn(packet.Position, packet.Rotation);
                        }

                        break;
                    }

                case MessageType.MsgDeath:
                    {
                        MsgDeathPacket packet = (MsgDeathPacket)message.MessageData;

                        // only interested in it if it's us that was killed
                        if (packet.Slot == this.Slot)
                        {
                            Die(World.PlayerManager.GetPlayerBySlot(packet.Killer));
                        }

                        break;
                    }

                case MessageType.MsgScore:
                    {
                        MsgScorePacket packet = (MsgScorePacket)message.MessageData;

                        // only interested in it if it's our new score
                        if (packet.Slot == this.Slot)
                        {
                            this.Score = packet.Score;
                        }

                        break;
                    }

                case MessageType.MsgShotEnd:
                    {
                        MsgShotEndPacket packet = (MsgShotEndPacket)message.MessageData;

                        // only interested if it's a shot by this player that's ending
                        if (packet.Slot == this.Slot)
                        {
                            try
                            {
                                Shots[packet.ShotSlot].End();
                            }
                            catch (KeyNotFoundException e)
                            {
                                Log.Warn(e.Message);
                                Log.Warn(e.StackTrace);
                            }
                        }

                        break;
                    }

                default:
                    break;
            }

            return;
        }

        public virtual void Spawn(Vector2 position, Single rotation)
        {
            // spawning invalidates any shots that you previously had fired
            // this is possible if the respawn time is quicker than shots expire
            foreach (Shot shot in ActiveShots)
            {
                shot.End();
            }

            // set position and rotation...
            Position = newPosition = oldPosition = position;
            Rotation = newRotation = oldRotation = rotation;

            // move into alive state
            state = PlayerState.Alive;
        }

        public virtual void Die(Player killer)
        {
            // move into the exploding state
            state = PlayerState.Exploding;
        }

        protected virtual void Shoot(Byte shotSlot, bool local)
        {
            // get starting position
            Single tankLength = (Single)World.VarDB["tankLength"].Value;
            Vector2 initialPosition = Position + new Vector2((tankLength / 2) * (Single)Math.Cos(Rotation - Math.PI / 2),
                                                             (tankLength / 2) * (Single)Math.Sin(Rotation - Math.PI / 2));

            Shoot(shotSlot, initialPosition, Rotation, Velocity, local);
        }

        protected virtual void Shoot(Byte shotSlot, Vector2 initialPosition, Single rotation, Vector2 initialVelocity, bool local)
        {
            // create the shot
            Shots[shotSlot] = new Shot(World, this, shotSlot, initialPosition, Rotation, Velocity);
        }

        /// <summary>
        /// Draws the callsign behind the <see cref="Player"/>.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public virtual void DrawCallsign(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteFont font = World.Content.Load<SpriteFont>("fonts/ConsoleFont14");
            Vector2 side = Bounds.UpperLeft - Bounds.LowerLeft;
            Vector2 position = Position - side;

            Vector2 pixelPosition = World.WorldUnitsToPixels(position);

            SpriteEffects flip = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;

            if (Rotation > (-Math.PI / 2) && Rotation < (Math.PI / 2))
                flip = SpriteEffects.None;

            spriteBatch.DrawString(font, Callsign, pixelPosition,
                                   Color.White, Rotation,
                                   font.MeasureString(Callsign) / 2,
                                   MathHelper.Clamp(1 / World.Camera.Zoom, 1, 8), flip, 1);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw all our shots
            foreach (Shot shot in Shots.Values)
            {
                shot.Draw(gameTime, spriteBatch);
            }

            // if we're dead, there's nothing to draw
            if (State == PlayerState.Dead)
                return;

            DrawCallsign(gameTime, spriteBatch);

            DrawStretched(gameTime, spriteBatch);
        }
    }
}
