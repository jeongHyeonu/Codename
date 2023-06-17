using DG.Tweening;
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

partial class EnemyTurn : MonoBehaviour
{
    // ���� Ÿ�ϸ� ����
    [SerializeField]
    public Tilemap tilemap;

    // �� Ÿ��
    [SerializeField]
    public EnemyType enemyType;

    // ��ä�� Ž������
    [SerializeField]
    public GameObject circularSector;

    // �̵�����
    [SerializeField]
    public List<Vector2> enemyMovePos;


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
                    //Debug.Log("Tile at position (" + x + ", " + y + "): " + tilemap.GetTile(new Vector3Int(x, y)).name);
                    
                }
                else
                {
                    // Ÿ���� ���� ��쿡 ���� ó��
                    //Debug.Log("No tile at position (" + x + ", " + y + ")");
                }
            }
        }

        // Enemy A
        if(enemyType == EnemyType.enemyA) 
        {
            StartCoroutine(enemyA_move());
        }
    }


}

partial class EnemyTurn
{
    int curPosIdx = 0;
    float velocity = 2f;
    public Tweener tweener;

    IEnumerator enemyA_move()
    {
        // ���� �������� �������
        if (circularSector.GetComponent<CircularSector>().isCollision == true) yield return null;
        else
        {
            Vector3 nextMovePos = enemyMovePos[(curPosIdx++) % enemyMovePos.Count];
            float duration = Vector2.Distance(transform.position, nextMovePos) / velocity;

            // ��ä�� ũ���� Ž�� ���� ȸ��
            Vector3 _dir = nextMovePos - transform.position;
            circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f); // circularSector.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)));// = Quaternion.Euler(_dir);
                                                                                                                      //Debug.Log(Mathf.Atan2(_dir.y, _dir.x)*Mathf.Rad2Deg+180);

            // �̵� ��θ� �����մϴ�.
            tweener = transform.DOMove(nextMovePos, duration).SetEase(Ease.Linear);



            // �ݺ�
            yield return new WaitForSeconds(duration);

            StartCoroutine(enemyA_move());
        }
    }
}