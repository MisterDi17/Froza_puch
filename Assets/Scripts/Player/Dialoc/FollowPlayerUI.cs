using UnityEngine;
using UnityEngine.UI;

public class FollowPlayerUI : MonoBehaviour
{
    [SerializeField] public Transform player; // ���� ����� ������ � ����������
    [SerializeField] public Vector3 offset = new Vector3(0, 1.5f, 0); // ����� ������� ��� �������

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}
