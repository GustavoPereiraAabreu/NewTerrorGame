using UnityEngine;
using TMPro;

public class RecordingTimer : MonoBehaviour
{
    public TMP_Text timerText;

    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        int horas = Mathf.FloorToInt(elapsedTime / 3600);
        int minutos = Mathf.FloorToInt((elapsedTime % 3600) / 60);

        timerText.text = $"{horas:00}:{minutos:00}";
    }
}