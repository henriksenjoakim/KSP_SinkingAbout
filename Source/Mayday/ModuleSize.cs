using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace sinkingabout
{
    public class ModuleSize : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "CenterX")]
        public double centerx;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "ExtentX")]
        public double extentx;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "SizeX")]
        public double sizex;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "minSizeX")]
        public double minsizex;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "CenterY")]
        public double centery;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "ExtentY")]
        public double extenty;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "SizeY")]
        public double sizey;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "minSizeY")]
        public double minsizey;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "CenterZ")]
        public double centerz;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "ExtentZ")]
        public double extentz;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "SizeZ")]
        public double sizez;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "minSizeZ")]
        public double minsizez;

        public float timerCurrent = 0f;
        public float timerTotal = 1f;

        private void tickHandler()
        {
            timerCurrent += Time.deltaTime;
            if (timerCurrent >= timerTotal)
            {
                timerCurrent -= timerTotal;

                    sizex = this.part.collider.bounds.max.x;
                    sizey = this.part.collider.bounds.max.y;
                    sizez = this.part.collider.bounds.max.z;
                    minsizex = this.part.collider.bounds.min.x;
                    minsizey = this.part.collider.bounds.min.y;
                    minsizez = this.part.collider.bounds.min.z;
                    centerx = this.part.collider.bounds.center.x;
                    centery = this.part.collider.bounds.center.y;
                    centerz = this.part.collider.bounds.center.z;
                    extentx = this.part.collider.bounds.extents.x;
                    extenty = this.part.collider.bounds.extents.y;
                    extentz = this.part.collider.bounds.extents.z;
            }
        }

        public void Update()
        {
            tickHandler();
        }
    }
}
