using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetItemRenderTexture : MonoBehaviour
{
    public Transform[] prefabs;
    public Transform nftPrefab;

    public Transform nftBtn;
    
    public Image imageItem;
    public Transform imageNFTItem;

    public Transform Contents;
    
    
    public Camera imageCamera;

    
    public Material renderMaterial;
    
    
    
    
    private Vector3 objItemPos = new Vector3(20, 0, 0);
    
    private Image[] imageMaterials;
    
    GameObject[] allCreateObj;
    private Vector3[] allCreateObjCameraPos;
    private Quaternion[] allCreateObjCameraRotation;
    private RenderTexture[] allCreateObjTexture;
    private Material[] allCreateObjMaterial;
    
    //nft
    private Vector3 objNFTItemPos = new Vector3(20, 200, 0);
    
    private Image[] imageNFTMaterials;

    GameObject[] allNFTCreateObj;
    private Vector3[] allNFTCreateObjCameraPos;
    private Quaternion[] allNFTCreateObjCameraRotation;
    private RenderTexture[] allNFTCreateObjTexture;
    private Material[] allNFTCreateObjMaterial;

    
    private string[] gifUrls = new string[]
    {
     "https://lh3.googleusercontent.com/YJ3DWZGWmtmF13MfTWUZQl9sIrJjnZB_6NKTEDI0vMBdY5VZDsR74NeyzlA_JOz6h-CWMP9M7w0hZtC1Lvqq6Ihvu_0eGj6h00HF",
     "https://lh3.googleusercontent.com/VSTJaJntjLDPSYdMCPJVydjcQfw40b2z667V34YvR8g8cvbtf4Tlf-MKGi4i2dxE14kuJTdwAd6WYQNckxU3J2bfZv_2KrUzRZH3"
    };
    
    
    void Start()
    {
        CreateNormalPrefab();
    }
    
    
    void CreateNormalPrefab()
    {
        allCreateObj = new GameObject[prefabs.Length];
        allCreateObjCameraPos = new Vector3[prefabs.Length];
        allCreateObjCameraRotation = new Quaternion[prefabs.Length];
        allCreateObjTexture = new RenderTexture[prefabs.Length];
        allCreateObjMaterial = new Material[prefabs.Length];
        
        imageMaterials = new Image[prefabs.Length];
        
        for (int i = 0; i < prefabs.Length; i++)
        {
            Transform item = Instantiate(imageItem, Contents).transform;
            item.GetComponent<DropUIObject>().SetItemIdx(i, this);
            
            imageMaterials[i] = item.GetComponent<Image>();
            Transform objItem = Instantiate(prefabs[i]);
            objItem.position = objItemPos + new Vector3(i * 20,0,0);
            
            item.GetComponent<DropUIObject>().TargerObject = prefabs[i].gameObject;

            allCreateObj[i] = objItem.gameObject;
            allCreateObjCameraPos[i] = objItem.Find("Main Camera").position;
            allCreateObjCameraRotation[i] = objItem.Find("Main Camera").rotation;
            allCreateObjTexture[i] = new RenderTexture(256, 256, 8);
            
            allCreateObjMaterial[i] = new Material(renderMaterial);
            allCreateObjMaterial[i].SetTexture("_BaseMap",allCreateObjTexture[i]);
            
            imageMaterials[i].material = allCreateObjMaterial[i];
        }
        
        
        StartCoroutine(setCameraRender());
    }
    
     
    IEnumerator setCameraRender()
    {
        for (int i = 0; i < imageMaterials.Length; i++)
        {
            
            imageCamera.transform.position = allCreateObjCameraPos[i];
            imageCamera.transform.rotation = allCreateObjCameraRotation[i];

            imageCamera.targetTexture = allCreateObjTexture[i];
            imageCamera.Render();
            imageCamera.targetTexture = null;
            
            yield return null;
        }
        
        imageCamera.gameObject.SetActive(false);
    }


    private Coroutine rotation;
    /// <summary>
    /// Start adjust
    /// </summary>
    public void StartRotationObj(int idx)
    {
        imageCamera.transform.position = allCreateObjCameraPos[idx];
        imageCamera.transform.rotation = allCreateObjCameraRotation[idx];

        
        rotation = StartCoroutine(rotationObjIE(idx));
    }

    /// <summary>
    /// stop adjust
    /// </summary>
    /// <param name="idx"></param>
    public void StopRotationObj(int idx)
    {
        StopAllCoroutines();
    }
    
    IEnumerator rotationObjIE(int idx)
    {
        allCreateObj[idx].transform.Rotate(new Vector3(0,2,0));
        
        imageCamera.targetTexture = allCreateObjTexture[idx];
        imageCamera.Render();
        imageCamera.targetTexture = null;

        yield return null;
        
        StartCoroutine(rotationObjIE(idx));
    }



    public void TestNFT()
    {
        CreateNftPrefab(gifUrls);
    }

    private List<Transform> nftItem = new List<Transform>();
    
    /// <summary>
    /// create NFT object
    /// </summary>
    /// <param name="urls"></param>
    public void CreateNftPrefab(string[] urls)
    {
        nftBtn.gameObject.SetActive(false);
        
        if (nftItem.Count > 0)
        {
            for (int i = nftItem.Count - 1; i >= 0; i--)
            {
                Destroy(nftItem[i].gameObject);
            }

            nftItem = new List<Transform>();
        }
        
        for (int i = 0; i < urls.Length; i++)
        {
            Transform item = Instantiate(imageNFTItem, Contents).transform;
            item.GetComponent<UniGifImage>().m_loadOnStartUrl = urls[i];
            item.GetComponent<DropUIObject>().SetNft(urls[i]);

            nftItem.Add(item);
        }
    }

}
