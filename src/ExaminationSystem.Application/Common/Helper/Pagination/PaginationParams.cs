using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Common.Helper.Pagination
{
    public record PaginationParams
    {
        private const int MaxPerPage = 100;
        private const int DefaultPage = 1;
        private const int DefaultPerPage = 20;

        private int _page = DefaultPage;
        private int _perPage = DefaultPerPage;

        public int Page
        {
            get => _page;
            init => _page = value < 1 ? DefaultPage : value;
        }

        public int PerPage
        {
            get => _perPage;
            init => _perPage = value < 1
                ? DefaultPerPage
                : value > MaxPerPage
                    ? MaxPerPage      
                    : value;
        }

        // Skip rows for SQL OFFSET
        public int Skip => (Page - 1) * PerPage;
    }
}

