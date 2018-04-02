using System.Collections;

// 组合结点 
public interface ICompositeNode : IBehaviourTreeNode {
    void AddNode(IBehaviourTreeNode node);
    void RemoveNode(IBehaviourTreeNode node);
    bool HasNode(IBehaviourTreeNode node);

    void AddCondition(IConditionNode node);
    void RemoveCondition(IConditionNode node);
    bool HasCondition(IConditionNode node);

    ArrayList nodeList { get; }
    ArrayList conditionList { get; }
}