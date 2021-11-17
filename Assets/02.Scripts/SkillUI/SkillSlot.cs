using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour ,IBeginDragHandler, IDragHandler , IEndDragHandler,IDropHandler
{
    public Image skillImage;

    //스킬사용버튼과 스킬슬롯 구분용
    public bool canDrop;

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragSkillSlot.instance.dragSlot = this;
        DragSkillSlot.instance.DragSetImage(skillImage);
        DragSkillSlot.instance.SetColor(1);
        DragSkillSlot.instance.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragSkillSlot.instance.dragSlot !=null)
        {
            DragSkillSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("drop");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSkillSlot.instance.SetColor(0);
        Debug.Log("end");
    }
}
