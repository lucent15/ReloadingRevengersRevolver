using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaller : MonoBehaviour   // 敵キャラの部位ごとのダメージを分けるためのクラス
{
    EnemyController enecon;
    private int bodydamage;
    private TotalGameDirector gamdir;

 
    void Start()
    {
        enecon = transform.root.gameObject.GetComponent<EnemyController>();
        if (transform.name == "HEAD")
        {
            bodydamage = 2;

        }
        if (transform.name == "BODY")
        {
            bodydamage = 1;
        }

        gamdir = GameObject.Find("GameDirector").GetComponent<TotalGameDirector>();

    }

    // Update is called once per frame
    public void EnemyDamageCalling()
    {
        if (transform.name == "HEAD")
        {
            gamdir.HeadShotCount();
            gamdir.HitCount();
        }
        if (transform.name == "BODY")
        {
            gamdir.HitCount();
        }
        enecon.EnemyDamage(bodydamage);
    }
}
