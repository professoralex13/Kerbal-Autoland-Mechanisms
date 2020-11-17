using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Kerbal_Autoland_Mechanisms
{
    public static class AutolandCalculator
    {
        /// <summary>
        /// Calculates seconds until burn
        /// </summary>
        /// <param name="vessel"></param>
        /// <returns></returns>
        public static float CalculateBurnTime(Vessel vessel)
        {
            float velocity = (float)vessel.speed;
            float Gforce = (float)vessel.mainBody.gravParameter;
            float altitude = vessel.GetHeightFromTerrain();
            float isp = GetIsp(vessel);
            float mass = (float)vessel.totalMass;
            float maxThrust = 0;
            if (vessel.mainBody.atmosphere)
                maxThrust = GetMaxThrust(vessel);
            else
                maxThrust = GetMaxThrust(vessel);


            float upperHalf;
            float expE;
            float lowerHalf;

            expE = Mathf.Pow(2.71828182845904523536f, ((-velocity + Mathf.Sqrt(2 *
            Gforce * altitude)) / (Gforce * isp)));
            upperHalf = mass - (mass / expE);
            lowerHalf = maxThrust / (Gforce * isp);
            float final = upperHalf / lowerHalf;

            return final;


        }
        public static float CalculateBurnAltitude(float burnTime, Vessel vessel)
        {
            float upperhalf = -(float)vessel.speed + (float)Math.Sqrt(2f * (float)vessel.mainBody.gravParameter * (float)vessel.GetHeightFromTerrain());
            float final = (upperhalf / 2) * burnTime;

            return final;
        }
        public static float GetIsp(Vessel vessel)
        {
            var activeEngines = GetActiveEngines(vessel);
            float isp = 0;
            if (activeEngines.Count == 0)
            {
                Debug.Log("No Active Engines");
                return 0;
            }


            for (int i = 0; i < activeEngines.Count; i++)
            {

                isp += activeEngines[i].realIsp;
            }

            isp = isp / activeEngines.Count;
            return isp;
        }
        public static List<ModuleEngines> GetActiveEngines(Vessel vessel)
        {
            List<ModuleEngines> engines = new List<ModuleEngines>();

            for (int i = 0; i < vessel.parts.Count; i++)
            {
                if (vessel.parts[i].Modules.Contains<ModuleEngines>())
                {
                    for (int num = 0; num < vessel.parts[i].Modules.Count; num++)
                    {
                        if (vessel.parts[i].Modules.GetModule<ModuleEngines>() && vessel.Parts[i].Modules.GetModule<ModuleEngines>().enabled)
                        {
                            engines.Add(vessel.Parts[i].Modules.GetModule<ModuleEngines>());
                        }
                    }
                }


            }
            return engines;
        }
        public static float GetMaxThrust(Vessel vessel)
        {
            var body = vessel.mainBody;
            var activeEngines = GetActiveEngines(vessel);
            float thrust = 0;
            if (activeEngines.Count == 0)
            {
                Debug.Log("No Active Engines");
                return 0;
            }
            for (int i = 0; i < activeEngines.Count; i++)
            {
                if (!body.atmosphere)
                    thrust += activeEngines[i].MaxThrustOutputVac();
                else
                    thrust += activeEngines[i].MaxThrustOutputAtm(true, false, (float)body.GetPressureAtm(vessel.altitude), body.GetTemperature(vessel.altitude), body.GetDensity(body.GetPressureAtm(vessel.altitude), body.GetTemperature(vessel.altitude)));
            }
            return thrust;
        }
    }
}
