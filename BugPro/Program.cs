using Stateless;

namespace BugPro;

public class Bug
{
    public enum State
    {
        New,
        Assigned,
        InProgress,
        Resolved,
        Verified,
        Closed,
        Reopened,
        Deferred,
        Rejected
    }

    public enum Trigger
    {
        Assign,
        StartProgress,
        Resolve,
        Verify,
        Close,
        Reopen,
        ReopenFromClosed,
        Defer,
        Reject
    }

    private readonly StateMachine<State, Trigger> _stateMachine;
    private readonly List<string> _transitionHistory = [];

    public Bug()
    {
        _stateMachine = new StateMachine<State, Trigger>(State.New);
        ConfigureStateMachine();
    }

    public State CurrentState => _stateMachine.State;
    public IReadOnlyList<string> TransitionHistory => _transitionHistory;

    public void Assign() => _stateMachine.Fire(Trigger.Assign);
    public void StartProgress() => _stateMachine.Fire(Trigger.StartProgress);
    public void Resolve() => _stateMachine.Fire(Trigger.Resolve);
    public void Verify() => _stateMachine.Fire(Trigger.Verify);
    public void Close() => _stateMachine.Fire(Trigger.Close);
    public void Reopen() => _stateMachine.Fire(Trigger.Reopen);
    public void ReopenFromClosed() => _stateMachine.Fire(Trigger.ReopenFromClosed);
    public void Defer() => _stateMachine.Fire(Trigger.Defer);
    public void Reject() => _stateMachine.Fire(Trigger.Reject);

    private void ConfigureStateMachine()
    {
        _stateMachine.OnTransitioned(transition =>
        {
            _transitionHistory.Add($"{transition.Source} --{transition.Trigger}--> {transition.Destination}");
        });

        _stateMachine.Configure(State.New)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reject, State.Rejected);

        _stateMachine.Configure(State.Assigned)
            .Permit(Trigger.StartProgress, State.InProgress)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reject, State.Rejected);

        _stateMachine.Configure(State.InProgress)
            .Permit(Trigger.Resolve, State.Resolved)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reject, State.Rejected);

        _stateMachine.Configure(State.Resolved)
            .Permit(Trigger.Verify, State.Verified)
            .Permit(Trigger.Reopen, State.Reopened);

        _stateMachine.Configure(State.Verified)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        _stateMachine.Configure(State.Closed)
            .Permit(Trigger.ReopenFromClosed, State.Reopened);

        _stateMachine.Configure(State.Reopened)
            .Permit(Trigger.StartProgress, State.InProgress)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reject, State.Rejected);

        _stateMachine.Configure(State.Deferred)
            .Permit(Trigger.StartProgress, State.InProgress)
            .Permit(Trigger.Reject, State.Rejected);

        _stateMachine.Configure(State.Rejected)
            .Permit(Trigger.Reopen, State.Reopened);
    }
}

public static class Program
{
    public static void Main()
    {
        var bug = new Bug();

        Console.WriteLine($"Initial state: {bug.CurrentState}");

        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        bug.ReopenFromClosed();

        Console.WriteLine($"Current state: {bug.CurrentState}");
        Console.WriteLine("Transition history:");

        foreach (var transition in bug.TransitionHistory)
        {
            Console.WriteLine($"- {transition}");
        }
    }
}
