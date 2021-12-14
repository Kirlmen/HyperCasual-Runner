using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController CurrentPlayerController;

    [SerializeField] float limitX;
    public float xSpeed;
    public float runnigSpeed;
    private float _currentRunningSpeed;
    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders; //silindirleri depoladığımız liste.

    public

    void Start()
    {
        CurrentPlayerController = this;
        _currentRunningSpeed = runnigSpeed;
    }

    void Update()
    {
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount < 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if (Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);


        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            IncraseCylinderVolume(0.2f);
            Destroy(other.gameObject);
        }
    }


    public void IncraseCylinderVolume(float value) //silindirin hacmini arttır.
    {
        if (cylinders.Count == 0) //ayağımızın altında hiç silindir yoksa;
        {
            if (value > 0) //silindiri toplayarak arttırmaya çalışıyorsak;
            {
                CreateRidingCylinder(value); //girilen değer büyüklüğünde bir silindir yarat.
            }
            else
            {
                //gameover.
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncraseCylinderVolume(value); //ayağımızda silindir varsa en aşağıdaki elemanın(listenin en sonundaki) boyutunu güncelleyeceğiz. girilen value kadar. 
        }
    }

    public void CreateRidingCylinder(float value) //silindir yarat. ne kadar büyük olması gerektiği de parametreden geliyor.
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>(); //prefabi bir variableda tutuyoruz. Objeyi burada yaratıp ridingcylinder componentında referans yapacağız.
        cylinders.Add(createdCylinder); //yarattığımız prefabi listeye ekliyoruz.
        createdCylinder.IncraseCylinderVolume(value); //yarattığımız silindirin boyutunu güncellemek için.
    }

    public void DestroyCylinder(RidingCylinder cylinder) //silindiri yok etmek için o silindiri listeden çıkar.
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject); //listeden çıkardığımız silindiri sahneden yok ediyoruz.
    }

}
