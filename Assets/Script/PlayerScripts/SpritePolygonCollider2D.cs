using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class SpritePolygonCollider2D : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D silhouetteCollider;

    private readonly Dictionary<Sprite, Vector2[][]> pathsBySprite = new();
    private readonly List<Vector2> workPath = new();

    private SpriteRenderer spriteRenderer;
    private Sprite lastSprite;
    private bool lastFlipX;
    private bool lastFlipY;
    private bool synchronized;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (silhouetteCollider == null || !silhouetteCollider.isTrigger ||
            silhouetteCollider.transform.parent != transform ||
            silhouetteCollider.transform.localPosition != Vector3.zero ||
            silhouetteCollider.transform.localRotation != Quaternion.identity ||
            silhouetteCollider.transform.localScale != Vector3.one ||
            silhouetteCollider.offset != Vector2.zero)
        {
            Debug.LogError(
                "Asigna un PolygonCollider2D con Is Trigger en un hijo directo " +
                "con Transform restablecido y Offset (0, 0).", this);
            enabled = false;
            return;
        }

        spriteRenderer.RegisterSpriteChangeCallback(OnSpriteChanged);
        synchronized = false;
        Synchronize();
    }

    private void OnDisable()
    {
        spriteRenderer.UnregisterSpriteChangeCallback(OnSpriteChanged);

        if (synchronized && silhouetteCollider != null)
            silhouetteCollider.enabled = false;
        synchronized = false;
    }

    private void OnSpriteChanged(SpriteRenderer renderer)
    {
        Synchronize();
    }

    private void LateUpdate()
    {
        // flipX y flipY pueden cambiar sin que cambie la referencia del sprite.
        Synchronize();
    }

    private void Synchronize()
    {
        Sprite sprite = spriteRenderer.sprite;
        bool flipX = spriteRenderer.flipX;
        bool flipY = spriteRenderer.flipY;

        if (synchronized && sprite == lastSprite && flipX == lastFlipX && flipY == lastFlipY)
            return;

        lastSprite = sprite;
        lastFlipX = flipX;
        lastFlipY = flipY;
        synchronized = true;

        Vector2[][] paths = sprite != null ? GetPaths(sprite) : Array.Empty<Vector2[]>();
        if (paths.Length == 0)
        {
            silhouetteCollider.enabled = false;
            return;
        }

        if (silhouetteCollider.pathCount != paths.Length)
            silhouetteCollider.pathCount = paths.Length;

        for (int i = 0; i < paths.Length; i++)
        {
            Vector2[] path = paths[i];
            if (!flipX && !flipY)
            {
                silhouetteCollider.SetPath(i, path);
                continue;
            }

            bool reverse = flipX ^ flipY;
            workPath.Clear();

            for (int j = 0; j < path.Length; j++)
            {
                Vector2 point = path[reverse ? path.Length - 1 - j : j];
                workPath.Add(new Vector2(flipX ? -point.x : point.x, flipY ? -point.y : point.y));
            }

            silhouetteCollider.SetPath(i, workPath);
        }

        if (!silhouetteCollider.enabled)
            silhouetteCollider.enabled = true;
    }

    private Vector2[][] GetPaths(Sprite sprite)
    {
        if (pathsBySprite.TryGetValue(sprite, out Vector2[][] paths))
            return paths;

        paths = new Vector2[sprite.GetPhysicsShapeCount()][];
        for (int i = 0; i < paths.Length; i++)
        {
            workPath.Clear();
            if (sprite.GetPhysicsShape(i, workPath) < 3)
            {
                paths = Array.Empty<Vector2[]>();
                break;
            }

            paths[i] = workPath.ToArray();
        }

        pathsBySprite.Add(sprite, paths);

        if (paths.Length == 0)
            Debug.LogWarning($"El sprite '{sprite.name}' carece de una forma física válida.", this);

        return paths;
    }
}
