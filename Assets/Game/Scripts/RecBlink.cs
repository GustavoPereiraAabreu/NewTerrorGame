using UnityEngine;
using UnityEngine.UI;

public class RecBlink : MonoBehaviour
{
    public float blinkInterval = 0.5f;

    private SpriteRenderer recDot;
    private float timer;

    void Start()
    {
        recDot = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= blinkInterval)
        {
            recDot.enabled = !recDot.enabled;
            timer = 0f;
        }
    }
}