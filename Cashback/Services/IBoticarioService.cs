using System.Threading.Tasks;

namespace Cashback.Service
{
    public interface IBoticarioService
    {
        Task<decimal?> Cashback(string cpf);
    }
}