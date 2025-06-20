using Microsoft.EntityFrameworkCore;

namespace DataAccess.Common
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }

    public class PagingDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
        public int TotalRecord { get; set; }

        public static PagingDTO Default = new PagingDTO()
        {
            PageNumber = 1,
            PageSize = 10
        };

        public void SetDefaultValueToPage()
        {
            if (PageNumber <= 0) PageNumber = 1;
            if (PageSize <= 0) PageSize = 10;
        }
    }

    public class PagingDataDTO<T>
    {
        public PagingDataDTO(IEnumerable<T> listData, dynamic pagingModel)
        {
            ListObjects = listData;
            Paging = pagingModel;
        }
        public dynamic Paging { get; set; }
        public IEnumerable<T> ListObjects { get; set; }
    }

    public class PagingDataModel<T1, T2> where T2 : PagingDTO
    {
        public PagingDataModel(IEnumerable<T1> listData, T2 pagingModel)
        {
            ListObjects = listData;
            Paging = pagingModel;
        }
        public T2 Paging { get; set; }
        public int TotalPages => (int)Math.Ceiling(Paging.TotalRecord * 1f / Paging.PageSize);
        public bool HasPreviousPage => Paging.PageNumber > 1;
        public bool HasNextPage => Paging.PageNumber < TotalPages;
        public IEnumerable<T1> ListObjects { get; set; }

    }
}
