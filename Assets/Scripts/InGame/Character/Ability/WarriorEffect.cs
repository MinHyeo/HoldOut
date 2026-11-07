using System.Runtime.Serialization;
using UnityEngine;

public class WarriorEffect : MonoBehaviour
{
    private float _startAngle;
    private float _endAngle;
    private float _duration;
    private float _elapsed;

    public void Play(Vector2 direction, float angle, float duration)
    {
        float halfAngle = angle / 2;
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        _startAngle = centerAngle + halfAngle;
        _endAngle = centerAngle - halfAngle;
        _duration = duration;
        _elapsed = 0f;

        transform.rotation = Quaternion.Euler(0f, 0f, _startAngle);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / _duration);

        float eased = 1f - (1f - t) * (1f - t);
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(_startAngle, _endAngle, eased));

        if (t >= 1f)
        {
            GameObjectManager.Instance.RequestDestroyObject(gameObject);
        }
    }
}