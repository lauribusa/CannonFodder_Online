using UnityEngine;

public static class TransformExtension
{
    public static void SetPositionAndRotation(this Transform transform, Transform otherTransform) =>
        transform.SetPositionAndRotation(otherTransform.position, otherTransform.rotation);
}