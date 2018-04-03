using System;
using System.Runtime.InteropServices;
using SHU.InputAggregators;
using SHU.InputSources;
using SHU.ObjectFactories;
using SHU.Sim.Events;
using SHU.Sim.Objects;
using UnityEngine;

namespace SHU.SimManager
{
  public class TestSimManager : MonoBehaviour
  {
    public int fps = 30;
    public UnityInputSource InputSource;
    public UnityObjectFactory ObjectFactory;

    private IInputAggregator _inputAggregator;
    private Sim.Simulation _sim;

    private float _targetInterval;
    private float _remainingDelta;

    private void Start()
    {
      // instantiate input manager, and register unity input
      var inputManager = new InputManager();
      inputManager.RegisterInputSource(InputSource);

      // instantiate local input aggregator and register the input manager
      var localInputAggregator = new LocalInputAggregator();
      localInputAggregator.InputManager = inputManager;
      _inputAggregator = localInputAggregator;

      _sim = new Sim.Simulation();

      _sim.AddObjectFactory(ObjectFactory);

      _sim.ScheduleEvent(new CreateObject
      {
        Obj = new Player()
      });
      _sim.UpdateTick();

      ReCaluclateFrameInterval();
      _remainingDelta = _targetInterval;
    }

    private void Update()
    {
      ReCaluclateFrameInterval();

      _remainingDelta -= Time.deltaTime;

      if (!(_remainingDelta < 0.0f)) return;

      _remainingDelta += _targetInterval;

      var inputs = _inputAggregator.GetInputs(_sim.GetCurrentTick());

      var inputCount = inputs.Count;

      for (var i = 0; i < inputCount; ++i)
      {
        _sim.ScheduleEvent(inputs[i]);
      }
      _sim.UpdateTick();
    }

    private void ReCaluclateFrameInterval()
    {
      _targetInterval = 1.0f / fps;
    }
  }
}