using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class GManager : MonoBehaviour
 {
     public static GManager instance = null;
 
     public int score = 0;
     public int stageNum = 1;
     public int continueNum = 0;
     public int heartNum = 3;
 
     private void Awake()
     {
         if(instance == null)
         {
             instance = this;
             DontDestroyOnLoad(this.gameObject);
         }
         else
         {
             Destroy(this.gameObject);
         }
     }
 }