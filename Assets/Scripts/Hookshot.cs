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

    [SerializeField] private float dashSpeed = 20f;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private JumpHandler jumpHandler;
    [SerializeField] private Rigidbody2D playerRB;

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

    public void StartHookshot()
    {
        hookState = HookStateEnum.SPINNING;

        movementHandler.ForceStop();
        jumpHandler.ForceLanding();
        jumpHandler.DisableGravity();
        playerRB.velocity = Vector2.zero;

        hookshotTransform.localPosition = Vector3.zero;

        ThrowHook();
    }

    private void ThrowHook()
    {
        hookState = HookStateEnum.THROWING;

        //Throw hookshot towards mouse
        spinHookRoutine.Stop();
        hookshotTransform.localScale = Vector3.one;

        Vector2 toMouse = ((Vector2)mainCamera.ScreenToWorldPoint(InputHandler.Instance.MousePos) - (Vector2)playerCenter.position).normalized;
        hookshotTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg);

        RaycastHit2D rayHit = Physics2D.Raycast(playerCenter.position, toMouse, hookThrowDistance, hookableLayer);
        if (rayHit)
        {
            throwHookRoutine.Stop();
            throwHookRoutine = Routine.Start(this, ThrowHookRoutine(rayHit.point, true));
        }
        else
        {
            throwHookRoutine.Stop();
            throwHookRoutine = Routine.Start(this, ThrowHookRoutine((Vector2)playerCenter.position + toMouse * hookThrowDistance, false));
        }
    }

    // Update is called once per frame
    void Update()
    {
        lineRend.SetPosition(0, playerCenter.position);
        lineRend.SetPosition(1, hookshotBottom.position);
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

            Vector2 toTarget = (_target - (Vector2)playerCenter.position);
            playerRB.velocity = toTarget.normalized * dashSpeed;
            movementHandler.ForceDir(toTarget.x > 0 ? 1 : -1);
            float duration = toTarget.magnitude / dashSpeed;

            yield return Tween.Float(0, 1, (t) =>
            {
                hookshotTransform.localScale = Vector3.one * (1 - t);
                hookshotTransform.position = _target;
            }, duration);

            //yield return new WaitForSeconds(0.1f);

            //Dash is done
            jumpHandler.ResetGravity();

        }
        else
        {
            //Did not hit anything with hookshot
            jumpHandler.ResetGravity();

            //Reel in hook
            yield return Tween.Float(0, 1, (t) =>
            {
                hookshotTransform.position = Vector2.Lerp(_target, playerCenter.position, t);
                hookshotTransform.localScale = Vector3.one * (1 - t);
            }, distance / hookThrowSpeed);
        }

        hookshotTransform.localPosition = Vector3.zero;
        playerController.EndGrapple();
    }
}
