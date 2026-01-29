using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Managers")]
    public BoardManager board_Manager;
    public UI_Manager UI_Manager;
    public CameraManager cameraManager;
    public DeadlockManager deadlockManager;

    public enum GameState {
    Prepare,
    Playing,
    Deadlock,
    Waiting,
    }
    public GameState state;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
