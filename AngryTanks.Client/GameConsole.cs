using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input;
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

    public interface IGameConsole
    {
        #region Properties

        bool Opened
        {
            get;
            set;
        }

        Vector2 Position
        {
            get;
            set;
        }

        Vector2 Size
        {
            get;
            set;
        }

        Vector2 Margin
        {
            get;
            set;
        }

        Vector2 Padding
        {
            get;
            set;
        }

        Color BackgroundColor
        {
            get;
            set;
        }

        ConsoleMessagePart PromptPrefix
        {
            get;
            set;
        }

        bool PromptActive
        {
            get;
            set;
        }

        Int16 PromptBlinkRate
        {
            get;
            set;
        }

        RectangleF Bounds
        {
            get;
        }

        RectangleF InnerBounds
        {
            get;
        }

        #endregion

        #region Methods

        void WriteLine(String message);
        void WriteLine(String message, Color color);
        void WriteLine(ConsoleMessageLine consoleMessage);

        #endregion
    }

    public class GameConsole : DrawableGameComponent, IGameConsole
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GameConsole Properties

        public bool Opened
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public Vector2 Size
        {
            get;
            set;
        }

        public Vector2 Margin
        {
            get;
            set;
        }

        public Vector2 Padding
        {
            get;
            set;
        }

        public Color BackgroundColor
        {
            get;
            set;
        }

        public ConsoleMessagePart PromptPrefix
        {
            get;
            set;
        }

        public bool PromptActive
        {
            get;
            set;
        }

        public Int16 PromptBlinkRate
        {
            get;
            set;
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF(Position + Margin, Size - (Margin * 2));
            }
        }

        public RectangleF InnerBounds
        {
            get
            {
                return new RectangleF(Bounds.X + Padding.X, Bounds.Y + Padding.Y,
                                      Bounds.Width - Padding.X, Bounds.Height - Padding.Y);
            }
        }
        
        #endregion

        private IInputService inputService;

        private SpriteBatch spriteBatch;
        private SpriteFont consoleFont;
        private Texture2D background;

        private int currentBlinkTime;
        private bool promptBlinking;

        private String currentPromptInput = "";
        private bool promptJustOpened = false;

        private LinkedList<ConsoleMessageLine> lines = new LinkedList<ConsoleMessageLine>();

        public GameConsole(Game game, Vector2 position, Vector2 size, Vector2 margin, Vector2 padding, Color backgroundColor)
            : base(game)
        {
            this.Position = position;
            this.Size     = size;
            this.Margin   = margin;
            this.Padding  = padding;
            this.BackgroundColor = backgroundColor;

            this.Opened = true;
            this.PromptPrefix = new ConsoleMessagePart("# ", Color.Yellow);
            this.PromptActive = false;
            this.PromptBlinkRate = 200; // 200 ms prompt blink rate

            // register ourselves as a service
            if (Game.Services != null)
                Game.Services.AddService(typeof(IGameConsole), this);

            inputService = (IInputService)Game.Services.GetService(typeof(IInputService));

            inputService.GetKeyboard().CharacterEntered += CharacterEntered;
            inputService.GetKeyboard().KeyPressed += KeyPressed;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // unregister event handlers
                try
                {
                    inputService.GetKeyboard().CharacterEntered -= CharacterEntered;
                    inputService.GetKeyboard().KeyPressed -= KeyPressed;
                }
                // our input service disposed before we did... a dirty hack
                catch (NullReferenceException e)
                {
                    Log.Error(e.Message);
                    Log.Error(e.StackTrace);
                }

                // remove service
                if (Game.Services != null)
                {
                    IGameConsole gameConsoleService = (IGameConsole)Game.Services.GetService(typeof(IGameConsole));

                    if (ReferenceEquals(gameConsoleService, this))
                        Game.Services.RemoveService(typeof(IGameConsole));
                }
            }

            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            consoleFont = Game.Content.Load<SpriteFont>("fonts/ConsoleFont12");
            background = new Texture2D(GraphicsDevice, 1, 1);
            background.SetData(new Color[] { Color.Black });

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected void KeyPressed(Keys key)
        {
            switch (key)
            {
                // shows/hides console
                // closing an open prompt if it's open 
                case Keys.OemTilde:
                    if (!PromptActive)
                        Opened = !Opened;

                    // prompt is never active if we're closed
                    if (!Opened)
                    {
                        PromptActive = false;
                        currentPromptInput = "";
                    }

                    break;

                // starts a new prompt if one wasn't already started (/ is a valid input key as well)
                case Keys.OemQuestion:
                    // open if we weren't already open
                    if (!Opened)
                        Opened = true;

                    if (!PromptActive)
                    {
                        currentPromptInput = "";
                        PromptActive = true;
                        promptJustOpened = true;
                    }

                    break;

                // backspaces prompt
                case Keys.Back:
                    if (currentPromptInput.Length > 0)
                        currentPromptInput = currentPromptInput.Substring(0, currentPromptInput.Length - 1);

                    break;

                // abandon whatever was in prompt
                case Keys.Escape:
                    PromptActive = false;
                    currentPromptInput = "";

                    break;

                // submits contents of prompt
                case Keys.Enter:
                    // TODO handle submitting prompt
                    PromptActive = false;
                    currentPromptInput = "";

                    break;

                default:
                    break;
            }
        }

        protected void CharacterEntered(char c)
        {
            // we should ignore this key input because it was used to open the prompt
            if (promptJustOpened)
            {
                promptJustOpened = false;
                return;
            }

            if (PromptActive && c >= (char)32 && c <= (char)126 && c != '\\')
                currentPromptInput += c;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Opened)
            {
                base.Draw(gameTime);
                return;
            }

            currentBlinkTime += gameTime.ElapsedGameTime.Milliseconds;

            Vector2 position = new Vector2(InnerBounds.X, InnerBounds.Y + InnerBounds.Height);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            spriteBatch.Draw(background,
                             (Rectangle)Bounds,
                             null, BackgroundColor, 0,
                             Vector2.Zero,
                             SpriteEffects.None, 0);

            position.Y -= DrawPrompt(gameTime, position);

            foreach (ConsoleMessageLine line in lines)
            {
                // we have no more room left to draw
                if (position.Y < (InnerBounds.Y + consoleFont.MeasureString("qABC!").Y))
                    break;

                position.Y -= DrawLine(gameTime, line, position);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public virtual Single DrawLine(GameTime gameTime, ConsoleMessageLine line, Vector2 position)
        {
            // decrement our intial position
            Vector2 messageSize = consoleFont.MeasureString("ABC!");
            position.Y -= messageSize.Y / 2; // nuclex's truetype font importer measures from the baseline

            foreach (ConsoleMessagePart part in line.Parts)
            {
                // TODO break lines/words to span across multiple lines if necessary
                spriteBatch.DrawString(consoleFont,
                                       part.Message,
                                       position,
                                       part.Color, 0,
                                       Vector2.Zero,
                                       1, SpriteEffects.None, 1);

                position.X += consoleFont.MeasureString(part.Message).X;
            }

            return messageSize.Y;
        }

        public virtual Single DrawPrompt(GameTime gameTime, Vector2 position)
        {
            ConsoleMessageLine promptLine = new ConsoleMessageLine();

            // blink prompt
            if (currentBlinkTime > PromptBlinkRate)
            {
                promptBlinking = !promptBlinking;
                currentBlinkTime -= PromptBlinkRate;
            }
            
            if (promptBlinking && PromptActive)
                promptLine.Parts.AddLast(new ConsoleMessagePart(PromptPrefix.Message, Color.Black));
            else
                promptLine.Parts.AddLast(PromptPrefix);

            promptLine.Parts.AddLast(new ConsoleMessagePart(currentPromptInput == null ? "" : currentPromptInput,
                                                            Color.White));

            return DrawLine(gameTime, promptLine, position);
        }

        public void WriteLine(String message)
        {
            WriteLine(new ConsoleMessageLine(message));
        }

        public void WriteLine(String message, Color color)
        {
            WriteLine(new ConsoleMessageLine(message, color));            
        }

        public void WriteLine(ConsoleMessageLine consoleMessage)
        {
            lines.AddFirst(consoleMessage);
        }
    }
}
