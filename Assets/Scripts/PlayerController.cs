using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController CurrentPlayerController;

    [SerializeField] float limitX; //yatay x limiti.
    public float xSpeed; //swipe
    public float runnigSpeed;
    private float _currentRunningSpeed;
    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders; //silindirleri depoladığımız liste.
    public GameObject bridgePieces; //köprüde yürürken kullanacağı parçalar.
    private bool _isSpawningBridge; //true da köprü oluşturuyor olacak. false oluşturmuyor olacak.
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;



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

        //köprü yaratıp yaratmadığını kontrol;
        if (_isSpawningBridge)
        {
            _creatingBridgeTimer -= Time.deltaTime; //bu değişkenden geriye saniye sayıyormuş gibi oluyor. Her karede değilde belli bir zaman aralığında parçaları koymaya yarayacak.
            if (_creatingBridgeTimer < 0) //0dan küçük ise yeni köprü parçasını yaratıcaz
            {
                _creatingBridgeTimer = 0.01f;
                IncraseCylinderVolume(-0.01f);
                GameObject createdBridgePiece = Instantiate(bridgePieces);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction; //objenin yönünü köprünün yönüyle eşitliyoruz.
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            IncraseCylinderVolume(0.1f); //10 tane add cylinder var. Value max 1 yapacağım. Dolayısıyla 0.1*10 = 1;
            Destroy(other.gameObject);
        }
        else if (other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "StopSpawnBridge")
        {
            StopSpawningBridge();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Trap")
        {
            IncraseCylinderVolume(-Time.fixedDeltaTime); // ontriggerstay fizik ile çalıştığı için içinde kaldığı zamanla azaltma yapmamız lazım.
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

    public void StartSpawningBridge(BridgeSpawner bridgeSpawner)
    {
        _bridgeSpawner = bridgeSpawner;
        _isSpawningBridge = true;
    }

    public void StopSpawningBridge()
    {
        _isSpawningBridge = false;
    }
}
