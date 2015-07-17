using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockFactory : MonoBehaviour {

	// Use this for initialization
    public GameObject[] blockList;
    public GameObject[] bonusList;
    public float gap;
    public Transform lastBlock;
    public List<GameObject> obstacleList;

    private List<GameObject> createdObject;

    private GameObject _initBlock;
    void Awake()
    {
        _initBlock = lastBlock.gameObject;
    }
    private static BlockFactory _instant;
	void Start () {
        createdObject = new List<GameObject>();
        _instant = this;
	}

    public static BlockFactory getInstance()
    {
        return _instant;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
    }


    public void CreatBlock()
    {
        GameData.blockNum ++;

        Object prefab;
        prefab = blockList[Random.Range(0, blockList.Length)];

        GameObject obj = Instantiate(prefab) as GameObject;
        createdObject.Add(obj);

        obj.transform.position = new Vector3(lastBlock.transform.position.x, lastBlock.transform.position.y+gap);
        lastBlock = obj.transform;

        if (GameData.blockNum % 10 != 0)
        {
            if (GameData.blockNum % 20 == 0)//增加难度,删除工厂中速度慢的单位
            {
                if (obstacleList.Count > 4)
                {
                    obstacleList.RemoveAt(obstacleList.Count - 1);
                }
            }
            int index = Random.Range(0, obj.transform.childCount);
            Transform child = obj.transform.GetChild(index);
            Object prefab2 = obstacleList[Random.Range(0, obstacleList.Count)];
            createdObject.Add(Instantiate(prefab2, child.position, child.rotation) as GameObject);
        }

        if (Random.Range(0, 1.0f) < 0.1f)
        {
            GameObject bonusPrefab = bonusList[Random.Range(0, bonusList.Length)];
            GameObject bonus = Instantiate(bonusPrefab) as GameObject;
            bonus.transform.position = new Vector3(obj.transform.position.x + Random.Range(-3, 3), obj.transform.position.y + Random.Range(2, 4));
            createdObject.Add(bonus);
        }

    }

    public void Reset()
    {
        for (int i = 0; i < createdObject.Count; i++)
        {
            Destroy(createdObject[i]);
        }
        createdObject.Clear();
        GameData.blockNum = 0;
        lastBlock = _initBlock.transform;
    }

}
