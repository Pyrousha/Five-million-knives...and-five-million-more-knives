using BeauRoutine;
using System.Collections;
using UnityEngine;

public class Hookshot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hookshotSprite;
    [SerializeField] private LineRenderer lineRend;
    [SerializeField] private Transform playerCenter;
    [SerializeField] private Transform hookshotBottom;

    [SerializeField] private float hookSpinSpeed = 0.25f;
    [SerializeField] private float hookThrowSpeed = 20f;
    [SerializeField] private float hookThrowDistance = 2f;
    [SerializeField] private LayerMask hookableLayer;

    private Camera mainCamera;

    private Transform hookshotTransform;

    private Routine spinHookRoutine;
    private Routine throwHookRoutine;

    private enum HookStateEnum
    {
        IDLE,
        SPINNING,
        THROWING
    }

    private HookStateEnum hookState;

    // Start is called before the first frame update
    void Start()
    {
        hookshotTransform = hookshotSprite.transform;

        mainCamera = Camera.main;


        hookState = HookStateEnum.IDLE;
        hookshotTransform.localScale = Vector3.zero;
    }

    public void TryStartHookshot()
    {
        if (hookState != HookStateEnum.IDLE)
            return;

        hookState = HookStateEnum.SPINNING;

        //Start hookshot spin
        spinHookRoutine.Stop();
        spinHookRoutine = Routine.Start(this, SpinHookRoutine());
    }

    private void TryThrowHook()
    {
        if (hookState != HookStateEnum.SPINNING)
            return;

        hookState = HookStateEnum.THROWING;

        //Throw hookshot towards mouse
        spinHookRoutine.Stop();
        hookshotTransform.localScale = Vector3.one;

        Vector2 toMouse = ((Vector2)mainCamera.ScreenToWorldPoint(InputHandler.Instance.MousePos) - (Vector2)playerCenter.position).normalized;

        hookshotTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg);

        RaycastHit2D rayHit = Physics2D.Raycast(playerCenter.position, toMouse, hookThrowDistance, hookableLayer);
        if (rayHit.collider != null)
        {
            throwHookRoutine.Stop();
            throwHookRoutine = Routine.Start(this, ThrowHookRoutine(rayHit.point, true));
        }
        else
        {
            throwHookRoutine.Stop();
            throwHookRoutine = Routine.Start(this, ThrowHookRoutine((Vector2)playerCenter.position + toMouse * hookThrowDistance, true));
        }
    }

    // Update is called once per frame
    void Update()
    {
        lineRend.SetPosition(0, playerCenter.position);
        lineRend.SetPosition(1, hookshotBottom.position);
    }

    private IEnumerator SpinHookRoutine()
    {
        yield return Tween.Float(0, 1, (t) =>
        {
            hookshotTransform.localScale = Vector3.one * t;
            hookshotTransform.localEulerAngles = new Vector3(0, 0, -90f + Mathf.Lerp(0, 360 + 90, t));
        }, hookSpinSpeed);

        TryThrowHook();
    }

    private IEnumerator ThrowHookRoutine(Vector2 _target, bool _hitTarget)
    {
        Vector2 startPos = playerCenter.position;
        float distance = (_target - startPos).magnitude;

        yield return Tween.Float(0, 1, (t) =>
        {
            hookshotTransform.position = Vector2.Lerp(startPos, _target, t);
        }, distance / hookThrowSpeed);

        if (_hitTarget)
        {
            //Reel player towards hook
        }

        yield return Tween.Float(0, 1, (t) =>
        {
            hookshotTransform.position = Vector2.Lerp(_target, playerCenter.position, t);
            hookshotTransform.localScale = Vector3.one * (1 - t);
        }, distance / hookThrowSpeed);

        hookState = HookStateEnum.IDLE;
    }
}
