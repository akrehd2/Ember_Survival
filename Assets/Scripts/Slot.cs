using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems; // IDropHandler 상속을 위한
public class Slot : MonoBehaviour, IDropHandler // 드랍이벤트 감지를 위한 상속
{
    GameObject Icon()
    {
        // 슬롯에 아이템(자식 트랜스폼)이 있으면 아이템의 gameobject를 리턴
        // 슬롯에 아이템(자식 트랜스폼)이 없다면 null을 리턴
        if (transform.childCount > 0)
            return transform.GetChild(0).gameObject;
        else
            return null;
    }

    // IDropHandler 인터페이스 상속시 구현해야 되는 콜백 함수
    // 이 스크립트가 컨포넌트로 추가 된 게임 오브젝트 RactTransform내에
    // 포인터 드랍이 발생하면 실행되는 콜백함수
    public void OnDrop(PointerEventData eventData)
    {
        // 슬롯이 비어있다면 Icon을 자식으로 추가 위치변경 함.
        if (Icon() == null)
        {
            if (IconDrag.beingDraggedIcon != null)
            {
                if (gameObject.name == "Slot")
                {
                    IconDrag.beingDraggedIcon.transform.SetParent(transform);
                    IconDrag.beingDraggedIcon.transform.position = transform.position;
                }
                else
                {
                    string slotName = gameObject.name.Split('S')[0];

                    if (IconDrag.beingDraggedIcon.GetComponent<IconDrag>().itemType.ToString() == slotName)
                    {
                        IconDrag.beingDraggedIcon.transform.SetParent(transform);
                        IconDrag.beingDraggedIcon.transform.position = transform.position;
                    }
                }
            }
        }
    }
}