using UnityEngine;

public class JsonSerializer : MonoBehaviour
{
    public static JsonSerializer Instance;

    public void saveObject<T>(T obj, string name)
    {
        PSaveable<T> save = new PSaveable<T>();
        save.SetObj(obj);
        string json = JsonUtility.ToJson(save);
        PlayerPrefs.SetString(name, json);
    }

    public T loadObject<T>(string name)
    {
        string json = PlayerPrefs.GetString(name);
        PSaveable<T> saved = JsonUtility.FromJson<PSaveable<T>>(json);
        T obj = saved.GetObj();
        return obj;
    }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
