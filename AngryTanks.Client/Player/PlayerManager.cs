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
                return (Byte)remotePlayers.Count;
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
            {
                remotePlayers.Remove(remotePlayer.Slot);
                remotePlayer.Dispose();
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

                        if (!packet.AddMyself)
                            AddPlayer(packet.Player);
                        else
                            localPlayer = new LocalPlayer(world, packet.Player);

                        break;
                    }

                case MessageType.MsgRemovePlayer:
                    {
                        MsgRemovePlayerPacket packet = (MsgRemovePlayerPacket)message.MessageData;

                        RemovePlayer(packet.Slot);

                        break;
                    }

                default:
                    break;
            }            
        }

        public void AddPlayer(PlayerInformation playerInfo)
        {
            // add player to our list
            remotePlayers[playerInfo.Slot] = new RemotePlayer(world, playerInfo);
        }

        public void RemovePlayer(Byte slot)
        {
            // nuke player from the dictionary
            remotePlayers.Remove(slot);
        }

        public Player GetPlayerBySlot(Byte slot)
        {
            if (LocalPlayer.Slot == slot)
                return LocalPlayer;

            return remotePlayers[slot];
        }
    }
}
