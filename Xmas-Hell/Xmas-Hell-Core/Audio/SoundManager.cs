using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using XmasHell.PlayerData;

namespace XmasHell.Audio
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundEffectInstance> _soundEffectInstances = new Dictionary<string, SoundEffectInstance>();

        public static void PlaySound(SoundEffect sound)
        {
            SoundEffectInstance instance;

            if (_soundEffectInstances.ContainsKey(sound.Name))
                instance = _soundEffectInstances[sound.Name];
            else
            {
                instance = sound.CreateInstance();
                instance.Volume = PlayerSettings.MasterVolume * PlayerSettings.SoundVolume;

                _soundEffectInstances.Add(sound.Name, instance);
            }

            instance.Play();
        }

        public static void PlayRandomSound(List<SoundEffect> sounds)
        {
            sounds[XmasHell.Instance().GameManager.Random.Next(0, sounds.Count - 1)].Play();
        }
    }
}
