using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : Entity
    {
        // This class controls how the different types of enemies move.


        //<------- Variables ------->

        public int hearts;                                              //Enemy health in hearts
        //The enemy's attack sound effects
        public AudioClip enemyAttack1;
        public AudioClip enemyAttack2;

        private Transform _target;                                      //The player the enemies are targeting
        private bool _skipMove;

        //<------- Methods ------->

        protected override void Start()
        {
            //Adds the enemy to the list of enemies
            Game.instance.AddEnemyToList(this);

            //Searches for the Player using their tag
            _target = GameObject.FindGameObjectWithTag("Player").transform;

            //Calls the base function
            base.Start();
        }

        protected override void AttemptMove<T>(int xMove, int yMove)
        {
            if (_skipMove)
            {
                _skipMove = false;
                return;
            }

            base.AttemptMove<T>(xMove, yMove);

            _skipMove = true;
        }

        public void MoveEnemy()
        {
            //The move direction
            int xMove = 0;
            int yMove = 0;

            //Determines where the player is relative to the enemy and moves towards them
            if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
            {
                yMove = _target.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                xMove = _target.position.x > transform.position.x ? 1 : -1;
            }

            AttemptMove<Player>(xMove, yMove);
        }

        protected override void OnBlocked<T>(T component)
        {
            //If the enemy encounters a player

            Player player = component as Player;

            //Attack sound effect
            SoundManager.instance.RandomiseSfx(enemyAttack1, enemyAttack2);

            //The player loses hearts appropriate to the enemy's damage level
            if ((attack - player.defense) < 1)
            {
                player.LoseHearts(1);
            }
            else
            {
                player.LoseHearts((attack - player.defense));
            }
        }

        public override void LoseHearts(int damage)
        {
            hearts -= damage;

            //Destroys the enemy if their health is depleted
            if (hearts <= 0)
            {
                Game.instance.RemoveEnemyFromList(this);

                Destroy(this.transform.gameObject);
            }
        }
    }