using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static PlayerInputAcition Player { get; private set; }
    public static GameInput Instance { get; private set; }
    private bool toggleModMenuPressed;
    private bool dashPressed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Player = new PlayerInputAcition();
        Player.Player.Enable();
        Player.Player.Dash.performed += _ => dashPressed = true;
        Player.Player.ModMeneger.performed += _ => toggleModMenuPressed = true;
    }

    private void OnDestroy()
    {
        if (Player != null)
        {
            Player.Player.Dash.performed -= _ => dashPressed = true;
            Player.Player.ModMeneger.performed -= _ => toggleModMenuPressed = true;
        }  
    }

    public bool IsAiming() => Player.Player.Aim.ReadValue<float>() > 0.1f;
    public Vector2 GetMousePosition() => Mouse.current.position.ReadValue();
    public Vector2 GetMovementVector() => Player.Player.Move.ReadValue<Vector2>();
    public bool IsRunning() => Player.Player.Run.ReadValue<float>() > 0.1f;

    /// <summary>
    /// —брасываем флаг после чтени€, чтобы реагировать один раз на нажатие.
    /// </summary>
    public bool IsDashPressed()
    {
        if (dashPressed)
        {
            dashPressed = false;
            return true;
        }
        return false;
    }
    public bool IsModMenuTogglePressed()
    {
        if (toggleModMenuPressed)
        {
            toggleModMenuPressed = false;
            return true;
        }
        return false;
    }
}
