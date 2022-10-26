namespace APRP.Services.AuthorityAPI.Wrappers
{
    public class PagedResponse<T>:Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri? FirstPage { get; set; }
        public Uri? LastPage { get; set; }
        public int? TotalPages { get; set; }
        public int? TotalRecords { get; set; }
        public Uri? NextPage { get; set; }
        public Uri? PreviousPage { get; set; }

        public PagedResponse(T Data,int pageNumber, int pageSize 
            /*,Uri firstPage, Uri lastPage, int totalPages, int totalRecords, Uri nextPage,Uri previousPage*/
            )
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            //FirstPage = firstPage;
            //LastPage = lastPage;
            //TotalPages = totalPages;
            //TotalRecords = totalRecords;
            //NextPage = nextPage;
            //PreviousPage = previousPage;
            this.Data = Data;
            this.Message = null;
            this.Succeeded = false;
            this.Errors = null;
        }
    }
}
