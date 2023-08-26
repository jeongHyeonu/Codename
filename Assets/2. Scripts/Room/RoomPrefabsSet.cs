﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefabsSet : Singleton<RoomPrefabsSet> 
{
    [SerializeField]
    public Dictionary<string, List<GameObject>> roomPrefabs = new Dictionary<string, List<GameObject>>();
    public List<string> roomPrefabsName;
    public List<GameObject>[] roomPrefabsList;
    public List<GameObject> normalRoomPrefabsList;
    public List<GameObject> bossRoomPrefabsList;

    // Start is called before the first frame update
    void Awake()
    {
        roomPrefabsList = new List<GameObject>[2]
        {
            normalRoomPrefabsList, bossRoomPrefabsList
        };
        for (int i = 0; i < roomPrefabsName.Count; i++)
        {
            roomPrefabs.Add(roomPrefabsName[i], roomPrefabsList[i]);
        }
    }
    
}