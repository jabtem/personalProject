using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIconManager : MonoBehaviour
{

    public static MinimapIconManager instance;
    public Image enemyIcon;//적의 미니맵아이콘
    public Image playerIcon;//플레이어
    List<Image> enemyIcons = new List<Image>();
    List<Image> playerIcons = new List<Image>();
    // Start is called before the first frame update
    void Awake()
    {
        if(MinimapIconManager.instance == null)
        {
            MinimapIconManager.instance = this;
        }
    }

    void Start()
    {
        CreateEenmyIcon(5);
    }

    void CreateEenmyIcon(int enemyCnt)
    {
        for(int i = 0; i < enemyCnt; i++)
        {
            Image minimapIcon = Instantiate(enemyIcon) as Image;
            minimapIcon.transform.SetParent(transform);
            minimapIcon.gameObject.SetActive(false);
            enemyIcons.Add(minimapIcon);

        }
    }

    void CreatePlayerIcon(int plyerCnt)
    {
        for(int i = 0; i<plyerCnt; i++)
        {
            Image minimapIcon = Instantiate(playerIcon) as Image;
            minimapIcon.transform.SetParent(transform);
            minimapIcon.gameObject.SetActive(false);
            playerIcons.Add(minimapIcon);

        }
    }

    public Image GetMinimapIcon(string tag)
    {
        Image reqObject = null;

        switch(tag)
        {
            case "Enemy":
                for (int i = 0; i < enemyIcons.Count; i++)
                {
                    if (enemyIcons[i].gameObject.activeSelf == false)
                    {
                        reqObject = enemyIcons[i];
                        break;
                    }
                }
                //초기설정 개수보다 부족할경우 추가생성
                if (reqObject == null)
                {
                    Image newObject = Instantiate(enemyIcon) as Image;
                    newObject.transform.SetParent(transform);
                    enemyIcons.Add(newObject);
                    reqObject = newObject;
                }
                break;
            case "Player":
                for (int i = 0; i < playerIcons.Count; i++)
                {
                    if (playerIcons[i].gameObject.activeSelf == false)
                    {
                        reqObject = playerIcons[i];
                        break;
                    }
                }
                //초기설정 개수보다 부족할경우 추가생성
                if (reqObject == null)
                {
                    Image newObject = Instantiate(playerIcon) as Image;
                    newObject.transform.SetParent(transform);
                    playerIcons.Add(newObject);
                    reqObject = newObject;
                }
                break;
        }

        reqObject.gameObject.SetActive(true);


        return reqObject;
    }


}
