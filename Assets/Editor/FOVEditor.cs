using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAI))]

public class NewBehaviourScript : Editor
{
    private void OnSceneGUI()
    {
        EnemyAI fov = (EnemyAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position + new Vector3(0,fov.headOffset,0), Vector3.up, Vector3.forward, 360, fov.visRange);

        Vector3 viewAngle1 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.visAngle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.visAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position + new Vector3 (0, fov.headOffset, 0), fov.transform.position + new Vector3 (0, fov.headOffset, 0) + viewAngle1 * fov.visRange);
        Handles.DrawLine(fov.transform.position + new Vector3 (0, fov.headOffset, 0), fov.transform.position + new Vector3 (0, fov.headOffset, 0) + viewAngle2* fov.visRange);

        if(fov.playerDetected)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position + new Vector3(0, fov.headOffset, 0), fov.player.transform.position);
        }
        else if(fov.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position + new Vector3(0, fov.headOffset, 0), fov.player.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
