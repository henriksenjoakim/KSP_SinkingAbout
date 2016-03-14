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
        private string defaultShortName;
        private bool defaultGuidanceActive;
        private string defaultMissileType;
        private string defaultHomingType;
        private string defaultTargetingType;
        private float defaultActiveRadarRange;
        private float defaultMaxOffBoresight;
        private float defaultLockedSensorFOV;
        private float defaultMinStaticLaunchRange;
        private float defaultMaxStaticLaunchRange;
        private bool defaultRadarLOAL;
        private float defaultHeatThreshold;

        [KSPField(isPersistant = true)]
        private int selectedGuidanceID = 0;

        [KSPField(isPersistant = true)]
        private int totalLoadedGuidance = 0;

        [KSPEvent(active = true, guiActive = false, guiActiveEditor = true, externalToEVAOnly = false, guiActiveUnfocused = false, guiName = "Next Guidance System")]
        private void nextGuidance()
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

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "Guidance")]
        private String selectedGuidanceDisplay;

        private void loadGuidance(int guidanceNumber)
        {
            if (isRunning)
            {
                if (guidanceNumber == 0)
                {
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        pp.guidanceActive = defaultGuidanceActive;
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
                        pp.activeRadarRange = defaultActiveRadarRange;
                        pp.maxOffBoresight = defaultMaxOffBoresight;
                        pp.lockedSensorFOV = defaultLockedSensorFOV;
                        pp.minStaticLaunchRange = defaultMinStaticLaunchRange;
                        pp.maxStaticLaunchRange = defaultMaxStaticLaunchRange;
                        pp.radarLOAL = defaultRadarLOAL;
                        pp.heatThreshold = defaultHeatThreshold;
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
                        //
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("activeRadarRange"))
                        {
                            pp.activeRadarRange = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("activeRadarRange"));
                        }
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("maxOffBoresight"))
                        {
                            pp.maxOffBoresight = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("maxOffBoresight"));
                        }
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("lockedSensorFOV"))
                        {
                            pp.lockedSensorFOV = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("lockedSensorFOV"));
                        }
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("minStaticLaunchRange"))
                        {
                            pp.minStaticLaunchRange = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("minStaticLaunchRange"));
                        }
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("maxStaticLaunchRange"))
                        {
                            pp.maxStaticLaunchRange = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("maxStaticLaunchRange"));
                        }
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("radarLOAL"))
                        {
                            pp.radarLOAL = Convert.ToBoolean(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("radarLOAL"));
                        }
                        if (loadedGuidanceArray != null && loadedGuidanceArray[(guidanceNumber - 1)].HasValue("heatThreshold"))
                        {
                            pp.heatThreshold = Convert.ToSingle(loadedGuidanceArray[(guidanceNumber - 1)].GetValue("heatThreshold"));
                        }
                        //pp.GetSubLabel();
                        selectedGuidanceDisplay = loadedGuidanceArray[(guidanceNumber - 1)].GetValue("name") + " (" + pp.targetingType + ")";
                    }
                }
            }
        }


        private bool hasChanged = true;
        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;
            foreach (Part p in this.part.vessel.parts)
            {
                if (p.Modules.Contains("MissileFire"))
                {
                    var pp = p.Modules.OfType<MissileFire>().Single();
                    pp = p.FindModulesImplementing<MissileFire>().First();
                    if (pp.isArmed)
                    {
                        if (pp.selectedWeapon != null && pp.selectedWeapon.GetPart() == this.part)
                        {
                            if (hasChanged == true)
                            {
                                foreach (Part ppp in this.part.vessel.parts)
                                {
                                    if (ppp.Modules.Contains("ModuleGuidanceSwitcher"))
                                    {
                                        if (pp.selectedWeapon.GetPart() == ppp)
                                        {
                                            var pppp = ppp.Modules.OfType<ModuleGuidanceSwitcher>().Single();
                                            pppp = ppp.FindModulesImplementing<ModuleGuidanceSwitcher>().First();
                                            ScreenMessages.PostScreenMessage("Guidance: " + pppp.selectedGuidanceDisplay, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                            hasChanged = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            hasChanged = true;
                        }
                    }

                }
            }
        }


        private void setUp()
        {
            UrlDir.UrlConfig[] cfg = GameDatabase.Instance.GetConfigs("PART");
            foreach (UrlDir.UrlConfig cn in cfg)
            {
                if (cn.name == this.part.partInfo.name)
                {
                    if (cn.config.HasNode("GUIDANCE"))
                    {
                        if (this.part.Modules.Contains("MissileLauncher"))
                        {
                            var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                            pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                            defaultGuidanceActive = pp.guidanceActive;
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
                            defaultActiveRadarRange = pp.activeRadarRange;
                            defaultMaxOffBoresight = pp.maxOffBoresight;
                            defaultLockedSensorFOV = pp.lockedSensorFOV;
                            defaultMinStaticLaunchRange = pp.minStaticLaunchRange;
                            defaultMaxStaticLaunchRange = pp.maxStaticLaunchRange;
                            defaultRadarLOAL = pp.radarLOAL;
                            defaultHeatThreshold = pp.heatThreshold;
                            loadedGuidanceArray = cn.config.GetNodes("GUIDANCE");
                            if (loadedGuidanceArray.Length != 0)
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
            }
            /*
            if (this.part.partInfo != null && this.part.partInfo.partConfig != null)
            {

            }*/
        }

        public override void OnStart(PartModule.StartState state)
        {
            setUp();
            base.OnStart(state);
        }
    }
}
