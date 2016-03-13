using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    public class ModuleGuidanceSwitcher : PartModule
    {
        ConfigNode[] loadedGuidanceArray;
        private bool isRunning = false;
        public bool defaultGuidanceActive;
        public string defaultMissileType;
        public string defaultHomingType;
        public string defaultTargetingType;
        public float defaultActiveRadarRange;
        public float defaultMaxOffBoresight;
        public float defaultLockedSensorFOV;
        public float defaultMinStaticLaunchRange;
        public float defaultMaxStaticLaunchRange;
        public bool defaultRadarLOAL;
        public float defaultHeatThreshold;

        [KSPField(isPersistant = true)] 
        public int selectedGuidanceID = 0;

        [KSPField(isPersistant = true)] 
        public int totalLoadedGuidance = 0;

        [KSPEvent(active = true, guiActive = false, guiActiveEditor = true, externalToEVAOnly = false, guiActiveUnfocused = false, guiName = "Next Guidance Sys")]
        public void nextGuidance()
        {
            if (isRunning)
            {
                selectedGuidanceID += 1;
                if ((selectedGuidanceID) > totalLoadedGuidance)
                {
                    loadGuidance(0);
                    selectedGuidanceID = 0;
                }
                else
                {
                    loadGuidance(selectedGuidanceID);
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("Something has gone wrong, with the guidance switcher", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "Guidance:")]
        public String selectedGuidanceDisplay;

        public void loadGuidance(int guidanceNumber)
        {
            if (isRunning)
            {
                if (guidanceNumber == 0)
                {
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        if (defaultGuidanceActive != null)
                        {
                            pp.guidanceActive = defaultGuidanceActive;
                        }
                        if (defaultMissileType != null)
                        {
                            pp.missileType = defaultMissileType;
                        }
                        if (defaultTargetingType != null)
                        {
                            pp.targetingType = defaultTargetingType;
                        }
                        if (defaultHomingType != null)
                        {
                            pp.homingType = defaultHomingType;
                        }
                        if (defaultActiveRadarRange != null)
                        {
                            pp.activeRadarRange = defaultActiveRadarRange;
                        }
                        if (defaultMaxOffBoresight != null)
                        {
                            pp.maxOffBoresight = defaultMaxOffBoresight;
                        }
                        if (defaultLockedSensorFOV != null)
                        {
                            pp.lockedSensorFOV = defaultLockedSensorFOV;
                        }
                        if (defaultMinStaticLaunchRange != null)
                        {
                            pp.minStaticLaunchRange = defaultMinStaticLaunchRange;
                        }
                        if (defaultMaxStaticLaunchRange != null)
                        {
                            pp.maxStaticLaunchRange = defaultMaxStaticLaunchRange;
                        }
                        if (defaultRadarLOAL != null)
                        {
                            pp.radarLOAL = defaultRadarLOAL;
                        }
                        if (defaultHeatThreshold != null)
                        {
                            pp.heatThreshold = defaultHeatThreshold;
                        }
                        selectedGuidanceDisplay = "Default (" + pp.targetingType + ")";
                    }
                }
                else
                {
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("guidanceActive"))
                        {
                            pp.guidanceActive = Convert.ToBoolean(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("guidanceActive"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("missileType"))
                        {
                            pp.missileType = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("missileType");
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("homingType"))
                        {
                            string homingType = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("homingType");
                            if (homingType == "aam")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.AAMLead;
                            }
                            if (homingType == "aamlead")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.AAMLead;
                            }
                            if (homingType == "aamPure")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.AAMPure;
                            }
                            if (homingType == "AGM")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.AGM;
                            }
                            if (homingType == "agmballistic")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.AGMBallistic;
                            }
                            if (homingType == "cruise")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.Cruise;
                            }
                            if (homingType == "sts")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.STS;
                            }
                            if (homingType == "rcs")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.RCS;
                            }
                            if (homingType == "beamriding")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.BeamRiding;
                            }
                            if (homingType == "none")
                            {
                                pp.guidanceMode = MissileLauncher.GuidanceModes.None;
                            }
                            pp.homingType = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("homingType");

                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("targetingType"))
                        {
                            string targetingType = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("targetingType");
                            if (targetingType == "radar")
                            {
                                pp.targetingMode = MissileLauncher.TargetingModes.Radar;
                            }
                            if (targetingType == "heat")
                            {
                                pp.targetingMode = MissileLauncher.TargetingModes.Heat;
                            }
                            if (targetingType == "laser")
                            {
                                pp.targetingMode = MissileLauncher.TargetingModes.Laser;
                            }
                            if (targetingType == "gps")
                            {
                                pp.targetingMode = MissileLauncher.TargetingModes.GPS;
                            }
                            if (targetingType == "antirad")
                            {
                                pp.targetingMode = MissileLauncher.TargetingModes.AntiRad;
                            }
                            if (targetingType == "none")
                            {
                                pp.targetingMode = MissileLauncher.TargetingModes.None;
                            }
                            pp.targetingType = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("targetingType");
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("activeRadarRange"))
                        {
                            pp.activeRadarRange = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("activeRadarRange"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("maxOffBoresight"))
                        {
                            pp.maxOffBoresight = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("maxOffBoresight"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("lockedSensorFOV"))
                        {
                            pp.lockedSensorFOV = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("lockedSensorFOV"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("minStaticLaunchRange"))
                        {
                            pp.minStaticLaunchRange = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("minStaticLaunchRange"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("maxStaticLaunchRange"))
                        {
                            pp.maxStaticLaunchRange = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("maxStaticLaunchRange"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("radarLOAL"))
                        {
                            pp.radarLOAL = Convert.ToBoolean(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("radarLOAL"));
                        }
                        if (loadedGuidanceArray[(guidanceNumber - 1)].HasValue("heatThreshold"))
                        {
                            pp.heatThreshold = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("heatThreshold"));
                        }
                        pp.GetSubLabel();
                        selectedGuidanceDisplay = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("name") + " (" + pp.targetingType + ")";
                    }
                }
            }
        }


        private void setUp()
        {
            if (this.part.partInfo.partConfig.HasNode("GUIDANCE"))
            {
                if (this.part.Modules.Contains("MissileLauncher"))
                {
                    var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                    pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                    if (pp.guidanceActive != null)
                    {
                        defaultGuidanceActive = pp.guidanceActive;
                    }
                    if (pp.missileType != null)
                    {
                        defaultMissileType = pp.missileType;
                    }
                    if (pp.targetingType != null)
                    {
                        defaultTargetingType = pp.targetingType;
                    }
                    if (pp.homingType != null)
                    {
                        defaultHomingType = pp.homingType;
                    }
                    if (pp.activeRadarRange != null)
                    {
                        defaultActiveRadarRange = pp.activeRadarRange;
                    }
                    if (pp.maxOffBoresight != null)
                    {
                        defaultMaxOffBoresight = pp.maxOffBoresight;
                    }
                    if (pp.lockedSensorFOV != null)
                    {
                        defaultLockedSensorFOV = pp.lockedSensorFOV;
                    }
                    if (pp.minStaticLaunchRange != null)
                    {
                        defaultMinStaticLaunchRange = pp.minStaticLaunchRange;
                    }
                    if (pp.maxStaticLaunchRange != null)
                    {
                        defaultMaxStaticLaunchRange = pp.maxStaticLaunchRange;
                    }
                    if (pp.radarLOAL != null)
                    {
                        defaultRadarLOAL = pp.radarLOAL;
                    }
                    if (pp.heatThreshold != null)
                    {
                        defaultHeatThreshold = pp.heatThreshold;
                    }
                    loadedGuidanceArray = this.part.partInfo.partConfig.GetNodes("GUIDANCE");
                    if (loadedGuidanceArray.Length > 0)
                    {
                        totalLoadedGuidance = loadedGuidanceArray.Length;
                        isRunning = true;
                        if (selectedGuidanceID > totalLoadedGuidance)
                        {
                            selectedGuidanceID = 0;
                            loadGuidance(0);
                        }
                        else
                        {
                            loadGuidance(selectedGuidanceID);
                        }
                    }
                }
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            setUp();
            base.OnStart(state);
        }
    }
}
