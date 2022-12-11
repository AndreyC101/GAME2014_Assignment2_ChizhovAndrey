using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text lifeCounter;
    [SerializeField]
    private TMP_Text coinCounter;

    [SerializeField]
    private RectTransform healthBarFill;

    public int lives;
    public float maxHP;
    public float currentHP;
    public int coins;

    public void OnUpdated()
    {
        lifeCounter.text = $"{lives}";
        coinCounter.text = $"{coins}";
        float hpRatio = currentHP / maxHP;
        healthBarFill.localScale = new Vector3(hpRatio, 1.0f, 1.0f);
    }
}
