using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.StarterCode.Services
{
    public interface IMultitenantService
    {
        Task<TokenResponse> ClientIdClientSecretToGenerateToken();
        Task<TokenResponse> GetGeneratedToken();
    }
}
