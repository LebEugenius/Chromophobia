using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Image Face;
    [SerializeField] private Sprite[] Emotions;
    [SerializeField] private int[] PanicLevels;
    [SerializeField] private Dialog[] Dialogs;

    [Header("Screens")]
    [SerializeField] private GameObject IntroScreen;
    [SerializeField] private GameObject InGameScreen;
    [SerializeField] private GameObject DeathScreen;

    [Header("Texts")]
    [SerializeField] private TMPro.TMP_Text DeathText;
    [SerializeField] private TMPro.TMP_Text Dialog;
    [SerializeField] private TMPro.TMP_Text LevelText;

    [Header("Flash")]
    [SerializeField] private SpriteRenderer Flash;
    [SerializeField] private float FlashFadeSpeed;

    public void OnGameStarted()
    {
        DeathScreen.SetActive(false);
        IntroScreen.SetActive(false);
        InGameScreen.SetActive(true);
    }

    public void OnGameFinished(int currentLevel)
    {
        DeathScreen.SetActive(true);
        InGameScreen.SetActive(false);
        DeathText.text = "You failed to save him on " + currentLevel + " try";
    }

    public void SetEmotion(float panicRatio)
    {
        if (Emotions.Length != PanicLevels.Length)
        {
            Debug.LogWarning("Emotion Array has different length with Panic's level Array");
            return;
        }

        for (var i = Emotions.Length - 1; i >= 0; i--)
        {
            if (panicRatio * 100 < PanicLevels[i]) continue;
            Face.sprite = Emotions[i];
            break;
        }
    }

    public void ColorUI(Color color)
    {
        Face.color = color;
        Dialog.color = color;
        LevelText.color = color;
    }

    public void ShowText(int currentLevel)
    {
        foreach (var t in Dialogs)
        {
            if (t.DialogLevel != currentLevel) continue;

            string color;
            if (Dialog.color == Color.red)
                color = "red";
            else if (Dialog.color == Color.green)
                color = "green";
            else
                color = "blue";

            Dialog.text = string.Format(t.DialogText, color);
            return;
        }

        Dialog.text = "";
    }

    public void SetLevel(int currentLevel)
    {
        LevelText.text = currentLevel.ToString();
    }

    public void ActivateFlash(Color color)
    {
        StopCoroutine(FadeFlash(Flash));
        Flash.color = color;     
        StartCoroutine(FadeFlash(Flash));
    }

    private IEnumerator FadeFlash(SpriteRenderer sprite)
    {
        var color = sprite.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * FlashFadeSpeed;
            sprite.color = color;
            yield return null;
        }
    }
}
