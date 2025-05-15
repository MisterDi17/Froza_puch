using UnityEngine;

[System.Serializable]
public class RotationSettings
{
    [Header("Base Rotation")]
    public float minRotationSpeed = 50f;    // ����������� �������� �������� (����� ����� ������� �� ������)
    public float maxRotationSpeed = 300f;   // ������������ �������� �������� (����� ������� � ��������������� �������)
    public float accelerationCurve = 1.5f;  // ������ ��������� ��������
}