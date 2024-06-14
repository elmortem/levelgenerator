using System.Linq;
using LevelGenerator.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor
{
    [CustomNodeEditor(typeof(SplineNode))]
    public class SplineNodeEditor : NodeEditor
    {
        private static GameObject _editContainerObject;
        private static SplineContainer _editContainer;
        private static SplineNode _editNode;
        
        private SplineNode _node;

        public override void OnBodyGUI()
        {
            if(_node == null)
                _node = (SplineNode)target;
            
            if (_editNode != _node && GUILayout.Button("Start Edit"))
            {
                StartEdit(_node);
            }
            
            if (_editNode != null && GUILayout.Button("Stop Edit"))
            {
                StopEdit();
            }

            base.OnBodyGUI();
            
            NodeEditorGUI.DrawPreviewAndLock(_node);
        }

        private void StartEdit(SplineNode node)
        {
            StopEdit();
            
            if(node == null)
                return;
            
            _editNode = node;
            
            _editContainerObject = new GameObject("_SplineContainer");
            _editContainer = _editContainerObject.AddComponent<SplineContainer>();

            if (_editNode.Result != null)
            {
                _editContainer.Splines = _editNode.Result.Splines;
                //TODO _editContainer.KnotLinkCollection = _editNode.Result.Knots;
            }
            
            _editNode.SetEditContainer(_editContainer.transform);

            Selection.activeGameObject = _editContainerObject;
        }

        private void StopEdit()
        {
            if (_editNode != null && _editContainer != null)
            {
                _editNode.SetData(_editContainer.Splines, _editContainer.KnotLinkCollection);
                _editNode.SetEditContainer(null);
            }

            if (_editContainerObject != null)
            {
                GameObject.DestroyImmediate(_editContainerObject);
                _editContainerObject = null;
            }

            _editContainer = null;
            _editNode = null;
        }
    }
}
