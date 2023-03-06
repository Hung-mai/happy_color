using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayScript : MonoBehaviour, IObjectPooler
{
    public ParticleSystem[] allParticles;

    public void OnSpawnObject()
    {
        foreach (ParticleSystem p in allParticles)
        {
            p.Play();
        }
    }
}
