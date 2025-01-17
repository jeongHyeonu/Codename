﻿using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : Singleton<RoomController>
{
    public string globalRoomTitle = "Basement";

    public RoomInfo currentLoadRoomData;
    public DungeonRoom currRoom;

    public List<DungeonRoom> loadedRooms = new List<DungeonRoom>();

    public Material DefaultBackground;
    public Material VisitedBack;
    public Material currMaterial;
    public Material bossBackground;

    public bool isLoadingRoom = false;
    public bool isCreateRoom = false;

    public void Update()
    {
        if (!isCreateRoom)
        {
            CreatedRoom();
            isCreateRoom = true;
        }
    }

    public void CreatedRoom()
    {
        isLoadingRoom = false;

        for(int i=0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        
        loadedRooms.Clear();

        LevelManager.Instance.Players[0].transform.position = new Vector3(1, 1, 0);
        DungeonCrawlerController.Instance.CreatedRoom();
    }

    public void LoadRoom(RoomInfo settingRoom)
    {
        if (DoesRoomExist(settingRoom.center_Position.x, settingRoom.center_Position.y, settingRoom.center_Position.z))
        {
            return;
        }

        string roomPreName = settingRoom.roomName;

        if(settingRoom.roomType == "Single" && roomPreName != "Boss")
        {
            if (settingRoom.center_Position == Vector3Int.zero)
                roomPreName = "Start";
            else
                roomPreName = "Box";
        }

        int ranIdx = Random.Range(0, RoomPrefabsSet.Instance.roomPrefabs[roomPreName].Count);
        GameObject room = Instantiate(RoomPrefabsSet.Instance.roomPrefabs[roomPreName][ranIdx]);

        room.transform.position = new Vector3(
                    (settingRoom.center_Position.x * room.transform.GetComponent<DungeonRoom>().Width*3.2f),
                     settingRoom.center_Position.z * room.transform.GetComponent<DungeonRoom>().Height*3.2f,
                    (settingRoom.center_Position.z)
        );

        room.transform.localScale = new Vector3(
                    (room.transform.GetComponent<DungeonRoom>().Width / 5),
                     1,
                    (room.transform.GetComponent<DungeonRoom>().Height / 5)
        );
        room.transform.GetComponent<DungeonRoom>().center_Position = settingRoom.center_Position;
        room.name = globalRoomTitle + "-" + settingRoom.roomName + " " + settingRoom.center_Position.x + ", " + settingRoom.center_Position.z;

        room.transform.GetComponent<DungeonRoom>().roomName                = settingRoom.roomName;
        room.transform.GetComponent<DungeonRoom>().roomType                = settingRoom.roomType;
        room.transform.GetComponent<DungeonRoom>().roomId                  = settingRoom.roomID;
        room.transform.GetComponent<DungeonRoom>().parent_Position         = settingRoom.parent_Position;
        room.transform.GetComponent<DungeonRoom>().mergeCenter_Position    = settingRoom.mergeCenter_Position;
        room.transform.GetComponent<DungeonRoom>().distance                = settingRoom.distance;

        room.transform.parent = transform;

        loadedRooms.Add(room.GetComponent<DungeonRoom>());
    }

    // 빈 데이터 혹은 삭제된 방이 있을 경우를 위한 예외처리
    public bool DoesRoomExist(int x, int y, int z)
    {
        return loadedRooms.Find(item => item.center_Position.x == x && item.center_Position.y == y && item.center_Position.z == z) != null;
    }

    //    
    public DungeonRoom FindRoom(int x, int y, int z)
    {
        // List.Find : item 변수 조건에 맞는 Room을 찾아 반환
        return loadedRooms.Find(item => item.center_Position.x == x && item.center_Position.y == y && item.center_Position.z == z);
    }

    // 해당 Room에서 Player가 있는 방을 반환
    public void OnPlayerEnterRoom(DungeonRoom room) {
        if (!room.isUpdatedWalls) return;

        currRoom = room;

        for (int i = 0; i < loadedRooms.Count; i++)
        {
            if (room.parent_Position == loadedRooms[i].parent_Position)
            {
                loadedRooms[i].childRooms.minimapUpdate();
                loadedRooms[i].childRooms.gameObject.SetActive(true);
            }
            else
            {
                loadedRooms[i].childRooms.gameObject.SetActive(false);
            }
        }
    }
}
