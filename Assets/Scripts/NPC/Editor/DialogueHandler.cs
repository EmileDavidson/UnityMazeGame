using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NPC.Editor
{
    [System.Serializable]
    public class DialogueHandler : GraphView
    {
        public DialogueHandler()
        {
            AddManipulators();
            CreateNode();
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(CreateNodeContextMenu());
        }

        private void CreateNode()
        {
            GraphNode node = new GraphNode();
        
            node.Initialize();
            node.Draw();
        
            AddElement(node);
        }
    
    
        private IManipulator CreateNodeContextMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => CreateNode())
            );
            return contextualMenuManipulator;
        }
    }

    public class DialogueEditor : EditorWindow
    {
    
        [MenuItem("Window/Dialogue")]
        public static void OpenEditor()
        {
            GetWindow<DialogueEditor>("Dialogue Editor");
        }

        private void OnEnable()
        {
            AddDialogueView();
        }

        private void AddDialogueView()
        {
            DialogueHandler graphView = new DialogueHandler();
        
            graphView.StretchToParentSize();
        
            rootVisualElement.Add(graphView);
        }
    }
}