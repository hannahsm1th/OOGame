using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Entity : MonoBehaviour
    {
        // This class controls how the players and enemies move and collide with each other.


        //<------- Variables ------->

        public float moveTime = 0.1f;                                   //Time it will take object to move
        public LayerMask collision;                                     //The layer where the entities collide
        public int attack;                                              //The entity attack
        public int defense;                                             //The entity defense

        private BoxCollider2D _boxCollider;                             //The box collider for the entity
        private Rigidbody2D _rigidBody;                                 //The rigid body object
        private bool _isMoving;                                         //A bool describing if the entity is moving or not
        private float _inverseMoveTime;                                 //Used for the movement method

        protected RaycastHit2D hit;                                     //The cast for the hit when the entities collide
        protected bool validMove;                                       //Whether the entity can move onto a space


        //<------- Methods ------->

        protected virtual void Start()
        {
            //Gets the components for the box collider and rigid body
            _boxCollider = GetComponent<BoxCollider2D>();

            _rigidBody = GetComponent<Rigidbody2D>();

            _inverseMoveTime = 1f / moveTime;
        }

        protected bool Move(int xMove, int yMove, out RaycastHit2D hit)
        {
            //This returns true if the entity can move, false if it can't

            Vector2 start = transform.position;

            Vector2 end = start + new Vector2(xMove, yMove);

            //Disabling the box collider so the entity's cast doesn't hit their own box collider
            _boxCollider.enabled = false;

            hit = Physics2D.Linecast(start, end, collision);

            //Checking for a collision
            _boxCollider.enabled = true;

            //If the space is free, moving there and returning true
            if (hit.transform == null && !_isMoving)
            {
                StartCoroutine(SmoothMovement(end));
                return true;
            }

            //Otherwise returning false
            return false;
        }

        protected IEnumerator SmoothMovement(Vector3 end)
        {
            _isMoving = true;

            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end and move there
                Vector3 newPosition = Vector3.MoveTowards(_rigidBody.position, end, _inverseMoveTime * Time.deltaTime);

                _rigidBody.MovePosition(newPosition);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }

            //Make sure the object is exactly at the end of its movement.
            _rigidBody.MovePosition(end);

            _isMoving = false;
        }

        protected virtual void AttemptMove<T>(int xMove, int yMove)
            where T : UnityEngine.Component
        {
            validMove = Move(xMove, yMove, out hit);

            //Exits if nothing was hit
            if (hit.transform == null)
            {
                return;
            }

            T component = hit.transform.GetComponent<T>();

            if (!validMove && component != null)
            {
                OnBlocked(component);
            }
        }

        public abstract void LoseHearts(int damage);

        protected abstract void OnBlocked<T>(T component)
            where T : UnityEngine.Component;
    }