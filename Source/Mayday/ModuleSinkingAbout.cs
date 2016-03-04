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

        public AudioSource waterAudio;
        public AudioSource bubbleAudio;




        public void takeOnWater()
        {
            //if (!this.part.WaterContact) return;
            if (!hasHullBreach)
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
            if (hasHullBreach)
            {
                if (this.part.submergedPortion == 1)
                {
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
                    if (this.part.Resources["SeaWater"].amount < (this.part.submergedPortion * this.part.Resources["SeaWater"].maxAmount))
                    {
                        if (isCritical)
                        {
                            this.part.RequestResource("SeaWater", (0 - (critFlowRate * this.part.submergedPortion)));
                        }
                        else
                        {
                            this.part.RequestResource("SeaWater", (0 - (flowRate * this.part.submergedPortion)));
                        }
                    }
                }
                else
                {
                    if (this.part.Resources["SeaWater"].amount > (this.part.submergedPortion * this.part.Resources["SeaWater"].maxAmount))
                    {
                        if (isCritical)
                        {
                            this.part.RequestResource("SeaWater", (critFlowRate));
                        }
                        else
                        {
                            this.part.RequestResource("SeaWater", (flowRate));
                        }
                    }
                }
            }
        }

        public void checkForHullBreach()
        {
            if (this.part.temperature >= (this.part.maxTemp * breachTemp))
            {
                hasHullBreach = true;
            }   
            else
            {
                if (isDoomed)
                {

                }
                else
                {
                    hasHullBreach = false;
                }
            }

            
            if (this.part.temperature >= (this.part.maxTemp * critBreachTemp))
            {
                isCritical = true;
            }
            else
            {
                if (isDoomed)
                {
                
                }
                else
                {
                    isCritical = false;
                }
            }

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
                isCritical = true;
                isDoomed = true;
            }
            if (!initialCheck)
            {
                initialCheck = true;
            }
            if (isDoomed)
            {

            }
            else
            {
                connectedParts = partCount;
            } 
            partCount = 0;
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
                checkForHullBreach();
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
