using UnityEngine;

namespace EffectsLab.Sample001
{
    [ExecuteAlways]
    public class HoloDissolveDemo : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private bool animate = true;
        [SerializeField, Min(0.05f)] private float cycleSeconds = 3.0f;
        [SerializeField] private float minDissolve = 0.18f;
        [SerializeField] private float maxDissolve = 0.72f;
        [SerializeField] private float spinDegreesPerSecond = 18f;

        private static readonly int DissolveId = Shader.PropertyToID("_Dissolve");
        private MaterialPropertyBlock block;

        private void OnEnable()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>();

            block = new MaterialPropertyBlock();
            ApplyDissolve(minDissolve);
        }

        private void Update()
        {
            if (targetRenderer == null)
                return;

            if (animate)
            {
                float t = Application.isPlaying ? Time.time : (float)UnityEditorTime.timeSinceStartup;
                float pingPong = Mathf.PingPong(t / Mathf.Max(cycleSeconds, 0.05f), 1f);
                float eased = pingPong * pingPong * (3f - 2f * pingPong);
                ApplyDissolve(Mathf.Lerp(minDissolve, maxDissolve, eased));
            }

            if (Application.isPlaying)
                transform.Rotate(Vector3.up, spinDegreesPerSecond * Time.deltaTime, Space.World);
        }

        private void ApplyDissolve(float value)
        {
            block ??= new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(block);
            block.SetFloat(DissolveId, value);
            targetRenderer.SetPropertyBlock(block);
        }

        private static class UnityEditorTime
        {
#if UNITY_EDITOR
            public static double timeSinceStartup => UnityEditor.EditorApplication.timeSinceStartup;
#else
            public static double timeSinceStartup => Time.realtimeSinceStartupAsDouble;
#endif
        }
    }
}
