using System.Collections.Generic;
using UnityEngine;

namespace Game.Tasks {
    public interface IWaveData {
        string ID { get; }
        float MaxX { get; }
        float MaxY { get; }
        float MinX { get; }
        float MinY { get; }
        List<Vector2> ModData { get; }
        string Name { get; }
        List<Vector2> OrgData { get; }
        float Scale { get; }
        List<float> Xs { get; }
        List<float> Ys { get; }

        List<Vector2> listToVector(List<List<float>> orig);
        void SetID(string value);
        void SetName(string value);
    }
}