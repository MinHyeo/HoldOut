using System.Collections;
using TMPro;
using UnityEngine;

public class ErrorMessageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] private float _shakeDuration = 0.4f;
    [SerializeField] private float _shakeStrength = 8f;
    [SerializeField] private int _vibrato = 25;

    private RectTransform _rect;
    private Vector2 _originalPos;
    private Coroutine _shakeRoutine;

    private void Awake()
    {
        _rect = _text.rectTransform;
        _originalPos = _rect.anchoredPosition;

        gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        gameObject.SetActive(true);
        _text.text = message;

        if(_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
        }

        _rect.anchoredPosition = _originalPos;
        _shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while(elapsed < _shakeDuration)
        {
            elapsed += Time.deltaTime;
            float damper = 1f - (elapsed / _shakeDuration);
            float offsetX = Mathf.Sin(elapsed * _vibrato) * _shakeStrength * damper;
            _rect.anchoredPosition = _originalPos + new Vector2(offsetX, 0f);
            yield return null;
        }

        _rect.anchoredPosition = _originalPos;
        _shakeRoutine = null;
        gameObject.SetActive(false);
    }
}
