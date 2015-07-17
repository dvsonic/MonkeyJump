using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{

    public Text tfScore;
    public Text tfGuide;

    // Use this for initialization
    void Awake()
    {
        if (tfGuide)
            tfGuide.text = GameData.getLanguage("guide");
    }
    void Start()
    {
        GameData.score = 0;
        GameData.hp = GameData.MAX_HP;
    }

    public GameObject ninjia;
    public GameObject factory;
    void Reset()
    {
        Debug.Log("Reset");
        Start();
        if (ninjia)
            ninjia.SendMessage("Reset");
        if (factory)
            factory.SendMessage("Reset");
        Camera.main.SendMessage("Reset");
    }

    public void ShowGuide()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (tfScore)
            tfScore.text = GameData.score.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        GameData.score = 0;
        GameScene.GotoScene(1);
    }
}
