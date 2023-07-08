using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// �ǰ� : �� Ÿ�Կ� ���� ���߿� Ŭ������ �������̽��� ���ؼ� �����ϴ°� ���� �� ���ƿ�
//        ������ �ϵ��ڵ����� enemyA,B,C���� üũ�ϴ� ����̶�.. 
//public enum EnemyType
//{
//    enemyA,
//    enemyB,
//    enemyC,
//}

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
    Vector3 prevMovePos;
    public Sequence s;

    // ���״��� ����
    public bool isAlert = false;
    [SerializeField] GameObject alertSprite;

    // �� ��� ����Ʈ
    GameObject RoadList;

    private void Start()
    {
        RoadList = GameObject.Find("RoadList");

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
            enemyA_move();
        }

        // Enemy C
        if (enemyType == EnemyType.enemyC)
        {
            enemyC_move();
        }

        StartCoroutine(roadUX());
    }

    
}

partial class EnemyTurn
{
    int curPosIdx = 0;
    float velocity = .5f;
    public Tweener tweener;

    private void enemyA_move(bool isUserTurn=true)
    {
        // ���� �������� �������
        //if (circularSector.GetComponent<CircularSector>().isCollision == true) { return; }

        // ���� ���� ���  ��� ǥ��
        if (isUserTurn)
        {
            nextMovePos = enemyMovePos[(curPosIdx++) % enemyMovePos.Count];

            GenerateRoad();
        }
        // ��� ���� ��� �̵�
        else
        {
            s = DOTween.Sequence();
            for (int i = 0; i < FinalNodeList.Count; i++)
            {
                nextMovePos = new Vector3(FinalNodeList[i].x, FinalNodeList[i].y);
                if (i != 0) prevMovePos = new Vector3(FinalNodeList[i - 1].x, FinalNodeList[i - 1].y);

                // ��ä�� ũ���� Ž�� ���� ȸ��
                Vector3 _dir = nextMovePos - prevMovePos;
                s.Append(transform.DOMove(nextMovePos, velocity).SetEase(Ease.Linear)).Join(circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f));


            }

            // �̵� �Ϸ��
            s.Play().OnComplete(() => {
                // �̵���� ���� ���� �̵���� ����
                newEnemyRoad.SetActive(false);
                GetComponent<LineRenderer>().positionCount = 0;
                this.transform.GetComponent<EnemyTurn>().enemyA_move(); // ���������� �н�
            });

        }
    }


    private void enemyC_move(bool isUserTurn=true)
    {
        if (this.enemyType != EnemyType.enemyC) return;

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
            s = DOTween.Sequence();
            for (int i = 0; i < FinalNodeList.Count; i++)
            {
                nextMovePos = new Vector3(FinalNodeList[i].x, FinalNodeList[i].y);
                if(i!=0) prevMovePos = new Vector3(FinalNodeList[i-1].x, FinalNodeList[i-1].y);

                // ��ä�� ũ���� Ž�� ���� ȸ��
                Vector3 _dir = nextMovePos - prevMovePos;
                s.Append(transform.DOMove(nextMovePos, velocity).SetEase(Ease.Linear)).Join(circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f));
                //circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f);

            }

            // �̵� �Ϸ��
            s.Play().OnComplete(() => {
                // �̵���� ���� ���� �̵���� ����
                newEnemyRoad.SetActive(false);
                GetComponent<LineRenderer>().positionCount = 0;
                this.transform.GetComponent<EnemyTurn>().enemyC_move(); // ���������� �н�
            });

        }
    }

    // �� �ѱ��
    // ���� �ϸŴ����� ���ӸŴ������� �����ϴ°� ���� �� ����
    public void TurnPass()
    {
        bool isUserTurn = false;
        GameObject.Find("EnemyA").GetComponent<EnemyTurn>().enemyA_move(isUserTurn);
        GameObject.Find("EnemyC").GetComponent<EnemyTurn>().enemyC_move(isUserTurn);
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
        newEnemyRoad.transform.SetParent(RoadList.transform);

        GameObject _road = roadPrefab.transform.GetChild(0).GetChild(0).gameObject;
        GameObject _point = roadPrefab.transform.GetChild(0).GetChild(1).gameObject;
        
        // ������ �� ����
        GameObject startPoint = Instantiate(_point, newEnemyRoad.transform);
        GameObject endPoint = Instantiate(_point, newEnemyRoad.transform);
        startPoint.SetActive(true);
        endPoint.SetActive(true);
        endPoint.transform.position = nextMovePos;
        startPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        targetPos = new Vector2Int((int)nextMovePos.x, (int)nextMovePos.y);
        PathFinding();


        // LineRenderer ����, �� �̵����(A* �ִܰŸ� �˰���)��� ������ ǥ��
        LineRenderer lr = this.GetComponent<LineRenderer>();
        lr.positionCount = FinalNodeList.Count;
        for (int i = 0; i < FinalNodeList.Count; i++)
        {
            lr.SetPosition(i, new Vector3(FinalNodeList[i].x, FinalNodeList[i].y));
        }

        //// �� ��� �� ��ġ ����
        //float dist = Vector2.Distance(startPoint.transform.localPosition, endPoint.transform.localPosition);
        //for (int i = 1; i <= (int)dist; i++)
        //{
        //    GameObject road = Instantiate(_road, newEnemyRoad.transform.GetChild(0));
        //    road.SetActive(true);
        //    road.transform.localPosition = new Vector2(startPoint.transform.localPosition.x + i, 0);
        //    //float dist = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
        //}
        //float angle = Mathf.Atan2(endPoint.transform.localPosition.y, endPoint.transform.localPosition.x) * Mathf.Rad2Deg;
        //newEnemyRoad.transform.GetChild(0).DORotate(new Vector3(0, 0, angle),0f);

    }
    IEnumerator roadUX()
    {
        LineRenderer lr = this.GetComponent<LineRenderer>();
        lr.material.SetTextureOffset("_MainTex", new Vector2(lr.material.GetTextureOffset("_MainTex").x - 0.1f, 0f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(roadUX());
    }
}



// �� �̵���� - Astar
public class EnemyNode
{
    public EnemyNode(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public EnemyNode ParentNode;

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}
partial class EnemyTurn
{
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<EnemyNode> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeY;
    EnemyNode[,] NodeArray;
    EnemyNode StartNode, TargetNode, CurNode;
    List<EnemyNode> OpenList, ClosedList;


    public void PathFinding()
    {
        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new EnemyNode[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                NodeArray[i, j] = new EnemyNode(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }


        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<EnemyNode>() { StartNode };
        ClosedList = new List<EnemyNode>();
        FinalNodeList = new List<EnemyNode>();


        while (OpenList.Count > 0)
        {
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // ������
            if (CurNode == TargetNode)
            {
                EnemyNode TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int i = 0; i < FinalNodeList.Count; i++)
                {
                    //print(i + "��°�� " + FinalNodeList[i].x + ", " + FinalNodeList[i].y);
                }
                return;
            }


            // �֢آע�
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // �� �� �� ��
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;


            // �̿���忡 �ְ�, ������ 10, �밢���� 14���
            EnemyNode NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (FinalNodeList != null)
        {
            if (FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                    Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
        }
    }
}

// ��Ŵ ����
partial class EnemyTurn
{
    // �÷��̾� ����
    public void DetectPlayer()
    {
        s.Pause();
        this.isAlert = true;
        alertSprite.SetActive(true);
    }
}