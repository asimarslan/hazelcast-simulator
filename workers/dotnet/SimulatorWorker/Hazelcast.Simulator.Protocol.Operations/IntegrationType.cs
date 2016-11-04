namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Integration test types for <see cref="IntegrationTestOperation"/>
    /// </summary>
    public enum IntegrationType
    {
        AreEquals,
        NestedSync,
        NestedAsync,
        DeepNestedSync,
        DeepNestedAsync
    }
}