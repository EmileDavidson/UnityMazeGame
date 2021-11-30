using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

[System.Serializable]
public class GraphNode : Node
{
    public string DialogueName { get; set; }
    public List<string> Choices { get; set; }
    public string Text { get; set; }
    public DialogueTypes DialogueType { get; set; }

    public void Initialize()
    {
        DialogueName = "Empty Node";
        Choices = new List<string>();
        Text = "Empty Dialogue";
    }

    public void Draw()
    {
        TextField dialogueNameTextField = new TextField(){value = DialogueName};
        titleContainer.Insert(0, dialogueNameTextField);
        
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

        inputPort.portName = "Input";
        outputPort.portName = "Output";
        
        inputContainer.Add(inputPort);
        outputContainer.Add(outputPort);

        VisualElement customDataContainer = new VisualElement();

        Foldout textFoldout = new Foldout()
        {
            text = "Dialogue Text"
        };

        TextField textField = new TextField()
        {
            value = Text
        };
        
        textFoldout.Add(textField);
        
        customDataContainer.Add(textFoldout);
        
        extensionContainer.Add(customDataContainer);
        
        RefreshExpandedState();
        RefreshPorts();
    }
}
