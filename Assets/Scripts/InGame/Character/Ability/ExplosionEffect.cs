using UnityEngine;
public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _duration = 0.3f;

    private float _targetScale;
    private float _elapsed;
    private bool _active;

    public void Play(float radius)
    {
        _targetScale = radius * 2f;
        _elapsed = 0f;
        _active = true;

        transform.localScale = Vector3.zero;
        SetAlpha(1f);
    }

    private void Update()
    {
        if (!_active)
            return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / _duration);

        // 빠르게 커지고, 커진 뒤 서서히 투명해짐
        float scale = Mathf.Lerp(0f, _targetScale, 1f - (1f - t) * (1f - t));
        transform.localScale = new Vector3(scale, scale, 1f);
        SetAlpha(1f - t);

        if (t >= 1f)
        {
            _active = false;
            GameObjectManager.Instance.RequestDestroyObject(gameObject);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (_sprite == null)
            return;

        Color color = _sprite.color;
        color.a = alpha;
        _sprite.color = color;
    }
}