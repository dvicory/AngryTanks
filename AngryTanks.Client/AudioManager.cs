using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

using log4net;

using AngryTanks.Common.Extensions.ContentManagerExtensions;

namespace AngryTanks.Client
{
    public interface IAudioManager
    {
        void Play(String soundName);
        void Play(String soundName, Vector2 cameraPosition, Vector2 objectPosition);
        void playSong(String soundName);
    }

    public class AudioManager : IAudioManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, SoundEffect> soundBank = new Dictionary<string, SoundEffect>();
        private Dictionary<string, Song> musicBank = new Dictionary<string, Song>();

        private AudioEmitter emitter;
        private AudioListener listener;

        public AudioManager(Game game)
        {
            soundBank = game.Content.LoadDirectory<SoundEffect>("audio/bz");
            musicBank = game.Content.LoadDirectory<Song>("audio/music");

            // register ourselves as a service
            if (game.Services != null)
                game.Services.AddService(typeof(IAudioManager), this);

            emitter = new AudioEmitter();
            listener = new AudioListener();

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

        public void Play(String soundName, Vector2 cameraPosition, Vector2 objectPosition)
        {
            listener.Position = new Vector3(cameraPosition.X, cameraPosition.Y, 0);
            emitter.Position = new Vector3(objectPosition.X, objectPosition.Y, 0);

            try
            {
                SoundEffectInstance instance = soundBank[soundName].CreateInstance();
                instance.Apply3D(listener, emitter);
                instance.Play();
            }
            catch (KeyNotFoundException e)
            {
                Log.Warn(e.Message);
                Log.Warn(e.StackTrace);
            }
        }

        public void playSong(String soundName)
        {

            try
            {                
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(musicBank[soundName]);
            }
            catch (KeyNotFoundException e)
            {
                Log.Warn(e.Message);
                Log.Warn(e.StackTrace);
            }
        }

    }
}
