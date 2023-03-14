using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private float timer;

    protected void OnEnable()
    {
        OnObjectSpawn(timer);
    }

    protected void OnDisable()
    {
        OnObjectDestroy();
    }

    protected void OnObjectSpawn(float timer)
    {
        CancelInvoke();

        if (TryGetComponent(out Rigidbody2D rb2d))
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
        }

        if (timer >= 0)
        {
            Invoke(nameof(DestroyObject), timer);
        }
    }
    protected void OnObjectDestroy()
    {
        if (TryGetComponent(out Rigidbody2D rb2d))
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }

    public void SpawnObject()
    {
        SpawnObject(-1f);
    }

    public void SpawnObject(float timer)
    {
        SetTimer(timer);
        gameObject.SetActive(true);
    }

    public void SetTimer(float _timer)
    {
        this.timer = _timer;
    }
}
