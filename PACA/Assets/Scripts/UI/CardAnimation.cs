using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardTiltClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("弹出动画")]
    public float popTime = 0.6f;
    public float rotateY = 18f;

    [Header("鼠标倾斜设置")]
    public float tiltAmount = 15f;
    public float tiltSmooth = 8f;
    public float hoverScale = 1.05f;

    [Header("关闭动画")]
    public float closeTime = 0.35f;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    private bool _isClosing;
    private bool _isHovering;
    private Vector3 _targetRot;
    private Image _img;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _img = GetComponent<Image>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (_img != null)
            _img.raycastTarget = true;
    }

    void Start()
    {
        PlayPopAnim();
    }

    void PlayPopAnim()
    {
        _isClosing = false;
        _rect.localScale = Vector3.zero;
        _rect.rotation = Quaternion.Euler(0, rotateY, 0);
        _canvasGroup.alpha = 0f;

        _rect.DOScale(1f, popTime).SetEase(Ease.OutBack);
        _rect.DORotate(Vector3.zero, popTime).SetEase(Ease.OutQuad);
        _canvasGroup.DOFade(1f, popTime);
    }

    void Update()
    {
        if (_isClosing) return;

        if (_isHovering)
        {
            // 兼容新旧输入系统 —— 修复报错
            Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rect,
                mousePos,
                null, // UIOverlay模式传null即可
                out Vector2 localPoint
            );

            float x = localPoint.x / (_rect.rect.width * 0.5f);
            float y = localPoint.y / (_rect.rect.height * 0.5f);

            x = Mathf.Clamp(x, -1f, 1f);
            y = Mathf.Clamp(y, -1f, 1f);

            _targetRot = new Vector3(-y * tiltAmount, x * tiltAmount, 0);
            _rect.localRotation = Quaternion.Lerp(_rect.localRotation, Quaternion.Euler(_targetRot), Time.deltaTime * tiltSmooth);
        }
        else
        {
            // 自动回归原位
            _rect.localRotation = Quaternion.Lerp(_rect.localRotation, Quaternion.identity, Time.deltaTime * tiltSmooth);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        _rect.DOScale(hoverScale, 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        _rect.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClosing) return;
        CloseCardAnim();
    }

    void CloseCardAnim()
    {
        _isClosing = true;
        _rect.DOScale(0f, closeTime).SetEase(Ease.InBack);
        _canvasGroup.DOFade(0f, closeTime).OnComplete(() => Destroy(gameObject));
    }
}