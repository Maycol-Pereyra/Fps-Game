using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    private AudioClip gunshotClip;
    private AudioClip impactClip;
    private AudioClip explosionClip;
    private AudioClip emptyGunClip;
    private AudioClip targetHitClip;

    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        gunshotClip = Resources.Load<AudioClip>("Audio/Gunshot");
        impactClip = Resources.Load<AudioClip>("Audio/Impact");
        explosionClip = Resources.Load<AudioClip>("Audio/Explosion");
        emptyGunClip = Resources.Load<AudioClip>("Audio/EmptyGun");
        targetHitClip = Resources.Load<AudioClip>("Audio/TargetHit");
    }

    public void PlayGunshot(Vector3 position)
    {
        if (gunshotClip != null)
            AudioSource.PlayClipAtPoint(gunshotClip, position, 0.7f);
    }

    public void PlayImpact(Vector3 position)
    {
        if (impactClip != null)
            AudioSource.PlayClipAtPoint(impactClip, position, 0.5f);
    }

    public void PlayExplosion(Vector3 position)
    {
        if (explosionClip != null)
            AudioSource.PlayClipAtPoint(explosionClip, position, 1f);
    }

    public void PlayEmptyGun()
    {
        if (emptyGunClip != null)
        {
            sfxSource.PlayOneShot(emptyGunClip, 0.6f);
        }
    }

    public void PlayTargetHit(Vector3 position)
    {
        if (targetHitClip != null)
            AudioSource.PlayClipAtPoint(targetHitClip, position, 0.6f);
    }
}
