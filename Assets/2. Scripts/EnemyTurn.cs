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

    // �̵��� ����
    Vector3 nextMovePos;

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

        // Enemy C
        if (enemyType == EnemyType.enemyC)
        {
            enemyC_move();
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
            nextMovePos = enemyMovePos[(curPosIdx++) % enemyMovePos.Count];
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


    private void enemyC_move(bool isUserTurn=true)
    {

        // ���� ���� ��� �̵��� ��� ǥ��
        if (isUserTurn)
        {
            int Rand_X = Random.Range((int)this.transform.localPosition.x-3, (int)this.transform.localPosition.x+3);
            int Rand_Y = Random.Range((int)this.transform.localPosition.y-3, (int)this.transform.localPosition.y+3);
            nextMovePos = new Vector2(Rand_X, Rand_Y);

            GenerateRoad();
        }
        // ��� ���� ��� �̵�
        else
        {
            float duration = Vector2.Distance(transform.position, nextMovePos) / velocity;

            // ��ä�� ũ���� Ž�� ���� ȸ��
            Vector3 _dir = nextMovePos - transform.position;
            circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f); // circularSector.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)));// = Quaternion.Euler(_dir);
                                                                                                                      //Debug.Log(Mathf.Atan2(_dir.y, _dir.x)*Mathf.Rad2Deg+180);

            newEnemyRoad.SetActive(false);
            tweener = transform.DOMove(nextMovePos, duration).SetEase(Ease.Linear)
                .OnComplete(() => { 
                    enemyC_move(true);
                    newEnemyRoad.SetActive(true);
                }); // �̵� ������ ���������� �н�
        }
    }

    public void TurnPass()
    {
        enemyC_move(false);
    }
}

// �� �̵���� ǥ��
partial class EnemyTurn
{
    [SerializeField] GameObject roadPrefab;
    private GameObject newEnemyRoad;

    private void GenerateRoad()
    {
        newEnemyRoad = Instantiate(roadPrefab,this.transform);
        newEnemyRoad.transform.localPosition = new Vector3(0,0,0);

        GameObject _road = roadPrefab.transform.GetChild(0).GetChild(0).gameObject;
        GameObject _point = roadPrefab.transform.GetChild(0).GetChild(1).gameObject;

        // ������ �� ����
        GameObject startPoint = Instantiate(_point, newEnemyRoad.transform);
        GameObject endPoint = Instantiate(_point, newEnemyRoad.transform);
        startPoint.SetActive(true);
        endPoint.SetActive(true);
        endPoint.transform.position = nextMovePos;


        // �� ��� �� ��ġ ����
        float dist = Vector2.Distance(startPoint.transform.localPosition, endPoint.transform.localPosition);
        for (int i = 1; i <= (int)dist; i++)
        {
            GameObject road = Instantiate(_road, newEnemyRoad.transform.GetChild(0));
            road.SetActive(true);
            road.transform.localPosition = new Vector2(startPoint.transform.localPosition.x + i, 0);
            //float dist = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
        }
        float angle = Mathf.Atan2(endPoint.transform.localPosition.y, endPoint.transform.localPosition.x) * Mathf.Rad2Deg;
        newEnemyRoad.transform.GetChild(0).DORotate(new Vector3(0, 0, angle),0f);

    }
}