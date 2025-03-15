using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image background;  // 조이스틱 배경 이미지
    public Image handle;     // 조이스틱 손잡이 이미지
    public float moveRange = 50f; // 조이스틱 이동 범위
    public Vector2 inputVector; // 조이스틱 입력 벡터

    private Vector2 startPosition;  // 터치 시작 위치

    void Start()
    {
        startPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // 터치 시작 시 드래그 처리
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 터치 위치와 조이스틱 중앙 위치 간의 거리 계산
        Vector2 touchPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, eventData.position, eventData.enterEventCamera, out touchPos);

        // 조이스틱 손잡이 이동
        Vector2 delta = touchPos - startPosition;
        delta = Vector2.ClampMagnitude(delta, moveRange); // 이동 범위 제한
        handle.rectTransform.anchoredPosition = delta;

        // 입력 벡터 계산
        inputVector = delta / moveRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.rectTransform.anchoredPosition = startPosition;
        inputVector = Vector2.zero;
    }
}