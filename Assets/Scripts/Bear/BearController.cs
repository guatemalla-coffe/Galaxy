using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pathfinding;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BearController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;

    public BearState currentState { get; private set; }
    public Command currentCommand = null;

    public BearAnimations bearAnimations;
    
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public Material outlineMaterial; // Установите материал с шейдером OutlineShader

    public string nameBear;
    public string work;

    private AIPath _path;

    public float currentHunger;
    public string textPlayerText;

    public void Feed(float amount)
    {
        currentHunger += amount;
    }


    private void Awake()
    {
        bearAnimations = GetComponent<BearAnimations>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        _path = GetComponent<AIPath>();
    }

    public void SpawnHungerEmote()
    {
        print("SpawnHungerEmote");
        if (currentHunger <= 15f)
        {
            print("Хочу есть!");

            UIManager.Instance.ShopTipPlayer("Хочу есть!", transform);
        }
    }

    private void Start()
    {
        SetState(new IdleState(this));
        
    }

    public void SetState(BearState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    
    public BearState GetState()
    {
        return currentState;
    }   

    public void Select()
    {
        Debug.Log($"{gameObject.name} selected.");
        TaskManager.Instance.CompleteTask(0);
        Furnace[] furs = GameObject.FindObjectsByType<Furnace>(FindObjectsSortMode.None);
        foreach (var fur in furs)
        {
            fur.Deselect();
        }  
        UIManager.Instance.contextMenu.SetActive(false);
        if (outlineMaterial != null)
        {
            spriteRenderer.material = outlineMaterial;
        }
    }

    public void Deselect()
    {
        Debug.Log($"{gameObject.name} deselected.");
        spriteRenderer.material = originalMaterial;
    }


    private float currentFeedTimer = 20f;
    public float currentTipTimer = 5f;
    private void Update()
    {
        currentState?.Update();
        if (BearManager.Instance.GetSelectedBear() == this)
        {
            if (currentCommand != null) UIManager.Instance.commandText.text = currentCommand.ToString();
            else UIManager.Instance.commandText.text = "Null";

            if (currentState != null) UIManager.Instance.stateText.text = currentState.ToString();
            else UIManager.Instance.stateText.text = "Null";
        }

        currentHunger -= Time.deltaTime * 0.1f;
        currentHunger = Mathf.Clamp(currentHunger, 0f, 100f);
        if (currentHunger < 15f)
        {
            _path.maxSpeed = 1f;
        }
        else
        {
            _path.maxSpeed = 2f;
        }

        currentFeedTimer -= Time.deltaTime;
        if (currentFeedTimer <= 0f)
        {
            currentFeedTimer = Random.Range(22f, 36f);
            SpawnHungerEmote();
        }

        currentTipTimer -= Time.deltaTime;
        if (currentTipTimer <= 0f)
        {
            currentTipTimer = Random.Range(100f, 300f);
            UIManager.Instance.ShopTipPlayer(textPlayerText, transform, Color.gray, 3f);
        }
    }
    
}