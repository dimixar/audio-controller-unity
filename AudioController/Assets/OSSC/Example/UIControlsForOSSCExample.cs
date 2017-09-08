using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSSC;

public class UIControlsForOSSCExample : MonoBehaviour
{
    public SoundController soundController;

    ISoundCue coins;
    ISoundCue whosh;
    ISoundCue crackingIce;
    ISoundCue sfxCue;
    ISoundCue sfxCueLoop;
    ISoundCue music;
    ISoundCue musicWithTag;

    public void PlayCoins()
    {
        //You should send this struct to soundController in order to play something.
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();

        settings.name = "Coins";
        //SoundController returns a SoundCueProxy, that can be reused or subscribe to one of it's events.
        coins = soundController.Play(settings);
    }

    public void PlayWhosh()
    {
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();

        settings.name = "Whosh";
        whosh = soundController.Play(settings);
        whosh.OnPlayCueEnded += (Cue) =>
        {
            settings = new PlaySoundSettings();
            settings.Init();
            //It is a good practice to reuse the cue you already played.
            //Because soundCueProxy caches all the data and SoundController
            //will not have to go through finding it again.
            settings.soundCueProxy = whosh;
            soundController.Play(settings);
        };
    }

    public void PlayCrackingIce()
    {
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();

        settings.name = "CrackingIce";
        crackingIce = soundController.Play(settings);
    }

    public void PlaySFXCue()
    {
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();

        settings.names = new[] {"Coins", "Whosh", "CrackingIce"};
        sfxCue = soundController.Play(settings);
    }

    public void PlaySFXCueLooped()
    {
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();

        settings.names = new[] {"Coins", "Whosh", "CrackingIce"};
        settings.isLooped = true;
        sfxCueLoop = soundController.Play(settings);

        //You can also check when any cue finished playing
        sfxCueLoop.OnPlayCueEnded += (cue) =>
        {
            Debug.LogFormat("Cue with ID {0} finished playing", cue.ID);
        };

        sfxCueLoop.OnPlayEnded += (name) => Debug.LogFormat("Sound \"{0}\" finished playing", name);
    }

    public void PlayMusic()
    {
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();

        settings.name = "Ballade";
        settings.fadeInTime = 1f;
        settings.fadeOutTime = 2f;
        settings.categoryName = "Music"; // search only in that category
        settings.parent = transform; // Use this parent to put the AudioSource's position here.
        settings.isLooped = true;
        music = soundController.Play(settings);
    }

    public void PlayMusicWithTag()
    {
        PlaySoundSettings settings = new PlaySoundSettings();
        settings.Init();
        
        settings.name = "Ballade";
        settings.tagName = "classical";
        settings.isLooped = true;
        musicWithTag = soundController.Play(settings);
    }

    public void StopCoins()
    {
        if (coins != null)
            coins.Stop();
    }

    public void StopWhosh()
    {
        if (whosh != null)
            whosh.Stop();
    }

    public void StopCrackingIce()
    {
        if (crackingIce != null)
            crackingIce.Stop();
    }

    public void StopSFXCue()
    {
        if (sfxCue != null)
            sfxCue.Stop();
    }

    public void StopSFXCueLooped()
    {
        if (sfxCueLoop != null)
            sfxCueLoop.Stop();
    }

    public void StopMusic()
    {
        if (music != null)
            music.Stop();
    }

    public void StopAll()
    {
        //Stop all playing cues by calling this method;
        soundController.StopAll();
    }
}
