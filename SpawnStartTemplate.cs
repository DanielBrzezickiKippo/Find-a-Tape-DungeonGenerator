using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStartTemplate : MonoBehaviour
{
    public GameObject[] templates;

    int num;

    private void Awake()
    {
        num = FindObjectOfType<SummaryUI>().num - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        //num = FindObjectOfType<SummaryUI>().num-1;
        GameObject obj = (GameObject)Instantiate(templates[num], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
        obj.transform.parent = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
