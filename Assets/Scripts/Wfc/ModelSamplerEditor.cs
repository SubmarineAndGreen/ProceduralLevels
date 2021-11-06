using System.Linq;
using UnityEditor;

[CustomEditor(typeof(ModelSampler))]
public class ModelSamplerEditor : Editor {

    ModelSampler sampler;

    private void OnEnable() {
        sampler = target as ModelSampler;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        sampler.currentModelName = EditorGUILayout.TextField("Model Name", sampler.currentModelName);
        EditorUtils.guiButton("Create Simple Tiled Model", () => {
            SimpleTiledModel model = sampler.run();
            model.saveToFile(sampler.currentModelName);
        });
    }
}