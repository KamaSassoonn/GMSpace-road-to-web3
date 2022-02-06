using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MetaMask : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void AwarkMetaMask();

    public SetItemRenderTexture SetItemRenderTexture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNFTS()
    {
        Debug.Log("Get NFT GetNFTS");
        AwarkMetaMask();
    }

    public void NftsDataCall(string data)
    {
        Debug.Log("NFT Call Back");
        
        string[] astr = data.Split ('|');
        // for (int i = 0; i < astr.Length; i++)
        // {
        //     Debug.Log(astr[i]);
        // }

        SetItemRenderTexture.CreateNftPrefab(astr);
    }
}
