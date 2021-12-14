using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool isFilled; // silindir en büyük haline gelip gelmediğin kontrol eden bool.
    private float currentCylinderValue; // şu anki silindir hacim büyüklüğü.

    public void IncraseCylinderVolume(float value) //silindir boyutunu arttırma ve küçültme methodu.
    {
        currentCylinderValue += value;
        if (currentCylinderValue > 1) //silindir en büyük haline geldiyse;
        {
            float leftValue = value - 1; // tam halinden fazla kalan hali bulmak için.
            //silindirin boyutunu 1e eşitle.
            PlayerController.CurrentPlayerController.CreateRidingCylinder(leftValue); //kalan değer kadar yeni bir silindir oluştur.
        }
        else if (currentCylinderValue < 0)
        {
            PlayerController.CurrentPlayerController.DestroyCylinder(this);//karakterimize silindiri yok etmesini söyle. kendini listeden çıkar. eksil.
        }
        else // silindir hacim kaybetti ama yok olmadı.
        {
            //silindirin boyutunu güncelle.
        }
    }
}
