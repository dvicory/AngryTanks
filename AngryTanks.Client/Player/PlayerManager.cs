using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Messages;
using AngryTanks.Common.Protocol;

namespace AngryTanks.Client
{
    public class PlayerManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region PlayerManager Properties

        public List<Player> RemotePlayers
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

        private Dictionary<Byte, Player> remotePlayers = new Dictionary<Byte, Player>();

        public PlayerManager(World world)
        {
            this.world = world;
        }

        public void HandleIncomingData(MsgBasePacket packet)
        {
        }

        public void AddPlayer(PlayerInformation playerInfo)
        {
            // add player to our list
            remotePlayers[playerInfo.Slot] = new RemotePlayer(world, playerInfo);
        }

        public void RemovePlayer(Player player, String reason)
        {
            Log.InfoFormat("Removing player #{0} ({1})", player.Callsign, reason);

            world.Console.WriteLine(String.Format("{0} has left the game ({1})", player.Callsign, reason));

            // nuke player from the dictionary
            remotePlayers.Remove(player.Slot);
        }

        public Player GetPlayerBySlot(Byte slot)
        {
            if (LocalPlayer.Slot == slot)
                return LocalPlayer;

            return remotePlayers[slot];
        }
    }
}
