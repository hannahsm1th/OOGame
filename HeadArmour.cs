using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HeadArmour : Equipment
    {
        // This class controls how the different types of head armour behave


        //<------- Methods ------->

        public override void PickedUp(Player other)
        {
            //Checks if there is already some armour equipped
            if (other.HeadArm != -1)
            {
                other.Attack -= Game.instance.equipmentArray[other.HeadArm].attack;
                other.Defense -= Game.instance.equipmentArray[other.HeadArm].defense;
                other.MaxHearts -= Game.instance.equipmentArray[other.HeadArm].bonusHearts;
                base.DropItem(other, Game.instance.equipmentArray[other.HeadArm]);
            }

            //Sets the head armour as this item
            other.HeadArm = this.index;

            base.PickedUp(other);
        }
    }