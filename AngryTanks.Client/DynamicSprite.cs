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
        public readonly string Message;
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
            PositionChangedEvent += new EventHandler<StateChangedEvent<Vector2>>(DynamicSprite_PositionChangedEvent);
            SizeChangedEvent     += new EventHandler<StateChangedEvent<Vector2>>(DynamicSprite_SizeChangedEvent);
            RotationChangedEvent += new EventHandler<StateChangedEvent<Single>> (DynamicSprite_RotationChangedEvent);
            VelocityChangedEvent += new EventHandler<StateChangedEvent<Vector2>>(DynamicSprite_VelocityChangedEvent);
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

        #endregion

        #region Properties

        /*
         * TODO
         * use velocity verlet integration to update position more accurately
         * see http://lol.zoy.org/blog/2011/12/14/understanding-motion-in-games
         * 
         */

        private   Vector2 _Position;
        protected Vector2 OldPosition, NewPosition;

        protected override Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                FireChangeEvent<Vector2>(PositionChangedEvent, Position, value);
                _Position = value;
            }
        }

        private Vector2 _Size;

        protected override Vector2 Size
        {
            get
            {
                return _Size;
            }
            set
            {
                FireChangeEvent<Vector2>(SizeChangedEvent, Size, value);
                _Size = value;
            }
        }

        private Single _Rotation;

        protected override Single Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                FireChangeEvent<Single>(RotationChangedEvent, Rotation, value);
                _Rotation = value;
            }
        }

        // Velocity is backed by _Velocity, this is so we can set it initially in the constructor without firing an event or doing our funky velocity stuff
        private   Vector2 _Velocity;
        protected Vector2 OldVelocity, NewVelocity;

        protected virtual Vector2 Velocity
        {
            get
            {
                return _Velocity;
            }
            set
            {
                FireChangeEvent<Vector2>(VelocityChangedEvent, Velocity, value);
                _Velocity = value;
            }
        }

        #endregion

        public DynamicSprite(Texture2D texture, Vector2 position, Vector2 size, Single rotation)
            : base(texture, position, size, rotation)
        {
            if (Log.IsDebugEnabled)
                AttachTestEventHandlers();
        }

        public DynamicSprite(Texture2D texture, Vector2 position, Vector2 size, Single rotation, Vector2 velocity)
            : base(texture, position, size, rotation)
        {
            this._Velocity = velocity;

            if (Log.IsDebugEnabled)
                AttachTestEventHandlers();
        }

        public virtual void Update(GameTime gameTime)
        {
            // TODO implement changing position and velocity
        }
    }
}
