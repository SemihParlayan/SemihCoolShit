using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeedrunManager : MonoBehaviour 
{
    //public variables

    //private variables
    private bool isRunning = false;
    [SerializeField] private float timer = 0f;
    private List<Speedrun_Checkpoint> allCheckpoints = new List<Speedrun_Checkpoint>();
    private CanvasManager canvas;
    private Player player;

    //unity methods
    private void Awake()
    {
        ComponentManager<SpeedrunManager>.Value = this;
        player = ReInput.players.GetPlayer(0);
    }
    private void Start()
    {
        canvas = ComponentManager<CanvasManager>.Value;
    }
    private void OnEnable()
    {
        StartLevel();
    }
    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;

            int min = Mathf.FloorToInt(timer / 60f);
            int sec = Mathf.FloorToInt(timer - (min * 60f));
            int msec = (int)((timer - Mathf.FloorToInt(timer)) * 100f);

            string text = string.Empty;
            if (min < 10)
                text += "0";
            text += min;
            text += ":";

            if (sec < 10)
                text += "0";
            text += sec;
            text += ":";

            if (msec < 10)
                text += "0";
            text += msec;

            canvas.speedrunTimer.text = text;
        }

        if (player.GetButtonDown("Restart"))
        {
            ResetLevel();
        }
    }

    //public methods
    public void OnCheckpointReached(Speedrun_Checkpoint checkpoint)
    {
        if (!checkpoint.IsActivated)
        {
            checkpoint.ActivateCheckpoint();
        }

        foreach (Speedrun_Checkpoint point in allCheckpoints)
        {
            if (!point.IsActivated)
                return;
        }

        isRunning = false;
    }

    //private methods
    private void StartLevel()
    {
        isRunning = true;
        timer = 0f;

        allCheckpoints = FindObjectsOfType<Speedrun_Checkpoint>().ToList();
        foreach (Speedrun_Checkpoint checkpoint in allCheckpoints)
        {
            checkpoint.ResetCheckpoint();
        }
    }
    private void ResetLevel()
    {
        timer = 0f;
        isRunning = true;

        foreach (Speedrun_Checkpoint checkpoint in allCheckpoints)
        {
            checkpoint.ResetCheckpoint();
        }
        GameObject.FindObjectOfType<Entity_Player>().Kill();
    }
}
