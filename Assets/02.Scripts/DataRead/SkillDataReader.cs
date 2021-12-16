using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Skill
{
    public Skill(){}
    public Skill(string name, int id, float coolTime)
    {
        skill_name = name;
        skill_id = id;
        skill_coolTime = coolTime;
    }

    //스킬명
    public string skill_name
    {
        get
        {
            return _skill_name;
        }
        set
        {
            _skill_name = value;
        }
    }
    [SerializeField]
    string _skill_name;
    //스킬의 고유id
    public int skill_id
    {
        get
        {
            return _skill_id;
        }
        set
        {
            _skill_id = value;
        }
    }
    [SerializeField]
    int _skill_id;

    public float skill_coolTime
    {
        get
        {
            return _skill_coolTime;
        }
        set
        {
            _skill_coolTime = value;
        }
    }
    [SerializeField]
    float _skill_coolTime;
}


//아이템데이터 읽기용 
public class SkillDataReader : MonoBehaviour
{
    static public SkillDataReader instance;
    public TextAsset skillData;


    //public Skill[] skills;

    //딕셔너리의경우 해쉬맵과 같아 시간복잡도가 O(1)이므로 배열을탐색하는것보다 빠름
    Dictionary<int, Skill> skillMap = new Dictionary<int, Skill>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }

    }
    private void Start()
    {
        ReadCSV();
    }

    
    [ContextMenu("ReadCSV")]
    void ReadCSV()
    {
        string[] data = skillData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);//특정 문자를 기준으로 문자열 분할

        //실제 데이터 라인수 = 텍스트의 줄넘김 기준으로 문자열 분할했을때 데이터길이 -1
        //CSV데이터는 항상 값이 비어있는 라인이 하나 추가되어있음
        //행개수 하나 제거(빈행)
        int line = skillData.text.Split('\n').Length - 1;

        //열 개수 = (전체 문자열수 / 행 개수)
        int tableSize =data.Length / line;

        //skills = new Skill[line-1];

        for(int i=0; i< line-1; i++)
        {
            //첫행은 실제 데이터값이아닌 필드명칭이므로 제외하여 계산


            //인스펙터 데이터 확인용
            //skills[i] = new Skill();
            //skills[i].skill_name = data[tableSize * (i+1)];
            //skills[i].skill_id = int.Parse(data[tableSize * (i + 1) +1 ]);
            //skills[i].skill_coolTime = float.Parse(data[tableSize * (i + 1) + 2]);

            string name = data[tableSize * (i + 1)];
            int id = int.Parse(data[tableSize * (i + 1) + 1]);
            float coolTime = float.Parse(data[tableSize * (i + 1) + 2]);
            skillMap.Add(id, new Skill(name,id,coolTime));

        }
    }


    //현재 스킬정보를 가져옴
    public int GetCurrentSKilInfo(Action<bool> disalbeSkill)
    {
        //현재 스킬버튼에 할당된 스킬id정보를받아오기위함
        SkillSlot curSkill = GetComponent<SkillSlot>();
        SkillCoolTime skillCollTime = GetComponent<SkillCoolTime>();

        //해당 스킬id가 존재하는경우
        if (skillMap.ContainsKey(curSkill.skillId) )
        {
            skillCollTime.StartCoolTime(skillMap[curSkill.skillId].skill_coolTime, (value) => disalbeSkill(value));
            Debug.Log("test");

            return curSkill.skillId;

            
        }

        return 0;

    }
}
