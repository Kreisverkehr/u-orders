namespace UOrders.WebUI.Common;

public class DataPage<T>
{
    #region Public Constructors

    public DataPage(IEnumerable<T> data, int size, int index, int totalRecords, int totalPages)
    {
        PageData = data;
        Size = size;
        Index = index;
        TotalRecords = totalRecords;
        TotalPages = totalPages;
    }

    #endregion Public Constructors

    #region Public Properties

    public int Index { get; set; }

    public IEnumerable<T> PageData { get; set; }

    public int Size { get; set; }

    public int TotalPages { get; set; }

    public int TotalRecords { get; set; }

    #endregion Public Properties
}