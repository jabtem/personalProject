using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManger : MonoBehaviour
{
    public Image loadingBar;

    void Awake()
    {
        loadingBar.fillAmount = 0;
    }

    private void Update()
    {

        loadingBar.fillAmount += Time.deltaTime;
    }
}
