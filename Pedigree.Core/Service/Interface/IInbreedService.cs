using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IInbreedService
    {
        Task RemoveUnnecessaryInbreeds();
    }
}
