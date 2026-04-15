using BugPro;

namespace BugTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void InitialState_ShouldBeNew()
    {
        var bug = new Bug();
        Assert.AreEqual(Bug.State.New, bug.CurrentState);
    }

    [TestMethod]
    public void New_Assign_ShouldMoveToAssigned()
    {
        var bug = new Bug();
        bug.Assign();
        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void New_Defer_ShouldMoveToDeferred()
    {
        var bug = new Bug();
        bug.Defer();
        Assert.AreEqual(Bug.State.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void New_Reject_ShouldMoveToRejected()
    {
        var bug = new Bug();
        bug.Reject();
        Assert.AreEqual(Bug.State.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void Assigned_StartProgress_ShouldMoveToInProgress()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void InProgress_Resolve_ShouldMoveToResolved()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        Assert.AreEqual(Bug.State.Resolved, bug.CurrentState);
    }

    [TestMethod]
    public void Resolved_Verify_ShouldMoveToVerified()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        Assert.AreEqual(Bug.State.Verified, bug.CurrentState);
    }

    [TestMethod]
    public void Verified_Close_ShouldMoveToClosed()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Closed_ReopenFromClosed_ShouldMoveToReopened()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        bug.ReopenFromClosed();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Resolved_Reopen_ShouldMoveToReopened()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Reopened_StartProgress_ShouldMoveToInProgress()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Reopen();
        bug.StartProgress();
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Deferred_StartProgress_ShouldMoveToInProgress()
    {
        var bug = new Bug();
        bug.Defer();
        bug.StartProgress();
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Rejected_Reopen_ShouldMoveToReopened()
    {
        var bug = new Bug();
        bug.Reject();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void FullHappyPath_ShouldEndInReopenedAfterCloseReopen()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        bug.ReopenFromClosed();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void InvalidTransition_NewToResolve_ShouldThrowInvalidOperationException()
    {
        var bug = new Bug();
        Assert.ThrowsException<InvalidOperationException>(() => bug.Resolve());
    }

    [TestMethod]
    public void InvalidTransition_AssignedToVerify_ShouldThrowInvalidOperationException()
    {
        var bug = new Bug();
        bug.Assign();
        Assert.ThrowsException<InvalidOperationException>(() => bug.Verify());
    }

    [TestMethod]
    public void InvalidTransition_ClosedToClose_ShouldThrowInvalidOperationException()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        Assert.ThrowsException<InvalidOperationException>(() => bug.Close());
    }

    [TestMethod]
    public void InvalidTransition_RejectedToClose_ShouldThrowInvalidOperationException()
    {
        var bug = new Bug();
        bug.Reject();
        Assert.ThrowsException<InvalidOperationException>(() => bug.Close());
    }

    [TestMethod]
    public void InvalidTransition_ShouldNotChangeState()
    {
        var bug = new Bug();
        bug.Assign();

        try
        {
            bug.Verify();
        }
        catch (InvalidOperationException)
        {
            // Expected Stateless transition error.
        }

        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void TransitionHistory_ShouldContainAllExecutedTransitions()
    {
        var bug = new Bug();
        bug.Assign();
        bug.StartProgress();
        bug.Resolve();

        Assert.AreEqual(3, bug.TransitionHistory.Count);
        CollectionAssert.AreEqual(
            new[]
            {
                "New --Assign--> Assigned",
                "Assigned --StartProgress--> InProgress",
                "InProgress --Resolve--> Resolved"
            },
            bug.TransitionHistory.ToList());
    }

    [TestMethod]
    public void TransitionHistory_ShouldStayEmptyOnInitialization()
    {
        var bug = new Bug();
        Assert.AreEqual(0, bug.TransitionHistory.Count);
    }
}
