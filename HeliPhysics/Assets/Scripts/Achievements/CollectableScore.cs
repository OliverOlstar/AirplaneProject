using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScore : MonoBehaviour
{
    public static CollectableScore Instance { get; private set; }

    [SerializeField] private AchievementHUD achievementHUD;

    private int[] collectables = new int[3];
    private int[] collectablesMaxes = new int[3];
    [SerializeField] private string[] collectablesText = new string[3];

    private void Awake()
    {
        collectables = new int[collectablesText.Length];
        collectablesMaxes = new int[collectablesText.Length];

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Collected(int pIndex)
    {
        collectables[pIndex]++;
        achievementHUD.ShowText(pIndex, collectables[pIndex] + "/" + collectablesMaxes[pIndex] + " - " + collectablesText[pIndex]);
    }

    public void addToMax(int pIndex)
    {
        collectablesMaxes[pIndex]++;
    }
}
