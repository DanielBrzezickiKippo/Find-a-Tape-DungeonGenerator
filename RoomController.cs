using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class Story
{
    public GameObject[] NPC;
}


public class RoomInfo
{
    public string name;

    public int X;
    public int Y;
}


public class RoomController : MonoBehaviour
{
    public static RoomController instance;
    private string currentWorldName;
    RoomInfo currentLoadRoomData;
    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();
    public List<Room> loadedRooms = new List<Room>();
    bool isLoadingRoom = false;

    bool spawnedBossRoom = false;
    bool updatedRooms = false;

    public Room currRoom;

    private minimap Minimap;
    private GameObject mp;

    private GameObject particles;
    //LoadingScreen
    private GameObject loading;
    private int load_count=0;
    private UnityEngine.UI.Text loadingText;
    private GameObject UI_canvas;
    private player p;
    private guns g;
    private Timer t;


    void Awake()
    {
        particles = GameObject.FindGameObjectWithTag("mesh_particle");


        loading = GameObject.FindGameObjectWithTag("loading");
        loadingText = GameObject.FindGameObjectWithTag("loadingText").GetComponent<UnityEngine.UI.Text>();

        currentWorldName = PlayerPrefs.GetString("level", "room");

        Minimap = GameObject.FindGameObjectWithTag("minimap").GetComponent<minimap>();
        mp = GameObject.FindGameObjectWithTag("minimap_canvas");

        UI_canvas = GameObject.FindGameObjectWithTag("canvas");
        UI_canvas.GetComponent<Canvas>().enabled = false;
        //Time.timeScale = 0;

        p = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        g = GameObject.FindGameObjectWithTag("gun_manager").GetComponent<guns>();
        p.enabled = false;
        g.enabled = false;

        t = GameObject.FindGameObjectWithTag("timer").GetComponent<Timer>();

        player = GameObject.FindGameObjectWithTag("Player");
        instance = this;

    }

    void Start()
    {
        //sss();
        //Debug.Log(Mathf.Pow(4,2));
    }

    void Update()
    {
        UpdateRoomQueue();

    }

    private GameObject player;

    public int playerX = 0;
    public int playerY = 0;

    public void TurnOnRooms()
    {
        for (int i = playerX - 10; i <= playerX + 10; i++)
        {
            for (int j = playerY - 10; j <= playerY + 10; j++)
            {
                if (DoesRoomExist(i, j))
                {
                    if ((i < playerX+2 && i > playerX - 2 && j==playerY) || (i==playerX && j < playerY + 2 && j > playerY - 2))
                    {
                        loadedRooms.Find(item => item.X == i && item.Y == j).gameObject.SetActive(true);
                    }
                    else
                    {
                        loadedRooms.Find(item => item.X == i && item.Y == j).gameObject.SetActive(false);
                    }

                }
            }
        }
    }

    float distance = 0;
    float old_distance = 0;

    int X_distance = 0;
    int Y_distance = 0;

    void UpdateRoomQueue()
    {
        if (isLoadingRoom)
        {
            return;
        }

        if (loadRoomQueue.Count == 0)
        {
            if (!spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());

            }
            else if (spawnedBossRoom && !updatedRooms)
            {
                foreach(Room room in loadedRooms)
                {
                    
                    room.RemoveUnconnectedDoors();
                    distance = Mathf.Sqrt(Mathf.Pow(room.X, 2) + Mathf.Pow(room.Y, 2));
                    if (old_distance < distance)
                    {
                        old_distance = distance;
                        X_distance = room.X;
                        Y_distance = room.Y;
                    }

                }
                updatedRooms = true;
                spawnNPC();

                //TurnOnRooms();
            }
            return;
        }

        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;


        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    private int[] gen;
    private int r_num;
    public int currentStory;
    public Story[] story;

    public int currentToSpawn = 0;

    private void spawnNPC()
    {
        currentStory = FindObjectOfType<chapterScript>().cnum;
        generateNumsForNPC();

        /*for (int i = 0; i < story[currentStory - 1].NPC.Length; i++)
        {
            Room r = loadedRooms.ElementAt(gen[i]);
            r.GetComponent<SpawnNPC>().enabled = true;
        }*/
    }

    private bool canGenerate(int[] gen)
    {
        for (int i = 0; i < gen.Length; i++)
        {
            if (gen[i] == r_num)
            {
                return false;
            }
        }
        return true;
    }

    private void generateNumsForNPC()
    {
        gen = new int[story[currentStory-1].NPC.Length];

        for (int i = 0; i < gen.Length; i++)
        {
            if (i == 0)
            {
                gen[i] = Random.Range(1, (loadedRooms.Count - 1));
            }
            else
            {
                while (!canGenerate(gen))
                {
                    r_num = Random.Range(1, (loadedRooms.Count - 1));
                }
                gen[i] = r_num;
            }
        }
        for (int i = 0; i < story[currentStory - 1].NPC.Length; i++)
        {
            Room r = loadedRooms.ElementAt(gen[i]);
            r.GetComponent<SpawnNPC>().enabled = true;
        }
        //TurnOnRooms();

        //Debug.LogWarning("SPAWNED NPC");
    }


    IEnumerator WaitToStart()
    {
        load_count = 99;
        loadingText.text = load_count + "%";
        yield return new WaitForSeconds(2f);
        load_count = 100;
        loadingText.text = load_count + "%";
        loading.GetComponent<Animator>().SetBool("isOpened", false);
        //loading.SetActive(false);//anim
        mp.GetComponent<Canvas>().enabled = true;
        particles.SetActive(true);
        UI_canvas.GetComponent<Canvas>().enabled = true;
        p.enabled = true;
        g.enabled = true;
        t.currentTime = 0f;
        FindObjectOfType<chapterScript>().ChapterAnim();
        FindObjectOfType<minimap>().SetRoomMax();
        TurnOnRooms();
        //Time.timeScale = 1;
    }


    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if (loadRoomQueue.Count == 0)
        {
            UpdateRoomQueue();
            Room bossRoom = FindRoom(X_distance, Y_distance);
            Room tempRoom = new Room(bossRoom.X, bossRoom.Y);
            Destroy(bossRoom.gameObject);
            var roomToRemove = loadedRooms.Single(r => r.X == tempRoom.X && r.Y == tempRoom.Y);
            loadedRooms.Remove(roomToRemove);
            LoadRoom("End", tempRoom.X, tempRoom.Y);

            StartCoroutine(WaitToStart());
        }
        //yield return new WaitForSeconds(0.5f);
    }

    public void FarthestRoom()
    {
        
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y))
        {
            return;
        }
        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.X = x;
        newRoomData.Y = y;

        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        string roomName = currentWorldName + info.name;

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        while (loadRoom.isDone == false)
        {
            yield return null;
        }
    }

    public void RegisterRoom(Room room)
    {
        if (!DoesRoomExist(currentLoadRoomData.X, currentLoadRoomData.Y))
        {
            room.transform.position = new Vector3(
                currentLoadRoomData.X * room.Width,
                currentLoadRoomData.Y * room.Height,
                0);


            room.X = currentLoadRoomData.X;
            room.Y = currentLoadRoomData.Y;
            room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.X + ", " + room.Y;
            room.transform.parent = transform;

            isLoadingRoom = false;

            if (loadedRooms.Count == 0)
            {
                CameraController.instance.currRoom = room;
            }

            loadedRooms.Add(room);

            room.RemoveUnconnectedDoors();


            load_count++;
            loadingText.text = load_count + "%";
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y) != null;
    }

    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y);
    }

    public void OnPlayerEnterRoom(Room room)
    {
        room.RemoveUnconnectedDoors();

        if (room.spawn == true)
        {
            //room.SpawnEnemy();
        }
        CameraController.instance.currRoom = room;
        currRoom = room;
        playerX = currRoom.X;
        playerY = currRoom.Y;



        Minimap.CreateRoomIcon(currRoom.X, currRoom.Y);
        Minimap.CurrentRoom(currRoom.X, currRoom.Y);

        TurnOnRooms();

        //Debug.Log("tyle razy sie wykonuje");
    }

    public void startRoomEnabled()
    {
        Room r = loadedRooms.ElementAt(0);
        r.gameObject.SetActive(true);
    }

}
