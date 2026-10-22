using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundId
{
    BoomerangThrow,
    BoomerangHit,
    InvalidSelection,
    PlatformEnter,
    PlatformExit,
    CoinCollect
}

public enum BgmId
{
    Rest,
    Run
}

public class AudioManager : MonoBehaviour
{
    const string SFX_DIR = "Audio/SFX";
    const string BGM_DIR = "Audio/BGM";
    const int SFX_POOL_SIZE = 12;
    const float MASTER_VOLUME = 1f;
    const float BGM_VOLUME = 0.4f;
    const float SFX_VOLUME = 1f;
    const float SFX_3D_MIN_DISTANCE = 5f;
    const float SFX_3D_MAX_DISTANCE = 80f;

    struct SoundSettings
    {
        public float Volume;
        public float PitchJitter;
        public bool Is3D;

        public SoundSettings(float volume, float pitchJitter, bool is3D)
        {
            Volume = volume;
            PitchJitter = pitchJitter;
            Is3D = is3D;
        }
    }

    static readonly Dictionary<SoundId, SoundSettings> soundSettings = new Dictionary<SoundId, SoundSettings>
    {
        { SoundId.BoomerangThrow,   new SoundSettings(0.8f, 0.05f, false) },
        { SoundId.BoomerangHit,     new SoundSettings(1.0f, 0.10f, true)  },
        { SoundId.InvalidSelection, new SoundSettings(0.7f, 0.00f, false) },
        { SoundId.PlatformEnter,    new SoundSettings(0.8f, 0.00f, false) },
        { SoundId.PlatformExit,     new SoundSettings(0.8f, 0.00f, false) },
        { SoundId.CoinCollect,      new SoundSettings(0.6f, 0.10f, false) },
    };

    Dictionary<SoundId, AudioClip[]> sfxClips;
    Dictionary<BgmId, AudioClip> bgmClips;
    AudioSource bgmSource;
    List<AudioSource> sfxPool;
    int nextPoolIndex;

    public void Init()
    {
        LoadClips();
        CreateSources();
        Subscribe();
        PlayBgm(BgmId.Rest);
    }

    public void PlaySfx(SoundId id)
    {
        Play(id, null);
    }

    public void PlaySfxAt(SoundId id, Vector3 position)
    {
        Play(id, position);
    }

    public void PlayBgm(BgmId id)
    {
        if (!bgmClips.TryGetValue(id, out AudioClip clip) || bgmSource.clip == clip)
            return;

        bgmSource.clip = clip;
        bgmSource.volume = BGM_VOLUME * MASTER_VOLUME;
        bgmSource.Play();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void LoadClips()
    {
        sfxClips = new Dictionary<SoundId, AudioClip[]>();
        bgmClips = new Dictionary<BgmId, AudioClip>();

        foreach (SoundId id in Enum.GetValues(typeof(SoundId)))
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>($"{SFX_DIR}/{id}");
            if (clips.Length == 0)
                Debug.LogWarning($"[AudioManager] no clip found in Resources/{SFX_DIR}/{id}");
            sfxClips[id] = clips;
        }

        foreach (BgmId id in Enum.GetValues(typeof(BgmId)))
        {
            AudioClip clip = Resources.Load<AudioClip>($"{BGM_DIR}/{id}");
            if (clip == null)
            {
                Debug.LogWarning($"[AudioManager] no clip found at Resources/{BGM_DIR}/{id}");
                continue;
            }
            bgmClips[id] = clip;
        }
    }

    private void CreateSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;

        sfxPool = new List<AudioSource>();
        for (int i = 0; i < SFX_POOL_SIZE; i++)
        {
            GameObject child = new GameObject($"SFX_{i}");
            child.transform.SetParent(transform);

            AudioSource source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = SFX_3D_MIN_DISTANCE;
            source.maxDistance = SFX_3D_MAX_DISTANCE;
            sfxPool.Add(source);
        }
    }

    private void Subscribe()
    {
        GameplayEvents.PlayerAttack += OnPlayerAttack;
        GameplayEvents.BoomerangHit += OnBoomerangHit;
        GameplayEvents.InvalidSelection += OnInvalidSelection;
        GameplayEvents.EnterRestState += OnEnterRestState;
        GameplayEvents.EnterRunState += OnEnterRunState;
        GameplayEvents.ItemCollected += OnItemCollected;
    }

    private void Unsubscribe()
    {
        GameplayEvents.PlayerAttack -= OnPlayerAttack;
        GameplayEvents.BoomerangHit -= OnBoomerangHit;
        GameplayEvents.InvalidSelection -= OnInvalidSelection;
        GameplayEvents.EnterRestState -= OnEnterRestState;
        GameplayEvents.EnterRunState -= OnEnterRunState;
        GameplayEvents.ItemCollected -= OnItemCollected;
    }

    private void OnPlayerAttack(Target target)
    {
        PlaySfx(SoundId.BoomerangThrow);
    }

    private void OnBoomerangHit(Vector3 position)
    {
        PlaySfxAt(SoundId.BoomerangHit, position);
    }

    private void OnInvalidSelection()
    {
        PlaySfx(SoundId.InvalidSelection);
    }

    private void OnEnterRestState()
    {
        PlaySfx(SoundId.PlatformEnter);
        PlayBgm(BgmId.Rest);
    }

    private void OnEnterRunState()
    {
        PlaySfx(SoundId.PlatformExit);
        PlayBgm(BgmId.Run);
    }

    private void OnItemCollected(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Coin:
                PlaySfx(SoundId.CoinCollect);
                break;
        }
    }

    private void Play(SoundId id, Vector3? position)
    {
        AudioClip[] clips = sfxClips[id];
        if (clips.Length == 0)
            return;

        SoundSettings settings = soundSettings[id];
        AudioSource source = GetFreeSource();

        source.transform.position = position ?? transform.position;
        source.spatialBlend = settings.Is3D && position.HasValue ? 1f : 0f;
        source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        source.volume = settings.Volume * SFX_VOLUME * MASTER_VOLUME;
        source.pitch = 1f + UnityEngine.Random.Range(-settings.PitchJitter, settings.PitchJitter);
        source.Play();
    }

    private AudioSource GetFreeSource()
    {
        foreach (AudioSource source in sfxPool)
        {
            if (!source.isPlaying)
                return source;
        }

        AudioSource oldest = sfxPool[nextPoolIndex];
        nextPoolIndex = (nextPoolIndex + 1) % sfxPool.Count;
        return oldest;
    }
}
