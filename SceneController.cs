using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class SceneController : MonoBehaviour
{

    public GameObject bigfade;
    public GameObject introtext;
    private bool textbreeki;
    public GameObject backbuttonintro;
    public AudioSource introsong;

    public Image mutebuttonimg;
    private bool mutesw;

    public GameObject saatexpanel;
    public GameObject saatext;
    public int page = 1;

    public GameObject saagunsl;
    public GameObject saabutton;
    void Start()
    {
        backbuttonintro.SetActive(false);
        Time.timeScale = 1;

    }
    void Update()
    {
        if (textbreeki && introtext.transform.position.y <= 1000) introtext.transform.position += new Vector3(0, 0.28f, 0);

        if (TotalGameDirector.saainfounlock==true)
        {
            saagunsl.SetActive(true);
            saabutton.SetActive(true);
        }
        else
        {
            saagunsl.SetActive(false);
            saabutton.SetActive(false);
        }
    }

    public void LoadGame() { SceneManager.LoadScene("GameScene"); }
    public void IntroStart() { StartCoroutine(IntroViewing()); }

    public void LoadShootingRange() { SceneManager.LoadScene("ShootingRange"); }
    public void BackTitleFromIntro()
    {
        introsong.Play();

        backbuttonintro.SetActive(false);
        introtext.transform.localPosition = new Vector3(0, -1100, 0);
        bigfade.transform.DOLocalMoveX(500, 0.5f);
        textbreeki = false;
    }

    IEnumerator IntroViewing()
    {
        bigfade.transform.DOLocalMoveX(-0, 0.5f);
        introsong.Play();
        yield return new WaitForSeconds(1);
        textbreeki = true;
        yield return new WaitForSeconds(5);
        backbuttonintro.SetActive(true);

    }

    public void SAAPanelOpen()
    {
        bigfade.transform.DOLocalMoveX(-0, 0.2f);
        saatexpanel.transform.DOLocalMoveX(0,0.7f);
    }
    public void SAAPanelClose()
    {
        saatext.transform.DOLocalMoveX(0, 0.1f);
        bigfade.transform.DOLocalMoveX(500, 0.2f);
        saatexpanel.transform.DOLocalMoveX(500, 0.3f);
        page = 1;
    }
    public void SAATextNext()
    {
       if(page<3)page++;
       if(page==2) saatext.transform.DOLocalMoveX(-500, 0.3f);
       if(page==3) saatext.transform.DOLocalMoveX(-1000, 0.3f);
    }
    public void SAATextPrev()
    {
        if(page>1)page--;
        if (page == 1) saatext.transform.DOLocalMoveX(0, 0.3f);
        if (page==2) saatext.transform.DOLocalMoveX(-500, 0.3f);
    }

    public void Mute()
    {
        if (!mutesw)
        {
            //
            introsong.volume = 0;
            mutebuttonimg.fillAmount = 0.7f;
            mutesw = true;
        }
        else if (mutesw)
        {
            introsong.volume = 0.5f;

            mutebuttonimg.fillAmount = 1;
            mutesw = false;

        }
    }
}
