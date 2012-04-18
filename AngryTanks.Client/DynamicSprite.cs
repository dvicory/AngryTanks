using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using log4net;

namespace AngryTanks.Client
{
    public class StateChangedEvent<T> : EventArgs
    {
        public readonly T OldValue, NewValue;

        public StateChangedEvent(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class DynamicSprite : Sprite
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region State Event Handlers

        public event EventHandler<StateChangedEvent<Vector2>> PositionChangedEvent;
        public event EventHandler<StateChangedEvent<Vector2>> SizeChangedEvent;
        public event EventHandler<StateChangedEvent<Single>>  RotationChangedEvent;
        public event EventHandler<StateChangedEvent<Vector2>> VelocityChangedEvent;
        public event EventHandler<StateChangedEvent<Single>>  AngularVelocityChangedEvent;

        protected void FireChangeEvent<T>(EventHandler<StateChangedEvent<T>> handler, T oldValue, T newValue)
        {
            // prevent race condition
            if (handler != null)
            {
                // notify delegates attached to event
                StateChangedEvent<T> e = new StateChangedEvent<T>(oldValue, newValue);
                handler(this, e);
            }
        }

        #endregion

        #region State Event Testers

        private void AttachTestEventHandlers()
        {
            PositionChangedEvent        += new EventHandler<StateChangedEvent<Vector2>>(DynamicSprite_PositionChangedEvent);
            SizeChangedEvent            += new EventHandler<StateChangedEvent<Vector2>>(DynamicSprite_SizeChangedEvent);
            RotationChangedEvent        += new EventHandler<StateChangedEvent<Single>> (DynamicSprite_RotationChangedEvent);
            VelocityChangedEvent        += new EventHandler<StateChangedEvent<Vector2>>(DynamicSprite_VelocityChangedEvent);
            AngularVelocityChangedEvent += new EventHandler<StateChangedEvent<Single>> (DynamicSprite_AngularVelocityChangedEvent);
        }

        private void DynamicSprite_PositionChangedEvent(object sender, StateChangedEvent<Vector2> e)
        {
            Log.DebugFormat("received position change event (old position: {0}, new position: {1}", e.OldValue, e.NewValue);
        }

        private void DynamicSprite_SizeChangedEvent(object sender, StateChangedEvent<Vector2> e)
        {
            Log.DebugFormat("received size change event (old size: {0}, new size: {1}", e.OldValue, e.NewValue);
        }

        private void DynamicSprite_RotationChangedEvent(object sender, StateChangedEvent<Single> e)
        {
            Log.DebugFormat("received rotation change event (old rotation: {0}, new rotation: {1}", e.OldValue, e.NewValue);
        }

        private void DynamicSprite_VelocityChangedEvent(object sender, StateChangedEvent<Vector2> e)
        {
            Log.DebugFormat("received velocity change event (old velocity: {0}, new velocity: {1}", e.OldValue, e.NewValue);
        }

        private void DynamicSprite_AngularVelocityChangedEvent(object sender, StateChangedEvent<Single> e)
        {
            Log.DebugFormat("received angular velocity change event (old angular velocity: {0}, new angular velocity: {1}", e.OldValue, e.NewValue);
        }

        #endregion

        #region Properties

        /*
         * TODO
         * use velocity verlet integration to update position more accurately
         * see http://lol.zoy.org/blog/2011/12/14/understanding-motion-in-games
         * 
         */

        private   Vector2 position;
        protected Vector2 oldPosition, newPosition;

        public override Vector2 Position
        {
            get { return position; }
            set
            {
                FireChangeEvent<Vector2>(PositionChangedEvent, Position, value);
                position = value;
            }
        }

        private Vector2 size;

        public override Vector2 Size
        {
            get { return size; }
            set
            {
                FireChangeEvent<Vector2>(SizeChangedEvent, Size, value);
                size = value;
            }
        }

        private   Single rotation;
        protected Single oldRotation, newRotation;

        public override Single Rotation
        {
            get { return rotation; }
            set
            {
                FireChangeEvent<Single>(RotationChangedEvent, Rotation, value);
                rotation = value;
            }
        }

        private   Vector2 velocity;
        protected Vector2 oldVelocity, newVelocity;

        public virtual Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                FireChangeEvent<Vector2>(VelocityChangedEvent, Velocity, value);
                velocity = value;
            }
        }

        private   Single angularVelocity;
        protected Single oldAngularVelocity, newAngularVelocity;

        public virtual Single AngularVelocity
        {
            get { return angularVelocity; }
            set
            {
                FireChangeEvent<Single>(AngularVelocityChangedEvent, AngularVelocity, value);
                angularVelocity = value;
            }
        }

        #endregion

        public DynamicSprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(world, texture, position, size, rotation)
        {
            if (Log.IsDebugEnabled)
                AttachTestEventHandlers();
        }

        public DynamicSprite(World world, Texture2D texture, Vector2 position, Vector2 size, Single rotation, Color color)
            : base(world, texture, position, size, rotation, color)
        {
            if (Log.IsDebugEnabled)
                AttachTestEventHandlers();
        }

        public virtual void Update(GameTime gameTime)
        {
            // TODO implement changing position and velocity

            // update rectangle bounds
            Bounds.X = Position.X - Size.X / 2;
            Bounds.Y = Position.Y - Size.Y / 2;
            Bounds.Rotation = Rotation;

            // size doesn't *normally* change, but...
            Bounds.Width = Size.X;
            Bounds.Height = Size.Y;
        }
    }
}
