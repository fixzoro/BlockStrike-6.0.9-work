using System;
using UnityEngine;

//фикс камеры 6.0.9

public class InputTouchLook : MonoBehaviour
{
    private Rect touchZone;

    private bool move;

    private int id = -1;

    private float dpi;

    private Vector2 value;

    private Touch touch;

    private static bool fixTouch;

    private Vector2 fixPos;

    private void Start()
    {
        touchZone = new Rect((float)Screen.width / 2f, 0f, (float)Screen.width, (float)(Screen.height));
        this.dpi = Screen.dpi / 100f;
        if (this.dpi == 0f)
        {
            this.dpi = 1.6f;
        }
        EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
        this.OnSettings();
        InputTouchLook.fixTouch = GameConsole.Load<bool>("fix_touch_look", false);
    }

    private void OnDisable()
    {
        this.UpdateValue(Vector2.zero);
        this.id = -1;
        this.move = false;
    }

    private void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            this.touch = Input.GetTouch(i);
            if (this.touch.phase == TouchPhase.Began && this.touchZone.Contains(new Vector3(this.touch.position.x, (float)Screen.height - this.touch.position.y, 0f)))
            {
                this.move = true;
                this.id = this.touch.fingerId;
                if (InputTouchLook.fixTouch)
                {
                    this.fixPos = this.touch.position;
                }
            }
            if ((this.touch.phase == TouchPhase.Moved || this.touch.phase == TouchPhase.Stationary) && this.id == this.touch.fingerId && this.move)
            {
                if (InputTouchLook.fixTouch)
                {
                    this.UpdateValue((this.touch.position - this.fixPos) / this.dpi);
                    this.fixPos = this.touch.position;
                }
                else
                {
                    this.UpdateValue(this.touch.deltaPosition / (this.dpi + 5f));
                }
            }
            if ((this.touch.phase == TouchPhase.Canceled || this.touch.phase == TouchPhase.Ended) && this.id == this.touch.fingerId && this.move)
            {
                this.id = -1;
                this.move = false;
                this.UpdateValue(Vector2.zero);
                if (InputTouchLook.fixTouch)
                {
                    this.fixPos = this.touch.position;
                }
            }
        }
    }

    private void UpdateValue(Vector2 v)
    {
        this.value = v;
        InputManager.SetAxis("Mouse X", this.value.x);
        InputManager.SetAxis("Mouse Y", this.value.y);
    }

    private void OnSettings()
    {
        //this.touchZone = nPlayerPrefs.GetRect("Look_Rect", new Rect((float)(Screen.width / 2), 0f, (float)(Screen.width / 2), (float)Screen.height));
    }
}
