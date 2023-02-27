using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //プレイヤーライフ
    public int playerlife;
    public bool showingface;
    public GameObject[] lifecard = new GameObject[3];

    public bool invincibling;
    
    //装弾数
    [HideInInspector] public int ammocount = 6;
    public Text UIammocounter;

    //クリックシューティング処理関連
    public GameObject reticle;
    public GameObject mousefollower;

    SpriteRenderer reticleimg;
    SpriteRenderer musfollowerimg;
    
    private Vector2 mouse;
    private Vector3 target;
    GameObject clickedGameObject;
    
    private bool cocked;

    public GameObject muzzleflash;
    private Animator mzlflshanim;

    //プレイヤーアニメ関連
    public GameObject Gunslinger;
    private Animator gunslingeranim;

    private bool damageanimating;


    //シリンダー処理関連
    public GameObject revolver;
    private Vector3 reloadpos;
    private Animator revolveranim;
    public int[] revolverstate = new int[12];//シリンダーホールと装填する弾を意味する。
    public GameObject[] loadedcartridge = new GameObject[12];//装填されてる弾の画像
    public GameObject[] emptycartridge = new GameObject[12];//撃った後の弾の画像
    public bool reloading;//リロードの時オンになる。
    private int fullloadchecker;

    public bool animationinplaying;//アニメーション再生中を知らせる。

    public GameObject UpCartridge;
    private Animator upcrtrganim;

    public GameObject playercover;

    public GameObject[] playercovers;

    private Vector3 coverfirstpos;

    public GameObject hitflash;
    private Animator hitflashanim;

    private int rollingtimes;

    private TotalGameDirector gamdir;

    private bool liveordie;
    private int emptychecker;
    private bool reloadtimecheck;
    private float reloadtimecount;
    
    
    void Start()
    {
        Cursor.visible = false;
        gunslingeranim = Gunslinger.GetComponent<Animator>();
        revolveranim = revolver.GetComponent<Animator>();
        reloadpos = revolver.transform.position;
        upcrtrganim = UpCartridge.GetComponent<Animator>();

        for (int i = 0; i < revolverstate.Length; i++)
        {
            if (i % 2 == 1)
            {
                revolverstate[i] = 1;
            }
            else
            {
                revolverstate[i] = 0;
            }
        }
        CartridgeVisualizing();
        RevolverWheeling();
        ShootPosOnRevolver();
        cocked = true;

        reticleimg = reticle.GetComponent<SpriteRenderer>();
        musfollowerimg = mousefollower.GetComponent<SpriteRenderer>();

        //coverfirstpos = playercover.transform.position;
        //coverfirstpos = playercovers[gamdir.GetStageNum()-1].transform.position;

        UIammocounter.text = ammocount + "/6";
        mzlflshanim = muzzleflash.GetComponent<Animator>();
        invincibling = false;

        hitflashanim = hitflash.GetComponent<Animator>();

        gamdir = GameObject.Find("GameDirector").GetComponent<TotalGameDirector>();
        liveordie = true;
        coverfirstpos = playercovers[gamdir.GetStageNum() - 1].transform.position;

    }

    public void FULLLOAD()
    {
        if (reloading) { RevolverWheeling(); reloading = false; }
        for (int i = 0; i < revolverstate.Length; i++)
        {
            if (i % 2 == 1)
            {
                revolverstate[i] = 1;
            }
            else
            {
                revolverstate[i] = 0;
            }
        }
        CartridgeVisualizing();
        RevolverWheeling();
        ShootPosOnRevolver();
        cocked = true;
        ammocount = 6;
    }

    void Update()
    {
        if (gamdir.GetFireOrder() && liveordie)
        {

            if (playerlife <= 0 && liveordie)
            {
                gamdir.GameOver();
                liveordie = false;
            }

            if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1) && !reloading)
            {
                if (Time.timeScale != 0) Cursor.visible = false;
                if (!cocked)
                {
                    HammerCockingWhenShots();
                }
            }

            mouse = Input.mousePosition;
            target = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 3));


            if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1) && !reloading)
            {
                if (Time.timeScale != 0) Cursor.visible = false;
                if (cocked)
                {
                    if (revolverstate[0] == 1)
                    {
                        ShotSAA();
                        cocked = false;
                    }
                    else if (revolverstate[0] != 1)
                    {
                        cocked = false;
                        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAEmpty);
                    }
                }
                else if (!cocked)
                {
                    HammerCockingWhenShots();
                }
            }

            if (Input.GetMouseButtonDown(1) && !reloading)
            {
                gunslingeranim.Play("Player_FaceUp");
                gunslingeranim.SetBool("aiming", true);
                if (Time.timeScale != 0) Cursor.visible = false;
            }

            if (Input.GetMouseButton(1) && !reloading)
            {
                AimState();
            }
            else
            {
                gunslingeranim.SetBool("aiming", false);
                HideState();
            }
            if (Input.GetMouseButtonUp(1)) gunslingeranim.Play("Player_FaceDown");

            if (Input.GetKeyDown(KeyCode.R) && animationinplaying == false && !Input.GetMouseButton(1))
            {
                if (reloading == false)
                {
                    reloading = true;
                    RevolverWheeling();
                    ReloadPosOnRevolver();
                    SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAhalfCock);
                    EmptyChecker();

                }
                else if (reloading == true)
                {
                    reloading = false;
                    ShootPosOnRevolver();
                    RevolverWheeling();
                    cocked = true;
                    SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAFastCock);


                }
            }
            if (reloading && Input.GetKeyDown(KeyCode.S) && animationinplaying == false && revolverstate[3] != 0)
            {
                revolveranim.Play("Revolver_Extraction");
                upcrtrganim.Play("UC_Extraction");
                animationinplaying = true;
            }
            if (reloading && Input.GetKeyDown(KeyCode.D) && animationinplaying == false)
            {
                revolveranim.Play("Revolver_Wheel");
                animationinplaying = true;
            }
            if (reloading && Input.GetKeyDown(KeyCode.W) && animationinplaying == false && revolverstate[3] == 0)
            {
                revolveranim.Play("Revolver_Loading");
                upcrtrganim.Play("UC_Loading");
                animationinplaying = true;
            }
            if (!cocked) { musfollowerimg.enabled = true; }

            //レティクルとマウス位置表示の追従
            //reticle.transform.position = target;
        }
        reticle.transform.position = Vector2.Lerp(reticle.transform.position, target, 50.0f * Time.deltaTime);
        mousefollower.transform.position = Vector2.Lerp(mousefollower.transform.position, target, 4.0f * Time.deltaTime);

        /*if (Input.GetKeyDown(KeyCode.A) && animationinplaying == false && !Input.GetMouseButton(1))
        {
            StartCoroutine(RouletteRevolver());
        }*/

        if (reloadtimecheck)
        {
            reloadtimecount += Time.deltaTime;
        }
        UIammocounter.text = ammocount + "/6";

    }

    public void ReticleVisible(bool onoff) { reticle.SetActive(onoff); }
    public void MouseFollowervisible(bool onoff) { mousefollower.SetActive(onoff); }

    void AimState()
    {

        reticleimg.enabled = true;
        showingface = true;
        if (cocked) musfollowerimg.enabled = false;
    }
    //常に追従させるようにして、レティクル画像の中央からレイを飛ばす。オンオフ切り替えはSpriteレンダラーのみにする
    void HideState()
    {
        reticleimg.enabled = false;
        musfollowerimg.enabled = true;
        showingface = false;
    }

    public void Damage()
    {
        if (!invincibling)
        {
            if(!gamdir.GetShootingRange())playerlife -= 1;
            SoundManager.instance.PlaySE(SoundManager.SE_Type.DAMAGE);
            if (playerlife >= 0&&!gamdir.GetShootingRange()) lifecard[playerlife].SetActive(false);
            if (!damageanimating)
            {
                StartCoroutine(DamageShake());
                damageanimating = true;
            }
            StartCoroutine(InvincibleTime());
        }
    }
    IEnumerator DamageShake()
    {
        Gunslinger.transform.DOShakePosition(1f, 5f);
        yield return new WaitForSeconds(1);
        damageanimating = false;
    }

    IEnumerator InvincibleTime()
    {
        invincibling = true;
        yield return new WaitForSeconds(2);
        invincibling = false;
    }

    public void CoverShaking()
    {
        StartCoroutine(CoverShaker());

    }
    IEnumerator CoverShaker()
    {
        //playercover.transform.DOShakePosition(1f, 5f);
        playercovers[gamdir.GetStageNum() - 1].transform.DOShakePosition(1f, 5f);
        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Ricochet);
        yield return new WaitForSeconds(0.8f);
        //  playercover.transform.position = coverfirstpos;
        playercovers[gamdir.GetStageNum() - 1].transform.position = coverfirstpos;


    }


    public void RevolverWheeling()//ハーフ回転。
    {
        var temp = revolverstate[revolverstate.Length - 1];
        // Debug.Log("temp is " + temp);
        for (int i = (revolverstate.Length - 1); i > 0; i--)
        {
            revolverstate[i] = revolverstate[i - 1];
            //Debug.Log(i + "番目は" + revolverstate[i]);
        }
        revolverstate[0] = temp;
        //Debug.Log("0番目は" + revolverstate[0]);
        CartridgeVisualizing();
    }

    public void HammerCockingWhenShots()
    {
        RevolverWheeling();
        RevolverWheeling();
        cocked = true;
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAFastCock);

    }

    IEnumerator RouletteRevolver()
    {
        rollingtimes = Random.Range(100, 200);

        for (int i = 0; rollingtimes >= i; i++)
        {
            revolveranim.Play("Revolver_Wheel");
            yield return new WaitForSeconds(0.005f);
        }
        rollingtimes = 0;
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAFastCock);
    }

    public void RevolverWheelingEnd()
    {
        RevolverWheeling();
        CartridgeVisualizing();
        animationinplaying = false;
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAWheeling);
    }

    void CheckFullLoad()//リロード時にフル装填されているかチェックする。
    {
        fullloadchecker = 0;
        for (int i = 0; i < revolverstate.Length; i++)
        {
            if (i % 2 == 1)
            {
                if (revolverstate[i] == 1) { fullloadchecker++; }
            }
            else
            {
                if (revolverstate[i] == 0) { fullloadchecker++; }
            }
        }
        if (revolverstate.Length == fullloadchecker)
        {
            if (reloading == true)
            {
                fullloadchecker = 0;
                ShootPosOnRevolver();
                reloading = false;
                RevolverWheeling();
                cocked = true;
                SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAFastCock);
                reloadtimecheck = false;
                gamdir.RecordingReloadTime(reloadtimecount);
                reloadtimecount = 0;

            }
        }
    }
    void EmptyChecker()
    {
        emptychecker = 0;
        for (int i = 0; i < revolverstate.Length; i++)
        {
            if (i % 2 == 1)
            {
                if (revolverstate[i] == 2) { emptychecker++; }
            }
            else
            {
                if (revolverstate[i] == 0) { emptychecker++; }
            }
        }
        if (revolverstate.Length == emptychecker)
        {
            reloadtimecheck = true;
        }
    }

    void ShotSAA()
    {
        //Debug.Log("SHOTS FIRED");
        clickedGameObject = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ray = new Ray(reticle.transform.position, reticle.transform.forward);
        // Ray ray= Camera.main.ScreenPointToRay(new Vector2(mouse.x, mouse.y));
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        if (hit2d)
        {
            clickedGameObject = hit2d.transform.gameObject;
            if (hit2d.transform.tag == "Enemy") hit2d.transform.GetComponent<DamageCaller>().EnemyDamageCalling();
            if (hit2d.transform.name == "ufo") gamdir.UFODEATH();
        }

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 4);

        mzlflshanim.Play("shot");

        revolverstate[0] = 2;
        ammocount--;
        // UIammocounter.text = ammocount + "/6";
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAShot);
        HitFlashIgnition();
        gamdir.ShotsFireCount();
    }

    void HitFlashIgnition()
    {
        hitflash.transform.position = new Vector3(reticle.transform.position.x,
            reticle.transform.position.y, reticle.transform.position.z + 0.01f);
        hitflashanim.Play("shot");
    }

    public void ExtractionCartridge()
    {
        if (revolverstate[3] == 1)
        {
            ammocount--;
            UIammocounter.text = ammocount + "/6";
        }
        revolverstate[3] = 0; CartridgeVisualizing();
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAAExtraction);
        animationinplaying = false;

    }
    public void LoadingCartridge()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SAALoading);
        if (revolverstate[3] == 0) { revolverstate[3] = 1; }
        animationinplaying = false;
        CartridgeVisualizing();
        CheckFullLoad();
        ammocount++;
        UIammocounter.text = ammocount + "/6";

    }
    void CartridgeVisualizing()
    {
        for (int i = 0; i < revolverstate.Length; i++)
        {
            if (revolverstate[i] == 0)
            {
                //何も画像を表示しない
                loadedcartridge[i].SetActive(false);
                emptycartridge[i].SetActive(false);
            }

            if (revolverstate[i] == 1)
            {
                //弾入りを表示

                loadedcartridge[i].SetActive(true);
                emptycartridge[i].SetActive(false);

            }

            if (revolverstate[i] == 2)
            {
                //弾切れを表示
                loadedcartridge[i].SetActive(false);
                emptycartridge[i].SetActive(true);
            }
        }
    }

    void ShootPosOnRevolver()
    {
        revolver.transform.DOMoveX(reloadpos.x - 200, 0.2f);
    }

    void ReloadPosOnRevolver()
    {
        revolver.transform.DOMoveX(reloadpos.x, 0.2f);
    }

    public void CoverImgChange(int stagenum)
    {
        playercovers[stagenum - 2].SetActive(false);
        playercovers[stagenum - 1].SetActive(true);
        coverfirstpos = playercovers[stagenum - 1].transform.position;
        if (stagenum == 5) { Gunslinger.transform.position = new Vector3(Gunslinger.transform.position.x, Gunslinger.transform.position.y - 10, Gunslinger.transform.position.z); }
    }

    public int GetLife()
    {
        return playerlife;
    }

    public bool GetReloadState()
    {
        return reloading;
    }
}
