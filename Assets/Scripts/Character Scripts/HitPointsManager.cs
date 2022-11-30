using UnityEngine;
using TMPro;

public class HitPointsManager : MonoBehaviour
{
    public HealthBar healthBar;
    public bool useThisScriptToDestroy;
    public TextMeshProUGUI hitPointsText;
    public int maxHitPoints = 10;
    public int currentHitPoints;
    public ParticleSystem hitEffect;
    public ParticleSystem deathEffect;
    private Camera mainCamera;
    public TextMeshProUGUI entityName;
    private AudioSource audioSource;
    public AudioClip gotHitSound;
    //public AudioClip deathSound;

    void Start()
    {
        currentHitPoints = maxHitPoints;
        audioSource = GetComponentInParent<AudioSource>();

        mainCamera = Camera.main;

        // DEBUG: Show entity name.
        if (entityName != null)
        {
            entityName.text = this.name.ToString();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHitPoints);
        }
    }

    private void Update()
    {
        // DEBUG: Show HP text in game.
        if (hitPointsText != null)
        {
            hitPointsText.text = currentHitPoints.ToString();
            hitPointsText.transform.LookAt(mainCamera.transform);
            hitPointsText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }

        // DEBUG: Show entity name.
        if (entityName != null)
        {
            entityName.transform.LookAt(mainCamera.transform);
            entityName.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }

    public void RegisterHit(int damage)
    {
        Debug.Log(this.name + " got hit. HP=" + currentHitPoints);
        currentHitPoints -= damage;

        // Play a hit effect if the character was hit.
        // NOTE that HEALING is also performed through the RegisterHit function (damage = -1),
        // Therefore only play the effect if damage > 0.
        if (hitEffect != null && currentHitPoints > 0 && damage > 0)
        {
            Debug.Log(this.name + ": hit effect spawned");
            PlayEffect(hitEffect);
            AudioSource.PlayClipAtPoint(gotHitSound, transform.position);
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHitPoints);
        }

        if (currentHitPoints <= 0 && useThisScriptToDestroy == true)
        {
            Die();
        }
    }

    public void Die()
    {
        //Debug.Log(this.name + " died.");
        //AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (deathEffect != null)
        {
            PlayEffect(deathEffect);
        }
        Destroy(this.gameObject);
    }

    public void PlayEffect(ParticleSystem effect)
    {
        GetComponentInParent<PlayParticleEffect>().PlayParticleSystem(effect);
    }
}
