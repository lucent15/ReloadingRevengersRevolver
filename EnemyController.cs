using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour//敵キャラクターのパラメータや制御
{
    //体力、攻撃関連
    public bool gatringgun;
    public int enemylife;
    public int ammocapacity;
    public float reloadtime;
    private bool reloading;
    public float randomshotspan;//攻撃を不規則に
    private float currentTime = 0f;
    private PlayerController placon;
    private int actiondice;//行動を決める
    //確定ダメージ関連
    public float truehitval;//カウンタで増える値を入れる枠
    public float truehitmax;//固定だが調整用に変数とする。６秒で確実に当たるものが最低1発は出る
    public float truehitrate;//ランダムで変動するカウンタが増える値の量。1秒ごとに追加される値に追加で加算する値。

    private bool actioning;//移動ちゅうなどでオン。ズレ防ぎ
    //カバー関連
    public bool covertype;//カバーしない敵はオフ。棒立ちの代わりにtruehitrateが常に増える。
    private bool covering;
    [System.NonSerialized]
    public float covertime;//カウンタで増える値を入れる枠

    public bool coververticalmove;//上下にカバー移動する
    //マズルフラッシュ,警告マーク
    public GameObject muzzleflash;
    private Animator mzlflshanim;
    public GameObject truehitinfo;
    private SpriteRenderer imgtruehitinfo;

    //ダメージ処理用画像とか
    private SpriteRenderer mysprite;
    private bool deathing = false;

    private TotalGameDirector gamdir;

    void Start()
    {
        placon = GameObject.Find("PlayerDirector").GetComponent<PlayerController>();
        truehitval = 0;
        truehitrate = Random.Range(0.001f, 0.05f);//ランダム関数
        randomshotspan = Random.Range(0.6f, 1);
        if (gatringgun) randomshotspan = 0.2f;
        if (!covertype)
        {
            reloadtime -= 2;
            truehitrate += 0.003f;
        }

        imgtruehitinfo = truehitinfo.GetComponent<SpriteRenderer>();
        imgtruehitinfo.enabled = false;
        mzlflshanim = muzzleflash.GetComponent<Animator>();


        mysprite = GetComponent<SpriteRenderer>();

        gamdir = GameObject.Find("GameDirector").GetComponent<TotalGameDirector>();
        gamdir.EnemyReport();

    }
    private void Update()
    {
        // Debug.Log(Time.deltaTime);

        if (enemylife <= 0 && !deathing)//ライフがゼロになったら死ぬ
        {
            StartCoroutine(DeathEnshutu());
            deathing = true;
        }

        if (gamdir.GetFireOrder()) { currentTime += Time.deltaTime;//ゲーム開始受け取り
            truehitval += (Time.deltaTime + truehitrate);
        }
        
        if (currentTime > randomshotspan && enemylife > 0)//撃つまでの時間がたまるか田舎
        {
            if (!gatringgun) randomshotspan = Random.Range(0.6f, 1);
            actiondice = Random.Range(1, 7);
            if (actiondice > 2 && ammocapacity > 0 && !covering)//行動決定：射撃
            {
                StopCoroutine(EnemyReloading());
                ammocapacity--;
                if (truehitmax <= truehitval && !actioning)//確定ダメが実行される値がたまるかいなか
                {
                    StartCoroutine(EnemyTrueHitShot());
                    actioning = true;
                }
                else
                {
                    mzlflshanim.Play("shot");
                    SoundManager.instance.PlaySE(SoundManager.SE_Type.ENEMYSHOT);//ダミー射撃
                }
            }
            else if (ammocapacity == 0 && !reloading && !actioning)//弾切れになったらリロード
            {
                StartCoroutine(EnemyReloading());
                if (covertype) EnemyCovering();
                reloading = true;
            }
            else if (covertype && actiondice <= 2 && !covering && !reloading && !actioning)//遮蔽物に隠れる
            {
                StartCoroutine(EnemyTacCovering());
                covering = true;
            }
            currentTime = 0;
        }
    }

    IEnumerator EnemyReloading()//リロード
    {
        yield return new WaitForSeconds(reloadtime);
        if (!gatringgun) ammocapacity = 6; else if (gatringgun) { ammocapacity = 30; }
        truehitval = 0;
        currentTime = 0;
        reloading = false;
        if (covertype) EnemyShowingFace();
    }

    IEnumerator EnemyTrueHitShot()//確定ヒット処理
    {
        imgtruehitinfo.enabled = true;
        yield return new WaitForSeconds(0.7f);
        mzlflshanim.Play("shot");
        SoundManager.instance.PlaySE(SoundManager.SE_Type.ENEMYSHOT);

        if (placon.showingface && enemylife > 0)
        {
            placon.Damage();
        }
        else if (!placon.showingface && enemylife > 0)
        {
            placon.CoverShaking();
        }
        truehitval = 0;
        currentTime = 0;
        truehitrate = Random.Range(0.001f, 0.05f);
        actioning = false;
        imgtruehitinfo.enabled = false;
    }

    IEnumerator EnemyTacCovering()//カバー出入り
    {
        EnemyCovering();
        covertime = Random.Range(1, 4);
        yield return new WaitForSeconds(covertime);
        EnemyShowingFace();
        yield return new WaitForSeconds(0.3f);
        covering = false;
        truehitval = (truehitval * 2) / 3;
    }

    void EnemyCovering()//カバーに隠れる
    {

        if (coververticalmove)
        {
            transform.DOMoveY(transform.position.y - 30, 0.2f);
        }
        else
        {
            transform.DOMoveX(transform.position.x + 30, 0.2f);
        }
    }

    void EnemyShowingFace()//カバーから出る
    {
        if (coververticalmove)
        {
            transform.DOMoveY(transform.position.y + 30, 0.2f);
        }
        else
        {
            transform.DOMoveX(transform.position.x - 30, 0.2f);
        }
    }

    public void EnemyDamage(int damage)
    {
        enemylife -= damage;
        if (enemylife > 0) { StartCoroutine(EnemyDamShaker()); }

    }

    IEnumerator EnemyDamShaker()//ダメージ受けた時のリアクション
    {
        var tenmetu = 0.05f;
        mysprite.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator DeathEnshutu()//死亡演出
    {
        var tenmetu = 0.1f;

        var boisu = Random.Range(1, 4);
        if (Random.Range(1, 100) > 96)
        {
            SoundManager.instance.PlaySE(SoundManager.SE_Type.WILLHELM);
        }
        else
        {
            if (boisu == 1)
            {
                SoundManager.instance.PlaySE(SoundManager.SE_Type.ENEMYDEAD1);

            }
            else if (boisu == 2)
            {
                SoundManager.instance.PlaySE(SoundManager.SE_Type.ENEMYDEAD2);

            }
            else if (boisu == 3)
            {
                SoundManager.instance.PlaySE(SoundManager.SE_Type.ENEMYDEAD3);

            }
        }
        mysprite.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = Color.clear;
        yield return new WaitForSeconds(tenmetu);
        mysprite.color = new Color(1, 1, 1, 1);

        gamdir.DeadReport();
        this.gameObject.SetActive(false);

    }



}
