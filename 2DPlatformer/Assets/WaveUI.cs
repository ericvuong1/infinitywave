using UnityEngine;
using UnityEngine.UI;
public class WaveUI : MonoBehaviour {



    [SerializeField]
    WaveSpawner spawner;
    [SerializeField]
    Animator waveAnimator;
    [SerializeField]
    Text waveCountdownText;
    [SerializeField]
    Text waveNumberText;

    private WaveSpawner.SpawnState previousState;



	// Use this for initialization
	void Start () {
        if (spawner == null || waveAnimator == null || waveCountdownText == null || waveNumberText == null)
        {
            Debug.LogError("Not properly referenced (spawner, waveAnimator, waveCountdown, waveNumberText)");
            this.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {

        switch(spawner.State)
        {
            case WaveSpawner.SpawnState.COUNTING:
                UpdateCountingUI();
                break;
            case WaveSpawner.SpawnState.SPAWNING:
                UpdateSpawninggUI();
                break;
        }
        previousState = spawner.State;
	}
    void UpdateCountingUI()
    {
        if (previousState != WaveSpawner.SpawnState.COUNTING)
        {
            waveAnimator.SetBool("WaveCountdown", true); //trigger the animator
            waveAnimator.SetBool("WaveIncoming", false); 
        }
        waveCountdownText.text = ((int)spawner.WaveCountdown).ToString();
    }
    void UpdateSpawninggUI()
    {
        if (previousState != WaveSpawner.SpawnState.SPAWNING)
        {
            waveAnimator.SetBool("WaveCountdown", false); //trigger the animator
            waveAnimator.SetBool("WaveIncoming", true);

            waveNumberText.text = spawner.NextWave.ToString();
        }
    }
}
