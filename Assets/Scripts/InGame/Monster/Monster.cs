using UnityEngine;

public class Monster : MonoBehaviour
{
    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage}만큼 받음");
    }
}