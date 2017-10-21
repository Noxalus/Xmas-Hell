﻿using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace XmasHell.Sound
{
    public static class SoundManager
    {
        public static void PlaySound(SoundEffect sound)
        {
            sound.Play();
        }

        public static void PlayRandomSound(List<SoundEffect> sounds)
        {
            sounds[XmasHell.Instance().GameManager.Random.Next(0, sounds.Count - 1)].Play();
        }
    }
}