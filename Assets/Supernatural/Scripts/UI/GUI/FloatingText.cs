using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 2f;
    public float Speed = 10f;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, DestroyTime);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up * 10f, Speed * Time.deltaTime);
    }

    public void SetText(string text, Color color)
    {
        _text.color = color;
        _text.text = text;
    }
}