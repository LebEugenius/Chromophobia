using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum State { Intro, Game, Ending }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public GameObject ColorGoPrefab;
    public Color[] colors;
    public GameObject Intro;
    public GameObject DeathScreen;
    public TMPro.TMP_Text DeathText;
    public GameObject InGameUI;
    public TMPro.TMP_Text helpText;

    [Header("Common")]
    public int startGOCount;
    public int minimumAggresiveColors;
    public float minScale;
    public float maxScale;
    [Tooltip("X - Left, Y - Right, Z - Top, W - Bottom ")]
    public Vector4 spawnOffset;

    public int tutorialLength = 5;


    private State _state = State.Intro;
    [System.NonSerialized] public int currentLevel;
    [System.NonSerialized] public Color currentColor;
    private List<Colored> ColorGOs = new List<Colored>();

    [Header("Moving")]
    public int startMovingLevel;
    public int minimumMovingGOs;
    public float minSpeed;
    public float maxSpeed;

    [Header("Panic")]
    public Image Face;
    public Sprite[] emotions;
    public int[] panicLevels;
    public float MaxPanic;
    [System.NonSerialized] public float PanicLevel;

    [Header("Blindness")]
    public SpriteRenderer blindColor;
    public float reduceBlindSpeed;

    [Header("Ending")] 
    public float restartClickDelay = 1f;

    [Header("Audio")] 
    public AudioSource SFXPlayer;
    public AudioClip correctClickClip;
    public float correctClickVolume = 0.1f;
    public AudioClip wrongClickClip;
    public float wrongClickVolume = 2f;

    private float _endTime;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        switch (_state)
        {
            case State.Intro:
                    IntroUpdate();
                break;
            case State.Game:
                    GameUpdate();
                break;
            case State.Ending: 
                    FinishUpdate();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void IntroUpdate()
    {
        if (Input.anyKeyDown)
        {
            Intro.SetActive(false);
            Face.gameObject.SetActive(true);
            for (var i = 0; i < startGOCount; i++)
                AddNewGO();

            currentLevel = 0;
            StartNewRound();
            _state = State.Game;
            InGameUI.SetActive(true);
        }
    }
    void GameUpdate()
    {
        if(PanicLevel < MaxPanic && currentLevel > tutorialLength)
            PanicLevel += Time.deltaTime;
        else if(PanicLevel >= MaxPanic)
            FinishGame();

        for (var i = emotions.Length - 1; i >= 0; i--)
        {
            if (PanicLevel / MaxPanic < panicLevels[i] / 100f) continue;
            Face.sprite = emotions[i];
            break;
        }
    }
    void FinishUpdate()
    {
        if (Input.anyKeyDown && Time.time > _endTime + restartClickDelay)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void StartNewRound()
    {
        PanicLevel = 0;
        //Choose new color
        currentColor = GetRandomColor();
        Face.color = currentColor;
        
        currentLevel++;

        SetDifficulty();

        ActivateBlindness();

        SetColoreds();
        SetMovingColoreds();
    }

    public void SetColoreds()
    {
        for (var i = 0; i < ColorGOs.Count; i++)
        {
            var isBadOne = minimumAggresiveColors > i;
            var randomSize = Random.Range(minScale, maxScale);

            //Randomize position in screen bounds
            ColorGOs[i].transform.position = GetRandomPos(isBadOne);
            ColorGOs[i].transform.localScale = Vector3.one * randomSize;

            //Reset Moving
            ColorGOs[i].StartMoving(Vector3.zero, 0);

            //Color objects
            ColorGOs[i].sprite.color = isBadOne ? currentColor : GetRandomColor();
            ColorGOs[i].sprite.sortingOrder = isBadOne ? 1 : 0;

            //Activate ready go
            ColorGOs[i].gameObject.SetActive(true);
        }
    }
    public void SetDifficulty()
    {
        startGOCount = Mathf.Max(startGOCount, 1);

        minimumAggresiveColors =  Mathf.Max(minimumAggresiveColors, 1);;

        if (currentLevel % 2 == 0)
            AddNewGO();

        if (currentLevel % 5 == 0)
            minimumAggresiveColors++;
        
        if(currentLevel < startMovingLevel) return;

        if (currentLevel % 5 == 0 && currentLevel != startMovingLevel)
            minimumMovingGOs++;

        minimumMovingGOs = Mathf.Min(minimumMovingGOs, ColorGOs.Count);
    }
    public void SetMovingColoreds()
    {
        if(currentLevel < startMovingLevel) return;

        var currentMoving = 0;
        do
        {
            var randomGO = Random.Range(0, ColorGOs.Count);

            var size = ColorGOs[randomGO].transform.localScale.x;
            var randomSpeed = Random.Range(minSpeed, maxSpeed);

            if (ColorGOs[randomGO].IsMoving()) continue;

            currentMoving++;
            ColorGOs[randomGO].StartMoving( GetRandomPos(ColorGOs[randomGO].sprite.color == currentColor),
                randomSpeed * size );

        } while (currentMoving < minimumMovingGOs);
    }

    public void SetTutorial()
    {
        if (currentLevel < tutorialLength)
        {
            helpText.color = currentColor;
          
            if(currentColor == Color.blue)
                helpText.text = "Click on blue color";
            else if(currentColor == Color.red)
                helpText.text = "Click on red color";
            else
                helpText.text = "Click on green color";
        }
        else
            helpText.text = "";
    }

    void FinishGame()
    {
        _state = State.Ending;

        _endTime = Time.time;
        DeathScreen.SetActive(true);
        InGameUI.SetActive(false);
        DeathText.text = "You failed to save him on " + currentLevel + " try";

        foreach (var colored in ColorGOs)
        {
            colored.gameObject.SetActive(false);  
        }
        Application.ExternalCall("kongregate.stats.submit", "Record", currentLevel);
    }

    public void ActivateBlindness()
    {
        blindColor.color = currentColor;
        StopCoroutine("ReduceBlindness");
        StartCoroutine(ReduceBlindness(blindColor));
    }

    IEnumerator ReduceBlindness(SpriteRenderer sprite)
    {
        var color = sprite.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * reduceBlindSpeed;
            sprite.color = color;
            yield return null;
        }
    }

    Vector3 GetRandomPos(bool isBadOne)
    {
        var screenPosX = Random.Range(spawnOffset.x, Screen.width - spawnOffset.y);
        var screenPosY = Random.Range(spawnOffset.w, Screen.height - spawnOffset.z);

        var newPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosX, screenPosY));
        newPos.z = isBadOne ? 0 : 1;
        return newPos;
    }
    Color GetRandomColor()
    {
        Color color;
        do
        {
            color = colors[Random.Range(0, colors.Length)];
        } while (color == currentColor);

        return color;
    }
    void AddNewGO()
    {
        var go = Instantiate(ColorGoPrefab, Vector3.zero, Quaternion.identity, transform);
        ColorGOs.Add(go.GetComponent<Colored>());
    }

    public void CheckForComplete()
    {
        foreach (var go in ColorGOs)
        {
            if(go.gameObject.activeInHierarchy && go.sprite.color == currentColor)
                return;
        }

        StartNewRound();
    }

    public void PlayClickSound(bool correct)
    {
        SFXPlayer.PlayOneShot(correct ? correctClickClip : wrongClickClip, correct ? correctClickVolume : wrongClickVolume);
    }
}
