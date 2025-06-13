using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool ShootPressed { get; private set; }
    public bool TiePressed { get; private set; }
    public bool TieReleased { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance != null && GameManager.Instance.isGamePaused)
        { return; }
        

        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        JumpPressed = Input.GetKeyDown(KeyCode.Space);
        DashPressed = Input.GetKeyDown(KeyCode.LeftControl);

        ShootPressed = Input.GetMouseButtonDown(0);
        TiePressed = Input.GetMouseButtonDown(1);
        TieReleased = Input.GetMouseButtonUp(1);

    }
}
