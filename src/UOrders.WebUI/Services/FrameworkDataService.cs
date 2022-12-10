using System.ComponentModel;

namespace UOrders.WebUI.Services;

public class FrameworkDataService
{
    #region Private Fields

    private bool _currentPageIsLoading = false;

    #endregion Private Fields

    #region Public Events

    public event EventHandler? BreadCrumbsHasChanged;

    public event EventHandler? CurrentPageIsLoadingHasChanged;

    #endregion Public Events

    #region Public Properties

    public IList<Tuple<string, string>> Crumbs { get; private set; } = new BindingList<Tuple<string, string>>();

    public bool CurrentPageIsLoading
    {
        get
        {
            return _currentPageIsLoading;
        }
        set
        {
            _currentPageIsLoading = value;
            OnCurrentPageIsLoadingHasChanged();
        }
    }
    public FrameworkDataService()
    {
        if(Crumbs is BindingList<Tuple<string, string>> crumbsBinding)
            crumbsBinding.ListChanged += (caller, args) => OnBreadCrumbsHasChanged();
        
    }
    #endregion Public Properties

    #region Public Methods

    protected virtual void OnBreadCrumbsHasChanged() => BreadCrumbsHasChanged?.Invoke(this, new EventArgs());

    #endregion Public Methods

    #region Protected Methods

    protected virtual void OnCurrentPageIsLoadingHasChanged() => CurrentPageIsLoadingHasChanged?.Invoke(this, new EventArgs());

    #endregion Protected Methods
}