using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public enum State { Intro, Game, Ending }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameSettings Settings;
    public SpawnOffset Offset;

    [Header("Managers")]
    public AudioManager Audio;
    public UIManager UI;

    [Header("References")]
    public GameObject ColoredPrefab;
    public Color[] colors;

    [Header("Ending")] 
    public float restartClickDelay = 1f;

    

    private State _state = State.Intro;   

    private Color _currentColor;
    private int _currentLevel;

    private float _endTime;

    private int _coloredsCount;
    private int _agroColoredsCount;
    private int _movingColoredsCount;

    private float _panicCapacity;
    private float _panic;

    private List<Colored> _coloreds = new List<Colored>();

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
            Initialize();
            
            StartNewRound();

            UI.OnGameStarted();
            _state = State.Game;
        }
    }
    void GameUpdate()
    {
       if ( _currentLevel <= Settings.TutorialLength ) return;
        
        if (_panic < _panicCapacity )
           IncreasePanic(Time.deltaTime);
        else
            FinishGame();

        UI.SetEmotion(_panic / _panicCapacity);
    }
    void FinishUpdate()
    {
        if (Input.anyKeyDown && Time.time > _endTime + restartClickDelay)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Initialize()
    {
        _currentLevel = 0;

        _coloredsCount = Settings.StartColoredsCount;
        _agroColoredsCount = Settings.StartAgroColoredsCount;
        _movingColoredsCount = Settings.StartMovingColoredsCount;

        _panicCapacity = Settings.StartPanicCapacity;

        for (var i = 0; i < Settings.StartColoredsCount; i++)
            AddNewGO();
    }

    void StartNewRound()
    {
        _panic = 0;
        _currentLevel++;
        _currentColor = GetRandomColor();

        UI.ColorFace(_currentColor);
        UI.ActivateFlash(_currentColor);
        UI.SetTutorial(_currentLevel < Settings.TutorialLength, _currentColor);

        IncreaseDifficulty();

        SetColoreds();
        SetMovingColoreds();
    }

    void IncreaseDifficulty()
    {
        _coloredsCount = Mathf.Max(_coloredsCount, 1);
        _agroColoredsCount =  Mathf.Max(_agroColoredsCount, 1);

        if (_currentLevel % Settings.PanicCapacityBonusRate == 0)
            _panicCapacity += Settings.PanicCapacityBonus;

        if (_currentLevel % Settings.NewColoredsRate == 0)
            AddNewGO();

        if (_currentLevel % Settings.AgroColoredsRate == 0)
            _agroColoredsCount++;
        
        if(_currentLevel < Settings.MovingColoredLevel) return;

        if (_currentLevel % 5 == 0 && _currentLevel != Settings.MovingColoredLevel)
            _movingColoredsCount++;

        _movingColoredsCount = Mathf.Min(_movingColoredsCount, _coloreds.Count);
    }

    void SetColoreds()
    {
        for (var i = 0; i < _coloreds.Count; i++)
        {
            var isBadOne = _agroColoredsCount > i;
            var randomSize = Random.Range(Settings.minSize, Settings.maxSize);

            //Randomize position in screen bounds
            _coloreds[i].transform.position = GetRandomPos(isBadOne);
            _coloreds[i].transform.localScale = Vector3.one * randomSize;

            //Reset Moving
            _coloreds[i].StartMoving(Vector3.zero, 0);

            //Color objects
            _coloreds[i].sprite.color = isBadOne ? _currentColor : GetRandomColor();
            _coloreds[i].sprite.sortingOrder = isBadOne ? 1 : 0;

            //Activate ready go
            _coloreds[i].gameObject.SetActive(true);
        }
    }
    void SetMovingColoreds()
    {
        if(_currentLevel < Settings.MovingColoredLevel) return;

        var currentMoving = 0;
        do
        {
            var randomGO = Random.Range(0, _coloreds.Count);

            var size = _coloreds[randomGO].transform.localScale.x;
            var randomSpeed = Random.Range(Settings.minSpeed, Settings.maxSpeed);

            if (_coloreds[randomGO].IsMoving()) continue;

            currentMoving++;
            _coloreds[randomGO].StartMoving( GetRandomPos(_coloreds[randomGO].sprite.color == _currentColor),
                randomSpeed * size );

        } while (currentMoving < _movingColoredsCount);
    }

    void FinishGame()
    {
        _state = State.Ending;

        _endTime = Time.time;
        
        UI.OnGameFinished(_currentLevel);
        UI.ActivateFlash(Color.white);

        foreach (var colored in _coloreds)
        {
            colored.gameObject.SetActive(false);  
        }

        Application.ExternalCall("kongregate.stats.submit", "Record", _currentLevel);
    }

    Vector3 GetRandomPos(bool isBadOne)
    {
        var screenPosX = Random.Range(Screen.width * Offset.Left, Screen.width * (0.5f - Offset.Right) + Screen.width / 2f);
        var screenPosY = Random.Range(Screen.height * Offset.Down, Screen.height * (0.5f - Offset.Top) + Screen.height / 2f);

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
        } while (color == _currentColor);

        return color;
    }

    void AddNewGO()
    {
        var go = Instantiate(ColoredPrefab, Vector3.zero, Quaternion.identity, transform);
        _coloreds.Add(go.GetComponent<Colored>());
    }

    public void CheckForComplete()
    {
        foreach (var go in _coloreds)
        {
            if(go.gameObject.activeInHierarchy && go.sprite.color == _currentColor)
                return;
        }

        StartNewRound();
    }

    public void IncreasePanic(float value)
    {
        _panic += value;
    }

    public bool IsAgroColored(Color coloredColor)
    {
        return coloredColor == _currentColor;
    }

    public void OnWrongColoredClick()
    {
        UI.ActivateFlash(_currentColor);
        Audio.PlayWrongColoredClickClip();
    }

    public void OnCorrectColoredClick()
    {
        Audio.PlayCorrectColoredClickClip();
    }
}
