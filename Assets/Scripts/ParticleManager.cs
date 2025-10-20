using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    [SerializeField] private Particle[] types;
    [SerializeField] private GameObject[] effects;
    private Dictionary<Particle, GameObject> particles;
    public enum Particle
    {
        Walkcloud,
        Pushcloud,
        Deathcloud
    }

    private void Start()
    {
        int size = types.Length;
        particles = new Dictionary<Particle, GameObject>();
        for (int i = 0; i < size; i++)
        {
            particles.Add(types[i], effects[i]);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateParticleEffect(Particle type, Vector3 pos, float destroyTime)
    {
        GameObject effect = Instantiate(particles[type], pos, Quaternion.identity);
        effect.GetComponent<ParticleSystem>().Play();
        Destroy(effect, destroyTime);
    }

}
