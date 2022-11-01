using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HUDSystem
    {
        // This class controls how the different types of head armour behave


        //<------- Variables ------->

        //These are the events the HUDSystem uses to communicate with the HUD class
        public event EventHandler OnDamaged;                            //This event is published when the player takes damage
        public event EventHandler OnHealed;                             //This event is published when the player heals
        public event EventHandler OnHeartsAdded;                        //This event is published when the player equips an item that gives bonus max hearts
        public event EventHandler OnEquip;                              //This event is published whenever the player equips and item

        private List<Heart> _heartList;                                 //This is a list of the player's hearts
        private List<int> _equipmentList;                               //This list tracks the player's equipment

        //<------- Methods ------->

        public HUDSystem(int hearts, int maxHearts)
        {
            //Checks if the player has a half heart
            bool halfHeart = false;

            if (hearts % 2 == 1)
            {
                halfHeart = true;
                hearts -= 1;
            }

            _heartList = new List<Heart>();

            //Adds the whole hearts
            for (int i = 0; i < hearts / 2; i++)
            {
                Heart heart = new Heart(2);
                _heartList.Add(heart);
            }

            //If there was a half heart, adds that to the end of the list so it displays last
            if (halfHeart)
            {
                Heart heart = new Heart(1);
                _heartList.Add(heart);
            }

            //Displays any empty heart containers
            int emptyHearts = maxHearts - hearts;

            if (emptyHearts > 0)
            {
                for (int i = 0; i < emptyHearts / 2; i++)
                {
                    Heart heart = new Heart(0);
                    _heartList.Add(heart);
                }
            }

            //Creates and adds the player's equipment to the equipment list
            _equipmentList = new List<int>();

            _equipmentList.Add(Game.instance.playerWeaponIndex);

            _equipmentList.Add(Game.instance.playerHeadArmourIndex);

            _equipmentList.Add(Game.instance.playerBodyArmourIndex);

            _equipmentList.Add(Game.instance.playerPendantIndex);

        }

        public void UpdateMaxHearts(int hearts, int maxHearts)
        {
            //This is called whenever the player equips something that affects their maximum hearts

            int emptyHearts = maxHearts - hearts;

            if (emptyHearts > 0)
            {
                for (int i = 0; i < emptyHearts / 2; i++)
                {
                    Heart heart = new Heart(0);
                    _heartList.Add(heart);
                }
            }

            //Triggers the appropriate event
            if (OnHeartsAdded != null)
            {
                OnHeartsAdded(this, EventArgs.Empty);
            }
        }

        public void AddEquipment(int equipmentIndex)
        {
            //This is called whenever the player equips something

            _equipmentList.Add(equipmentIndex);

            //Triggers the appropriate event
            if (OnEquip != null)
            {
                OnEquip(this, EventArgs.Empty);
            }
        }

        public void RemoveEquipment(int equipmentIndex)
        {
            //This is called whenever the player drops equipment
            //Since this only happens accompanied by an AddEquipment call, there is no need for a separate event
            _equipmentList.Remove(equipmentIndex);
        }

        public void TakeDamage(int damage)
        {
            //This is called whenever the player takes damage

            for (int i = _heartList.Count - 1; i >= 0; i--)
            {
                Heart heart = _heartList[i];
                if (damage > heart.HeartHalves)
                {
                    damage -= heart.HeartHalves;
                    heart.Damage(heart.HeartHalves);
                }
                else
                {
                    heart.Damage(damage);
                    break;
                }
            }

            //Triggers the appropriate event
            if (OnDamaged != null)
            {
                OnDamaged(this, EventArgs.Empty);
            }
        }

        public void HealDamage(int healAmount)
        {
            //This is called whenever the player heals

            for (int i = 0; i < _heartList.Count; i++)
            {
                Heart heart = _heartList[i];
                int missingHalf = 2 - heart.HeartHalves;
                if (healAmount > missingHalf)
                {
                    healAmount -= missingHalf;
                    heart.Heal(missingHalf);
                }
                else
                {
                    heart.Heal(healAmount);
                    break;
                }
            }

            //Triggers the appropriate event
            if (OnHealed != null)
            {
                OnHealed(this, EventArgs.Empty);
            }
        }

        //<------- Properties ------->

        public List<Heart> GetHeartList
        {
            get => _heartList;
        }

        public List<int> EquipmentList
        {
            get => _equipmentList;
        }


        //This class is encapsulated inside the HUDSystem
        public class Heart
        {
            //<------- Variables ------->

            private int _halves;

            //<------- Methods ------->

            public Heart(int halves)
            {
                this._halves = halves;
            }

            public void Damage(int damage)
            {
                //The heart's behaviour when it takes damage
                if (damage >= _halves)
                {
                    _halves = 0;
                }
                else
                {
                    _halves -= damage;
                }
            }

            public void Heal(int healAmount)
            {
                //The heart's behaviour when it heals
                if (_halves + healAmount > 2)
                {
                    _halves = 2;
                }
                else
                {
                    _halves += healAmount;
                }
            }

            //<------- Properties ------->
            public int HeartHalves
            {
                get => _halves;
                set => _halves = value;
            }
        }
    }