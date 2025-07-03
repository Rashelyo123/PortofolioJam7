
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public float fadeSpeed = 1f;
    public float lifetime = 2f;
    public Vector3 moveDirection = Vector3.up;

    private TextMeshProUGUI textComponent;
    private Color originalColor;
    private float timer = 0f;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            originalColor = textComponent.color;
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move text
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Fade out
        if (textComponent != null)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, timer / lifetime);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }

    public void Initialize(string text, Color color)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = color;
            originalColor = color;
        }
    }

    public static void Create(string text, Vector3 position, Color color)
    {
        GameObject floatingTextPrefab = Resources.Load<GameObject>("FloatingText");
        if (floatingTextPrefab != null)
        {
            GameObject instance = Instantiate(floatingTextPrefab, position, Quaternion.identity);
            FloatingText floatingText = instance.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.Initialize(text, color);
            }
        }
    }
}
