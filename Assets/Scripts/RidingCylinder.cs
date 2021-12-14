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
            float leftValue = currentCylinderValue - 1; // tam halinden fazla kalan hali bulmak için.
            int cylindersCount = PlayerController.CurrentPlayerController.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylindersCount - 1) - 0.25f, transform.localPosition.z); //son elemanı -0.5f ile çarparak bir önceki silindirin en altına indir.zaten bu kısma tam boyutta geldiği için -0.25 ile çarparak yerini konumlanıdr.
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);//en büyük halindeki scale'i.
            PlayerController.CurrentPlayerController.CreateRidingCylinder(leftValue); //kalan değer kadar yeni bir silindir oluştur.
        }
        else if (currentCylinderValue < 0)
        {
            PlayerController.CurrentPlayerController.DestroyCylinder(this);//karakterimize silindiri yok etmesini söyle. kendini listeden çıkar. eksil.
        }
        else // silindir hacim kaybetti ama yok olmadı veya tam dolmadı. Burada pozisyon ve boyutunu güncelleyeceğiz.
        {
            int cylindersCount = PlayerController.CurrentPlayerController.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylindersCount - 1) - 0.25f * currentCylinderValue, transform.localPosition.z); //şı anki silindirin boyutuyla çarpıyoruz bu kısımda.
            transform.localScale = new Vector3(0.5f * currentCylinderValue, transform.localScale.y, 0.5f * currentCylinderValue);
        }
    }
}
