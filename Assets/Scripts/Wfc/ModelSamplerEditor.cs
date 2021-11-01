using System.Linq;
using UnityEditor;

[CustomEditor(typeof(ModelSampler))]
public class ModelSamplerEditor : Editor {

    ModelSampler sampler;
    string modelName = "";

    private void OnEnable() {
        sampler = target as ModelSampler;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        modelName = EditorGUILayout.TextField("Model Name", modelName);
        EditorUtils.guiButton("Create Simple Tiled Model", () => {
            var constraints = sampler.run();
            WfcModel model = new WfcModel(constraints.ToList());
            model.saveToFile(modelName);
        });
    }
}