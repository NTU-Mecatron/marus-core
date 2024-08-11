using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoloResult
{
    public float x1 { get; set; }
    public float x2 { get; set; }
    public float y1 { get; set; }
    public float y2 { get; set; }
    public float confidence { get; set; }
    public int classId { get; set; }

    public YoloResult(float x1, float y1, float x2, float y2, float confidence, int classId)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.confidence = confidence;
        this.classId = classId;
    }
}
