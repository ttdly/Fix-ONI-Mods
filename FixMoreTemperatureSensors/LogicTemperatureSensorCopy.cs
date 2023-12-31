﻿// Decompiled with JetBrains decompiler
// Type: LogicTemperatureSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2BFC01D3-C011-441F-99ED-A7DA2F70C2DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicTemperatureSensorCopy : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
  private HandleVector<int>.Handle structureTemperature;
  private int simUpdateCounter;
  [Serialize]
  public float thresholdTemperature = 280f;
  [Serialize]
  public bool activateOnWarmerThan;
  [Serialize]
  private bool dirty = true;
  public float minTemp;
  public float maxTemp = 373.15f;
  private const int NumFrameDelay = 8;
  private float[] temperatures = new float[8];
  private float averageTemp;
  private bool wasOn;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicTemperatureSensorCopy> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicTemperatureSensorCopy>((Action<LogicTemperatureSensorCopy, object>) ((component, data) => component.OnCopySettings(data)));

  public float StructureTemperature => GameComps.StructureTemperatures.GetPayload(this.structureTemperature).Temperature;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicTemperatureSensorCopy>(-905833192, LogicTemperatureSensorCopy.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicTemperatureSensorCopy component = ((GameObject) data).GetComponent<LogicTemperatureSensorCopy>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    this.Threshold = component.Threshold;
    this.ActivateAboveThreshold = component.ActivateAboveThreshold;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
    this.OnToggle += new Action<bool>(this.OnSwitchToggled);
    this.UpdateVisualState(true);
    this.UpdateLogicCircuit();
    this.wasOn = this.switchedOn;
  }

  public void Sim200ms(float dt)
  {
    if (this.simUpdateCounter < 8 && !this.dirty)
    {
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      if ((double) Grid.Mass[cell] <= 0.0)
        return;
      this.temperatures[this.simUpdateCounter] = Grid.Temperature[cell];
      ++this.simUpdateCounter;
    }
    else
    {
      this.simUpdateCounter = 0;
      this.dirty = false;
      this.averageTemp = 0.0f;
      for (int index = 0; index < 8; ++index)
        this.averageTemp += this.temperatures[index];
      this.averageTemp /= 8f;
      if (this.activateOnWarmerThan)
      {
        if (((double) this.averageTemp <= (double) this.thresholdTemperature || this.IsSwitchedOn) && ((double) this.averageTemp > (double) this.thresholdTemperature || !this.IsSwitchedOn))
          return;
        this.Toggle();
      }
      else
      {
        if (((double) this.averageTemp < (double) this.thresholdTemperature || !this.IsSwitchedOn) && ((double) this.averageTemp >= (double) this.thresholdTemperature || this.IsSwitchedOn))
          return;
        this.Toggle();
      }
    }
  }

  public float GetTemperature() => this.averageTemp;

  private void OnSwitchToggled(bool toggled_on)
  {
    this.UpdateVisualState();
    this.UpdateLogicCircuit();
  }

    private void UpdateLogicCircuit() {
        LogicPorts lo = this.GetComponent<LogicPorts>();
        lo?.SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);
        //this.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0); 
    }

  private void UpdateVisualState(bool force = false)
  {
    if (!(this.wasOn != this.switchedOn | force))
      return;
    this.wasOn = this.switchedOn;
    KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
    component.Play((HashedString) (this.switchedOn ? "on_pre" : "on_pst"));
    component.Queue((HashedString) (this.switchedOn ? "on" : "off"));
  }

  protected override void UpdateSwitchStatus()
  {
    StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
    this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
  }

  public float Threshold
  {
    get => this.thresholdTemperature;
    set
    {
      this.thresholdTemperature = value;
      this.dirty = true;
    }
  }

  public bool ActivateAboveThreshold
  {
    get => this.activateOnWarmerThan;
    set
    {
      this.activateOnWarmerThan = value;
      this.dirty = true;
    }
  }

  public float CurrentValue => this.GetTemperature();

  public float RangeMin => this.minTemp;

  public float RangeMax => this.maxTemp;

  public float GetRangeMinInputField() => GameUtil.GetConvertedTemperature(this.RangeMin);

  public float GetRangeMaxInputField() => GameUtil.GetConvertedTemperature(this.RangeMax);

  public LocString Title => UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;

  public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;

  public string AboveToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;

  public string BelowToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;

  public string Format(float value, bool units) => GameUtil.GetFormattedTemperature(value, displayUnits: units, roundInDestinationFormat: true);

  public float ProcessedSliderValue(float input) => Mathf.Round(input);

  public float ProcessedInputValue(float input) => GameUtil.GetTemperatureConvertedToKelvin(input);

  public LocString ThresholdValueUnits()
  {
    LocString locString = (LocString) null;
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        locString = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
        break;
      case GameUtil.TemperatureUnit.Fahrenheit:
        locString = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
        break;
      case GameUtil.TemperatureUnit.Kelvin:
        locString = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
        break;
    }
    return locString;
  }

  public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

  public int IncrementScale => 1;

  public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[4]
  {
    new NonLinearSlider.Range(25f, 260f),
    new NonLinearSlider.Range(50f, 400f),
    new NonLinearSlider.Range(12f, 1500f),
    new NonLinearSlider.Range(13f, 10000f)
  };
}
