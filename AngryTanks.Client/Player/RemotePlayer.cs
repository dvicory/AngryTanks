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

        public RemotePlayer(World world, PlayerInformation playerInfo)
            : base(world, playerInfo)
        {
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

                        // if not, set our new location
                        Position = packet.Position;
                        Velocity = packet.Velocity;
                        Rotation = packet.Rotation;

                        break;
                    }
                    
                default:
                    break;
            }

            base.HandleReceivedMessage(sender, message);
        }
    }
}
