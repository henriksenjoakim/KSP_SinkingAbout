using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    public class ModuleClearanceCheckDisabler : PartModule
    {
        private bool thisWeaponSelected = false;

        private void checkForClearance()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;
            if (this.part.children.Count != 0)
            {
                foreach (Part p in this.part.vessel.parts)
                {
                    if (p.Modules.Contains("MissileFire"))
                    {
                        var pp = p.Modules.OfType<MissileFire>().Single();
                        pp = p.FindModulesImplementing<MissileFire>().First();
                        if (pp.selectedWeapon != null)
                        {
                            if (pp.selectedWeapon.GetPart() == this.part && pp.isArmed)
                            {
                                BDArmorySettings.BOMB_CLEARANCE_CHECK = false;
                                thisWeaponSelected = true;
                            }
                            else
                            {
                                if (thisWeaponSelected == true)
                                {
                                    BDArmorySettings.BOMB_CLEARANCE_CHECK = true;
                                    thisWeaponSelected = false;
                                }

                            }
                        }
                        else
                        {
                            if (thisWeaponSelected == true)
                            {
                                BDArmorySettings.BOMB_CLEARANCE_CHECK = true;
                            }
                            thisWeaponSelected = false;
                        }
                    }
                }
            }
        }


        private void Update()
        {
            checkForClearance();
        }
    }
}
