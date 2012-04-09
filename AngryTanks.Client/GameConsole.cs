using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using log4net;

using AngryTanks.Common;

namespace AngryTanks.Client
{
    public struct ConsoleMessagePart
    {
        public readonly String Message;
        public readonly Color Color;

        public ConsoleMessagePart(String message)
        {
            this.Message = message;
            this.Color   = Color.White;
        }

        public ConsoleMessagePart(String message, Color color)
        {
            this.Message = message;
            this.Color   = color;
        }
    }

    public class ConsoleMessageLine
    {
        public readonly LinkedList<ConsoleMessagePart> Parts = new LinkedList<ConsoleMessagePart>();

        public ConsoleMessageLine(String message)
        {
            this.Parts.AddFirst(new ConsoleMessagePart(message));
        }

        public ConsoleMessageLine(String message, Color color)
        {
            this.Parts.AddFirst(new ConsoleMessagePart(message, color));
        }

        public ConsoleMessageLine(params object[] list)
        {
            if (list.Length % 2 != 0)
                throw new ArgumentException("you must pass a message and then a color");

            this.Parts = new LinkedList<ConsoleMessagePart>();

            for (int i = 0; i < list.Length; i += 2)
            {
                String message = Convert.ToString(list[i]);
                Color color = (Color)list[i+1];

                this.Parts.AddLast(new ConsoleMessagePart(message, color));
            }
        }
    }

    public class GameConsole
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GameConsole Properties

        public Vector2 Position
        {
            get; set;
        }

        public Vector2 Size
        {
            get; set;
        }
        
        #endregion

        /// <summary>
        /// Allows us to get a reference to various services
        /// </summary>
        private readonly IServiceProvider IService;

        private ContentManager contentManager;
        private SpriteBatch spriteBatch;
        private SpriteFont consoleFont;

        private LinkedList<ConsoleMessageLine> lines = new LinkedList<ConsoleMessageLine>();

        public GameConsole(IServiceProvider iservice, Vector2 position, Vector2 size)
        {
            this.IService = iservice;
            this.Position = position;
            this.Size     = size;
        }

        public void Initialize()
        {
            contentManager = new ContentManager(IService, "Content");

            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)IService.GetService(typeof(IGraphicsDeviceService));

            spriteBatch = new SpriteBatch(graphicsDeviceService.GraphicsDevice);
        }

        public void LoadContent()
        {
            consoleFont = contentManager.Load<SpriteFont>("fonts/ConsoleFont10");
        }

        public void UnloadContent()
        {
            contentManager.Unload();
        }

        public void Draw(GameTime gameTime)
        {
            Vector2 remaining = Size;
            remaining.X = Position.X;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            foreach (ConsoleMessageLine line in lines)
            {
                foreach (ConsoleMessagePart part in line.Parts)
                {
                    Vector2 messageSize = consoleFont.MeasureString(part.Message);
                    remaining.Y -= messageSize.Y;

                    if (remaining.Y <= 0)
                        break;

                    spriteBatch.DrawString(consoleFont, part.Message, Position + remaining, part.Color);
                }
            }

            spriteBatch.End();
        }

        public void WriteLine(String message)
        {
            WriteLine(new ConsoleMessageLine(message));
        }

        public void WriteLine(String message, Color color)
        {
            WriteLine(new ConsoleMessageLine(message, color));            
        }

        public virtual void WriteLine(ConsoleMessageLine consoleMessage)
        {
            lines.AddFirst(consoleMessage);
        }
    }
}
