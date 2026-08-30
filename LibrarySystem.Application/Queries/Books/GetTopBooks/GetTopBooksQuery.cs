using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Application.Queries.Books.GetTopBooks
{
    public sealed record GetTopBooksQuery: IRequest<IReadOnlyList<TopBooksDto>>;
}
