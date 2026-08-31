#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace EffectsLab.Sample001.Editor
{
    public static class CreateHoloDissolveDemo
    {
        private const string Root = "Assets/EffectsLab/Sample001_HoloDissolve";
        private const string Generated = Root + "/Generated";
        private const string MaterialPath = Generated + "/HoloDissolveCard_Demo.mat";
        private const string ScenePath = Generated + "/HoloDissolveCard_Demo.unity";

        [MenuItem("Realtime Effects Lab/Sample 001/Create Holo Dissolve Demo")]
        public static void CreateDemo()
        {
            EnsureFolder(Generated);

            Shader shader = Shader.Find("EffectsLab/HoloDissolveCard");
            if (shader == null)
            {
                Debug.LogError("EffectsLab/HoloDissolveCard shader was not found. Let Unity finish importing, then try again.");
                return;
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                material = new Material(shader)
                {
                    name = "HoloDissolveCard_Demo"
                };
                material.SetColor("_BaseColor", new Color(0.025f, 0.055f, 0.11f, 0.96f));
                material.SetColor("_HoloColor", new Color(0.16f, 0.95f, 1.0f, 1f));
                material.SetColor("_EdgeColor", new Color(1.0f, 0.18f, 0.65f, 1f));
                material.SetFloat("_Emission", 2.6f);
                material.SetFloat("_NoiseScale", 8.0f);
                material.SetFloat("_EdgeWidth", 0.06f);
                AssetDatabase.CreateAsset(material, MaterialPath);
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "HoloDissolveCard_Demo";

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0.25f, -6.4f);
            cameraObject.transform.rotation = Quaternion.Euler(2f, 0f, 0f);
            camera.fieldOfView = 38f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.006f, 0.009f, 0.018f, 1f);

            GameObject card = GameObject.CreatePrimitive(PrimitiveType.Cube);
            card.name = "Holographic Dissolve Card";
            card.transform.position = new Vector3(0f, 0.15f, 0f);
            card.transform.rotation = Quaternion.Euler(-4f, -18f, 2f);
            card.transform.localScale = new Vector3(2.4f, 3.4f, 0.08f);
            Renderer renderer = card.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            HoloDissolveDemo demo = card.AddComponent<HoloDissolveDemo>();
            SerializedObject serializedDemo = new SerializedObject(demo);
            serializedDemo.FindProperty("targetRenderer").objectReferenceValue = renderer;
            serializedDemo.FindProperty("cycleSeconds").floatValue = 2.8f;
            serializedDemo.FindProperty("spinDegreesPerSecond").floatValue = 14f;
            serializedDemo.ApplyModifiedPropertiesWithoutUndo();

            GameObject rim = new GameObject("Soft Rim Light");
            Light rimLight = rim.AddComponent<Light>();
            rimLight.type = LightType.Point;
            rimLight.range = 8f;
            rimLight.intensity = 4f;
            rimLight.color = new Color(0.2f, 0.55f, 1f);
            rim.transform.position = new Vector3(-2.6f, 2.2f, -1.8f);

            GameObject fill = new GameObject("Warm Fill Light");
            Light fillLight = fill.AddComponent<Light>();
            fillLight.type = LightType.Point;
            fillLight.range = 8f;
            fillLight.intensity = 2.5f;
            fillLight.color = new Color(1f, 0.2f, 0.55f);
            fill.transform.position = new Vector3(2.2f, -1.2f, -0.8f);

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Ground";
            floor.transform.position = new Vector3(0f, -2.0f, 1f);
            floor.transform.localScale = new Vector3(1.5f, 1f, 1.5f);
            Material floorMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            floorMaterial.name = "DemoGround_Runtime";
            floorMaterial.SetColor("_BaseColor", new Color(0.012f, 0.016f, 0.028f, 1f));
            floor.GetComponent<Renderer>().sharedMaterial = floorMaterial;

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.035f, 0.045f, 0.075f);

            Selection.activeGameObject = card;
            SceneView.lastActiveSceneView?.FrameSelected();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Realtime Effects Lab Sample 001 created: {ScenePath}. Press Play to preview the looping dissolve and card rotation.");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
