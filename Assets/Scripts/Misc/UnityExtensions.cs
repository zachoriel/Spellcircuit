using UnityEngine;
using UnityEngine.Assertions;

public static class UnityExtensions
{
    #region TryGetComponentInChildren

    // Extension method for GameObject
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>();
        Assert.IsNotNull(component, $"Could not find component of type {typeof(T)} in children of {gameObject.name}");
        return component != null;
    }

    // Overload for including inactive components
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>(includeInactive);
        Assert.IsNotNull(component, $"Could not find component of type {typeof(T)} in children of {gameObject.name}");
        return component != null;
    }

    // Extension method for Transform
    public static bool TryGetComponentInChildren<T>(this Transform transform, out T component) where T : Component
    {
        component = transform.GetComponentInChildren<T>();
        Assert.IsNotNull(component, $"Could not find component of type {typeof(T)} in children of {transform.name}");
        return component != null;
    }

    // Overload for including inactive components
    public static bool TryGetComponentInChildren<T>(this Transform transform, out T component, bool includeInactive) where T : Component
    {
        component = transform.GetComponentInChildren<T>(includeInactive);
        Assert.IsNotNull(component, $"Could not find component of type {typeof(T)} in children of {transform.name}");
        return component != null;
    }

    #endregion
}