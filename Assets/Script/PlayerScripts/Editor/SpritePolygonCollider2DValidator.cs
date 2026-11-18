using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal static class SpritePolygonCollider2DValidator
{
    [MenuItem("CONTEXT/SpritePolygonCollider2D/Validar sprites del Animator")]
    private static void ValidateSprites(MenuCommand command)
    {
        var component = (SpritePolygonCollider2D)command.context;
        Animator animator = component.GetComponentInParent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("No hay un Animator con controlador para validar.", component);
            return;
        }

        string rendererPath = AnimationUtility.CalculateTransformPath(component.transform, animator.transform);
        var sprites = new HashSet<Sprite>();
        var clips = new HashSet<AnimationClip>();
        int emptyKeys = 0;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (!clips.Add(clip))
                continue;

            var clipSprites = new HashSet<Sprite>();
            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                if (binding.type != typeof(SpriteRenderer) || binding.propertyName != "m_Sprite" ||
                    binding.path != rendererPath)
                    continue;

                foreach (ObjectReferenceKeyframe key in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                {
                    if (key.value is Sprite sprite)
                        clipSprites.Add(sprite);
                    else
                        emptyKeys++;
                }
            }

            sprites.UnionWith(clipSprites);
            Debug.Log($"{clip.name}: {clipSprites.Count} sprites distintos en este SpriteRenderer.", component);
        }

        Sprite initialSprite = component.GetComponent<SpriteRenderer>().sprite;
        if (initialSprite != null)
            sprites.Add(initialSprite);

        var points = new List<Vector2>();
        int invalidSprites = 0;

        foreach (Sprite sprite in sprites)
        {
            if (HasValidPaths(sprite, points))
                continue;

            invalidSprites++;
            Debug.LogWarning($"Sin forma física válida: {AssetDatabase.GetAssetPath(sprite)} / {sprite.name}", sprite);
        }

        string result = $"{sprites.Count} sprites inspeccionados; " +
            $"{invalidSprites} sin forma válida y {emptyKeys} claves sin sprite.";
        if (sprites.Count == 0 || invalidSprites != 0 || emptyKeys != 0)
            Debug.LogWarning(result, component);
        else
            Debug.Log(result, component);
    }

    private static bool HasValidPaths(Sprite sprite, List<Vector2> points)
    {
        int count = sprite.GetPhysicsShapeCount();
        for (int i = 0; i < count; i++)
        {
            points.Clear();
            if (sprite.GetPhysicsShape(i, points) < 3)
                return false;
        }

        return count > 0;
    }
}
