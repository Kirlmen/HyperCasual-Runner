using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController CurrentPlayerController;


    [SerializeField] public Animator animator;
    [SerializeField] float limitX; //yatay x limiti.
    [SerializeField] float cylinderDecreasingValue = 0.1f;

    public float xSpeed; //swipe
    public float runnigSpeed;
    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders; //silindirleri depoladığımız liste.
    public GameObject bridgePieces; //köprüde yürürken kullanacağı parçalar.
    public AudioSource cylinderSound, triggerSound, itemSound;
    public AudioClip gatherClip, dropClip, coinClip, buyClip, equipItemClip, unequipItemClip;
    public List<GameObject> wearSpots;


    private float _dropSoundTimer;
    private bool _isSpawningBridge; //true da köprü oluşturuyor olacak. false oluşturmuyor olacak.
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;
    private float _currentRunningSpeed;
    private bool _isFinished;
    private float _scoreTimer;
    private float _lastTouchedX;




    void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.isGameActive)
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width; //fixed movement
                _lastTouchedX = Input.GetTouch(0).position.x;
            }

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
            PlayDropSound();
            _creatingBridgeTimer -= Time.deltaTime; //bu değişkenden geriye saniye sayıyormuş gibi oluyor. Her karede değilde belli bir zaman aralığında parçaları koymaya yarayacak.
            if (_creatingBridgeTimer < 0) //0dan küçük ise yeni köprü parçasını yaratıcaz
            {
                _creatingBridgeTimer = 0.1f;
                IncreaseCylinderVolume(-cylinderDecreasingValue);
                GameObject createdBridgePiece = Instantiate(bridgePieces, this.transform);
                createdBridgePiece.transform.SetParent(null);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction; //objenin yönünü köprünün yönüyle eşitliyoruz.
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;

                if (_isFinished)
                {
                    _scoreTimer -= Time.deltaTime; //geriye saymaya başla.
                    if (_scoreTimer <= 0)
                    {
                        _scoreTimer = 0.3f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }


    }

    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            cylinderSound.PlayOneShot(gatherClip, 0.1f);
            IncreaseCylinderVolume(0.1f); //10 tane add cylinder var. Value max 1 yapacağım. Dolayısıyla 0.1*10 = 1;
            Destroy(other.gameObject);
        }
        else if (other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "StopSpawnBridge")
        {
            StopSpawningBridge();
            if (_isFinished)
            {
                LevelController.Current.FinishGame();
            }

        }
        else if (other.tag == "Finish")
        {
            _isFinished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "Coin")
        {
            triggerSound.PlayOneShot(coinClip, 0.1f);
            other.tag = "Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.isGameActive)
        {
            if (other.tag == "Trap")
            {
                PlayDropSound();
                IncreaseCylinderVolume(-Time.fixedDeltaTime); // ontriggerstay fizik ile çalıştığı için içinde kaldığı zamanla azaltma yapmamız lazım.
            }
        }

    }


    public void IncreaseCylinderVolume(float value) //silindirin hacmini arttır.
    {
        if (cylinders.Count == 0) //ayağımızın altında hiç silindir yoksa;
        {
            if (value > 0) //silindiri toplayarak arttırmaya çalışıyorsak;
            {
                CreateRidingCylinder(value); //girilen değer büyüklüğünde bir silindir yarat.
            }
            else
            {
                if (_isFinished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    StartCoroutine(Dead());
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncraseCylinderVolume(value); //ayağımızda silindir varsa en aşağıdaki elemanın(listenin en sonundaki) boyutunu güncelleyeceğiz. girilen value kadar. 
        }
    }


    IEnumerator Dead()
    {
        animator.SetBool("dead", true);
        LevelController.Current.GameOver();
        Camera.main.transform.SetParent(null); //ölünce kamera takip etmesin.
        yield return new WaitForSeconds(2.3f);
        GameObject hidden = GameObject.FindGameObjectWithTag("HiddenPlatform");
        hidden.SetActive(false);

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


    public void PlayDropSound()
    {
        _dropSoundTimer -= Time.deltaTime; //geri sayma
        if (_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;
            cylinderSound.PlayOneShot(dropClip, 0.1f);
        }
    }
}
