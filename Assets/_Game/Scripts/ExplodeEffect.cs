using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffect : MonoBehaviour
{
    public ParticleSystem core;
    public ParticleSystem area;
    public ParticleSystem spark;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeEffectColor(Color color1)
    {
        var coreCol = core.colorOverLifetime;
        var areaCol = area.colorOverLifetime;
        var sparkCol = spark.colorOverLifetime;

        var coreStartCol = core.main;
        var areaStartCol = area.main;
        var sparkStartCol = spark.main;

        coreCol.enabled = true;
        areaCol.enabled = true;
        sparkCol.enabled = true;

        Gradient coreColgrad = new Gradient();
        coreColgrad.SetKeys(new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color1, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });

        Gradient areaColgrad = new Gradient();
        areaColgrad.SetKeys(new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color1, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });

        Gradient sparkColgrad = new Gradient();
        sparkColgrad.SetKeys(new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color1, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });

        coreCol.color = coreColgrad;
        areaCol.color = areaColgrad;
        sparkCol.color = sparkColgrad;
        coreStartCol.startColor = color1;
        areaStartCol.startColor = color1;
        sparkStartCol.startColor = color1;

    }
}
