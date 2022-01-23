using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private GameObject Player;

    private GameObject Camera;

    private GameObject Target;
    // Start is called before the first frame update
    void Start()
    {
        Player = Resources.Load("Prefabs/Player") as GameObject;
        Camera = Resources.Load("Prefabs/Camera") as GameObject;
        Target = Resources.Load("Prefabs/Target") as GameObject;

        var p = Instantiate(Player);
        p.transform.position = new Vector3(Random.Range(-3.8f, 3.8f), 1, Random.Range(-3.8f, 3.8f));
        var c = Instantiate(Camera, transform);
        var t = Instantiate(Target);
        t.transform.position = new Vector3(Random.Range(-4, +4), 1, Random.Range(-4, +4));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
