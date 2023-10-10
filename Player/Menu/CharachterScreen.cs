// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CharachterScreen : Menu
// {
//     public CanvasGroup menu;
//     public GameObject weaponSection;
//     public GameObject combatSlot;
//     public MainMenu mainMenu;

//     public GameObject staffSlot;
//     public GameObject swordSlot;
//     public GameObject knifeSlot;

//     public GameObject helmetSlot;
//     public GameObject chestSlot;
//     public GameObject pantsSlot;
//     public GameObject bootsSlot;

//     public List<GameObject> equippedItems = new List<GameObject>();

//     private bool isShowing;
//     private List<GameObject> combatItemsInInventory = new List<GameObject>();
//     private GameObject newItemSlot;

//     public override bool isOpen => isShowing;

//     private void Start()
//     {
//         allMenus.Add(this);
//         isShowing = false;
//     }

//     public override void Close()
//     {
//         menu.alpha = 0;
//         menu.blocksRaycasts = false;
//         foreach(GameObject g in combatItemsInInventory)
//         {
//             Destroy(g);
//         }
//         isShowing = false;
//         mainMenu.Open();
//     }

//     public override void Open()
//     {
//         CloseOthers();
//         isShowing = true;
//         foreach(CombatItem i in combatItems)
//         {
//             if (NotEquipped(i))
//             {
//                 combatItemsInInventory.Add(CreateCombatSlot(i, weaponSection.transform, false));
//             }
//         }

//         menu.blocksRaycasts = true;
//         menu.alpha = 1;
//     }

//     private GameObject CreateCombatSlot(CombatItem i, Transform t, bool isEquipped)
//     {
//         newItemSlot = Instantiate(combatSlot, t);
//         CombatContainer c = newItemSlot.GetComponent<CombatContainer>();
//         c.combatItem = i;
//         c.cs = this;
//         c.isEquipped = isEquipped;
//         return newItemSlot;
//     }

//     private bool NotEquipped(CombatItem c)
//     {
//         if(equippedItems.Count > 0)
//         {
//             foreach (GameObject g in equippedItems)
//             {
//                 if (g.GetComponent<CombatContainer>().combatItem == c)
//                     return false;
//             }
//         }
//         return true;
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Escape) && isShowing)
//             Close();
//     }

//     public void PickUp(CombatItem i)
//     {
//         combatItems.Add(i);
//     }

//     public override void CloseOthers()
//     {
//         foreach(Menu m in allMenus)
//         {
//             if(m != this)
//             {
//                 if (m.isOpen)
//                     m.Close();
//             }
//         }
//     }

//     public void Equip(GameObject g)
//     {
//         Transform slot = null;

//         if (combatItemsInInventory.Contains(g) | equippedItems.Contains(g))
//         {
//             CombatContainer c = g.GetComponent<CombatContainer>();
//             switch (c.combatItem.combatType)
//             {
//                 case CombatItem.CombatType.Armour_Boots:
//                     slot = bootsSlot.transform;
//                     break;
//                 case CombatItem.CombatType.Armour_ChestPlate:
//                     slot = chestSlot.transform;
//                     break;
//                 case CombatItem.CombatType.Armour_Helmet:
//                     slot = helmetSlot.transform;
//                     break;
//                 case CombatItem.CombatType.Armour_Pants:
//                     slot = pantsSlot.transform;
//                     break;
//                 case CombatItem.CombatType.Weapon_Knife:
//                     slot = knifeSlot.transform;
//                     break;
//                 case CombatItem.CombatType.Weapon_Staff:
//                     slot = staffSlot.transform;
//                     break;
//                 case CombatItem.CombatType.Weapon_Sword:
//                     slot = swordSlot.transform;
//                     break;
//                 default:
//                     return;
//             }

//             if (!c.isEquipped)
//             {
//                 equippedItems.Add(CreateCombatSlot(c.combatItem, slot, true));
//                 combatItemsInInventory.Remove(g);
//                 Destroy(g);
//             }
//             else
//             {
//                 combatItemsInInventory.Add(CreateCombatSlot(c.combatItem, weaponSection.transform, false));
//                 equippedItems.Remove(g);
//                 Destroy(g);
//             }
//         }
//     }
// }
