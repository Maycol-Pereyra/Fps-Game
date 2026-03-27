
using UnityEngine;
using UnityEditor;

public class SceneOrganizer
{
    [MenuItem("Tools/Organize Scene")]
    public static void OrganizeScene()
    {
        var targets = GameObject.Find("--- Targets ---")?.transform;
        var explosives = GameObject.Find("--- Explosives ---")?.transform;
        var enemies = GameObject.Find("--- Enemies ---")?.transform;
        var environment = GameObject.Find("--- Environment ---")?.transform;
        var obstacles = GameObject.Find("--- Obstacles ---")?.transform;

        int moved = 0;

        int[] targetIds = { -4316, -4338, -4360, -4382, -4404, -4426, -4448, -4470, -4492, -5210, -5254 };
        foreach (int id in targetIds)
        {
            var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (obj != null && targets != null) { Undo.SetTransformParent(obj.transform, targets, "Reparent"); moved++; }
        }

        int[] explosiveIds = { -5064, -5086, -5110, -5160 };
        foreach (int id in explosiveIds)
        {
            var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (obj != null && explosives != null) { Undo.SetTransformParent(obj.transform, explosives, "Reparent"); moved++; }
        }

        int[] enemyIds = { -4542, -4568, -4594 };
        foreach (int id in enemyIds)
        {
            var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (obj != null && enemies != null) { Undo.SetTransformParent(obj.transform, enemies, "Reparent"); moved++; }
        }

        int[] envIds = { -3292, -3310, -3328, -3422, -19022, -19040 };
        foreach (int id in envIds)
        {
            var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (obj != null && environment != null) { Undo.SetTransformParent(obj.transform, environment, "Reparent"); moved++; }
        }

        int[] obstacleIds = { -5296, -5314, -5332 };
        foreach (int id in obstacleIds)
        {
            var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (obj != null && obstacles != null) { Undo.SetTransformParent(obj.transform, obstacles, "Reparent"); moved++; }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("SceneOrganizer: Moved " + moved + " objects into groups.");
    }
}
