using UnityEngine;

[DisallowMultipleComponent]
public class MenuEnemyDecoration : MonoBehaviour
{
    [SerializeField] private bool moveHorizontally;
    [SerializeField] private float distance = 18f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float phase;

    private RectTransform rectTransform;
    private Vector2 startingPosition;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            startingPosition = rectTransform.anchoredPosition;
        }
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }

        float offset = Mathf.Sin((Time.unscaledTime + phase) * speed) * distance;
        rectTransform.anchoredPosition = startingPosition + (moveHorizontally ? Vector2.right : Vector2.up) * offset;
    }

#if UNITY_EDITOR
    public void Configure(bool horizontal, float movementDistance, float movementSpeed, float movementPhase)
    {
        moveHorizontally = horizontal;
        distance = movementDistance;
        speed = movementSpeed;
        phase = movementPhase;
    }
#endif
}
