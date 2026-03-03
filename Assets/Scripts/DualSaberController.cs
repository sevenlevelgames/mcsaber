using UnityEngine;

public class DualSaberController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform leftSaber;
    public Transform rightSaber;
    public float moveSpeed = 10f;

    [Header("Maxmin")]
    public float minY = 0.5f;
    public float maxY = 1.5f;

    [Header("Mobile controls")]
    public float mobileTouchSensitivity = 1.3f;
    public float touchYOffset = 0.1f;

    [Header("Swing Settings")]
    public float tiltAngle = 30f;
    public float tiltSpeed = 10f;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        float pcLeftInput = GetKeyboardInput(KeyCode.W, KeyCode.S);
        float pcRightInput = GetKeyboardInput(KeyCode.O, KeyCode.L);

        if (Input.touchCount == 0)
        {
            HandleSaberMovement(leftSaber, pcLeftInput);
            HandleSaberMovement(rightSaber, pcRightInput);
            return;
        }

        float mobileLeftTargetY = leftSaber.position.y;
        float mobileRightTargetY = rightSaber.position.y;

        float centerY = (minY + maxY) / 2f;

        bool leftTouched = false;
        bool rightTouched = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            Vector3 touchWorldPos = GetWorldPositionFromTouch(t.position, leftSaber.position.z);

            float rawDiff = touchWorldPos.y - centerY;
            float amplifiedY = centerY + (rawDiff * mobileTouchSensitivity);

            amplifiedY -= touchYOffset;

            float finalTargetY = Mathf.Clamp(amplifiedY, minY, maxY);

            if (t.position.x < Screen.width / 2)
            {
                mobileLeftTargetY = finalTargetY;
                leftTouched = true;
            }
            else
            {
                mobileRightTargetY = finalTargetY;
                rightTouched = true;
            }
        }

        if (leftTouched) MoveSaberToTarget(leftSaber, mobileLeftTargetY);
        else HandleSwingOnly(leftSaber, 0);

        if (rightTouched) MoveSaberToTarget(rightSaber, mobileRightTargetY);
        else HandleSwingOnly(rightSaber, 0);
    }


    float GetKeyboardInput(KeyCode upKey, KeyCode downKey)
    {
        if (Input.GetKey(upKey)) return 1;
        else if (Input.GetKey(downKey)) return -1;
        return 0;
    }

    Vector3 GetWorldPositionFromTouch(Vector2 screenPos, float zDepth)
    {
        float distanceToScreen = Mathf.Abs(mainCam.transform.position.z - zDepth);
        return mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceToScreen));
    }

    void HandleSaberMovement(Transform saber, float input)
    {
        if (saber == null) return;
        saber.Translate(Vector3.up * input * moveSpeed * Time.deltaTime, Space.World);

        Vector3 clampedPos = saber.position;
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
        saber.position = clampedPos;

        HandleSwingOnly(saber, input);
    }

    void MoveSaberToTarget(Transform saber, float targetY)
    {
        Vector3 currentPos = saber.position;
        float newY = Mathf.MoveTowards(currentPos.y, targetY, moveSpeed * Time.deltaTime);

        float calculatedInput = 0;
        if (newY > currentPos.y + 0.001f) calculatedInput = 1;
        else if (newY < currentPos.y - 0.001f) calculatedInput = -1;

        currentPos.y = newY;
        saber.position = currentPos;

        HandleSwingOnly(saber, calculatedInput);
    }

    void HandleSwingOnly(Transform saber, float inputDirection)
    {
        float targetRotX = 0;
        if (inputDirection > 0) targetRotX = tiltAngle;
        else if (inputDirection < 0) targetRotX = -tiltAngle;

        Quaternion targetRotation = Quaternion.Euler(targetRotX, 0, 0);
        saber.localRotation = Quaternion.Lerp(saber.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}