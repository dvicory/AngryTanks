using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using log4net;

namespace AngryTanks.Client
{
    /// <summary>
    /// Class that provides camera functionality to SpriteBatch.
    /// Once provided with a Viewport, it can provide a transformation Matrix.
    /// Based on in part from code by David Gouveia.
    /// </summary>
    public class Camera
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Viewport viewport;

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
            Origin = new Vector2(this.viewport.Width / 2.0f, this.viewport.Height / 2.0f);
            Zoom = 1.0f;
        }

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;

                // if there's a limit set and the camera is not transformed clamp position to limits
                if (Limits != null && Zoom == 1.0f && Rotation == 0.0f)
                {
                    position.X = MathHelper.Clamp(position.X, Limits.Value.X, Limits.Value.X + Limits.Value.Width - viewport.Width);
                    position.Y = MathHelper.Clamp(position.Y, Limits.Value.Y, Limits.Value.Y + Limits.Value.Height - viewport.Height);
                }
            }
        }

        public Vector2 Origin { get; set; }
        public Single Zoom { get; set; }
        public Single Rotation { get; set; }

        private Rectangle? limits;

        public Rectangle? Limits
        {
            get { return limits; }
            set
            {
                if (value != null)
                {
                    // assign limit but make sure it's always bigger than the viewport
                    limits = new Rectangle
                    {
                        X = value.Value.X,
                        Y = value.Value.Y,
                        Width = System.Math.Max(viewport.Width, value.Value.Width),
                        Height = System.Math.Max(viewport.Height, value.Value.Height)
                    };

                    // validate camera position with new limit
                    Position = Position;
                }
                else
                {
                    limits = null;
                }
            }
        }

        public void Move(Vector2 displacement, bool respectRotation)
        {
            if (respectRotation)
            {
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));
            }

            Position += displacement;
        }

        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
        }

        public Matrix GetViewMatrix(Single zoomFactor, bool preventParallax)
        {
            // we want a view matrix with a specific zoom factor
            // so save our existing zoom
            Single savedZoom = Zoom;
            Vector2 savedPosition = Position;

            if (preventParallax)
                Position /= zoomFactor;

            // now get the matrix with the new zoom
            Zoom *= zoomFactor;
            Matrix matrix = GetViewMatrix();

            // reset our zoom
            Zoom = savedZoom;

            if (preventParallax)
                Position = savedPosition;

            return matrix;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                   Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0));
        }

        public Matrix GetScrollMatrix(Vector2 textureSize)
        {
            return Matrix.CreateTranslation(new Vector3(-Origin / textureSize, 0)) *
                     Matrix.CreateScale(1 / Zoom) *
                     Matrix.CreateRotationZ(Rotation) *
                     Matrix.CreateTranslation(new Vector3(Origin / textureSize, 0)) *
                     Matrix.CreateTranslation(new Vector3(Position / textureSize, 0));
        }
    }
}
