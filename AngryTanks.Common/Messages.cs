using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

using Lidgren.Network;

namespace AngryTanks.Common
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

    namespace Messages
    {
        public class PlayerInformation
        {
            public readonly Byte Slot;
            public readonly TeamType Team;
            public readonly String Callsign, Tag;

            public PlayerInformation(Byte slot, String callsign, String tag, TeamType team)
            {
                this.Slot = slot;
                this.Callsign = callsign;
                this.Tag = tag;
                this.Team = team;
            }

            public static PlayerInformation Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                TeamType team = (TeamType)packet.ReadByte();
                String callsign = packet.ReadString();
                String tag = packet.ReadString();

                return new PlayerInformation(slot, callsign, tag, team);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(Slot);
                packet.Write((Byte)Team);
                packet.Write(Callsign);
                packet.Write(Tag);
            }
        }

        public abstract class MsgBasePacket
        {
            public abstract MessageType MsgType
            {
                get;
            }
        }

        public class MsgStatePacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgState; }
            }

            public readonly Byte Slot;

            public MsgStatePacket(Byte slot)
            {
                this.Slot = slot;
            }

            public static MsgStatePacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();

                return new MsgStatePacket(slot);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(Slot);
            }
        }

        public class MsgEnterPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgEnter; }
            }

            public readonly PlayerInformation Player;

            public MsgEnterPacket(PlayerInformation player)
            {
                this.Player = player;
            }

            public static MsgEnterPacket Read(NetIncomingMessage packet)
            {
                PlayerInformation player = PlayerInformation.Read(packet);

                return new MsgEnterPacket(player);
            }

            public void Write(NetOutgoingMessage packet)
            {
                Player.Write(packet);
            }
        }

        public class MsgAddPlayerPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgAddPlayer; }
            }

            public readonly PlayerInformation Player;
            public readonly bool AddMyself;

            public MsgAddPlayerPacket(PlayerInformation player, bool addMyself)
            {
                this.Player = player;
                this.AddMyself = addMyself;
            }

            public static MsgAddPlayerPacket Read(NetIncomingMessage packet)
            {
                PlayerInformation player = PlayerInformation.Read(packet);
                bool addMyself = packet.ReadBoolean();

                return new MsgAddPlayerPacket(player, addMyself);
            }

            public void Write(NetOutgoingMessage packet)
            {
                Player.Write(packet);
                packet.Write(AddMyself);
            }
        }

        public class MsgRemovePlayerPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgRemovePlayer; }
            }

            public readonly Byte Slot;
            public readonly String Reason;

            public MsgRemovePlayerPacket(Byte slot, String reason)
            {
                this.Slot = slot;
                this.Reason = reason;
            }

            public static MsgRemovePlayerPacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                String reason = packet.ReadString();

                return new MsgRemovePlayerPacket(slot, reason);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(Slot);
                packet.Write(Reason);
            }
        }

        public class MsgWorldPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgWorld; }
            }

            public readonly UInt16 Length;
            public readonly StreamReader Map;

            public MsgWorldPacket(UInt16 mapLength, StreamReader map)
            {
                this.Length = mapLength;
                this.Map = map;
            }

            public static MsgWorldPacket Read(NetIncomingMessage packet)
            {
                throw new NotImplementedException();

                /*
                UInt16 mapLength = packet.ReadUInt16();
                Byte[] rawWorld = new Byte[mapLength];
                packet.ReadBytes(mapLength, out rawWorld);

                MemoryStream ms = new MemoryStream(rawWorld);
                StreamReader sr = new StreamReader(ms);

                return new MsgWorldPacket(mapLength, sr);
                */
            }

            public void Write(NetOutgoingMessage packet)
            {
                throw new NotImplementedException();
            }
        }

        public abstract class MsgPlayerUpdatePacket : MsgBasePacket
        {
            public readonly Vector2 Position;
            public readonly Single Rotation;

            public MsgPlayerUpdatePacket(Vector2 position, Single rotation)
            {
                this.Position = position;
                this.Rotation = rotation;
            }

            public virtual void Write(NetOutgoingMessage packet)
            {
                packet.Write(Position.X);
                packet.Write(Position.Y);
                packet.Write(Rotation);
            }
        }

        public class MsgPlayerClientUpdatePacket : MsgPlayerUpdatePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgPlayerClientUpdate; }
            }

            public MsgPlayerClientUpdatePacket(Vector2 position, Single rotation)
                : base(position, rotation)
            {
            }

            public static MsgPlayerClientUpdatePacket Read(NetIncomingMessage packet)
            {
                Vector2 position = new Vector2 { X = packet.ReadSingle(), Y = packet.ReadSingle() };
                Single rotation = packet.ReadSingle();

                return new MsgPlayerClientUpdatePacket(position, rotation);
            }
        }

        public class MsgPlayerServerUpdatePacket : MsgPlayerUpdatePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgPlayerServerUpdate; }
            }

            public readonly Byte Slot;

            public MsgPlayerServerUpdatePacket(Byte slot, Vector2 position, Single rotation)
                : base(position, rotation)
            {
                this.Slot = slot;
            }

            public MsgPlayerServerUpdatePacket(Byte slot, MsgPlayerClientUpdatePacket clientUpdate)
                : base(clientUpdate.Position, clientUpdate.Rotation)
            {
                this.Slot = slot;
            }

            public static MsgPlayerServerUpdatePacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                Vector2 position = new Vector2 { X = packet.ReadSingle(), Y = packet.ReadSingle() };
                Single rotation = packet.ReadSingle();

                return new MsgPlayerServerUpdatePacket(slot, position, rotation);
            }

            public override void Write(NetOutgoingMessage packet)
            {
                packet.Write(Slot);
                base.Write(packet);
            }
        }
    }
}
