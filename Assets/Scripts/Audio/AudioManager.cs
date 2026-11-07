using UnityEngine;

// Plays the game's music and sound effects in response to gameplay events.
// Lives on a root object in the Game scene (like Timer / ScoreSystem).
// Music starts when gameplay begins and stops on win or lose.
public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("Untick to turn off only the background music; sound effects still play.")]
    [SerializeField]
    private bool musicEnabled = true;

    [SerializeField]
    private AudioClip musicLoop;

    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 0.6f;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip hitClip;

    [SerializeField]
    private AudioClip hurtClip;

    [SerializeField]
    private AudioClip pickupClip;

    [SerializeField]
    private AudioClip winClip;

    [SerializeField]
    private AudioClip loseClip;

    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 0.8f;

    [Tooltip("Hit sound has its own volume so it always cuts through the music.")]
    [SerializeField, Range(0f, 1f)]
    private float hitVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource hitSource; // own source: other sounds can't change its pitch or steal its voice
    private int lastHitFrame = -1;

    private void Awake()
    {
        musicSource = CreateSource(loop: true, musicVolume);
        musicSource.clip = musicLoop;
        musicSource.mute = !musicEnabled;

        sfxSource = CreateSource(loop: false, sfxVolume);

        hitSource = CreateSource(loop: false, hitVolume);
        hitSource.priority = 0; // highest: never dropped when many sounds play

        SequenceEvents.LevelStarted += OnLevelStarted;
        SequenceEvents.Win += OnWin;
        SequenceEvents.Timeout += OnLose;
        GameplayEvents.TargetHit += OnTargetHit;
        GameplayEvents.PlayerHurt += OnPlayerHurt;
        GameplayEvents.ItemCollected += OnItemCollected;
    }

    private void OnDestroy()
    {
        SequenceEvents.LevelStarted -= OnLevelStarted;
        SequenceEvents.Win -= OnWin;
        SequenceEvents.Timeout -= OnLose;
        GameplayEvents.TargetHit -= OnTargetHit;
        GameplayEvents.PlayerHurt -= OnPlayerHurt;
        GameplayEvents.ItemCollected -= OnItemCollected;
    }

    // lets the checkbox / volume sliders take effect live while in Play mode
    private void OnValidate()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicEnabled;
            musicSource.volume = musicVolume;
        }
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
        if (hitSource != null)
            hitSource.volume = hitVolume;
    }

    // turn background music on/off from code (e.g. a settings menu); sound effects are unaffected
    public void SetMusicEnabled(bool isEnabled)
    {
        musicEnabled = isEnabled;
        musicSource.mute = !isEnabled;
    }

    // gameplay begins at level 1; the scene reloads on restart, so this runs once per game
    private void OnLevelStarted(int level)
    {
        if (level == 1 && musicLoop != null)
            musicSource.Play();
    }

    private void OnWin()
    {
        musicSource.Stop();
        Play(winClip);
    }

    private void OnLose()
    {
        musicSource.Stop();
        Play(loseClip);
    }

    // every attack that lands on a box, lever or monster plays one hit sound
    private void OnTargetHit()
    {
        if (hitClip == null) return;

        // one swing can hit several targets in the same frame; stacking identical
        // copies only distorts, so play once per frame
        if (lastHitFrame == Time.frameCount) return;
        lastHitFrame = Time.frameCount;

        // small random pitch so repeated hits don't sound identical
        hitSource.pitch = Random.Range(0.9f, 1.1f);
        hitSource.PlayOneShot(hitClip);
    }

    private void OnPlayerHurt() => Play(hurtClip);

    private void OnItemCollected(CollectibleType type) => Play(pickupClip);

    private void Play(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }

    // 2D source: same volume wherever the player is
    private AudioSource CreateSource(bool loop, float volume)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.volume = volume;
        source.spatialBlend = 0f;
        return source;
    }
}
