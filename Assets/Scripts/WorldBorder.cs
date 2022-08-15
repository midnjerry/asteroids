using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class WorldBorder : MonoBehaviour
{
    public Camera mainCamera;
    BoxCollider2D boxCollider;

    public UnityEvent<Collider2D> outOfBoundsEvent;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void Start()
    {
        transform.position = Vector3.zero;
        UpdateBorderSize();
    }

    public void UpdateBorderSize()
    {
        float height = mainCamera.orthographicSize * 2;
        Vector2 boxColliderSize = new Vector2(height * mainCamera.aspect, height);
        boxCollider.size = boxColliderSize;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        outOfBoundsEvent?.Invoke(collision);
    }

    public bool IsOutOfBounds(Vector3 position)
    {
        return Mathf.Abs(position.x) > boxCollider.bounds.max.x ||
            Mathf.Abs(position.y) > boxCollider.bounds.max.y;
    }

    public Vector2 CalculateWrappedPosition(Vector2 position)
    {
        bool isOutsideX = Mathf.Abs(position.x) > boxCollider.bounds.max.x;
        bool isOutsideY = Mathf.Abs(position.y) > boxCollider.bounds.max.y;
        float x = position.x * (isOutsideX ? -1 : 1);
        float y = position.y * (isOutsideY ? -1 : 1);
        return new Vector2(x, y);
    }
}
