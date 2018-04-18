using System.Collections.Generic;
using FlatBuffers;
using SHU.FlatBuffers.Input;
using SHU.Sim.InputEvents;
using UnityEngine;
using Event = SHU.Sim.Event;

namespace SHU.InputSources
{
  public class UnityInputSource : MonoBehaviour, IInputSource
  {
    public uint ObjectId;
    public KeyCode KeyUp;
    public KeyCode KeyDown;
    public KeyCode KeyLeft;
    public KeyCode KeyRight;

    private bool _isKeyUpPressed;
    private bool _isKeyDownPressed;
    private bool _isKeyLeftPressed;
    private bool _isKeyRightPressed;

    // Update is called once per frame
    void Update ()
    {
      _isKeyUpPressed = Input.GetKey(KeyUp);
      _isKeyDownPressed = Input.GetKey(KeyDown);
      _isKeyLeftPressed = Input.GetKey(KeyLeft);
      _isKeyRightPressed = Input.GetKey(KeyRight);
    }

    public List<Event> GetInputs(uint tick)
    {
      var fbb = new FlatBufferBuilder(1);
      var offset = PlayerMovement.CreatePlayerMovement(
        fbb,
        ObjectId,
        _isKeyUpPressed ? KeyboardAction.KeyDown : KeyboardAction.KeyUp,
        _isKeyDownPressed ? KeyboardAction.KeyDown : KeyboardAction.KeyUp,
        _isKeyLeftPressed ? KeyboardAction.KeyDown : KeyboardAction.KeyUp,
        _isKeyRightPressed ? KeyboardAction.KeyDown : KeyboardAction.KeyUp
      );
      PlayerMovement.FinishPlayerMovementBuffer(fbb, offset);

      return new List<Event>
      {
        new Move(PlayerMovement.GetRootAsPlayerMovement(fbb.DataBuffer))
      };
    }
  }
}
