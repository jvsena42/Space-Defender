using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header ("Enemy Stats")] 
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header ("Shotting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 2f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10;

    [Header ("Visual Efects")]
    [SerializeField] GameObject deathVFX;

    [Header ("Audio Efects")]
    [SerializeField] float durationOfExplosion =1f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.7f;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.25f;


    // Start is called before the first frame update
    void Start()
    {
        initShotCounter();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot(){
        shotCounter -= Time.deltaTime;

        if(shotCounter <=0){
            Fire();
            initShotCounter();
        }
    }

    private void initShotCounter(){
        shotCounter = Random.Range(minTimeBetweenShots,maxTimeBetweenShots);
    }

    private void Fire(){

        GameObject laser = Instantiate(
            projectile,
            transform.position,
            Quaternion.identity
        ) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0,-projectileSpeed);

        AudioSource.PlayClipAtPoint(shootSFX,Camera.main.transform.position,shootSFXVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

        //if there is no damage dealer...
        if(!damageDealer){return;}

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die(){
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX,transform.position,transform.rotation);
        Destroy(explosion,durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX,Camera.main.transform.position,deathSFXVolume);
    }
}
