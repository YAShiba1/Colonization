using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class Base : MonoBehaviour
{
    [SerializeField] private Transform _botSpawnPoint;
    [SerializeField] private Bot _botPrefab;
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private ColonyBase _colonyBasePrefab;
    [SerializeField] protected LayerMask LayerMask;

    protected List<Bot> Bots;
    protected Ray Ray;

    protected int GoldCount = 0;
    protected bool IsBaseSelected = false;

    private bool _isFlagSpawned = false;
    private Camera _mainCamera;
    private Queue<Gold> _golds;

    private Flag _flag;
    private Bot _botForNewColonyBase;

    public event UnityAction<int> GoldChanged;

    private void Awake()
    {
        Bots = new List<Bot>();
        _golds = new Queue<Gold>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        ScanArea();
        TrySpawnFlag();
        TrySelectBase();
        TryBuildNewColonyBase();
    }

    public void TryBuyNewBot()
    {
        int botPrice = 3;

        if (GoldCount >= botPrice)
        {
            SpawnBot();

            GoldCount -= botPrice;
            GoldChanged?.Invoke(GoldCount);
        }
        else
        {
            Debug.Log("Недостаточно gold.");
        }
    }

    public void TakeGold()
    {
        GoldCount++;
        GoldChanged?.Invoke(GoldCount);
    }

    protected void SpawnBot()
    {
        Vector3 spawnPosition = _botSpawnPoint.position;
        spawnPosition.y = _botPrefab.transform.position.y;

        Bot bot = Instantiate(_botPrefab, spawnPosition, Quaternion.identity);

        bot.SetParentBase(this);
        Bots.Add(bot);
    }

    protected virtual void TrySelectBase()
    {
        if (Input.GetMouseButtonDown(0) && IsBaseSelected == true)
        {
            IsBaseSelected = false;
        }
    }

    private void ResetGold()
    {
        GoldCount = 0;
        GoldChanged?.Invoke(GoldCount);
    }

    private void ScanArea()
    {
        Gold goldResource = _scanner.TryGetNextGold();

        if (goldResource != null)
        {
            _golds.Enqueue(goldResource);
        }

        if (_golds.Count > 0)
        {
            TryToSendFreeBotForGold();
        }
    }

    private void TryToSendFreeBotForGold()
    {
        Bot freeBot = TryGetFreeBot();

        while (freeBot != null && _golds.Count > 0 && _golds.Peek().IsReserved)
        {
            _golds.Dequeue();
        }

        if (freeBot != null && _golds.Count > 0)
        {
            _golds.Peek().Reserve();
            freeBot.SetTarget(_golds.Dequeue().transform);
        }
    }

    private Bot TryGetFreeBot()
    {
        Bot freeBot = null;

        if (Bots != null && Bots.Count > 0)
        {
            foreach (Bot bot in Bots)
            {
                if (bot.CurrentTarget == null)
                {
                    freeBot = bot;

                    break;
                }
            }
        }

        return freeBot;
    }

    private Flag CreateFlag(Vector3 spawnPosition)
    {
        Flag flag = Instantiate(_flagPrefab, spawnPosition, Quaternion.identity);

        return flag;
    }

    private void TrySpawnFlag()
    {
        int minimumNumberOfBaseBots = 1;

        if (IsBaseSelected == true && Input.GetMouseButtonDown(0) && Bots.Count > minimumNumberOfBaseBots)
        {
            if (Physics.Raycast(Ray, out RaycastHit hit, Mathf.Infinity, LayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject != null && hitObject.TryGetComponent(out Ground ground) && EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Vector3 flagSpawnPosition = new Vector3(hit.point.x, _flagPrefab.transform.position.y, hit.point.z);

                    if (_isFlagSpawned == true && _flag != null)
                    {
                        Destroy(_flag.gameObject);

                        _isFlagSpawned = false;
                    }

                    _flag = CreateFlag(flagSpawnPosition);
                    _isFlagSpawned = true;
                }
            }
        }
    }

    private void TrySendFreeBotToFlag()
    {
        if(_isFlagSpawned == true)
        {
            _botForNewColonyBase = TryGetFreeBot();

            if(_botForNewColonyBase != null && _flag != null)
            {
                Bots.Remove(_botForNewColonyBase);

                _botForNewColonyBase.SetTarget(_flag.transform);
            }
        }
    }


    private void TryBuildNewColonyBase()
    {
        int newBasePrice = 5;

        if (GoldCount >= newBasePrice)
        {
            if (_isFlagSpawned == true && _botForNewColonyBase == null)
            {
                TrySendFreeBotToFlag();
            }

            if (_botForNewColonyBase != null && _botForNewColonyBase.IsFlagReached == true)
            {
                int distanceFromBotToNewBase = 3;

                GoldCount -= newBasePrice;
                GoldChanged?.Invoke(GoldCount);

                Vector3 colonyBaseSpawnPosition = _botForNewColonyBase.transform.position;
                colonyBaseSpawnPosition.y = _colonyBasePrefab.transform.position.y;
                colonyBaseSpawnPosition.z += distanceFromBotToNewBase;

                CreateColonyBase(colonyBaseSpawnPosition, _botForNewColonyBase);

                _botForNewColonyBase = null;
                _isFlagSpawned = false;
            }
        }
    }

    private void CreateColonyBase(Vector3 colonyBaseSpawnPosition, Bot givenBot)
    {
        if (givenBot != null)
        {
            ColonyBase newColonyBase = Instantiate(_colonyBasePrefab, colonyBaseSpawnPosition, Quaternion.identity);

            newColonyBase.TakeBot(givenBot);
            newColonyBase.ResetGold();
        }
    }
}
