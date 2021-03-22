using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disquise
{
    private readonly Colored _colored;
    public Disquise(Colored colored)
    {
        _colored = colored;
    }

    public void StartDisquise(Color disquiseColor, float disquiseDuration)
    {
        _colored.StartCoroutine(ChangeDisquise(disquiseColor, disquiseDuration));
    }

    private IEnumerator ChangeDisquise(Color disquiseColor, float disquiseDuration)
    {
        var oldColor = _colored.sprite.color;
        _colored.sprite.color = disquiseColor;

        yield return new WaitForSeconds(disquiseDuration);

        _colored.StartCoroutine(ChangeDisquise(oldColor, disquiseDuration));
    }
}

public class Colored : MonoBehaviour
{
    public SpriteRenderer sprite;

    public Movement movement;
    public Disquise disquise;
    
    public OnColoredClick OnColoredClick;

    private Transform _transform;
    private void Awake()
    {
        disquise = new Disquise(this);
        _transform = transform;
    }

    public bool IsAgro { get; set; }
    
    void OnMouseDown()
    {
        gameObject.SetActive(false);

        ParticlesManager.Instance.ActivateParticles(_transform.position, sprite.color);
        StopAllCoroutines();
        OnColoredClick(IsAgro);
    }
}
