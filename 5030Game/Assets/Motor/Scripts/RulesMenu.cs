using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuBackgroundGObj;
    [SerializeField] private GameObject gameOverlayTextBackgroundGObj;
    private bool isShowing;
    private GameObject gameOverlayTextBackground;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        isShowing = true;
        menuBackgroundGObj.SetActive(true);
        menuBackgroundGObj.SetActive(isShowing);
        gameObject.SetActive(isShowing);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RulesOn()
    {
        isShowing = true;
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        gameOverlayTextBackgroundGObj.SetActive(true);
        menuBackgroundGObj.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void RulesOff()
    {
        isShowing = false;
        gameObject.SetActive(false);
        gameOverlayTextBackgroundGObj.SetActive(false);
        menuBackgroundGObj.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        //RulesOff();
    }

    private void OnEnable()
    {
        //RulesOn();
    }
    public bool GetIsShowing() { return isShowing; }

    public void QuitApp()
    {
        Debug.Log("RM-quitting");
        Application.Quit();
    }

}
