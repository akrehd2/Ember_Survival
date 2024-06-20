using UnityEngine;
using UnityEngine.EventSystems;

public class DragUi : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    private Transform targetTr; // 이동될 UI

    private Vector2 startingPoint;
    private Vector2 moveBegin;
    private Vector2 moveOffset;

    private void Awake()
    {
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (targetTr == null)
            targetTr = transform;
    }
    
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        startingPoint = targetTr.position;
        moveBegin = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        moveOffset = eventData.position - moveBegin;
        targetTr.position = startingPoint + moveOffset;
    }
}