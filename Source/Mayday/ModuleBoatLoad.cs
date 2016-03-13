using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace sinkingabout
{
    public class ModuleBoatLoad : PartModule
    {
        [KSPField(isPersistant = true)]
        double vesselAltitude;

        public void onVesselChange(Vessel v)
        {
            if (this.part.vessel.Splashed)
            {
                
                vesselAltitude = this.part.vessel.altitude;
                this.part.vessel.altitude = part.vessel.altitude + 100;
            }            
        }


        public override void OnStart(StartState state)
        {
            //GameEvents.onVesselGoOffRails.Add(onVesselGoOffRails);
            GameEvents.onVesselChange.Add(onVesselChange);
            base.OnStart(state);
        }
    }
}
