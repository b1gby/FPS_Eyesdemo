using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject readyButton;
    public bool is_Ready = false;
    static int escCount = 0;

    public GameObject escMenu;
    public Button btnResume;
    public Button btnExit;
    public bool is_clickEscMenu = false;

    public int EyeNum;
    public Text Wintxt;
    public Text Losetxt;

    private int TotalTime = 180;
    public Text timerTxt;
    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = true;
        Time.timeScale = 0;


        StartCoroutine(CountDown());
    }

    public void ReadyToPlay()
    {
        readyButton.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;//锁定指针到视图中心
        Cursor.visible = false;

        is_Ready = true;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(is_Ready)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escCount++;
            }
            if (escCount % 2 == 0)
            {
                Cursor.lockState = CursorLockMode.Locked;//锁定指针到视图中心
                Cursor.visible = false;

                escMenu.SetActive(false);
                is_clickEscMenu = false;
            }
            else
            {

                Cursor.lockState = CursorLockMode.None;//锁定指针到视图中心
                Cursor.visible = true;

                escMenu.SetActive(true);
                is_clickEscMenu = true;
            }
        }

        EyeNum = GameObject.FindGameObjectsWithTag("Eye").GetLength(0);
        if(EyeNum == 0)
        {
            Wintxt.gameObject.SetActive(true);
            if(Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("SampleScene");
                Wintxt.gameObject.SetActive(false);
            }
        }

        if(TotalTime<=0)
        {
            Losetxt.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("SampleScene");
                Losetxt.gameObject.SetActive(false);
            }
        }
    }

    public void ClickExitButton()
    {
        Application.Quit();
    }

    public void ClickResumeButton()
    {
        escCount++;
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimer(int time)
    {
        int minute = time / 60;
        int second = time % 60;

        timerTxt.text = minute.ToString().PadLeft(2, '0') + " : " + second.ToString().PadLeft(2, '0');
    }

    IEnumerator CountDown()
    {
        while (TotalTime >= 0)
        {
            UpdateTimer(TotalTime);
            yield return new WaitForSeconds(1);
            TotalTime--;
        }
    }
}
