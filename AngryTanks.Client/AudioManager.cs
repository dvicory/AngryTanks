using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace AngryTanks.Client
{
    public interface IAudioManager
    {
        #region Methods
        void play(String soundname);
        #endregion
    }
    public class AudioManager : IAudioManager
    {        
        private Dictionary<string, SoundEffect> soundBank = new Dictionary<string, SoundEffect>();
        //maybe a musicBank later?

        public AudioManager(Game game)
        {
            //Load directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo(game.Content.RootDirectory + "\\" + "audio" + "\\" + "bz");
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                if (key.CompareTo("LICENSE") != 0)
                {                    
                    soundBank[key] = game.Content.Load<SoundEffect>("audio" + "/" + "bz" + "/" + key);
                }
            }

            // register ourselves as a service
            if (game.Services != null)
                game.Services.AddService(typeof(IAudioManager), this);
        }

        public void play(string soundName)
        {
            try
            {
                soundBank[soundName].Play();
            }
            catch (KeyNotFoundException e)
            {
                System.Diagnostics.Debug.Write(e.Message);
                System.Diagnostics.Debug.Write(e.StackTrace);
            }
            
        }

    }
}
