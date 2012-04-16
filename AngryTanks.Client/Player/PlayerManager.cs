using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public class PlayerManager : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region PlayerManager Properties

        public List<RemotePlayer> RemotePlayers
        {
            get { return remotePlayers.Values.ToList(); }
        }

        private LocalPlayer localPlayer;

        public LocalPlayer LocalPlayer
        {
            get { return localPlayer; }
        }

        public Byte PlayerCount
        {
            get
            {
                Byte count = (Byte)remotePlayers.Count;

                if (localPlayer != null)
                    count++;

                return count;
            }
        }

        #endregion

        private World world;

        private Dictionary<Byte, RemotePlayer> remotePlayers = new Dictionary<Byte, RemotePlayer>();

        public PlayerManager(World world)
        {
            this.world = world;

            this.world.ServerLink.MessageReceivedEvent += HandleReceivedMessage;
        }

        ~PlayerManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            world.ServerLink.MessageReceivedEvent -= HandleReceivedMessage;

            foreach (RemotePlayer remotePlayer in remotePlayers.Values)
                remotePlayer.Dispose();

            remotePlayers.Clear();

            if (localPlayer != null)
            {
                localPlayer.Dispose();
                localPlayer = null;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (localPlayer != null)
                localPlayer.Update(gameTime);

            foreach (RemotePlayer remotePlayer in remotePlayers.Values)
                remotePlayer.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (localPlayer != null)
                localPlayer.Draw(gameTime, spriteBatch);

            foreach (RemotePlayer remotePlayer in remotePlayers.Values)
                remotePlayer.Draw(gameTime, spriteBatch);
        }

        private void HandleReceivedMessage(object sender, ServerLinkMessageEvent message)
        {
            switch (message.MessageType)
            {
                case MessageType.MsgAddPlayer:
                    {
                        MsgAddPlayerPacket packet = (MsgAddPlayerPacket)message.MessageData;

                        if (message.ServerLinkStatus == NetServerLinkStatus.Connected)
                            world.Console.WriteLine(String.Format("{0} has joined the {1}",
                                                  packet.Player.Callsign, packet.Player.Team));
                        else if (message.ServerLinkStatus == NetServerLinkStatus.GettingState)
                            world.Console.WriteLine(String.Format("{0} is on the {1}",
                                                  packet.Player.Callsign, packet.Player.Team));

                        if (!packet.AddMyself)
                            AddPlayer(packet.Player);
                        else
                            localPlayer = new LocalPlayer(world, packet.Player);

                        break;
                    }

                case MessageType.MsgRemovePlayer:
                    {
                        MsgRemovePlayerPacket packet = (MsgRemovePlayerPacket)message.MessageData;

                        world.Console.WriteLine(String.Format("{0} has left the server ({1})", GetPlayerBySlot(packet.Slot).Callsign, packet.Reason));

                        RemovePlayer(packet.Slot);

                        break;
                    }

                default:
                    break;
            }            
        }

        /// <summary>
        /// Adds a new remote player.
        /// </summary>
        /// <param name="playerInfo"></param>
        public void AddPlayer(PlayerInformation playerInfo)
        {
            // add player to our list
            remotePlayers[playerInfo.Slot] = new RemotePlayer(world, playerInfo);
        }

        /// <summary>
        /// Removes and disposes a remote player.
        /// </summary>
        /// <param name="slot"></param>
        public void RemovePlayer(Byte slot)
        {
            RemotePlayer remotePlayer = remotePlayers[slot];

            // nuke player from the dictionary
            remotePlayers.Remove(slot);

            // dispose of remote player
            remotePlayer.Dispose();
        }

        public Player GetPlayerBySlot(Byte slot)
        {
            if (LocalPlayer.Slot == slot)
                return LocalPlayer;

            return remotePlayers[slot];
        }
    }
}
