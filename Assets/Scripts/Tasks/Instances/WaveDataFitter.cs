using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Tasks {
    public class WaveDataFitter : WaveData, IWaveData {
        private WaveDataFitter old;

        public float T0 { get; protected set; }
        public float TotalMassCorrect { get; protected set; }
        public float TotalMassMin { get; protected set; }
        public float TotalMassMax { get; protected set; }
        public float TotalMassStartMin { get; protected set; }
        public float TotalMassStartMax { get; protected set; }
        public float DistanceCorrect { get; protected set; }
        public float DistanceMin { get; protected set; }
        public float DistanceMax { get; protected set; }
        public float DistanceStartMin { get; protected set; }
        public float DistanceStartMax { get; protected set; }
        public float WinConditionPercent { get; protected set; }
        public float Mass { get; protected set; }
        public float Dist { get; protected set; }

        public WaveDataFitter(List<List<float>> data, float m0 = 65, float d0 = 420, float mass = 65, float dist = 420) : base(data) {
            T0 = 0.423f;
            TotalMassCorrect = m0;
            DistanceCorrect = d0;
            OrgData = new List<Vector2>();
            OrgData = listToVector(data);
            scale(mass, dist);
        }
        [JsonConstructor]
        public WaveDataFitter(List<List<float>> data, float t0, float totalMassCorrect, float totalMassMin, float totalMassMax, float totalMassStartMin, float totalMassStartMax, float distanceCorrect, float distanceMin, float distanceMax, float distanceStartMin, float distanceStartMax, float winConditionPercent) : base(data) {
            T0 = t0;
            TotalMassCorrect = totalMassCorrect;
            TotalMassMin = totalMassMin;
            TotalMassMax = totalMassMax;
            TotalMassStartMin = totalMassStartMin;
            TotalMassStartMax = totalMassStartMax;
            DistanceCorrect = distanceCorrect;
            DistanceMin = distanceMin;
            DistanceMax = distanceMax;
            DistanceStartMin = distanceStartMin;
            DistanceStartMax = distanceStartMax;
            WinConditionPercent = winConditionPercent;
            scale(TotalMassCorrect, DistanceCorrect);
        }

        public WaveDataFitter(WaveDataFitter old) : base(old.OrgData) {
            T0 = old.T0;
            TotalMassCorrect = old.TotalMassCorrect;
            TotalMassMin = old.TotalMassMin;
            TotalMassMax = old.TotalMassMax;
            TotalMassStartMin = old.TotalMassStartMin;
            TotalMassStartMax = old.TotalMassStartMax;
            DistanceCorrect = old.DistanceCorrect;
            DistanceMin = old.DistanceMin;
            DistanceMax = old.DistanceMax;
            DistanceStartMin = old.DistanceStartMin;
            DistanceStartMax = old.DistanceStartMax;
            WinConditionPercent = old.WinConditionPercent;
            scale(TotalMassCorrect, DistanceCorrect);
        }

        public void shiftt(float t0) {
            foreach (Vector2 coords in OrgData) {
                //this.t[i] += t0;
            }
        }

        public void scale(float m, float d) {
            ModData = new List<Vector2>();
            //Debug.LogFormat("scaling from {0} to {1} and from {2} to {3}", M0, m, D0, d);
            Mass = m;
            Dist = d;
            for (int i = 0; i < OrgData.Count; i++) {
                float xMod = (OrgData[i].x - T0) * Mass / TotalMassCorrect + T0;

                //float yMod = (OrgData[i].y - T0) * Mass / M0 + T0;
                float yMod = OrgData[i].y * (Mass / TotalMassCorrect) * (DistanceCorrect / Dist);
                ModData.Add(new Vector2(xMod, yMod));
            }
        }

        public WaveDataFitter Clone() {
            return new WaveDataFitter(this);
        }
    }
}