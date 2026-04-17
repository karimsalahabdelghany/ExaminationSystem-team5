using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Interfaces
{
    public interface IQuery : IRequest
    {
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}
