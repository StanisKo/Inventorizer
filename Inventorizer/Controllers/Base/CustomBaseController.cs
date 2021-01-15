using Microsoft.AspNetCore.Mvc;

namespace Inventorizer.Controllers.Base
{
    public abstract class CustomBaseController : Controller
    {
        protected const int _PAGE_SIZE = 10;

        protected int _pageIndex;

        protected int _totalPages;

        protected bool _hasPreviousPage { get => _pageIndex > 1; }

        protected bool _hasNextPage { get => _pageIndex < _totalPages; }
    }
}