using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    public class Protocol
    {
        public readonly static UInt16 ProtocolVersion = 2;

        public enum MessageType
        {
            MsgEnter,
            MsgWorld
        }

        public enum TeamType
        {
            AutomaticTeam = 240,
            RogueTeam,
            RedTeam,
            GreenTeam,
            BlueTeam,
            PurpleTeam,
            ObserverTeam
        }
    }
}
