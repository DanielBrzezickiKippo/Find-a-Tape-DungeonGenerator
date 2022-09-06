using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public int Width;
    public int Height;

    public int X;
    public int Y;

    private bool updatedDoors = false;

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;

    public List<Door> doors = new List<Door>();

    private GameObject player;
    public float distance = 0f;
    private bool switched=false;

    private GameObject roomController;

    public GameObject enemySpawner;
    public bool spawn = false;
    public bool startedWaves =false;


   /* public void SpawnEnemy()
    {
        if (startedWaves == false)
        {
            enemySpawner.GetComponent<EnemySpawnManager>().StartWaves();
            //StartCoroutine(enemySpawner.GetComponent<EnemySpawnManager>().StartWaves());
            startedWaves = true;
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        roomController = GameObject.FindGameObjectWithTag("RoomController");

        player = GameObject.FindGameObjectWithTag("Player");

        if (RoomController.instance == null)
        {
            Debug.Log("You pressed play in wrong scene!");
            return;
        }

        Door[] ds = GetComponentsInChildren<Door>();
        foreach(Door d in ds)
        {
            doors.Add(d);
            switch (d.doorType){
                case Door.DoorType.right:
                    rightDoor = d;
                    break;
                case Door.DoorType.left:
                    leftDoor = d;
                    break;
                case Door.DoorType.top:
                    topDoor = d;
                    break;
                case Door.DoorType.bottom:
                    bottomDoor = d;
                    break;
            }
        }

        RoomController.instance.RegisterRoom(this);

        /*if (Vector2.Distance(new Vector2(0, 0), new Vector2(X, Y)) > 1)
        {
            gameObject.SetActive(false);
        }*/
    }

    int currRoomX;
    int currRoomY;

    void Update()
    {
        if (name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
    }

    public void RemoveUnconnectedDoors()
    {
        foreach(Door door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.right:
                    if (GetRight() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.left:
                    if (GetLeft() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.top:
                    if (GetTop() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.bottom:
                    if (GetBottom() == null)
                        door.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public Room GetRight()
    {
        if (RoomController.instance.DoesRoomExist(X + 1, Y))
        {
            return RoomController.instance.FindRoom(X + 1, Y);
        }
        return null;
    }

    public Room GetLeft()
    {
        if (RoomController.instance.DoesRoomExist(X - 1, Y))
        {
            return RoomController.instance.FindRoom(X - 1, Y);
        }
        return null;

    }

    public Room GetTop()
    {
        if (RoomController.instance.DoesRoomExist(X, Y + 1))
        {
            return RoomController.instance.FindRoom(X, Y + 1);
        }
        return null;
    }

    public Room GetBottom()
    {
        if (RoomController.instance.DoesRoomExist(X, Y - 1))
        {
            return RoomController.instance.FindRoom(X, Y - 1);
        }
        return null;
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));

    }

    public Vector3 GetRoomCentre()
    {
        return new Vector3(X * Width, Y * Height);
    }

    private bool toChange = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (toChange == false)
        {
            if (other.tag == "cam_switch")
            {
                RoomController.instance.OnPlayerEnterRoom(this);
                toChange = true;
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "cam_switch")
        {
            toChange = false;
        }
    }

}
