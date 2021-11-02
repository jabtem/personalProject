using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject katana;
    public GameObject spear;
    public GameObject twoHandSword;
    public RadarMap miniMap;

    public void Create(int num)
    {
        GameObject go;

        switch (num)
        {
            case 1:
                go = GameObject.Instantiate(katana, this.gameObject.transform.position, this.gameObject.transform.rotation);
                miniMap.playerPos = go.transform;
                miniMap.player = go.GetComponent<PlayerMoveCtrl>();
                break;
            case 2:
                go = GameObject.Instantiate(spear, this.gameObject.transform.position, this.gameObject.transform.rotation);
                miniMap.playerPos = go.transform;
                miniMap.player = go.GetComponent<PlayerMoveCtrl>();
                break;
            case 3:
                go = GameObject.Instantiate(twoHandSword, this.gameObject.transform.position, this.gameObject.transform.rotation);
                miniMap.playerPos = go.transform;
                miniMap.player = go.GetComponent<PlayerMoveCtrl>();
                break;
        }
    }
}
