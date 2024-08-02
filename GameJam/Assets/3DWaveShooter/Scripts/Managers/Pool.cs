using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Used to create and destroy objects efficiently. Spawns then in at the start of the game and can create them on the go.
/// </summary>
public class Pool : MonoBehaviour
{
    public List<PoolContainer> objects = new List<PoolContainer>();

    //Instance
    public static Pool inst;
    void Awake () { inst = this; }

    void Start ()
    {
        SpawnObjectsOnStart();
    }

    //Create all the predefined objects for the pool.
    void SpawnObjectsOnStart ()
    {
        for(int i = 0; i < objects.Count; ++i)
        {
            for(int x = 0; x < objects[i].instancesToCreate; ++x)
            {
                InstantiateNewObjectToPool(objects[i]).SetActive(false);
            }
        }
    }

    //Create a set number of a ceratin object, adding them to the pool.
    public void SpawnSetOfPoolObjects (GameObject objectToSpawn, int amount)
    {
        //Find the container.
        PoolContainer container = Pool.inst.objects.Find(x => x.objPrefab.name == objectToSpawn.name);

        //Container doesn't exist? Then create a new one.
        if(container == null)
            container = CreateNewPoolContainer(objectToSpawn);

        //Spawn the amount of objects.
        for(int i = 0; i < amount; ++i)
        {
            InstantiateNewObjectToPool(container).SetActive(false);
        }
    }

    //Spawns requested object (acts like Instantiate on the surface).
    public static GameObject Spawn (GameObject objectToSpawn, Vector3 position, Quaternion rotation)
    {
        return Pool.inst.SpawnObject(objectToSpawn, position, rotation, null);
    }

    //Spawns requested object (acts like Instantiate on the surface).
    public static GameObject Spawn (GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Pool.inst.SpawnObject(objectToSpawn, position, rotation, parent);
    }

    //Spawns requested object (acts like Instantiate on the surface).
    public GameObject SpawnObject (GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent)
    {
        //PoolContainer container = Pool.inst.objects.Find(x => PrefabUtility.GetPrefabObject(objectToSpawn) == x.objPrefab);
        PoolContainer container = Pool.inst.objects.Find(x => x.objPrefab.name == objectToSpawn.name);

        //If the object is in the pool, enable it and set values.
        if(container != null)
        {
            GameObject obj = null;

            if(container.usableInstances.Count > 0)
                obj = container.usableInstances[0];
            else
                obj = InstantiateNewObjectToPool(container);

            container.usableInstances.Remove(obj);

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.parent = parent;

            obj.SetActive(true);

            return obj;
        }
        //Otherwise create new pool container and instantiate object.
        else
        {
            PoolContainer newContainer = Pool.inst.CreateNewPoolContainer(objectToSpawn);
            GameObject obj = Pool.inst.InstantiateNewObjectToPool(newContainer);

            newContainer.usableInstances.Remove(obj);

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.parent = parent;

            return obj;
        }
    }

    //If a container has run out of objects to spawn, Instantiate a new one.
    public GameObject InstantiateNewObjectToPool (PoolContainer container)
    {
        GameObject obj = Instantiate(container.objPrefab);
        container.allInstances.Add(obj);
        container.usableInstances.Add(obj);

        obj.name = container.objPrefab.name;
        obj.transform.parent = transform;

        return obj;
    }

    //Creates a new pool container with the sent object prefab.
    public PoolContainer CreateNewPoolContainer (GameObject objectToSpawn)
    {
        PoolContainer newObjContainer = new PoolContainer(objectToSpawn);
        Pool.inst.objects.Add(newObjContainer);

        return newObjContainer;
    }

    //Disables object, or destroys it if it's not in the pool with a timed delay.
    public static void Destroy (GameObject objectToDestroy, float time)
    {
        Pool.inst.StartCoroutine(Pool.inst.DestroyDelay(objectToDestroy, time));
    }

    //Waits the delay, then destroys the object.
    public IEnumerator DestroyDelay (GameObject objectToDestroy, float time)
    {
        yield return new WaitForSeconds(time);
        Pool.Destroy(objectToDestroy);
    }

    //Disables object, or destroys it if it's not in the pool.
    public static void Destroy (GameObject objectToDestroy)
    {
        //PoolContainer container = Pool.inst.objects.Find(x => PrefabUtility.GetPrefabObject(objectToDestroy) == x.objPrefab);
        PoolContainer container = Pool.inst.objects.Find(x => x.objPrefab.name == objectToDestroy.name);

        //If the object is in the pool, disable it.
        if(container != null)
        {
            objectToDestroy.SetActive(false);
            objectToDestroy.transform.parent = Pool.inst.transform;
            container.usableInstances.Add(objectToDestroy);
        }
        //Otherwise destroy it.
        else
        {
            Object.Destroy(objectToDestroy);
        }
    }
}

[System.Serializable]
public class PoolContainer
{
    public GameObject objPrefab;
    public int instancesToCreate;

    public List<GameObject> allInstances = new List<GameObject>();
    public List<GameObject> usableInstances = new List<GameObject>();

    public PoolContainer (GameObject objPrefab)
    {
        this.objPrefab = objPrefab;
    }
}