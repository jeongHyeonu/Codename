﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;

[System.Serializable]
public class DungeonRoom : MonoBehaviour
{
    public int Width;
    public int Height;

    public string roomName;
    public string roomType;
    public string roomId;

    public Vector3Int center_Position;
    public Vector3Int parent_Position;
    public Vector3 mergeCenter_Position;

    public int distance;

    public bool isUpdatedWalls = false;
    public bool isVisitedRoom = false;
    public bool isClearRoom = false;
    public GameObject prefabsDoor;
    public GameObject prefabsWall;

    public List<GameObject> enemyList;

    public DungeonRoom(int x, int y, int z)
    {
        center_Position.x = x;
        center_Position.y = y;
        center_Position.z = z;
    }

    public SubRoom childRooms;
    public GameObject prefabCamera;
    public GameObject prefabConfiner;
    public GameObject cvCamera;
    public GameObject confiner;

    public bool updateRoomStatus = false;


    // Room을 생성 시 초기에 호출(Start)
    public void SetUpdateWalls(bool setup)
    {
        isUpdatedWalls = setup;
    }

    void Start()
    {
        if (RoomController.Instance == null)
        {
            Debug.Log("You pressed play in the wrong scene!");
            return;
        }

        childRooms = GetComponentInChildren<SubRoom>();

        if (childRooms != null)
        {
            childRooms.center_Position       = center_Position;
            childRooms.roomType              = roomType;
            childRooms.Width                 = Width;
            childRooms.Height                = Height;
            childRooms.roomName              = roomName;
            childRooms.parent_Position       = parent_Position;
            childRooms.mergeCenter_Position  = mergeCenter_Position;

        }

        isUpdatedWalls = false;
    }

    public void RemoveUnconnectedWalls()
    {
        if (childRooms != null)
        {
            childRooms.RemoveUnconnectedWalls();
        }
    }

    void Update()
    {
        if (!isUpdatedWalls)
        {
            RemoveUnconnectedWalls();

            InitRoomCamera();

            isUpdatedWalls = true;
        }
    }

    private void InitRoomCamera()
    {
        Transform[] child = gameObject.GetComponentsInChildren<Transform>();

        if (child.Length != 1)
        {
            if(roomType == "Double" || roomType == "Quad")
            {
                BoxCollider2D boxColider = gameObject.GetComponent<BoxCollider2D>();
                Vector3 coliderCenter = mergeCenter_Position - center_Position;
                boxColider.offset = new Vector2(coliderCenter.x * 16, coliderCenter.z * 16 + 1);
                boxColider.size = new Vector2(Mathf.Abs(coliderCenter.x) * 32 + 18, Mathf.Abs(coliderCenter.z) * 32 + 18);
            }
            
            cvCamera = Instantiate(prefabCamera, childRooms.transform);
            confiner = Instantiate(prefabConfiner, childRooms.transform);

            Room room = gameObject.GetComponent<Room>();
            room.VirtualCamera = cvCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            room.CinemachineCameraConfiner = cvCamera.GetComponent<Cinemachine.CinemachineConfiner>();
            room.Confiner = confiner.GetComponent<BoxCollider>();
            cvCamera.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingVolume = room.Confiner;
            cvCamera.GetComponent<CinemachineCameraController>().TargetCharacter = LevelManager.Instance.Players[0];
            cvCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = LevelManager.Instance.Players[0].transform;
        }
    }

    public Vector3 GetRoomCenter()
    {
        return new Vector3(center_Position.x, 0, center_Position.z);
    }

    private void CheckRoomClear()
    {
        Transform[] child = gameObject.GetComponentsInChildren<Transform>();

        if(enemyList.Count == 0 && child.Length != 1 && (roomType == "Double" || roomType == "Quad"))
        {
            isClearRoom = true;
            DoorControl();
        }
    }

    public void DoorControl()
    {
        Door[] doors = gameObject.GetComponentsInChildren<Door>();
        
        foreach(Door door in doors)
        {
            BoxCollider2D doorColider = door.gameObject.GetComponentInChildren<BoxCollider2D>();
            doorColider.enabled = isClearRoom;
            door.gameObject.GetComponentInChildren<DoorEvent>().DoorLightControl(isClearRoom);
        }
    }
    
    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
        CheckRoomClear();    
    }
}
