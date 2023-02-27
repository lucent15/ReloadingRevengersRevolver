using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class TotalGameDirector : MonoBehaviour
{

    
    public Text timertext;
    public Text infotext;
    public GameObject infowindow;
    public GameObject ufo;
    private SpriteRenderer ufoimg;
    public static bool saainfounlock;

    public GameObject bigfade;
    public Text resulttexttitle;
    public Text resulttextscore;

    public float totalTimemax;
    public float totalTime;
    private bool timerswitcher;

    public float totaltimeforscore;

    public bool fireorder;

    public int onestagetotalenemy;

    private int hitcount;
    private int headshotcount;
    private int shotsfirecount;

    private float tempreloadtime;
    private float fastreloadtime;

    private float[] everyextincttime = new float[5]; //save alltime-totaltime
    private float[] everyaccuracyrate = new float[5];//save shotsfirecount/hitcount
    private int[] everyheadshotval = new int[5];// save headshotcount
    private float[] everyfastreloadtime = new float[5];// save fast reloadtime

    private float totalextincttime;
    private float totalaccuracyrate;
    private int totalheadshotval;
    private float mostfastreloadtime;


    public int stagenum;

    public GameObject[] stage1object;
    public GameObject[] stage2object;
    public GameObject[] stage3object;
    public GameObject[] stage4object;
    public GameObject[] stage5object;


    private bool retrydecide;

    private PlayerController placon;

    public Text clearresulttext;

    public bool shootingrange;

    public bool pause;

    public GameObject bgmslider;
    public GameObject seslider;

    private bool cleard;


    public GameObject title;

    public AudioMixer audioMixer;
    public static float sevolume;
    public static float bgmvolume;
     

    void Start()
    {
        if (!shootingrange)
        {
            fireorder = false;
            StartCoroutine(StartCountDown());

            SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Dusk_Flutters);
        }
        bigfade.SetActive(true);
        bigfade.transform.DOLocalMoveX(1000, 0.2f);
        resulttextscore.enabled = false;
        resulttexttitle.enabled = false;
        placon = GameObject.Find("PlayerDirector").GetComponent<PlayerController>();

        if (shootingrange)
        {
            fireorder = true;
        }

        SliderVisible(false);
        ufoimg = ufo.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (timerswitcher)
        {
            totalTime -= Time.deltaTime;
            totaltimeforscore += Time.deltaTime;
        }

        timertext.text = totalTime.ToString("f2");

        if (totalTime <= 0) { timerswitcher = false; }

        if (onestagetotalenemy == 0 && fireorder && !shootingrange)
        {
            bigfade.transform.DOLocalMoveX(-0, 0.5f);
            StartCoroutine(ResultStarter());
            fireorder = false;
        }
        if (retrydecide)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SoundManager.instance.StopBGM();

                SceneManager.LoadScene("GameScene");

            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                BackTitle();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && fireorder)
        {
            if (!pause)
            {
                //                infowindow.transform.DOScaleY(6, 0.001f);
                infowindow.transform.localScale = new Vector3(1, 6, 1);

                Time.timeScale = 0;
                Cursor.visible = true;
                pause = true;
                placon.ReticleVisible(false);
                placon.MouseFollowervisible(false);
                infotext.enabled = true;
                SliderVisible(true);
                infotext.text = "PAUSE\n\nEscで戻る";
            }
            else if (pause)
            {
                Time.timeScale = 1;
                pause = false;
                Cursor.visible = false;

                placon.ReticleVisible(true);
                placon.MouseFollowervisible(true);
                infotext.text = "";
                infotext.enabled = true;
                infowindow.transform.DOScaleY(0, 0.2f);
                SliderVisible(false);


            }
        }

        if (stagenum == 4 && fireorder && totaltimeforscore > 6 && ufo.activeSelf)
        {
            ufo.transform.position += new Vector3(-0.7f, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.R) && cleard)
        {
            SoundManager.instance.StopBGM();
            Cursor.visible = true;
            SceneManager.LoadScene("TitleScene");
        }
    }

    void SliderVisible(bool onoff) { seslider.SetActive(onoff); bgmslider.SetActive(onoff); title.SetActive(onoff); }

    IEnumerator StartCountDown()
    {
        infowindow.transform.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(0.2f);
        var onesecond = new WaitForSeconds(0.7f);
        infotext.text = "制限時間以内に敵を全て倒せ！";
        placon.FULLLOAD();
        yield return onesecond;
        infotext.text = "3";
        yield return onesecond;
        infotext.text = "2";
        yield return onesecond;
        infotext.text = "1";
        yield return onesecond;
        infotext.text = "ACTION!";
        yield return new WaitForSeconds(0.7f);
        infotext.text = "";
        infotext.enabled = false;
        infowindow.transform.DOScaleY(0, 0.2f);
        timerswitcher = true;
        fireorder = true;
    }

    public void GameOver()
    {
        bigfade.transform.DOLocalMoveX(-0, 0.5f);
        StartCoroutine(GameOverCoRoutine());
    }
    IEnumerator GameOverCoRoutine()
    {
        var offset = new WaitForSeconds(0.7f);
        yield return offset;
        infotext.enabled = true;
        infotext.text = "GAME OVER";
        yield return new WaitForSeconds(1);
        infotext.text += "\n" + "Retry?";
        yield return offset;
        infotext.text += "\n" + "y/n";
        retrydecide = true;
    }



    IEnumerator ResultStarter()
    {
        if (stagenum == 5)
        {
        }
        timerswitcher = false;
        var offset = new WaitForSeconds(0.3f);
        yield return offset;
        resulttextscore.enabled = true;
        resulttexttitle.enabled = true;
        resulttexttitle.text = "ステージクリアタイム\n";
        resulttextscore.text = totaltimeforscore.ToString("f2") + "\n";
        yield return offset;
        resulttexttitle.text += "命中率\n";
        resulttextscore.text += "" + (((float)hitcount / (float)shotsfirecount) * 100).ToString("f0") + "%\n";
        yield return offset;
        resulttexttitle.text += "ヘッドショット回数\n";
        resulttextscore.text += "" + headshotcount + "回\n";
        yield return offset;
        resulttexttitle.text += "最短リロード時間\n";
        if (fastreloadtime != 0) resulttextscore.text += fastreloadtime.ToString("f2") + "\n"; else { resulttextscore.text += "NO RECORD"; }
        yield return new WaitForSeconds(1.2f);
        infotext.enabled = true;
        if (stagenum != 5) infotext.text = "GO TO NEXT STAGE"; else if (stagenum == 6) { infotext.text = ""; }
        yield return new WaitForSeconds(2);
        ScoreSaving(stagenum);
        DeleteRecord();
        stagenum++;//ステージ番号の上昇
        fireorder = false;//うちかたやめー
        if (stagenum == 6)
        {
            StartCoroutine(ClearTotalResult());
        }
        if (stagenum != 6) placon.CoverImgChange(stagenum);
        if (stagenum != 6) StageChange();
    }

    public void StageChange()
    {
        totalTime = totalTimemax;//タイマーリセット
        totaltimeforscore = 0;

        //現行の番号のオブジェクトをfor分で全て非表示。その後次の番号のオブジェクトをfor分で術tえ表示。
        if (stagenum == 2)
        {
            for (int i = 0; stage1object.Length > i; i++)
            {
                stage1object[i].SetActive(false);
            }
            for (int i = 0; stage2object.Length > i; i++)
            {
                stage2object[i].SetActive(true);
            }
        }
        if (stagenum == 3)
        {
            for (int i = 0; stage2object.Length > i; i++)
            {
                stage2object[i].SetActive(false);
            }
            for (int i = 0; stage3object.Length > i; i++)
            {
                stage3object[i].SetActive(true);
            }
        }
        if (stagenum == 4)
        {
            for (int i = 0; stage3object.Length > i; i++)
            {
                stage3object[i].SetActive(false);
            }
            for (int i = 0; stage4object.Length > i; i++)
            {
                stage4object[i].SetActive(true);
            }
            totalTime += 10;
        }
        if (stagenum == 5)
        {
            for (int i = 0; stage4object.Length > i; i++)
            {
                stage4object[i].SetActive(false);
            }
            for (int i = 0; stage5object.Length > i; i++)
            {
                stage5object[i].SetActive(true);
            }
            totalTime += 30;
        }


        if (stagenum != 6)
        {
            bigfade.transform.DOLocalMoveX(1000, 0.2f);
            resulttextscore.enabled = false;
            resulttexttitle.enabled = false;
            infotext.text = "";
            StartCoroutine(StartCountDown());
        }
    }

    IEnumerator ClearTotalResult()
    {
        resulttexttitle.text = "";
        resulttextscore.text = "";
        ClearResultGathering();
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.CLEAR);
        timerswitcher = false;
        var offset = new WaitForSeconds(2);
        yield return new WaitForSeconds(2.5f);
        clearresulttext.text += "REVENGE REDEEMED\n\n";
        yield return offset;
        clearresulttext.text += "トータルクリアタイム\n";
        clearresulttext.text += totalextincttime.ToString("f2") + "\n\n";
        yield return offset;
        clearresulttext.text += "平均命中率\n";
        clearresulttext.text += "" + totalaccuracyrate.ToString("f0") + "%\n\n";
        yield return offset;
        clearresulttext.text += "総合ヘッドショット数\n";
        clearresulttext.text += "" + totalheadshotval + "回\n\n";
        yield return offset;
        clearresulttext.text += "最短リロード時間\n";
        clearresulttext.text += mostfastreloadtime.ToString("f2") + "\n\n";
        yield return offset;
        clearresulttext.text += "残りHP\n";
        if (placon.GetLife() == 3)
        {
            clearresulttext.text += "ノーダメージ！\n\n";
        }
        else
        {
            clearresulttext.text += placon.GetLife() + "/3\n\n";
        }
        yield return offset;
        if (saainfounlock) clearresulttext.text += "UFO撃墜:「ピースメーカーとは」解放\n\n";
        yield return offset;
        clearresulttext.text += "THANK YOU FOR PLAYING!\n\n";
        yield return offset;
        clearresulttext.text += "PRESS 'R' BACK TO TITLE";
        cleard = true;

    }

    void ClearResultGathering()
    {
        /*totalextincttime
       * totalaccuracyrate
       * totalheadshotval
       * mostfastreloadtime
        */
        float tempacurasi = 0;
        for (int i = 0; everyextincttime.Length > i; i++)
        {
            totalextincttime += everyextincttime[i];
        }
        for (int i = 0; everyaccuracyrate.Length > i; i++)
        {
            tempacurasi += everyaccuracyrate[i];
        }
        totalaccuracyrate += tempacurasi / everyaccuracyrate.Length;

        for (int i = 0; everyheadshotval.Length > i; i++)
        {
            totalheadshotval += everyheadshotval[i];
        }
        Array.Sort(everyfastreloadtime);
        var temp = 0;
        for (int i = 0; everyfastreloadtime[i] == 0; i++)
        {
            temp = i;
        }
        mostfastreloadtime = everyfastreloadtime[temp + 1];
    }

    public int GetStageNum() { return stagenum; }
    public bool GetFireOrder() { return fireorder; }
    public void EnemyReport() { onestagetotalenemy++; }
    public void DeadReport() { onestagetotalenemy--; }
    public void ShotsFireCount() { shotsfirecount++; }
    public void HitCount() { hitcount++; }
    public void HeadShotCount() { headshotcount++; }
    public void RecordingReloadTime(float reloadtime)
    {
        tempreloadtime = reloadtime;
        if (tempreloadtime < fastreloadtime || fastreloadtime == 0)
        {
            fastreloadtime = tempreloadtime;
        }
    }

    public void BackTitle()
    {
        SoundManager.instance.StopBGM();
        Cursor.visible = true;
        SceneManager.LoadScene("TitleScene");
    }

    public void ScoreSaving(int stagenum) //every
    {
        everyextincttime[stagenum - 1] = totaltimeforscore;
        totalextincttime += totaltimeforscore;
        everyaccuracyrate[stagenum - 1] = (((float)hitcount / (float)shotsfirecount) * 100);
        everyheadshotval[stagenum - 1] = headshotcount;
        everyfastreloadtime[stagenum - 1] = fastreloadtime;
    }

    public float GetReloadTime()
    {
        return tempreloadtime;
    }

    public void DeleteRecord()
    {
        fastreloadtime = 0;
        tempreloadtime = 0;
        shotsfirecount = 0;
        hitcount = 0;
        headshotcount = 0;
        onestagetotalenemy = 0;
    }

    public void UFODEATH()
    {
        StartCoroutine(UFOBREAK());
        saainfounlock = true;
    }

    IEnumerator UFOBREAK()
    {
        var tenmetu = 0.1f;

        ufoimg.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        ufoimg.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(tenmetu);
        ufoimg.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        ufoimg.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(tenmetu);
        ufoimg.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        ufoimg.color = new Color(1, 1, 1, 1);
        ufo.SetActive(false);
    }

    public void SetAudioMixerVolume(float vol)
    {
        if (vol == 0)
        {
            audioMixer.SetFloat("volumeSE", -80);
        }
        else
        {
            audioMixer.SetFloat("volumeSE", 0);
        }
    }
    public void SetBGMMixerVolume(float volume)
    {
        audioMixer.SetFloat("BGMVol", volume);
        sevolume = volume;
    }
    public void SetSEMixerVolume(float volume)
    {
        audioMixer.SetFloat("SEVol", volume);
        bgmvolume = volume;

    }
    public bool GetShootingRange()
    {
        return shootingrange;
    }

}
