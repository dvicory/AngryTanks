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
using AngryTanks.Common.Messages;

namespace AngryTanks.Client
{
    public class LocalPlayer : Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private KeyboardState kb;

        private Single velocityFactor, angularVelocityFactor;
        private Single maxVelocity, maxAngularVelocity;

        public LocalPlayer(World world, PlayerInformation playerInfo)
            : base(world, playerInfo)
        {
            // TODO support if these variables change
            this.maxVelocity = (Single)World.VarDB["tankSpeed"].Value;
            this.maxAngularVelocity = (Single)World.VarDB["tankAngVel"].Value;
        }

        public override void Update(GameTime gameTime, List<Sprite> collisionObjects)
        {
            kb = Keyboard.GetState();

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
                newVelocity.X = velocityFactor * maxVelocity * (Single)Math.Cos(Rotation - Math.PI / 2);
                newVelocity.Y = velocityFactor * maxVelocity * (Single)Math.Sin(Rotation - Math.PI / 2);
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
            {
                newAngularVelocity = angularVelocityFactor * maxAngularVelocity;
            }
            else
            {
                newAngularVelocity = 0;
            }

            // update based on newly found out velocities/positions
            Velocity = (oldVelocity + newVelocity) * 0.5f;
            newPosition += Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            AngularVelocity = MathHelper.WrapAngle((Single)(newAngularVelocity + oldAngularVelocity) * 0.5f);
            newRotation += AngularVelocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            // check for any collisions at our new location
            Single overlap;
            Vector2 collisionProjection;

            if (FindNearestCollision(collisionObjects, out overlap, out collisionProjection))
            {
                // move our position back to old position
                newPosition += overlap * collisionProjection;
                oldPosition = newPosition;
            }

            // finally confirm our position
            Position = newPosition;
            Rotation = newRotation;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawStretched(gameTime, spriteBatch);
        }
    }
}