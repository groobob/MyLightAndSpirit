using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event EventHandler OnPause;
    public event EventHandler OnUnpause;

    public enum State
    {
        playing,
        paused
    }

    public State currentState;

    private void Awake()
    {
        Instance = this;
        SoundManager.Instance.PlayMusic(0, AudioSourceType.MainMusic);
    }

    private void Start()
    {
        OnPause += GameManager_OnPause;
        OnUnpause += GameManager_OnUnpause;
    }

    private void GameManager_OnUnpause(object sender, EventArgs e)
    {
        currentState = State.playing;
    }

    private void GameManager_OnPause(object sender, EventArgs e)
    {
        currentState = State.paused;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if(currentState == State.playing) OnPause?.Invoke(this, EventArgs.Empty);
            else if(currentState == State.paused) OnUnpause?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Pause()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    public void Unpause()
    {
        OnUnpause?.Invoke(this, EventArgs.Empty);
    }
}
