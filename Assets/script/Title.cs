using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.SceneManagement;

 public class Title : MonoBehaviour
 {
     private bool firstPush = false;

     public void Select()
     {
          Debug.Log("Press Start!");
          if (!firstPush)
          {
              Debug.Log("Go Next Scene!");
              SceneManager.LoadScene("stage1");
              firstPush = true;
          }
     }
 }