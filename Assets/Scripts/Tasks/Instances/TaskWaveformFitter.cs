using Game.Managers;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts;

namespace Game.Tasks {
    public class TaskWaveformFitter : MonoBehaviour, ITaskPrefab {

        public Button CompleteBtn { get; private set; }
        public Toggle CheckBox { get; private set; }
        public TaskWindow Parent { get; private set; }
        public LineChart LineChart { get; private set; }

        public WaveData ObvservedWave { get; private set; }
        public WaveDataFitter PredictedWave { get; private set; }

        public bool Ready { get; private set; }
        public GameObject TotalMassContainer { get; private set; }
        public Slider TotalMassSlider { get; private set; }
        public TextMeshProUGUI TotalMassLabel { get; private set; }
        public GameObject DistanceContainer { get; private set; }
        public Slider DistanceSlider { get; private set; }
        public TextMeshProUGUI DistanceLabel { get; private set; }

        public float TotalMassCorrect => PredictedWave.TotalMassCorrect;
        public float TotalMassMin => PredictedWave.TotalMassMin;
        public float TotalMassMax => PredictedWave.TotalMassMax;
        public float TotalMassStartMin => PredictedWave.TotalMassStartMin;
        public float TotalMassStartMax => PredictedWave.TotalMassStartMax;

        public float DistanceCorrect => PredictedWave.DistanceCorrect;
        public float DistanceMin => PredictedWave.DistanceMin;
        public float DistanceMax => PredictedWave.DistanceMax;
        public float DistanceStartMin => PredictedWave.DistanceStartMin;
        public float DistanceStartMax => PredictedWave.DistanceStartMax;

        public float WinConditionPercent => PredictedWave.WinConditionPercent;

        private void OnEnable() {
            if (!Ready) {
                TotalMassContainer = transform.Find("Interaction Area").Find("Total Mass").gameObject;
                TotalMassSlider = TotalMassContainer.GetComponentInChildren<Slider>();
                TotalMassLabel = TotalMassContainer.transform.Find("Label").GetComponentInChildren<TextMeshProUGUI>();

                DistanceContainer = transform.Find("Interaction Area").Find("Distance").gameObject;
                DistanceSlider = DistanceContainer.GetComponentInChildren<Slider>();
                DistanceLabel = DistanceContainer.transform.Find("Label").GetComponentInChildren<TextMeshProUGUI>();

                CompleteBtn = transform.Find("Button Area").Find("Complete Task Button").GetComponent<Button>();
                LineChart = transform.Find("Graph").GetComponentInChildren<LineChart>();
                Ready = true;
            }
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnDestroy() {
            CompleteBtn.onClick.RemoveAllListeners();
            TotalMassSlider.onValueChanged.RemoveAllListeners();
            DistanceSlider.onValueChanged.RemoveAllListeners();
        }

        public void SetParent(TaskWindow parent) {
            Parent = parent;
            CompleteBtn.onClick.AddListener(CompleteTask);
            StartTask();
        }

        public void StartTask() {
            Debug.Log("Starting Task");

            RectTransform rect = Parent.transform.GetComponent<RectTransform>();
            float left, right, top, bottom;
            left = 100;
            right = -100;
            bottom = 100;
            top = -100;

            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(right, top);

            Parent.Task.Started();

            //ObvservedWave = new WaveData(AssetManager.JSON<List<List<float>>>("dataHanford"));
            //PredictedWave = new WaveDataFitter(AssetManager.JSON<List<List<float>>>("NRsim"), TotalMassCorrect, DistanceCorrect, initalTotalMass, initalDistance);
            ObvservedWave = AssetManager.Wave("dataHanford.json");
            WaveDataFitter PredictedWaveFitter = AssetManager.Wave("NRsim.json") as WaveDataFitter;
            PredictedWave = PredictedWaveFitter.Clone();


            float initalTotalMass = UnityEngine.Random.Range(TotalMassStartMin, TotalMassStartMax);
            float initalDistance = UnityEngine.Random.Range(DistanceStartMin, DistanceStartMax);

            initSliders(initalTotalMass, initalDistance);

            UpdateLineOnGraph(ObvservedWave, "Data");
            UpdateLineOnGraph(PredictedWave, "Predicted");

            RefreshChart();

            TotalMassSlider.onValueChanged.AddListener(SliderChanged);
            DistanceSlider.onValueChanged.AddListener(SliderChanged);

        }

        public void initSliders(float initalTotalMass, float initalDistance) {
            TotalMassSlider.minValue = TotalMassMin;
            TotalMassSlider.maxValue = TotalMassMax;
            TotalMassSlider.value = initalTotalMass;

            DistanceSlider.minValue = DistanceMin;
            DistanceSlider.maxValue = DistanceMax;
            DistanceSlider.value = initalDistance;
        }

        public void UpdateLineOnGraph(WaveData wave, string name) {
            Debug.LogFormat("UpdateLineOnGraph wave: {0}", name);
            Serie line;
            if (LineChart.series.Contains(name)) {
                line = LineChart.series.GetSerie(name);
                line.ClearData();
            } else {
                line = LineChart.AddSerie(SerieType.Line, name);
                line.clip = true;
            }


            foreach (Vector2 coords in wave.ModData) {
                line.AddXYData(coords.x, coords.y);
            }
            updateMaxMin();
            //LineChart.RefreshChart();
        }



        public void updateMaxMin() {
            LineChart.xAxis0.min = ObvservedWave.MinX; //(float)Math.Round(ObvservedWave.MinX, 2);
            LineChart.xAxis0.max = ObvservedWave.MaxX; //(float)Math.Round(ObvservedWave.MaxX, 2);
            LineChart.yAxis0.min = ObvservedWave.MinY * 2; //(float)Math.Round(ObvservedWave.MinY, 0, MidpointRounding.AwayFromZero);
            LineChart.yAxis0.max = ObvservedWave.MaxY * 2; //(float)Math.Round(ObvservedWave.MaxY*2, 0, MidpointRounding.AwayFromZero);
            LineChart.RefreshChart();
        }

        public void RefreshChart() {
            PredictedWave.scale(TotalMassSlider.value, DistanceSlider.value);
            UpdateLineOnGraph(PredictedWave, "Predicted");
        }
        public bool WinCondition() {
            float massRange = TotalMassMax - TotalMassMin;
            float massOffset = massRange * (WinConditionPercent / 100);

            float distanceRange = DistanceMax - DistanceMin;
            float distanceOffset = distanceRange * (WinConditionPercent / 100);

            bool massCorrect = TotalMassSlider.value >= TotalMassCorrect - massOffset && TotalMassSlider.value <= TotalMassCorrect + massOffset;
            bool distanceCorrect = DistanceSlider.value >= DistanceCorrect - distanceOffset && DistanceSlider.value <= DistanceCorrect + distanceOffset;
            Debug.LogFormat("mass win range: +/-{0}, guess: {1}, correct: {2}, distance win range: +/-{3}, guess:{4}, correct: {5}", massOffset, TotalMassSlider.value, massCorrect, DistanceSlider.value, distanceOffset, distanceCorrect);
            return massCorrect && distanceCorrect;
        }

        public void CompleteTask() {
            Debug.LogFormat("Complete button clicked for Task {0}", Parent.Task.Name);

            if (WinCondition()) { //condition
                Parent.CompleteTask();
            }
        }

        public void SliderChanged(float change) {
            RefreshChart();
        }
    }
}