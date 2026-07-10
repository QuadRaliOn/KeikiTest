#if UNITY_EDITOR
using GamePlay;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(LevelPathEditor))]
    public class LevelPathEditorInspector : UnityEditor.Editor {
        private LevelPathEditor _editor;

        private void OnEnable() {
            _editor = (LevelPathEditor)target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            GUILayout.Space(10);

            CreateLoadPointsButton();
            CreateSavePointsButton();

            GUILayout.Space(10);
            GUILayout.Label("Scene view controls:", EditorStyles.boldLabel);
            GUILayout.Label("Hold Shift + Left Click in Scene View to add a point.");
            GUILayout.Label("Drag handles to reposition points.");

            GUILayout.Space(5);

            CreateDistributePointsButton();
            CreateClearPointsButton();
            CreateRemovePointButton();
        }

        private void OnSceneGUI() {
            CreatePoint();
            DrawLine();
            DrawLabels();
        }

        private void CreatePoint() {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && e.shift) {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(-_editor.transform.forward, _editor.transform.position);

                if (plane.Raycast(ray, out float enter)) {
                    Vector3 worldPos = ray.GetPoint(enter);
                    Vector2 localPos = _editor.transform.InverseTransformPoint(worldPos);

                    Undo.RecordObject(_editor, "Add Path Point");
                    _editor.points.Add(localPos);
                    EditorUtility.SetDirty(_editor);

                    e.Use();
                    Repaint();
                }
            }
        }

        private void CreateLoadPointsButton() {
            if (GUILayout.Button("Load Points from Database")) {
                _editor.LoadFromDatabase();
            }
        }

        private void CreateSavePointsButton() {
            if (GUILayout.Button("Save Points to Database")) {
                _editor.SaveToDatabase();
            }
        }

        private void CreateDistributePointsButton() {
            if (_editor.points.Count > 2) {
                if (GUILayout.Button("Distribute Points Evenly")) {
                    Undo.RecordObject(_editor, "Distribute Points Evenly");
                    Vector2 start = _editor.points[0];
                    Vector2 end = _editor.points[_editor.points.Count - 1];
                    int count = _editor.points.Count;
                    for (int i = 1; i < count - 1; i++) {
                        float t = (float)i / (count - 1);
                        _editor.points[i] = Vector2.Lerp(start, end, t);
                    }
                    EditorUtility.SetDirty(_editor);
                }
            }
        }

        private void CreateClearPointsButton() {
            if (GUILayout.Button("Clear Points")) {
                Undo.RecordObject(_editor, "Clear Points");
                _editor.points.Clear();
                EditorUtility.SetDirty(_editor);
            }
        }

        private void CreateRemovePointButton() {
            if (GUILayout.Button("Remove Last Point")) {
                if (_editor.points.Count > 0) {
                    Undo.RecordObject(_editor, "Remove Last Point");
                    _editor.points.RemoveAt(_editor.points.Count - 1);
                    EditorUtility.SetDirty(_editor);
                }
            }
        }

        private void DrawLine() {
            Handles.color = Color.yellow;
            for (int i = 0; i < _editor.points.Count - 1; i++) {
                Vector3 p1 = _editor.transform.TransformPoint(_editor.points[i]);
                Vector3 p2 = _editor.transform.TransformPoint(_editor.points[i + 1]);
                Handles.DrawLine(p1, p2, 4f);
            }
        }

        private void DrawLabels() {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.black;
            labelStyle.fontSize = 10;

            for (int i = 0; i < _editor.points.Count; i++) {
                Vector3 worldPos = _editor.transform.TransformPoint(_editor.points[i]);

                if (i == 0) {
                    Handles.color = Color.cyan;
                } else if (i == _editor.points.Count - 1) {
                    Handles.color = Color.red;
                } else {
                    Handles.color = Color.green;
                }

                Handles.DrawSolidDisc(worldPos, _editor.transform.forward, 0.08f);
                Handles.color = Color.white;
                Handles.Label(worldPos + new Vector3(0.15f, 0.15f, 0f), $"Point {i}", labelStyle);

                EditorGUI.BeginChangeCheck();

                Vector3 newWorldPos = Handles.FreeMoveHandle(worldPos, 0.15f, Vector3.zero, Handles.CircleHandleCap);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_editor, "Move Path Point");
                    _editor.points[i] = _editor.transform.InverseTransformPoint(newWorldPos);
                    EditorUtility.SetDirty(_editor);
                    Repaint();
                }
            }
        }
    }
}
#endif