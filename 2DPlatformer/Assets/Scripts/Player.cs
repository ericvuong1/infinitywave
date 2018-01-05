using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 100;
        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }
        public void Init()
        {
            curHealth = maxHealth;
        }


    }
    public PlayerStats playerStats = new PlayerStats();
    public int fallBoundary = -20;


    [SerializeField]
    private StatusIndicator statusIndicator;

    private void Start()
    {
        playerStats.Init();

        if (statusIndicator == null)
        {
            Debug.LogError("No status indicator referenced on Player");
        }
        else
        {
            statusIndicator.SetHealth(playerStats.curHealth, playerStats.maxHealth);
        }
    }

    private void Update()
    {
        if(transform.position.y <= fallBoundary)
        {
            DamagePlayer(99999);
        }
    }

    public void DamagePlayer(int damage)
    {
        playerStats.curHealth -= damage;
        if (playerStats.curHealth <= 0)
        {
            if(this.tag == "Player") {
                Debug.Log("KILL PLAYER");
                GameMaster.KillPlayer(this);
            }else if(this.tag == "Player2")
            {
                Debug.Log("KILL PLAYER");
                GameMaster.KillPlayer2(this);
            }
            
        }
        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(playerStats.curHealth, playerStats.maxHealth);
        }
    }


}
