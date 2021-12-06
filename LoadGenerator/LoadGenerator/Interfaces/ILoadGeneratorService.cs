using System.Threading.Tasks;
using LoadGenerator.Models.Internal;

namespace LoadGenerator.Interfaces
{
    public interface ILoadGeneratorService
    {
        ValueTask<LoadGeneratorModel[]> Run();
    }
}
