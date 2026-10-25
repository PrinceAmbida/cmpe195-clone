using System.Collections.Generic;
using UnityEngine;

public sealed class WorldScreenLabel : MonoBehaviour
{
    private static readonly List<Rect> occupied = new List<Rect>();
    private static int occupiedFrame = -1;

    private Transform target;
    private Camera viewCamera;
    private RectTransform canvasRect;
    private RectTransform labelRect;
    private CanvasGroup group;
    private Vector3 worldOffset;

    public void Initialize(Transform labelTarget, Camera camera, RectTransform rootRect, Vector3 offset)
    {
        target = labelTarget;
        viewCamera = camera;
        canvasRect = rootRect;
        labelRect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        worldOffset = offset;
    }

    void LateUpdate()
    {
        if (occupiedFrame != Time.frameCount)
        {
            occupied.Clear();
            occupiedFrame = Time.frameCount;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (viewCamera == null || canvasRect == null)
        {
            group.alpha = 0f;
            return;
        }

        Vector3 worldPosition = target.position + worldOffset;
        Vector3 viewport = viewCamera.WorldToViewportPoint(worldPosition);
        float distance = Vector3.Distance(viewCamera.transform.position, worldPosition);
        bool visible = distance <= 16f && viewport.z > 0.1f &&
                       viewport.x > 0.02f && viewport.x < 0.98f &&
                       viewport.y > 0.04f && viewport.y < 0.96f;
        if (!visible)
        {
            group.alpha = 0f;
            return;
        }

        Vector3 screenPosition = viewCamera.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, new Vector2(screenPosition.x, screenPosition.y), null, out Vector2 localPosition);

        Rect bounds = canvasRect.rect;
        Vector2 size = labelRect.rect.size;
        float minX = bounds.xMin + size.x * 0.5f + 12f;
        float maxX = bounds.xMax - size.x * 0.5f - 12f;
        float minY = bounds.yMin + Mathf.Min(150f, bounds.height * 0.21f);
        float maxY = bounds.yMax - size.y - Mathf.Min(90f, bounds.height * 0.12f);
        Vector2 desired = new Vector2(
            Mathf.Clamp(localPosition.x, minX, Mathf.Max(minX, maxX)),
            localPosition.y + 12f
        );

        for (int attempt = 0; attempt < 5; attempt++)
        {
            float verticalStep = size.y + 12f;
            float offset = attempt == 0 ? 0f : (attempt % 2 == 1 ? 1f : -1f) *
                           ((attempt + 1) / 2) * verticalStep;
            float y = Mathf.Clamp(desired.y + offset, minY, Mathf.Max(minY, maxY));
            Rect candidate = new Rect(desired.x - size.x * 0.5f - 6f, y - 6f,
                                      size.x + 12f, size.y + 12f);
            bool overlaps = false;
            foreach (Rect used in occupied)
            {
                if (candidate.Overlaps(used))
                {
                    overlaps = true;
                    break;
                }
            }
            if (overlaps)
            {
                continue;
            }

            occupied.Add(candidate);
            labelRect.anchoredPosition = new Vector2(desired.x, y);
            group.alpha = 1f;
            return;
        }

        group.alpha = 0f;
    }
}
