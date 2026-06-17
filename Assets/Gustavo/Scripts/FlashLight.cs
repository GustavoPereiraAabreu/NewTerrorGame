using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public Light lanternaLight;

    [Header("Áudio (sem AudioSource)")]
    public AudioClip toggleOnClip;
    public AudioClip toggleOffClip;
    [Range(0f, 1f)]
    public float volume = 100f;
    public KeyCode toggleKey = KeyCode.Mouse0;

    void Start()
    {
        if (lanternaLight != null)
            lanternaLight.enabled = false;

    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (lanternaLight != null)
            {
                bool isOn = !lanternaLight.enabled;
                lanternaLight.enabled = isOn;
                PlayToggleSound(isOn);
            }
        }
    }

    void PlayToggleSound(bool isOn)
    {
        AudioClip clip = isOn ? toggleOnClip : toggleOffClip;
        if (clip == null) return;


        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}