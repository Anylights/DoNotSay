using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    private static TeleportManager _instance;
    public static TeleportManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TeleportManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("TeleportManager");
                    _instance = go.AddComponent<TeleportManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void DisableTeleportPoint(TeleportPoint point)
    {
        if (point != null)
        {
            point.isActive = false;
        }
    }

    public void EnableTeleportPoint(TeleportPoint point)
    {
        if (point != null)
        {
            point.isActive = true;
        }
    }
}
