
public interface IBehaviourTreeNode {
    RunStatus Status { get; set; }  
    string NodeName { get; set; }  
    bool Enter(object input);  
    bool Leave(object input);  
    bool Tick(object input, object output);  
    //RenderableNode RenderNode { get; set; }  
    IBehaviourTreeNode Parent { get; set; }  
    IBehaviourTreeNode Clone();  
}
