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
using Nuclex.Input;
using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public class LocalPlayer : Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TimeSpan lastMsgUpdate = new TimeSpan();
        private TimeSpan msgUpdateFrequency;

        private KeyboardState kb;
        private IInputService inputService;

        private Single velocityFactor, angularVelocityFactor;
        private Single maxVelocity, maxAngularVelocity;

        public LocalPlayer(World world, PlayerInformation playerInfo)
            : base(world, playerInfo)
        {
            // set our update frequency
            this.msgUpdateFrequency = new TimeSpan(0, 0, 0, 0, (int)(1000 / (UInt16)World.VarDB["updatesPerSecond"].Value));

            // TODO support if these variables change
            this.maxVelocity = (Single)World.VarDB["tankSpeed"].Value;
            this.maxAngularVelocity = (Single)World.VarDB["tankAngVel"].Value;

            inputService = (IInputService)World.IService.GetService(typeof(IInputService));
            inputService.GetKeyboard().KeyPressed += KeyPressed;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // unregister event handlers
                try
                {
                    inputService.GetKeyboard().KeyPressed -= KeyPressed;
                }
                // our input service disposed before we did... a dirty hack
                catch (NullReferenceException e)
                {
                    Log.Error(e.Message);
                    Log.Error(e.StackTrace);
                }
            }

            base.Dispose(disposing);
        }

        private void KeyPressed(Keys key)
        {
            // jump out early if console prompt is active
            if (World.Console.PromptActive)
                return;

            switch (key)
            {
                // kill ourselves if we hit delete
                case Keys.Delete:
                    if (State == PlayerState.Alive)
                        Die(this);

                    break;

                // shoot when you hit enter
                case Keys.Enter:
                    {
                        // first limit how many shots we can fire
                        // how many active shots are there?
                        Byte numActiveShots = (Byte)ActiveShots.Count;

                        // is there shot slots left for the player to fire?
                        Byte maxShots = (Byte)World.VarDB["shotSlots"].Value;

                        if (numActiveShots >= maxShots)
                            return;

                        // find first available shot ID
                        Byte shotSlot = Shot.AllocateSlot(Shots);

                        // no more shot slots
                        if (shotSlot == ProtocolInformation.DummyShot)
                            return;

                        // fire away
                        Shoot(Shot.AllocateSlot(Shots), true);

                        break;
                    }

                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();

            if (State == PlayerState.Alive)
            {
                UpdatePosition(gameTime);
                CheckShots(gameTime);
            }

            base.Update(gameTime);
        }

        private void UpdatePosition(GameTime gameTime)
        {
            /*  Basing my calculations on this:
             *  Velocity.X = VelocityFactor * MaxVelocity.X * cos(Rotation)
             *  Velocity.Y = VelocityFactor * MaxVelocity.X * sin(Rotation)
             *  
             *  OldVelocity = Velocity;
             *  Position += (OldVelocity + Velocity) * 0.5 * dt;
             *  
             *  Fixed thanks to Daniel G.
             */

            if (kb.IsKeyDown(Keys.W))
                velocityFactor = 1;
            if (kb.IsKeyDown(Keys.S))
                velocityFactor = -1;
            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.S))
                velocityFactor = 0;
            if (World.Console.PromptActive)
                velocityFactor = 0;

            oldPosition = Position;
            oldVelocity = Velocity;
            oldAngularVelocity = AngularVelocity;
            oldRotation = Rotation;

            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.S))
            {
                newVelocity = new Vector2((Single)Math.Cos(Rotation - Math.PI / 2),
                                          (Single)Math.Sin(Rotation - Math.PI / 2));

                newVelocity *= velocityFactor *= maxVelocity;
            }
            else
            {
                newVelocity = Vector2.Zero;
            }

            if (kb.IsKeyDown(Keys.A))
                angularVelocityFactor = -1;
            if (kb.IsKeyDown(Keys.D))
                angularVelocityFactor = 1;
            if (kb.IsKeyDown(Keys.A) && kb.IsKeyDown(Keys.D))
                angularVelocityFactor = 0;
            if (World.Console.PromptActive)
                angularVelocityFactor = 0;

            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D))
                newAngularVelocity = angularVelocityFactor * maxAngularVelocity;
            else
                newAngularVelocity = 0;

            // update based on newly found out velocities/positions
            Velocity = (oldVelocity + newVelocity) * 0.5f;
            newPosition += Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            AngularVelocity = MathHelper.WrapAngle((Single)(newAngularVelocity + oldAngularVelocity) * 0.5f);
            newRotation += AngularVelocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            // check for any collisions at our new location
            Single overlap;
            Vector2 collisionProjection;

            if (FindNearestCollision(World.MapGrid.PotentialIntersects(this), out overlap, out collisionProjection))
            {
                // move our position back to old position
                newPosition += overlap * collisionProjection;
                oldPosition = newPosition;
            }

            // finally confirm our position
            Position = newPosition;
            Rotation = newRotation;

            // now see if we should send out a MsgPlayerClientUpdate
            if ((lastMsgUpdate + msgUpdateFrequency) < gameTime.TotalGameTime)
            {
                lastMsgUpdate = gameTime.TotalGameTime;

                NetOutgoingMessage playerClientUpdateMessage = World.ServerLink.CreateMessage();

                MsgPlayerClientUpdatePacket playerClientUpdatePacket = new MsgPlayerClientUpdatePacket(Position, Rotation);

                playerClientUpdateMessage.Write((Byte)playerClientUpdatePacket.MsgType);
                playerClientUpdatePacket.Write(playerClientUpdateMessage);

                World.ServerLink.SendMessage(playerClientUpdateMessage, NetDeliveryMethod.UnreliableSequenced, 0);
            }
        }

        private void CheckShots(GameTime gameTime)
        {
            IWorldObject collidingObject;

            List<Shot> activeShots = World.PlayerManager.AllActiveShots;
            activeShots.RemoveAll(s => s.Player == this);
            activeShots.RemoveAll(s => s.State != ShotState.Active);

            if (FindNearestCollision(activeShots.Cast<IWorldObject>().ToList(), out collidingObject))
            {
                // get the shot that killed us
                Shot shot = (Shot)collidingObject;

                // we shall die
                Die(shot.Player);

                // now end that shot
                shot.End(false, true);
            }
        }

        protected override void HandleReceivedMessage(object sender, ServerLinkMessageEvent message)
        {
            base.HandleReceivedMessage(sender, message);
        }

        public override void Spawn(Vector2 position, float rotation)
        {
            // reset camera
            World.Camera.Zoom = 1;
            World.Camera.PanPosition = Vector2.Zero;

            base.Spawn(position, rotation);
        }

        public override void Die(Player killer)
        {
            // send out the death packet right away
            NetOutgoingMessage deathMessage = World.ServerLink.CreateMessage();

            MsgDeathPacket deathPacket = new MsgDeathPacket(killer.Slot);

            deathMessage.Write((Byte)deathPacket.MsgType);
            deathPacket.Write(deathMessage);

            World.ServerLink.SendMessage(deathMessage, NetDeliveryMethod.ReliableOrdered, 0);

            // write to console that you were killed
            ConsoleMessageLine consoleMessage;

            if (killer != this)
            {
                consoleMessage =
                    new ConsoleMessageLine(
                        "You were killed by ", Color.White,
                        killer.Callsign, ProtocolHelpers.TeamTypeToColor(killer.Team));
            }
            else
            {
                consoleMessage = new ConsoleMessageLine("You blew yourself up!", Color.White);
            }

            World.Console.WriteLine(consoleMessage);

            base.Die(killer);
        }

        protected override void Shoot(Byte shotSlot, Vector2 initialPosition, Single rotation, Vector2 initialVelocity, bool local)
        {
            // send out the shot begin packet right away
            NetOutgoingMessage shotBeginMessage = World.ServerLink.CreateMessage();

            MsgBeginShotPacket shotBeginPacket = new MsgBeginShotPacket(shotSlot, initialPosition, rotation, initialVelocity);

            shotBeginMessage.Write((Byte)shotBeginPacket.MsgType);
            shotBeginPacket.Write(shotBeginMessage);

            World.ServerLink.SendMessage(shotBeginMessage, NetDeliveryMethod.ReliableUnordered, 0);

            base.Shoot(shotSlot, initialPosition, rotation, initialVelocity, local);
        }
    }
}
