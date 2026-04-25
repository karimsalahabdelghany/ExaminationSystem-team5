using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Interfaces
{
    public interface ICurrentUser
    {
         Guid? Id { get;}
        string? Email { get; }
        string? Role { get; }
        bool IsInRole(string role);
        bool IsAuthenticated { get; }
    }

}
