using System;

namespace Chilite.ViewModels
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}