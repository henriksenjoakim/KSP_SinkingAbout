using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    public class ModuleWarheadSwitcher : PartModule
    {
        private ConfigNode[] loadedWarheadArray;
        private bool isRunning = false;
        private float defaultBlastRadius;
        private float defaultBlastPower;
        private float defaultBlastHeat;
        private string defaultShortName;

        [KSPField(isPersistant = true)]
        private int selectedWarheadID = 0;

        [KSPField(isPersistant = true)]
        private int totalLoadedWarheads = 0;

        [KSPEvent(active = true, guiActive = false, guiActiveEditor = true, externalToEVAOnly = false, guiActiveUnfocused = false, guiName = "Next Warhead")]
        private void nextWarhead()
        {
            if (isRunning)
            {
                selectedWarheadID += 1;
                if (selectedWarheadID > totalLoadedWarheads)
                {
                    loadWarhead(0);
                    selectedWarheadID = 0;
                }
                else
                {
                    loadWarhead(selectedWarheadID);
                }
            }
            else
            {
                ScreenMessages.PostScreenMessage("Something has gone wrong with the warhead switcher", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "Warhead")]
        private String selectedWarheadDisplay;

        private void loadWarhead(int warheadNumber)
        {
            if (isRunning)
            {               
                if (warheadNumber == 0)
                {
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        pp.blastPower = defaultBlastPower;
                        pp.blastRadius = defaultBlastRadius;
                        pp.blastHeat = defaultBlastHeat;
                        //pp.shortName = defaultShortName;
                        selectedWarheadDisplay = "Default";
                       
                    }
                }
                else
                {
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        if (loadedWarheadArray[(warheadNumber - 1)].HasValue("blastRadius"))
                        {
                            pp.blastRadius = Convert.ToSingle(loadedWarheadArray[(warheadNumber - 1)].GetValue("blastRadius"));
                        }
                        if (loadedWarheadArray[(warheadNumber - 1)].HasValue("blastPower"))
                        {
                            pp.blastPower = Convert.ToSingle(loadedWarheadArray[(warheadNumber - 1)].GetValue("blastPower"));
                        }
                        if (loadedWarheadArray[(warheadNumber - 1)].HasValue("blastHeat"))
                        {
                            pp.blastHeat = Convert.ToSingle(loadedWarheadArray[(warheadNumber - 1)].GetValue("blastHeat"));
                        }
                        //pp.shortName = defaultShortName + " " + loadedWarheadArray[(warheadNumber - 1)].GetValue("name");
                        selectedWarheadDisplay = loadedWarheadArray[(warheadNumber - 1)].GetValue("name");
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
                                    if (ppp.Modules.Contains("ModuleWarheadSwitcher"))
                                    {
                                        if (pp.selectedWeapon.GetPart() == ppp)
                                        {
                                            var pppp = ppp.Modules.OfType<ModuleWarheadSwitcher>().Single();
                                            pppp = ppp.FindModulesImplementing<ModuleWarheadSwitcher>().First();
                                            ScreenMessages.PostScreenMessage("Warhead: " + pppp.selectedWarheadDisplay, 5.0f, ScreenMessageStyle.UPPER_CENTER);
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
                    if (cn.config.HasNode("WARHEAD"))
                    {
                        if (this.part.Modules.Contains("MissileLauncher"))
                        {
                            var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                            pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                            defaultBlastPower = pp.blastPower;
                            defaultBlastRadius = pp.blastRadius;
                            defaultBlastHeat = pp.blastHeat;
                            if (pp.shortName != null)
                            {
                                defaultShortName = pp.shortName;
                            }
                            loadedWarheadArray = cn.config.GetNodes("WARHEAD");
                            if (loadedWarheadArray.Length != 0)
                            {
                                totalLoadedWarheads = loadedWarheadArray.Length;
                                isRunning = true;
                                if (selectedWarheadID > totalLoadedWarheads)
                                {
                                    selectedWarheadID = 0;
                                    loadWarhead(0);
                                }
                                else
                                {
                                    loadWarhead(selectedWarheadID);
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

            /*
            if (this.part.partInfo != null && this.part.partInfo.partConfig != null)
            {


                
                if (this.part.partInfo.partConfig.HasNode("WARHEAD"))
                {                   
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        defaultBlastPower = pp.blastPower;
                        defaultBlastRadius = pp.blastRadius;
                        defaultBlastHeat = pp.blastHeat;
                        if (pp.shortName != null)
                        {
                            defaultShortName = pp.shortName;
                        }
                        loadedWarheadArray = this.part.partInfo.partConfig.GetNodes("WARHEAD");
                        if (loadedWarheadArray.Length != 0)
                        {
                            totalLoadedWarheads = loadedWarheadArray.Length;
                            isRunning = true;
                            if (selectedWarheadID > totalLoadedWarheads)
                            {
                                selectedWarheadID = 0;
                                loadWarhead(0);
                            }
                            else
                            {
                                loadWarhead(selectedWarheadID);
                            }
                        }
                    }
                }
            }*/

        }

        public override void OnStart(PartModule.StartState state)
        {
            setUp();
            base.OnStart(state);
        }
    }
}
