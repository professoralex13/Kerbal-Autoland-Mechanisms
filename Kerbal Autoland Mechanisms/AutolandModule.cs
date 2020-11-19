using System;
using System.Collections;

using UnityEngine;

namespace Kerbal_Autoland_Mechanisms
{
    public class AutolandModule : VesselModule
    {
        bool running;

        public VesselAutopilot auto;

        //public KAMUIController uiController;

        private new void Start()
        {
            base.Start();
            if (!vessel.isActiveVessel)
                Destroy(this);

            //string path = KSPUtil.ApplicationRootPath + "GameData/YourMod/PrefabFolder";

            //AssetBundle prefabs = AssetBundle.LoadFromFile(path + "/your_bundle_name");
            // uiController = Instantiate(prefabs.LoadAsset("Your_Prefab_Name") as GameObject).GetComponent<KAMUIController>();
        }
        private void Update()
        {
            if (ExtendedInput.GetKeyDown(new KeyCodeExtended(KeyCode.H)))
            {
                if (running)
                    StopAllCoroutines();
                else
                    StartCoroutine(SuborbitalUntargeted());
            }
        }
        public IEnumerator SuborbitalUntargeted()
        {
            yield return StartCoroutine(CoastToApoapsis());
            yield return StartCoroutine(Landing());
        }
        public IEnumerator CoastToApoapsis()
        {
            Debug.Log("Coasting to Apoapsis");
            yield return new WaitUntil(() => vessel.verticalSpeed <= -10);
        }
        public IEnumerator Landing()
        {
            float SuicideBurnTime = 0;
            float SuicideBurnAltitude = 0;
            SuicideBurnTime = AutolandCalculator.CalculateBurnTime(vessel);
            SuicideBurnAltitude = AutolandCalculator.CalculateBurnAltitude(SuicideBurnTime, vessel);
            Debug.Log("Coasting To Suicide Burn");
            auto.SetMode(VesselAutopilot.AutopilotMode.Retrograde);
            yield return new WaitUntil(() => vessel.GetHeightFromTerrain() > SuicideBurnAltitude);
            auto.SetMode(VesselAutopilot.AutopilotMode.StabilityAssist);
            vessel.ctrlState.mainThrottle = 100;
            Debug.Log("Burning");
            yield return new WaitUntil(() => vessel.verticalSpeed < -1);
            vessel.ctrlState.mainThrottle = 0;
        }
    }
}
