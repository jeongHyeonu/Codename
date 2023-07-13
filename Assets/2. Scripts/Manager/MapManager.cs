using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect; //�и��� ������ rect����
    public RectInt roomRect; //�и��� ���� �� ���� rect����
    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
        //���� ��� ��. ��� ���� ���� �� ���
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}
public class MapManager : MonoBehaviour
{
    [SerializeField] public Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //������ �������� �ּ� ����
    [SerializeField] float maximumDivideRate; //������ �������� �ִ� ����

    [SerializeField] public int maxDepth; //Ʈ���� ����, ���� ���� ���� �� �ڼ��� ������ ��

    [SerializeField] public Tilemap tileMap;
    [SerializeField] public Tile roomTile; //���� �����ϴ� Ÿ��
    [SerializeField] Tile wallTile; //��� �ܺθ� ���������� �� Ÿ��
    [SerializeField] Tile outTile; //�� �ܺ��� Ÿ��
    [SerializeField] public Tile ckTile; //�� �ܺ��� Ÿ��

    [Header("=== Room Manager / Road Manager ===")]
    [SerializeField] public GameObject roomManager;
    [SerializeField] public GameObject roadManager;

    private bool isPlayerSpawned;
    //private GameObject player;

    //private void Awake()
    //{
    //    player = GameManager.Instance.player;
    //}
    void Start()
    {
        FillBackground();//�� �ε� �� ���δ� �ٱ�Ÿ�Ϸ� ����
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        
        GenerateLoad(root, 0);
        FillWall(); //�ٱ��� ���� ������ ������ ������ ĥ���ִ� �Լ�
    }

    void Divide(Node tree, int n)
    {
        if (n == maxDepth) return; //���� ���ϴ� ���̿� �����ϸ� �� �������� �ʴ´�.
                                       //�� ���� ��쿡��

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //���ο� ������ �� ����� ������, ���ΰ� ��ٸ� �� ��, ��� ���ΰ� �� ��ٸ� ��, �Ʒ��� �����ְ� �� ���̴�.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //���� �� �ִ� �ִ� ���̿� �ּ� �����߿��� �������� �� ���� ����
        if (tree.nodeRect.width >= tree.nodeRect.height) //���ΰ� �� ����� ��쿡�� �� ��� ������ �� ���̸�, �� ��쿡�� ���� ���̴� ������ �ʴ´�.
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            //���� ��忡 ���� ������ 
            //��ġ�� ���� �ϴ� �����̹Ƿ� ������ ������, ���� ���̴� ������ ���� �������� �־��ش�.
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            //���� ��忡 ���� ������ 
            //��ġ�� ���� �ϴܿ��� ���������� ���� ���̸�ŭ �̵��� ��ġ�̸�, ���� ���̴� ���� ���α��̿��� ���� ���� ���ΰ��� �� ������ �κ��� �ȴ�. 
        }
        else
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
            //DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
        }
        tree.leftNode.parNode = tree; //�ڽĳ����� �θ��带 �������� ���� ����
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //����, ������ �ڽ� ���鵵 �����ش�.
        Divide(tree.rightNode, n + 1);//����, ������ �ڽ� ���鵵 �����ش�.
    }
    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maxDepth) //�ش� ��尡 ��������� ���� ����� �� ���̴�.
        {
            rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);
            //���� ���� �ּ� ũ��� ����� ���α����� ����, �ִ� ũ��� ���α��̺��� 1 �۰� ������ �� �� ���� ���� ������ ���� �����ش�.
            int height = Random.Range(rect.height / 2, rect.height - 1);
            //���̵� ���� ����.
            int x = rect.x + Random.Range(1, rect.width - width);
            //���� x��ǥ�̴�. ���� 0�� �ȴٸ� �پ� �ִ� ��� �������� ������,�ּڰ��� 1�� ���ְ�, �ִ��� ���� ����� ���ο��� ���� ���α��̸� �� �� ���̴�.
            int y = rect.y + Random.Range(1, rect.height - height);
            //y��ǥ�� ���� ����.
            rect = new RectInt(x, y, width, height);
            FillRoom(rect);
            Room room = new Room();
            room.rect = rect;
            room.roomCenter = new Vector2Int(x + width / 2, y + height / 2);
            roomManager.GetComponent<RoomManager>().roomLIst.Add(room);
            roomManager.GetComponent<RoomManager>().roomCnt++;

            int player_x = Random.Range(0, 10);
            if(!isPlayerSpawned && player_x == 0 || !isPlayerSpawned && roomManager.GetComponent<RoomManager>().roomCnt == 15)
            {
                Instantiate(GameManager.Instance.player, new Vector3(room.roomCenter.x-mapSize.x/2, room.roomCenter.y-mapSize.y / 2, 0), Quaternion.identity);
                Instantiate(GameManager.Instance.enemyA, new Vector3(Random.RandomRange(room.rect.x, room.rect.x + room.rect.width) - mapSize.x / 2, Random.RandomRange(room.rect.y, room.rect.y + room.rect.height) - mapSize.y / 2, 0), Quaternion.identity);
                Instantiate(GameManager.Instance.enemyB, new Vector3(Random.RandomRange(room.rect.x, room.rect.x + room.rect.width) - mapSize.x / 2, Random.RandomRange(room.rect.y, room.rect.y + room.rect.height) - mapSize.y / 2, 0), Quaternion.identity);
                Instantiate(GameManager.Instance.enemyC, new Vector3(Random.RandomRange(room.rect.x, room.rect.x + room.rect.width) - mapSize.x / 2, Random.RandomRange(room.rect.y, room.rect.y + room.rect.height) - mapSize.y / 2, 0), Quaternion.identity);


                isPlayerSpawned = true;
            }
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }
    private void GenerateLoad(Node tree, int n)
    {
        if (n == maxDepth) //���� ����� ���� �ڽ��� ����.
            return;

        roadManager.GetComponent<RoadManager>().GenerateRoad();
        {
            //Vector2Int leftNodeCenter = tree.leftNode.center;
            //Vector2Int rightNodeCenter = tree.rightNode.center;

            //bool flag = false;

            //if (Mathf.Min(leftNodeCenter.x, rightNodeCenter.x) == leftNodeCenter.x)
            //{
            //    for (int num = 0; num < tree.leftNode.roomRect.height; num++)
            //    {
            //        if (tileMap.GetTile(new Vector3Int(tree.leftNode.roomRect.x + tree.leftNode.roomRect.width - mapSize.x / 2, tree.leftNode.roomRect.y + num - mapSize.y/2 , 0)) == roomTile)
            //        {
            //            flag = true;
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int num = 0; num < tree.rightNode.roomRect.height; num++)
            //    {
            //        if (tileMap.GetTile(new Vector3Int(tree.rightNode.roomRect.x - tree.rightNode.roomRect.width - mapSize.x / 2, tree.rightNode.roomRect.y + num - mapSize.y / 2, 0)) == roomTile)
            //        {
            //            flag = true;
            //            break;
            //        }
            //    }
            //}
            //// ���ι��� ��
            //for (int i = Mathf.Min(leftNodeCenter.x, rightNodeCenter.x); i <= Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++)
            //{
            //    if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2, 0)) == roomTile)
            //        continue;
            //    // ���ι��� �� �̹� ����������� �˻�
            //    if (!flag)
            //        tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2, 0), ckTile);
            //}

            //flag = false;
            //if (Mathf.Min(leftNodeCenter.y, rightNodeCenter.y) == leftNodeCenter.y)
            //{
            //    for (int num = 0; num < tree.leftNode.roomRect.width; num++)
            //    {
            //        if (tileMap.GetTile(new Vector3Int(tree.leftNode.roomRect.x + num - mapSize.x/2, tree.leftNode.roomRect.y + tree.leftNode.roomRect.height - mapSize.y/2 , 0)) == roomTile)
            //        {
            //            flag = true;
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int num = 0; num < tree.rightNode.roomRect.width; num++)
            //    {

            //        if (tileMap.GetTile(new Vector3Int(tree.rightNode.roomRect.x + num - mapSize.x/2, tree.rightNode.roomRect.y - tree.rightNode.roomRect.height - mapSize.y/2 , 0)) == roomTile)
            //        {
            //            flag = true;
            //            break;
            //        }
            //    }
            //}
            //// ���ι��� ��
            //for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++)
            //{
            //    if (tileMap.GetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0)) == roomTile)
            //        continue;

            //    if (!flag)
            //        tileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0), ckTile);
            //}
            ////���� �����ÿ��� ������ ������� �κ��� room tile�� ä��� ����
            //GenerateLoad(tree.leftNode, n + 1); //�ڽ� ���鵵 Ž��
            //GenerateLoad(tree.rightNode, n + 1);
        }
    }

    void FillBackground() //����� ä��� �Լ�, �� load�� ���� ���� ���ش�.
    {
        for (int i = -10; i < mapSize.x + 10; i++) //�ٱ�Ÿ���� �� �����ڸ��� ���� ������� �ʰ�
        //�� ũ�⺸�� �а� ä���ش�.
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }
    void FillWall() //�� Ÿ�ϰ� �ٱ� Ÿ���� ������ �κ�
    {
        for (int i = 0; i < mapSize.x; i++) //Ÿ�� ��ü�� ��ȸ
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == outTile)
                {
                    //�ٱ�Ÿ�� �� ���
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;//�ٱ� Ÿ�� ���� 8������ Ž���ؼ� room tile�� �ִٸ� wall tile�� �ٲ��ش�.
                            if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2 + x, j - mapSize.y / 2 + y, 0)) == roomTile)
                            {
                                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void FillRoom(RectInt rect)
    { 
        // �� ��ġ ������ �������� ������Ʈ, ���� ����(enum���� ���� ������ ǥ��, 2���� �迭�� �� ���� ǥ��)


        //room�� rect������ �޾Ƽ� tile�� set���ִ� �Լ�
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
            }
        }
    }

}