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

        #endregion

        public Dictionary<Byte, Shot> Shots = new Dictionary<Byte, Shot>();

        public Player(World world, PlayerInformation playerInfo)
            : base(world, GetTexture(world, playerInfo), Vector2.Zero, GetTankSize(world, playerInfo), 0)
        {
            this.playerInfo = playerInfo;

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
            // TODO get the correct texture depending on team
            return world.Content.Load<Texture2D>("textures/tank_rogue");
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
            List<Shot> shots = Shots.Values.ToList();
            foreach (Shot shot in shots)
            {
                shot.Update(gameTime);

                if (shot.State == ShotState.None)
                    Shots.Remove(shot.Slot);
            }

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
                            // set position and rotation...
                            Position = newPosition = oldPosition = packet.Position;
                            Rotation = newRotation = oldRotation = packet.Rotation;

                            // reset camera
                            World.Camera.Zoom = 1;
                            World.Camera.PanPosition = Vector2.Zero;

                            // move into alive state
                            state = PlayerState.Alive;
                        }

                        break;
                    }

                case MessageType.MsgDeath:
                    {
                        MsgDeathPacket packet = (MsgDeathPacket)message.MessageData;

                        // only interested in it if it's us that was killed
                        if (packet.Slot == this.Slot)
                        {
                            // move into exploding state
                            state = PlayerState.Exploding;
                        }

                        break;
                    }

                default:
                    break;
            }

            return;
        }

        protected virtual void Die(Player killer)
        {
            // send out the death packet right away
            NetOutgoingMessage deathMessage = World.ServerLink.CreateMessage();

            MsgDeathPacket deathPacket = new MsgDeathPacket(killer.Slot);

            deathMessage.Write((Byte)MessageType.MsgDeath);
            deathPacket.Write(deathMessage);

            World.ServerLink.SendMessage(deathMessage, NetDeliveryMethod.ReliableOrdered, 0);

            // move into the exploding state
            state = PlayerState.Exploding;
        }

        protected virtual Byte Shoot()
        {
            // find first available shot ID
            Byte shotSlot = Shot.AllocateSlot(Shots);

            // no more shot slots
            if (shotSlot == ProtocolInformation.DummyShot)
                return shotSlot;

            // get starting position
            Vector2 front = Bounds.LowerLeft - Bounds.UpperLeft;
            Vector2 initialPosition = Position - front;

            // create the shot
            Shots[shotSlot] = new Shot(World, World.Content.Load<Texture2D>("textures/bz/rogue_bolt"), initialPosition, Rotation, Velocity, shotSlot, this);

            return shotSlot;
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
