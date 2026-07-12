using UnityEngine;

[CreateAssetMenu(fileName = "WaveGroupData", menuName = "Scriptable Objects/WaveGroupData")]
public class WaveGroupData : ScriptableObject
{
	[Header("Wave Sequence")]
	public WaveData[] waves;

	[Header("Flow")]
	[Min(0f)] public float delayBetweenWaves = 2f;
}
