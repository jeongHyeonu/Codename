using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// �ǰ� : �� Ÿ�Կ� ���� ���߿� Ŭ������ �������̽��� ���ؼ� �����ϴ°� ���� �� ���ƿ�
//        ������ �ϵ��ڵ����� enemyA,B,C���� üũ�ϴ� ����̶�.. 
public enum EnemyType
{
    enemyA,
    enemyB,
    enemyC,
}

public class EnemyTurn : MonoBehaviour
{
    // ���� Ÿ�ϸ� ����
    [SerializeField]
    public Tilemap tilemap;

    // �� Ÿ��
    [SerializeField]
    public EnemyType enemyType;

    private void Start()
    {
        // Tilemap�� ��ü ������ ������
        BoundsInt bounds = tilemap.cellBounds;

        // ������ ���� Ÿ�� ������ 2���� �迭�� �޾ƿ�
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);

        // �迭�� ����Ͽ� Ÿ�� ���� Ȯ��
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                TileBase tile = tiles[(x - bounds.xMin) + (y - bounds.yMin) * bounds.size.x];

                if (tile != null)
                {
                    // Ÿ���� �ִ� ��쿡 ���� ó��
                    Debug.Log("Tile at position (" + x + ", " + y + "): " + tilemap.GetTile(new Vector3Int(x, y)).name);
                    
                }
                else
                {
                    // Ÿ���� ���� ��쿡 ���� ó��
                    Debug.Log("No tile at position (" + x + ", " + y + ")");
                }
            }
        }
    }
}
