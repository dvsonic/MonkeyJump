using UnityEngine;
using System.Collections;

public class FireworksFactory : MonoBehaviour {

    public AudioSource fire;
    public GameObject[] fireworksList;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Create()
    {
        for (int i = 0; i < fireworksList.Length; i++)
        {
            StartCoroutine(Emittor(fireworksList[i]));
        }
        if (fire)
            fire.Play();
    }

    IEnumerator Emittor(GameObject obj)
    {
        yield return new WaitForSeconds(Random.Range(0.1f,2.0f));
        GameObject fireworks = Instantiate(obj);
        fireworks.transform.position = new Vector3(Random.Range(-3, 3), Random.Range(0, 5))+transform.position;
    }
}
