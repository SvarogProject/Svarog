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
    private float _waitTime = 1f;

    public DoubleClick() { }

    public DoubleClick(float waitTime) {
        _waitTime = waitTime;
    }
    
    public void HandleDoubleBool(bool name, OnSeconed onSeconed) {
        HandleDoubleBool(name, null, null, onSeconed);
    }
    
    public void HandleDoubleBool(bool name, OnFirstDown onFirstDown, OnFirstUp onFirstUp, OnSeconed onSeconed) {
        _timer -= Time.deltaTime;

        if (name && _clickCount == ClickCount.ZeroTime) {
            _timer = _waitTime;
            _clickCount = ClickCount.FirstTime;

            if (onFirstDown != null) {
                onFirstDown();
            }
        }

        if (!name && _clickCount == ClickCount.FirstTime) {
            _clickCount = ClickCount.SecondTime;

            if (onFirstUp != null) {
                onFirstUp();
            }
        }

        if (_timer < 0) {
            _clickCount = ClickCount.ZeroTime;
        }

        if (name && _clickCount == ClickCount.SecondTime && _timer > 0f) {
            onSeconed();
            _clickCount = ClickCount.ZeroTime;
        }
    }

    public void HandleDoubleClickWithAxis(string axisName, bool isPositiveValue, OnSeconed onSeconed) {
        HandleDoubleClickWithAxis(axisName, isPositiveValue, null, null, onSeconed);
    }
    
    public void HandleDoubleClickWithAxis(string axisName, bool isPositiveValue, OnFirstDown onFirstDown,
        OnFirstUp onFirstUp, OnSeconed onSeconed) {
        _timer -= Time.deltaTime;

        float axisValue = Input.GetAxis(axisName);

        if (_clickCount == ClickCount.ZeroTime &&
            (axisValue > 0 && isPositiveValue || axisValue < 0 && !isPositiveValue)) {
            if (_clickCount == ClickCount.ZeroTime) {
                _timer = _waitTime;
                _clickCount = ClickCount.FirstTime;

                if (onFirstDown != null) {
                    onFirstDown();
                }
            }
        }

        if (_clickCount == ClickCount.FirstTime &&
            (axisValue <= 0 && isPositiveValue || axisValue >= 0 && !isPositiveValue)) {
            _clickCount = ClickCount.SecondTime;

            if (onFirstUp != null) {
                onFirstUp();
            }
        }


        if (_timer < 0) {
            _clickCount = ClickCount.ZeroTime;
        }

        if (_clickCount == ClickCount.SecondTime && _timer > 0f &&
            (axisValue > 0 && isPositiveValue || axisValue < 0 && !isPositiveValue)) {
            onSeconed();
            _clickCount = ClickCount.ZeroTime;
        }
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

        if (Input.anyKeyDown) {
            if (Input.GetButtonDown(buttonName)) {
                if (_clickCount == ClickCount.ZeroTime) {
                    _timer = _waitTime;
                    _clickCount = ClickCount.FirstTime;

                    if (onFirstDown != null) {
                        onFirstDown();
                    }
                }
            }
            /*
            else {
                //TODO 这里有问题，是玩家x的其他键盘，别的玩家按下其他键没问题
                _clickCount = ClickCount.ZeroTime; // 按下了别的键应重置 
            }
            */
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