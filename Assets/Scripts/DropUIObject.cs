using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropUIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject TargerObject;

    public UniGifImage UniGifImage;
    
    Ray ray;//
    RaycastHit hitInfo;

    GameObject TempMoveObject;//temp
    private EditorObjPUN TempObjPun;


    private int itemIdx;
    private SetItemRenderTexture itemRender;

    /// <summary>
    /// set idx
    /// </summary>
    /// <param name="itemIdx"></param>
    /// <param name="itemRender"></param>
    public void SetItemIdx(int itemIdx, SetItemRenderTexture itemRender)
    {
        this.itemIdx = itemIdx;
        this.itemRender = itemRender;
    }


    private string nftUrl;
    private bool isNft;
    /// <summary>
    /// set NFT
    /// </summary>
    /// <param name="url"></param>
    public void SetNft(string url)
    {
        
    }
     
 
    public void OnBeginDrag(PointerEventData eventData)//Drop  
    {
        if (PhotonNetwork.IsConnected)
        {
            TempMoveObject = PhotonNetwork.Instantiate(TargerObject.name, new Vector3(0f,0f,0f), TargerObject.transform.rotation * Quaternion.Euler(0,180,0), 0);
            TempObjPun = TempMoveObject.GetComponent<EditorObjPUN>();
        }
        else
        {
            TempMoveObject = Instantiate(TargerObject, MainEditorControl.ObjectFather);
        }

        if (isNft && TempObjPun)
        {
            TempObjPun.SetUrl(nftUrl);
        }
    }

    

    private float scrollWheelNum;
    float timer = 0;
    bool isLeft = false;
    bool isRight = false;
    void Update()
    {
        if (!TempMoveObject) return;


        if (Input.GetKeyDown(KeyCode.Z))
        {
            isLeft = true;
            isRight = false;
            timer = 0;

            TempMoveObject.transform.localEulerAngles += new Vector3(0, 10, 0);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            isRight = true;
            isLeft = false;
            timer = 0;

            TempMoveObject.transform.localEulerAngles += new Vector3(0, -10, 0);
        }

        if (Input.GetKeyUp(KeyCode.Z) && isLeft)
        {
            isLeft = false;
        }
        else if (Input.GetKeyUp(KeyCode.X) && isRight)
        {
            isRight = false;
        }


        if (isLeft || isRight)
        {
            timer += Time.deltaTime;
            if (timer >= 0.1)
            {
                timer = 0;
                TempMoveObject.transform.localEulerAngles += new Vector3(0, isLeft ? 10 : -10, 0);
            }
        }




     
    }


}
