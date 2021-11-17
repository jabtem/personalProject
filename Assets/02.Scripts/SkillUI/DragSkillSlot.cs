using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSkillSlot : MonoBehaviour
{

    //스킬 드래그&드롭용슬롯이 단하나만 존재하기때문에 허용
    static public DragSkillSlot instance;
    Image skillImage;


    //this 안붙이면 스택오버플로우
    public SkillSlot dragSlot
    {
        get
        {
            return _dragSlot;
        }
        set
        {
            _dragSlot = value;
        }
    }
    [SerializeField]
    SkillSlot _dragSlot;


    void Awake()
    {
        instance = this;
        skillImage = GetComponent<Image>();
    }

    public void DragSetImage(Image _skillImage)
    {
        skillImage.sprite = _skillImage.sprite;
    }
    public void SetColor(float _alpha)
    {
        Color color = skillImage.color;
        color.a = _alpha;
        skillImage.color = color;
    }
}
