using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //configuration parameters
    [Header ("Player")]
     [SerializeField] float moveSpeed = 10f;
     [SerializeField] int health = 200;

     [Header("Projectile")]
     [SerializeField] GameObject laserPrefab; 
     [SerializeField] float projectileSpeed = 10;
     [SerializeField] float projectilePeriod = 0.1f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion =1f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.7f;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.25f;

     Coroutine firingCoroutine;

     float padding = 0.5f;
     float xMin;
     float xMax;
     float yMin;
     float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();   
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire(){
       if(Input.GetButtonDown("Fire1")){
           firingCoroutine = StartCoroutine(FireContinuously());
       }
       if(Input.GetButtonUp("Fire1")){
           StopCoroutine(firingCoroutine);
       }
       
    }

     private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        
        //if there is no damage dealer...
        if(!damageDealer){return;}

        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die(){
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX,transform.position,transform.rotation);
        Destroy(explosion,durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX,Camera.main.transform.position,deathSFXVolume);
    }

    IEnumerator FireContinuously(){
        while(true){
            GameObject laser = Instantiate(laserPrefab,transform.position,Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0,projectileSpeed);
            
            AudioSource.PlayClipAtPoint(shootSFX,Camera.main.transform.position,shootSFXVolume);
            yield return new WaitForSeconds(projectilePeriod);
        }
    }

    private void Move(){
        var deltaX =  Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX,xMin,xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY,yMin,yMax); 

        transform.position = new Vector2(newXPos,newYPos);
    }

     private void SetUpMoveBoundaries(){
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1,0,0)).x -padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0,1,0)).y - padding;
    }
}
