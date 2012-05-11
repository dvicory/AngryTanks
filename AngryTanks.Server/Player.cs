using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using log4net;
using Lidgren.Network;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Server
{
    public class Player
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Player Properties

        private PlayerState state;
        public PlayerState State
        {
            get { return state; }
        }

        public readonly Byte          Slot;
        public readonly NetConnection Connection;
        public readonly String        Callsign, Tag;
        public readonly TeamType      Team;

        public PlayerInformation PlayerInfo
        {
            get
            {
                return new PlayerInformation(Slot, Callsign, Tag, Team);
            }
        }

        private Score score;
        public Score Score
        {
            get { return score; }
        }

        #endregion

        private GameKeeper gameKeeper;

        // when they last died so we know when to spawn him
        private DateTime lastDiedTime = DateTime.MinValue;

        // 5 second respawn
        private TimeSpan respawnTime = new TimeSpan(0, 0, 5);

        public Player(GameKeeper gameKeeper, Byte slot, NetConnection connection, PlayerInformation playerInfo)
        {
            this.Slot       = slot;
            this.Connection = connection;
            this.Team       = playerInfo.Team;
            this.Callsign   = playerInfo.Callsign;
            this.Tag        = playerInfo.Tag;

            this.gameKeeper = gameKeeper;

            this.state = PlayerState.Joining;
            this.score = new Score();

            Log.InfoFormat("Player #{0} \"{1}\" <{2}> created and joined to {3}", Slot, Callsign, Tag, Team);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUpdate"></param>
        public void Update(DateTime lastUpdate)
        {
            if ((State == PlayerState.Dead) && (lastDiedTime + respawnTime <= DateTime.Now))
                Spawn();

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incomingMsg"></param>
        public void HandleData(NetIncomingMessage incomingMsg)
        {
            MessageType messageType = (MessageType)incomingMsg.ReadByte();

            switch (messageType)
            {
                case MessageType.MsgState:
                    SendState();
                    break;

                case MessageType.MsgPlayerClientUpdate:
                    BroadcastUpdate(incomingMsg);
                    break;

                case MessageType.MsgDeath:
                    Die(incomingMsg);
                    break;

                case MessageType.MsgBeginShot:
                    Shoot(incomingMsg);
                    break;

                case MessageType.MsgEndShot:
                    EndShot(incomingMsg);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sends initial state to this <see cref="Player"/>.
        /// </summary>
        private void SendState()
        {
            Log.DebugFormat("Sending state to #{0}...", Slot);

            // TODO we should clamp world size to no more than UInt16.MaxValue bytes large
            // first send the world
            NetOutgoingMessage worldMsg = gameKeeper.Server.CreateMessage(1 + 2 + (UInt16)gameKeeper.RawWorld.Length);
            worldMsg.Write((Byte)MessageType.MsgWorld);
            worldMsg.Write((UInt16)gameKeeper.RawWorld.Length);
            worldMsg.Write(gameKeeper.RawWorld);
            SendMessage(worldMsg, NetDeliveryMethod.ReliableOrdered, 0);

            // TODO send other state information... like flags

            // tell him about everyone else
            Log.DebugFormat("Sending MsgAddPlayer for each player to #{0}", Slot);

            NetOutgoingMessage addPlayerMessage;
            MsgAddPlayerPacket addPlayerPacket;

            foreach (Player otherPlayer in gameKeeper.Players)
            {
                // don't want to tell our player about himself just yet...
                if (otherPlayer == this)
                    continue;

                addPlayerMessage = gameKeeper.Server.CreateMessage();

                addPlayerPacket = new MsgAddPlayerPacket(otherPlayer.PlayerInfo, false);

                addPlayerMessage.Write((Byte)addPlayerPacket.MsgType);
                addPlayerPacket.Write(addPlayerMessage);

                Log.DebugFormat("MsgAddPlayer Compiled ({0} bytes) for player #{1} and being sent to player #{2}",
                                addPlayerMessage.LengthBytes, otherPlayer.Slot, this.Slot);

                SendMessage(addPlayerMessage, NetDeliveryMethod.ReliableOrdered, 0);
            }

            // send everyones' scores to him
            NetOutgoingMessage scoreMessage;

            foreach (Player otherPlayer in gameKeeper.Players)
            {
                // no reason to give our player his score... he has none yet
                if (otherPlayer == this)
                    continue;

                scoreMessage = otherPlayer.GetMsgScore();

                SendMessage(scoreMessage, NetDeliveryMethod.ReliableOrdered, 0);
            }

            // send back one last MsgAddPlayer with their full information (which could be changed!)
            addPlayerMessage = gameKeeper.Server.CreateMessage();

            addPlayerPacket = new MsgAddPlayerPacket(PlayerInfo, true);

            addPlayerMessage.Write((Byte)addPlayerPacket.MsgType);
            addPlayerPacket.Write(addPlayerMessage);

            SendMessage(addPlayerMessage, NetDeliveryMethod.ReliableOrdered, 0);

            // let them know we're ready to move on, and give him his slot
            NetOutgoingMessage stateMessage = gameKeeper.Server.CreateMessage();

            MsgStatePacket statePacket = new MsgStatePacket(Slot);

            stateMessage.Write((Byte)statePacket.MsgType);
            statePacket.Write(stateMessage);

            SendMessage(stateMessage, NetDeliveryMethod.ReliableOrdered, 0);

            // we're now ready to move to the spawn state and spawn
            this.state = PlayerState.Spawning;

            this.Spawn();
        }

        /// <summary>
        /// Broadcasts a <see cref="MsgPlayerServerUpdatePacket"/> to all other
        /// <see cref="Player"/>s after receiving a <see cref="MsgPlayerClientUpdatePacket"/>.
        /// </summary>
        /// <param name="msg"></param>
        private void BroadcastUpdate(NetIncomingMessage msg)
        {
            MsgPlayerClientUpdatePacket clientUpdatePacket = MsgPlayerClientUpdatePacket.Read(msg);

            NetOutgoingMessage serverUpdateMessage = gameKeeper.Server.CreateMessage();
            MsgPlayerServerUpdatePacket serverUpdatePacket = new MsgPlayerServerUpdatePacket(this.Slot, clientUpdatePacket);

            serverUpdateMessage.Write((Byte)serverUpdatePacket.MsgType);
            serverUpdatePacket.Write(serverUpdateMessage);

            // send to everyone but us
            gameKeeper.Server.SendToAll(serverUpdateMessage, this.Connection, NetDeliveryMethod.UnreliableSequenced, 0);
        }

        /// <summary>
        /// Spawns the <see cref="Player"/> and notifies all other <see cref="Player"/>s about it.
        /// </summary>
        public void Spawn()
        {
            // create our spawn message and packet
            NetOutgoingMessage spawnMessage = gameKeeper.Server.CreateMessage();

            MsgSpawnPacket spawnPacket = new MsgSpawnPacket(this.Slot, Vector2.Zero, 0);

            // write to the message
            spawnMessage.Write((Byte)spawnPacket.MsgType);
            spawnPacket.Write(spawnMessage);

            // send the spawn message to everyone
            gameKeeper.Server.SendToAll(spawnMessage, null, NetDeliveryMethod.ReliableOrdered, 0);

            // they're now alive as far as we're concerned
            state = PlayerState.Alive;
        }

        /// <summary>
        /// Handles death reports by players and broadcasts that to all other <see cref="Player"/>s.
        /// </summary>
        /// <param name="incomingMessage"></param>
        public void Die(NetIncomingMessage incomingMessage)
        {
            MsgDeathPacket incomingDeathPacket = MsgDeathPacket.Read(incomingMessage);

            // create our death message and packet
            NetOutgoingMessage deathMessage = gameKeeper.Server.CreateMessage();

            MsgDeathPacket deathPacket = new MsgDeathPacket(this.Slot, incomingDeathPacket.Killer);

            // write to the message
            deathMessage.Write((Byte)deathPacket.MsgType);
            deathPacket.Write(deathMessage);

            // send the death message to everyone except the player who reported it
            gameKeeper.Server.SendToAll(deathMessage, this.Connection, NetDeliveryMethod.ReliableOrdered, 0);

            // update our score
            this.Score.Losses++;

            // update killer's score, but only if the killer wasn't myself
            if (this.Slot != incomingDeathPacket.Killer)
            {
                Player killer = gameKeeper.GetPlayerBySlot(incomingDeathPacket.Killer);

                if (killer != null)
                {
                    killer.Score.Wins++;
                    gameKeeper.Server.SendToAll(killer.GetMsgScore(), null, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }

            // broadcast our score
            gameKeeper.Server.SendToAll(this.GetMsgScore(), null, NetDeliveryMethod.ReliableOrdered, 0);

            // update our last died time
            lastDiedTime = DateTime.Now;

            // we're now dead as far as we're concerned
            state = PlayerState.Dead;
        }

        /// <summary>
        /// Handles new shots by players and broadcasts that to all other <see cref="Player"/>s.
        /// </summary>
        /// <param name="incomingMessage"></param>
        public void Shoot(NetIncomingMessage incomingMessage)
        {
            MsgBeginShotPacket incomingBeginShotPacket = MsgBeginShotPacket.Read(incomingMessage);

            // create our shot begin message and packet
            NetOutgoingMessage beginShotMessage = gameKeeper.Server.CreateMessage();

            MsgBeginShotPacket beginShotPacket =
                new MsgBeginShotPacket(this.Slot,
                                       incomingBeginShotPacket.ShotSlot,
                                       incomingBeginShotPacket.Position,
                                       incomingBeginShotPacket.Rotation,
                                       incomingBeginShotPacket.Velocity);

            // write to the message
            beginShotMessage.Write((Byte)beginShotPacket.MsgType);
            beginShotPacket.Write(beginShotMessage);

            // send the shot begin message to everyone except the player who reported it
            gameKeeper.Server.SendToAll(beginShotMessage, this.Connection, NetDeliveryMethod.ReliableUnordered, 0);
        }

        /// <summary>
        /// Handles end shots by players and broadcasts that to all other <see cref="Player"/>s.
        /// </summary>
        /// <param name="incomingMessage"></param>
        public void EndShot(NetIncomingMessage incomingMessage)
        {
            MsgEndShotPacket incomingShotEndPacket = MsgEndShotPacket.Read(incomingMessage);

            // create our shot end message and packet
            NetOutgoingMessage shotEndMessage = gameKeeper.Server.CreateMessage();

            MsgEndShotPacket shotEndPacket = new MsgEndShotPacket(incomingShotEndPacket.Slot, incomingShotEndPacket.ShotSlot, incomingShotEndPacket.Explode);

            // write to the message
            shotEndMessage.Write((Byte)shotEndPacket.MsgType);
            shotEndPacket.Write(shotEndMessage);

            // send the shot end message to everyone except the player who reported it
            gameKeeper.Server.SendToAll(shotEndMessage, this.Connection, NetDeliveryMethod.ReliableUnordered, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public NetOutgoingMessage GetMsgScore()
        {
            NetOutgoingMessage scoreMessage = gameKeeper.Server.CreateMessage();

            MsgScorePacket scorePacket = new MsgScorePacket(this.Slot, this.Score);

            scoreMessage.Write((Byte)scorePacket.MsgType);
            scorePacket.Write(scoreMessage);

            return scoreMessage;
        }

        #region Connection Helpers

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            return gameKeeper.Server.SendMessage(msg, Connection, method);
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
        {
            return gameKeeper.Server.SendMessage(msg, Connection, method, sequenceChannel);
        }

        #endregion
    }
}
