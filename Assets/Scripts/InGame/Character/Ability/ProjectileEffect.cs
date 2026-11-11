using System;
using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    private Vector2 _direction;
    private float _speed;
    private float _remainTime;
    private Action _onFinished;
    private bool _active;

    public void Play(Vector2 direction, float speed, float lifetime, Action onFinished)
    {
        _active = true;
        _direction = direction;
        _speed = speed;
        _remainTime = lifetime;
        _onFinished += onFinished;
    }

    private void Update()
    {
        if (_remainTime <= 0f) return;

        transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
        _remainTime -= Time.deltaTime;

        if (_remainTime <= 0f)
        {
            Finish();
        }
    }
    public void Finish()
    {
        if (_active == false)
            return;

        _active = false;
        _remainTime = 0f;
        _onFinished?.Invoke();
        _onFinished = null;
        GameObjectManager.Instance.RequestDestroyObject(gameObject);
    }
}
