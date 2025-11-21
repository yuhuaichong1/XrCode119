using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateTimeline
{
    private static string dataPath = "Assets/SCustomUI/TimeLineSC.asset";

    [MenuItem("GameObject/UI/SUI/TimeLine", false, 10)]
    private static void TimeLine()
    {
        Transform trans = Selection.activeTransform;
        if (trans != null)
        {
            TimeLineSC timeLineSC = AssetDatabase.LoadAssetAtPath<TimeLineSC>(dataPath);
            if (timeLineSC != null)
            {
                if (timeLineSC.NormalTimePoint == null)
                {
                    Debug.LogError("NormalTimePoint not found");
                    return;
                }
                if (timeLineSC.InteractTimePoint == null)
                {
                    Debug.LogError("InteractTimePoint not found");
                    return;
                }
                if (timeLineSC.UnknownTimePoint == null)
                {
                    Debug.LogError("UnknownTimePoint not found");
                    return;
                }

                Create(trans, timeLineSC);
            }
            else
                Debug.LogError("TimeLineSC not found, please Create one in 'Assets/SCustomUI/TimeLineSC.asset'");
        }
    }

    private static void Create(Transform trans, TimeLineSC timeLineSC)
    {
        NormalTimePoint normalTimePoint = timeLineSC.NormalTimePoint;
        InteractTimePoint interactTimePoint = timeLineSC.InteractTimePoint;
        UnknownTimePoint unknownTimePoint = timeLineSC.UnknownTimePoint;


    }
}
