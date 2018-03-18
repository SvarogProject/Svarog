using UnityEngine;

public enum ClickCount {
    FirstTime,
    SecondTime,
    ZeroTime
}

public delegate void OnFirstDown();

public delegate void OnFirstUp();

public delegate void OnSeconed();

public class DoubleClick {
    private ClickCount _clickCount = ClickCount.ZeroTime;
    private float _timer;
    private float _waitTime = 0.5f;

    public DoubleClick() { }

    public DoubleClick(float waitTime) {
        _waitTime = waitTime;
    }

    public void HandleDoubleClick(KeyCode keyCode, OnSeconed onSeconed) {
        HandleDoubleClick(keyCode, null, null, onSeconed);
    }

    public void HandleDoubleClick(string buttonName, OnSeconed onSeconed) {
        HandleDoubleClick(buttonName, null, null, onSeconed);
    }

    public void HandleDoubleClick(KeyCode keyCode, OnFirstDown onFirstDown, OnFirstUp onFirstUp, OnSeconed onSeconed) {
        _timer -= Time.deltaTime;

        if (Input.GetKeyDown(keyCode) && _clickCount == ClickCount.ZeroTime) {
            _timer = _waitTime;
            _clickCount = ClickCount.FirstTime;

            if (onFirstDown != null) {
                onFirstDown();
            }
        }

        if (Input.GetKeyUp(keyCode) && _clickCount == ClickCount.FirstTime) {
            _clickCount = ClickCount.SecondTime;

            if (onFirstUp != null) {
                onFirstUp();
            }
        }

        if (_timer < 0) {
            _clickCount = ClickCount.ZeroTime;
        }

        if (Input.GetKey(keyCode) && _clickCount == ClickCount.SecondTime && _timer > 0f) {
            onSeconed();
            _clickCount = ClickCount.ZeroTime;
        }
    }

    public void HandleDoubleClick(string buttonName, OnFirstDown onFirstDown, OnFirstUp onFirstUp,
        OnSeconed onSeconed) {
        _timer -= Time.deltaTime;

        if (Input.GetButtonDown(buttonName) && _clickCount == ClickCount.ZeroTime) {
            _timer = _waitTime;
            _clickCount = ClickCount.FirstTime;

            if (onFirstDown != null) {
                onFirstDown();
            }
        }

        if (Input.GetButtonUp(buttonName) && _clickCount == ClickCount.FirstTime) {
            _clickCount = ClickCount.SecondTime;

            if (onFirstUp != null) {
                onFirstUp();
            }
        }

        if (_timer < 0) {
            _clickCount = ClickCount.ZeroTime;
        }

        if (Input.GetButton(buttonName) && _clickCount == ClickCount.SecondTime && _timer > 0f) {
            onSeconed();
            _clickCount = ClickCount.ZeroTime;
        }
    }

    public void Reset() {
        _clickCount = ClickCount.ZeroTime;
    }
}