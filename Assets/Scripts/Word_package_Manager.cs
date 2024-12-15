using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // 引入UI命名空间

public class WordPackageManager : MonoBehaviour
{
    public static WordPackageManager Instance { get; private set; }

    public GameObject buttonPrefab;  // 引用带有TextMeshPro的Button预制件
    public RectTransform panel;      // 引用面板的RectTransform
    public float maxFontSizeVariance = 3f; // 字体大小浮动的最大值
    public float minFontSizeVariance = -3f; // 字体大小浮动的最小值

    private List<string> wordList;
    private List<RectTransform> buttonObjects = new List<RectTransform>(); // 用于存储所有实例化的Button

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            wordList = new List<string>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 添加字符串到容器中并实例化Button
    public void AddWord(string word)
    {
        wordList.Add(word);

        // 实例化一个带有TextMeshPro的Button对象
        GameObject newButtonObject = Instantiate(buttonPrefab, panel);
        Button newButton = newButtonObject.GetComponent<Button>();
        TextMeshProUGUI newText = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();  // 获取Button下的TextMeshProUGUI组件

        // 设置文本内容
        newText.text = word;

        // 随机字体大小变化
        float fontSizeVariance = Random.Range(minFontSizeVariance, maxFontSizeVariance);
        newText.fontSize += fontSizeVariance;

        // 随机旋转
        float randomRotation = Random.Range(-3f, 3f);
        newButtonObject.transform.rotation = Quaternion.Euler(0, 0, randomRotation);

        // 确保Button的位置不重叠，并且完全在Panel内
        Vector3 newPos = GetRandomPosition(newText);
        newButtonObject.transform.localPosition = newPos;

        // 将新对象的RectTransform保存到列表
        buttonObjects.Add(newButtonObject.GetComponent<RectTransform>());
    }

    // 从容器中移除字符串
    public void RemoveWord(string word)
    {
        wordList.Remove(word);

        // 找到对应的Button并销毁
        foreach (var buttonObj in buttonObjects)
        {
            if (buttonObj.GetComponentInChildren<TextMeshProUGUI>().text == word)
            {
                Destroy(buttonObj.gameObject);
                buttonObjects.Remove(buttonObj);
                break;
            }
        }
    }

    // 获取一个有效的随机位置，确保不重叠并完全在Panel内
    private Vector3 GetRandomPosition(TextMeshProUGUI newText)
    {
        Vector3 randomPos = Vector3.zero; // 设置一个默认值
        bool positionValid = false;

        // 获取面板的尺寸
        float panelWidth = panel.rect.width;
        float panelHeight = panel.rect.height;

        // 获取TextMeshPro的尺寸
        float textWidth = newText.preferredWidth;
        float textHeight = newText.preferredHeight;

        // 最大尝试次数
        int maxAttempts = 50;
        int attemptCount = 0;

        // 尝试生成一个有效位置
        while (!positionValid && attemptCount < maxAttempts)
        {
            // 生成随机位置（确保位置不超出Panel的边界）
            randomPos = new Vector3(Random.Range(-(panelWidth / 2) + (textWidth), (panelWidth / 2) - (textWidth)),
                                    Random.Range(-(panelHeight / 2) + (textHeight), (panelHeight / 2) - (textHeight)), 0);

            positionValid = true;

            // 检查新位置是否与现有的TextMeshPro重叠
            foreach (var existingButton in buttonObjects)
            {
                if (Vector3.Distance(randomPos, existingButton.localPosition) < 100) // 可以调节重叠的最小距离（此处为50）
                {
                    positionValid = false;
                    break;
                }
            }

            attemptCount++;
        }

        if (attemptCount >= maxAttempts)
        {
            Debug.Log("Exceeded maximum attempts to find a valid position.");
        }

        return randomPos; // 返回随机位置
    }

    // 按钮点击事件的响应



    // Start is called before the first frame update
    void Start()
    {
        // 示例：初始化时添加一些word
        AddWord("?");
    }

    // Update is called once per frame
    void Update()
    {
        // 可以在这里添加动态添加词语的逻辑
    }
}
