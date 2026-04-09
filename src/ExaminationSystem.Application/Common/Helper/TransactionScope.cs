namespace ExaminationSystem.Application.Common.Helper;

public class TransactionScope
{
    private bool _isActive = false; // في البداية مفيش scope

    public bool IsRoot { get; private set; }

    public IDisposable Begin()
    {
        // أول حد بيكال Begin → _isActive = false → هو الـ Root
        // أي حد تاني → _isActive = true → مش Root
        IsRoot = !_isActive;
        _isActive = true;

        return new ScopeHandle(() =>
        {
            if (IsRoot)
                _isActive = false; // لما الـ Root يخلص بس امسح
        });
    }

    private sealed class ScopeHandle(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}
