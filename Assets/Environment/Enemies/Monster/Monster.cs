using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

public class Monster : MonoBehaviour
{
    private float moveSpeed = 2.5f; //Each moment the monster goes further to the right.  This speed increases over time
    private float attackBudget = 0f; //The monster also has two other means of attacking.  Eyeballs that spawn in various places which fire projectiles, and occasionally the monster will swipe at the player horizontally.  

    private GameObject playerReference = null;

    public float baseSpeed;
    public float speedIncrement1;
    public float speedIncrement2;

    public void setBudget(float budget) { attackBudget = budget; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void initializeMonster(GameObject playerRef)
    {
        this.playerReference = playerRef;
    }

    private void setSpeed()
    {
        float x = Vector3.Distance(this.gameObject.transform.position, playerReference.gameObject.transform.position);

        // Base speed increment
        moveSpeed = baseSpeed + (speedIncrement1 * x * Time.time);
        print("Distance To Player: "+ x);

        float distanceMod = Mathf.Clamp((x / 50), .4f, 1);

        // Second speed increment
        moveSpeed *= distanceMod;
        moveSpeed += (speedIncrement2 * x * Time.time);
        print("Enemy Move Speed: " + moveSpeed);
    }

    public float getSpeed() //add to projectile speed when firing from eye turret.  
    {
        return this.moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        setSpeed();
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x + (moveSpeed * Time.deltaTime), this.gameObject.transform.position.y, this.gameObject.transform.position.z);
    }
}