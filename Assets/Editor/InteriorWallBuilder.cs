
using UnityEngine;
using UnityEditor;

public class InteriorWallBuilder
{
    [MenuItem("Tools/Build Interior Walls")]
    public static void BuildInteriorWalls()
    {
        var obstacles = GameObject.Find("--- Obstacles ---")?.transform;
        if (obstacles == null) { Debug.LogError("--- Obstacles --- group not found!"); return; }

        // Load wall material from pack
        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Infima Games/Low Poly Shooter Pack - Free Sample/Art/Materials/Environment/M_Grid_Wall_4x5.mat");

        int count = 0;

        // Wall 1: Left side cover wall at z=25, partial wall
        count += CreateWall("Wall_Cover_Left", new Vector3(-12, 2.5f, 25), new Vector3(8, 5, 0.5f), obstacles, wallMat);

        // Wall 2: Right side cover wall at z=25
        count += CreateWall("Wall_Cover_Right", new Vector3(12, 2.5f, 25), new Vector3(8, 5, 0.5f), obstacles, wallMat);

        // Wall 3: Center low wall at z=35 (half-height for taking cover)
        count += CreateWall("Wall_Cover_Center", new Vector3(0, 1.25f, 35), new Vector3(10, 2.5f, 0.5f), obstacles, wallMat);

        // Wall 4: Left angled barrier at z=45
        count += CreateWall("Wall_Barrier_Left", new Vector3(-8, 2.5f, 45), new Vector3(6, 5, 0.5f), obstacles, wallMat);

        // Wall 5: Right angled barrier at z=45
        count += CreateWall("Wall_Barrier_Right", new Vector3(8, 2.5f, 45), new Vector3(6, 5, 0.5f), obstacles, wallMat);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("InteriorWallBuilder: Created " + count + " interior walls.");
    }

    static int CreateWall(string name, Vector3 position, Vector3 scale, Transform parent, Material mat)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.SetParent(parent);
        wall.isStatic = true;

        if (mat != null)
        {
            var renderer = wall.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = mat;
        }

        Undo.RegisterCreatedObjectUndo(wall, "Create " + name);
        return 1;
    }
}
