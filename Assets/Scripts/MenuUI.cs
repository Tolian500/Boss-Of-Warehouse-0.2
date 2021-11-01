using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUI : MonoBehaviour
{
    //public Rigidbody rg_Player;
    //public Slider slider;
    //public Material PlayerRenderer;
    //public Image mainImage;
    

   
    public void StartNew()
    {
        SceneManager.LoadScene(2);
    }
    public void Exit()
    {
#if UNITY_EDITOR

        EditorApplication.ExitPlaymode();
#else

Application.Quit();

#endif

    }





}
