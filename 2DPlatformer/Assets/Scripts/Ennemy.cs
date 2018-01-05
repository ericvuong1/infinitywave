using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour {

    [System.Serializable]
    public class EnnemyStats
    {
        public int maxHealth = 100;

        private int _curHealth;
        public int curHealth
            {
                get{ return _curHealth; }
                set{ _curHealth = Mathf.Clamp(value, 0, maxHealth); }
            }

        public int damage = 40;

        public void Init()
        {
            curHealth = maxHealth;
        }

    }
    public EnnemyStats stats = new EnnemyStats();

    public Transform deathParticles;

    public float shakeAmt = 0.1f;
    public float shakeLength = 0.1f;

    [Header("Optional")] //structure inspector
    [SerializeField]
    private StatusIndicator statusIndicator;

    private void Start()
    {
        stats.Init();
        if(statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
        if (deathParticles == null)
        {
            Debug.LogError("No ennemy death particles!");
        }
    }

    public void DamageEnnemy(int damage)
    {
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            Debug.Log("KILL PLAYER");
            GameMaster.KillEnnemy(this);
        }
        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
    }


    private void OnCollisionEnter2D(Collision2D _colInfo)
    {
        Player _player = _colInfo.collider.GetComponent<Player>();
        if (_player != null)
        {
            _player.DamagePlayer(stats.damage);
            DamageEnnemy(9999999);
        }
    }
}
