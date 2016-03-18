using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    public class ModuleExplosiveStorage : PartModule
    {
        [KSPField(isPersistant = false)]
        private String resource;

        [KSPField(isPersistant = false)]
        private float blastRadius = 0;

        [KSPField(isPersistant = false)]
        private float blastPower = 0;

        [KSPField(isPersistant = false)]
        private float blastHeat = 0;

        public override void OnStart(StartState state)
        {
            this.part.OnJustAboutToBeDestroyed += new Callback(checkResource);
            this.part.force_activate();
            base.OnStart(state);
        }

        private void checkResource()
        {
            if (resource != null)
            {
                if (this.part.Resources.Contains(resource))
                {
                    if (this.part.Resources[resource].amount > 0)
                    {
                        float amount = Convert.ToSingle(Math.Floor(this.part.Resources[resource].amount));
                        if (this.part.Modules.Contains("BDExplosivePart"))
                        {
                            var pm = this.part.Modules.OfType<BDExplosivePart>().Single();
                            pm = this.part.FindModulesImplementing<BDExplosivePart>().First();
                            pm.blastRadius = amount * blastRadius;
                            pm.blastPower = amount * blastPower;
                            pm.blastHeat = amount * blastHeat;
                        }
                    }
                }
            }
        }
    }
}
