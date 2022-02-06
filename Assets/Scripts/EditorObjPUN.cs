using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EditorObjPUN : MonoBehaviourPunCallbacks,IPunObservable
{
    public UniGifImage nftImage;
    
    private bool meshColliderIsOpen;
    private string nfturl;

    private Collider[] selfCollider;
    private Collider[] childrenCollider;

    
    private void Start()
    {
        selfCollider = GetComponents<Collider>();
        childrenCollider = GetComponentsInChildren<Collider>();

        SetMeshCollider(false);
    }

    public void SetMeshCollider(bool state)
    {
        meshColliderIsOpen = state;
        // foreach (Collider collider1 in selfCollider)
        // {
        //     collider1.enabled = state;
        // }
        
        foreach (Collider collider1 in childrenCollider)
        {
            collider1.enabled = state;
        }
    }

    public void SetUrl(string url)
    {
        nfturl = url;
        StartCoroutine(nftImage.SetGifFromUrlCoroutine(url));
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(meshColliderIsOpen);
            stream.SendNext(nfturl);
        }
        else
        {
            // Network player, receive data
            meshColliderIsOpen = (bool)stream.ReceiveNext();
            nfturl = (string)stream.ReceiveNext();
            
            StartCoroutine(nftImage.SetGifFromUrlCoroutine(nfturl));

            // foreach (Collider collider1 in selfCollider)
            // {
            //     collider1.enabled = meshColliderIsOpen;
            // }
            
            foreach (Collider collider1 in childrenCollider)
            {
                collider1.enabled = meshColliderIsOpen;
            }
        }
    }
}
