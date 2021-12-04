using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour ,IBeginDragHandler, IDragHandler , IEndDragHandler,IDropHandler
{
    public Image skillImage;
    public int skillId;


    //스킬사용버튼과 스킬슬롯 구분용
    public bool canDrop;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!canDrop)
        {
            DragSkillSlot.instance.dragSlot = this;
            DragSkillSlot.instance.DragSlotSet(skillImage,skillId);
            DragSkillSlot.instance.SetColor(1);
            DragSkillSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragSkillSlot.instance.dragSlot !=null && !canDrop)
        {
            DragSkillSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSkillSlot.instance.dragSlot ==null)
        {
            return;
        }

        if(canDrop)
        {
            skillImage.sprite = DragSkillSlot.instance.dragSlot.skillImage.sprite;
            skillId = DragSkillSlot.instance.dragSlot.skillId;
            SetColor(1);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSkillSlot.instance.SetColor(0);
        Debug.Log("end");
    }
    void SetColor(float _alpha)
    {
        Color color = skillImage.color;
        color.a = _alpha;
        skillImage.color = color;
    }
}
