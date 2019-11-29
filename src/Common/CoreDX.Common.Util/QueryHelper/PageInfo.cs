namespace CoreDX.Common.Util.QueryHelper
{
    public class PageInfo
    {
        public PageInfo(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public int PageNumber { get; }
        public int PageSize { get; }
    }
}
