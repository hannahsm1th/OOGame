using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HUD : MonoBehaviour
    {
        // This class controls how the different types of head armour behave


        //<------- Variables ------->

        public Sprite[] HeartSprites;                                   //The sprites for the hearts
        public Sprite[] EquipmentSprites;                               //The sprites for the equipment

        private List<HeartImage> _heartImageList;                       //The images reflecting a full, empty or half heart
        private List<EquipmentImage> _equipmentImageList;               //The images of the equipment
        private HUDSystem _hudSystem;                                   //The HUDSystem that manages this display
        private List<HUDSystem.Heart> _heartList;                       //The list of Hearts
        private List<int> _equipmentList;                               //A list of the indexes of the equipped items from the master list
        private List<GameObject> _hudImageObjects;                       //A list containing the GameObjects created when updating the HUD

        //<------- Methods ------->

        private void Awake()
        {
            _heartImageList = new List<HeartImage>();

            _equipmentImageList = new List<EquipmentImage>();

            _hudImageObjects = new List<GameObject>();
        }

        private void Start()
        {
            //Attached the HUD to the game's HUD system and sets it up

            _hudSystem = new HUDSystem(Game.instance.playerHearts, Game.instance.maxPlayerHearts);

            Game.instance.hudSystemStatic = _hudSystem;

            SetupHUDSystem(_hudSystem);
        }

        public void SetupHUDSystem(HUDSystem hudSystem)
        {
            this._hudSystem = hudSystem;

            //Populates and displays the HUD
            DisplayHUD();

            //Subscribes to the various events the HUDSystem can publish about
            _hudSystem.OnDamaged += HUDSystem_OnDamaged;

            _hudSystem.OnHealed += HUDSystem_OnHealed;

            _hudSystem.OnHeartsAdded += HUDSystem_OnHeartsAdded;

            _hudSystem.OnEquip += HUDSystem_OnEquip;
        }

        private void DisplayHUD()
        {
            //Gets the relevant information from the management system
            _heartList = _hudSystem.GetHeartList;

            _equipmentList = _hudSystem.EquipmentList;

            //Creates the heart objects
            Vector2 heartPosition = new Vector2(40, 40);

            for (int i = 0; i < _heartList.Count; i++)
            {
                HUDSystem.Heart heart = _heartList[i];
                DisplayHearts(heartPosition).SetHearts(heart.HeartHalves);
                heartPosition += new Vector2(40, 0);
            }

            //Creates the equipment objects
            Vector2 equipmentPosition = new Vector2(40, 80);

            //Removes all of the null equipment slots using a lambda expression
            _equipmentList.RemoveAll(item => item == -1);

            for (int j = 0; j < _equipmentList.Count; j++)
            {
                DisplayEquipment(equipmentPosition, _equipmentList[j]).SetImage(_equipmentList[j]);
                equipmentPosition += new Vector2(40, 0);
            }
        }

        private void HUDSystem_OnDamaged(object sender, System.EventArgs e)
        {
            UpdateHUDDisplay();
        }

        private void HUDSystem_OnHealed(object sender, System.EventArgs e)
        {
            UpdateHUDDisplay();
        }

        private void HUDSystem_OnHeartsAdded(object sender, System.EventArgs e)
        {
            UpdateHUDDisplay();
        }

        private void HUDSystem_OnEquip(object sender, System.EventArgs e)
        {
            UpdateHUDDisplay();
        }

        public void UpdateHUDDisplay()
        {
            //Clears the lists and destroys the previous game objects to make way for the updated HUD
            _heartImageList.Clear();

            _equipmentImageList.Clear();

            for (int k = 0; k < _hudImageObjects.Count; k++)
            {
                Destroy(_hudImageObjects[k]);
            }

            DisplayHUD();

            //Iterates through the two lists of images and sets them to the appropriate sprite
            for (int i = 0; i < _heartImageList.Count; i++)
            {
                HeartImage heartImage = _heartImageList[i];
                HUDSystem.Heart heart = _heartList[i];
                heartImage.SetHearts(heart.HeartHalves);
            }

            for (int j = 0; j < _equipmentList.Count; j++)
            {
                EquipmentImage equipmentImage = _equipmentImageList[j];
                equipmentImage.SetImage(_equipmentList[j]);
            }
        }

        private HeartImage DisplayHearts(Vector2 position)
        {
            //Creates new game objects for each heart
            GameObject heartObject = new GameObject("Heart", typeof(Image));

            heartObject.transform.SetParent(transform, false);

            heartObject.transform.localPosition = Vector3.zero;

            _hudImageObjects.Add(heartObject);

            heartObject.GetComponent<RectTransform>().position = position;

            heartObject.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);

            //Set the heart sprite
            Image heartImageUI = heartObject.GetComponent<Image>();

            heartImageUI.sprite = HeartSprites[2];

            //Creates a new image and returns it
            HeartImage heartImage = new HeartImage(this, heartImageUI);

            _heartImageList.Add(heartImage);

            return heartImage;
        }

        private EquipmentImage DisplayEquipment(Vector2 position, int index)
        {
            //Creates new game objects for each piece of equipment
            GameObject equipmentObject = new GameObject("EquipmentImage", typeof(Image));

            equipmentObject.transform.SetParent(transform, false);

            equipmentObject.transform.localPosition = Vector3.zero;

            _hudImageObjects.Add(equipmentObject);

            equipmentObject.GetComponent<RectTransform>().position = position;

            equipmentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);

            //Set the equipment sprite
            Image equipmentObjectUI = equipmentObject.GetComponent<Image>();

            equipmentObjectUI.sprite = EquipmentSprites[index];

            //Creates a new image and returns it
            EquipmentImage equipmentImage = new EquipmentImage(this, equipmentObjectUI);

            _equipmentImageList.Add(equipmentImage);

            return equipmentImage;
        }

        //These two classes (HeartImage and Equipment Image) are encapsulated inside the HUD
        //They each ensure that the correct image displays for each game object in the HUD

        public class HeartImage
        {
            //<------- Variables ------->

            private Image _heartImage;
            private HUD _HUD;

            //<------- Methods ------->

            public HeartImage(HUD hud, Image heartImage)
            {
                this._heartImage = heartImage;
                this._HUD = hud;
            }

            public void SetHearts(int halves)
            {
                switch (halves)
                {
                    case 0:
                        _heartImage.sprite = _HUD.HeartSprites[0];
                        break;
                    case 1:
                        _heartImage.sprite = _HUD.HeartSprites[1];
                        break;
                    case 2:
                        _heartImage.sprite = _HUD.HeartSprites[2];
                        break;
                }
            }

        }

        public class EquipmentImage
        {
            //<------- Variables ------->

            private Image _equipmentImage;
            private HUD _HUD;

            //<------- Methods ------->

            public EquipmentImage(HUD hud, Image equipmentImage)
            {
                this._equipmentImage = equipmentImage;
                this._HUD = hud;
            }

            public void SetImage(int index)
            {
                switch (index)
                {
                    case 0:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[0];
                        break;
                    case 1:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[1];
                        break;
                    case 2:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[2];
                        break;
                    case 3:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[3];
                        break;
                    case 4:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[4];
                        break;
                    case 5:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[5];
                        break;
                    case 6:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[6];
                        break;
                    case 7:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[7];
                        break;
                    case 8:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[8];
                        break;
                    case 9:
                        _equipmentImage.sprite = _HUD.EquipmentSprites[9];
                        break;

                }
            }
        }
    }