using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngryTanks.Common
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

    namespace Messages
    {
        public abstract class MsgBaseData
        {
            public abstract MessageType MsgType
            {
                get;
            }
        }

        public class PlayerInformation
        {
            public readonly Byte Slot;
            public readonly String Callsign, Tag;
            public readonly TeamType Team;
        }

        public class MsgEnterData : MsgBaseData
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgEnter; }
            }

            public readonly PlayerInformation MsgBaseData;
        }

        public class MsgAddPlayerData : MsgBaseData
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgAddPlayer; }
            }

            public readonly PlayerInformation Player;
        }

        public class MsgWorldData : MsgBaseData
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgWorld; }
            }

            public readonly StreamReader Map;

            public MsgWorldData(StreamReader map)
            {
                this.Map = map;
            }
        }

        public class MsgPlayerUpdateData : MsgBaseData
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgPlayerUpdate; }
            }

            public readonly Byte Slot;
            public readonly Vector2 Position, Velocity;
        }
    }
}
