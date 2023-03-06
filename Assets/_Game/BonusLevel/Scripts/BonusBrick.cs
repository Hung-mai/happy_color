using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBrick : MonoBehaviour
{
    public Transform tf;
    public Transform cubeTransform;
    public AnimationCurve curve;
    public MeshRenderer meshRenderers;
    public BoxCollider boxCollider;
    public GameObject SampleCurve;
    public GameObject cubeDefault;
    public GameObject brickShadow;
    public GameObject cubeBrickEndgame;
    public MeshRenderer cubeBrickEndgameRenderer;
    public Material defaultMaterial;
    public Material clearMaterial;
    public Material wallMaterial;
    public Material currentMaterial;//Màu hiện tại của Brick
    public Material exactMaterial;  //Màu của Brick khi hoàn thành
    public Vector2Int id;
    public bool isWaving = false;
    private float timeCounting = 0;
    private float executetime = 0.5f;
    private bool allowAnimWave = false;
    public bool isComplete;
    public ParticleSystem.ColorOverLifetimeModule colorOverLifetime;
    public void BrickInit(Vector2Int id, Material exactMat)
    {
        this.id = id;
        Material defaultMat;

        if (exactMat.color == new Color32(0, 0, 0, 255)) //Nếu màu đúng của Brick là (0,0,0) thì Brick đó là tường
        {
            boxCollider.enabled = false;
            exactMat = wallMaterial;
            defaultMat = exactMat;
        }
        else if(exactMat.color == new Color32(255, 255, 255, 255))  //Màu (255,255,255) là trong suốt
        {
            defaultMat = clearMaterial;
            boxCollider.enabled = false;
        }
        /*else if (exactMat.color == new Color32(254, 254, 254, 255))  //Màu (254,254,254) là màu trắng
        {
            defaultMat = clearMaterial;
            for (int i = 0; i < BonusLevelManager.Instance.materialsData.listMaterials.Count; i++)
            {
                if(BonusLevelManager.Instance.materialsData.listMaterials[i].color== new Color32(254, 254, 254, 255))
                {
                    defaultMat = BonusLevelManager.Instance.materialsData.listMaterials[i];
                }
            }
            boxCollider.enabled = false;
        }*/
        else
        {
            defaultMat = defaultMaterial;
            for (int i = 0; i < BonusLevelManager.Instance.colorList.Count; i++)
            {
                if (exactMat.color == new Color32((byte)BonusLevelManager.Instance.colorList[i].x, (byte)BonusLevelManager.Instance.colorList[i].y, (byte)BonusLevelManager.Instance.colorList[i].z, 255))
                {
                    defaultMat = BonusLevelManager.Instance.defaultMaterialsData.listMaterials[i];
                }
            }
        }
        exactMaterial = exactMat; //Setup Material đúng của Brick
        ChangeMaterial(defaultMat);
    }

    public void SampleBrickInit(Material exactMat)
    {
        if (exactMat.color == new Color32(0, 0, 0, 255)) //Nếu màu đúng của Brick là (0,0,0) thì Brick đó là tường
        {
            exactMat = wallMaterial;
        }
        ChangeMaterial(exactMat);
    }

    public void OnHit()
    {
        if (isWaving) return;
        Vibration.Vibrate(3);
        ChangeMaterial(BonusGameManager.Instance.choosingMaterial);
        timeCounting = 0;
        SoundController.Instance.sound_Bubble.PlaySoundOneShot();
        StartCoroutine(WaveAnim());
    }

    public IEnumerator WaveAnim()
    {
        isWaving = true;
        yield return Cache.GetWFS(0.02f);
        timeCounting += 0.02f;
        cubeTransform.position = new Vector3(cubeTransform.position.x, curve.Evaluate(timeCounting), cubeTransform.position.z);
        if (timeCounting < executetime) StartCoroutine(WaveAnim());
        else isWaving = false;
    }
    public void ChangeMaterial(Material material)
    {
        meshRenderers.material = material;
        currentMaterial = material;
        isComplete = (currentMaterial == exactMaterial);
    }

    public void ChangeLayerEndGame()
    {
        gameObject.layer = LayerMask.NameToLayer(Constant.DEFAULT);
        SampleCurve.layer = LayerMask.NameToLayer(Constant.DEFAULT);
    }

    public void SkipButton()    //Chuyển sang màu đúng của Brick nếu người chơi nhấn vào Skip
    {
        ChangeMaterial(exactMaterial);
    }

    public void ExplodeCube()
    {
        StartCoroutine(ExplodeCubeEndGame());
    }

    IEnumerator ExplodeCubeEndGame()
    {
        yield return Cache.GetWFS(0.2f);
        SoundController.Instance.sound_TEP.PlaySoundOneShot(Random.Range(0, 5));
        ExplodeEffect tempExplode = BonusBrickManager.Instance.endGameExplodePool.Spawn(tf.position, Quaternion.identity);
        tempExplode.ChangeEffectColor(currentMaterial.color);
        cubeDefault.SetActive(false);
        if(brickShadow!=null) brickShadow.SetActive(false);
        yield return Cache.GetWFS(0.3f);
        BonusBrickManager.Instance.endGameExplodePool.Despawn(tempExplode.gameObject);
    }
}
