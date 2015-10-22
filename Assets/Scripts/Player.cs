﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    private float radius;
    public float Radius
    {
        get { return radius; }
    }
    float goal;

    private int level;
    public int Level
    {
        get { return level; }
    }
    private int direction;
    public int Direction
    {
        get { return direction; }
    }

    float timer;
    Vector3 move;

    bool falling;
    bool jumping;
    bool playing;
	bool grounded;

	Quaternion rot;

    public Text score;
    public Camera cam;
    public GameObject spawner;
    public GameObject bulletP;
    public Text youWin;

	// Use this for initialization
	void Start () {
        level = 1;
        radius = 0.5f;
        direction = 1;
        goal = 1.0f;
        timer = 0;
        falling = true;
        jumping = false;
        playing = true;
		grounded = true;
		rot = new Quaternion (0, 0, 0, 1);
        youWin.fontSize = 24;
        score.text = "Current Size: " + radius + "\n Goal Size: 2";
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Platform")
        {
            falling = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            Vector3 hit = collision.contacts[0].normal;

            if (Vector3.Dot(hit, Vector3.up) > 0)
            {
                falling = false;
                grounded = true;
                jumping = false;
            }
            else if (Vector3.Dot(hit, Vector3.up) < 0)
            {
                falling = true;
                grounded = false;
                jumping = false;
            }
            else if (Vector3.Dot(hit, Vector3.forward) > 0 || Vector3.Dot(hit, Vector3.forward) < 0 && !grounded)
            {
                falling = false;
                grounded = false;
                jumping = true;
            }
        }
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy2")
        {
            if (collision.gameObject.GetComponent<Enemy>().radius <= radius)
            {
                float v1 = (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(radius, 3);
                float v2 = (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(collision.gameObject.GetComponent<Enemy>().radius, 3);
                float volTot = v1 + v2;
                float rb = Mathf.Pow((volTot * 3.0f) / (4.0f * Mathf.PI), (1.0f / 3.0f));
                float rNew = rb - radius;
                radius += rNew;
                GameObject.Destroy(collision.gameObject);
            }
            else if (collision.gameObject.GetComponent<Enemy>().radius >= radius)
            {
                radius -= collision.gameObject.GetComponent<Enemy>().Damage;
            }
        }
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            radius += .01f;
        }
        if(collision.gameObject.tag == "Pit")
        {
            playing = false;
            youWin.enabled = true;
            youWin.text = "You Lose!";
            score.text = "Current Size: " + radius + "\n Goal Size: " + goal;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(radius >= goal && level == 1)
        {
            level = 2;
            spawner.transform.position = new Vector3(0.0f, 7.41f, 30.06f);
            cam.transform.position = new Vector3(9f, 1f, 30.06f);
            transform.position = new Vector3(0.29f,0.98f,30.06f);
            goal = 3.0f;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject i in enemies)
            {
                Destroy(i);
            }
        }
        if (radius >= goal && level == 2)
        {
            playing = false;
            youWin.enabled = true;
            score.text = "Current Size: " + radius + "\n Goal Size: " + goal;
        }
        if(radius <= 0)
        {
            playing = false;
            youWin.enabled = true;
            youWin.text = "You Lose!";
            score.text = "Current Size: " + radius + "\n Goal Size: " + goal;
        }
        GetComponent<Transform>().localRotation = new Quaternion(0, 0, 0, 1);
        if (playing)
        {
            move = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                move = new Vector3(0, 0, -0.1f);
                direction = 1;
				rot = new Quaternion(0,0,0,1);
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                move = new Vector3(0, 0, 0.1f);
				direction = 2;
				rot = new Quaternion(0,180,0,1);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (!jumping && grounded)
                {
                    jumping = true;
                    grounded = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && level >= 2)
            {
                GameObject bC = Instantiate(bulletP);
                Vector3 pos = transform.position;
                if(direction == 1)
                {
                    bC.transform.position = new Vector3(pos.x, pos.y, pos.z - (5 * radius) / 8);
                }
                if (direction == 2)
                {
                    bC.transform.position = new Vector3(pos.x, pos.y, pos.z + (5 * radius) / 8);
                }
                radius -= .01f;
            }
            if (falling && !jumping)
            {
                move.y = -0.1f;
                timer = 0;
            }
            if (jumping && !grounded)
            {
                timer += Time.deltaTime;
                if (timer >= 0.6f)
                {
                    jumping = false;
					falling = true;
                    timer = 0;
                }
                move.y = 0.1f;
            }
			transform.localRotation = rot;
            float scale = 1.0f * (radius / goal);
            transform.localScale = new Vector3(0.3f, scale, scale);
            GetComponent<CharacterController>().Move(move);
            score.text = "Current Size: " + radius + "\n Goal Size: " + goal;
        }
    }
}
