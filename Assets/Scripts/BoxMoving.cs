using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxMoving : MonoBehaviour
{
    private new SpriteRenderer renderer; // 공 그리기
    public GameObject trace; // 자취
    public TMP_InputField massInput; // 질량 입력
    public TMP_InputField forceInput; // 힘 입력 단위는 N

    private float mass; // 질량값
    private float force; // 힘값
    private float forceText; // 힘값 정보 표시를 위한 값
    private float acceleration; // 가속도
    private readonly float gravity = -9.81f; // g
    private float gAccel; // 중력가속도
    private float speedY; // 아래 위치 변화 값
    private float speedX; // 오른쪽 위치 변화 값

    private bool bMove = false; // 고 이동 여부
    private Vector3 initPos; // 공 초기 위치
    private int colorIndex = 0; // 공 색상 변화 인덱스

    private float time = 0; // 공 낙하 시간

    // 공 색상 정보들
    private Color[] colors = new Color[] { Color.yellow, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta }; 

    // Start is called before the first frame update
    void Start()
    {
        // 공 초기 위치 저장
        initPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // 공 초기 색상
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(colors[1].r, colors[1].g, colors[1].b, 1f);
    }
    private void Update()
    {
        // 프로그램 종료
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
        // 공 이동
        if (bMove)
        {
            time += Time.fixedDeltaTime;

            // F = ma
            acceleration = force / mass;
            speedX += acceleration * Time.fixedDeltaTime;
            gAccel = gravity / mass;
            speedY += gAccel * Time.fixedDeltaTime;

            // 공 이동
            transform.Translate(speedX, speedY, 0);

            // 벽에 부딛힘
            if(transform.position.y < -0.5f || transform.position.x > 45.5f)
            {
                bMove = false; // 이동 중지
                CancelInvoke("DrawTimeTrace"); // 자취 그리기 중지

                GameObject newTrace = DrawTimeTrace(); // 마지막에 정보표시
                GameObject infoText = newTrace.transform.Find("InfoText").gameObject;
                infoText.SetActive(true);

                Trace trace = infoText.GetComponent<Trace>();
                float timeInfo = Mathf.Round(time * 100) * 0.01f;
                string infoString = string.Format($"Mass:{mass}, Force:{forceText}, Time:{timeInfo}");
                trace.SetInfoText(infoString, colors[colorIndex]);
            }

            force = 0; // 처음 주어진 힘으로 설정
        }
    }

    // 자취 그리기
    private GameObject DrawTimeTrace() 
    {
        GameObject newTrace = Instantiate(trace, transform.position, transform.rotation);
        SpriteRenderer newRenderer = newTrace.GetComponent<SpriteRenderer>();
        // 색상 변화
        newRenderer.color = new Color(colors[colorIndex].r, colors[colorIndex].g, colors[colorIndex].b, 0.3f);

        return newTrace;
    }

    // 모든 초기화
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

    // 모든 공 지우기
    public void ClearAll()
    {
        ResetObjects();

        GameObject [] gameObjects = GameObject.FindGameObjectsWithTag("Trace");
        foreach(GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }

    // 공 이동
    public void StartSimulation()
    {
        ResetObjects();

        float.TryParse(massInput.text, out mass);
        float.TryParse(forceInput.text, out force);
        forceText = force;

        bMove = true;
        InvokeRepeating("DrawTimeTrace", 0, 0.1f); // 0.1초마다 자취를 그림
    }
}
