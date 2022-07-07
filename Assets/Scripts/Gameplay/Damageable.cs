using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

// !! Execution order modified, just after default time (trigger events after other scripts have subscribed..), 
// we cannot guarantee execution order Awake and OnEnable between different gameobjects
public class Damageable : NetworkBehaviour
{
    [SerializeField] float _maxHealth = 10;

    NetworkVariable<float> _currentHealth = new NetworkVariable<float>();

    public static event Action<Damageable> OnDamageableSpawn, OnDamageableDespawn;
    public UnityEvent OnDamageTaken, OnDeath;

    public float CurrentHealth => _currentHealth.Value;
    public float MaxHealth => _maxHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) _currentHealth.Value = _maxHealth;
        OnDamageableSpawn?.Invoke(this);
    }

    public override void OnNetworkDespawn()
    {
        OnDamageableDespawn?.Invoke(this);
        base.OnNetworkDespawn();
    }

    public void TakeDamage(int damageValue)
    {
        if (!IsServer) return;

        _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - damageValue, 0, _maxHealth);

        TriggerOnDamageTakenClientRpc();

        if (_currentHealth.Value <= 0)
        {
            TriggerOnDeathClientRpc();
            Destroy(this.gameObject);
        }
    }

    [ClientRpc]
    public void TriggerOnDeathClientRpc()
    {
        OnDeath?.Invoke();
    }

    [ClientRpc]
    public void TriggerOnDamageTakenClientRpc()
    {
        OnDamageTaken?.Invoke();
    }
}
