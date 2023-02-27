using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverAnimationOffset : MonoBehaviour
{

    PlayerController placon;
        void Start()
    {
        placon = GameObject.Find("PlayerDirector").GetComponent<PlayerController>();
    }

  //animation eventでつかう
    void OffsetWheelAnimation(){placon.RevolverWheeling();}
    void OffsetWheelAnimationEnd() { placon.RevolverWheelingEnd(); }
    void OffsetExtractionCartridge() { placon.ExtractionCartridge(); }
    void OffsetLoadingCartridge() { placon.LoadingCartridge(); }
}
