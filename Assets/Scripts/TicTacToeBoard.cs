using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;



public class TicTacToeBoard : MonoBehaviour
{
 

   [SerializeField]
   public Transform slotParent;
   
   
   public BoardTile[] slotList;

   private int id;
   
   private void Start()
   {
      slotList = slotParent.GetComponentsInChildren<BoardTile>();
     

      
   }


   public void ResetBoard()
   {
      foreach (var slot in slotList)
      {
         slot._image = null;
         slot._positionID = identifier.BLANK;
         slot.isBlank = true;
      }
   }

   


   
}
