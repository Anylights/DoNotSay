using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public enum PlayerState
{
    Move,
    ChooseWord
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerState currentState = PlayerState.Move;

    public Animator Panel_animator;//面板动画机
    public CinemachineVirtualCamera moveCamera; // 移动状态下的虚拟摄像机
    public CinemachineVirtualCamera chooseWordCamera; // 选择单词状态下的虚拟摄像机

    public string wordInHand; // 手中的单词
    public TextMeshProUGUI wordInHand_UI; // 手中单词的UI


    private CinemachineBrain cinemachineBrain; // CinemachineBrain 组件
    private PlayerMovement playerMovement;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        chooseWordCamera.Priority = 0; // 初始时选择单词摄像机优先级较低
        moveCamera.Priority = 10; // 初始时移动摄像机优先级较高
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        EventCenter.Instance.Subscribe<string>("PickUpWord", PickUpWord);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentState == PlayerState.Move)
            {
                EnterChooseWordState();
            }
            else if (currentState == PlayerState.ChooseWord)
            {
                EnterMoveState();
            }
        }
    }

    void EnterChooseWordState()
    {
        currentState = PlayerState.ChooseWord;
        Panel_animator.SetBool("isChoosing", true);
        chooseWordCamera.Priority = 10; // 提高选择单词摄像机的优先级
        moveCamera.Priority = 0; // 降低移动摄像机的优先级
        playerMovement.enabled = false;
    }

    void EnterMoveState()
    {
        currentState = PlayerState.Move;
        Panel_animator.SetBool("isChoosing", false);
        chooseWordCamera.Priority = 0; // 降低选择单词摄像机的优先级
        moveCamera.Priority = 10; // 提高移动摄像机的优先级
        playerMovement.enabled = true;
    }

    void PickUpWord(string word)
    {
        wordInHand = word;
        wordInHand_UI.text = word;
    }
}