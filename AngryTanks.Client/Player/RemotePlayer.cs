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
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public class RemotePlayer : Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TimeSpan lastMsgUpdate;
        private TimeSpan msgUpdateFrequency;

        public RemotePlayer(World world, PlayerInformation playerInfo)
            : base(world, playerInfo)
        {
            this.lastMsgUpdate = new TimeSpan();

            // set our update frequency
            this.msgUpdateFrequency = new TimeSpan(0, 0, 0, 0, (int)(1000 / (UInt16)World.VarDB["updatesPerSecond"].Value));
        }

        public override void Update(GameTime gameTime)
        {
            Double factor = (gameTime.TotalGameTime.TotalMilliseconds - lastMsgUpdate.TotalMilliseconds) / msgUpdateFrequency.TotalMilliseconds;

            Position = Vector2.SmoothStep(oldPosition, newPosition, MathHelper.Clamp((Single)factor, 0, 1));
            
            // if the difference in the two rotations is greater than PI/4, then the angle was probably just wrapped
            // smoothstep goes crazy when angles are wrapped
            // TODO see if we can fix this
            if (Math.Abs(newRotation - oldRotation) > MathHelper.PiOver4)
                Rotation = newRotation;
            else
                Rotation = MathHelper.SmoothStep(oldRotation, newRotation, MathHelper.Clamp((Single)factor, 0, 1));

            base.Update(gameTime);
        }

        protected override void HandleReceivedMessage(object sender, ServerLinkMessageEvent message)
        {
            switch (message.MessageType)
            {
                case MessageType.MsgPlayerServerUpdate:
                    {
                        MsgPlayerServerUpdatePacket packet = (MsgPlayerServerUpdatePacket)message.MessageData;

                        // we are only interested if it is an update about this remote player
                        if (packet.Slot != Slot)
                            break;

                        // save our old stuff
                        oldPosition = Position;
                        oldRotation = Rotation;

                        // and set our new location
                        newPosition = packet.Position;
                        newRotation = packet.Rotation;

                        // and set our last update
                        lastMsgUpdate = message.Time.TotalGameTime;

                        break;
                    }
                    
                default:
                    break;
            }

            base.HandleReceivedMessage(sender, message);
        }
    }
}
