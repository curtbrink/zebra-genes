namespace Csp.Builders.Quiz;

public abstract class SelfRefBuilder;

public abstract class SelfRefBuilder<TSelf> : SelfRefBuilder where TSelf : SelfRefBuilder;
