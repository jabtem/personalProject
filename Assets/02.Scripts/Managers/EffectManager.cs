using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public string[] paths;
    public GameObject[] effectObjs;
    Dictionary<int, Stack<GameObject>> effectsDic = new Dictionary<int, Stack<GameObject>>();

    private void Awake()
    {
        paths = System.IO.Directory.GetFiles("Assets/03.Prefabs/Effect", "*.prefab");
        effectObjs = new GameObject[paths.Length];
        for (int i = 0; i < paths.Length; ++i)
        {
            effectObjs[i] = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(paths[i], typeof(GameObject));
        }

        if (GameManager.instance !=null)
        {
            GameManager.instance.Effect = this;
        }
    }

    void CreateEffect(int num)
    {
        Stack<GameObject> stack;
        GameObject go = EffectObj(num);
        //해당 넘버 스킬이펙트 스택이 존재하지않으면 새로운스택할당
        if (!effectsDic.TryGetValue(num,out stack))
        {
            effectsDic.Add(num, stack = new Stack<GameObject>());
        }
        effectsDic[num].Push(go);

        go.transform.SetParent(this.transform);
        go.SetActive(false);
    }

    public void PopEffect(int num, Vector3 pos, Vector3 dir)
    {
        GameObject reqObject = null;
        SkillBase skill;
        Stack<GameObject> stack;
        if (!effectsDic.TryGetValue(num,out stack))
        {
            CreateEffect(num);
        }
        else if(effectsDic.TryGetValue(num, out stack)&& effectsDic[num].Count <=0)
        {
            CreateEffect(num);
        }
        reqObject = effectsDic[num].Pop();
        reqObject.TryGetComponent<SkillBase>(out skill);
        reqObject.transform.SetParent(null);
        skill.SkillNum = num;
        skill.Pos = pos;
        skill.Dir = dir;
        reqObject.SetActive(true);

    }

    public void PushEffect(int num, GameObject go)
    {
        go.SetActive(false);
        effectsDic[num].Push(go);
        go.transform.SetParent(this.transform);
    }

    GameObject EffectObj(int num)
    {
        GameObject effect =null;

        switch((PlayerCharacter)GameManager.instance.CharacterNum)
        {
            case PlayerCharacter.Katana:
                switch(num)
                {
                    case 1:
                        effect = Instantiate(effectObjs[0]);
                        break;
                    case 2:
                        effect = Instantiate(effectObjs[1]);
                        break;
                }    
                break;
            case PlayerCharacter.Spear:
                break;
            case PlayerCharacter.TwohandSword:
                break;
        }


        return effect;
    }
}
