﻿using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left, right, top, bottom
    }
    public GameObject nextRoom;
    public Door SideDoor;
    public DoorType doorType;
    public Transform doorPos;
    public bool isUpdate = false;



    public void setNextRoom(GameObject _nextRoom)
    {
        nextRoom = _nextRoom;
    }

    public void setSideDoor(Door _sideDoor)
    {
        SideDoor = _sideDoor;
        gameObject.GetComponentInChildren<Teleporter>().Destination = _sideDoor.GetComponentInChildren<Teleporter>();

        Vector2 exitOffset = Vector2.zero;
        if (doorType == DoorType.right) exitOffset = new Vector2(-3, 0);
        else if (doorType == DoorType.left) exitOffset = new Vector2(3, 0);
        else if (doorType == DoorType.top) exitOffset = new Vector2(0, -3);
        else if (doorType == DoorType.bottom) exitOffset = new Vector2(0, 3);

        gameObject.GetComponentInChildren<Teleporter>().ExitOffset = exitOffset;
        gameObject.GetComponentInChildren<Teleporter>().CurrentRoom = gameObject.GetComponentInParent<Room>();
        gameObject.GetComponentInChildren<Teleporter>().TargetRoom = nextRoom.GetComponent<Room>();

        if (nextRoom.GetComponent<DungeonRoom>().roomName == "Boss")
        {
            Teleporter bossDoor = gameObject.GetComponentInChildren<Teleporter>();
            bossDoor.Destination = RoomPrefabsSet.Instance.realBossRoom.GetComponentInChildren<Teleporter>();
            bossDoor.TargetRoom = RoomPrefabsSet.Instance.realBossRoom.GetComponent<Room>();
            RoomPrefabsSet.Instance.realBossRoom.SetActive(false);
        }
    }

    public void ChangeCurrentRoom()
    {
        RoomController.Instance.currRoom = nextRoom.GetComponent<DungeonRoom>();

        gameObject.GetComponentInParent<DungeonRoom>().childRooms.gameObject.SetActive(false);
        nextRoom.GetComponentInParent<DungeonRoom>().childRooms.gameObject.SetActive(true);
    }
}
