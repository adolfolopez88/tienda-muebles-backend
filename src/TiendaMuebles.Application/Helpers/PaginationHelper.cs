namespace TiendaMuebles.Application.Helpers;

public static class PaginationHelper
{
    public static (int skip, int take) GetOffset(int page, int limit)
    {
        var safePage = Math.Max(1, page);
        var safeLimit = Math.Clamp(limit, 1, 100);
        return ((safePage - 1) * safeLimit, safeLimit);
    }

    public static DTOs.PaginationInfo GetInfo(int page, int limit, int total) =>
        new(page, limit, total, (int)Math.Ceiling((double)total / Math.Max(1, limit)));
}
