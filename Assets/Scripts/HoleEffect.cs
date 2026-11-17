using UnityEngine;

public class HoleEffect : MonoBehaviour
{
    public GameObject effectPrefab; 
    public AudioClip holeSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayEffect()
    {
        if (effectPrefab != null)
        {
            GameObject fx = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }

        if (holeSound != null)
            audioSource.PlayOneShot(holeSound);
    }
}