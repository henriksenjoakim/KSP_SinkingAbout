using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    public class ModuleColliderIgnore : PartModule
    {
        private GameObject launchFx;
        private GameObject fireFx;
        private GameObject sparkFx;
        private Light fireLight;
        private Light fireLightLauncher;
        private GameObject glowFx;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false, guiName = "IsTubeloaded")]
        private bool isTubeLoaded = false;

        private Color lightColorYellow = new Color(240, 184, 49);
        private Color lightColorRed = new Color(237, 49, 12);
        private Color lightColorWhite = new Color(255, 255, 255);

        private AudioSource launchAudio;
        private GameObject soundObject = new GameObject();

        private Part hullcollider;
        private Part launcher;
        private bool hasLaunched = false;
        private float defaultCrashTolerance;

        private void Update()
        {
            checkForClearance();
        }

        private void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;
            launchDetector();            
            launchTimer();
        }

        private float launchTimerCurrent = 0f;
        private float launchTimerTotal = 5f;
        private float oneSec = 1f;
        private float twoSec = 2f;
        private bool runOnce = false;

        private void launchTimer()
        {
            if (!isTubeLoaded) return;
            if (!hasLaunched) return;
            if (!runOnce)
            {
                
                if (launchTimerCurrent < oneSec)
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
                                    launchFx.particleEmitter.localVelocity = p.transform.up * 20;
                                    launchFx.particleEmitter.emit = true;
                                }
                                if (fireFx != null)
                                {
                                    fireFx.transform.position = p.transform.position;
                                    fireFx.particleEmitter.localVelocity = p.transform.up * 20;
                                    fireFx.particleEmitter.emit = true;
                                }
                                if (sparkFx != null)
                                {
                                    sparkFx.transform.position = p.transform.position;
                                    sparkFx.particleEmitter.localVelocity = p.transform.up * 20;
                                    sparkFx.particleEmitter.emit = true;
                                }
                                if (fireLightLauncher != null)
                                {
                                    //fireLightLauncher.transform.position = p.transform.position;
                                    fireLightLauncher.color = Color.Lerp(lightColorRed, lightColorYellow, UnityEngine.Random.Range(0f, 3f));
                                    fireLightLauncher.intensity = UnityEngine.Random.Range(0.05f, 0.1f);
                                    fireLightLauncher.enabled = true;
                                }

                            }
                        }
                    }
                }
                else
                {
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
                    if (fireLightLauncher != null)
                    {
                        fireLightLauncher.enabled = false;
                    }
                }

                if (launchTimerCurrent > twoSec)
                {
                    this.part.crashTolerance = defaultCrashTolerance;
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
                }

                if (fireLight != null)
                {
                    //fireLight.transform.position = this.part.transform.position;
                    fireLight.color = Color.Lerp(lightColorRed, lightColorYellow, UnityEngine.Random.Range(0f, 3f));
                    fireLight.intensity = UnityEngine.Random.Range(0.2f, 0.3f);
                    fireLight.enabled = true;
                }
                

                glowFx.transform.position = this.part.transform.position;

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
                    if (fireLightLauncher != null)
                    {
                        fireLightLauncher.enabled = false;
                    }
                    if (fireLight != null)
                    {
                        fireLight.enabled = false;
                    }
                    if (glowFx != null)
                    {
                        foreach (var pe in glowFx.GetComponentsInChildren<KSPParticleEmitter>())
                        {
                            pe.emit = false;
                            pe.enabled = false;
                        }
                        GameObject.Destroy(glowFx);
                        //Destroy(glowFx);
                    }
       
                    runOnce = true;
                }
            }
        }


        private bool thisWeaponSelected = false;
        private void checkForClearance()
        {
            if (!isTubeLoaded) return;
            if (hullcollider == null) return;

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

        private void launchDetector()
        {
            if (!isTubeLoaded) return;
            if (hasLaunched) return;
            if (this.part.Modules.Contains("MissileLauncher"))
            {
                var pm = this.part.Modules.OfType<MissileLauncher>().Single();
                pm = this.part.FindModulesImplementing<MissileLauncher>().First();
                if (pm.hasFired)
                {                    
                    if (launchAudio != null)
                    {
                        if (!launchAudio.isPlaying)
                        {
                            launchAudio.Play();
                        }
                    }
                    if (glowFx != null)
                    {
                        foreach (var pe in glowFx.GetComponentsInChildren<KSPParticleEmitter>())
                        {
                            pe.emit = true;
                            pe.enabled = true;
                        }
                        glowFx.SetActive(true);
                    }

                    hasLaunched = true;
                }
            }
        }


        private void OnCollisionEnter(Collision c)
        {
            if (!isTubeLoaded) return;
            if (hullcollider != null)
            {
                foreach (ContactPoint cp in c.contacts)
                {                    
                    if (cp.otherCollider.name == hullcollider.collider.name)
                    {
                        Physics.IgnoreCollision(this.part.collider, cp.otherCollider, true);
                        //hasLaunched = true;
                    } 

                }
            }          
        }

        private void setupFx()
        {
     
            launchFx = (GameObject)GameObject.Instantiate(UnityEngine.Resources.Load("Effects/fx_smokeTrail_light"));
            launchFx.transform.position = this.part.parent.transform.position;
            launchFx.particleEmitter.localVelocity = this.part.parent.transform.up * 10;
            launchFx.particleEmitter.useWorldSpace = true;
            launchFx.particleEmitter.maxEnergy = 8;
            launchFx.particleEmitter.maxEmission = 450;
            launchFx.particleEmitter.minEnergy = 5;
            launchFx.particleEmitter.minEmission = 400;
            launchFx.particleEmitter.maxSize = 3f;
            launchFx.particleEmitter.minSize = 1f;
            launchFx.particleEmitter.emitterVelocityScale = 1;
            //launchFx.particleEmitter.angularVelocity = 10;
            launchFx.particleEmitter.emit = false;

            fireFx = (GameObject)GameObject.Instantiate(UnityEngine.Resources.Load("Effects/fx_exhaustFlame_yellow"));
            fireFx.transform.position = this.part.parent.transform.position;
            fireFx.particleEmitter.localVelocity = this.part.parent.transform.up * 10;
            fireFx.particleEmitter.useWorldSpace = true;
            fireFx.particleEmitter.maxEnergy = 0.8f;
            fireFx.particleEmitter.maxEmission = 1000;
            fireFx.particleEmitter.minEnergy = 0.5f;
            fireFx.particleEmitter.minEmission = 850;
            fireFx.particleEmitter.maxSize = 1.2f;
            fireFx.particleEmitter.minSize = 0.8f;
            fireFx.particleEmitter.emitterVelocityScale = 1;
            //fireFx.particleEmitter.angularVelocity = 10;
            fireFx.particleEmitter.emit = false;

            sparkFx = (GameObject)GameObject.Instantiate(UnityEngine.Resources.Load("Effects/fx_exhaustSparks_flameout"));
            sparkFx.transform.position = this.part.transform.position;
            sparkFx.particleEmitter.localVelocity = this.part.parent.transform.up * 10;
            sparkFx.particleEmitter.useWorldSpace = true;
            sparkFx.particleEmitter.minEnergy = 5;
            sparkFx.particleEmitter.minEmission = 50;
            sparkFx.particleEmitter.maxEnergy = 5;
            sparkFx.particleEmitter.maxEmission = 60;
            sparkFx.particleEmitter.maxSize = 0.1f;
            sparkFx.particleEmitter.maxSize = 0.1f;
            sparkFx.particleEmitter.emitterVelocityScale = 1;
            //sparkFx.particleEmitter.angularVelocity = 10;
            sparkFx.particleEmitter.emit = false;

            glowFx = (GameObject)GameObject.Instantiate(GameDatabase.Instance.GetModel("NANA/SinkingAbout/FX/glow")/*, this.part.transform.position, Quaternion.identity*/);
            glowFx.SetActive(true);
            foreach (var pe in glowFx.GetComponentsInChildren<KSPParticleEmitter>())
            {
                pe.emit = false;
                pe.enabled = false;
            }
        
            fireLight = glowFx.AddComponent<Light>();
            fireLight.transform.position = this.part.transform.position;
            fireLight.type = LightType.Point;
            fireLight.shadows = LightShadows.Hard;
            fireLight.enabled = false;
            fireLight.intensity = 1f;
            fireLight.range = 50f;

            fireLightLauncher = launchFx.AddComponent<Light>();
            fireLightLauncher.transform.position = this.part.parent.transform.position;
            fireLightLauncher.type = LightType.Point;
            fireLightLauncher.shadows = LightShadows.Hard;
            fireLightLauncher.enabled = false;
            fireLightLauncher.intensity = 0.2f;
            fireLightLauncher.range = 30f;

            launchAudio = soundObject.AddComponent<AudioSource>();
            launchAudio.volume = GameSettings.SHIP_VOLUME;
            launchAudio.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/LaunchSound");
            launchAudio.loop = false;
            launchAudio.dopplerLevel = 0;
            launchAudio.Stop();

            GameEvents.onGamePause.Add(onGamePause);
            GameEvents.onGameUnpause.Add(onGameUnpause);

        }


        private void onGamePause()
        {
            if (launchAudio != null)
            {
                launchAudio.volume = 0;
            }
        }

        private void onGameUnpause()
        {
            if (launchAudio != null)
            {
                launchAudio.volume = GameSettings.SHIP_VOLUME;
            }
        }


        private void onVesselWillDestroy(Vessel v)
        {
            if (v = this.part.vessel)
            {
                if (fireLight != null)
                {
                    fireLight.enabled = false;
                }
                if (fireLightLauncher != null)
                {
                    fireLightLauncher.enabled = false;
                }
                //GameObject.Destroy(glowFx);
                //Destroy(glowFx);
            }
        }

        private void OnDestroy()
        {
            if (fireLight != null)
            {
                fireLight.enabled = false;
                //Destroy(fireLight);
            }
            if (fireLightLauncher != null)
            {
                fireLightLauncher.enabled = false;
                //Destroy(fireLightLauncher);
            }
            if (launchFx != null)
            {
                launchFx.particleEmitter.emit = false;
                //Destroy(launchFx);
            }
            if (fireFx != null)
            {
                fireFx.particleEmitter.emit = false;
            }
            if (sparkFx != null)
            {
                sparkFx.particleEmitter.emit = false;
            }
            if (launchAudio != null)
            {
                launchAudio.Stop();
            }
            if (glowFx != null)
            {
                foreach (var pe in glowFx.GetComponentsInChildren<KSPParticleEmitter>())
                {
                    pe.emit = false;
                    pe.enabled = false;
                }
                GameObject.Destroy(glowFx);
                //Destroy(glowFx);
            }
            GameEvents.onGamePause.Remove(onGamePause);
            GameEvents.onGameUnpause.Remove(onGameUnpause);
        }


        private void setup()
        {
            if (this.part.parent != null)
            {
                if (this.part.parent.name.Contains("MK41VLSINGLE") || this.part.parent.name.Contains("MK41VLSQUAD"))
                {

                    defaultCrashTolerance = this.part.crashTolerance;
                    isTubeLoaded = true;
                    launcher = this.part.parent;
                    setupFx();
                    if (this.part.parent.parent != null)
                    {
                        hullcollider = this.part.parent.parent;
                    }
                    this.part.crashTolerance = this.part.parent.crashTolerance;
                    this.part.breakingForce = this.part.parent.breakingForce;
                    this.part.breakingTorque = this.part.parent.breakingForce;
                    if (this.part.Modules.Contains("MissileLauncher"))
                    {
                        var pp = this.part.Modules.OfType<MissileLauncher>().Single();
                        pp = this.part.FindModulesImplementing<MissileLauncher>().First();
                        pp.boostClipPath = "NANA/SinkingAbout/Sounds/LaunchSound";
                        pp.boostExhaustPrefabPath = "BDArmory/Models/exhaust/largeExhaust";
                    }
                }
            }
        }

        public override void OnStart(StartState state)
        {            
            if (state == StartState.Editor || state == StartState.None) return;

            setup();

            base.OnStart(state);
        }
    }
}
