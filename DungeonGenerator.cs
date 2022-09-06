using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    public DungeonGenerationData dungeonGenerationData;
    private List<Vector2Int> dungeonRooms;

    //public GameObject[] room_templates;
    private int chapterNum, numberOfCrawlers,iterationMin,iterationMax;

    private void Start()
    {
        DungeonSize();

    }


    void DungeonSize()
    {
        chapterNum = FindObjectOfType<chapterScript>().cnum;
        switch (chapterNum)
        {
            case 1:
                numberOfCrawlers = 2;
                iterationMin = 8;
                iterationMax = 12;
                break;
            case 2:
                numberOfCrawlers = 2;
                iterationMin = 12;
                iterationMax = 16;
                break;
            case 3:
                numberOfCrawlers = 2;
                iterationMin = 16;
                iterationMax = 22;
                break;
            case 4:
                numberOfCrawlers = 2;
                iterationMin = 22;
                iterationMax = 25;
                break;
            case 5:
                numberOfCrawlers = 2;
                iterationMin = 25;
                iterationMax = 30;
                break;
            case 6:
                numberOfCrawlers = 2;
                iterationMin = 28;
                iterationMax = 32;
                break;
            case 7:
                numberOfCrawlers = 2;
                iterationMin = 33;
                iterationMax = 40;
                break;
            default:
                numberOfCrawlers = 2;
                iterationMin = 16;
                iterationMax = 22;
                break;
        }

        DungeonGenerationData data = new DungeonGenerationData();
        data.numberOfCrawlers = numberOfCrawlers;
        data.iterationMin = iterationMin;
        data.iterationMax = iterationMax;
        //dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(data);
        dungeonRooms.Clear();
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(data);
        SpawnRooms(dungeonRooms);

    }

    private void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        RoomController.instance.LoadRoom("Start", 0, 0);
        foreach(Vector2Int roomLocation in rooms)
        {
            RoomController.instance.LoadRoom("Empty", roomLocation.x, roomLocation.y);
        }
    }
}
