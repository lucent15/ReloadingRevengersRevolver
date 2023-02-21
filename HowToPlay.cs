using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public Text shotmodetext;
    public Text reloadmodetext;
    public Text enemytext;
    private string forsave1;
    private string forsave2;

    public Text reloadtime;

    public GameObject howtopanel;

    PlayerController placon;
    TotalGameDirector gamdir;
    void Start()
    {
        forsave1 = shotmodetext.text;
        forsave2 = reloadmodetext.text;
        shotmodetext.enabled = true;
        reloadmodetext.enabled = false;
        placon = GameObject.Find("PlayerDirector").GetComponent<PlayerController>();
        gamdir = GameObject.Find("GameDirector").GetComponent<TotalGameDirector>();

    }

    void Update()
    {
        if (placon.GetReloadState() == true)
        {

            shotmodetext.enabled = false;
            reloadmodetext.enabled = true;
            reloadmodetext.text = forsave2;
        }

        if (placon.GetReloadState() == false)
        {
            shotmodetext.enabled = true;
            reloadmodetext.enabled = false;
            shotmodetext.text = forsave1;

        }
        reloadtime.text = gamdir.GetReloadTime().ToString("f2") + "";

        if (Time.timeScale == 0)
        {
            howtopanel.SetActive(false);
        }
        else
        {
            howtopanel.SetActive(true);
        }

        if (Input.GetMouseButton(1))
        {
            shotmodetext.enabled = false;
            reloadmodetext.enabled = false; 
            enemytext.enabled = true;
        }else
        {
                     enemytext.enabled = false;
        }
    }
}
