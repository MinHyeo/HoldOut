using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    private Queue<CharacterInfoSlot> _spawned = new Queue<CharacterInfoSlot>();
    private List<CharacterInfoSlot> _actived = new List<CharacterInfoSlot>();

    private void OnEnable()
    {
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            CharacterInfoSlot slot;
            if (_spawned.Count <= 0)
            {
                GameObject slotObject = Instantiate(_slotPrefab, transform);
                slot = slotObject.GetComponent<CharacterInfoSlot>();
            }
            else
            {
                slot = _spawned.Dequeue();
                slot.gameObject.SetActive(true);
            }
            _actived.Add(slot);
            slot.SetSlot(client);
        }
    }

    private void OnDisable()
    {
        foreach(var slot in _actived)
        {
            slot.gameObject.SetActive(false);
            _spawned.Enqueue(slot);
        }
        _actived.Clear();
    }
}
