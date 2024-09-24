#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public static class ObjectPoolManager
{
    readonly static List<ObjectPool> objectPools = new ();

    static Transform objectPoolParent;

    // Dictionary to cache the object pools by prefab for faster lookup.
    public readonly static Dictionary<GameObject, ObjectPool> ObjectPoolLookup = new ();
    public static Transform ObjectPoolParent
    {
        get
        {
            if (objectPoolParent == null)
            {
                var objectPoolParentGameObject = new GameObject("--- Object Pools ---");
                objectPoolParent = objectPoolParentGameObject.transform;
            }

            return objectPoolParent;
        }
    }

    // List of all objects in every pool.
    public static List<GameObject> AllPooledObjects
    {
        get
        {
            List<GameObject> allPooledObjects = new ();
            foreach (ObjectPool objectPool in objectPools) { allPooledObjects.AddRange(objectPool.pooledObjects); }
            return allPooledObjects;
        }
    }

    public static List<GameObject> AllActivePooledObjects
    {
        get
        {
            List<GameObject> allActivePooledObjects = new ();
            foreach (ObjectPool objectPool in objectPools) { allActivePooledObjects.AddRange(objectPool.pooledObjects.FindAll(pooledObject => pooledObject.activeInHierarchy)); }
            return allActivePooledObjects;
        }
    }

    /// <summary>
    ///     Adds an existing pool to the list of object pools.
    /// </summary>
    /// <param name="objectPool"></param>
    public static void AddExistingPool(ObjectPool objectPool)
    {
        if (objectPool == null)
        {
            Debug.LogError("Object pool cannot be null!");
            return;
        }

        objectPools.Add(objectPool);
        objectPool.transform.parent = ObjectPoolParent;
    }

    /// <summary>
    ///     Creates a new object pool as a new gameobject.
    /// </summary>
    /// <param name="objectPrefab"> The object prefab to pool. </param>
    /// <param name="startAmount"> The number of objects to instantiate at the start. </param>
    /// <param name="parent"> The parent transform for the new pool. </param>
    /// <returns>The pool that was created.</returns>
    public static ObjectPool CreateNewPool(GameObject objectPrefab, int startAmount = 5)
    {
        if (objectPrefab == null)
        {
            Debug.LogError("Object prefab cannot be null!");
            return null;
        }

        var newObjectPool = new GameObject().AddComponent<ObjectPool>();
        newObjectPool.SetUpPool(objectPrefab, startAmount);

        return newObjectPool;
    }

    /// <summary>
    ///     Returns the pool containing the specified object prefab.
    ///     Creates and returns a new pool if none is found.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <returns></returns>
    public static ObjectPool FindObjectPool(GameObject objectPrefab)
    {
        if (objectPrefab == null)
        {
            Debug.LogError("Object prefab cannot be null!");
            return null;
        }

        if (ObjectPoolLookup.TryGetValue(objectPrefab, out ObjectPool objectPool)) return objectPool;

        //Debug.LogWarning("That object is NOT yet pooled! Creating a new pool...");
        objectPool                     = CreateNewPool(objectPrefab);
        ObjectPoolLookup[objectPrefab] = objectPool;
        return objectPool;
    }
}
