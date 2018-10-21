using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 0.5f;
    [SerializeField] int health = 200;
    [SerializeField] GameObject deathVfx;
    [SerializeField] float durationOfExplosion = 1f;
    [Header("Audio")]
    [SerializeField] AudioClip deathSfx;
    [SerializeField] [Range(0, 1)] float deathSfxVolume = 0.7f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.7f;
    [Header("Projectile")]
    [SerializeField] GameObject LaserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    Coroutine firingCoroutine;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

	// Use this for initialization
	void Start () {
        SetUpMoveBoundaries();
	}
	
	// Update is called once per frame
	void Update () {
        Move();
        Fire();
	}

    private void SetUpMoveBoundaries () {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    private void Move () {
        // check options for setting

        // follow finger
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        gameObject.transform.position = mousePosition;

        // joystick

        // keyboard
        // var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        // var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        // var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        // var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        // axis
        // var newXPos = Mathf.Clamp(transform.position.x + Input.acceleration.x, xMin, xMax);
        // var newYPos = Mathf.Clamp(transform.position.y + Input.acceleration.y, yMin, yMax);


        // transform.position = new Vector2(newXPos, newYPos);
    }

    private void Fire () {
        if (Input.GetButtonDown("Fire1")) {
            firingCoroutine = StartCoroutine(FireContinuously());
        } else if (Input.GetButtonUp("Fire1")) {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously () {
        while (true) {
            GameObject laser = Instantiate(
                LaserPrefab,
                transform.position,
                Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        // screen shake
        // subtract from healthDisplay
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<LevelManager>().LoadGameOver();
        Destroy(gameObject);
        GameObject Explosion = Instantiate(
            deathVfx,
            transform.position,
            transform.rotation);
        Destroy(Explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSfx, Camera.main.transform.position, deathSfxVolume);
    }

    public int GetHealth() {
        return health;
    }
}
