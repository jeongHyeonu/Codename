using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  // OnDrawGizmos

public class CircularSector : MonoBehaviour
{
    public Transform target;    // ��ä�ÿ� ���ԵǴ��� �Ǻ��� Ÿ��
    public float angleRange = 30f;
    public float radius = 3f;

    Color _blue = new Color(0f, 0f, 1f, 0.2f);
    Color _red = new Color(1f, 0f, 0f, 0.2f);

    public bool isCollision = false;

    void Update()
    {
        Vector3 interV = target.position - transform.position;

        // target�� �� ������ �Ÿ��� radius ���� �۴ٸ�
        if (interV.magnitude <= radius)
        {
            // 'Ÿ��-�� ����'�� '�� ���� ����'�� ����
            float dot = Vector3.Dot(interV.normalized, transform.right);
            // �� ���� ��� ���� �����̹Ƿ� ���� ����� cos�� ���� ���ؼ� theta�� ����
            float theta = Mathf.Acos(dot);
            // angleRange�� ���ϱ� ���� degree�� ��ȯ
            float degree = Mathf.Rad2Deg * theta;

            // �þ߰� �Ǻ�
            if (degree <= angleRange / 2f)
                isCollision = true;
            else
                isCollision = false;

        }
        else
            isCollision = false;
    }

    // ����Ƽ �����Ϳ� ��ä���� �׷��� �޼ҵ�
    private void OnDrawGizmos()
    {
        // ��ä���� �߽� ��ġ
        Vector3 position = transform.position;

        // ��ä���� ���� ���� ����
        Vector3 startDirection = Quaternion.Euler(0f, 0f, -angleRange / 2f) * transform.right;

        // ��ä���� ������
        Vector3 startPoint = position + startDirection * radius;

        // ��ä���� �߽ɰ� ������ ������ ��
        Gizmos.DrawLine(position, startPoint);

        // ��ä���� ��
        Gizmos.DrawWireSphere(position, radius);

        // ��ä���� �������� ���� ������ ��
        Vector3 endPoint = Quaternion.Euler(0f, 0f, angleRange / 2f) * transform.right * radius;
        Gizmos.DrawLine(position, position + endPoint);

        // ��ä���� ����
        Vector3 arcPoint = Quaternion.Euler(0f, 0f, angleRange / 2f) * startDirection;
        Gizmos.DrawLine(position, position + arcPoint * radius);
    }
}