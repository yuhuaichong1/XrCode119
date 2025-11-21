using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeLineSC", menuName = "SUI/TimeLineSC", order = 1)]
public class TimeLineSC : ScriptableObject
{
    public NormalTimePoint NormalTimePoint;
    public InteractTimePoint InteractTimePoint;
    public UnknownTimePoint UnknownTimePoint;
}
