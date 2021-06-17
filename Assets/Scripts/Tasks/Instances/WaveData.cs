using Game.Network;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Tasks {
    public class WaveData : IServerData {

        public float Scale { get; protected set; }
        public List<Vector2> OrgData { get; protected set; }
        public List<Vector2> ModData { get; protected set; }

        public List<float> Xs => ModData.Select(x => x.x).ToList();
        public List<float> Ys => ModData.Select(x => x.y).ToList();

        public float MinX => Xs.Min();
        public float MaxX => Xs.Max();
        public float MinY => Ys.Min();
        public float MaxY => Ys.Max();

        public string Name { get; protected set; }
        public string ID { get; protected set; }

        [JsonConstructor]
        public WaveData(List<List<float>> data) {
            OrgData = new List<Vector2>();
            OrgData = listToVector(data);
            ModData = listToVector(data);
        }

        public WaveData(List<Vector2> data) {
            OrgData = data.Select(x => new Vector2(x.x,x.y)).ToList();
        }

        public List<Vector2> listToVector(List<List<float>> orig) {
            List<Vector2> list = new List<Vector2>();
            int count = 0;
            //int divisable = orig.Count / 2000;
            foreach (List<float> coords in orig) {
                //if (coords.Count > 1 && (count % divisable == 0)) {
                if (coords.Count > 1) {
                    list.Add(new Vector2(coords[0], coords[1]));
                }
                count++;
            }
            return list;
        }

        public void SetName(string value) {
            Name = value;
        }
        public void SetID(string value) {
            ID = value;
        }
    }
}