using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    public static ParticlesManager Instance;
    [SerializeField] private Pool particlesPool;
    void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ActivateParticles(Vector2 pos, Color color)
    {
        var particles = particlesPool.GetEntity().GetComponent<ParticleSystem>();

        particles.transform.position = pos;
        var main = particles.main;
        main.startColor = color;
        
        particles.gameObject.SetActive(true);
        particles.Play();
    }
    
}
