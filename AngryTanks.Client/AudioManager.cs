using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using log4net;

using AngryTanks.Common.Extensions.ContentManagerExtensions;

namespace AngryTanks.Client
{
    public interface IAudioManager
    {
        void Play(String soundName);
    }

    public class AudioManager : IAudioManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, SoundEffect> soundBank = new Dictionary<string, SoundEffect>();

        public AudioManager(Game game)
        {
            soundBank = game.Content.LoadDirectory<SoundEffect>("audio/bz");

            // register ourselves as a service
            if (game.Services != null)
                game.Services.AddService(typeof(IAudioManager), this);
        }

        public void Play(String soundName)
        {
            try
            {
                soundBank[soundName].Play();
            }
            catch (KeyNotFoundException e)
            {
                Log.Warn(e.Message);
                Log.Warn(e.StackTrace);
            }
        }

    }
}
