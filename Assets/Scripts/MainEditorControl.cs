using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MainEditorControl : MonoBehaviour
{
    public static Transform ObjectFather;  
    public static bool UIDropOut = false;  

    private float scrollWheelNum;
    float timer = 0;
    bool isLeft = false;
    bool isRight = false;
    void Update()
    {
        if (!UIDropOut)
        {
            DropObjectOnSence();
        }
 
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


    Ray ray;
    RaycastHit hitInfo;

    float DropTime = 0.3f; //drop delay

    
    Transform TempMoveObject;
    private PhotonView TempMoveObjectPhoton;
    private EditorObjPUN TempObjPun;

    void DropObjectOnSence() //on draging
    {


        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int lay = 1 << LayerMask.NameToLayer("EditorObj");
        

        if (Physics.Raycast(ray, out hitInfo, 100, lay)) //cllasion
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            {
                DropTime -= Time.deltaTime * 1.5f;

                if (TempMoveObject == null && hitInfo.transform.CompareTag("EditorObject"))
                {
                    Transform tempObj = hitInfo.transform;
                    TempMoveObjectPhoton = tempObj.GetComponent<PhotonView>();
                    if (!TempMoveObjectPhoton)
                    {
                        TempMoveObjectPhoton = tempObj.parent.GetComponent<PhotonView>();
                        TempMoveObjectPhoton.TransferOwnership(PhotonNetwork.LocalPlayer);
                        TempObjPun = tempObj.parent.GetComponent<EditorObjPUN>();
                    }
                    else
                    {
                        TempMoveObjectPhoton.TransferOwnership(PhotonNetwork.LocalPlayer);
                        TempObjPun = tempObj.GetComponent<EditorObjPUN>();
                    }
                    Debug.Log("Set this object to main object");
                    
                     TempMoveObject = TempMoveObjectPhoton.transform;
                    
                    TempObjPun.SetMeshCollider(false);
                   
                }

                
                if (TempMoveObject != null && DropTime < 0)
                {
                    TempObjPun.SetMeshCollider(false);
                    TempMoveObject.transform.position = hitInfo.point;
                }


            }


            if (Input.GetKeyUp(KeyCode.Mouse0) && TempMoveObject != null) //unlock mouse
            {
                DropTime = 0.3f;
                TempObjPun.SetMeshCollider(true);
                TempMoveObject = null;
            } else if (Input.GetKeyUp(KeyCode.Mouse1) && TempMoveObject != null)//delete
            {
                DropTime = 0.3f;
                Invoke(nameof(delObj), 0.1f);
            }

        }
    }

    void delObj()
    {
        PhotonNetwork.Destroy(TempMoveObjectPhoton);
    }
    
    
}
