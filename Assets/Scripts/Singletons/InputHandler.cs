using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    public float Dir
    {
        get;
        private set;
    }
    public Vector2 MousePos
    {
        get;
        private set;
    }
    public ButtonState Move
    {
        get { return buttons[0]; }
    }
    public ButtonState Jump
    {
        get { return buttons[1]; }
    }
    public ButtonState Attack
    {
        get { return buttons[2]; }
    }
    public ButtonState Grapple
    {
        get { return buttons[3]; }
    }
    public ButtonState Summon
    {
        get { return buttons[4]; }
    }
    public ButtonState Interact
    {
        get { return buttons[5]; }
    }
    public ButtonState Menu
    {
        get { return buttons[6]; }
    }

    [SerializeField] private int buttonCount = 1;
    [SerializeField] private short bufferFrames = 5;
    [SerializeField] private bool bufferEnabled = false;
    private short IDSRC = 0;
    private ButtonState[] buttons;
    private Queue<Dictionary<short, short>> inputBuffer = new Queue<Dictionary<short, short>>();
    private Dictionary<short, short> currentFrame;

    public void Start()
    {
        buttons = new ButtonState[buttonCount];
        for (int i = 0; i < buttonCount; i++)
            buttons[i].Init(ref IDSRC, this);

        if (SaveData.CurrSaveData.reboundControls != null)
            GetComponent<PlayerInput>().actions.LoadBindingOverridesFromJson(SaveData.CurrSaveData.reboundControls);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < buttonCount; i++)
            buttons[i].Reset();

        if (bufferEnabled)
        {
            UpdateBuffer();
        }
    }

    public void CTX_Move(InputAction.CallbackContext ctx)
    {
        this.Dir = ctx.ReadValue<float>();
        this.buttons[0].Set(ctx);
    }

    public void CTX_Jump(InputAction.CallbackContext ctx)
    {
        this.buttons[1].Set(ctx);
    }

    public void CTX_Attack(InputAction.CallbackContext ctx)
    {
        this.buttons[2].Set(ctx);
    }

    public void CTX_Grapple(InputAction.CallbackContext ctx)
    {
        this.buttons[3].Set(ctx);
    }

    public void CTX_Summon(InputAction.CallbackContext ctx)
    {
        this.buttons[4].Set(ctx);
    }

    public void CTX_Interact(InputAction.CallbackContext ctx)
    {
        this.buttons[5].Set(ctx);
    }

    public void CTX_Menu(InputAction.CallbackContext ctx)
    {
        this.buttons[6].Set(ctx);
    }

    public void CTX_MousePos(InputAction.CallbackContext ctx)
    {
        this.MousePos = ctx.ReadValue<Vector2>();
    }

    public void FlushBuffer()
    {
        inputBuffer.Clear();
    }

    public void UpdateBuffer()
    {
        if (inputBuffer.Count >= bufferFrames)
            inputBuffer.Dequeue();
        currentFrame = new Dictionary<short, short>();
        inputBuffer.Enqueue(currentFrame);
    }

    public void PrintBuffer()
    {
        string bufferData = $"InputBuffer: count-{inputBuffer.Count}";
        foreach (var frame in inputBuffer)
            if (frame.Count > 0)
                bufferData += $"\n{frame.Count}";
        Debug.Log(bufferData);
    }

    public struct ButtonState
    {
        private short id;
        private static short STATE_PRESSED = 0,
                                STATE_RELEASED = 1;
        private InputHandler handler;
        private bool firstFrame;
        public bool down
        {
            get;
            private set;
        }
        public bool pressed
        {
            get
            {
                if (handler.bufferEnabled && handler.inputBuffer != null)
                {
                    foreach (var frame in handler.inputBuffer)
                    {
                        if (frame.ContainsKey(id) && frame[id] == STATE_PRESSED)
                        {
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return down && firstFrame;
            }
        }

        public bool released
        {
            get
            {
                if (handler.bufferEnabled && handler.inputBuffer != null)
                {
                    foreach (var frame in handler.inputBuffer)
                    {
                        if (frame.ContainsKey(id) && frame[id] == STATE_RELEASED)
                        {
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return !down && firstFrame;
            }
        }

        public void Set(InputAction.CallbackContext ctx)
        {
            down = !ctx.canceled;
            firstFrame = true;

            if (handler.bufferEnabled && handler.currentFrame != null)
            {
                handler.currentFrame.TryAdd(id, down ? STATE_PRESSED : STATE_RELEASED);
            }
        }

        public void Reset()
        {
            firstFrame = false;
        }

        public void Init(ref short IDSRC, InputHandler handler)
        {
            id = IDSRC++;
            this.handler = handler;
        }
    }
}
