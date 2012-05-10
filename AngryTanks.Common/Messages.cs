using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

using Lidgren.Network;

using AngryTanks.Common.Extensions.LidgrenExtensions;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Common
{
    using MessageType = Protocol.MessageType;
    using TeamType    = Protocol.TeamType;

    namespace Messages
    {
        /// <summary>
        /// Core information tied to a player.
        /// </summary>
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
                packet.Write(this.Slot);
                packet.Write((Byte)this.Team);
                packet.Write(this.Callsign);
                packet.Write(this.Tag);
            }
        }

        /// <summary>
        /// Abstract base class for all messages.
        /// </summary>
        public abstract class MsgBasePacket
        {
            public abstract MessageType MsgType
            {
                get;
            }
        }

        /// <summary>
        /// Sent by the client upon connecting to server with desired <see cref="PlayerInformation"/>.
        /// </summary>
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
                this.Player.Write(packet);
            }
        }

        /// <summary>
        /// Sent by the server containing basic game information.
        /// </summary>
        public class MsgGameInformationPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgGameInformation; }
            }

            public readonly GamePlayType GamePlayType;

            public MsgGameInformationPacket(GamePlayType gamePlayType)
            {
                this.GamePlayType = gamePlayType;
            }

            public static MsgGameInformationPacket Read(NetIncomingMessage packet)
            {
                GamePlayType gamePlayType = (GamePlayType)packet.ReadByte();

                return new MsgGameInformationPacket(gamePlayType);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write((Byte)this.GamePlayType);
            }
        }

        /// <summary>
        /// Sent by the client to indicate it is ready to receive initial state.
        /// Sent by the server to indicate initial state is fully sent.
        /// </summary>
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
                packet.Write(this.Slot);
            }
        }

        /// <summary>
        /// Sent by the server to set a variable in the <see cref="VariableDatabase"/>.
        /// </summary>
        public class MsgSetVariablePacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgSetVariable; }
            }

            public readonly TypeCode TypeCode;
            public readonly String Name;
            public readonly Object Value;

            public MsgSetVariablePacket(String name, Object value, TypeCode typeCode)
            {
                this.Name = name;
                this.Value = value;
                this.TypeCode = typeCode;
            }

            public MsgSetVariablePacket(VariableStore variable)
            {
                this.TypeCode = variable.TypeCode;
                this.Name = variable.Name;
                this.Value = variable.Value;
            }

            public static MsgSetVariablePacket Read(NetIncomingMessage packet)
            {
                TypeCode typeCode = (TypeCode)packet.ReadByte();
                String name = packet.ReadString();
                Object value = ReadValue(packet, typeCode);

                return new MsgSetVariablePacket(name, value, typeCode);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write((Byte)this.TypeCode);
                packet.Write(this.Name);
                WriteValue(packet, this.Value, this.TypeCode);
            }

            private static Object ReadValue(NetIncomingMessage packet, TypeCode typeCode)
            {
                Object value;

                switch (typeCode)
                {
                    case TypeCode.Empty:
                        throw new NotSupportedException("Empty is not a supported type for variables");

                    case TypeCode.Object:
                        throw new NotSupportedException("Object is not a supported type for variables");

                    case TypeCode.DBNull:
                        throw new NotSupportedException("DBNull is not a supported type for variables");

                    case TypeCode.Boolean:
                        value = packet.ReadBoolean();
                        break;

                    case TypeCode.Char:
                        throw new NotSupportedException("Char is not a supported type for variables");

                    case TypeCode.SByte:
                        value = packet.ReadSByte();
                        break;

                    case TypeCode.Byte:
                        value = packet.ReadByte();
                        break;

                    case TypeCode.Int16:
                        value = packet.ReadInt16();
                        break;

                    case TypeCode.UInt16:
                        value = packet.ReadUInt16();
                        break;

                    case TypeCode.Int32:
                        value = packet.ReadInt32();
                        break;

                    case TypeCode.UInt32:
                        value = packet.ReadUInt32();
                        break;

                    case TypeCode.Int64:
                        value = packet.ReadInt64();
                        break;

                    case TypeCode.UInt64:
                        value = packet.ReadUInt64();
                        break;

                    case TypeCode.Single:
                        value = packet.ReadSingle();
                        break;

                    case TypeCode.Double:
                        value = packet.ReadDouble();
                        break;

                    case TypeCode.Decimal:
                        throw new NotSupportedException("Decimal is not a supported type for variables");

                    case TypeCode.DateTime:
                        throw new NotSupportedException("DateTime is not a supported type for variables");

                    case TypeCode.String:
                        value = packet.ReadString();
                        break;

                    default:
                        throw new NotSupportedException(String.Format("Unknown type {0} is not a supported type for variables", typeCode));
                }

                return value;
            }

            private static void WriteValue(NetOutgoingMessage packet, Object value, TypeCode typeCode)
            {
                switch (typeCode)
                {
                    case TypeCode.Empty:
                        throw new NotSupportedException("Empty is not a supported type for variables");

                    case TypeCode.Object:
                        throw new NotSupportedException("Object is not a supported type for variables");

                    case TypeCode.DBNull:
                        throw new NotSupportedException("DBNull is not a supported type for variables");

                    case TypeCode.Boolean:
                        packet.Write((Boolean)Convert.ChangeType(value, typeof(Boolean)));
                        break;

                    case TypeCode.Char:
                        throw new NotSupportedException("Char is not a supported type for variables");

                    case TypeCode.SByte:
                        packet.Write((SByte)Convert.ChangeType(value, typeof(SByte)));
                        break;

                    case TypeCode.Byte:
                        packet.Write((Byte)Convert.ChangeType(value, typeof(Byte)));
                        break;

                    case TypeCode.Int16:
                        packet.Write((Int16)Convert.ChangeType(value, typeof(Int16)));
                        break;

                    case TypeCode.UInt16:
                        packet.Write((UInt16)Convert.ChangeType(value, typeof(UInt16)));
                        break;

                    case TypeCode.Int32:
                        packet.Write((Int32)Convert.ChangeType(value, typeof(Int32)));
                        break;

                    case TypeCode.UInt32:
                        packet.Write((UInt32)Convert.ChangeType(value, typeof(UInt32)));
                        break;

                    case TypeCode.Int64:
                        packet.Write((Int64)Convert.ChangeType(value, typeof(Int64)));
                        break;

                    case TypeCode.UInt64:
                        packet.Write((UInt64)Convert.ChangeType(value, typeof(UInt64)));
                        break;

                    case TypeCode.Single:
                        packet.Write((Single)Convert.ChangeType(value, typeof(Single)));
                        break;

                    case TypeCode.Double:
                        packet.Write((Double)Convert.ChangeType(value, typeof(Double)));
                        break;

                    case TypeCode.Decimal:
                        throw new NotSupportedException("Decimal is not a supported type for variables");

                    case TypeCode.DateTime:
                        throw new NotSupportedException("DateTime is not a supported type for variables");

                    case TypeCode.String:
                        packet.Write((String)Convert.ChangeType(value, typeof(String)));
                        break;

                    default:
                        throw new NotSupportedException(String.Format("Unknown type {0} is not a supported type for variables", typeCode));
                }
            }
        }

        /// <summary>
        /// Sent by the server to add a player.
        /// </summary>
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
                this.Player.Write(packet);
                packet.Write(this.AddMyself);
            }
        }

        /// <summary>
        /// Sent by the server to remove a player.
        /// </summary>
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
                packet.Write(this.Slot);
                packet.Write(this.Reason);
            }
        }

        /// <summary>
        /// Sent by the server to give the client the map.
        /// </summary>
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

        /// <summary>
        /// Abstract base class for player updates.
        /// </summary>
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
                packet.Write(this.Position);
                packet.Write(this.Rotation);
            }
        }

        /// <summary>
        /// Player update sent by the client.
        /// </summary>
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
                Vector2 position = packet.ReadVector2();
                Single rotation = packet.ReadSingle();

                return new MsgPlayerClientUpdatePacket(position, rotation);
            }
        }

        /// <summary>
        /// Player update sent by the server.
        /// </summary>
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
                Vector2 position = packet.ReadVector2();
                Single rotation = packet.ReadSingle();

                return new MsgPlayerServerUpdatePacket(slot, position, rotation);
            }

            public override void Write(NetOutgoingMessage packet)
            {
                packet.Write(this.Slot);
                base.Write(packet);
            }
        }

        /// <summary>
        /// Sent by the client to request a spawn.
        /// Sent by the server to spawn a player.
        /// </summary>
        public class MsgDeathPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgDeath; }
            }

            public readonly Byte Slot;
            public readonly Byte Killer;

            /// <summary>
            /// Used to construct a <see cref="MsgDeathPacket"/> on the client to tell the server it got killed.
            /// </summary>
            public MsgDeathPacket(Byte killer)
            {
                this.Slot = ProtocolInformation.DummySlot;
                this.Killer = killer;
            }

            /// <summary>
            /// Used to construct a <see cref="MsgDeathPacket"/> on the server to inform about a death.
            /// </summary>
            public MsgDeathPacket(Byte slot, Byte killer)
            {
                this.Slot = slot;
                this.Killer = killer;
            }

            public static MsgDeathPacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                Byte killer = packet.ReadByte();

                return new MsgDeathPacket(slot, killer);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(this.Slot);
                packet.Write(this.Killer);
            }
        }

        /// <summary>
        /// Sent by the client to request a spawn.
        /// Sent by the server to spawn a player.
        /// </summary>
        public class MsgSpawnPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgSpawn; }
            }

            public readonly Byte Slot;
            public readonly Vector2 Position;
            public readonly Single Rotation;

            /// <summary>
            /// Used to construct a <see cref="MsgSpawnPacket"/> on the client to request a spawn.
            /// </summary>
            public MsgSpawnPacket()
            {
                this.Slot = ProtocolInformation.DummySlot;
                this.Position = Vector2.Zero;
                this.Rotation = 0;
            }

            /// <summary>
            /// Used to construct a <see cref="MsgSpawnPacket"/> on the server to inform about a spawn.
            /// </summary>
            public MsgSpawnPacket(Byte slot, Vector2 position, Single rotation)
            {
                this.Slot = slot;
                this.Position = position;
                this.Rotation = rotation;
            }

            public static MsgSpawnPacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                Vector2 position = packet.ReadVector2();
                Single rotation = packet.ReadSingle();

                return new MsgSpawnPacket(slot, position, rotation);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(this.Slot);
                packet.Write(this.Position);
                packet.Write(this.Rotation);
            }
        }

        /// <summary>
        /// Sent by the client to begin a shot.
        /// Sent by the server to tell other players to begin the shot.
        /// </summary>
        public class MsgShotBeginPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgShotBegin; }
            }

            public readonly Byte Slot;
            public readonly Byte ShotSlot;
            public readonly Vector2 Position;
            public readonly Single Rotation;
            public readonly Vector2 Velocity;

            /// <summary>
            /// Used to construct a <see cref="MsgShotBeginPacket"/> on the client to notify about a new shot.
            /// </summary>
            public MsgShotBeginPacket(Byte shotSlot, Vector2 position, Single rotation, Vector2 velocity)
                : this(ProtocolInformation.DummySlot, shotSlot, position, rotation, velocity)
            { }

            /// <summary>
            /// Used to construct a <see cref="MsgShotBegin"/> on the server to inform about a shot.
            /// </summary>
            public MsgShotBeginPacket(Byte slot, Byte shotSlot, Vector2 position, Single rotation, Vector2 velocity)
            {
                this.Slot     = slot;
                this.ShotSlot = shotSlot;
                this.Position = position;
                this.Rotation = rotation;
                this.Velocity = velocity;
            }

            public static MsgShotBeginPacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                Byte shotSlot = packet.ReadByte();
                Vector2 position = packet.ReadVector2();
                Single rotation = packet.ReadSingle();
                Vector2 velocity = packet.ReadVector2();

                return new MsgShotBeginPacket(slot, shotSlot, position, rotation, velocity);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(this.Slot);
                packet.Write(this.ShotSlot);
                packet.Write(this.Position);
                packet.Write(this.Rotation);
                packet.Write(this.Velocity);
            }
        }

        /// <summary>
        /// Sent by the client to end a shot.
        /// Sent by the server to tell other players to end the shot.
        /// </summary>
        public class MsgShotEndPacket : MsgBasePacket
        {
            public override MessageType MsgType
            {
                get { return MessageType.MsgShotEnd; }
            }

            public readonly Byte Slot;
            public readonly Byte ShotSlot;

            /// <summary>
            /// Used to construct a <see cref="MsgShotEndPacket"/> on the client to notify about a new shot.
            /// </summary>
            public MsgShotEndPacket(Byte shotSlot)
                : this(ProtocolInformation.DummySlot, shotSlot)
            { }

            /// <summary>
            /// Used to construct a <see cref="MsgShotEndPacket"/> on the server to inform about a shot.
            /// </summary>
            public MsgShotEndPacket(Byte slot, Byte shotSlot)
            {
                this.Slot = slot;
                this.ShotSlot = shotSlot;
            }

            public static MsgShotEndPacket Read(NetIncomingMessage packet)
            {
                Byte slot = packet.ReadByte();
                Byte shotSlot = packet.ReadByte();

                return new MsgShotEndPacket(slot, shotSlot);
            }

            public void Write(NetOutgoingMessage packet)
            {
                packet.Write(this.Slot);
                packet.Write(this.ShotSlot);
            }
        }
    }
}
