using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace sinkingabout
{
    public class ModuleSinkingAbout: PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "SA: Sponge")]
        public bool sponge = false;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "SA: Sinks if damaged")]
        public bool damageable = false;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "SA: Explodes under water")]
        public bool hydroExplosive = false;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "SA: Hull part")]
        public bool hull = false;

        [KSPField(isPersistant = false)]
        public double flowRate = 1;

        [KSPField(isPersistant = false)]
        public double critFlowRate = 1;

        [KSPField(isPersistant = false)]
        public double breachTemp = 0.6;

        [KSPField(isPersistant = false)]
        public double critBreachTemp = 0.9;

        [KSPField(isPersistant = true)]
        public bool initialCheck = false;

        public bool hasHullBreach = false;
        public bool isCritical = false;
        public bool isDoomed = false;
        public double previousAmount;     
        public int connectedParts;
        public bool takingOnWater = false;
        public bool sinkingAbout = false;
        
        public AudioSource waterAudio;
        public AudioSource bubbleAudio;

        /*
        [UI_FloatRange(minValue = 1, maxValue = 100, stepIncrement = 1)]
        [KSPField(guiActive = true, guiActiveEditor = true, guiFormat = "P0", isPersistant = true, guiName = "FlowMultiplier")]
        public float flowMultiplier = 1;

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, externalToEVAOnly = false, guiActiveUnfocused = true, guiName = "Test HullBreach")]
        public void hullbreach()
        {
            if (hasHullBreach)
            {
                hasHullBreach = false;
                isDoomed = false;
            }
            else
            {
                hasHullBreach = true;
                isDoomed = true;
            }
        }
        */

        private float hydroExoplosiveTimerCurrent = 0f;
        private float hydroExplosiveTimerTotal = 30f;
        public void hydroExplosiveness()
        {
            if (!hydroExplosive) return;
            if (!this.part.WaterContact) return;
            if (hydroExplosive)
            {
                if (this.part.WaterContact && this.part.submergedPortion == 1)
                {
                    hydroExoplosiveTimerCurrent += Time.deltaTime;
                    if (hydroExoplosiveTimerCurrent >= hydroExplosiveTimerTotal)
                    {
                        hydroExoplosiveTimerCurrent -= hydroExplosiveTimerTotal;
                        this.part.explode();
                    }
                }
                else
                {
                    hydroExoplosiveTimerCurrent = 0f;
                }
            }
        }

        public void takeOnWater()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;
            //if (!this.part.WaterContact) return;
            if (!sinkingAbout)
            {
                if (waterAudio != null)
                {
                    if (waterAudio.isPlaying)
                    {
                        waterAudio.Stop();
                    }
                }
                return;
            }
            if (sinkingAbout)
            {                
                if (this.part.submergedPortion == 1)
                {
                    hydroExplosiveness();
                    if (waterAudio != null)
                    {
                        if (waterAudio.isPlaying)
                        {
                            waterAudio.Stop();
                        }
                    }
                    if (this.part.Resources["SeaWater"].amount != previousAmount)
                    {
                        if (bubbleAudio != null)
                        {
                            if (!bubbleAudio.isPlaying)
                            {
                                bubbleAudio.Play();
                            }
                        }
                    }
                    else
                    {
                        if (bubbleAudio != null)
                        {
                            if (bubbleAudio.isPlaying)
                            {
                                bubbleAudio.Stop();
                            }
                        }
                    }
                }
                else
                {
                    if (bubbleAudio != null)
                    {
                        if (bubbleAudio.isPlaying)
                        {
                            bubbleAudio.Stop();
                        }
                    }
                    if (this.part.Resources["SeaWater"].amount != previousAmount)
                    {
                        if (waterAudio != null)
                        {
                            if (!waterAudio.isPlaying)
                            {
                                waterAudio.Play();
                            }
                        }
                    }
                    else
                    {
                        if (waterAudio != null)
                        {
                            if (waterAudio.isPlaying)
                            {
                                waterAudio.Stop();
                            }
                        }
                    }
                }


                previousAmount = this.part.Resources["SeaWater"].amount;

                if (this.part.WaterContact)
                {
                    takingOnWater = true;
                    if (isCritical)
                    {
                        this.part.RequestResource("SeaWater", (0 - (critFlowRate * (0.1 + this.part.submergedPortion)/* * flowMultiplier*/)));
                    }
                    else
                    {
                        this.part.RequestResource("SeaWater", (0 - (flowRate * (0.1 + this.part.submergedPortion)/* * flowMultiplier*/)));
                    }
                }
                else
                {
                    if (isCritical)
                    {
                        this.part.RequestResource("SeaWater", (critFlowRate/* * flowMultiplier*/));
                    }
                    else
                    {
                        this.part.RequestResource("SeaWater", (flowRate/* * flowMultiplier*/));
                    }
                    takingOnWater = false;
                }
            }
            else
            {
                takingOnWater = false;
            }
        }


        public void checkConditions()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;

            if (sponge)
            {
                if (this.part.WaterContact)
                {
                    sinkingAbout = true;
                }
                else
                {
                    if (!isDoomed)
                    {
                        sinkingAbout = false;
                    }
                }
            }

            if (damageable)
            {
                if (this.part.temperature >= (this.part.maxTemp * breachTemp))
                {
                    hasHullBreach = true;
                    sinkingAbout = true;

                }
                else
                {
                    if (!isDoomed)
                    {
                        hasHullBreach = false;
                        sinkingAbout = false;
                    }
                }
                if (this.part.temperature >= (this.part.maxTemp * critBreachTemp))
                {
                    isCritical = true;
                }
                else
                {
                    if (!isDoomed)
                    {
                        isCritical = false;
                    }
                }


                if (hull)
                {
                    int partCount = 0;
                    if (this.part.parent != null)
                    {
                        if (this.part.parent.Modules.Contains("ModuleSinkingAbout"))
                        {
                            partCount += 1;
                        }
                    }
                    if (this.part.children.Count > 0)
                    {
                        foreach (Part p in this.part.children)
                        {
                            if (p.Modules.Contains("ModuleSinkingAbout"))
                            {
                                partCount += 1;
                            }
                        }
                    }
                    if (partCount < connectedParts && initialCheck)
                    {
                        hasHullBreach = true;
                        sinkingAbout = true;
                        isCritical = true;
                        isDoomed = true;
                    }
                    if (!initialCheck)
                    {
                        initialCheck = true;
                    }
                    if (!isDoomed)
                    {
                        connectedParts = partCount;
                    }
                    partCount = 0;
                }

            }
        }

        //Ticker
        private float timerCurrent = 0f;
        private float timerTotal = 1f;
        private void tickHandler()
        {
            timerCurrent += Time.deltaTime;
            if (timerCurrent >= timerTotal)
            {
                timerCurrent -= timerTotal;
                checkConditions();
            }
        }


        private void setupAudio()
        {
            waterAudio = gameObject.AddComponent<AudioSource>();
            waterAudio.volume = GameSettings.SHIP_VOLUME / 8;
            waterAudio.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/WaterSound");
            waterAudio.loop = true;
            waterAudio.Stop();

            bubbleAudio = gameObject.AddComponent<AudioSource>();
            bubbleAudio.volume = GameSettings.SHIP_VOLUME / 8;
            bubbleAudio.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/BubbleSound");
            bubbleAudio.loop = true;
            bubbleAudio.Stop();
        }

        public void FixedUpdate()
        {
            tickHandler();
            takeOnWater(); 
        }

        public void OnDestroy()
        {
            if (waterAudio != null)
            {
                waterAudio.Stop();
            }
            if (bubbleAudio != null)
            {
                bubbleAudio.Stop();
            }

            GameEvents.onGamePause.Remove(onGamePause);
            GameEvents.onGameUnpause.Remove(onGameUnpause);
        }

        public void onGamePause()
        {
            if (waterAudio != null)
            {
                waterAudio.volume = 0;
            }
            if (bubbleAudio != null)
            {
                bubbleAudio.volume = 0;
            }
        }

        public void onGameUnpause()
        {
            if (waterAudio != null)
            {
                waterAudio.volume = GameSettings.SHIP_VOLUME / 8;
            }
            if (bubbleAudio != null)
            {
                bubbleAudio.volume = GameSettings.SHIP_VOLUME / 8;
            }
        }   

        public override void OnStart(PartModule.StartState state)
        {
            if (state == StartState.Editor || state == StartState.None) return;
            setupAudio();
            GameEvents.onGamePause.Add(onGamePause);
            GameEvents.onGameUnpause.Add(onGameUnpause);
            base.OnStart(state);
        }
    }
}
