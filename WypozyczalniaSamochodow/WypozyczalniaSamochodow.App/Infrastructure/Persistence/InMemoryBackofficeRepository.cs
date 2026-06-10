using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Infrastructure.Persistence;

internal sealed class InMemoryBackofficeRepository : InMemoryUserRepository<Backoffice>, IBackofficeRepository
{
}