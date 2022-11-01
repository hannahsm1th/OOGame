using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Equipment : MonoBehaviour
    {
        // This class controls equipment gets picked up and what effect it has on the player.


        //<------- Variables ------->

        [HideInInspector] public int defense = 0;                                         //How many defense points the equipment adds
        [HideInInspector] public int attack = 0;                                          //How many attack points the equipment adds
        [HideInInspector] public int bonusHearts = 0;                                     //How many bonus max hearts the equipment adds
        public int index;                                               //The index of the equipment in the master list

        //<------- Methods ------->

        void Awake()
        {
            switch (this.index)
            {
                case 0:
                    this.defense = 1;
                    break;
                case 1:
                    this.bonusHearts = 2;
                    break;
                case 2:
                    this.defense = 1;
                    break;
                case 3:
                    this.defense = 2;
                    break;
                case 4:
                    this.attack = 1;
                    break;
                case 5:
                    this.defense = 1;
                    break;
                case 6:
                    this.bonusHearts = 1;
                    break;
                case 7:
                    this.attack = 1;
                    break;
                case 8:
                    this.attack = 2;
                    break;
                case 9:
                    this.attack = 3;
                    break;
                default:
                    break;
            }
        }

        protected void DropItem(Player other, Equipment equipment)
        {
            //Updates the HUD
            Game.instance.hudSystemStatic.RemoveEquipment(equipment.index);

            EquipmentWithHearts(other);
        }

        public virtual void PickedUp(Player other)
        {
            //Called when a player moves onto the space of the item

            other.Attack += this.attack;

            other.Defense += this.defense;

            other.MaxHearts += (this.bonusHearts);

            //Updates the HUD
            Game.instance.hudSystemStatic.AddEquipment(this.index);

            EquipmentWithHearts(other);

            //Hides the picked up object
            gameObject.SetActive(false);
        }

        public void EquipmentWithHearts(Player other)
        {
            //Updates the HUD if the equipment has bonus hearts

            if (this.index == 1 || this.index == 6)
            {
                Game.instance.hudSystemStatic.UpdateMaxHearts(other.HeartHalves, other.MaxHearts);
            }
        }
    }