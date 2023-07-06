using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMoveNode
{
    public TurnMoveNode(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public TurnMoveNode ParentNode;

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}
public class TurnModePathFinder : MonoBehaviour
{
    [SerializeField] GameObject roadPrefab;

    public Vector2 bottomLeft, topRight, startPos, targetPos;
    //Vector2 startPos, targetPos;
    public List<TurnMoveNode> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeY;
    TurnMoveNode[,] NodeArray;
    TurnMoveNode StartNode, TargetNode, CurNode;
    List<TurnMoveNode> OpenList, ClosedList;
    
    // �̵��� ����
    //Vector3 nextMovePos;

    private GameObject newRoad;
    // �� ��� ����Ʈ
    //[SerializeField]
    //GameObject RoadList;

    void Update()
    {
        
    }

    public List<TurnMoveNode> GenerateRoad(Vector3 curPos, Vector3 nextMovePos, Transform roadList)
    {
        if (roadList.transform.childCount > 0) newRoad=null;

        newRoad = Instantiate(roadPrefab, this.transform);
        newRoad.transform.localPosition = new Vector3(0, 0, 0);
        newRoad.transform.SetParent(roadList.transform);

        

        GameObject _road = roadPrefab.transform.GetChild(0).GetChild(0).gameObject;
        GameObject _point = roadPrefab.transform.GetChild(0).GetChild(1).gameObject;

        // ������ �� ����
        GameObject startPoint = Instantiate(_point, newRoad.transform);
        GameObject endPoint = Instantiate(_point, newRoad.transform);
        startPoint.SetActive(true);
        endPoint.SetActive(true);
        endPoint.transform.position = new Vector3(nextMovePos.x, nextMovePos.y, 0);
        startPos = new Vector2(curPos.x, curPos.y);
        targetPos = nextMovePos;
        PathFinding();

        return FinalNodeList;

        // LineRenderer ����, �� �̵����(A* �ִܰŸ� �˰���)��� ������ ǥ��
        //LineRenderer lr = this.GetComponent<LineRenderer>();
        //lr.positionCount = FinalNodeList.Count;
        //for (int i = 0; i < FinalNodeList.Count; i++)
        //{
        //    lr.SetPosition(i, new Vector3(FinalNodeList[i].x, FinalNodeList[i].y));
        //}
    }
    private void PathFinding()
    {
        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
        sizeX = (int)topRight.x - (int)bottomLeft.x + 1;
        sizeY = (int)topRight.y - (int)bottomLeft.y + 1;
        NodeArray = new TurnMoveNode[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                NodeArray[i, j] = new TurnMoveNode(isWall, i + (int)bottomLeft.x, j + (int)bottomLeft.y);
            }
        }


        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[(int)startPos.x - (int)bottomLeft.x, (int)startPos.y - (int)bottomLeft.y];
        TargetNode = NodeArray[(int)targetPos.x - (int)bottomLeft.x, (int)targetPos.y - (int)bottomLeft.y];

        OpenList = new List<TurnMoveNode>() { StartNode };
        ClosedList = new List<TurnMoveNode>();
        FinalNodeList = new List<TurnMoveNode>();


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
                TurnMoveNode TargetCurNode = TargetNode;
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
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - (int)bottomLeft.x, checkY - (int)bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - (int)bottomLeft.x, checkY - (int)bottomLeft.y]))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (allowDiagonal) if (NodeArray[(int)CurNode.x - (int)bottomLeft.x, checkY - (int)bottomLeft.y].isWall && NodeArray[(int)checkX - (int)bottomLeft.x, (int)CurNode.y - (int)bottomLeft.y].isWall) return;

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (dontCrossCorner) if (NodeArray[CurNode.x - (int)bottomLeft.x, checkY - (int)bottomLeft.y].isWall || NodeArray[checkX - (int)bottomLeft.x, CurNode.y - (int)bottomLeft.y].isWall) return;


            // �̿���忡 �ְ�, ������ 10, �밢���� 14���
            TurnMoveNode NeighborNode = NodeArray[checkX - (int)bottomLeft.x, checkY - (int)bottomLeft.y];
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
    IEnumerator roadUX()
    {
        LineRenderer lr = this.GetComponent<LineRenderer>();
        lr.material.SetTextureOffset("_MainTex", new Vector2(lr.material.GetTextureOffset("_MainTex").x - 0.1f, 0f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(roadUX());
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

