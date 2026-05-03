namespace AdaptiveSummaryGenerator.UI.Services;

// ─── AppState ─────────────────────────────────────────────────────────────────
// Shared state injected into components that need to communicate with each other.
// E.g. Footer toggle ↔ ThoughtProcessPanel, Navbar ↔ About modal.
public class AppState
{
    private bool _showThoughtProcess;

    public bool ShowThoughtProcess
    {
        get => _showThoughtProcess;
        set
        {
            _showThoughtProcess = value;
            OnThoughtProcessChanged?.Invoke();
        }
    }

    public event Action? OnThoughtProcessChanged;
}
