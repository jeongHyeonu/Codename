using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadManager : MonoBehaviour
{

    private static RoadManager instance = null; // �̱��� ��ü

    public static RoadManager Instance // �̱��� �޾ƿ���
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
    }

    public void GenerateRoad()
    {
        Tilemap tileMap = MapGenerateManager.Instance.tileMap;
        Tile roomTile = MapGenerateManager.Instance.roomTile;
        Vector2Int mapSize = MapGenerateManager.Instance.mapSize;

        // ���� �� ���� = 2�� (Ʈ������/2)��ŭ ������ ��
        int maxRoomCnt_Y = (int)Mathf.Pow(2, MapGenerateManager.Instance.maximumDepth / 2);
        // ���� �� ���� = �ִ� �� �� / ���� �� ����
        int maxRoomCnt_X = RoomManager.Instance.roomCnt / maxRoomCnt_Y;

        // �� �ε��� ��ȣ �ޱ��, �ٽ� �ε������ �� ����Ʈ ����
        for (int idx = 0; idx < RoomManager.Instance.roomCnt; idx++)
        {
            Room targetRoom = RoomManager.Instance.roomLIst[idx];
            targetRoom.roomIdx = (targetRoom.roomCenter.x / (mapSize.x / maxRoomCnt_X)) + (targetRoom.roomCenter.y / (mapSize.y / maxRoomCnt_Y)) * maxRoomCnt_Y;
        }
        RoomManager.Instance.roomLIst = RoomManager.Instance.roomLIst.OrderBy(x => x.roomIdx).ToList();


        for (int idx = 0; idx < RoomManager.Instance.roomCnt; idx++)
        {
            // �� ������ Ÿ�� room
            Room targetRoom = RoomManager.Instance.roomLIst[idx];

            // ������ Room ����ŭ �����¿� �� ���翩�� �˻�
            Room leftRoom = null;
            Room rightRoom = null;
            Room upRoom = null;
            Room downRoom = null;
            if ((idx % maxRoomCnt_X) > 0) leftRoom = RoomManager.Instance.roomLIst[idx-1];
            if ((idx % maxRoomCnt_X) < maxRoomCnt_X-1) rightRoom = RoomManager.Instance.roomLIst[idx+1];
            if ((idx / maxRoomCnt_X) > 0) upRoom = RoomManager.Instance.roomLIst[idx-maxRoomCnt_Y];
            if ((idx / maxRoomCnt_X) < maxRoomCnt_Y-1) downRoom = RoomManager.Instance.roomLIst[idx+maxRoomCnt_Y];

            // ���� �� ����
            if (leftRoom != null)
            {
                // �̹� ���� �濡 ������ ���� �����Ǿ� �ִٸ� �������� ����
                if (!leftRoom.isRightRoad)
                {
                    leftRoom.isRightRoad = true;
                    targetRoom.isLeftRoad = true;
                    for (int i = targetRoom.roomCenter.x; i > leftRoom.roomCenter.x; i--)
                        tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, (leftRoom.roomCenter.y+targetRoom.roomCenter.y)/2 - mapSize.y / 2, 0), roomTile);
                }
            }

            // ������ �� ����
            if (rightRoom != null)
            {
                // �̹� ������ �濡 ���� ���� �����Ǿ� �ִٸ� �������� ����
                if (!rightRoom.isLeftRoad)
                {
                    rightRoom.isLeftRoad = true;
                    targetRoom.isRightRoad = true;
                    for (int i = targetRoom.roomCenter.x; i < rightRoom.roomCenter.x; i++)
                        tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, (targetRoom.roomCenter.y + rightRoom.roomCenter.y) / 2 - mapSize.y / 2, 0), roomTile);
                }
            }

            // ���� �� ����
            if (upRoom != null)
            {
                // �̹� ���� �濡 �Ʒ����� �����Ǿ� �ִٸ� �������� ����
                if (!upRoom.isDownRoad)
                {
                    upRoom.isDownRoad = true;
                    targetRoom.isUpRoad = true;
                    for (int i = targetRoom.roomCenter.y; i > upRoom.roomCenter.y; i--)
                        tileMap.SetTile(new Vector3Int((targetRoom.roomCenter.x+upRoom.roomCenter.x)/2 - mapSize.x / 2, i - mapSize.y / 2, 0), roomTile);
                }
            }

            // �Ʒ��� �� ����
            if (downRoom != null)
            {
                // �̹� �Ʒ��� �濡 ������ �����Ǿ� �ִٸ� �������� ����
                if (!downRoom.isUpRoad)
                {
                    downRoom.isUpRoad = true;
                    targetRoom.isDownRoad = true;
                    for (int i = targetRoom.roomCenter.y; i < downRoom.roomCenter.y; i++)
                        tileMap.SetTile(new Vector3Int((targetRoom.roomCenter.x + downRoom.roomCenter.x) / 2 - mapSize.x / 2, i - mapSize.y / 2, 0), roomTile);
                }
            }

        }


    }
}
