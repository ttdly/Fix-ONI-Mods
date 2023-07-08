﻿using TUNING;
using UnityEngine;
using NightLib.AddBuilding;
using System.Collections.Generic;

namespace MoreTemperatureSensors
{
    public class SolidConduitTemperatureSensorConfig : IBuildingConfig
    {
        public const string ID = "Nightinggale.SolidConduitTemperatureSensor";

        public static void Setup()
        {
            LocString NAME = "Conveyor Rail Temperature Sensor";
            LocString DESC = "";
            LocString EFFECT = SolidConduitThresholdSensor.MakeEffect("temperature");

            AddBuilding.AddStrings(ID, NAME, DESC, EFFECT);

            AddBuilding.AddBuildingToPlanScreen("Conveyance", ID);
            AddBuilding.IntoTechTree("SolidTransport", ID);
        }

        public static readonly LogicPorts.Port OUTPUT_PORT = SolidConduitThresholdSensor.MakePort("Temperature");

        public override BuildingDef CreateBuildingDef()
        {
            int width = 1;
            int height = 1;
            string anim = "gas_temperature_sensor_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = true;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;

            buildingDef.LogicOutputPorts = buildingDef.LogicOutputPorts = new List<LogicPorts.Port>() {
                OUTPUT_PORT
            };

            SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            //GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            //GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            //GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);

            SolidConduitTemperatureSensor sensor = go.AddOrGet<SolidConduitTemperatureSensor>();
            sensor.manuallyControlled = false;
            sensor.Threshold = 280f;
        }
    }
}
