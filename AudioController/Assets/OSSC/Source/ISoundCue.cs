using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    public interface ISoundCue
    {
        Action<string> OnPlayEnded { get; set; }
        Action<SoundCue> OnPlayCueEnded { get; set; }
        SoundObject AudioObject { get; set; }
        SoundCueData Data { get; }
        bool IsPlaying { get; }
        int ID { get; }

        void Play(SoundCueData data);
        void Pause();
        void Resume();
        void Stop(bool shouldCallOnFinishedCue = true);
    }
}
