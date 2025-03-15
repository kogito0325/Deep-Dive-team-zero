using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image background;  // ���̽�ƽ ��� �̹���
    public Image handle;     // ���̽�ƽ ������ �̹���
    public float moveRange = 50f; // ���̽�ƽ �̵� ����
    public Vector2 inputVector; // ���̽�ƽ �Է� ����

    private Vector2 startPosition;  // ��ġ ���� ��ġ

    void Start()
    {
        startPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // ��ġ ���� �� �巡�� ó��
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ��ġ ��ġ�� ���̽�ƽ �߾� ��ġ ���� �Ÿ� ���
        Vector2 touchPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, eventData.position, eventData.enterEventCamera, out touchPos);

        // ���̽�ƽ ������ �̵�
        Vector2 delta = touchPos - startPosition;
        delta = Vector2.ClampMagnitude(delta, moveRange); // �̵� ���� ����
        handle.rectTransform.anchoredPosition = delta;

        // �Է� ���� ���
        inputVector = delta / moveRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.rectTransform.anchoredPosition = startPosition;
        inputVector = Vector2.zero;
    }
}