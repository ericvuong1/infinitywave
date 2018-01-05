using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster gm;

    private void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
        }
    }

    public Transform playerPrefab;
    public Transform playerPrefab2;
    public Transform spawnPoint;
    public float spawnDelay = 3.5f;
    public Transform spawnPrefab;

    public Transform ennemyDeathParticles;

    public CameraShake cameraShake;

    private void Start()
    {
        if (cameraShake == null)
        {
            Debug.LogError("No camera shake referenced in GameMaster.");
        }
    }

    public IEnumerator RespawnPlayer()
    {
        GetComponent<AudioSource>().Play();
        Debug.Log("TODO: Add waiting for spawn sound");
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(clone.gameObject, 3f);
    }
    public IEnumerator RespawnPlayer2()
    {
        GetComponent<AudioSource>().Play();
        Debug.Log("TODO: Add waiting for spawn sound");
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(playerPrefab2, spawnPoint.position, spawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(clone.gameObject, 3f);
    }
    public static void KillPlayer(Player player){
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer());
            }
    public static void KillPlayer2(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer2());
    }
    public static void KillEnnemy(Ennemy ennemy)
    {
        gm._KillEnnemy(ennemy);
    }
    public void _KillEnnemy(Ennemy _ennemy)
    {
        Transform _clone = (Transform) Instantiate(_ennemy.deathParticles, _ennemy.transform.position, Quaternion.identity);
        cameraShake.ShakeCamera(_ennemy.shakeAmt, _ennemy.shakeLength);
        Destroy(_ennemy.gameObject);
        Destroy(_clone.gameObject, 5f);

    }

}
