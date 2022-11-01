using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Wall : MonoBehaviour
    {
        // This class controls how the walls behave


        //<------- Variables ------->

        public Sprite dmgSprite;                                        //Alternate sprite to display after Wall has been attacked by player
        public int hitpoints;                                           //hit points for the wall (variable, set in Inspector)
                                                                        //The sound effects for when the player attacks the wall
        public AudioClip chopSound1;
        public AudioClip chopSound2;

        private SpriteRenderer _spriteRenderer;                         //Store a component reference to the attached SpriteRenderer


        //<------- Methods ------->

        private void Awake()
        {
            //Get a component reference to the SpriteRenderer
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void DamageWall(int loss)
        {
            //Called when the player digs against a wall

            SoundManager.instance.RandomiseSfx(chopSound1, chopSound2);

            //Set spriteRenderer to the damaged wall sprite
            _spriteRenderer.sprite = dmgSprite;

            //Subtract loss from hit point total
            hitpoints -= loss;

            //If the wall has been damaged completely, removes it from the game
            if (hitpoints <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }