using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BahaTurret;

namespace sinkingabout
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ModuleShipAlarm : MonoBehaviour
    {
        public AudioSource generalAlarm;
        public AudioSource panicAlarm;
        public AudioSource clearAudio;
        public AudioSource firingAudio;
        GameObject soundObject = new GameObject();

        public int activeBreach = 0;
        public int criticalBreach = 0;
        public int commandModules = 0;
        public int shipAlarmModules = 0;
        public int armedModules = 0;
        public bool criticalMode = false;
        public bool delay = false;
        public bool leaking = false;
        public bool commandModulePresent = false;
        public bool shipAlarmModulePresent = false;
        public bool isArmed = false;

        public void checkForBreach()
        {
            Vessel v = FlightGlobals.ActiveVessel;
            if (v != null && v.isEVA != true && v.isCommandable && v.IsControllable)
            {
                foreach (Part p in v.parts)
                {
                    if (p.Modules.Contains("ModuleShipAlarmModule"))
                    {
                        shipAlarmModules += 1;
                    }
                    if (p.Modules.Contains("ModuleCommand"))
                    {
                        commandModules += 1;
                    }
                    if (p.Modules.Contains("ModuleSinkingAbout"))
                    {
                        var pp = p.Modules.OfType<ModuleSinkingAbout>().Single();
                        pp = p.FindModulesImplementing<ModuleSinkingAbout>().First();
                        if (pp.hasHullBreach)
                        {
                            activeBreach += 1;
                        }
                        if (pp.isCritical)
                        {
                            criticalBreach += 1;
                        }
                    }
                    if (p.Modules.Contains("MissileFire"))
                    {
                        var pp = p.Modules.OfType<MissileFire>().Single();
                        pp = p.FindModulesImplementing<MissileFire>().First();
                        if (pp.selectedWeapon != null && pp.isArmed)
                        {
                            armedModules += 1;
                        }
                    }
                }
            }
            else
            {
                commandModules = 0;
                shipAlarmModules = 0;
                criticalBreach = 0;
                activeBreach = 0;
                armedModules = 0;
            }
            if (activeBreach > 0)
            {
                leaking = true;
            }
            else
            {
                leaking = false;
            }
            if (criticalBreach > 0)
            {
                criticalMode = true;
            }
            else
            {
                criticalMode = false;
            }

            if (commandModules > 0)
            {
                commandModulePresent = true;
            }
            else
            {
                commandModulePresent = false;
            }

            if (shipAlarmModules > 0)
            {
                shipAlarmModulePresent = true;
            }
            else
            {
                shipAlarmModulePresent = false;
            }
            if (armedModules > 0)
            {
                isArmed = true;
            }
            else
            {
                isArmed = false;
            }

            shipAlarmModules = 0;
            activeBreach = 0;
            commandModules = 0;
            criticalBreach = 0;
            armedModules = 0;
        }

        public void clearAudioHandler()
        {
            if (!delay) return;
            else
            {
                if (!leaking)
                {
                    if (clearAudio != null)
                    {
                        if (!clearAudio.isPlaying && FlightGlobals.ActiveVessel.isEVA != true && commandModulePresent)
                        {
                            clearAudio.Play();
                        }
                    }
                    delay = false;
                }
            }
        }

        public void firingAlert()
        {
            if (isArmed)
            {
                if (FlightGlobals.ActiveVessel.isEVA == false && shipAlarmModulePresent && commandModulePresent)
                {
                    if (firingAudio != null)
                    {
                        if (!firingAudio.isPlaying)
                        {
                            firingAudio.Play();
                        }
                    }
                }
            }
            else
            {
                if (firingAudio != null)
                {
                    if (firingAudio.isPlaying)
                    {
                        firingAudio.Stop();
                    }
                }
            }
        }


        public void alarmHandler()
        {
            if (leaking)
            {
                delay = true;
                if (FlightGlobals.ActiveVessel.isEVA == false && commandModulePresent)
                {
                    onScreenMessages();
                    if (generalAlarm != null)
                    {
                        if (!generalAlarm.isPlaying)
                        {
                            generalAlarm.Play();
                        }
                    }
                    if (criticalMode)
                    {
                        if (panicAlarm != null)
                        {
                            if (!panicAlarm.isPlaying)
                            {
                                panicAlarm.Play();
                            }
                        }
                    }
                }
                else
                {
                    if (generalAlarm != null)
                    {
                        if (generalAlarm.isPlaying)
                        {
                            generalAlarm.Stop();
                        }
                    }
                    if (panicAlarm != null)
                    {
                        if (panicAlarm.isPlaying)
                        {
                            panicAlarm.Stop();
                        }
                    }
                }
            }
            else
            {
                if (generalAlarm != null)
                {
                    if (generalAlarm.isPlaying)
                    {
                        generalAlarm.Stop();
                    }
                }
                if (panicAlarm != null)
                {
                    if (panicAlarm.isPlaying)
                    {
                        panicAlarm.Stop();
                    }
                }
            }
        }

        float timerCurrent = 0f;
        float timerTotal = 1f;

        private void tickHandler()
        {
            timerCurrent += Time.deltaTime;
            if (timerCurrent >= timerTotal)
            {
                timerCurrent -= timerTotal;
                checkForBreach();
                alarmHandler();
                clearAudioHandler();
                firingAlert();
            }
        }

        public void Update()
        {
            tickHandler();
            if (generalAlarm.isPlaying || panicAlarm.isPlaying || clearAudio.isPlaying || firingAudio.isPlaying)
            {
                soundObject.transform.position = FlightGlobals.ActiveVessel.transform.position;
            }
            else
            {
                return;
            }
        }

        public void onScreenMessages()
        {
            if (leaking)
            {
                ScreenMessages.PostScreenMessage("Warning: Taking on SeaWater!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        public void onGamePause()
        {
            if (generalAlarm != null)
            {
                generalAlarm.volume = 0;
            }
            if (panicAlarm != null)
            {
                panicAlarm.volume = 0;
            }
            if (clearAudio != null)
            {
                clearAudio.volume = 0;
            }
            if (firingAudio != null)
            {
                firingAudio.volume = 0;
            }
        }

        public void onGameUnpause()
        {
            if (generalAlarm != null)
            {
                generalAlarm.volume = GameSettings.SHIP_VOLUME / 5;
            }
            if (panicAlarm != null)
            {
                panicAlarm.volume = GameSettings.SHIP_VOLUME / 5;
            }
            if (clearAudio != null)
            {
                clearAudio.volume = GameSettings.SHIP_VOLUME / 5;
            }
            if (firingAudio != null)
            {
                firingAudio.volume = GameSettings.SHIP_VOLUME / 5;
            }
        }

        private void OnDestroy()
        {
            if (generalAlarm != null)
            {
                generalAlarm.Stop();
            }
            if (panicAlarm != null)
            {
                panicAlarm.Stop();
            }
            if (clearAudio != null)
            {
                clearAudio.Stop();
            }
            if (firingAudio != null)
            {
                firingAudio.Stop();
            }
            GameEvents.onGamePause.Remove(onGamePause);
            GameEvents.onGameUnpause.Remove(onGameUnpause);
        }

        public void Start()
        {
            soundObject.transform.position = FlightGlobals.ActiveVessel.transform.position;

            generalAlarm = soundObject.AddComponent<AudioSource>();
            generalAlarm.volume = GameSettings.SHIP_VOLUME / 5;
            generalAlarm.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/WarningSound");
            generalAlarm.loop = true;
            generalAlarm.dopplerLevel = 0;
            generalAlarm.Stop();

            panicAlarm = soundObject.AddComponent<AudioSource>();
            panicAlarm.volume = GameSettings.SHIP_VOLUME / 5;
            panicAlarm.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/PanicSound");
            panicAlarm.loop = true;
            panicAlarm.dopplerLevel = 0;
            panicAlarm.Stop();

            clearAudio = soundObject.AddComponent<AudioSource>();
            clearAudio.volume = GameSettings.SHIP_VOLUME / 5;
            clearAudio.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/ClearSound");
            clearAudio.loop = false;
            clearAudio.dopplerLevel = 0;
            clearAudio.Stop();

            firingAudio= soundObject.AddComponent<AudioSource>();
            firingAudio.volume = GameSettings.SHIP_VOLUME / 5;
            firingAudio.clip = GameDatabase.Instance.GetAudioClip("NANA/SinkingAbout/Sounds/ArmedSound");
            firingAudio.loop = true;
            firingAudio.dopplerLevel = 0;
            firingAudio.Stop();

            GameEvents.onGamePause.Add(onGamePause);
            GameEvents.onGameUnpause.Add(onGameUnpause);
        }
    }
}
