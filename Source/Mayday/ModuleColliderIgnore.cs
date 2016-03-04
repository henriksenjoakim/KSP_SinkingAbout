using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    public class ModuleColliderIgnore: PartModule
    {
        private GameObject launchFx;
        private GameObject fireFx;
        private GameObject sparkFx;
        private Light fireLight;

        private Color lightColorYellow = new Color(240, 184, 49);
        private Color lightColorRed = new Color(237, 49, 12);
        private Color lightColorWhite = new Color(255, 255, 255);

        public Part hullcollider;
        public Part launcher;
        public bool hasLaunched = false;



        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;
            checkForClearance();
            launchTimer();
        }

        float launchTimerCurrent = 0f;
        float launchTimerTotal = 2f;

        private void launchTimer()
        {
            if (!hasLaunched) return;
            if (hasLaunched)
            {
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    foreach (Part p in v.parts)
                    {
                        if (p == launcher)
                        {
                            if (launchFx != null)
                            {
                                launchFx.transform.position = p.transform.position;
                                launchFx.particleEmitter.localVelocity = p.transform.up * 10;
                                launchFx.particleEmitter.emit = true;
                            }
                            if (fireFx != null)
                            {
                                fireFx.transform.position = p.transform.position;
                                fireFx.particleEmitter.localVelocity = p.transform.up * 10;
                                fireFx.particleEmitter.emit = true;
                            }
                            if (sparkFx != null)
                            {
                                sparkFx.transform.position = p.transform.position;
                                sparkFx.particleEmitter.localVelocity = p.transform.up * 10;
                                sparkFx.particleEmitter.emit = true;
                            }
                        }
                    }
                }
                if (fireLight != null)
                {
                    fireLight.transform.position = this.part.transform.position;
                    fireLight.color = Color.Lerp(lightColorRed, lightColorYellow, UnityEngine.Random.Range(0f, 3f));
                    fireLight.intensity = UnityEngine.Random.Range(2f, 5f);
                    fireLight.enabled = true;
                }
                launchTimerCurrent += Time.deltaTime;
                if (launchTimerCurrent >= launchTimerTotal)
                {
                    launchTimerCurrent -= launchTimerTotal;
                    if (launchFx != null)
                    {
                        launchFx.particleEmitter.emit = false;
                    }
                    if (fireFx != null)
                    {
                        fireFx.particleEmitter.emit = false;
                    }
                    if (sparkFx != null)
                    {
                        sparkFx.particleEmitter.emit = false;
                    }
                    if (fireLight != null)
                    {
                        fireLight.enabled = false;
                    }
                    foreach (Vessel v in FlightGlobals.Vessels)
                    {
                        foreach (Part p in v.parts)
                        {
                            if (p == hullcollider)
                            {
                                Physics.IgnoreCollision(this.part.collider, p.collider, false);
                            }
                        }
                    }             
                    hasLaunched = false;
                }
            }
        }

        public void checkForClearance()
        {
            if (hullcollider == null) return;
            if (hullcollider != null)
            {
                if (!BDArmorySettings.BOMB_CLEARANCE_CHECK) return;
                if (BDArmorySettings.BOMB_CLEARANCE_CHECK)
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
                                }
                                else
                                {
                                    BDArmorySettings.BOMB_CLEARANCE_CHECK = true;
                                }
                            }
                            else
                            {
                                BDArmorySettings.BOMB_CLEARANCE_CHECK = true;
                            }

                        }
                    }
                }
            }
        }
        /*
        public void launchDetector()
        {
            if (hullcollider != null)
            {
                if (this.part.Modules.Contains("MissileLauncher"))
                {
                    var pm = this.part.Modules.OfType<MissileLauncher>().Single();
                    pm = this.part.FindModulesImplementing<MissileLauncher>().First();
                    if (pm.hasFired)
                    {
                        foreach (Vessel v in FlightGlobals.Vessels)
                        {
                            foreach (Part p in v.parts)
                            {
                                if (p == hullcollider)
                                {
                                    Physics.IgnoreCollision(this.part.collider, p.collider, true);
                                    hasLaunched = true;
                                }
                            }
                        }                        
                    }
                }
            }
        }
        */
        
        public void OnCollisionEnter(Collision c)
        {
            if (hullcollider != null)
            {
                foreach (ContactPoint cp in c.contacts)
                {                    
                    if (cp.otherCollider.name == hullcollider.collider.name)
                    {
                        Physics.IgnoreCollision(this.part.collider, cp.otherCollider, true);
                        hasLaunched = true;
                    }              
                }
            }
        }
        
        public void setupFx()
        {
            launchFx = (GameObject)GameObject.Instantiate(UnityEngine.Resources.Load("Effects/fx_smokeTrail_light"));
            launchFx.transform.position = this.part.parent.transform.position;
            launchFx.particleEmitter.localVelocity = this.part.parent.transform.up * 10;
            launchFx.particleEmitter.useWorldSpace = true;
            launchFx.particleEmitter.maxEnergy = 9;
            launchFx.particleEmitter.maxEmission = 180;
            launchFx.particleEmitter.minEnergy = 6;
            launchFx.particleEmitter.minEmission = 140;
            launchFx.particleEmitter.maxSize = 2f;
            launchFx.particleEmitter.minSize = 1f;
            launchFx.particleEmitter.emitterVelocityScale = 1;
            //launchFx.particleEmitter.angularVelocity = 10;
            launchFx.particleEmitter.emit = false;

            fireFx = (GameObject)GameObject.Instantiate(UnityEngine.Resources.Load("Effects/fx_exhaustFlame_yellow"));
            fireFx.transform.position = this.part.parent.transform.position;
            fireFx.particleEmitter.localVelocity = this.part.parent.transform.up * 10;
            fireFx.particleEmitter.useWorldSpace = true;
            fireFx.particleEmitter.maxEnergy = 0.8f;
            fireFx.particleEmitter.maxEmission = 200;
            fireFx.particleEmitter.minEnergy = 0.5f;
            fireFx.particleEmitter.minEmission = 150;
            fireFx.particleEmitter.maxSize = 1f;
            fireFx.particleEmitter.minSize = 0.8f;
            fireFx.particleEmitter.emitterVelocityScale = 1;
            //fireFx.particleEmitter.angularVelocity = 10;
            fireFx.particleEmitter.emit = false;

            sparkFx = (GameObject)GameObject.Instantiate(UnityEngine.Resources.Load("Effects/fx_exhaustSparks_flameout"));
            sparkFx.transform.position = this.part.transform.position;
            sparkFx.particleEmitter.localVelocity = this.part.parent.transform.up * 10;
            sparkFx.particleEmitter.useWorldSpace = true;
            sparkFx.particleEmitter.minEnergy = 5;
            sparkFx.particleEmitter.minEmission = 10;
            sparkFx.particleEmitter.maxEnergy = 5;
            sparkFx.particleEmitter.maxEmission = 10;
            sparkFx.particleEmitter.maxSize = 0.1f;
            sparkFx.particleEmitter.maxSize = 0.1f;
            sparkFx.particleEmitter.emitterVelocityScale = 1;
            //sparkFx.particleEmitter.angularVelocity = 10;
            sparkFx.particleEmitter.emit = false;

            fireLight = this.part.gameObject.AddComponent<Light>();
            fireLight.transform.position = this.part.transform.position;
            fireLight.type = LightType.Point;
            fireLight.shadows = LightShadows.Hard;
            fireLight.enabled = false;
            fireLight.intensity = 1f;
            fireLight.range = 5f;
        }


        public void onVesselWillDestroy(Vessel v)
        {
            if (v = this.part.vessel)
            {
                if (fireLight != null)
                {
                    fireLight.enabled = false;
                }
            }
        }

        public void OnDestroy()
        {
            if (fireLight != null)
            {
                fireLight.enabled = false;
            }
            if (launchFx != null)
            {
                launchFx.particleEmitter.emit = false;
            }
            if (fireFx != null)
            {
                fireFx.particleEmitter.emit = false;
            }
            if (sparkFx != null)
            {
                sparkFx.particleEmitter.emit = false;
            }
        }

        public override void OnStart(StartState state)
        {
            if (state == StartState.Editor || state == StartState.None) return;
            if (this.part.parent != null)
            {
                if (this.part.parent.name.Contains("MK41VLSINGLE") || this.part.parent.name.Contains("MK41VLSQUAD"))
                {
                    launcher = this.part.parent;
                    setupFx();
                    if (this.part.parent.parent != null)
                    {
                        hullcollider = this.part.parent.parent;
                        
                    }
                }
            }

            base.OnStart(state);
        }
    }
}
