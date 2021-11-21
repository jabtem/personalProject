using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
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
}


//아이템데이터 읽기용 
public class SkillDataReader : MonoBehaviour
{
    public TextAsset skillData;

    public Skill[] skills;

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

        skills = new Skill[line-1];

        for(int i=0; i< line-1; i++)
        {
            //첫행은 실제 데이터값이아닌 필드명칭이므로 제외하여 계산
            skills[i] = new Skill();

            
            skills[i].skill_name = data[tableSize * (i+1)];
            skills[i].skill_id = int.Parse(data[tableSize * (i + 1) +1 ]);

        }
    }



}
