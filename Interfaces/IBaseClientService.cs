using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Interfaces
{
    public interface IBaseClientService
    {
        Task<T> CallService<T>(string url, HttpMethod method, object requestBody = null, string token = null, object parameters = null);
    }
}
