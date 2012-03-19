using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    public class Protocol
    {
        public readonly static Byte ProtocolVersion = 1;

        public enum MessageType
        {
            MsgEnter,
            MsgWorld
        }

        public enum TeamType
        {
            RogueTeam,
            RedTeam,
            GreenTeam,
            BlueTeam,
            PurpleTeam,
            ObserverTeam
        }
    }
}
