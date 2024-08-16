using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]

public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    private GameInput gameInput;
    public event UnityAction<Vector2> MoveEvent = delegate {  }; 
    public event UnityAction ShootEvent = delegate {  };
    public event UnityAction JumpEvent = delegate {  };
    public event UnityAction EmoteEvent = delegate {  };

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) { ShootEvent.Invoke(); }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) { JumpEvent.Invoke(); }
    }

    public void OnEmote(InputAction.CallbackContext context)
    {
        if(context.performed){ EmoteEvent.Invoke();}
    }

    private void OnEnable()
    {
        if (gameInput == null) {
            gameInput = new GameInput();
            gameInput.Gameplay.SetCallbacks(this);
            gameInput.Gameplay.Enable();
        }
    }
}
