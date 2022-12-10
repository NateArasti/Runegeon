using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProvider : MonoBehaviour
{
    [SerializeField] private float m_Damage = 1;

    public float Damage => m_Damage;
}
