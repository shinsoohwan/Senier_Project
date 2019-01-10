using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SoundManager
{
    public class BGMSetInfo : DatatableManager.DatatableBaseInfo
    {
        public class _BGMInfo
        {
            public string FileName { get; set; }
            public double StartTime { get; set; }
            public string Dump()
            {
                return "[" + FileName + ", " + StartTime + "]";
            }
        }

        public List<_BGMInfo> BGMInfo { get; set; }
        
        public override string Dump()
        {
            string result = "ClassID:" + ClassID + ", ClassName:" + ClassName + ", FileNames: [";
            foreach (_BGMInfo bgmInfo in BGMInfo)
            {
                result += bgmInfo.Dump() + ", ";
            }
            result += "]";

            return result;
        }
    }

    public class SEInfo : DatatableManager.DatatableBaseInfo
    {
        public List<string> FileNames { get; set; }

        public override string Dump()
        {
            string result = "ClassID:" + ClassID + ", ClassName:" + ClassName + ", FileNames: [";
            foreach (string filename in FileNames)
            {
                result += filename + ", ";
            }
            result += "]";

            return result;
        }
    }

    public static SoundManager instance;
    public Dictionary<string, BGMSetInfo> datatableBGM;

    Dictionary<string, AudioClip[]> m_soundEffects = new Dictionary<string, AudioClip[]>();

    AudioClip m_bgm;

    AudioSource audioSource;

    public static void InitSoundManager(AudioSource audioSource)
    {
        if (SoundManager.instance != null)
            Debug.Log("SoundManager already initialized");

        SoundManager.instance = new SoundManager(audioSource);
    }

    SoundManager(AudioSource audioSource)
    {
        if (GameManager.instance == null)
            Debug.Log("Initialize GameManager First.");

        if (DatatableManager.instance == null)
        {
            DatatableManager.InitDatatableManager();
        }

        this.audioSource = audioSource;

        {
            List<SEInfo> soundeffects = DatatableManager.instance.LoadJson<List<SEInfo>>("Datatables/datatable_soundeffect");

            foreach (SEInfo soundeffect in soundeffects)
            {
                var filenames = soundeffect.FileNames;
                AudioClip[] clips = new AudioClip[filenames.Count];
                for (int i = 0; i < filenames.Count; i++)
                {
                    clips[i] = Resources.Load<AudioClip>(filenames[i]);
                }

                m_soundEffects.Add(soundeffect.ClassName, clips);
            }
        }

        datatableBGM = DatatableManager.instance.LoadDatatableByClassName<BGMSetInfo>("Datatables/datatable_bgm");
    }

    private AudioClip ReadMP3(string fileName)
    {
        return Resources.Load<AudioClip>(fileName);
    }

    public void PlayMusic(string name)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        List<BGMSetInfo._BGMInfo> list = datatableBGM[name].BGMInfo;

        audioSource.clip = ReadMP3(list[UnityEngine.Random.Range(0, list.Count)].FileName);
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlaySound(string name)
    {
        AudioClip[] clipset = m_soundEffects[name];

        audioSource.PlayOneShot(clipset[UnityEngine.Random.Range(0, clipset.Length)]);
    }
}
