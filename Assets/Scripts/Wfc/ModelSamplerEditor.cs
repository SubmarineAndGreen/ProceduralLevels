using UnityEditor;

[CustomEditor(typeof(ModelSampler))]
public class ModelSamplerEditor : Editor {

    ModelSampler sampler;

    private void OnEnable() {
        sampler = target as ModelSampler;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorUtils.guiButton("Create Simple Tiled Model", sampler.run);
    }
}