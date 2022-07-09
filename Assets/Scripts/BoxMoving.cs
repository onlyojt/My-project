using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxMoving : MonoBehaviour
{
    private new SpriteRenderer renderer; // �� �׸���
    public GameObject trace; // ����
    public TMP_InputField massInput; // ���� �Է�
    public TMP_InputField forceInput; // �� �Է� ������ N

    private float mass; // ������
    private float force; // ����
    private float forceText; // ���� ���� ǥ�ø� ���� ��
    private float acceleration; // ���ӵ�
    private readonly float gravity = -9.81f; // g
    private float gAccel; // �߷°��ӵ�
    private float speedY; // �Ʒ� ��ġ ��ȭ ��
    private float speedX; // ������ ��ġ ��ȭ ��

    private bool bMove = false; // �� �̵� ����
    private Vector3 initPos; // �� �ʱ� ��ġ
    private int colorIndex = 0; // �� ���� ��ȭ �ε���

    private float time = 0; // �� ���� �ð�

    // �� ���� ������
    private Color[] colors = new Color[] { Color.yellow, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta }; 

    // Start is called before the first frame update
    void Start()
    {
        // �� �ʱ� ��ġ ����
        initPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // �� �ʱ� ����
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(colors[1].r, colors[1].g, colors[1].b, 1f);
    }
    private void Update()
    {
        // ���α׷� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �� �̵�
        if (bMove)
        {
            time += Time.fixedDeltaTime;

            // F = ma
            acceleration = force / mass;
            speedX += acceleration * Time.fixedDeltaTime;
            gAccel = gravity / mass;
            speedY += gAccel * Time.fixedDeltaTime;

            // �� �̵�
            transform.Translate(speedX, speedY, 0);

            // ���� �ε���
            if(transform.position.y < -0.5f || transform.position.x > 45.5f)
            {
                bMove = false; // �̵� ����
                CancelInvoke("DrawTimeTrace"); // ���� �׸��� ����

                GameObject newTrace = DrawTimeTrace(); // �������� ����ǥ��
                GameObject infoText = newTrace.transform.Find("InfoText").gameObject;
                infoText.SetActive(true);

                Trace trace = infoText.GetComponent<Trace>();
                float timeInfo = Mathf.Round(time * 100) * 0.01f;
                string infoString = string.Format($"Mass:{mass}, Force:{forceText}, Time:{timeInfo}");
                trace.SetInfoText(infoString, colors[colorIndex]);
            }

            force = 0; // ó�� �־��� ������ ����
        }
    }

    // ���� �׸���
    private GameObject DrawTimeTrace() 
    {
        GameObject newTrace = Instantiate(trace, transform.position, transform.rotation);
        SpriteRenderer newRenderer = newTrace.GetComponent<SpriteRenderer>();
        // ���� ��ȭ
        newRenderer.color = new Color(colors[colorIndex].r, colors[colorIndex].g, colors[colorIndex].b, 0.3f);

        return newTrace;
    }

    // ��� �ʱ�ȭ
    private void ResetObjects()
    {
        bMove = false;

        CancelInvoke("DrawTimeTrace");

        transform.position = initPos;

        acceleration = 0;
        speedX = 0;
        gAccel = 0;
        speedY = 0;
        time = 0;

        colorIndex++;
        if(colorIndex >= colors.Length)
            colorIndex = 0;

        renderer.color = new Color(colors[colorIndex].r, colors[colorIndex].g, colors[colorIndex].b, 1f);
    }

    // ��� �� �����
    public void ClearAll()
    {
        ResetObjects();

        GameObject [] gameObjects = GameObject.FindGameObjectsWithTag("Trace");
        foreach(GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }

    // �� �̵�
    public void StartSimulation()
    {
        ResetObjects();

        float.TryParse(massInput.text, out mass);
        float.TryParse(forceInput.text, out force);
        forceText = force;

        bMove = true;
        InvokeRepeating("DrawTimeTrace", 0, 0.1f); // 0.1�ʸ��� ���븦 �׸�
    }
}
