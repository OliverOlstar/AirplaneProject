using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHUD : MonoBehaviour
{
    [SerializeField] private int textCount = 5;
    [SerializeField] private float textDisplayLength = 4.0f;

    [SerializeField] private GameObject textPrefab;

    private Text[] texts = new Text[5];
    private List<RectTransform> visibleTexts = new List<RectTransform>();

    [Header("Positioning")]
    [SerializeField] private Vector2 defaultPos = new Vector2(0, 0);
    [SerializeField] private Vector2 additivePos = new Vector2(0, 1);

    private void Start()
    {
        defaultPos = GetComponent<RectTransform>().position;

        texts = new Text[textCount];

        for (int i = 0; i < textCount; i++)
        {
            texts[i] = Instantiate(textPrefab).GetComponent<Text>();
            texts[i].rectTransform.SetParent(transform);
            texts[i].rectTransform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ShowText(0, "HELLO");
            ShowText(1, "HELLO2");

        }
    }

    public void ShowText(int pIndex, string pText)
    {
        // Set text
        texts[pIndex].text = pText;

        // Set position
        if (visibleTexts.Contains(texts[pIndex].rectTransform) == false)
            visibleTexts.Add(texts[pIndex].rectTransform);
        UpdateTextPositions();
    }

    private void UpdateTextPositions()
    {
        for (int i = 0; i < visibleTexts.Count; i++)
        {
            visibleTexts[i].position = defaultPos + (additivePos * i);
        }
    }
}
