using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using log4net;

using AngryTanks.Common;

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

        public Vector2 Origin { get; set; }

        private Vector2 position, panPosition;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;

                // validate our position
                position = ValidatePosition(position);
            }
        }

        public Vector2 PanPosition
        {
            get { return panPosition; }
            set
            {
                panPosition = value;

                // validate our panned position
                panPosition = ValidatePosition(panPosition);
            }
        }

        public Vector2 CameraPosition
        {
            get { return Position + PanPosition; }
        }

        private Single zoom;

        public Single Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;

                // validate everything
                zoom = ValidateZoom(zoom);
                panPosition = ValidatePosition(panPosition);
                position = ValidatePosition(position);
            }
        }

        private Single rotation;

        public Single Rotation
        {
            get { return rotation; }
            set
            {
                rotation = MathHelper.WrapAngle(value);
            }
        }

        private RectangleF limits = null;

        public RectangleF Limits
        {
            get { return limits; }
            set
            {
                limits = value;

                // validate everything
                zoom = ValidateZoom(zoom);
                panPosition = ValidatePosition(panPosition);
                position = ValidatePosition(position);
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

        public void Pan(Vector2 displacement, bool respectRotation)
        {
            if (respectRotation)
            {
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));
            }

            PanPosition += displacement;
        }

        public void LookAt(Vector2 position)
        {
            Position = position + panPosition - Origin;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-CameraPosition, 0)) *
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

        private Single ValidateZoom(Single zoom)
        {
            if (limits != null)
            {
                Single minZoomX = viewport.Width / limits.Width;
                Single minZoomY = viewport.Height / limits.Height;
                return MathHelper.Max(zoom, MathHelper.Max(minZoomX, minZoomY));
            }
            return zoom;
        }

        private Vector2 ValidatePosition(Vector2 pos)
        {
            if (limits != null)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(GetViewMatrix()));
                Vector2 cameraSize = new Vector2(viewport.Width, viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(limits.Left, limits.Top);
                Vector2 limitWorldMax = new Vector2(limits.Right, limits.Bottom);
                Vector2 positionOffset = pos - cameraWorldMin;
                return Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
            return pos;
        }
    }
}
