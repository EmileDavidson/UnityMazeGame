using System.Linq;
using Toolbox.MethodExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Editor
{
    [CustomEditor(typeof(MazeGenerator), true)]
    public class MazeGeneratorInspector : UnityEditor.Editor
    {
        private MazeGenerator _mazeGenerator;
        private bool _showDefaultEvents = false;

        readonly string[] _excludes = new[]
        {
            "onStartCreatingHexagons",
            "onUpdateCreatingHexagons",
            "onFinishCreatingHexagons",
            "onStartCreatingWalls",
            "onUpdateCreatingWalls",
            "onFinishCreatingWalls",
            "onStartGeneratingMaze",
            "onUpdateGeneratingMaze",
            "onFinishGeneratingMaze"
        };

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Generate"))
            {
                _mazeGenerator.ResetMaze(true);
                _mazeGenerator.StartGeneration();
            }

            if (GUILayout.Button("Reset"))
            {
                _mazeGenerator.ResetMaze(true);
            }
            
            DrawPropertiesExcluding(serializedObject, _excludes);
            serializedObject.ApplyModifiedProperties();

            _showDefaultEvents = EditorGUILayout.Foldout(_showDefaultEvents, "Show Default Events");
            if (_showDefaultEvents)
            {
                foreach (var exclude in _excludes)
                {
                    
                    var propertyRelative = serializedObject.FindProperty(exclude);
                    EditorGUILayout.PropertyField(propertyRelative);
                }
            }
            
            EditorGUI.EndChangeCheck();
        }

        public void OnEnable()
        {
            _mazeGenerator = target as MazeGenerator;
        }
    }
}