using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject orSword;
    public GameObject dagger;
    public GameObject axe;
    public RadarMap miniMap;

    public void Create(int num)
    {
        GameObject go;

        switch (num)
        {
            case 1:
                go = GameObject.Instantiate(orSword, this.gameObject.transform.position, this.gameObject.transform.rotation);
                miniMap.playerPos = go.transform;
                miniMap.player = go.GetComponent<PlayerMoveCtrl>();
                break;
            case 2:
                go = GameObject.Instantiate(dagger, this.gameObject.transform.position, this.gameObject.transform.rotation);
                miniMap.playerPos = go.transform;
                miniMap.player = go.GetComponent<PlayerMoveCtrl>();
                break;
            case 3:
                go = GameObject.Instantiate(axe, this.gameObject.transform.position, this.gameObject.transform.rotation);
                miniMap.playerPos = go.transform;
                miniMap.player = go.GetComponent<PlayerMoveCtrl>();
                break;
        }
    }
}
