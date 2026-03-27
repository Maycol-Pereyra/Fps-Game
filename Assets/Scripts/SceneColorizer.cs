using UnityEngine;

public class SceneColorizer : MonoBehaviour
{
    private void Awake()
    {
        ColorByNamePrefix("Target_Close", new Color(1f, 0.15f, 0.15f));
        ColorByNamePrefix("Target_Mid", new Color(1f, 0.5f, 0.1f));
        ColorByNamePrefix("Target_Far", new Color(1f, 0.9f, 0.1f));
        ColorByNamePrefix("Enemy_", new Color(0.2f, 0.9f, 0.2f));
    }

    private void ColorByNamePrefix(string prefix, Color color)
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith(prefix))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }
    }
}
