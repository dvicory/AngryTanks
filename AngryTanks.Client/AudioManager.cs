using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace AngryTanks.Client
{
    class AudioManager
    {
        ContentManager content;
        private static SoundEffect boom;
        private static SoundEffect explosion;
        private static SoundEffect fire;
        private static SoundEffect flag_alert;
        private static SoundEffect flag_drop;
        private static SoundEffect flag_grab;
        private static SoundEffect flag_lost;
        private static SoundEffect flag_won;
        private static SoundEffect killteam;
        private static SoundEffect laser;
        private static SoundEffect shock;
        private static SoundEffect steamroller;
        private static SoundEffect teamgrab;

        public AudioManager(World world)
        {
            content = world.Content;
            LoadContent();
        }

        private void LoadContent()
        {
            boom = content.Load<SoundEffect>("audio/bz/boom");
            explosion = content.Load<SoundEffect>("audio/bz/explosion");
            fire = content.Load<SoundEffect>("audio/bz/fire");
            flag_alert = content.Load<SoundEffect>("audio/bz/flag_alert");
            flag_drop = content.Load<SoundEffect>("audio/bz/flag_drop");
            flag_grab = content.Load<SoundEffect>("audio/bz/flag_grab");
            flag_lost = content.Load<SoundEffect>("audio/bz/flag_lost");
            flag_won = content.Load<SoundEffect>("audio/bz/flag_won");
            killteam = content.Load<SoundEffect>("audio/bz/killteam");
            laser = content.Load<SoundEffect>("audio/bz/laser");
            shock = content.Load<SoundEffect>("audio/bz/shock");
            steamroller = content.Load<SoundEffect>("audio/bz/steamroller");
            teamgrab = content.Load<SoundEffect>("audio/bz/teamgrab");
        }

        public static void playBoom()
        {
            boom.Play();
        }
        public static void playExplosion()
        {
            explosion.Play();
        }
        public static void playfire()
        {
            fire.Play();
        }
        public static void playFlagAlert()
        {
            flag_alert.Play();
        }
        public static void playFlagDrop()
        {
            flag_drop.Play();
        }
        public static void playFlagGrab()
        {
            flag_grab.Play();
        }
        public static void playFlagLost()
        {
            flag_lost.Play();
        }
        public static void playKillTeam()
        {
            killteam.Play();
        }
        public static void playLaser()
        {
            laser.Play();
        }
        public static void playShock()
        {
            shock.Play();
        }
        public static void playSteamRoller()
        {
            steamroller.Play();
        }
        public static void playTeamGrab()
        {
            teamgrab.Play();
        }
    }
}
