using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using System.Linq;
using UnityEditor.Rendering;
using Unity.Mathematics;
using System;

public class ObjectDetectionYoloV8 : MonoBehaviour
{
    [SerializeField] NNModel modelAsset;
    [SerializeField] RenderTexture cameraView;
    [SerializeField] float confidenceThreshold = 0.5f;
    [SerializeField] float overlapThreshold = 0.45f;
    [SerializeField]
    List<String> labels = new()
    {
        "Blue flare",
        "Blue pail",
        "Cloth",
        "Gate",
        "Red flare",
        "Red pail",
        "Yellow flare"
    };
    [SerializeField] GUIStyle boundingBoxStyle;

    int channelCount = 3;
    Model RuntimeModel;
    IWorker Worker;
    List<YoloResult> results = new List<YoloResult>();

    public void Start()
    {
        RuntimeModel = ModelLoader.Load(modelAsset);
        Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, RuntimeModel);
        StartDetection();
    }

    async Task StartDetection()
    {
        while (true)
        {
            // Inference
            Tensor inputTensor = new Tensor(cameraView, channelCount);
            Tensor outputTensor = await ForwardAsync(Worker, inputTensor);
            await Task.Delay(32);

            inputTensor.Dispose();

            // Start post processing
            var selectedBoxes = new List<YoloResult>();

            for (int boxIndex = 0; boxIndex < outputTensor.width; boxIndex++)
            {
                // Find the class with greatest confidence and only select those with high enough confidence
                var classProbabilities = new List<float>();
                for (int i = 4; i < outputTensor.channels; i++) classProbabilities.Add(outputTensor[0, 0, boxIndex, i]);

                int classId = classProbabilities.IndexOf(classProbabilities.Max());
                float classProbability = classProbabilities[classId];
                if (classProbability < confidenceThreshold) continue;

                float x = outputTensor[0, 0, boxIndex, 0];
                float y = outputTensor[0, 0, boxIndex, 1];
                float w = outputTensor[0, 0, boxIndex, 2];
                float h = outputTensor[0, 0, boxIndex, 3];

                float x1 = x - w / 2;
                float y1 = y - h / 2;
                float x2 = x + w / 2;
                float y2 = y + h / 2;

                // if (w < 5 || h < 5) continue;
                //if (x2 < 50 && y2 < 50) continue;


                var selectedBox = new YoloResult(x1, y1, x2, y2, classProbability, classId);
                selectedBoxes.Add(selectedBox);
            }
            await Task.Delay(32);

            // NMS
            selectedBoxes.Sort((a, b) => b.confidence.CompareTo(a.confidence));
            while (selectedBoxes.Count > 0)
            {
                var currentBox = selectedBoxes[0];
                results.Add(currentBox);
                selectedBoxes.RemoveAt(0);

                for (var i = 0; i < selectedBoxes.Count; i++)
                {
                    var otherBox = selectedBoxes[i];
                    var overlap = computeIOU(currentBox, otherBox);
                    if (overlap > overlapThreshold)
                    {
                        // remove the box if it has a high overlap with the current box  
                        selectedBoxes.RemoveAt(i);
                        i--;
                    }
                }
            }
            //if (results.Count > 0) LogYoloResult(results[0]);
            await Task.Delay(200);

            Debug.Log(results.Count);
            results.Clear();
        }
    }


    float computeIOU(YoloResult box1, YoloResult box2)
    {
        float b1x1, b1y1, b1x2, b1y2;
        (b1x1, b1y1, b1x2, b1y2) = (box1.x1, box1.y1, box1.x2, box1.y2);

        float b2x1, b2y1, b2x2, b2y2;
        (b2x1, b2y1, b2x2, b2y2) = (box2.x1, box2.y1, box2.x2, box2.y2);

        float interRectX1 = Mathf.Max(b1x1, b2x1);
        float interRectY1 = Mathf.Max(b1y1, b2y1);
        float interRectX2 = Mathf.Min(b1x2, b2x2);
        float interRectY2 = Mathf.Min(b1y2, b2y2);

        float interArea = Mathf.Max(0, interRectX2 - interRectX1 + 1) * Mathf.Max(0, interRectY2 - interRectY1 + 1);
        float b1Area = (b1x2 - b1x1 + 1) * (b1y2 - b1y1 + 1);
        float b2Area = (b2x2 - b2x1 + 1) * (b2y2 - b2y1 + 1);
        float unionArea = b1Area + b2Area - interArea + 1E-10f;
        return interArea / unionArea;
    }

    public async Task<Tensor> ForwardAsync(IWorker modelWorker, Tensor inputs)
    {
        // Schedule inference
        var executor = modelWorker.StartManualSchedule(inputs);
        var it = 0;
        bool hasMoreWork;
        do
        {
            hasMoreWork = executor.MoveNext();
            if (++it % 20 == 0)
            {
                modelWorker.FlushSchedule();
                await Task.Delay(5);
            }
        } while (hasMoreWork);

        return modelWorker.PeekOutput();
    }

    public void OnGUI()
    {
        foreach (YoloResult boundingBox in results)
        {
            float x = boundingBox.x1;
            float y = boundingBox.y1;
            float w = boundingBox.x2 - boundingBox.x1 + 1;
            float h = boundingBox.y2 - boundingBox.y1 + 1;
            GUI.Box(new Rect(x, y, w, h), labels[boundingBox.classId], boundingBoxStyle);
        }
    }

    void LogYoloResult(YoloResult result)
    {
        Debug.Log($"Class: {labels[result.classId]}; Confidence: {result.confidence}, x1: {result.x1}, x2: {result.x2}, y1: {result.y1}, y2: {result.y2}");
    }

    public void OnDestroy()
    {
        Worker?.Dispose();
    }

}