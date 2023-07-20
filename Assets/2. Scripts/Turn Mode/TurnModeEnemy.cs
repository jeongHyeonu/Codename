using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    enemyA,
    enemyB,
    enemyC,
}
public class TurnModeEnemy : MonoBehaviour
{
    int curPosIdx = 0;
    float velocity = .5f;
    public Tweener tweener;
    // �� Ÿ��
    [SerializeField]
    public EnemyType enemyType;

    // ��ä�� Ž������
    [SerializeField]
    public GameObject circularSector;

    // �̵�����
    [SerializeField]
    public List<Vector2> enemyMovePos;
    [SerializeField]
    private GameObject roadPrefab;
    private GameObject startPoint;
    private GameObject endPoint;
    // �̵��� ����
    Vector3 nextMovePos;
    Vector3 prevMovePos;
    List<TurnMoveNode> turnMoves;

    public Sequence s;

    // ���״��� ����
    public bool isAlert = false;

    [SerializeField] GameObject alertSprite;

    TurnType turnType, prevTurnType;

    void Start()
    {
        startPoint = roadPrefab.transform.GetChild(1).gameObject;
        endPoint = roadPrefab.transform.GetChild(2).gameObject;
        if (GameManager.Instance.turnManager == null)
            return;
        turnType = GameManager.Instance.turnManager.turnType;
        prevTurnType = turnType;
        EnemyMove(turnType);

    }

    // Update is called once per frame

    private void Update()
    {
        if(GameManager.Instance.turnManager == null)
        {
            turnType = GameManager.Instance.turnManager.turnType;
            prevTurnType = turnType;
            EnemyMove(turnType);
        }
    }

    public void EnemyMove(TurnType turnType)
    {
        if (enemyType == EnemyType.enemyA)
        {
            enemyA_move(turnType);
        }
        // Enemy C
        if (enemyType == EnemyType.enemyC)
        {
            enemyC_move(turnType);
        }
        StartCoroutine(roadUX());
    }
    private void enemyA_move(TurnType turnType)
    {
        // ���� �������� �������
        //if (circularSector.GetComponent<CircularSector>().isCollision == true) { return; }

        // ���� ���� ���  ��� ǥ��
        if (turnType == TurnType.player)
        {
            nextMovePos = transform.position + new Vector3(enemyMovePos[(curPosIdx) % enemyMovePos.Count].x, enemyMovePos[(curPosIdx) % enemyMovePos.Count].y, 0);

            startPoint.transform.position = transform.position;
            endPoint.transform.position = nextMovePos;
;
            turnMoves = GameManager.Instance.turnPathFinder.GenerateRoad(transform.position, nextMovePos, this.transform);
            curPosIdx++;
            LineRenderer lr = this.GetComponent<LineRenderer>();
            lr.positionCount = turnMoves.Count;

            for (int i = 0; i < turnMoves.Count; i++)
            {
                lr.SetPosition(i, new Vector3(turnMoves[i].x, turnMoves[i].y));
            }
            StartCoroutine("roadUX");

        }
        // ��� ���� ��� �̵�
        else
        {
            s = DOTween.Sequence();
            for (int i = 0; i < turnMoves.Count; i++)
            {
                nextMovePos = new Vector3(turnMoves[i].x, turnMoves[i].y);
                if (i != 0) prevMovePos = new Vector3(turnMoves[i - 1].x, turnMoves[i - 1].y);

                // ��ä�� ũ���� Ž�� ���� ȸ��
                Vector3 _dir = nextMovePos - prevMovePos;
                s.Append(transform.DOMove(nextMovePos, velocity).SetEase(Ease.Linear)).Join(circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f));


            }

            // �̵� �Ϸ��
            s.Play().OnComplete(() =>
            {
                // �̵���� ���� ���� �̵���� ����
                GameObject newEnemyRoad = transform.Find("road(Clone)").gameObject;
                newEnemyRoad.SetActive(false);

                GetComponent<LineRenderer>().positionCount = 0;
                this.transform.GetComponent<TurnModeEnemy>().enemyA_move(TurnType.player); // ���������� �н�
            });

        }
    }
    private void enemyC_move(TurnType turnType)
    {
        if (this.enemyType != EnemyType.enemyC) return;

        // ���� ���� ��� �̵��� ��� ǥ��
        if (turnType == TurnType.player)
        {
            int Rand_X = Random.Range((int)this.transform.position.x - 3, (int)this.transform.position.x + 3);
            int Rand_Y = Random.Range((int)this.transform.position.y - 3, (int)this.transform.position.y + 3);
            
            nextMovePos = new Vector2(Rand_X, Rand_Y);

            turnMoves = GameManager.Instance.turnPathFinder.GenerateRoad(transform.position, nextMovePos, this.transform);
            startPoint.transform.position = Vector3.zero;
            endPoint.transform.position = new Vector3(nextMovePos.x, nextMovePos.y, 0);

            LineRenderer lr = this.GetComponent<LineRenderer>();
            lr.positionCount = turnMoves.Count;

            for (int i = 0; i < turnMoves.Count; i++)
            {
                lr.SetPosition(i, new Vector3(turnMoves[i].x, turnMoves[i].y));
            }
        }
        // ��� ���� ��� �̵�
        else
        {
            s = DOTween.Sequence();
            for (int i = 0; i < turnMoves.Count; i++)
            {
                nextMovePos = new Vector3(turnMoves[i].x, turnMoves[i].y);
                if (i != 0) prevMovePos = new Vector3(turnMoves[i - 1].x, turnMoves[i - 1].y);

                // ��ä�� ũ���� Ž�� ���� ȸ��
                Vector3 _dir = nextMovePos - prevMovePos;
                s.Append(transform.DOMove(nextMovePos, velocity).SetEase(Ease.Linear)).Join(circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f));
                //circularSector.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg)), .3f);

            }

            // �̵� �Ϸ��
            s.Play().OnComplete(() =>
            {
                // �̵���� ���� ���� �̵���� ����
                GameObject newEnemyRoad = transform.Find("road(Clone)").gameObject;
                newEnemyRoad.SetActive(false);
                GetComponent<LineRenderer>().positionCount = 0;
                this.transform.GetComponent<TurnModeEnemy>().enemyC_move(TurnType.player); // ���������� �н�
            });

        }
    }
    IEnumerator roadUX()
    {
        LineRenderer lr = this.GetComponent<LineRenderer>();
        lr.material.SetTextureOffset("_MainTex", new Vector2(lr.material.GetTextureOffset("_MainTex").x - 0.1f, 0f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(roadUX());
    }
}
