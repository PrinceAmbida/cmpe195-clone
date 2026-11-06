using UnityEngine;

public static class LayerBackgroundMusic
{
    public const string TransportTrack = "LayerMusic/TransportLayer/tecnological_messup_v2";
    public const string NetworkTrack = "LayerMusic/NetworkLayer/Searching";
    private const float DefaultVolume = 0.15f;

    public static void Play(Transform owner, string resourcePath)
    {
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip == null)
        {
            Debug.LogWarning("Background music was not found: " + resourcePath, owner);
            return;
        }

        Transform musicObject = owner.Find("BackgroundMusic");
        if (musicObject == null)
        {
            musicObject = new GameObject("BackgroundMusic").transform;
            musicObject.SetParent(owner, false);
        }

        AudioSource source = musicObject.GetComponent<AudioSource>();
        if (source == null)
        {
            source = musicObject.gameObject.AddComponent<AudioSource>();
        }

        source.playOnAwake = false;
        source.loop = true;
        source.volume = DefaultVolume;
        source.spatialBlend = 0f;
        source.dopplerLevel = 0f;

        if (source.clip != clip || !source.isPlaying)
        {
            source.clip = clip;
            source.Play();
        }
    }
}
