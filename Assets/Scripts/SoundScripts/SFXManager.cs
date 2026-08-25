using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip automaticShotSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip grenadeSound;
    [SerializeField] private AudioClip gruntSound;
    [SerializeField] private AudioClip individualShotSound;
    [SerializeField] private AudioClip maxHealthSound;
    [SerializeField] private AudioClip onClickSound;
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip punchSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip startGameSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAutomaticShot()
    {
        audioSource.PlayOneShot(automaticShotSound);
    }

    public void PlayGameOver()
    {
        audioSource.PlayOneShot(gameOverSound);
    }

    public void PlayExplosion()
    {
        audioSource.PlayOneShot(explosionSound);
    }

    public void PlayGrenade()
    {
        audioSource.PlayOneShot(grenadeSound);
    }

    public void PlayGrunt()
    {
        audioSource.PlayOneShot(gruntSound);
    }
    
    public void PlayIndividualShot()
    {
        audioSource.PlayOneShot(individualShotSound);
    }
    
    public void PlayMaxHealth()
    {
        audioSource.PlayOneShot(maxHealthSound);
    }
    
    public void PlayOnClick()
    {
        audioSource.PlayOneShot(onClickSound);
    }

    public void PlayPause()
    {
        audioSource.PlayOneShot(pauseSound);
    }

    public void PlayPowerUp()
    {
        audioSource.PlayOneShot(powerUpSound);
    }
    
    public void PlayPunch()
    {
        audioSource.PlayOneShot(punchSound);
    }

    public void PlayReload()
    {
        audioSource.PlayOneShot(reloadSound);
    }

    public void PlayStartGame()
    {
        audioSource.PlayOneShot(startGameSound);
    }

/* para usar los sonidos en código vamos a:

AudioManager.Instance.sfxManager.PlayAutomaticShot();
AudioManager.Instance.sfxManager.PlayGameOver();
AudioManager.Instance.sfxManager.PlayExplosion();
AudioManager.Instance.sfxManager.PlayGrenade();
AudioManager.Instance.sfxManager.PlayGrunt();
AudioManager.Instance.sfxManager.PlayIndividualShot();
AudioManager.Instance.sfxManager.PlayMaxHealth();
AudioManager.Instance.sfxManager.PlayOnClick();
AudioManager.Instance.sfxManager.PlayPause();
AudioManager.Instance.sfxManager.PlayPowerUp();
AudioManager.Instance.sfxManager.PlayPunch();
AudioManager.Instance.sfxManager.PlayReload();
AudioManager.Instance.sfxManager.PlayStartGame();

*/

}   
