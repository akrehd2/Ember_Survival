using UnityEngine;
using UnityEngine.EventSystems; // Drag관련 인터페이스 사용을 위함
using static UnityEngine.GraphicsBuffer;

// 드래그 시작, 드래그 중, 드래그가 끝날 이벤트 탐지를 위한 인퍼페이스 상속
public class IconDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //드래그 될때 이동되는 아이콘을 담는 static 변수
    //static으로 선언하는 이유는 Drop이벤트를 가진 슬롯 스크립트에서 접근하기 위함
    public static GameObject beingDraggedIcon;

    public ItemType itemType;

    // 슬롯이 아닌 다른 오브젝트에 Icon을 드랍할 경우 원복할 위치 백업용
    Vector3 startPosition;

    // Drag중 UI 레이어에 비정상적으로 보이기 때문에
    // Icon 드래그중 변경할 부모 RactTransfom 변수
    [SerializeField] Transform onDragParent;

    // 슬롯이 아닌 다른 오브젝트에 Icon을 드랍할 경우 원복할 부모 백업용
    [HideInInspector] public Transform startParent;

    private void Awake()
    {
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (onDragParent == null)
            onDragParent = UICtrl.instance.inventory.transform;
    }

    // 인터페이스 IBeginDragHandler를 상속 받았을 때 구현해야하는 콜백함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그가 시작될때 대상 Icon의 게임오브젝트를 static 변수에 할당
        beingDraggedIcon = gameObject;

        // 백업용 포지션과 부모 트랜스폼을 백업 해둔다.
        startPosition = transform.position;
        startParent = transform.parent;

        // Drop이벤트를 정상적으로 감지하기 위해 Icon RectTransform을 무시 
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // 드래그 시작할때 제일 위에 보이도록 부모 transform 변경
        transform.SetParent(UICtrl.instance.InGameUI.transform);
    }

    // 인터페이스 IDragHandler 상속 받았을 때 구현 해야하는 콜백함수
    public void OnDrag(PointerEventData eventData)
    {
        //드래그중에는 Icon을 마우스나 터치된 포인트의 위치로 이동시킨다.
        transform.position = Input.mousePosition;
    }

    // 인터페이스 IEndDragHandler 상속 받았을 때 구현 해야하는 콜백함수
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 대상을 지우고 해당 Icon에 이벤트감지를 허용한다.
        beingDraggedIcon = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        onDragParent = transform.parent.parent.parent;

        // 혹 드랍이벤트에 따라 부모가 변경되지 않고 
        // 이동중에 할당 되었던 부모 transform과 같다면
        // Icon의 부모와 위치를 원복한다.
        if (onDragParent == null)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }
    }
}

public enum ItemType
{
    Head,
    Body,
    Weapon,
    Sub,
    Shoes,
    Food,
    Ingredient,
    Quest,
    Etc
}