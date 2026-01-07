namespace NetEvolve.ForgingBlazor.Routing;

public interface IPaginationBuilder
{
    IPaginationBuilder WithPageSize(int pageSize);
    IPaginationBuilder WithPaginationMode(PaginationMode paginationMode);
}
