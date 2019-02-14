using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : Interactable
{
    public float timeToEmpty;
    
    private Rigidbody2D rb;
    private PlayerController player;
    private Light lampLight;
    private float maxIntensity;

    private Vector3 start;
    private Vector3 end, force;
    private float startTime, endTime, deltaTime;

    public AudioSource audioFill;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>();
        lampLight = gameObject.GetComponentInChildren<Light>();
        maxIntensity = lampLight.intensity;
    }

    public override void function(){
        if(inUse) {
            if(player.GetOverBarrel()) {
                // refill lamp
                lampLight.intensity = maxIntensity;
                audioFill.Play();
            }
            else if (player.GetOverInteractables() == 1){
                // set down lamp
                inUse = false;
                rb.isKinematic = false;

                rb.velocity = new Vector2(0.0f, 0.0f);
                transform.parent = null;
            }
        }
        else {
            // pick up lamp
            inUse = true;
            rb.isKinematic = true;

            transform.parent = player.transform;
            transform.position = player.transform.position;
        }
    }

    protected override void UpdateInteractable(){
        ThrowUsingMouse();
        ThrowUsingQ(player.GetFacingRight());

        lampLight.intensity -= maxIntensity * (Time.deltaTime / timeToEmpty);
    }

    void ThrowUsingQ(bool facingRight){
        if(Input.GetKeyDown(KeyCode.Q)) startTime = Time.time;
        else if(Input.GetKeyUp(KeyCode.Q))
        {
            endTime = Time.time;
            deltaTime = (endTime - startTime)*10;
        }
        if(Input.GetKeyUp(KeyCode.Q) && inUse)
        {
            rb.isKinematic = false;
            if(facingRight) { rb.velocity = new Vector2(deltaTime, deltaTime); } else { rb.velocity = new Vector2(-deltaTime, deltaTime); }
            transform.parent = null;
            inUse = false;
        }
    }


    void ThrowUsingMouse(){
        if(Input.GetMouseButtonDown(0)) start = Input.mousePosition;
        else if(Input.GetMouseButtonUp(0))
        {
            end = Input.mousePosition;
            force = end - start;
        }
        if(Input.GetMouseButtonUp(0) && inUse)
        {
            rb.isKinematic = false;
            rb.velocity = new Vector2(-force.x/10, -force.y/10);
            transform.parent = null;
            inUse = false;
        }
    }
}