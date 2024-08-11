using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[System.Serializable]
public class PID {
	public float pFactor, iFactor, dFactor, min, max;
		
	float integral;
	float lastError;
	
	
	public PID(float pFactor, float iFactor, float dFactor, float min, float max) {
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
		this.min = min;
		this.max = max;
	}
	
	
	public float Update(float setpoint, float actual, float timeFrame) {
		float present = setpoint - actual;
		integral += present * timeFrame;
		integral = Mathf.Clamp(integral, min, max);
		float deriv = (present - lastError) / timeFrame;
		lastError = present;
		float output = present * pFactor + integral * iFactor + deriv * dFactor;
		return Mathf.Clamp(output, min, max);
	}
}
