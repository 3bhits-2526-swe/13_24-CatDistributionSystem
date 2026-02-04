using UnityEngine;
using TMPro;
using System.Collections;

public class RetroTextEffect : MonoBehaviour
{
    [Header("Typewriter Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private bool showCursor = true;

    [Header("CRT Physicality")]
    [SerializeField] private float flickerIntensity = 0.05f;
    [SerializeField] private float jitterAmount = 0.5f;
    [SerializeField] private float jitterChance = 0.02f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] typeSounds;
    private AudioSource audioSource;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private TMP_Text textComponent;
    private string fullText;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
        originalPosition = rectTransform.anchoredPosition;

        fullText = textComponent.text;
        textComponent.text = "";
    }

    void Start()
    {
        StartCoroutine(TypeText());
        StartCoroutine(CrtFlicker());
    }

    void Update()
    {
        if (Random.value < jitterChance)
        {
            rectTransform.anchoredPosition = originalPosition + (Random.insideUnitCircle * jitterAmount);
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    IEnumerator TypeText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            string currentText = fullText.Substring(0, i);
            if (showCursor && i < fullText.Length)
                currentText += "█";

            textComponent.text = currentText;

            if (typeSounds.Length > 0 && audioSource != null)
                if (null != audioSource) audioSource.PlayOneShot(typeSounds[Random.Range(0, typeSounds.Length)]);

            yield return new WaitForSeconds(typeSpeed + Random.Range(0, 0.03f));
        }
    }

    IEnumerator CrtFlicker()
    {
        while (true)
        {
            float val = Random.Range(1f - flickerIntensity, 1f);
            Color c = textComponent.color;
            c.a = val;
            textComponent.color = c;

            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        }
    }
}