
// 条件节点
public interface IConditionNode {
    string NodeName { get; set; }
    bool ExternalCondition();
}